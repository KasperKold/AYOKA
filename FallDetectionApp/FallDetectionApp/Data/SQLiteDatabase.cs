using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using FallDetectionApp.Models;

namespace FallDetectionApp.Data
{
    public class SQLiteDatabase
    {
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public SQLiteDatabase()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        async Task InitializeAsync()
        {

            await Database.DropTableAsync<GeoLocation>();
            await Database.CreateTableAsync<GeoLocation>().ConfigureAwait(false);

            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(Contact).Name))
                {
                    //await Database.CreateTableAsync(CreateFlags.None, typeof(Contact)).ConfigureAwait(false);
                    await Database.CreateTableAsync<Contact>().ConfigureAwait(false);

                    initialized = true;
                }
            }
        }

        public Task<List<Contact>> GetItemsAsync()
        {
            return Database.Table<Contact>().ToListAsync();
        }

        public Task<List<GeoLocation>> GetGeoLocationItemsAsync()
        {
            return Database.Table<GeoLocation>().ToListAsync();
        }

        public Task<List<Contact>> GetAllContactsInDatabase()
        {
            return Database.QueryAsync<Contact>("SELECT * FROM [Contact] "); // WHERE[Done] = 0
        }

        public Task<Contact> GetItemAsync(int id)
        {
            return Database.Table<Contact>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(Contact item)
        {
            if (item.ID != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(Contact item)
        {
            return Database.DeleteAsync(item);
        }

        // For the new GeoLocation table
        public Task<int> SaveGeoLocationItemAsync(GeoLocation item)
        {
            if (item.Id != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        public Task<int> DeleteGeoLocationItemAsync(GeoLocation item)
        {
            return Database.DeleteAsync(item);
        }

    }
}

