using System;
using System.Threading.Tasks;

namespace SibersTest.Application.Interfaces
{
    /// <summary>
    /// Unit of Work pattern interface for managing database transactions.
    /// Ensures all operations within a transaction are committed atomically,
    /// preventing partial data writes if any step fails.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        Task BeginTransactionAsync(CancellationToken ct = default);

        /// <summary>
        /// Commits all changes made within the current transaction to the database.
        /// </summary>
        Task CommitAsync(CancellationToken ct = default);

        /// <summary>
        /// Rolls back all changes made within the current transaction.
        /// </summary>
        Task RollbackAsync(CancellationToken ct = default);

        /// <summary>
        /// Saves all pending changes to the database without committing a transaction.
        /// Used when transaction is managed externally.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}