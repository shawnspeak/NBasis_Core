using NBasis.Container;
using NBasis.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NBasis.Querying
{
    public class LocalQueryProcessorFactory : IQueryProcessorFactory
    {
        private IDictionary<Type, QueryHandlerInvoker> _queryInvokers;
        private static object _lock = new object();

        public LocalQueryProcessorFactory(ITypeFinder typeFinder)
        {
            BuildQueryInvokers(typeFinder);
        }

        public IQueryProcessor GetQueryProcessor(IContainer container)
        {
            return new QueryProcessor(container, this);
        }

        private class QueryProcessor : IQueryProcessor
        {
            readonly IContainer _container;
            readonly LocalQueryProcessorFactory _factory;

            public QueryProcessor(IContainer container, LocalQueryProcessorFactory factory)
            {
                _container = container;
                _factory = factory;
            }

            public Task<TResult> ExecuteAsync<TQuery, TResult>(Envelope<TQuery> query)
                where TQuery : IQuery<TResult>
            {
                // exception if not found
                var queryHandler = _factory.GetTheQueryHandler(query.Body);
                return queryHandler.Execute<TQuery, TResult>(_container, query.Body, query.Headers, query.CorrelationId);
            }
        }

        private void BuildQueryInvokers(ITypeFinder typeFinder)
        {
            if (_queryInvokers == null)
            {
                lock (_lock)
                {
                    if (_queryInvokers == null)
                    {
                        _queryInvokers = new Dictionary<Type, QueryHandlerInvoker>();
                        foreach (var queryHandlerType in typeFinder.GetInterfaceImplementations<IHandleQueries>())
                        {
                            foreach (var queryType in GetQueryTypesForCommandHandler(queryHandlerType))
                            {
                                if (_queryInvokers.ContainsKey(queryType))
                                    throw new DuplicateQueryHandlersException(queryType);

                                var resultType = GetResultTypeForQuery(queryType);
                                _queryInvokers.Add(queryType, new QueryHandlerInvoker(queryType, resultType, queryHandlerType));
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<Type> GetQueryTypesForCommandHandler(Type queryHandlerType)
        {
            return (from interfaceType in queryHandlerType.GetTypeInfo().ImplementedInterfaces
                    where interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandleQueries<,>)
                    select interfaceType.GetTypeInfo().GenericTypeArguments[0]).ToArray();
        }

        private static Type GetResultTypeForQuery(Type queryType)
        {
            return (from interfaceType in queryType.GetTypeInfo().ImplementedInterfaces
                    where interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IQuery<>)
                    select interfaceType.GetTypeInfo().GenericTypeArguments[0]).First();
        }

        private QueryHandlerInvoker GetTheQueryHandler<TQuery>(TQuery query)
        {
            if (!_queryInvokers.TryGetValue(query.GetType(), out QueryHandlerInvoker queryInvoker))
                throw new QueryHandlerNotFoundException(query.GetType());
            return queryInvoker;
        }

        public class QueryHandlerInvoker
        {
            readonly Type _queryHandlerType;
            readonly Type _queryType;
            readonly Type _resultType;

            public QueryHandlerInvoker(Type queryType, Type resultType, Type queryHandlerType)
            {
                this._queryType = queryType;
                this._resultType = resultType;
                this._queryHandlerType = queryHandlerType;
            }

            public Task<TResult> Execute<TQuery, TResult>(IContainer container, TQuery command, IDictionary<string, object> headers, string correlationId)
                where TQuery : IQuery<TResult>
            {
                var handlingContext = CreateTheHandlingContext<TQuery, TResult>(command, headers, correlationId);
                var handleMethod = GetTheHandleMethod();
                var queryHandler = container.Resolve(_queryHandlerType);

                try
                {
                    return (Task<TResult>)handleMethod.Invoke(queryHandler, new object[] { handlingContext });
                }
                catch (TargetInvocationException ex)
                {
                    throw new QueryHandlerInvocationException(
                        string.Format("Query handler '{0}' for '{1}' failed. Inspect inner exception.", queryHandler.GetType().Name, handlingContext.Query.GetType().Name),
                        ex.InnerException);
                }
            }
            

            private IQueryHandlingContext<TQuery,TResult> CreateTheHandlingContext<TQuery, TResult>(TQuery query, IDictionary<string, object> headers, string correlationId)
                where TQuery : IQuery<TResult>
            {
                var handlingContextType = typeof(QueryHandlingContext<,>).MakeGenericType(_queryType, _resultType);
                return (IQueryHandlingContext<TQuery, TResult>)Activator.CreateInstance(handlingContextType, query, headers, correlationId);
            }

            private MethodInfo GetTheHandleMethod()
            {
                return typeof(IHandleQueries<,>).MakeGenericType(_queryType, _resultType).GetTypeInfo().GetDeclaredMethod("Handle");
            }
        }

        private class QueryHandlingContext<TQuery, TResult> : IQueryHandlingContext<TQuery, TResult>
            where TQuery : IQuery<TResult>
        {
            public QueryHandlingContext(TQuery query, IDictionary<string, object> headers, string correlationId)
            {
                Query = query;
                Headers = headers;
                CorrelationId = correlationId;
            }

            public IDictionary<string, object> Headers { get; private set; }

            public TQuery Query { get; private set; }

            public string CorrelationId { get; private set; }
        }
    }
}
