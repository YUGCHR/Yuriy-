using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace TextSplitLibrary
{
    public interface IAllBookData
    {
        int GetFileToDo(int i);
        int SetFileToDo(int fileToDo, int i);

        int GetFileToSave(int i);        
        int SetFileToSave(int fileToSave, int i);

        string GetFilePath(int i);
        int SetFilePath(string filePath, int i);

        string GetFileContent(int i);
        int SetFileContent(string fileContent, int i);

        string GetSymbolsCount(int i);
        int SetSymbolsCount(int symbolsCount, int i);
    }

    public class AllBookData : IAllBookData
    {
        readonly private int filesQuantity;
        
        private int[] filesToDo;
        private int[] filesToSave;
        private int[] symbolsCounts;
        private string[] filesPath;
        private string[] filesContent;


        public AllBookData()
        {
            filesQuantity = Declaration.FilesQuantity;
            
            filesToDo = new int[filesQuantity];
            filesToSave = new int[filesQuantity];
            symbolsCounts = new int[filesQuantity];
            filesPath = new string[filesQuantity];
            filesContent = new string[filesQuantity];
        }        

        public int GetFileToDo(int i)
        {
            return filesToDo[i];
        }

        public int SetFileToDo(int fileToDo, int i)
        {
            filesToDo[i] = fileToDo;
            return 0;
        }

        public int GetFileToSave(int i)
        {
            return filesToSave[i];
        }

        public int SetFileToSave(int fileToSave, int i)
        {
            filesToSave[i] = fileToSave;
            return 0;
        }

        public string GetFilePath(int i)
        {
            return filesPath[i];
        }

        public int SetFilePath(string filePath, int i)
        {
            filesPath[i] = filePath;
            return 0;
        }

        public string GetFileContent(int i)
        {
            return filesContent[i];
        }

        public int SetFileContent(string fileContent, int i)
        {
            filesContent[i] = fileContent;
            return 0;
        }

        public string GetSymbolsCount(int i)
        {
            return symbolsCounts[i].ToString();
        }

        public int SetSymbolsCount(int symbolsCount, int i)
        {
            symbolsCounts[i] = symbolsCount;
            return 0;
        }
    }    
}
