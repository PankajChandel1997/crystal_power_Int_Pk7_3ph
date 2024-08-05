using Domain.Entities.ThreePhaseCTEntities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Helpers
{
    public static class StringExtensions
    {
        public static string CustomTrucate(this string str)
        {
            try
            {
                return (Math.Truncate((double.Parse(str, NumberStyles.Any)) * 100) / 100).ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static string CheckNullable(string value)
        {
            try
            {
                return !string.IsNullOrEmpty(value) ? (double.Parse(value, NumberStyles.Any) / 1000).ToString().CustomTrucate() : string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string CheckNullable100(string value)
        {
            try
            {
                return !string.IsNullOrEmpty(value) ? (double.Parse(value, NumberStyles.Any) / 100).ToString().CustomTrucate() : string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string CheckNullable10(string value)
        {
            try
            {
                return !string.IsNullOrEmpty(value) ? (double.Parse(value, NumberStyles.Any) / 10).ToString().CustomTrucate() : string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string CheckNullable3(string value)
        {
            try
            {
                return !string.IsNullOrEmpty(value) ? (double.Parse(value, NumberStyles.Any)/ 1000, 3).ToString() : string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string CheckNullableOnly(string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    return value == "0" ? value : (double.Parse(value, NumberStyles.Any)).ToString().CustomTrucate();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
