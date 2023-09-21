using Claims.BLL.Services.IServices;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Claims.BLL.Services
{
    public class CosmosDbService<T> : ICosmosDbService<T>
    {
        private readonly Container _container;

        public CosmosDbService(CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            if (dbClient == null) throw new ArgumentNullException(nameof(dbClient));
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<IEnumerable<T>> GetItemsAsync()
        {
            var query = _container.GetItemQueryIterator<T>(new QueryDefinition("SELECT * FROM c"));
            var results = new List<T>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<T> GetItemAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<T>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }
        }

        public Task AddItemAsync(T item)
        {
            PropertyInfo propertyInfo = item.GetType().GetProperty("Id");
            return _container.CreateItemAsync(item, new PartitionKey(propertyInfo.GetValue(item).ToString()));
        }

        public Task DeleteItemAsync(string id)
        {
            return _container.DeleteItemAsync<Claim>(id, new PartitionKey(id));
        }

    }
}
