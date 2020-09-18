using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestTask1_ChuchumakovEV
{
    public static class XLSX_Methods
    {
        private static readonly string EnglishAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static void Create_Cell(Worksheet worksheet, int NumbColumn, int NumbRow, string Formula, string Custom, int Size, bool IsBold, TextAlignmentType TextAlignmentType, Color BackgroundColor)
        {
            Cell cell = worksheet.Cells[Get_CellName(NumbColumn, NumbRow)];
            {
                Style style = cell.GetStyle();
                style.Font.Size = Size;
                style.Font.IsBold = IsBold;
                style.HorizontalAlignment = TextAlignmentType;
                style.Custom = Custom;

                if (BackgroundColor != default(Color))
                {
                    style.Pattern = BackgroundType.Solid;
                    style.ForegroundColor = BackgroundColor;
                }
                
                cell.SetStyle(style);
            } // Style
            cell.Formula = Formula;
        }
        private static string Get_CellName(int NumbColumn, int NumbRow)
        {
            return EnglishAlphabet.ToCharArray()[NumbColumn - 1] + "" + NumbRow;
        }
        public static Workbook Create_XLSX(int Columns, out List<Journal_List> Journal_Out, [Optional] List<Journal_List> Journal)
        {
            DateTime Start = DateTime.Now;

            if (Journal is null)
                Journal = new List<Journal_List>();

            Journal_Out = Journal;
            Journal.Add(new Journal_List()
            {
                Method = MethodBase.GetCurrentMethod().Name + ": Start",
                Status = Journal_List.Status_Done,
                Console_WriteLine = null
            });

            Workbook workbook = null;

            try
            {
                workbook = new Workbook();
                var worksheet = workbook.Worksheets[0];
                worksheet.Name = "ЗАКЛАДКА";

                for (int i = 1; i <= Columns; i++)
                {
                    Create_Cell(worksheet, i, 1, i + "", null, 14, true, TextAlignmentType.Center, default); // Header
                    Create_Cell(worksheet, i, 2, string.Format("{0} + 1", Get_CellName(i, 1)), "#.00", 12, false, TextAlignmentType.Left, default); // First row
                    Create_Cell(worksheet, i, 3, string.Format("{0} + 1", Get_CellName(i, 2)), "#.00", 12, false, TextAlignmentType.Left, i == 5 ? Color.Yellow : default); // Second row
                    Create_Cell(worksheet, i, 4, string.Format("{0} + 1", Get_CellName(i, 3)), "#.00", 12, false, TextAlignmentType.Left, default); // Third row
                    Create_Cell(worksheet, i, 5, string.Format("{0} + {1} + {2}", Get_CellName(i, 2), Get_CellName(i, 3), Get_CellName(i, 4)), null, 12, true, TextAlignmentType.Left, default); // Final row
                }

                Journal.Add(new Journal_List()
                {
                    Method = MethodBase.GetCurrentMethod().Name + ": End",
                    Status = Journal_List.Status_Done,
                    TimeSpend = DateTime.Now - Start,
                    Console_WriteLine = null
                });
            }
            catch (Exception ex)
            {
                Journal.Add(new Journal_List()
                {
                    Method = MethodBase.GetCurrentMethod().Name + ": End",
                    Status = Journal_List.Status_Failed,
                    TimeSpend = DateTime.Now - Start,
                    Exception = ex.Message.ToString(),
                    Console_WriteLine = null
                });
            }

            return workbook;           
        }
        public static void Create_Archive(Workbook workbook, out string New_zipPath, out List<Journal_List> Journal_Out, [Optional] List<Journal_List> Journal)
        {
            DateTime Start = DateTime.Now;
            New_zipPath = null;

            if (Journal is null)
                Journal = new List<Journal_List>();

            Journal_Out = Journal;
            Journal.Add(new Journal_List()
            {
                Method = MethodBase.GetCurrentMethod().Name + ": Start",
                Status = Journal_List.Status_Done,
                Console_WriteLine = null
            });

            try
            {
                string GUID = Guid.NewGuid().ToString();
                string Direct = string.Format(@"c:\!\{0}\", DateTime.UtcNow.ToString("ddMMyy-HHmmss"));

                if (!Directory.Exists(Direct))
                    Directory.CreateDirectory(Direct);

                string Excel_FilePath = Direct + GUID + ".Xlsx";
                workbook.Save(Excel_FilePath, SaveFormat.Xlsx);
                workbook.Dispose();
                string zipPath = @"c:\!\" + string.Format("{0}.zip", GUID);
                New_zipPath = Direct + string.Format("{0}.zip", GUID);
                ZipFile.CreateFromDirectory(Direct, zipPath);
                File.Move(zipPath, New_zipPath);
                File.Delete(Excel_FilePath);

                Journal.Add(new Journal_List()
                {
                    Method = MethodBase.GetCurrentMethod().Name + ": End",
                    Status = Journal_List.Status_Done,
                    TimeSpend = DateTime.Now - Start,
                    Console_WriteLine = null
                });
            }
            catch (Exception ex)
            {
                Journal.Add(new Journal_List()
                {
                    Method = MethodBase.GetCurrentMethod().Name + ": End",
                    Status = Journal_List.Status_Failed,
                    TimeSpend = DateTime.Now - Start,
                    Exception = ex.Message.ToString(),
                    Console_WriteLine = null
                });
            }
        }
        public static void Create_Journal(string FromPath, List<Journal_List> Journal)
        {
            try
            {
                if ((Directory.Exists(FromPath)|| File.Exists(FromPath)) & Journal is null ? false : Journal.Count > 0)
                {
                    string LogPath = Path.GetDirectoryName(FromPath) + "\\Log.txt";

                    if (!File.Exists(LogPath))
                        using (FileStream fs = File.Create(LogPath))
                        {
                            foreach (var Record in Journal)
                            {
                                Byte[] title = new UTF8Encoding(true).GetBytes(Record.Console_WriteLine);
                                fs.Write(title, 0, title.Length);
                            }

                            Console.WriteLine("\nJournal save done at: " + LogPath);
                        }
                    else
                        Console.WriteLine("\nJournal save failed. Log already exists");
                }
                else
                    Console.WriteLine("\nJournal save failed. Wrong Path.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nJournal save failed: ", ex.Message.ToString());
            }            
        }
        public static void StartMethod(int Columns)
        {
            DateTime Start = DateTime.Now;
            List<Journal_List> Journal = new List<Journal_List>();
            Journal.Add(new Journal_List()
            {
                Method = MethodBase.GetCurrentMethod().Name + ": Start",
                Status = Journal_List.Status_Done,
                Console_WriteLine = null
            });

            Workbook workbook = XLSX_Methods.Create_XLSX(6, out Journal, Journal);
            XLSX_Methods.Create_Archive(workbook, out string New_zipPath, out Journal, Journal);

            Journal.Add(new Journal_List()
            {
                Method = MethodBase.GetCurrentMethod().Name + ": End",
                Status = Journal.FirstOrDefault(x => x.Status == Journal_List.Status_Failed) is null ? Journal_List.Status_Done : Journal_List.Status_Failed,
                TimeSpend = DateTime.Now - Start,
                Console_WriteLine = null
            });
            Create_Journal(New_zipPath, Journal);
            Console.WriteLine("\nPress any key to close the console.");
            Console.ReadKey();
        }
    }
    public class Journal_List
    {
        public string Method { get; set; }
        public string Status { get; set; }
        public static string Status_Done = "Done";
        public static string Status_Failed = "Failed";
        public string Exception { get; set; }
        public TimeSpan TimeSpend { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        string Message;
        public string Console_WriteLine
        {
            get
            {
                return Message;
            }
            set 
            {
                Message = "\n";
                string Format = "  {0}:[{1}]";

                if (!string.IsNullOrEmpty(Method))
                    Message += string.Format(Format, nameof(Method), Method);

                if (!string.IsNullOrEmpty(Method))
                    Message += string.Format(Format, nameof(Status), Status);

                if (!string.IsNullOrEmpty(Exception))
                    Message += string.Format(Format, nameof(Exception), Exception);

                if (TimeSpend != default(TimeSpan))
                    Message += string.Format(Format, nameof(TimeSpend), TimeSpend);

                if (Date != default(DateTime))
                    Message += string.Format(Format, nameof(Date), Date);

                Console.WriteLine(Message);
            } 
        }
    }
}
