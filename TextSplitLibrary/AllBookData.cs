using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;//Temp

namespace TextSplitLibrary
{
    public interface IAllBookData
    {
        int GetFileToDo(int i);
        int SetFileToDo(int fileToDo, int i);
        int WhatFileNeedToDo(int whatNeedToDo);//возвращает индекс ToDo, в котором найдено значение whatNeedToDo, если не найдено, возвращает -1 (или лучше 0?)

        int GetFileToSave(int i);        
        int SetFileToSave(int fileToSave, int i);
        int WhatFileNeedToSave(int whatNeedToSave);//возвращает индекс ToDo, в котором найдено значение whatNeedToDo, если не найдено, возвращает -1 (или лучше 0?)

        string GetFilePath(int i);
        int SetFilePath(string filePath, int i);

        string GetSelectedText(int i);
        int SetSelectedText(string selectedText, int i);

        string GetFileContent(int i);
        int SetFileContent(string fileContent, int i);

        string GetParagraphText(int paragraphCount, int langauageIndex);//возвращает строку из двумерного списка List
        int GetParagraphTextLength(int langauageIndex);
        int AddParagraphText(string paragraphText, int langauageIndex);//тоже возвращает количество элементов
        int RemoveAtParagraphText(int paragraphCount, int langauageIndex);//удаляет элемент списка с индексом paragraphCount

        string GetChapterName(int chapterCount, int langauageIndex);
        int GetChapterNameLength(int langauageIndex);
        int AddChapterName(string chapterNameWithNumber, int langauageIndex);

        int GetChapterNumber(int chapterCount, int langauageIndex);
        int GetChapterNumberLength(int langauageIndex);
        int AddChapterNumber(int chapterNumberOnly, int langauageIndex);

        string GetSymbolsCount(int i);
        int SetSymbolsCount(int symbolsCount, int i);
    }

    public class AllBookData : IAllBookData
    {
        //private readonly IMessageService _messageService;

        readonly private int filesQuantity;

        private int[] filesToDo;
        private int[] filesToSave;
        private int[] symbolsCounts;        

        private string[] filesPath;
        private string[] selectedTexts;
        private string[] filesContents;
        
        private List<List<string>> paragraphsTexts = new List<List<string>>(); //инициализация динамического двумерного массива с абзацами (на двух языках + когда-то результат)        
        private List<List<string>> chaptersNamesWithNumbers = new List<List<string>>(); //массив полных названий глав
        private List<List<int>> chaptersNamesNumbersOnly = new List<List<int>>(); //только цифры из названий глав (надо ли?)        

        public AllBookData()//IMessageService service
        {
            //_messageService = service;

            filesQuantity = Declaration.FilesQuantity;

            filesToDo = new int[filesQuantity];
            filesToSave = new int[filesQuantity];
            symbolsCounts = new int[filesQuantity];
            filesPath = new string[filesQuantity];
            selectedTexts = new string[filesQuantity];
            filesContents = new string[filesQuantity];

            paragraphsTexts.Add(new List<string>());
            paragraphsTexts.Add(new List<string>()); //добавление второй строки для абзацев второго языка (пока нужно всего 2 строки)
            
            chaptersNamesWithNumbers.Add(new List<string>());
            chaptersNamesWithNumbers.Add(new List<string>());
            chaptersNamesNumbersOnly.Add(new List<int>());
            chaptersNamesNumbersOnly.Add(new List<int>());
        }
        //группа массива ToDo
        public int GetFileToDo(int i)
        {
            return filesToDo[i];
        }

        public int SetFileToDo(int fileToDo, int i)
        {
            filesToDo[i] = fileToDo;
            return (int)MethodFindResult.AllRight;
        }

        public int WhatFileNeedToDo(int whatNeedToDo)
        {
            for (int i = 0; i < filesQuantity; i++)
            {
                if (filesToDo[i] == whatNeedToDo) return i;
            }
            return (int)MethodFindResult.NothingFound;
        }
        //группа массива ToSave
        public int GetFileToSave(int i)
        {
            return filesToSave[i];
        }

        public int SetFileToSave(int fileToSave, int i)
        {
            filesToSave[i] = fileToSave;
            return (int)MethodFindResult.AllRight;
        }

        public int WhatFileNeedToSave(int whatNeedToSave)
        {
            for (int i = 0; i < filesQuantity; i++)
            {
                if (filesToSave[i] == whatNeedToSave) return i;
            }
            return (int)MethodFindResult.NothingFound;
        }
        //группа массива Path
        public string GetFilePath(int i)
        {
            return filesPath[i];
        }

        public int SetFilePath(string filePath, int i)
        {
            filesPath[i] = filePath;
            return (int)MethodFindResult.AllRight;
        }
        //группа массива Slection
        public string GetSelectedText(int i)
        {
            return selectedTexts[i];
        }

        public int SetSelectedText(string selectedText, int i)
        {
            selectedTexts[i] = selectedText;
            return (int)MethodFindResult.AllRight;
        }
        //группа массива Content
        public string GetFileContent(int i)
        {
            return filesContents[i];
        }

        public int SetFileContent(string fileContent, int i)
        {
            filesContents[i] = fileContent;
            return (int)MethodFindResult.AllRight;
        }
        //группа массива Абзац текста
        public string GetParagraphText(int paragraphCount, int langauageIndex)
        {
            return paragraphsTexts[langauageIndex][paragraphCount];            
        }

        public int GetParagraphTextLength(int langauageIndex)
        {
            return paragraphsTexts[langauageIndex].Count;
        }

        public int AddParagraphText(string paragraphText, int langauageIndex)
        {
            paragraphsTexts[langauageIndex].Add(paragraphText);//добавление нового элемента в строку
            return paragraphsTexts[langauageIndex].Count;
        }

        public int RemoveAtParagraphText(int paragraphCount, int langauageIndex)
        {
            if (paragraphCount >= paragraphsTexts[langauageIndex].Count)//сделать такие проверки во всех методах, придумать что-то с печатью (тревожное окно)
            {
                MessageBox.Show("запрошенный индекс = " + paragraphCount.ToString() + "\r\n" + "максимальный индекс = " + paragraphsTexts[langauageIndex].Count.ToString(), "AllBookData", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (paragraphCount < 0)
            {
                MessageBox.Show("запрошенный индекс = " + paragraphCount.ToString(), "AllBookData", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            paragraphsTexts[langauageIndex].RemoveAt(paragraphCount);//удаление элемента по индексу
            return paragraphsTexts[langauageIndex].Count;//получение и возврат новой длины списка
        }
        //группа массива Главы имя
        public string GetChapterName(int chapterCount, int langauageIndex)
        {
            if (chaptersNamesWithNumbers[langauageIndex][chapterCount] != null) return paragraphsTexts[langauageIndex][chapterCount];
            return null;
        }

        public int GetChapterNameLength(int langauageIndex)
        {
            return chaptersNamesWithNumbers[langauageIndex].Count;
        }

        public int AddChapterName(string chapterNameWithNumber, int langauageIndex)
        {
            chaptersNamesWithNumbers[langauageIndex].Add(chapterNameWithNumber);//добавление нового элемента в строку
            return chaptersNamesWithNumbers[langauageIndex].Count;
        }
        //группа массива Главы Номер
        public int GetChapterNumber(int chapterCount, int langauageIndex)
        {
            if (chaptersNamesNumbersOnly[langauageIndex][chapterCount] !=  0) return chaptersNamesNumbersOnly[langauageIndex][chapterCount];
            return (int)MethodFindResult.NothingFound;
        }

        public int GetChapterNumberLength(int langauageIndex)
        {
            return chaptersNamesNumbersOnly[langauageIndex].Count;
        }

        public int AddChapterNumber(int chapterNumberOnly, int langauageIndex)
        {
            chaptersNamesNumbersOnly[langauageIndex].Add(chapterNumberOnly);//добавление нового элемента в строку
            return chaptersNamesNumbersOnly[langauageIndex].Count;
        }
        //группа массива подсчета символов
        public string GetSymbolsCount(int i)
        {
            return symbolsCounts[i].ToString();
        }

        public int SetSymbolsCount(int symbolsCount, int i)
        {
            symbolsCounts[i] = symbolsCount;
            return (int)MethodFindResult.AllRight;
        }
    }    
}
