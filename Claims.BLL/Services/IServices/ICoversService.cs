using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.BLL.Services.IServices
{
    public interface ICoversService
    {
        public decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType);
    }
}
