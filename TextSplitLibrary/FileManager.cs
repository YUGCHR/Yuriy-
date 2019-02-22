using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        int FilesQuantity { get; set; }

    }
    public class FileManager : IFileManager
    {
        public int[] FilesToDo { get; set; }        
        private readonly Encoding _defaultEncoding = Encoding.GetEncoding(1251);
        public string[] FilesPath;
        public string[] FilesContent;
        public int FilesQuantity { get; set; }
        public int filesQuantity;//Declaration.LanguagesQuantity;

        public FileManager()
        {
            filesQuantity = FilesQuantity;
            MessageBox.Show(filesQuantity.ToString(), "FileManager - filesQuantity", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}


