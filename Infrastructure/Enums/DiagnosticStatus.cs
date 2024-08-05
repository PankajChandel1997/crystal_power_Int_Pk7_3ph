namespace Infrastructure.Enums
{
    public enum DiagnosticStatus : short
    {
        Ok = 0,
        RTC_Battery_or_RTC_Corrupt = 1,
        Main_Battery = 16,
        RTC_Battery_and_Main_Battery= 17,
    }
}
