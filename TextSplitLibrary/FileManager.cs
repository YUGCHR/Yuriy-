using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms; //Delete after finishing
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
        string CreateFile(string filesPath, string resultFileName);
    }
    public class FileManager : IFileManager
    {
        //private readonly IMessageService _messageService;        

        public int[] FilesToDo { get; set; }        
        private readonly Encoding _defaultEncoding = Encoding.GetEncoding(1251);
        public string[] FilesPath;
        public string[] FilesContent;        
        private readonly int filesQuantity;//we will reveice the value from Declaration.LanguagesQuantity;
        private string logFilePathName;
        private bool isLogFileExist;        
        string logFilePath = Directory.GetCurrentDirectory();//Will check log-file existing        

        public FileManager(int filesQuantity) //IMessageService service
        {            
            this.filesQuantity = filesQuantity;            
            
            FilesPath = new string[filesQuantity];
            FilesContent = new string[filesQuantity];
            FilesToDo = new int[filesQuantity];
            string[] logFilePathandName = { logFilePath, "log.txt" };
            logFilePathName = String.Join("\\", logFilePathandName);
            isLogFileExist = File.Exists(logFilePathName);

            if (!isLogFileExist)
            {                
                File.AppendAllText(logFilePathName, "LogFile Created \r\n", _defaultEncoding);
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
        public bool IsFilesExist(string filesPath)
        {
            return File.Exists(filesPath);
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
                if (filesToDo[i] == (int)WhatNeedDoWithFiles.ReadFirst)
                {
                    getContents[i] = GetContent(filesPath, i, _defaultEncoding);
                }                    
            }
            return getContents;
        }

        public string GetContent(string[] filesPath, int i, Encoding encoding)//This method cannot be access outside so we do not need to check isFileExist
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
            if (filesContent != null)
            {
                //MessageBox.Show("Start - filesContent not null", "GetSymbolCounts", MessageBoxButtons.OK, MessageBoxIcon.Information);
                int[] counts = new int[filesQuantity];
                for (int i = 0; i < filesQuantity; i++)
                {
                    counts[i] = GetSymbolCounts(filesContent, i);
                }
                return counts;
            }
            else return null;
        }
        public int GetSymbolCounts(string[] filesContent, int i)
        {
            //MessageBox.Show(i.ToString(), "GetSymbolCounts - Last", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (filesContent[i] != null) return filesContent[i].Length;
            else return 0;
        }

        #endregion
        #region AppendContent
        
        public void AppendContent(string tracePointName, string tracePointValue, string tracePointPlace)
        {
            AppendContent(tracePointName, tracePointValue, tracePointPlace, _defaultEncoding);
        }

        public void AppendContent(string tracePointName, string[] tracePointValue, string tracePointPlace)
        {
            AppendContent(tracePointName, tracePointValue, tracePointPlace, _defaultEncoding);
        }

        public void AppendContent(string tracePointName, string tracePointValue, string tracePointPlace, Encoding encoding)
        {
            string[] traceMessage = { "\r\n tracePointPlace - ", tracePointPlace, "\r\n ----------------- tracePointName - ", tracePointName, tracePointValue };//to remove all outsider symbols
            string fileLineAppend = String.Join(" ", traceMessage);            
            File.AppendAllText(logFilePathName, fileLineAppend, encoding);
        }        

        public void AppendContent(string tracePointName, string[] tracePointValue, string tracePointPlace, Encoding encoding)
        {
            int ii = tracePointValue.Length;            

            for (int i = 0; i < ii; i++)
            {
                string tracePointValueCut = tracePointValue[i].Remove(32);
                tracePointValue[i] = tracePointValueCut;
            }
            
            string tracePointValues = String.Join(" *** ", tracePointValue);

            string[] traceMessage = { "\r\n tracePointPlace - ", tracePointPlace, "\r\n --- tracePointName - ", tracePointName, "\r\n" };
            string fileLineAppend = String.Join("", traceMessage);
            File.AppendAllText(logFilePathName, fileLineAppend, encoding);

            traceMessage = new string[] { "", tracePointValues };
            fileLineAppend = String.Join(" ", traceMessage);            
            File.AppendAllText(logFilePathName, fileLineAppend, encoding);
        }
        #endregion
        #region CreateFile
        public string CreateFile(string filesPathSample, string resultFileName)
        {
            //check is path exist
            string logFilePathName = Path.GetDirectoryName(filesPathSample) + "\\" + resultFileName + ".txt";
            if (IsFilesExist(logFilePathName)) return null;//ordered file is exist, cannot create it
            
            string fileLineAppend = "New result file created \r\n";
            File.AppendAllText(logFilePathName, fileLineAppend, _defaultEncoding);
            return logFilePathName;
        }
        #endregion
    }
}


