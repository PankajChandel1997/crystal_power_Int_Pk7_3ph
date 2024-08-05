using ClosedXML.Excel;
using CrystalPowerBCS.DbFunctions;
using DocumentFormat.OpenXml.Spreadsheet;
using Infrastructure.DTOs;
using Infrastructure.DTOs.EventDTOs;
using Infrastructure.DTOs.SinglePhaseEventDTOs;
using Infrastructure.DTOs.ThreePhaseEventCTDTOs;
using Infrastructure.DTOs.ThreePhaseEventDTOs;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.SKCharts;
using LiveChartsCore.SkiaSharpView.WPF;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace CrystalPowerBCS.Helpers
{
    public static class ExportHelper
    {
        public static MeterCommand MeterCommand;
        public static string dateFormat = "dd-MM-yyyy HH:mm:ss";
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dt = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                if (prop.Name.ToLower() != "meterno" && prop.Name.ToLower() != "manspecificfirmwareversion")
                {
                    dt.Columns.Add(prop.DisplayName, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }

            }
            foreach (T item in data)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyDescriptor pdt in properties)
                {
                    if (pdt.Name.ToLower() != "meterno" && pdt.Name.ToLower() != "manspecificfirmwareversion")
                    {
                        row[pdt.DisplayName] = pdt.GetValue(item) ?? DBNull.Value;
                    }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static DataTable ToBlockThreePhaseDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dt = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                if (prop.Name.ToLower() != "meterno" && prop.Name.ToLower() != "manspecificfirmwareversion" && prop.Name.ToLower() != "powerfactorrphase" && prop.Name.ToLower() != "powerfactoryphase" && prop.Name.ToLower() != "powerfactorbphase")
                {
                    dt.Columns.Add(prop.DisplayName, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }

            }
            foreach (T item in data)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyDescriptor pdt in properties)
                {
                    if (pdt.Name.ToLower() != "meterno" && pdt.Name.ToLower() != "manspecificfirmwareversion" && pdt.Name.ToLower() != "powerfactorrphase" && pdt.Name.ToLower() != "powerfactoryphase" && pdt.Name.ToLower() != "powerfactorbphase")
                    {
                        row[pdt.DisplayName] = pdt.GetValue(item) ?? DBNull.Value;
                    }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static DataTable AddMeterHeader(string MeterNo)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool GenerateExcel(DataTable dt, string FileName, string DataGridName, string MeterNo)
        {
            try
            {
                //Fetching Meter Details
                MeterDto meterData = new MeterDto();
                Dispatcher.CurrentDispatcher.Invoke(async () =>
                {
                    meterData = await GetMeterDetailsByMeterNo(MeterNo);
                });

                List<MeterDto> meterList = new List<MeterDto>();

                if (meterData != null)
                {
                    meterList.Add(meterData);
                }

                //Creating Meter Header DataTable
                DataTable meterDt = ToDataTable(meterList);

                //Exporting to Excel
                string downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);
                string folderPath = downloadsPath + "\\CrystalPowerBCSExport\\";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                FileName = MeterNo + "_" + FileName;

                if (File.Exists(folderPath + FileName + ".xlsx"))
                {
                    FileName = FileName + "_" + Guid.NewGuid().ToString();
                }

                if (DataGridName == Constants.NamePlate)
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add();
                        ws.Cell(1, 1).Value = "Sinhal Udyog Pvt Ltd";
                        ws.Cell(3, 1).Value = DataGridName;
                        ws.Cell(6, 1).InsertTable(dt);

                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(folderPath + FileName + ".xlsx");
                        }
                    }
                }
                else
                {
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add();
                        ws.Cell(1, 1).Value = "Sinhal Udyog Pvt Ltd";
                        ws.Cell(3, 1).Value = DataGridName;
                        ws.Cell(6, 1).InsertTable(meterDt, DataGridName);
                        ws.Cell(10, 1).InsertTable(dt);

                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(folderPath + FileName + ".xlsx");
                        }
                    }
                }

                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification, DataGridName + " Exported SuccessFully", NotificationType.Success, CloseOnClick: true);

                Process.Start("explorer.exe", folderPath);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static bool GeneratePdf(DataTable dt, string FileName, string DataGridName, string MeterNo)
        {
            try
            {
                //Fetching Meter Details
                MeterDto meterData = new MeterDto();
                Dispatcher.CurrentDispatcher.Invoke(async () =>
                {
                    meterData = await GetMeterDetailsByMeterNo(MeterNo);
                });

                List<MeterDto> meterList = new List<MeterDto>();

                if (meterData != null)
                {
                    meterList.Add(meterData);
                }

                //Creating Meter Header DataTable
                DataTable meterDt = ToDataTable(meterList);

                //Exporting to Pdf
                string downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);
                string folderPath = downloadsPath + "\\CrystalPowerBCSExport\\";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                FileName = MeterNo + "_" + FileName;

                if (File.Exists(folderPath + FileName + ".pdf"))
                {
                    FileName = FileName + "_" + Guid.NewGuid().ToString();
                }
                Document document;
                if (FileName.ToLower().Contains("InstantaneousProfileSingle".ToLower()) || FileName.ToLower().Contains("InstantaneousProfileThree".ToLower()))
                {
                    document = new Document(iTextSharp.text.PageSize.A4.Rotate());
                }
                else
                {
                    document = new Document();
                }

                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(folderPath + FileName + ".pdf", FileMode.Create));
                document.Open();

                iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);

                //Adding Headings
                document.Add(new Paragraph(" "));
                var header = new Paragraph("Sinhal Udyog Pvt Ltd.");
                header.Alignment = Element.ALIGN_CENTER;
                document.Add(header);
                document.Add(new Paragraph(" "));
                var gridHeader = new Paragraph(DataGridName);
                gridHeader.Alignment = Element.ALIGN_CENTER;
                document.Add(gridHeader);
                document.Add(new Paragraph(" "));

                if (DataGridName == Constants.NamePlate)
                {
                    // Adding Data Grid Data
                    PdfPTable table = new PdfPTable(dt.Columns.Count);
                    PdfPRow row = null;

                    float[] widths = new float[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                        widths[i] = 4f;

                    table.SetWidths(widths);

                    table.WidthPercentage = 100;
                    int iCol = 0;
                    string colname = "";

                    PdfPCell cell = new PdfPCell(new Phrase("Products"));

                    cell.Colspan = dt.Columns.Count;

                    foreach (DataColumn c in dt.Columns)
                    {
                        table.AddCell(new Phrase(c.ColumnName, font5));
                    }

                    foreach (DataRow r in dt.Rows)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            for (int h = 0; h < dt.Columns.Count; h++)
                            {
                                table.AddCell(new Phrase(r[h].ToString(), font5));
                            }
                        }
                    }

                    document.Add(table);
                    document.Close();
                }
                else
                {
                    //Adding Meter Data
                    PdfPTable table1 = new PdfPTable(meterDt.Columns.Count);
                    PdfPRow row1 = null;

                    float[] widths1 = new float[meterDt.Columns.Count];
                    for (int i = 0; i < meterDt.Columns.Count; i++)
                        widths1[i] = 4f;

                    table1.SetWidths(widths1);

                    table1.WidthPercentage = 100;
                    int iCols = 0;
                    string colnames = "";

                    PdfPCell cells = new PdfPCell(new Phrase("Products"));

                    cells.Colspan = meterDt.Columns.Count;

                    foreach (DataColumn c in meterDt.Columns)
                    {
                        table1.AddCell(new Phrase(c.ColumnName, font5));
                    }

                    foreach (DataRow r in meterDt.Rows)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            for (int h = 0; h < meterDt.Columns.Count; h++)
                            {
                                table1.AddCell(new Phrase(r[h].ToString(), font5));
                            }
                        }
                    }

                    document.Add(table1);

                    document.Add(new Paragraph(" "));

                    // Adding Data Grid Data
                    PdfPTable table = new PdfPTable(dt.Columns.Count);
                    PdfPRow row = null;

                    float[] widths = new float[dt.Columns.Count];
                    for (int i = 0; i < dt.Columns.Count; i++)
                        widths[i] = 4f;

                    table.SetWidths(widths);

                    table.WidthPercentage = 100;
                    int iCol = 0;
                    string colname = "";

                    PdfPCell cell = new PdfPCell(new Phrase("Products"));

                    cell.Colspan = dt.Columns.Count;

                    foreach (DataColumn c in dt.Columns)
                    {
                        table.AddCell(new Phrase(c.ColumnName, font5));
                    }

                    foreach (DataRow r in dt.Rows)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            for (int h = 0; h < dt.Columns.Count; h++)
                            {
                                table.AddCell(new Phrase(r[h].ToString(), font5));
                            }
                        }
                    }

                    document.Add(table);
                    document.Close();
                }

                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification, DataGridName + " Exported SuccessFully", NotificationType.Success, CloseOnClick: true);

                Process.Start("explorer.exe", folderPath);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool GenerateGraphPng(CartesianChart ChartControl, string FileName, string FileType, string MeterNo)
        {
            try
            {
                //Exporting to Png
                string downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);
                string folderPath = downloadsPath + "\\CrystalPowerBCSExport\\";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                FileName = MeterNo + "_" + FileName;

                if (File.Exists(folderPath + FileName + "." + FileType))
                {
                    FileName = FileName + "_" + Guid.NewGuid().ToString();
                }

                var skChart = new SKCartesianChart(ChartControl) { Width = 1920, Height = 800 };
                skChart.SaveImage(folderPath + FileName + "." + FileType);

                Process.Start("explorer.exe", folderPath);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Get Meter Data
        public static async Task<MeterDto> GetMeterDetailsByMeterNo(string MeterNo)
        {
            try
            {
                MeterCommand = new MeterCommand();
                MeterDto meterData = await MeterCommand.GetMeterByMeterNo(MeterNo);
                return meterData;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        //Get Known Folders Path
        public enum KnownFolder
        {
            Contacts,
            Downloads,
            Favorites,
            Links,
            SavedGames,
            SavedSearches
        }

        public static class KnownFolders
        {
            private static readonly Dictionary<KnownFolder, Guid> _guids = new()
            {
                [KnownFolder.Contacts] = new("56784854-C6CB-462B-8169-88E350ACB882"),
                [KnownFolder.Downloads] = new("374DE290-123F-4565-9164-39C4925E467B"),
                [KnownFolder.Favorites] = new("1777F761-68AD-4D8A-87BD-30B759FA33DD"),
                [KnownFolder.Links] = new("BFB9D5E0-C6A9-404C-B2B2-AE6DB6AF4968"),
                [KnownFolder.SavedGames] = new("4C5C32FF-BB9D-43B0-B5B4-2D72E54EAAA4"),
                [KnownFolder.SavedSearches] = new("7D1D3A04-DEBB-4115-95CF-2F29DA2920DA")
            };

            public static string GetPath(KnownFolder knownFolder)
            {
                return SHGetKnownFolderPath(_guids[knownFolder], 0);
            }

            [DllImport("shell32",
                CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
            private static extern string SHGetKnownFolderPath(
                [MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags,
                nint hToken = 0);
        }

        // dataTable for Billing Profile
        public static DataTable ToDataTableBilling<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dt = new DataTable();
            dt.Columns.Add("RealTimeClock");

            if (data.Count == 0)
                return dt;

            // Create columns for each billing date
            foreach (T item in data)
            {
                var billingDateProp = properties.Find("RealTimeClock", true);
                string billingDate = billingDateProp != null ? billingDateProp.GetValue(item)?.ToString() : DBNull.Value.ToString();
                if (!dt.Columns.Contains(billingDate))
                {
                    dt.Columns.Add(billingDate);
                }
            }

            // Create rows for each property
            foreach (PropertyDescriptor pdt in properties)
            {
                string propertyName = pdt.DisplayName;

                if (
                    propertyName.ToLower() != "meterno" &&
                    propertyName.ToLower() != "number" &&
                    propertyName.ToLower() != "manspecificfirmwareversion" &&
                    propertyName.ToLower() != "cumulativeenergykwhimportconsumption" &&
                    propertyName.ToLower() != "cumulativeenergykwhexportconsumption" &&
                    propertyName.ToLower() != "cumulativeenergykvahimportconsumption" &&
                    propertyName.ToLower() != "cumulativeenergykvahexportconsumption" &&
                    propertyName.ToLower() != "mdkwimportdatetimetz6" &&
                    propertyName.ToLower() != "genericeventlogsequencenumber" &&
                    propertyName.ToLower() != "createdon" /*&& propertyName.ToLower() != "realtimeclock"*/
                )
                {
                    DataRow row = dt.NewRow();
                    row["RealTimeClock"] = propertyName;

                    foreach (T item in data)
                    {
                        var billingDateProp = properties.Find("RealTimeClock", true);
                        string billingDate = billingDateProp != null ? billingDateProp.GetValue(item)?.ToString() : DBNull.Value?.ToString();
                        row[billingDate] = pdt.GetValue(item) ?? DBNull.Value;
                    }

                    dt.Rows.Add(row);
                }
            }
            if (dt.Rows.Count > 0)
            {
                // Remove the first row
                dt.Rows.RemoveAt(0);
            }


            return dt;
        }
        public static DataTable ToSinglePhaseMeterDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dt = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                if (prop.Name.ToLower() != "meterconstant" && prop.Name.ToLower() != "metervoltagerating" && prop.Name.ToLower() != "nicfirmwareversionnumber" && prop.Name.ToLower() != "mdintegrationperiod" && prop.Name.ToLower() != "loadsurveyintegrationperiod" && prop.Name.ToLower() != "kvahenergydefinition")
                {
                    dt.Columns.Add(prop.DisplayName, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                }

            }
            foreach (T item in data)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyDescriptor pdt in properties)
                {
                    if (pdt.Name.ToLower() != "meterconstant" && pdt.Name.ToLower() != "metervoltagerating" && pdt.Name.ToLower() != "nicfirmwareversionnumber" && pdt.Name.ToLower() != "mdintegrationperiod" && pdt.Name.ToLower() != "loadsurveyintegrationperiod" && pdt.Name.ToLower() != "kvahenergydefinition")
                    {
                        row[pdt.DisplayName] = pdt.GetValue(item) ?? DBNull.Value;
                    }
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        // Generate PDF and Excel Methods for Billing Profile Only
        public static bool GenerateBillingExcel(List<BillingProfileSinglePhaseDto> BillingProfileSinglePhase, List<BillingProfileThreePhaseDto> BillingProfileThreePhase, List<BillingProfileThreePhaseCTDto> BillingProfileThreePhaseCT, string FileName, string DataGridName, string MeterNo)
        {
            try
            {
                MeterDto meterData = new MeterDto();
                Dispatcher.CurrentDispatcher.Invoke(async () =>
                {
                    meterData = await GetMeterDetailsByMeterNo(MeterNo);
                });

                List<MeterDto> meterList = new List<MeterDto>();

                if (meterData != null)
                {
                    meterList.Add(meterData);

                }

                //Creating Meter Header DataTable
                DataTable mData = ToDataTable(meterList);

                string folderPath = "";
                XLWorkbook wb = new XLWorkbook();
                DataTable meterDt = new();
                wb.Worksheets.Add(DataGridName);
                wb.Worksheet(1).Cell(1, 1).Value = "Sinhal Udyog Pvt Ltd";
                wb.Worksheet(1).Cell(1, 1).Style.Font.Bold = true;
                wb.Worksheet(1).Cell(1, 1).Style.Font.FontSize = 12;
                wb.Worksheet(1).Cell(3, 1).Value = DataGridName;
                wb.Worksheet(1).Cell(3, 1).Style.Font.Bold = true;
                wb.Worksheet(1).Cell(3, 1).Style.Font.FontSize = 11;

                wb.Worksheet(1).Columns().AdjustToContents();

                wb.Worksheet(1).Cell(6, 1).InsertTable(mData);

                List<string> dataDates = BillingProfileSinglePhase != null ? BillingProfileSinglePhase.OrderByDescending(y => DateTime.ParseExact(y.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).Select(x => x.RealTimeClock).Take(13).ToList() : BillingProfileThreePhase != null ? BillingProfileThreePhase.OrderByDescending(y => DateTime.ParseExact(y.RealTimeClock, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).Select(x => x.RealTimeClock).Take(13).ToList() : BillingProfileThreePhaseCT.OrderByDescending(y => DateTime.ParseExact(y.BillingDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture)).Select(x => x.BillingDate).Take(13).ToList();

                // Get the current month and year
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;

                // Filter out dates from the current month


                int Sum = 5;
                Sum = Sum + mData.Rows.Count;
                List<string> UniqueDates = new List<string>();
                var month = "";
                foreach (var date in dataDates)
                {
                    month = DateTime.Parse(date).Month.ToString();
                    if (!UniqueDates.Contains(month))
                    {
                        List<BillingProfileSinglePhaseDto> currentSinglePhaseMonth = new();
                        List<BillingProfileThreePhaseDto> currentThreePhaseMonth = new();
                        List<BillingProfileThreePhaseCTDto> currentThreePhaseCTMonth = new();

                        var SheetName = DateTime.ParseExact(date, dateFormat, CultureInfo.InvariantCulture).ToString("MMMM") + "-" + DateTime.ParseExact(date.ToString(), dateFormat, CultureInfo.InvariantCulture).Year;

                        if (BillingProfileSinglePhase != null)
                        {
                            currentSinglePhaseMonth = BillingProfileSinglePhase.Where(e => DateTime.ParseExact(e.RealTimeClock.ToString(), dateFormat, CultureInfo.InvariantCulture).Month == DateTime.ParseExact(date, dateFormat, CultureInfo.InvariantCulture).Month && DateTime.ParseExact(e.RealTimeClock, dateFormat, CultureInfo.InvariantCulture).Year == DateTime.ParseExact(date, dateFormat, CultureInfo.InvariantCulture).Year).ToList();
                            meterDt = ToDataTableBilling(currentSinglePhaseMonth);
                        }
                        if (BillingProfileThreePhase != null)
                        {
                            currentThreePhaseMonth = BillingProfileThreePhase.Where(e => DateTime.ParseExact(e.RealTimeClock, dateFormat, CultureInfo.InvariantCulture).Month == DateTime.ParseExact(date, dateFormat, CultureInfo.InvariantCulture).Month && DateTime.ParseExact(e.RealTimeClock, dateFormat, CultureInfo.InvariantCulture).Year == DateTime.ParseExact(date, dateFormat, CultureInfo.InvariantCulture).Year).ToList();
                            meterDt = ToDataTableBilling(currentThreePhaseMonth);
                        }
                        if (BillingProfileThreePhaseCT != null)
                        {
                            currentThreePhaseCTMonth = BillingProfileThreePhaseCT.Where(e => DateTime.ParseExact(e.BillingDate, dateFormat, CultureInfo.InvariantCulture).Month == DateTime.ParseExact(date, dateFormat, CultureInfo.InvariantCulture).Month && DateTime.ParseExact(e.BillingDate, dateFormat, CultureInfo.InvariantCulture).Year == DateTime.ParseExact(date, dateFormat, CultureInfo.InvariantCulture).Year).ToList();
                            meterDt = ToDataTableBilling(currentThreePhaseCTMonth);
                        }
                        if (meterDt.Rows.Count > 0)
                        {
                            Sum = Sum + 5;
                            wb.Worksheet(1).Cell(Sum - 1, 1).Value = SheetName;
                            wb.Worksheet(1).Cell(Sum - 1, 1).Style.Font.Bold = true;
                            wb.Worksheet(1).Cell(Sum - 1, 1).Style.Font.FontSize = 12;

                            wb.Worksheet(1).Columns().AdjustToContents();

                            wb.Worksheet(1).Cell(Sum, 1).InsertTable(meterDt);
                            Sum = Sum + meterDt.Rows.Count;
                        }
                        UniqueDates.Add(month);
                    }

                }

                //Exporting to Excel
                string downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);
                folderPath = downloadsPath + "\\CrystalPowerBCSExport\\";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                FileName = MeterNo + "_" + FileName;

                if (File.Exists(folderPath + FileName + ".xlsx"))
                {
                    FileName = FileName + "_" + Guid.NewGuid().ToString();
                }

                wb.SaveAs(folderPath + FileName + ".xlsx");
                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification, DataGridName + " Exported SuccessFully", NotificationType.Success, CloseOnClick: true);

                Process.Start("explorer.exe", folderPath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool GenerateBillingPdf(List<BillingProfileSinglePhaseDto> BillingProfileSinglePhase, List<BillingProfileThreePhaseDto> BillingProfileThreePhase, List<BillingProfileThreePhaseCTDto> BillingProfileThreePhaseCT, string FileName, string DataGridName, string MeterNo)
        {
            try
            {
                //Fetching Meter Details
                MeterDto meterData = new MeterDto();
                Dispatcher.CurrentDispatcher.Invoke(async () =>
                {
                    meterData = await GetMeterDetailsByMeterNo(MeterNo);
                });

                List<MeterDto> meterList = new List<MeterDto>();

                if (meterData != null)
                {
                    meterList.Add(meterData);
                }

                //Creating Meter Header DataTable
                DataTable meterDt;
                //Creating Meter Header DataTable
                if (FileName.ToLower().Contains("single"))
                {
                    meterDt = ToSinglePhaseMeterDataTable(meterList);
                }
                else
                {
                    meterDt = ToDataTable(meterList);
                }

                //Exporting to Pdf
                string downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);
                string folderPath = downloadsPath + "\\CrystalPowerBCSExport\\";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                FileName = MeterNo + "_" + FileName;

                if (File.Exists(folderPath + FileName + ".pdf"))
                {
                    FileName = FileName + "_" + Guid.NewGuid().ToString();
                }
                Document document = new Document(iTextSharp.text.PageSize.A4.Rotate());
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(folderPath + FileName + ".pdf", FileMode.Create));
                document.Open();


                iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);

                //Adding Meter Data
                PdfPTable table1 = new PdfPTable(meterDt.Columns.Count);
                PdfPRow row1 = null;

                float[] widths1 = new float[meterDt.Columns.Count];
                for (int i = 0; i < meterDt.Columns.Count; i++)
                    widths1[i] = 4f;

                table1.SetWidths(widths1);

                table1.WidthPercentage = 100;
                int iCols = 0;
                string colnames = "";

                PdfPCell cells = new PdfPCell(new Phrase("Products"));

                cells.Colspan = meterDt.Columns.Count;

                foreach (DataColumn c in meterDt.Columns)
                {
                    table1.AddCell(new Phrase(c.ColumnName, font5));
                }

                foreach (DataRow r in meterDt.Rows)
                {
                    if (meterDt.Rows.Count > 0)
                    {
                        for (int h = 0; h < meterDt.Columns.Count; h++)
                        {
                            table1.AddCell(new Phrase(r[h].ToString(), font5));
                        }
                    }
                }
                var header = new Paragraph("Sinhal Udyog Pvt Ltd.");
                header.Alignment = Element.ALIGN_CENTER;
                document.Add(header);
                var gridHeader = new Paragraph(DataGridName);
                gridHeader.Alignment = Element.ALIGN_CENTER;
                document.Add(gridHeader);
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph("Meter Details"));
                document.Add(new Paragraph(" "));
                document.Add(table1);
                DataTable meterDtPdf = new();

                List<string> dataDates = BillingProfileSinglePhase != null ? BillingProfileSinglePhase.OrderByDescending(y => System.DateTime.ParseExact(y.RealTimeClock, dateFormat, CultureInfo.InvariantCulture)).Select(x => x.RealTimeClock).Take(12).ToList() : BillingProfileThreePhase != null ? BillingProfileThreePhase.OrderByDescending(y => System.DateTime.ParseExact(y.RealTimeClock, dateFormat, CultureInfo.InvariantCulture)).Select(x => x.RealTimeClock).Take(12).ToList() : BillingProfileThreePhaseCT.OrderByDescending(y => System.DateTime.ParseExact(y.BillingDate, dateFormat, CultureInfo.InvariantCulture)).Select(x => x.BillingDate).Take(12).ToList();

                List<string> UniqueDates = dataDates.Distinct().ToList();

                //for (int i = 0; i < dataDates.Count; i += 3)
                //{
                //// Collect up to three dates in this batch
                //List<string> months = dataDates.Skip(i).Take(3).ToList();

                //// Ensure we have up to three months if possible
                //List<string> monthStrings = months.Select(date => DateTime.Parse(date).Month.ToString()).ToList();

                //// Check if all months in the current batch are unique
                //bool allUnique = monthStrings.All(month => !UniqueDates.Contains(month));

                //if (allUnique)
                //{

                List<BillingProfileSinglePhaseDto> currentSinglePhaseMonth = new();
                List<BillingProfileThreePhaseDto> currentThreePhaseMonth = new();
                List<BillingProfileThreePhaseCTDto> currentThreePhaseCTMonth = new();

                //var SheetName = DateTime.ParseExact(dataDates[0], dateFormat, CultureInfo.InvariantCulture).ToString("MMMM");
                //var SheetYear = DateTime.ParseExact(dataDates[0], dateFormat, CultureInfo.InvariantCulture).Year;

                foreach (var item in UniqueDates)
                {
                    if (BillingProfileSinglePhase != null)
                    {
                        var singlePhaseMonth = BillingProfileSinglePhase.Where(e => DateTime.ParseExact(e.RealTimeClock, dateFormat, CultureInfo.InvariantCulture).Month == DateTime.ParseExact(item, dateFormat, CultureInfo.InvariantCulture).Month && DateTime.ParseExact(e.RealTimeClock, dateFormat, CultureInfo.InvariantCulture).Year == DateTime.ParseExact(item, dateFormat, CultureInfo.InvariantCulture).Year).ToList();

                        currentSinglePhaseMonth.AddRange(singlePhaseMonth);
                    }
                    else if (BillingProfileThreePhase != null)
                    {
                        var threePhaseMonth = BillingProfileThreePhase.Where(e => DateTime.ParseExact(e.RealTimeClock, dateFormat, CultureInfo.InvariantCulture).Month == DateTime.ParseExact(item, dateFormat, CultureInfo.InvariantCulture).Month && DateTime.ParseExact(e.RealTimeClock, dateFormat, CultureInfo.InvariantCulture).Year == DateTime.ParseExact(item, dateFormat, CultureInfo.InvariantCulture).Year).ToList();

                        currentThreePhaseMonth.AddRange(threePhaseMonth);
                    }
                    else if (BillingProfileThreePhaseCT != null)
                    {
                        var threePhaseCTMonth = BillingProfileThreePhaseCT.Where(e => DateTime.ParseExact(e.BillingDate, dateFormat, CultureInfo.InvariantCulture).Month == DateTime.ParseExact(item, dateFormat, CultureInfo.InvariantCulture).Month && DateTime.ParseExact(e.BillingDate, dateFormat, CultureInfo.InvariantCulture).Year == DateTime.ParseExact(item, dateFormat, CultureInfo.InvariantCulture).Year).ToList();

                        currentThreePhaseCTMonth.AddRange(threePhaseCTMonth);
                    }
                }

                if (BillingProfileSinglePhase != null)
                {
                    meterDtPdf = ToDataTableBilling(currentSinglePhaseMonth);
                }
                if (BillingProfileThreePhase != null)
                {
                    meterDtPdf = ToDataTableBilling(currentThreePhaseMonth);
                }
                if (BillingProfileThreePhaseCT != null)
                {
                    meterDtPdf = ToDataTableBilling(currentThreePhaseCTMonth);
                }

                // document.Add(new Paragraph(" "));

                if (meterDtPdf.Rows.Count > 0)
                {
                    // Adding document title and spacing
                    //document.Add(new Paragraph(SheetName + "-" + SheetYear));
                    document.Add(new Paragraph(" "));

                    // Creating PdfPTable with the number of columns in the DataTable
                    PdfPTable table = new PdfPTable(meterDtPdf.Columns.Count);

                    // Setting column widths
                    float[] widths = new float[meterDtPdf.Columns.Count];
                    for (int n = 0; n < meterDtPdf.Columns.Count; n++)
                        widths[n] = 4f;
                    table.SetWidths(widths);

                    table.WidthPercentage = 100;

                    // Adding the table header
                    foreach (DataColumn c in meterDtPdf.Columns)
                    {
                        table.AddCell(new Phrase(c.ColumnName, font5));
                    }

                    // Adding the rows
                    foreach (DataRow r in meterDtPdf.Rows)
                    {
                        for (int h = 0; h < meterDtPdf.Columns.Count; h++)
                        {
                            table.AddCell(new Phrase(r[h].ToString(), font5));
                        }
                    }

                    // Adding the table to the document
                    document.Add(table);
                }
                //}
                //UniqueDates.AddRange(monthStrings);
                //}

                document.Close();

                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification, DataGridName + " Exported SuccessFully", NotificationType.Success, CloseOnClick: true);

                Process.Start("explorer.exe", folderPath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static DataTable ToDataTableByColumn<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dt = new DataTable();

            for (int i = 1; i < 5; i++)
            {
                DataRow dr = dt.NewRow();
                dr[i] = data[i];
            }

            return dt;
        }

        public static bool GenerateReportsPdf(List<InstantaneousProfileSinglePhaseDto> InstantaneousProfileSinglePhase, List<BillingProfileSinglePhaseDto> BillingProfileSinglePhase, List<BlockLoadProfileSinglePhaseDto> BlockLoadProfileSinglePhase, List<DailyLoadProfileSinglePhaseDto> DailyLoadProfileSinglePhaseReport, List<InstantaneousProfileThreePhaseDto> InstantaneousProfileThreePhase, List<BillingProfileThreePhaseDto> BillingProfileThreePhase, List<BlockLoadProfileThreePhaseDto> BlockLoadProfileThreePhase, List<DailyLoadProfileThreePhaseDto> DailyLoadProfileThreePhaseReport, List<InstantaneousProfileThreePhaseCTDto> InstantaneousProfileThreePhaseCT, List<BillingProfileThreePhaseCTDto> BillingProfileThreePhaseCT, List<BlockLoadProfileThreePhaseCTDto> BlockLoadProfileThreePhaseCT, List<DailyLoadProfileThreePhaseCTDto> DailyLoadProfileThreePhaseCTReport, List<AllEventsSinglePhaseDto> AllEventsSinglePhaseReport, List<AllEventsDTO> AllEventsThreePhaseReport, List<SelfDiagnosticDto> SelfDiagnosticDtos,string FileName, string DataGridName, string MeterNo, string MeterType)
        {
            try
            {
                //Fetching Meter Details
                MeterDto meterData = new MeterDto();
                Dispatcher.CurrentDispatcher.Invoke(async () =>
                {
                    meterData = await GetMeterDetailsByMeterNo(MeterNo);
                });

                List<MeterDto> meterList = new List<MeterDto>();

                if (meterData != null)
                {
                    meterList.Add(meterData);
                }

                //Creating Meter Header DataTable
                DataTable meterDt = ToDataTable(meterList);

                //Exporting to Pdf
                string downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);
                string folderPath = downloadsPath + "\\CrystalPowerBCSExport\\";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                FileName = MeterNo + "_" + FileName;

                if (File.Exists(folderPath + FileName + ".pdf"))
                {
                    FileName = FileName + "_" + Guid.NewGuid().ToString();
                }
                Document document;
                if (MeterType == Constants.ThreePhaseMeter)
                {
                    document = new Document(iTextSharp.text.PageSize.A4.Rotate());
                }
                else
                {
                    document = new Document();
                }


                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(folderPath + FileName + ".pdf", FileMode.Create));
                document.Open();


                iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);


                var header = new Paragraph("Sinhal Udyog Pvt Ltd.");

                header.Alignment = Element.ALIGN_CENTER;
                document.Add(header);
                var gridHeader = new Paragraph(DataGridName);
                gridHeader.Alignment = Element.ALIGN_CENTER;
                document.Add(gridHeader);
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph("Meter Details"));
                document.Add(new Paragraph(" "));
                document.Add(ExportHelper.CreateTable(meterDt));

                if (MeterType == Constants.SinglePhaseMeter)
                {
                    if (InstantaneousProfileSinglePhase != null && InstantaneousProfileSinglePhase.Count > 0)
                    {
                        DataTable billingDtPdf = new();

                        billingDtPdf = ToDataTable(InstantaneousProfileSinglePhase);
                        document.Add(new Paragraph("Instantaneous Profile"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(billingDtPdf));
                    }
                    if (DailyLoadProfileSinglePhaseReport != null && DailyLoadProfileSinglePhaseReport.Count > 0)
                    {
                        DataTable DlpDtPdf = new();

                        DlpDtPdf = ToDataTable(DailyLoadProfileSinglePhaseReport);
                        document.Add(new Paragraph("Daily Load Profile"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(DlpDtPdf));
                    }
                    if (BillingProfileSinglePhase != null && BillingProfileSinglePhase.Count > 0)
                    {
                        DataTable billingDtPdf = new();

                        billingDtPdf = ToDataTableBilling(BillingProfileSinglePhase);
                        document.Add(new Paragraph("Billing Profile"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(billingDtPdf));
                    }

                    if (BlockLoadProfileSinglePhase != null && BlockLoadProfileSinglePhase.Count > 0)
                    {
                        DataTable BLPDtPdf = new();

                        BLPDtPdf = ToDataTable(BlockLoadProfileSinglePhase);
                        document.Add(new Paragraph("Block Load Profile"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(BLPDtPdf));
                    }

                    if (SelfDiagnosticDtos != null && SelfDiagnosticDtos.Count > 0)
                    {
                        DataTable BLPDtPdf = new();

                        BLPDtPdf = ToDataTable(SelfDiagnosticDtos);
                        document.Add(new Paragraph("Self Diagnostic"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(BLPDtPdf));
                    }

                    if (AllEventsSinglePhaseReport != null && AllEventsSinglePhaseReport.Count > 0)
                    {
                        DataTable BLPDtPdf = new();

                        BLPDtPdf = ToDataTable(AllEventsSinglePhaseReport);
                        document.Add(new Paragraph("All Event Single Phase"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(BLPDtPdf));
                    }

                }
                else if (MeterType == Constants.ThreePhaseMeter)
                {
                    if (InstantaneousProfileThreePhase != null && InstantaneousProfileThreePhase.Count > 0)
                    {
                        DataTable billingDtPdf = new();

                        billingDtPdf = ToDataTable(InstantaneousProfileThreePhase);
                        document.Add(new Paragraph("Instantaneous Profile Three Phase"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(billingDtPdf));
                    }
                    if (DailyLoadProfileThreePhaseReport != null && DailyLoadProfileThreePhaseReport.Count > 0)
                    {
                        DataTable DlpDtPdf = new();

                        DlpDtPdf = ToDataTable(DailyLoadProfileThreePhaseReport);
                        document.Add(new Paragraph("Daily Load Profile Three Phase"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(DlpDtPdf));
                    }

                    if (BillingProfileThreePhase != null && BillingProfileThreePhase.Count > 0)
                    {
                        DataTable billingDtPdf = new();

                        billingDtPdf = ToDataTableBilling(BillingProfileThreePhase);
                        document.Add(new Paragraph("Billing Profile Three Phase"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(billingDtPdf));
                    }

                    if (BlockLoadProfileThreePhase != null && BlockLoadProfileThreePhase.Count > 0)
                    {
                        DataTable BLPDtPdf = new();

                        BLPDtPdf = ToDataTable(BlockLoadProfileThreePhase);
                        document.Add(new Paragraph("Block Load Profile Three Phase"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(BLPDtPdf));
                    }

                    if (SelfDiagnosticDtos != null && SelfDiagnosticDtos.Count > 0)
                    {
                        DataTable BLPDtPdf = new();

                        BLPDtPdf = ToDataTable(SelfDiagnosticDtos);
                        document.Add(new Paragraph("Self Diagnostic"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(BLPDtPdf));
                    }

                    if (AllEventsThreePhaseReport != null && AllEventsThreePhaseReport.Count > 0)
                    {
                        DataTable BLPDtPdf = new();

                        BLPDtPdf = ToDataTable(AllEventsThreePhaseReport);
                        document.Add(new Paragraph("All Event Three Phase"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(BLPDtPdf));
                    }
                }
                else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                {
                    if (InstantaneousProfileThreePhaseCT != null && InstantaneousProfileThreePhaseCT.Count > 0)
                    {
                        DataTable billingDtPdf = new();

                        billingDtPdf = ToDataTable(InstantaneousProfileThreePhaseCT);
                        document.Add(new Paragraph("Instantaneous Profile Three Phase"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(billingDtPdf));
                    }
                    if (DailyLoadProfileThreePhaseCTReport != null && DailyLoadProfileThreePhaseCTReport.Count > 0)
                    {
                        DataTable DlpDtPdf = new();

                        DlpDtPdf = ToDataTable(DailyLoadProfileThreePhaseCTReport);
                        document.Add(new Paragraph("Daily Load Profile Three Phase"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(DlpDtPdf));
                    }

                    if (BillingProfileThreePhaseCT != null && BillingProfileThreePhaseCT.Count > 0)
                    {
                        DataTable billingDtPdf = new();

                        billingDtPdf = ToDataTableBilling(BillingProfileThreePhaseCT);
                        document.Add(new Paragraph("Billing Profile Three Phase"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(billingDtPdf));
                    }

                    if (BlockLoadProfileThreePhaseCT != null && BlockLoadProfileThreePhaseCT.Count > 0)
                    {
                        DataTable BLPDtPdf = new();

                        BLPDtPdf = ToDataTable(BlockLoadProfileThreePhaseCT);
                        document.Add(new Paragraph("Block Load Profile Three Phase"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(BLPDtPdf));
                    }

                    if (SelfDiagnosticDtos != null && SelfDiagnosticDtos.Count > 0)
                    {
                        DataTable BLPDtPdf = new();

                        BLPDtPdf = ToDataTable(SelfDiagnosticDtos);
                        document.Add(new Paragraph("Self Diagnostic"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(BLPDtPdf));
                    }

                    if (AllEventsThreePhaseReport != null && AllEventsThreePhaseReport.Count > 0)
                    {
                        DataTable BLPDtPdf = new();

                        BLPDtPdf = ToDataTable(AllEventsThreePhaseReport);
                        document.Add(new Paragraph("All Event Three Three Phase"));
                        document.Add(new Paragraph(" "));
                        document.Add(ExportHelper.CreateTable(BLPDtPdf));
                    }
                }

                document.Close();

                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification, DataGridName + " Exported SuccessFully", NotificationType.Success, CloseOnClick: true);

                Process.Start("explorer.exe", folderPath);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static bool GenerateReportsExcel(List<InstantaneousProfileSinglePhaseDto> InstantaneousProfileSinglePhase, List<BillingProfileSinglePhaseDto> BillingProfileSinglePhase, List<BlockLoadProfileSinglePhaseDto> BlockLoadProfileSinglePhase, List<DailyLoadProfileSinglePhaseDto> DailyLoadProfileSinglePhaseReport, List<InstantaneousProfileThreePhaseDto> InstantaneousProfileThreePhase, List<BillingProfileThreePhaseDto> BillingProfileThreePhase, List<BlockLoadProfileThreePhaseDto> BlockLoadProfileThreePhase, List<DailyLoadProfileThreePhaseDto> DailyLoadProfileThreePhaseReport, List<InstantaneousProfileThreePhaseCTDto> InstantaneousProfileThreePhaseCT, List<BillingProfileThreePhaseCTDto> BillingProfileThreePhaseCT, List<BlockLoadProfileThreePhaseCTDto> BlockLoadProfileThreePhaseCT, List<DailyLoadProfileThreePhaseCTDto> DailyLoadProfileThreePhaseCTReport, List<AllEventsSinglePhaseDto> AllEventsSinglePhaseReport, List<AllEventsDTO> AllEventsThreePhaseReport,List<SelfDiagnosticDto> selfDiagnosticsReport, string FileName, string DataGridName, string MeterNo, string MeterType)
        {
            try
            {
                MeterDto meterData = new MeterDto();
                Dispatcher.CurrentDispatcher.Invoke(async () =>
                {
                    meterData = await GetMeterDetailsByMeterNo(MeterNo);
                });

                List<MeterDto> meterList = new List<MeterDto>();

                if (meterData != null)
                {
                    meterList.Add(meterData);

                }

                //Creating Meter Header DataTable
                DataTable mData = ToDataTable(meterList);

                string folderPath = "";
                XLWorkbook wb = new XLWorkbook();

                //Exporting to Excel
                string downloadsPath = KnownFolders.GetPath(KnownFolder.Downloads);
                folderPath = downloadsPath + "\\CrystalPowerBCSExport\\";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                FileName = MeterNo + "_" + FileName;

                if (File.Exists(folderPath + FileName + ".xlsx"))
                {
                    FileName = FileName + "_" + Guid.NewGuid().ToString();
                }

                if (MeterType == Constants.SinglePhaseMeter)
                {
                    if (InstantaneousProfileSinglePhase != null && InstantaneousProfileSinglePhase.Count > 0)
                    {
                        DataTable billingDtPdf = new();

                        billingDtPdf = ToDataTable(InstantaneousProfileSinglePhase);
                        ExportHelper.CreateExcel(wb, mData, billingDtPdf, "IP Single Phase");
                    }
                    if (DailyLoadProfileSinglePhaseReport != null && DailyLoadProfileSinglePhaseReport.Count > 0)
                    {
                        DataTable DlpDtPdf = new();

                        DlpDtPdf = ToDataTable(DailyLoadProfileSinglePhaseReport);
                        ExportHelper.CreateExcel(wb, mData, DlpDtPdf, "Daily Load Single Phase");
                    }
                    if (BillingProfileSinglePhase != null && BillingProfileSinglePhase.Count > 0)
                    {
                        DataTable billingDtPdf = new();

                        billingDtPdf = ToDataTable(BillingProfileSinglePhase);
                        ExportHelper.CreateExcel(wb, mData, billingDtPdf, "Billing  Single Phase");
                    }

                    if (BlockLoadProfileSinglePhase != null && BlockLoadProfileSinglePhase.Count > 0)
                    {
                        DataTable BLPDtPdf = new();

                        BLPDtPdf = ToDataTable(BlockLoadProfileSinglePhase);
                        ExportHelper.CreateExcel(wb, mData, BLPDtPdf, "BlockLoad Single Phase");
                    }

                    if (selfDiagnosticsReport != null && selfDiagnosticsReport.Count > 0)
                    {
                        DataTable SelfDiagnosis = new();

                        SelfDiagnosis = ToDataTable(selfDiagnosticsReport);
                        ExportHelper.CreateExcel(wb, mData, SelfDiagnosis, "Self Diagnosis");
                    }

                    if (AllEventsSinglePhaseReport != null && AllEventsSinglePhaseReport.Count > 0)
                    {
                        DataTable SinglePhaseEvent = new();

                        SinglePhaseEvent = ToDataTable(AllEventsSinglePhaseReport);
                        ExportHelper.CreateExcel(wb, mData, SinglePhaseEvent, "All Events Single Phase");
                    }
                }
                else if (MeterType == Constants.ThreePhaseMeter)
                {
                    if (InstantaneousProfileThreePhase != null && InstantaneousProfileThreePhase.Count > 0)
                    {
                        DataTable billingDtPdf = new();

                        billingDtPdf = ToDataTable(InstantaneousProfileThreePhase);
                        ExportHelper.CreateExcel(wb, mData, billingDtPdf, "IP Three Phase");
                    }
                    if (DailyLoadProfileThreePhaseReport != null && DailyLoadProfileThreePhaseReport.Count > 0)
                    {
                        DataTable DlpDtPdf = new();

                        DlpDtPdf = ToDataTable(DailyLoadProfileThreePhaseReport);
                        ExportHelper.CreateExcel(wb, mData, DlpDtPdf, "Daily Load Three Phase");
                    }

                    if (BillingProfileThreePhase != null && BillingProfileThreePhase.Count > 0)
                    {
                        DataTable billingDtPdf = new();
                        billingDtPdf = ToDataTable(BillingProfileThreePhase);
                        ExportHelper.CreateExcel(wb, mData, billingDtPdf, "Billing Three Phase");
                    }

                    if (BlockLoadProfileThreePhase != null && BlockLoadProfileThreePhase.Count > 0)
                    {
                        DataTable BLPDtPdf = new();
                        BLPDtPdf = ToDataTable(BlockLoadProfileThreePhase);
                        ExportHelper.CreateExcel(wb, mData, BLPDtPdf, "Block Load Three Phase");
                    }

                    if (selfDiagnosticsReport != null && selfDiagnosticsReport.Count > 0)
                    {
                        DataTable SelfDiagnosis = new();

                        SelfDiagnosis = ToDataTable(selfDiagnosticsReport);
                        ExportHelper.CreateExcel(wb, mData, SelfDiagnosis, "Self Diagnosis");
                    }

                    if (AllEventsThreePhaseReport != null && AllEventsThreePhaseReport.Count > 0)
                    {
                        DataTable ThreePhaseEvent = new();

                        ThreePhaseEvent = ToDataTable(AllEventsThreePhaseReport);
                        ExportHelper.CreateExcel(wb, mData, ThreePhaseEvent, "All Events Three Phase");
                    }
                }
                else if (MeterType == Constants.ThreePhaseLTCT || MeterType == Constants.ThreePhaseHTCT)
                {
                    if (InstantaneousProfileThreePhaseCT != null && InstantaneousProfileThreePhaseCT.Count > 0)
                    {
                        DataTable billingDtPdf = new();

                        billingDtPdf = ToDataTable(InstantaneousProfileThreePhaseCT);
                        ExportHelper.CreateExcel(wb, mData, billingDtPdf, "IP Three Phase CT");
                    }
                    if (DailyLoadProfileThreePhaseCTReport != null && DailyLoadProfileThreePhaseCTReport.Count > 0)
                    {
                        DataTable DlpDtPdf = new();

                        DlpDtPdf = ToDataTable(DailyLoadProfileThreePhaseCTReport);
                        ExportHelper.CreateExcel(wb, mData, DlpDtPdf, "Daily Load Three Phase CT");
                    }

                    if (BillingProfileThreePhaseCT != null && BillingProfileThreePhaseCT.Count > 0)
                    {
                        DataTable billingDtPdf = new();
                        billingDtPdf = ToDataTable(BillingProfileThreePhaseCT);
                        ExportHelper.CreateExcel(wb, mData, billingDtPdf, "Billing Three Phase CT");
                    }

                    if (BlockLoadProfileThreePhaseCT != null && BlockLoadProfileThreePhaseCT.Count > 0)
                    {
                        DataTable BLPDtPdf = new();
                        BLPDtPdf = ToDataTable(BlockLoadProfileThreePhaseCT);
                        ExportHelper.CreateExcel(wb, mData, BLPDtPdf, "Block Load Three Phase CT");
                    }

                    if (selfDiagnosticsReport != null && selfDiagnosticsReport.Count > 0)
                    {
                        DataTable SelfDiagnosis = new();

                        SelfDiagnosis = ToDataTable(selfDiagnosticsReport);
                        ExportHelper.CreateExcel(wb, mData, SelfDiagnosis, "Self Diagnosis");
                    }

                    if (AllEventsThreePhaseReport != null && AllEventsThreePhaseReport.Count > 0)
                    {
                        DataTable ThreePhaseEvent = new();

                        ThreePhaseEvent = ToDataTable(AllEventsThreePhaseReport);
                        ExportHelper.CreateExcel(wb, mData, ThreePhaseEvent, "All Events Three Phase CT");
                    }
                }

                wb.SaveAs(folderPath + FileName + ".xlsx");
                NotificationManager notificationManager = new NotificationManager();

                notificationManager.Show(Constants.Notification, DataGridName + " Exported SuccessFully", NotificationType.Success, CloseOnClick: true);

                Process.Start("explorer.exe", folderPath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static XLWorkbook CreateExcel(XLWorkbook wb, DataTable mData, DataTable meterDt, string SheetName)
        {
            if (meterDt.Rows.Count > 0)
            {
                wb.Worksheets.Add(mData, SheetName);
                var sheetCount = wb.Worksheets.Count();
                wb.Worksheet(1).Columns().AdjustToContents();
                wb.Worksheet(sheetCount).Cell(5, 1).InsertTable(meterDt);

            }
            return wb;
        }

        public static PdfPTable CreateTable(DataTable meterDt)
        {
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);
            PdfPTable table1 = new PdfPTable(meterDt.Columns.Count);
            PdfPRow row1 = null;

            float[] widths1 = new float[meterDt.Columns.Count];
            for (int i = 0; i < meterDt.Columns.Count; i++)
                widths1[i] = 4f;

            table1.SetWidths(widths1);

            table1.WidthPercentage = 100;
            int iCols = 0;
            string colnames = "";

            PdfPCell cells = new PdfPCell(new Phrase("Products"));

            cells.Colspan = meterDt.Columns.Count;

            foreach (DataColumn c in meterDt.Columns)
            {
                table1.AddCell(new Phrase(c.ColumnName, font5));
            }

            foreach (DataRow r in meterDt.Rows)
            {
                if (meterDt.Rows.Count > 0)
                {
                    for (int h = 0; h < meterDt.Columns.Count; h++)
                    {
                        table1.AddCell(new Phrase(r[h].ToString(), font5));
                    }
                }
            }
            return table1;
        }
    }
}
