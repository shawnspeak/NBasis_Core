using NBasis.Container;
using NBasis.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NBasis.Commanding
{
    public class LocalCommandBusFactory : ICommandBusFactory
    {
        private IDictionary<Type, CommandHandlerInvoker> commandInvokers;
        private static object _lock = new object();

        public LocalCommandBusFactory(ITypeFinder typeFinder)
        {
            BuildCommandInvokers(typeFinder);
        }

        public ICommandBus GetCommandBus(IContainer container)
        {
            return new CommandBus(container, this);
        }

        private class CommandBus : ICommandBus
        {
            readonly IContainer _container;
            readonly LocalCommandBusFactory _Factory;
            readonly CommandCleaner _cleaner;

            public CommandBus(IContainer container, LocalCommandBusFactory factory)
            {
                _container = container;
                _Factory = factory;
                _cleaner = new CommandCleaner();
            }

            public Task SendAsync(IEnumerable<Envelope<ICommand>> commands)
            {
                List<Task> tasks = new List<Task>();
                commands.ForEach((command) =>
                {
                    var body = _cleaner.Clean(command.Body);

                    var commandHandler = _Factory.GetTheCommandHandler(body);
                    if (commandHandler == null) return;

                    tasks.Add(commandHandler.SendAsync(_container, body, command.Headers, command.CorrelationId));
                });
                return Task.WhenAll(tasks);
            }

            public Task<TResult> AskAsync<TResult>(Envelope<ICommand> command)
            {
                var body = _cleaner.Clean(command.Body);
                var commandHandler = _Factory.GetTheCommandHandler(body);
                if (commandHandler == null) return Task.FromResult<TResult>(default(TResult));
                return commandHandler.AskAsync<TResult>(_container, body, command.Headers, command.CorrelationId);
            }
        }

        private void BuildCommandInvokers(ITypeFinder typeFinder)
        {
            if (commandInvokers == null)
            {
                lock (_lock)
                {
                    if (commandInvokers == null)
                    {
                        commandInvokers = new Dictionary<Type, CommandHandlerInvoker>();
                        foreach (var commandHandlerType in typeFinder.GetInterfaceImplementations<IHandleCommands>())
                        {
                            foreach (var commandType in GetCommandTypesForCommandHandler(commandHandlerType))
                            {
                                if (commandInvokers.ContainsKey(commandType))
                                    throw new DuplicateCommandHandlersException(commandType);

                                commandInvokers.Add(commandType, new CommandHandlerInvoker(commandType, commandHandlerType));
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<Type> GetCommandTypesForCommandHandler(Type commandHandlerType)
        {
            return (from interfaceType in commandHandlerType.GetTypeInfo().ImplementedInterfaces
                    where interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandleCommands<>)
                    select interfaceType.GetTypeInfo().GenericTypeArguments[0]).ToArray();
        }

        private CommandHandlerInvoker GetTheCommandHandler(ICommand command)
        {
            if (!commandInvokers.TryGetValue(command.GetType(), out CommandHandlerInvoker commandInvoker))
                throw new CommandHandlerNotFoundException(command.GetType());
            return commandInvoker;
        }

        public class CommandHandlerInvoker
        {
            readonly Type commandHandlerType;
            readonly Type commandType;

            public CommandHandlerInvoker(Type commandType, Type commandHandlerType)
            {
                this.commandType = commandType;
                this.commandHandlerType = commandHandlerType;
            }

            public Task SendAsync(IContainer container, ICommand command, IDictionary<string, object> headers, string correlationId)
            {
                var handlingContext = CreateTheCommandHandlingContext(command, headers, correlationId);
                return ExecuteTheCommandHandlerAsync(container, handlingContext);
            }

            public async Task<TResult> AskAsync<TResult>(IContainer container, ICommand command, IDictionary<string, object> headers, string correlationId)
            {
                var handlingContext = CreateTheCommandHandlingContext(command, headers, correlationId);
                await ExecuteTheCommandHandlerAsync(container, handlingContext);
                return handlingContext.GetReturn<TResult>();
            }

            private async Task ExecuteTheCommandHandlerAsync(IContainer container, ICommandHandlingContext<ICommand> handlingContext)
            {
                var handleMethod = GetTheHandleMethod();
                var commandHandler = container.Resolve(commandHandlerType);

                try
                {
                    await (Task)handleMethod.Invoke(commandHandler, new object[] { handlingContext });
                    return;
                }
                catch (TargetInvocationException ex)
                {
                    throw new CommandHandlerInvocationException(
                        string.Format("Command handler '{0}' for '{1}' failed. Inspect inner exception.", commandHandler.GetType().Name, handlingContext.Command.GetType().Name),
                        ex.InnerException)
                        {
                            HandlingContext = handlingContext
                        };
                }
            }

            private ICommandHandlingContext<ICommand> CreateTheCommandHandlingContext(ICommand command, IDictionary<string, object> headers, string correlationId)
            {
                var handlingContextType = typeof(CommandHandlingContext<>).MakeGenericType(commandType);
                return (ICommandHandlingContext<ICommand>)Activator.CreateInstance(handlingContextType, command, headers, correlationId);
            }

            private MethodInfo GetTheHandleMethod()
            {
                return typeof(IHandleCommands<>).MakeGenericType(commandType).GetTypeInfo().GetDeclaredMethod("Handle");
            }
        }

        private class CommandHandlingContext<TCommand> : ICommandHandlingContext<TCommand>
            where TCommand : ICommand
        {
            public CommandHandlingContext(TCommand command, IDictionary<string, object> headers, string correlationId)
            {
                Command = command;
                Headers = headers;
                CorrelationId = correlationId;
            }

            public IDictionary<string, object> Headers { get; private set; }

            public TCommand Command { get; private set; }

            public string CorrelationId { get; private set; }
            
            private object _returnValue;
            public T GetReturn<T>()
            {
                return (T)_returnValue;
            }

            public void SetReturn(object value)
            {
                _returnValue = value;
            }
        }
    }
}
