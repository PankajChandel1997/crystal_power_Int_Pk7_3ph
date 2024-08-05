using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CrystalPowerBCS.Helpers
{
    public class TimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(false, "Time cannot be empty.");
            }

            string timeString = value.ToString();

            if (TimeSpan.TryParseExact(timeString, "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out _))
            {
                return ValidationResult.ValidResult;
            }

            return new ValidationResult(false, "Invalid time format. Please enter time as hh:mm:ss.");
        }
    }
}
