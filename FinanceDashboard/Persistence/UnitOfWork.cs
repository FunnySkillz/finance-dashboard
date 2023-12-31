﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;


namespace Persistence
{
    public class UnitOfWork
    {
        const string FILENAME = "movies.csv";

        private readonly ApplicationDbContext _dbContext = new ApplicationDbContext();

        public UnitOfWork() : this(new ApplicationDbContext())
        { }

        private UnitOfWork(ApplicationDbContext context)
        {
            _dbContext = new ApplicationDbContext();

        }

        public UnitOfWork(IConfiguration configuration) : this(new ApplicationDbContext(configuration))
        { }
 

        public async Task<int> SaveChangesAsync()
        {
            var entities = _dbContext!.ChangeTracker.Entries()
                .Where(entity => entity.State == EntityState.Added
                                 || entity.State == EntityState.Modified)
                .Select(e => e.Entity)
                .ToArray();  // Geänderte Entities ermitteln

            // Allfällige Validierungen der geänderten Entities durchführen
            foreach (var entity in entities)
            {
                await ValidateEntityAsync(entity);
            }
            return await _dbContext.SaveChangesAsync();

        }

        private async Task ValidateEntityAsync(object entity)
        {

        }

        public async Task DeleteDatabaseAsync() => await _dbContext!.Database.EnsureDeletedAsync();
        public async Task MigrateDatabaseAsync() => await _dbContext!.Database.MigrateAsync();
        public async Task CreateDatabaseAsync() => await _dbContext!.Database.EnsureCreatedAsync();

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (disposing)
            {
                await _dbContext.DisposeAsync();
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        //public async Task FillDbAsync()
        //{
        //    await this.DeleteDatabaseAsync();
        //    await this.MigrateDatabaseAsync();

        //    string[][] csvMovies = await MyFile.ReadStringMatrixFromCsvAsync(FILENAME,true);

        //    List<Category> categories = csvMovies.GroupBy(line => line[2]).Select(grp => new Category() { CategoryName = grp.Key }).ToList();
        //    List<Movie> movies = csvMovies.Select(line =>
        //      new Movie()
        //      {
        //          Category = categories.Single(cat => cat.CategoryName == line[2]),
        //          Duration = int.Parse(line[3]),
        //          Title = line[0],
        //          Year = int.Parse(line[1]),
        //      }).ToList();

            
        //    await _dbContext.Categories.AddRangeAsync(categories);
        //    await _dbContext.Movies.AddRangeAsync(movies);
        //    await SaveChangesAsync();
        //}
    }

   
}
