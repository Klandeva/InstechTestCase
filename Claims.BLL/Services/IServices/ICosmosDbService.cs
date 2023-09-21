using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Claims.BLL.Services.IServices
{
    public interface ICosmosDbService<T> where T : class, new()
    {
        Task<IEnumerable<T>> GetItemsAsync();
        Task<T> GetItemAsync(string id);
        Task AddItemAsync(T item);
        Task DeleteItemAsync(string id);
    }
}
