using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FallDetectionApp.Services
{
    public interface IGeoLocation
    {
        Task<bool> GetGeoLocationAsync();
        //Task<bool> UpdateItemAsync(T item);
        //Task<bool> DeleteItemAsync(string id);
        //Task<T> GetItemAsync(string id);
        //Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}
