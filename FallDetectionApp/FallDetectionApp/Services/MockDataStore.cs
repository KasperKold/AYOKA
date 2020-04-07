using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FallDetectionApp.Models;

namespace FallDetectionApp.Services
{/*
    public class MockDataStore : IDataStore<Item>
    {
        readonly List<Item> items;

        public MockDataStore()
        {
            items = new List<Item>()
            {
                new Item { Id = Guid.NewGuid().ToString(),Prio=1, Text = "Kim Hammel", Description= "Mother", PhoneNr="+41733111111"},
                new Item { Id = Guid.NewGuid().ToString(),Prio=2,Text = "Christian List", Description="Father" , PhoneNr="+42733222222"  },
                new Item { Id = Guid.NewGuid().ToString(),Prio=3, Text = "Peder Nilsson", Description="Close colleague", PhoneNr="+43733333333" },
                new Item { Id = Guid.NewGuid().ToString(),Prio=4,Text = "Kasper Kold", Description="Close friend", PhoneNr="+44733444444" },
                new Item { Id = Guid.NewGuid().ToString(),Prio=5,Text = "Clark Kent", Description="My savior", PhoneNr="+45733555555" },
                new Item { Id = Guid.NewGuid().ToString(),Prio=6,Text = "Leonardo", Description="My favourite Ninja Turtle", PhoneNr="+46733666666" }
            };
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
    */
}