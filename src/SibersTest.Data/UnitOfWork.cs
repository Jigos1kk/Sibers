using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SibersTest.Application.Interfaces;

namespace SibersTest.Data
{
    /// <summary>
    /// Unit of Work implementation for managing database transactions.
    /// Ensures atomicity of operations - all changes are committed together
    /// or rolled back if any operation fails.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            try
            {
                await _context.SaveChangesAsync(ct);
                if (_transaction != null)
                {
                    await _transaction.CommitAsync(ct);
                }
            }
            catch
            {
                if (_transaction != null)
                {
                    var transaction = _transaction;
                    _transaction = null;
                    try
                    {
                        await transaction.DisposeAsync();
                    }
                    catch
                    {
                        // Ignore dispose errors to prevent masking the original exception
                    }
                }
                throw;
            }
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_transaction != null)
            {
                try
                {
                    await _transaction.RollbackAsync(ct);
                }
                catch (InvalidOperationException)
                {
                    // Transaction may have already been completed (rolled back by EF Core or committed).
                }
                finally
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }
}