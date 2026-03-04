using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConcurrencyDemo.Infrastructure.Services
{
    public class ConcurrencyConflictHandler
    {
        private readonly ILogger<ConcurrencyConflictHandler> _logger;

        public ConcurrencyConflictHandler(ILogger<ConcurrencyConflictHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(
            DbUpdateConcurrencyException exception,
            CancellationToken cancellationToken = default)
        {
            foreach (var entry in exception.Entries)
            {
                var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);

                if (databaseValues == null)
                {
                    _logger.LogWarning("Entity was deleted by another user.");
                    entry.State = EntityState.Detached;
                    continue;
                }

                _logger.LogWarning(
                    "Original: {Original}; Current: {Current}; Database: {Database}",
                    entry.OriginalValues.ToObject(),
                    entry.CurrentValues.ToObject(),
                    databaseValues.ToObject());

                entry.OriginalValues.SetValues(databaseValues);
                entry.CurrentValues.SetValues(databaseValues);
                entry.State = EntityState.Unchanged;
            }
        }
    }
}
