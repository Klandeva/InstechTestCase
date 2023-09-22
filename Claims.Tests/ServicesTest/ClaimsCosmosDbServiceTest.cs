using Claims.BLL.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Tests.ServicesTest
{
    public class ClaimsCosmosDbServiceTest : ICosmosDbService<Claim>
    {
        private readonly List<Claim> _claims = new List<Claim>();

        public ClaimsCosmosDbServiceTest() 
        { 
            _claims = new List<Claim>() 
            { 
                new Claim() { Id = "1234", CoverId = "4321", Name = "Claim1", Created = DateTime.Now, DamageCost = 100, Type = ClaimType.Collision },
                new Claim() { Id = "4567", CoverId = "7654", Name = "Claim2", Created = DateTime.Now, DamageCost = 200, Type = ClaimType.Fire },
                new Claim() { Id = "8901", CoverId = "1098", Name = "Claim3", Created = DateTime.Now, DamageCost = 300, Type = ClaimType.Grounding }
            };
        
        }
        public async Task AddItemAsync(Claim item)
        {
            _claims.Add(item);
        }

        public async Task DeleteItemAsync(string id)
        {
            Claim claim = _claims.Where(c => c.Id == id).First();
            _claims.Remove(claim);
        }

        public async Task<Claim> GetItemAsync(string id)
        {
            return _claims.Where(c => c.Id == id).First();
        }

        public async Task<IEnumerable<Claim>> GetItemsAsync()
        {
            return _claims;
        }
    }
}
