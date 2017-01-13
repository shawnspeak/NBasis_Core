using System;
using System.Threading.Tasks;

namespace NBasis.Models
{
    public enum WorkPosition
    {
        Transaction, // can only be set once
        BeforeTransaction,
        AfterTransaction
    }

    public interface IUnitOfWork
    {
        void AddWork(Func<Task> action, WorkPosition position = WorkPosition.AfterTransaction);

        void AddWork(Action action, WorkPosition position = WorkPosition.AfterTransaction);

        Task CompleteAsync();

        void Rollback();
    }
}
