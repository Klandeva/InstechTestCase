using Claims.DTO.Validation;
using Newtonsoft.Json;

namespace Claims;

public class Cover : IStartEndDate
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "startDate")]
    [EqualToOrGreaterThanTodayDate(ErrorMessage = "StartDate cannot be in the past")]
    public DateOnly StartDate { get; set; }

    [JsonProperty(PropertyName = "endDate")]
    [DateRange]
    public DateOnly EndDate { get; set; }
    
    [JsonProperty(PropertyName = "coverType")]
    public CoverType Type { get; set; }

    [JsonProperty(PropertyName = "premium")]
    public decimal Premium { get; set; }
}

public enum CoverType
{
    Yacht = 0,
    PassengerShip = 1,
    ContainerShip = 2,
    BulkCarrier = 3,
    Tanker = 4
}