using Domain.Interface;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Services
{
    public class NonqueryDataService<T> where T : class
    {
        private readonly ApplicationContextFactory _contextFactory;
        //private readonly NonqueryDataService<T> _nonQueryDataService;

        public NonqueryDataService(ApplicationContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<T> Create(T entity)
        {
            using ApplicationDBContext context = _contextFactory.CreateDbContext();
            EntityEntry<T> createdResult = await context.Set<T>().AddAsync(entity);
            OnBeforeSaving(context);
            await context.SaveChangesAsync();
            return createdResult.Entity;
        }

        public async Task<bool> CreateRange(List<T> entity)
        {
            using ApplicationDBContext context = _contextFactory.CreateDbContext();
            await context.Set<T>().AddRangeAsync(entity);
            OnBeforeSaving(context);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(T entity)
        {
            using ApplicationDBContext context = _contextFactory.CreateDbContext();
            context.Set<T>().Remove(entity);
            OnBeforeSaving(context);
            await context.SaveChangesAsync();
            return true;
        }


        public async Task<T> Update(T entity)
        {
            using ApplicationDBContext context = _contextFactory.CreateDbContext();
            context.Set<T>().Update(entity);
            OnBeforeSaving(context);
            await context.SaveChangesAsync();
            return entity;
        }

        private void OnBeforeSaving(DbContext dbContext)
        {
            var changedEntries = dbContext.ChangeTracker
                .Entries()
                .Where(e =>
                    (e.Entity is ITrackCreated || e.Entity is ISoftDelete || e.Entity is ITrackUpdated) &&
                    (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

            foreach (var entry in changedEntries)
            {
                if(entry.State == EntityState.Added) 
                    TrackAdded(entry);

                if(entry.State == EntityState.Modified)
                    TrackUpdated(entry);

                if(entry.State == EntityState.Deleted)
                    TrackDeleted(entry);
            }
        }

        private static void TrackUpdated(EntityEntry entry)
        {
            if (entry.Entity is ITrackUpdated updated)
            {
                if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    updated.UpdatedBy = -1;
                    updated.UpdatedOn = DateValidatorHelper.ToIndiaDateTime(DateTime.UtcNow);
                }
            }
        }

        private static void TrackDeleted(EntityEntry entry)
        {
            if (entry.Entity is ISoftDelete deleted)
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.CurrentValues["UpdatedOn"] = DateValidatorHelper.ToIndiaDateTime(DateTime.UtcNow);
                    deleted.IsDeleted = true;
                }
            }
        }

        private void TrackAdded(EntityEntry entry)
        {
            if (entry.Entity is ITrackCreated added)
            {
                if (entry.State == EntityState.Added && added.CreatedOn == default)
                {
                    added.CreatedBy = -1;
                    added.CreatedOn = DateValidatorHelper.ToIndiaDateTime(DateTime.UtcNow);
                }
            }
        }
    }
}