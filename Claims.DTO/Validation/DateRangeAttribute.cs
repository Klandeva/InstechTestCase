using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.DTO.Validation
{
    public class DateRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IStartEndDate startEndDate = validationContext.ObjectInstance as IStartEndDate;

            if(startEndDate.StartDate.AddYears(1) < startEndDate.EndDate)
                return new ValidationResult("Total insurance period cannot exceed 1 year");

            return ValidationResult.Success;
        }
    }

    public interface IStartEndDate
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
