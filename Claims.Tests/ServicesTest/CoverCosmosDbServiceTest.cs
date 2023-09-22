using Claims.BLL.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Tests.ServicesTest
{
    internal class CoverCosmosDbServiceTest : ICosmosDbService<Cover>
    {
        private readonly List<Cover> _covers;

        public CoverCosmosDbServiceTest() 
        {
            _covers = new List<Cover>()
            {
                new Cover() { Id = "4321", StartDate = new DateOnly(2023,1,1), EndDate = new DateOnly(2023,12,31), Premium = 101, Type = CoverType.Yacht },
                new Cover() { Id = "7654", StartDate = new DateOnly(2023,1,1), EndDate = new DateOnly(2023,12,31), Premium = 102, Type = CoverType.PassengerShip },
                new Cover() { Id = "1098", StartDate = new DateOnly(2023,1,1), EndDate = new DateOnly(2023,12,31), Premium = 103, Type = CoverType.Tanker },
                new Cover() { Id = "7531", StartDate = new DateOnly(2023,1,1), EndDate = new DateOnly(2023,12,31), Premium = 104, Type = CoverType.BulkCarrier }

            };
        }
        public async Task AddItemAsync(Cover item)
        {
            _covers.Add(item);
        }

        public async Task DeleteItemAsync(string id)
        {
            Cover cover = _covers.Where(x => x.Id == id).First();
            _covers.Remove(cover);
        }

        public async Task<Cover> GetItemAsync(string id)
        {
            return _covers.Where(x => x.Id == id).First();
        }

        public async Task<IEnumerable<Cover>> GetItemsAsync()
        {
            return _covers;
        }
    }
}
