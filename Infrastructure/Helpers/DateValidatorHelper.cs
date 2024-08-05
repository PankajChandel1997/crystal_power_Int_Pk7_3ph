using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public class DateValidatorHelper
    {
        public bool ValidateDate(string date)
        {
            try
            {
                DateTime.Parse(date.ToString()).ToString("dd-MM-yyyy HH:mm:ss");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static DateTime GetDateTimeByZone(DateTime datetime, string zone)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(zone);

            return TimeZoneInfo.ConvertTimeFromUtc(datetime, timeZoneInfo);
        }

        public static string ToIndiaDateTime(DateTime dateTime)
        {
            var date = GetDateTimeByZone(dateTime, "India Standard Time");
            return date.ToString("dd-MM-yyyy HH:mm:ss");
        }
    }
}
