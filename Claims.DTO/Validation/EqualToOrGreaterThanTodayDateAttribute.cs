using System.ComponentModel.DataAnnotations;

namespace Claims.DTO.Validation
{
    public class EqualToOrGreaterThanTodayDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object dateToValidate)
        {
            DateOnly date = (DateOnly)dateToValidate;
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            return date.CompareTo(today) >= 0;
        }
    }

}
