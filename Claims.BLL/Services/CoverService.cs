using Claims.BLL.Services.IServices;

namespace Claims.BLL.Services
{
    public class CoverService : ICoversService
    {
        public decimal ComputePremium(DateOnly startDate, DateOnly endDate, CoverType coverType)
        {
            decimal totalPremium = 0m;

            int baseDayRate = 1250;
            decimal multiplier = 1.3m;

            // Get specific multiplier per cover type
            switch (coverType)
            {
                case CoverType.Yacht: multiplier = 1.1m; break;
                case CoverType.PassengerShip: multiplier = 1.2m; break;
                case CoverType.Tanker: multiplier = 1.5m; break;
            }

            decimal premiumPerDay = baseDayRate * multiplier;
            int insuranceLength = endDate.DayNumber - startDate.DayNumber;

            // Calculate premiums based of the insurance length
            // insuranceLength < 30 => whole insurance period has the same premium per day
            // insuranceLength > 30 and < 180 => first 30 days has the base premium per day, the remaining days has specific discount
            // insuranceLength > 180 => first 30 days has the base premium per day, following 150 days has specific discount, the remaining days has additional discount
            switch (insuranceLength)
            {
                case < 30:
                    totalPremium = insuranceLength * premiumPerDay;
                    break;
                case > 30 and < 180:
                    var discount = coverType == CoverType.Yacht ? 0.05m : 0.02m;
                    totalPremium =
                        30 * premiumPerDay + // First 30 days
                        (insuranceLength - 30) * (premiumPerDay - premiumPerDay * discount); // Remaining days
                    break;
                case > 150 and < 365:
                    var firstDis = coverType == CoverType.Yacht ? 0.05m : 0.02m;
                    var secondDis = coverType == CoverType.Yacht ? 0.08m : 0.03m;
                    totalPremium =
                        30 * premiumPerDay + // First 30 days
                        150 * (premiumPerDay - premiumPerDay * firstDis) + // Following 150 days
                        (insuranceLength - 180) * (premiumPerDay - premiumPerDay * secondDis); // Remaining days
                    break;

            }

            return totalPremium;
        }
    }
}
