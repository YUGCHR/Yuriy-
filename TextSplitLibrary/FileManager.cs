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
        string[] GetContent(string[] FilePath);
        string[] GetContent(string[] FilePath, Encoding encoding);
        void SaveContent(string[] FileContent, string[] FilePath);
        void SaveContent(string[] FileContent, string[] FilePath, Encoding encoding);
        int[] GetSymbolCount(string[] FileContent);
        bool[] IsExist(string[] FilePath);
        int FilesQuantity { get; set; }
    }
    public class FileManager : IFileManager
    {
        public int FilesQuantity { get; set; }
        
        private readonly Encoding _defaultEncoding = Encoding.GetEncoding(1251);
        public string[] FilePath;
        public string[] FileContent;
        public int filesQuantity;

        public FileManager()
        {
            filesQuantity = FilesQuantity;
            FilePath = new string[filesQuantity];
            FileContent = new string[filesQuantity];            
        }

        public bool[] IsExist(string[] filePath)
        {            
            bool[] isExist = new bool[filesQuantity];
            for (int i = 0; i < filesQuantity; i++)
            {
                string currentFilePath = filePath[i];
                isExist[i] = File.Exists(currentFilePath);
            }
            return isExist;
        }

        public string[] GetContent(string[] filePath)
        {            
                return GetContent(filePath, _defaultEncoding); 
        }

        public string[] GetContent(string[] filePath, Encoding encoding)
        {
            string[] getContent = new string[filesQuantity];
            for (int i = 0; i < filesQuantity; i++)
            {
                getContent[i] = File.ReadAllText(filePath[i], encoding);
            }
            return getContent;
        }

        public void SaveContent(string[] fileContent, string[] filePath)
        {
            SaveContent(fileContent, filePath, _defaultEncoding);
        }

        public void SaveContent(string[] fileContent, string[] filePath, Encoding encoding)
        {
            for (int i = 0; i < filesQuantity; i++)
            {
                File.WriteAllText(filePath[i], fileContent[i], encoding);//вынести в метод единичного сохранения и сделать перегруженный с номером i
            }            
        }

        public int[] GetSymbolCount(string[] fileContent)
        {
            int[] count = new int[filesQuantity];
            for (int i = 0; i < filesQuantity; i++)
            {
                count[i] = fileContent[i].Length;
            }            
            return count;
        }
    }    
}


