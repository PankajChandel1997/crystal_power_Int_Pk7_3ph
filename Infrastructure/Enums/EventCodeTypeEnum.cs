using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Enums
{
    [Flags]
    public enum EventCodeTypeEnum : short
    {
        [Description("R-Phase – Voltage Missing – Occurrence")]
        R_Phase_Voltage_Missing_Occurrence = 1,

        [Description("R-Phase – Voltage Missing – Restoration")]
        R_Phase_Voltage_Missing_Restoration = 2,

        [Description("Y-Phase – Voltage Missing – Occurrence")]
        Y_Phase_Voltage_Missing_Occurrence = 3,

        [Description("Y-Phase – Voltage Missing – Restoration")]
        Y_Phase_Voltage_Missing_Restoration = 4,

        [Description("B-Phase – Voltage Missing – Occurrence")]
        B_Phase_VoltageMissing_Occurrence = 5,

        [Description("B-Phase – Voltage Missing – Restoration")]
        B_Phase_VoltageMissing_Restoration = 6,

        [Description("Over Voltage in any Phase - Occurrence")]
        Over_Voltage_in_any_Phase_Occurrence = 7,

        [Description("Over Voltage in any Phase - Restoration")]
        Over_Voltage_in_any_Phase_Restoration = 8,

        [Description("Low Voltage in any Phase - Occurrence")]
        Low_Voltage_in_any_Phase_Occurrence = 9,

        [Description("Low Voltage in any Phase - Restoration")]
        Low_Voltage_in_any_Phase_Restoration = 10,

        [Description("Voltage Unbalance - Occurrence")]
        Voltage_Unbalance_Occurrence = 11,

        [Description("Voltage Unbalance - Restoration")]
        Voltage_Unbalance_Restoration = 12,

        [Description("Phase Sequence Reverse - Occurrence")]
        Phase_Sequence_Reverse_Occurrence = 21,

        [Description("Phase Sequence Reverse - Restoration")]
        Phase_Sequence_Reverse_Restoration = 22,

        [Description("R Phase - Current reverse – Occurrence")]
        R_Phase_for_3P_Current_reverse_Occurrence = 51,

        [Description("R Phase - Current reverse – Restoration")]
        R_Phase_for_3P_Current_reverse_Restoration = 52,

        [Description("Y Phase – Current reverse – Occurrence")]
        Y_Phase_Current_reverse_Occurrence = 53,

        [Description("Y Phase – Current reverse – Restoration")]
        Y_Phase_Current_reverse_Restoration = 54,

        [Description("B Phase – Current reverse – Occurrence")]
        B_Phase_Current_reverse_Occurrence = 55,

        [Description("B Phase – Current reverse – Restoration")]
        B_Phase_Current_reverse_Restoration = 56,

        [Description("R Phase – CT open - Occurrence")]
        R_Phase_CT_open_Occurrence = 57,

        [Description("R Phase – CT Open - Restoration")]
        R_Phase_CT_Open_Restoration = 58,

        [Description("Y Phase – CT Open - Occurrence")]
        Y_Phase_CT_Open_Occurrence = 59,

        [Description("Y Phase – CT Open - Restoration")]
        Y_Phase_CT_Open_Restoration = 60,

        [Description("B Phase – CT Open - Occurrence")]
        B_Phase_CT_Open_Occurrence = 61,

        [Description("B Phase – CT Open - Restoration")]
        B_Phase_CT_Open_Restoration = 62,

        [Description("Current Unbalance - Occurrence")]
        Current_Unbalance_Occurrence = 63,

        [Description("Current Unbalance - Restoration")]
        Current_Unbalance_Restoration = 64,

        [Description("Curent bypass – Occurrence")]
        Curent_bypass_Occurrence = 65,

        [Description("Current bypass – Restoration")]
        Current_bypass_Restoration = 66,

        [Description("Over current — Occurrence")]
        Over_current_Occurrence = 67,

        [Description("Over current — Restoration")]
        Over_current_Restoration = 68,

        [Description("Earth Load - Occurrence")]
        Earth_Load_Occurrence = 69,

        [Description("Earth Load - Restoration")]
        Earth_Load_Restoration = 70,

        [Description("Current Mismatch - Occurrence")]
        Current_Mismatch_Occurrence = 81,

        [Description("Current Mismatch- Restoration")]
        Current_Mismatch_Restoration = 82,

        [Description("High Neutral Current - Occurrence")]
        High_Neutral_Current_Occurrence = 83,

        [Description("High Neutral Current - Restoration")]
        High_Neutral_Current_Restoration = 84,

        [Description("Power failure – Occurrence")]
        Power_failure_Occurrence = 101,

        [Description("Power failure – Restoration")]
        Power_failure_Restoration = 102,       

        [Description("Relay connect/disconnect")]
        Relay_connect_disconnect = 124,

        [Description("Passive Voltage Low Limit")]
        Passive_Voltage_Low_Limit = 126,

        [Description("Single action schedule for Instant Push")]
        Single_action_schedule_for_Instant_Push = 127,

        [Description("Single Action Schedule for Activation Date of Passive Billing Date")]
        Single_Action_Schedule_for_Activation_Date_of_Passive_Billing_Date = 128,

        [Description("Single Action Schedule for Activation Date of Passive Profile Capture Period")]
        Single_Action_Schedule_for_Activation_Date_of_Passive_Profile_Capture_Period = 129,

        [Description("Single Action Schedule for Activation Date of Passive Demand Integration Period")]
        Single_Action_Schedule_for_Activation_Date_of_Passive_Demand_Integration_Period = 130,

        [Description("Single Action Schedule for Activation Date of Passive Voltage High Limit")]
        Single_Action_Schedule_for_Activation_Date_of_Passive_Voltage_High_Limit = 131,

        [Description("Single Action Schedule for Activation Date of Passive Voltage Low Limit")]
        Single_Action_Schedule_for_Activation_Date_of_Passive_Voltage_Low_Limit = 132,

        [Description("Real Time Clock – Date and Time")]
        Real_Time_Clock_Date_and_Time = 151,

        [Description("Passive Demand Integration Period")]
        Passive_Demand_Integration_Period = 152,

        [Description("Passive Profile Capture Period")]
        Passive_Profile_Capture_Period = 153,

        [Description("Single-action Schedule for Billing Dates")]
        Single_action_Schedule_for_Billing_Dates = 154,

        [Description("Activity Calendar for Time Zones")]
        Activity_Calendar_for_Time_Zones = 155,

        [Description("New Firmware Activated")]
        New_Firmware_Activated = 157,

        [Description("Passive Over Load Limit")]
        Passive_Over_Load_Limitt = 158,

        [Description("Enabled - load limit function")]
        Enabled_load_limit_function = 159,

        [Description("Disabled - load limit function")]
        Disabled_load_limit_function = 160,

        [Description("LLS secret (MR) change")]
        LLS_secret_MR_change = 161,

        [Description("HLS key (US) change")]
        HLS_key_US_change = 162,

        [Description("HLS key (FW) change")]
        HLS_key_FW_change = 163,

        [Description("Global key change(encryption and authentication)")]
        Global_key_change_encryption_and_authentication = 164,

        [Description("ESWF change")]
        ESWF_change = 165,

        [Description("MDI reset")]
        MDI_reset = 166,

        [Description("Metering mode")]
        Metering_mode = 167,

        [Description("Single Action Schedule for Image Activation")]
        Passive_Voltage_High_Limit = 169,

        [Description("Configuration change to ‘Import-Export’mode")]
        Configuration_change_to_Import_Export_mode = 178,

        [Description("Passive Relay time (Time interval between auto-reconnection attempt, Lock out period of relay, Number of auto-reconnection attempts, Activation date-time) ")]
        Passive_Relay_time_Alert_time_Lockout_time_Lockout_Max_counter = 182,

        [Description("Single Action Schedule for Activation Date of Passive Over Load Limit")]
        Single_Action_Schedule_for_Activation_Date_of_Passive_Over_Load_Limit = 183,

        [Description("Apparent energy computation type (Lag + Lead)")]
        Apparent_energy_computation_type_Lag_Lead = 191,

        [Description("Apparent energy computation type (Lag Only)")]
        Apparent_energy_computation_type_Lag_Only = 192,

        [Description("Influence of Permanent Magnet or AC/ DC Electromagnet - Occurence")]
        Influence_of_Permanent_Magnet_or_AC_DC_Electromagnet_Occurence = 201,

        [Description("Influence of Permanent Magnet or AC/ DC Electromagnet - Restoration")]
        Influenc_of_Permanent_Magnet_or_AC_DC_Electromagnet_Restoration = 202,

        [Description("Neutral Disturbance - HF & DC - Occurence")]
        Neutral_Disturbance_HF_DC_Occurence = 203,

        [Description("Neutral Disturbance - HF & DC - Restoration")]
        Neutral_Disturbance_HF_DC_Restoration = 204,

        [Description("Low PF-- Occurance")]
        Low_PF_Occurance = 205,

        [Description("Low PF-- Restoration")]
        Low_PF_Restoration = 206,

        [Description("Single wire operation(Neutral Missing) - Occurrence")]
        Single_wire_operation_Neutral_Missing_Occurrence = 207,

        [Description("Single wire operation(Neutral Missing) - Restoration")]
        Single_wire_operation_Neutral_Missing_Restoration = 208,

        [Description("Plug in Communication module removal - Occurrence")]
        Plug_in_Communication_module_removal_Occurrence = 209,

        [Description("Plug in Communication module removal - Restoration")]
        Plug_in_Communication_module_removal_Restoration = 210,

        [Description("Configuration Change To Post-Paid Mode")]
        Configuration_Change_To_Post_Paid_Mode = 211,

        [Description("Configuration Change To Pre-Paid Mode")]
        Configuration_Change_To_Pre_Paid_Mode = 212,

        [Description("Configuration change to “Forwarded only” mode")]
        Configuration_change_to_Forwarded_only_mode = 213,

        [Description("Configuration change to“Import and Export” mode")]
        Configuration_change_to_Import_and_Export_mode = 214,

        [Description("Overload – occurrence")]
        Overload_occurrence = 215,

        [Description("Overload – restoration")]
        Overload_restoration = 216,

        [Description("Meter Cover Opening – Occurrence")]
        Meter_Cover_Opening_Occurrence = 251,

        [Description("Switch weld – Occurrence")]
        Switch_weld_Occurrence = 253,

        [Description("Load switch status - Disconnected")]
        Load_switch_status_Disconnected = 301,

        [Description("Load switch status - Connected")]
        Load_switch_status_Connected = 302,

        [Description("DI-1 - Occurrence")]
        DI1_Occurrence = 401,

        [Description("DI-1 - Restoration")]
        DI1_Restoration = 402,

        [Description("DI-2 - Occurrence")]
        DI2_Occurrence = 403,

        [Description("DI-2 - Restoration")]
        DI2_Restoration = 404,

        [Description("DI-3 - Occurrence")]
        DI3_Occurrence = 405,

        [Description("DI-3 - Restoration")]
        DI3_Restoration = 406,

        [Description("DI-4 - Occurrence")]
        DI4_Occurrence = 407,

        [Description("DI-4 - Restoration")]
        DI4_Restoration = 408,

        [Description("Wrong Phase association- Occurrence")]
        Wrong_Phase_Association_Occurrence = 887,

        [Description("Wrong Phase association- Restoration")]
        Wrong_Phase_Association_Restoration = 888,

        [Description("Temperature Rise- Occurrence")]
        Temperature_Rise_Occurrence = 889,

        [Description("Temperature Rise- Restoration")]
        Temperature_Rise_Restoration = 890,

        [Description("ESD- Occurrence")]
        ESD_Occurrence = 891,

        [Description("ESD- Restoration")]
        ESD_Restoration = 892,

        [Description("DI 4 Restored")]
        DI_4_Restored = 896,

        [Description("DI 3 Restored")]
        DI_3_Restored = 897,

        [Description("DI 2 Restored")]
        DI_2_Restored = 898,

        [Description("DI 1 Restored")]
        DI_1_Restored = 899,

        [Description("DI 1 Occured")]
        DI_1_Occured =999,

        [Description("DI 2 Occured")]
        DI_2_Occured = 998,

        [Description("DI 3 Occured")]
        DI_3_Occured = 997,

        [Description("DI 4 Occured")]
        DI_4_Occured = 996,
    }
}
