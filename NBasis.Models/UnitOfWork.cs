using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NBasis.Models
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly Dictionary<WorkPosition, List<Func<Task>>> _work = new Dictionary<WorkPosition, List<Func<Task>>>();

        public void AddWork(Action action, WorkPosition position = WorkPosition.AfterTransaction)
        {
            if (!_work.ContainsKey(position))
                _work[position] = new List<Func<Task>>();
            else
            {
                if (position == WorkPosition.Transaction)
                    throw new TransactionAlreadyAddedException("Transaction already added to the unit of work");
            }

            _work[position].Add(() =>
            {
                action();
                return Task.FromResult(0);
            });
        }

        public void AddWork(Func<Task> action, WorkPosition position = WorkPosition.AfterTransaction)
        {
            if (!_work.ContainsKey(position))
                _work[position] = new List<Func<Task>>();
            else
            {
                if (position == WorkPosition.Transaction)
                    throw new TransactionAlreadyAddedException("Transaction already added to the unit of work");
            }

            _work[position].Add(action);
        }

        public async Task CompleteAsync()
        {
            // execute each position
            if (_work.ContainsKey(WorkPosition.BeforeTransaction)) {
                var taskList = new List<Task>();
                _work[WorkPosition.BeforeTransaction].ForEach(w =>
                {
                    taskList.Add(w.Invoke());
                });
                await Task.WhenAll(taskList.ToArray());
            }

            if (_work.ContainsKey(WorkPosition.Transaction))
            {
                var taskList = new List<Task>();
                _work[WorkPosition.Transaction].ForEach(w =>
                {
                    taskList.Add(w.Invoke());
                });
                await Task.WhenAll(taskList.ToArray());
            }
            if (_work.ContainsKey(WorkPosition.AfterTransaction))
            {
                var taskList = new List<Task>();
                _work[WorkPosition.AfterTransaction].ForEach(w =>
                {
                    taskList.Add(w.Invoke());
                });
                await Task.WhenAll(taskList.ToArray());
            }
        }

        public void Rollback()
        {
            // clear all the work
            _work.Clear();
        }
    }
}
