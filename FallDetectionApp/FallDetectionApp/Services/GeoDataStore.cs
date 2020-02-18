using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FallDetectionApp.Models;

namespace FallDetectionApp.Services
{

    //THIS CLASS IS NOT USED ATM!!!!!!!!!!!!!!!!!!
    //
    //
    public class GeoDataStore
    {
        readonly List<GeoLocation> locations;

        public GeoDataStore()
        {
            locations = new List<GeoLocation>()
            {
                //new Location { Id = Guid.NewGuid().ToString(),Altitude=, Latitude, Longitude,},

            };
        }

        public async Task<bool> AddItemAsync(GeoLocation location)
        {
            locations.Add(location);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(GeoLocation location)
        {
            var oldLocation = locations.Where((GeoLocation arg) => arg.Id == location.Id).FirstOrDefault();
            locations.Remove(oldLocation);
            locations.Add(location);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldLocation = locations.Where((GeoLocation arg) => arg.Id == id).FirstOrDefault();
            locations.Remove(oldLocation);

            return await Task.FromResult(true);
        }

        public async Task<GeoLocation> GetItemAsync(string id)
        {
            return await Task.FromResult(locations.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<GeoLocation>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(locations);
        }
    }
}