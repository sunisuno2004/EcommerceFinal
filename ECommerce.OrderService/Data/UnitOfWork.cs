using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerce.OrderService.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OrderDbContext _context;

        private IDbContextTransaction? _transaction;

        public UnitOfWork(
            OrderDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
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