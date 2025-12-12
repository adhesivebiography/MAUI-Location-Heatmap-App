using SQLite;
using MauiApp1.Models;

namespace MauiApp1.Data
{
    public class LocationDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public LocationDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<LocationEntry>().Wait();
        }

        // Save a new location entry
        public Task<int> SaveLocationAsync(LocationEntry entry)
        {
            return _database.InsertAsync(entry);
        }

        // Retrieve all saved locations
        public Task<List<LocationEntry>> GetLocationsAsync()
        {
            return _database.Table<LocationEntry>().ToListAsync();
        }

        // Delete all locations (optional helper)
        public Task<int> DeleteAllAsync()
        {
            return _database.DeleteAllAsync<LocationEntry>();
        }
    }
}
