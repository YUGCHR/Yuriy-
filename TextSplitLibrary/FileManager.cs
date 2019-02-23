using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace TextSplitLibrary
{
    public interface IFileManager
    {        
        string[] GetContents(string[] FilesPath, int[] FilesToDo);
        string[] GetContents(string[] FilesPath, int[] FilesToDo, Encoding encoding);
        //to add third method for one file
        void SaveContents(string[] FilesContent, string[] FilePath, int[] FilesToDo);
        void SaveContents(string[] FilesContent, string[] FilePath, int[] FilesToDo, Encoding encoding);
        //to add third method for one file
        int[] GetSymbolCounts(string[] FilesContent);
        int GetSymbolCounts(string[] FilesContent, int i);
        bool[] IsFilesExist(string[] FilesPath);
        int[] FilesToDo { get; set; }
        void AppendContent(string tracePointName, string tracePointValue, string tracePointPlace);
        void AppendContent(string tracePointName, string tracePointValue, string tracePointPlace, Encoding encoding);
        void AppendContent(string tracePointName, string[] tracePointValue, string tracePointPlace);
        void AppendContent(string tracePointName, string[] tracePointValue, string tracePointPlace, Encoding encoding);
    }
    public class FileManager : IFileManager
    {
        //private readonly IMessageService _messageService;        

        public int[] FilesToDo { get; set; }        
        private readonly Encoding _defaultEncoding = Encoding.GetEncoding(1251);
        public string[] FilesPath;
        public string[] FilesContent;        
        private readonly int filesQuantity;//Declaration.LanguagesQuantity;        
        private string logFilePathName;
        private bool isLogFileExist;
        private string addZeros;
        string logFilePath = Directory.GetCurrentDirectory();//Will check log-file existing        

        public FileManager(int filesQuantity)//IMessageService service
        {
            //_messageService = service;
            this.filesQuantity = filesQuantity;            
            //_messageService.ShowTrace("filesQuantity - ", filesQuantity.ToString(),"FileManager", 1);
            FilesPath = new string[filesQuantity];
            FilesContent = new string[filesQuantity];
            FilesToDo = new int[filesQuantity];
            string[] logFilePathandName = { logFilePath, "log.txt" };
            logFilePathName = String.Join("\\", logFilePathandName);
            isLogFileExist = File.Exists(logFilePathName);

            if (!isLogFileExist)
            {                
                File.AppendAllText(logFilePathName, "0000 LogFile Created \r\n", _defaultEncoding);
            }
            else
            {                
                StreamReader logFileNameNumber = new StreamReader(logFilePathName, _defaultEncoding);
                Console.WriteLine($"logFileNameNumber = {logFileNameNumber}");
                string fisrtFileLine = logFileNameNumber.ReadLine();
                Console.WriteLine($"fisrtFileLine = {fisrtFileLine}");
                string logFileNameNumber4 = fisrtFileLine.Remove(4);
                Console.WriteLine($"logFileNameNumber4 = {logFileNameNumber4}");
                int logFileNameNumberInt = Convert.ToInt32(logFileNameNumber4);
                Console.WriteLine($"logFileNameNumberInt = {logFileNameNumberInt}");
                logFileNameNumberInt++;                
                if (logFileNameNumberInt < 10) addZeros = "000";                
                else if (logFileNameNumberInt < 100) addZeros = "00";
                else if (logFileNameNumberInt < 1000) addZeros = "0";
                else return;
                Console.WriteLine($"logFileNameNumberInt = {logFileNameNumberInt}");
                string nextFileLine = Convert.ToString(logFileNameNumberInt);
                Console.WriteLine($"nextFileLine = {nextFileLine}");
                string nextFileName = "log" + addZeros + nextFileLine + ".txt";
                Console.WriteLine($"nextFileName = {nextFileName}");
                string[] nextFilePathandName = { logFilePath, nextFileName };
                string nextLogFilePathName = String.Join("\\", nextFilePathandName);
                Console.WriteLine($"nextLogFilePathName = {nextLogFilePathName}");
                Console.Read();
                File.AppendAllText(nextLogFilePathName, nextFileLine + " LogFile Created \r\n", _defaultEncoding);
                logFilePathName = nextLogFilePathName;
                Console.Read();
            }
        }
        

        #region IsFileExists
        public bool[] IsFilesExist(string[] filesPath)
        {            
            bool[] isFilesExist = new bool[filesQuantity];
            for (int i = 0; i < filesQuantity; i++)
            {
                string currentFilePath = filesPath[i];
                isFilesExist[i] = File.Exists(currentFilePath);
            }
            return isFilesExist;
        }
        #endregion
        #region GetContent
        public string[] GetContents(string[] filesPath, int[] filesToDo)
        {            
                return GetContents(filesPath, filesToDo, _defaultEncoding); 
        }

        public string[] GetContents(string[] filesPath, int[] filesToDo, Encoding encoding)
        {
            string[] getContents = new string[filesQuantity];
            for (int i = 0; i < filesQuantity; i++)
            {
                if (filesToDo[i] != 0)
                {
                    getContents[i] = GetContent(filesPath, i, _defaultEncoding);
                }                    
            }
            return getContents;
        }

        public string GetContent(string[] filesPath, int i, Encoding encoding)
        {
            return File.ReadAllText(filesPath[i], encoding);            
        }
        #endregion
        #region SaveContent
        public void SaveContents(string[] filesContent, string[] filesPath, int[] filesToDo)
        {
            SaveContents(filesContent, filesPath, filesToDo, _defaultEncoding);
        }

        public void SaveContents(string[] filesContent, string[] filesPath, int[] filesToDo, Encoding encoding)
        {
            for (int i = 0; i < filesQuantity; i++)
            {
                if (filesToDo[i] != 0)
                {
                    SaveContent(filesContent, filesPath, i, _defaultEncoding);
                }
            }            
        }

        public void SaveContent(string[] filesContent, string[] filesPath, int i, Encoding encoding)
        {
            File.WriteAllText(filesPath[i], filesContent[i], encoding);
        }
        #endregion
        #region GetSymbolCount
        public int[] GetSymbolCounts(string[] filesContent)
        {
            int[] counts = new int[filesQuantity];
            for (int i = 0; i < filesQuantity; i++)
            {
                counts[i] = GetSymbolCounts(filesContent, i);
            }            
            return counts;
        }
        public int GetSymbolCounts(string[] filesContent, int i)
        {            
            return filesContent[i].Length;            
        }

        #endregion
        #region AppendContent
        
        public void AppendContent(string tracePointName, string tracePointValue, string tracePointPlace)
        {
            AppendContent(tracePointName, tracePointValue, tracePointPlace, _defaultEncoding);
        }

        public void AppendContent(string tracePointName, string tracePointValue, string tracePointPlace, Encoding encoding)
        {
            string[] traceMessage = { "\r\n tracePointPlace - ", tracePointPlace, "\r\n ----------------- tracePointName - ", tracePointName, tracePointValue };
            string fileLineAppend = String.Join(" ", traceMessage);            
            File.AppendAllText(logFilePathName, fileLineAppend, encoding);
        }

        public void AppendContent(string tracePointName, string[] tracePointValue, string tracePointPlace)
        {
            AppendContent(tracePointName, tracePointValue, tracePointPlace, _defaultEncoding);
        }

        public void AppendContent(string tracePointName, string[] tracePointValue, string tracePointPlace, Encoding encoding)
        {            
            string tracePointValues = String.Join(" *** ", tracePointValue);

            string[] traceMessage = { "\r\n tracePointPlace - ", tracePointPlace, "\r\n --- tracePointName - ", tracePointName, "\r\n" };
            string fileLineAppend = String.Join("", traceMessage);
            File.AppendAllText(logFilePathName, fileLineAppend, encoding);

            traceMessage = new string[] { "", tracePointValues };
            fileLineAppend = String.Join(" ", traceMessage);            
            File.AppendAllText(logFilePathName, fileLineAppend, encoding);
        }
        #endregion
    }
}


