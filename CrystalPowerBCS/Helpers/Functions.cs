using Infrastructure.Enums;
using Infrastructure.Helpers;
using System;

namespace CrystalPowerBCS.Helpers
{
    public class Functions
    {
        /// <summary>
        /// method will return OBIS code by command type id
        /// </summary>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        public static string GetOBISCodeByCommandType(int commandTypeId, bool isHDLC = false)
        {
            if (commandTypeId == (int)CommandTypeEnum.ConnectDisconnect)
            {
                return "0.0.96.3.10.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetLoadLimit || commandTypeId == (int)CommandTypeEnum.GetLoadLimit)
            {
                return "0.0.17.0.0.255"; 

            }
            else if (commandTypeId == (int)CommandTypeEnum.GetBlockLoadProfile || commandTypeId == (int)CommandTypeEnum.GRBlockLoadProfile)
            {
                return "1.0.99.1.0.255";

            }
            else if (commandTypeId == (int)CommandTypeEnum.GetDailyLoadProfile)
            {
                return "1.0.99.2.0.255";

            }
            else if (commandTypeId == (int)CommandTypeEnum.GetInstantProfile)
            {
                return "1.0.94.91.0.255";

            }
            else if (commandTypeId == (int)CommandTypeEnum.GetBillingProfile)
            {
                return "1.0.98.1.0.255";

            }
            else if (commandTypeId == (int)CommandTypeEnum.FirmwareUpgrade)
            {
                return "0.0.44.0.0.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetRTC || commandTypeId == (int)CommandTypeEnum.GetClockStatus
                || commandTypeId == (int)CommandTypeEnum.GetRTC)
            {
                return "0.0.1.0.0.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetDemandIntegrationPeriod || commandTypeId == (int)CommandTypeEnum.GetDemandIntegrationPeriod)
            {
                return "1.0.0.8.0.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetProfileCapturePeriod || commandTypeId == (int)CommandTypeEnum.GetProfileCapturePeriod)
            {
                return "1.0.0.8.4.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetSingleActionSchedule || commandTypeId == (int)CommandTypeEnum.GetSingleActionSchedule)
            {
                return "0.0.15.0.0.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetActivityCalendarName || commandTypeId == (int)CommandTypeEnum.GetActivityCalendarName)
            {
                return "0.0.13.0.0.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetCalendarTOD || commandTypeId == (int)CommandTypeEnum.GetCalendarTOD)
            {
                return "0.0.13.0.0.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetCalendarActivationDate
                || commandTypeId == (int)CommandTypeEnum.GetCalendarActivationDate ||
                commandTypeId == (int)CommandTypeEnum.ActivateCalendar)
            {
                return "0.0.13.0.0.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetLLSSecretMRChange)
            {
                return "0.0.40.0.2.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetHLSKeyUSChange)
            {
                return "0.0.40.0.3.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetHLSKeyFWChange)
            {
                return "0.0.40.0.5.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetGlobalKeyChange)
            {
                return "0.0.43.0.3.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetESWFChange || commandTypeId == (int)CommandTypeEnum.GetESWF)
            {
                return isHDLC ? "0.0.94.91.26.255" : "0.0.94.91.26.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetMDReset)
            {
                return "0.0.10.0.1.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetInstantCapturePeriod || commandTypeId == (int)CommandTypeEnum.GetInstantCapturePeriod)
            {
                return "0.0.15.0.4.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetNamePlate)
            {
                return "0.0.94.91.10.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetControlEvent)
            {
                return "0.0.99.98.6.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetCurrentEvent)
            {
                return "0.0.96.11.1.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetNonRolloverEvent)
            {
                return "0.0.96.11.5.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetPowerEvent)
            {
                return "0.0.96.11.2.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetTransactionEvent)
            {
                return "0.0.96.11.3.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetVoltageEvent)
            {
                return "0.0.96.11.0.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetRelayStatus)
            {
                return "0.0.96.3.10.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetCurrentLimit || commandTypeId == (int)CommandTypeEnum.SetCurrentLimit)
            {
                return "1.0.94.91.128.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetVoltageEventProfile)
            {
                return "0.0.99.98.0.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetCurrentEventProfile)
            {
                return "0.0.99.98.1.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetPowerEventProfile)
            {
                return "0.0.99.98.2.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetTransactionEventProfile)
            {
                return "0.0.99.98.3.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetOtherEventProfile)
            {
                return "0.0.99.98.4.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetNonRollOverEventProfile)
            {
                return "0.0.99.98.5.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetControlEventProfile)
            {
                return "0.0.99.98.6.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetNetMeteringMode)
            {
                return isHDLC ? "0.0.94.91.19.255" : "0.0.94.96.19.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.SetNetMeteringMode)
            {
                return isHDLC ? "0.0.94.91.19.255" : "0.0.94.96.19.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetPaymentMode ||
            commandTypeId == (int)CommandTypeEnum.SetPaymentMode)
            {
                return isHDLC ? "0.0.94.91.20.255" : "0.0.94.96.20.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetPrepaidBalance ||
           commandTypeId == (int)CommandTypeEnum.SetPrepaidBalance)
            {
                return isHDLC ? "0.0.94.91.24.255" : "0.0.94.96.24.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetLastRechargeAmount ||
          commandTypeId == (int)CommandTypeEnum.SetLastRechargeAmount)
            {
                return isHDLC ? "0.0.94.91.21.255" : "0.0.94.96.21.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetTotalAmountAtLastRecharge ||
         commandTypeId == (int)CommandTypeEnum.SetTotalAmountAtLastRecharge)
            {
                return isHDLC ? "0.0.94.91.23.255" : "0.0.94.96.23.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetCurrentBalanceTime ||
        commandTypeId == (int)CommandTypeEnum.SetCurrentBalanceTime)
            {
                return isHDLC ? "0.0.94.91.25.255" : "0.0.94.96.25.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.GetLastRechargeTime ||
       commandTypeId == (int)CommandTypeEnum.SetLastRechargeTime)
            {
                return isHDLC ? "0.0.94.91.22.255" : "0.0.94.96.22.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.MeterType)
            {
                return "0.0.94.91.9.255";
            }
            else if(commandTypeId == (int)CommandTypeEnum.ESW)
            {
                return "0.0.94.91.18.255";
            }
            else if (commandTypeId == (int)CommandTypeEnum.DI)
            {
                return "0.0.99.98.7.255";
            }

            return null;

        }

        public static decimal ConvertByteToDecimal(object raw, int multiplyBy)
        {
            var decimalValue = (decimal)(((float)raw) * multiplyBy);

            return Math.Truncate(decimalValue * 1000) / 1000;
        }

        /// <summary>
        /// method will convert byte array to date time
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static DateTime? ConvertByteArrayToDateTime(byte[] byteArray, bool throwErrorIfParsingFailed = true,
            bool setDefaultDate = false)
        {
            try
            {
                var month = ((int)byteArray[2]);
                month = month == 0 ? 1 : month;

                var day = ((int)byteArray[3]);
                day = day == 0 ? 1 : day;

                var date = new DateTime(((int)(byteArray[0]) << 8) | ((int)(byteArray[1])), month, day, ((int)byteArray[5]), ((int)byteArray[6]), ((int)byteArray[7]), ((int)byteArray[8]), DateTimeKind.Utc);

                //add/subtract offset(in minutes)
                var offsetInMinutes = GetTimeOffsetInMinutesFromByteArray(byteArray);
                date = date.AddMinutes(offsetInMinutes);


                return setDefaultDate && date.Year < 1970 ? new DateTime(1970, 1, 1) : date;
            }
            catch (Exception ex)
            {
                if (throwErrorIfParsingFailed)
                {
                    var receivedPayload = string.Empty;

                    if (byteArray != null)
                    {
                        foreach (byte byt in byteArray)
                        {
                            receivedPayload += byt.ToString("X2") + " ";
                        }
                    }

                    throw;
                }

                return setDefaultDate ? new DateTime(1970, 1, 1) : (DateTime?)null;
            }
        }

        /// <summary>
        /// convert U byte to decimal
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static decimal ConvertUIntByteToDecimal(object raw, int divideBy)
        {
            var decimalValue = (((decimal)(uint.Parse(raw.ToString()))) / (decimal)divideBy);

            return Math.Truncate(decimalValue * 1000) / 1000;
        }

        /// <summary>
        /// method will get time offset from byte array of date time 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static int GetTimeOffsetInMinutesFromByteArray(byte[] byteArray)
        {
            //add/subtract offset(in minutes)
            var offsetInMinutes = -330;// (short)(byteArray[9] << 8 | byteArray[10]);

            return offsetInMinutes;
        }

        /// <summary>
        /// convert byte to decimal
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static decimal ConvertIntByteToDecimal(object raw, int divideBy)
        {
            var decimalValue = (((decimal)(int.Parse(raw.ToString()))) / (decimal)divideBy);

            return Math.Truncate(decimalValue * 1000) / 1000;
        }

        /// <summary>
        /// convert byte to int
        /// </summary>
        /// <param name="raws"></param>
        /// <returns></returns>
        public static int ConvertByteToInt(object raw)
        {
            return int.Parse(raw.ToString());
        }

        public static bool ConvertByteToBoolean(object raw)
        {
            try
            {
                return (bool)raw;
            }
            catch (Exception)
            {
                return raw.ToString() == "1";
            }
        }

        public static DateTime GetDateTimeByZone(DateTime datetime, string zone)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(zone);

            return TimeZoneInfo.ConvertTimeFromUtc(datetime, timeZoneInfo);
        }

        public static string ToIndiaDateTime(DateTime dateTime)
        {
            var date =  GetDateTimeByZone(dateTime, Constants.IndiaStandardTime);
            WriteToFile.WriteLog(DateTime.UtcNow + " : ToIndiaDateTime : Date ==>" + date);
            return date.AddHours(-11).ToString("dd-MM-yyyy HH:mm:ss");
        }

        //Geeting Date as DateTime
        public static DateTime ToIndiaDateTimeObject(DateTime dateTime)
        {
            var date = GetDateTimeByZone(dateTime, Constants.IndiaStandardTime);
            return date;
        }

        public static string CheckIfValidDate(string date)
        {
            try
            {
                if(date.Contains("-") || date.Contains(":") || date.Contains("/"))
                {
                    try
                    {
                        return DateTime.Parse(date.ToString()).AddHours(-11).ToString("dd-MM-yyyy HH:mm:ss");
                    }
                    catch (Exception)
                    {
                        return new DateTime(1970, 1, 1).ToString("dd-MM-yyyy HH:mm:ss");
                    }
                }

                return null;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public static string ToBillingIndiaDateTime(DateTime dateTime)
        {
            var date = GetDateTimeByZone(dateTime, Constants.IndiaStandardTime);
            return date.ToString("dd-MM-yyyy HH:mm:ss");
        }

        public static string ToIndiaDateTimeAdd11(DateTime dateTime)
        {
            var date = GetDateTimeByZone(dateTime, Constants.IndiaStandardTime);
            return date.AddHours(11).ToString("dd-MM-yyyy HH:mm:ss");
        }
        public static string BillingCheckIfValidDate(string date)
        {
            try
            {
                if (date.Contains("-") || date.Contains(":") || date.Contains("/"))
                {
                    try
                    {
                        return DateTime.Parse(date.ToString()).ToString("dd-MM-yyyy HH:mm:ss");
                    }
                    catch (Exception)
                    {
                        return new DateTime(1970, 1, 1).ToString("dd-MM-yyyy HH:mm:ss");
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string CheckIfValidDateAdd11(string date)
        {
            try
            {
                if (date.Contains("-") || date.Contains(":") || date.Contains("/"))
                {
                    try
                    {
                        return DateTime.Parse(date.ToString()).AddHours(11).ToString("dd-MM-yyyy HH:mm:ss");
                    }
                    catch (Exception)
                    {
                        return new DateTime(1970, 1, 1).ToString("dd-MM-yyyy HH:mm:ss");
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}

