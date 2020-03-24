using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FallDetectionApp.Models;

namespace FallDetectionApp.Services
{
    public interface IUiTrigger
    {
        Task<bool> Trigger();
        //Task<bool> syncGeo();
        //Task<bool> DeleteItemAsync(string id);
        //Task<T> GetItemAsync(string id);
        //Task<IEnumerable<T>> GetItemsAsync(bool forceRefresh = false);
    }
}
