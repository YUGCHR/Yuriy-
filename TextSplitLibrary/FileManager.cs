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
        string GetContent(int i);
        string GetContent(int i, Encoding encoding);        
        int SaveContent(int i);
        int SaveContent(int i, Encoding encoding);        
        int GetSymbolsCount(int i);        
        bool IsFileExist(string FilesPath);
        
        void WriteToFilePathPlus(string[] textParagraphes, string filesPath, string filesPathPlus);
        void AppendContent(string tracePointName, string tracePointValue, string tracePointPlace);
        void AppendContent(string tracePointName, string tracePointValue, string tracePointPlace, Encoding encoding);
        void AppendContent(string tracePointName, string[] tracePointValue, string tracePointPlace);
        void AppendContent(string tracePointName, string[] tracePointValue, string tracePointPlace, Encoding encoding);
        string CreateFile(string filesPath, string resultFileName);
    }
    public class FileManager : IFileManager
    {
        private readonly IAllBookData _book;
        //private readonly IMessageService _messageService;
        private readonly Encoding _defaultEncoding = Encoding.GetEncoding(1251);
        
        private readonly int filesQuantity;//we will reveice the value from Declaration.LanguagesQuantity;
        private string logFilePathName;
        private bool isLogFileExist;        
        string logFilePath = Directory.GetCurrentDirectory();//Will check log-file existing        

        public FileManager(IAllBookData book) //IMessageService service
        {
            _book = book;
            filesQuantity = Declaration.FilesQuantity;
                        
            string[] logFilePathandName = { logFilePath, "log.txt" };
            logFilePathName = String.Join("\\", logFilePathandName);
            isLogFileExist = File.Exists(logFilePathName);

            if (!isLogFileExist)
            {                
                File.AppendAllText(logFilePathName, "LogFile Created \r\n", _defaultEncoding);
            }
            
        }

        
        public bool IsFileExist(string filePath)
        {
            return File.Exists(filePath);
        }        
       
        public string GetContent(int i)
        {            
                return GetContent(i, _defaultEncoding); 
        }

        public string GetContent(int i, Encoding encoding)//This method cannot be access outside so we do not need to check isFileExist
        {
            return File.ReadAllText(_book.GetFilePath(i), encoding);            
        }
        
        public int SaveContent(int i)
        {
            return SaveContent(i, _defaultEncoding);
        }

        public int SaveContent(int i, Encoding encoding)
        {
            try
            {
                File.WriteAllText(_book.GetFilePath(i), _book.GetFileContent(i), encoding);
                return (int)WhatDoSaveResults.Successfully;
            }
            catch
            {                
                return (int)WhatDoSaveResults.CannotWrite;
            }
        }
       
        public int GetSymbolsCount(int i)
        {
            string fileContent = _book.GetFileContent(i);
            if (fileContent != null) return fileContent.Length;
            else return 0;
        }
        
        #region WriteToFile
        public void WriteToFilePathPlus(string[] textParagraphes, string filesPath, string filesPathPlus)
        {
            string[] pathNameExt = filesPath.Split(new char[] { '.' });
            string filesPathAddPlus = pathNameExt[0] + filesPathPlus + "." + pathNameExt[1];
            MessageBox.Show(filesPathAddPlus, "WriteToFilePathPlus", MessageBoxButtons.OK, MessageBoxIcon.Information);
            File.WriteAllLines(filesPathAddPlus, textParagraphes, _defaultEncoding);
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
            if (IsFileExist(logFilePathName)) return null;//ordered file is exist, cannot create it
            
            string fileLineAppend = "New result file created \r\n";
            File.AppendAllText(logFilePathName, fileLineAppend, _defaultEncoding);
            return logFilePathName;
        }
        #endregion
    }
}


