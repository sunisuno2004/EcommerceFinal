using ECommerce.Business.Repositories;
using ECommerce.DataAccess.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerce.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ECommerceDbContext _context;

        private IDbContextTransaction? _transaction;

        public UnitOfWork(
            ECommerceDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction is not null)
            {
                return;
            }

            _transaction =
                await _context.Database
                    .BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction is null)
            {
                return;
            }

            await _transaction.CommitAsync();

            await _transaction.DisposeAsync();

            _transaction = null;
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction is null)
            {
                return;
            }

            await _transaction.RollbackAsync();

            await _transaction.DisposeAsync();

            _transaction = null;
        }
    }
}