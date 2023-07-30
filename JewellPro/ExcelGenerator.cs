namespace JewellPro
{
    using Microsoft.Office.Interop.Excel;
    using Spire.Xls;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using Excel = Microsoft.Office.Interop.Excel;

    public class ExcelGenerator
    {
        //private string excelTemplate = "";
        private string currentAppPath = "";
        string excelOutputFilePath = "";
        string excelEstimationSourceFilePath = "";
        string temDirPath = "";
        string excelEstimationOutputFilePath = "";

        public ExcelGenerator() { }

        private Excel.Workbook CreateExcelWorkBook(string filePath, Excel.Application xlApp)
        {
            xlApp.DisplayAlerts = false;
            xlApp.Visible = false;
            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(filePath, 0, false, 5, "", "", false,
                Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            return xlWorkBook;
        }

        private string GetEstimationOutputFilePath()
        {
            try
            {
                currentAppPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                temDirPath = Path.Combine(currentAppPath, "Reports", "Estimation", DateTime.Now.ToString("yyyyMMddHHmmss"));
                Directory.CreateDirectory(temDirPath);

                excelEstimationOutputFilePath = Path.Combine(temDirPath, "Estimation.xlsx");

                excelEstimationSourceFilePath = Path.Combine(currentAppPath, "template", "Estimation.xlsx");
                
                File.Copy(excelEstimationSourceFilePath, excelEstimationOutputFilePath, true);
                return excelEstimationOutputFilePath;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            return string.Empty;
        }

        public ReportStatus GenerateCustomerEstimationInvoice(OrderDetails orderDetails)
        {
            string estimationReportPath = GetEstimationOutputFilePath();

            if (string.IsNullOrWhiteSpace(estimationReportPath))
                throw new Exception("Estimation Report File not generated");

            ReportStatus reportStatus = new ReportStatus();            

            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = CreateExcelWorkBook(estimationReportPath, xlApp);
            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            try
            {
                xlWorkSheet.Range["H2"].Value2 = Convert.ToString(orderDetails.orderDate);
                xlWorkSheet.Range["E3"].Value2 = Convert.ToString(orderDetails.customer.name);
                xlWorkSheet.Range["E4"].Value2 = Convert.ToString(orderDetails.customer.address);
                xlWorkSheet.Range["E5"].Value2 = "Ph: +91 " +Convert.ToString(orderDetails.customer.mobile);// + "\n" + excelFileArgs.selectedCustomer.mobile);

                int sno = 1;
                int rowno = 9;
                int totalQuantity = 0;
                decimal totalGoldCharges = 0;
                decimal totalEstimatedValue = 0;
                decimal totalGoldWeight = 0;

                foreach (OrderDetails order in orderDetails.orderDetailsList)
                {
                    xlWorkSheet.Range["A" + rowno.ToString()].Value2 = Convert.ToString(sno);
                    xlWorkSheet.Range["B" + rowno.ToString()].Value2 = Convert.ToString(order.jewelType.name);
                    order.quantity = string.IsNullOrWhiteSpace(order.quantity) ? Convert.ToString("1") : order.quantity;
                    xlWorkSheet.Range["C" + rowno.ToString()].Value2 = Convert.ToString(order.quantity);
                    totalQuantity = totalQuantity + Convert.ToInt16(order.quantity);

                    xlWorkSheet.Range["D" + rowno.ToString()].Value2 = Convert.ToString(order.jewelPurity);
                    xlWorkSheet.Range["E" + rowno.ToString()].Value2 = Convert.ToString(order.netWeight)+"g";
                    totalGoldWeight = totalGoldWeight + (Convert.ToDecimal(order.quantity) * Convert.ToDecimal(order.netWeight));


                    decimal goldCharge = Helper.GetGoldCharges(order);
                    totalGoldCharges = totalGoldCharges + goldCharge;
                    xlWorkSheet.Range["F" + rowno.ToString()].Value2 = Helper.FormatRupees(goldCharge);

                    xlWorkSheet.Range["G" + rowno.ToString()].Value2 = Convert.ToString(order.wastage) + "%";

                    decimal estmateCharge = Helper.GetEstimatedValue(order);
                    totalEstimatedValue = totalEstimatedValue + estmateCharge;
                    xlWorkSheet.Range["H" + rowno.ToString()].Value2 = Helper.FormatRupees(estmateCharge);

                    rowno = rowno + 1;
                    sno = sno + 1;
                }
                xlWorkSheet.Range["C16"].Value2 = Convert.ToString(totalQuantity);
                xlWorkSheet.Range["E16"].Value2 = Convert.ToString(totalGoldWeight);
                xlWorkSheet.Range["H16"].Value2 = Helper.FormatRupees(totalEstimatedValue);

                StringBuilder rateFrozen = new StringBuilder();
                if(orderDetails.isRateFreeze)
                {
                    rateFrozen.AppendLine("Rate Frozen: Yes | Frozen Date: " + Convert.ToString(orderDetails.rateFreezeDate)+ " | Customer Signature: ");
                }
                else
                {
                    rateFrozen.AppendLine(Convert.ToString("Rate Frozen: No | Frozen Date: Nil | Customer Signature: Nil"));
                }
                xlWorkSheet.Range["A7"].Value2 = rateFrozen.ToString();

                StringBuilder rateDescription = new StringBuilder();
                rateDescription.Append("Standard Gold Rate: ");
                foreach (Rate rate in Helper.GetStandardMetalRates())
                {
                    if (rate.isChecked)
                    {
                        rateDescription.Append(rate.description + " – Rs. " +rate.rate.ToString().Trim() + "/- | ");
                    }
                }
                
                xlWorkSheet.Range["A6"].Value2 = rateDescription.ToString().TrimEnd('|', ' '); 
                //"Standard Gold Rate: 22 Karat (91.6% Purity) – Rs. 4,885/- | Gold Rate: (85% Purity) – Rs. 4,530/- Gold";

                xlWorkBook.SaveAs(estimationReportPath, Excel.XlFileFormat.xlOpenXMLWorkbook,
                    Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange,
                    Excel.XlSaveConflictResolution.xlLocalSessionChanges, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value);
                reportStatus.status = true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                reportStatus.status = false;
                reportStatus.errorInfo = ex.ToString();
            }
            finally
            {
                ReleaseExcelComObject(xlApp, xlWorkBook, xlWorkSheet);
            }

            if(reportStatus.status)
            {
                try
                {
                    string estimationPdfPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Estimation", orderDetails.orderRefNo + ".pdf");
                    Spire.Xls.Workbook workbook1 = new Spire.Xls.Workbook();
                    workbook1.LoadFromFile(estimationReportPath);
                    Spire.Xls.Worksheet sheet = workbook1.Worksheets[0];
                    sheet.SaveToPdf(estimationPdfPath);

                    reportStatus.reportPath = estimationPdfPath;
                    reportStatus.status = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                    reportStatus.status = false;
                    reportStatus.errorInfo = ex.ToString();
                }
            }
            return reportStatus;
        }

        public bool GenerateCustomerOrderDeliveryInvoice(ExcelFileArgs excelFileArgs)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = CreateExcelWorkBook(excelOutputFilePath, xlApp);
            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);

            xlWorkSheet.Cells[7, 1] = Convert.ToString("Name : " + excelFileArgs.selectedCustomer.name);
            xlWorkSheet.Cells[5, 6] = Convert.ToString("Order No : " + excelFileArgs.selectedCustomerOrder.orderRefNo);
            xlWorkSheet.Cells[6, 6] = Convert.ToString("Order Date : " + excelFileArgs.selectedCustomerOrder.orderDate);
            xlWorkSheet.Cells[7, 6] = Convert.ToString("Delivery Date : " + DateTime.Today.ToString("dd-MM-yyyy"));

            int sNo = 0;
            int orderInfoStartIndex = 10;
            foreach (OrderDetails order in excelFileArgs.orderDetails)
            {
                sNo += 1;
                orderInfoStartIndex = orderInfoStartIndex + 1;
                int tempStartIndex = orderInfoStartIndex;

                xlWorkSheet.Cells[orderInfoStartIndex, 1] = Convert.ToString(sNo);

                //JewelWeight
                xlWorkSheet.Range[xlWorkSheet.Cells[orderInfoStartIndex, 4], xlWorkSheet.Cells[orderInfoStartIndex, 5]].Merge(true);
                xlWorkSheet.Cells[orderInfoStartIndex, 4] = "Jewel Weight : ";
                xlWorkSheet.Cells[orderInfoStartIndex, 6] = Convert.ToString(order.jewellRecivedWeight);

                //Check for detection
                foreach (DetectionControl detection in order.detectionDetails)
                {
                    if (!string.IsNullOrEmpty(detection.value))
                    {
                        orderInfoStartIndex = orderInfoStartIndex + 1;
                        xlWorkSheet.Range[xlWorkSheet.Cells[orderInfoStartIndex, 4], xlWorkSheet.Cells[orderInfoStartIndex, 5]].Merge(true);
                        xlWorkSheet.Cells[orderInfoStartIndex, 4] = detection.description;
                        xlWorkSheet.Cells[orderInfoStartIndex, 6] = Convert.ToString(detection.value);
                    }
                }

                //Wastage %:
                orderInfoStartIndex = orderInfoStartIndex + 1;
                xlWorkSheet.Range[xlWorkSheet.Cells[orderInfoStartIndex, 4], xlWorkSheet.Cells[orderInfoStartIndex, 5]].Merge(true);
                xlWorkSheet.Cells[orderInfoStartIndex, 4] = "Wastage : " + Convert.ToString(order.wastagePercentage) + "% ";
                xlWorkSheet.Cells[orderInfoStartIndex, 6] = Convert.ToString(Helper.GetWastage(order));

                //Pure Weight after caluculation
                xlWorkSheet.Range[xlWorkSheet.Cells[tempStartIndex, 7], xlWorkSheet.Cells[orderInfoStartIndex, 7]].Merge(Missing.Value);
                xlWorkSheet.Cells[tempStartIndex, 7] = Convert.ToString(Math.Round((Convert.ToDecimal(order.grandTotal) * Convert.ToDecimal(order.jewelPurity)) / 100, 3));

                //Order info
                StringBuilder orderVal = new StringBuilder();
                orderVal.AppendLine(Convert.ToString(order.jewelType.description));
                orderVal.AppendLine(Convert.ToString(order.description));
                orderVal.AppendLine(Convert.ToString(order.seal));
                orderVal.AppendLine(Convert.ToString(order.jewelPurity));
                xlWorkSheet.Range[xlWorkSheet.Cells[tempStartIndex, 2], xlWorkSheet.Cells[orderInfoStartIndex, 3]].Merge(Missing.Value);
                xlWorkSheet.Cells[tempStartIndex, 2] = orderVal.ToString();

                //Empty Row
                orderInfoStartIndex = orderInfoStartIndex + 1;
                Excel.Range emptyRowRange = xlWorkSheet.Range[xlWorkSheet.Cells[orderInfoStartIndex, 1], xlWorkSheet.Cells[orderInfoStartIndex, 7]];
                emptyRowRange.Merge(Missing.Value);
                emptyRowRange.RowHeight = 7;
            }
            int ordersConsolidatedTotalRowInex = 24;
            xlWorkSheet.Cells[ordersConsolidatedTotalRowInex, 7] = Convert.ToString(Helper.GetGrandTotal(excelFileArgs.orderDetails));

            //Update advance details
            if (excelFileArgs.advanceDetails != null && excelFileArgs.advanceDetails.Count > 0)
            {
                UpdateAdvanceDetails(xlWorkSheet, excelFileArgs.advanceDetails, 27);
            }

            xlWorkSheet.Cells[33, 7] = Convert.ToString(Convert.ToDecimal(Helper.GetGrandTotal(excelFileArgs.advanceDetails)) - Convert.ToDecimal(Helper.GetGrandTotal(excelFileArgs.orderDetails)));

            xlWorkBook.SaveAs(excelOutputFilePath, Excel.XlFileFormat.xlOpenXMLWorkbook,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange,
                Excel.XlSaveConflictResolution.xlLocalSessionChanges, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value);

            ReleaseExcelComObject(xlApp, xlWorkBook, xlWorkSheet);
            //Workbook workbook = new Workbook();
            //workbook.LoadFromFile(excelOutputFilePath);
            //workbook.Worksheets[2].SaveToImage(Path.Combine(temDirPath, "result.png"));

            return true;
        }

        public void GenerateEmployeeOrderInvoice(ExcelFileArgs excelFileArgs)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook = CreateExcelWorkBook(excelOutputFilePath, xlApp);
            Excel.Worksheet xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            xlWorkSheet.Cells[6, 1] = Convert.ToString("Name : " + excelFileArgs.selectedEmployee.name);
            xlWorkSheet.Cells[5, 5] = Convert.ToString("Order No : " + excelFileArgs.orderRefNo);
            xlWorkSheet.Cells[6, 5] = Convert.ToString("Order Date : " + excelFileArgs.orderDate);

            int sNo = 0;
            int orderInfoStartIndex = 9;
            int orderTempIndex = 0;
            //int orderInfoImageIndex = 54;
            foreach (OrderDetails order in excelFileArgs.orderDetails)
            {
                sNo += 1;
                xlWorkSheet.Cells[orderInfoStartIndex, 1] = Convert.ToString(sNo);
                orderTempIndex = orderInfoStartIndex;

                //Order info
                StringBuilder orderVal = new StringBuilder();
                orderVal.AppendLine(Convert.ToString("Jewel Type : " + order.jewelType.description));
                orderVal.AppendLine(Convert.ToString("Jewel Purity : " + order.jewelPurity));
                orderVal.AppendLine(Convert.ToString("Weight : " + order.netWeight));
                orderInfoStartIndex += 3;
                
                if (!string.IsNullOrEmpty(order.seal))
                {
                    orderVal.AppendLine(Convert.ToString("Seal : " + order.seal));
                    orderInfoStartIndex += 1;
                }

                if (!string.IsNullOrEmpty(order.size))
                {
                    orderVal.AppendLine(Convert.ToString("Size \\ Length : " + order.size));
                    orderInfoStartIndex += 1;
                }

                if (!string.IsNullOrEmpty(order.description))
                {
                    orderVal.AppendLine(Convert.ToString("Description : " + Helper.Truncate(order.description)));
                    orderInfoStartIndex += 1;
                }
                //Merge Description
                xlWorkSheet.Range[xlWorkSheet.Cells[orderTempIndex, 2], xlWorkSheet.Cells[orderInfoStartIndex, 4]].Merge();
                xlWorkSheet.Range[xlWorkSheet.Cells[orderTempIndex, 2], xlWorkSheet.Cells[orderInfoStartIndex, 4]].Value = orderVal.ToString();
                
                //Merge S.No
                //Excel.Range SNoRowRange = xlWorkSheet.Range[xlWorkSheet.Cells[orderTempIndex, 1], xlWorkSheet.Cells[orderInfoStartIndex, 1]].Merge(Missing.Value);

                

                if (!string.IsNullOrEmpty(order.attachement))
                {
                    string srcImgPath = Path.Combine(order.attachementPath, order.attachement);
                    string destImgPath = Path.Combine(order.attachementPath, "CONV_" + order.attachement);
                    Helper.SaveImageForFixedSize(srcImgPath, destImgPath, 140, 120);

                    
                    xlWorkSheet.Range[xlWorkSheet.Cells[orderTempIndex, 5], xlWorkSheet.Cells[orderInfoStartIndex, 7]].Merge(Missing.Value);
                    Excel.Range oRange = xlWorkSheet.Range[xlWorkSheet.Cells[orderTempIndex, 5], xlWorkSheet.Cells[orderInfoStartIndex, 7]];
                    System.Drawing.Image oImage = System.Drawing.Image.FromFile(destImgPath);
                    System.Windows.Forms.Clipboard.SetDataObject(oImage, true);
                    xlWorkSheet.Paste(oRange, destImgPath);

                    //Excel.Range oRange = (Excel.Range)xlWorkSheet.Cells[orderInfoImageIndex, 2];
                    //System.Drawing.Image oImage = System.Drawing.Image.FromFile(Path.Combine(order.attachementPath, order.attachement));
                    //System.Windows.Forms.Clipboard.SetDataObject(oImage, true);
                    //xlWorkSheet.Paste(oRange, Path.Combine(order.attachementPath, order.attachement));
                    //orderInfoImageIndex = orderInfoImageIndex + 10;

                    //xlWorkSheet.Shapes.AddPicture(srcImgPath, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 400, 200, 300, 45);
                }
            }

            orderInfoStartIndex += 1;
            //Update advance details
            if (excelFileArgs.advanceDetails != null && excelFileArgs.advanceDetails.Count > 0)
            {
                UpdateAdvanceDetails(xlWorkSheet, excelFileArgs.advanceDetails, 37);
            }

            xlWorkBook.SaveAs(excelOutputFilePath, Excel.XlFileFormat.xlOpenXMLWorkbook,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value, Excel.XlSaveAsAccessMode.xlNoChange,
                Excel.XlSaveConflictResolution.xlLocalSessionChanges, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value);

            ReleaseExcelComObject(xlApp, xlWorkBook, xlWorkSheet);
            GenerateToPDF(excelFileArgs.orderRefNo +".pdf", excelOutputFilePath, 0);
        }

        private void GenerateToPDF(string fileName, string filePath, int sheetNo)
        {
            
        }

        public void GenerateCustomerOrderInvoice(ExcelFileArgs excelFileArgs)
        {
            throw new NotImplementedException();
        }

        private void UpdateAdvanceDetails(Excel.Worksheet xlWorkSheet, ObservableCollection<AdvanceDetails> advanceDetails, int index)
        {
            int advanceStartRow = index;
            int sNo = 1;
            foreach (AdvanceDetails advance in advanceDetails)
            {
                xlWorkSheet.Cells[advanceStartRow, 1] = Convert.ToString(sNo);
                xlWorkSheet.Cells[advanceStartRow, 2] = Convert.ToString(advance.goldPurity);
                xlWorkSheet.Cells[advanceStartRow, 3] = Convert.ToString(advance.goldWeight);
                xlWorkSheet.Cells[advanceStartRow, 5] = Convert.ToString(advance.pureGoldWeight);
                advanceStartRow += 1;
                sNo += 1;
            }
            int advanceGrandTotal = index + 5;
            xlWorkSheet.Cells[advanceGrandTotal, 7] = Convert.ToString(Helper.GetGrandTotal(advanceDetails));
        }

        private void ReleaseExcelComObject(Excel.Application xlApp, Excel.Workbook xlWorkBook, Excel.Worksheet xlWorkSheet)
        {
            xlWorkBook.Close();
            xlApp.Quit();

            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(xlApp);
        }

        Dictionary<string, string> GetCustomerEstimationKeyValue()
        {
            Dictionary<string, string> customerEstimation = new Dictionary<string, string>();
            customerEstimation.Add("Date", "H2");
            customerEstimation.Add("CustomerAddress", "E4");
            customerEstimation.Add("CustomerPhone", "E5");
            
            customerEstimation.Add("GoldRateInfo", "A6");
            customerEstimation.Add("RateFrozenInfo", "A7");

            customerEstimation.Add("TotalItems", "C17");
            customerEstimation.Add("TotalWeight", "E17");
            customerEstimation.Add("TotalWeight", "H17");
            return customerEstimation;
        }
    }
}







