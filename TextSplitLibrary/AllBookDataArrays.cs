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
        int WhatFileNeedToDo(int whatNeedToDo);//возвращает индекс ToDo, в котором найдено значение whatNeedToDo, если не найдено, возвращает -1

        int GetFileToSave(int i);        
        int SetFileToSave(int fileToSave, int i);
        int WhatFileNeedToSave(int whatNeedToSave);//возвращает индекс FileToSave, в котором найдено значение WhatFileNeedToSave, если не найдено, возвращает -1

        string GetFilePath(int i);
        int SetFilePath(string filePath, int i);

        string GetSelectedText(int i);
        int SetSelectedText(string selectedText, int i);

        string GetFileContent(int i);
        int SetFileContent(string fileContent, int i);

        string GetParagraphText(int paragraphCount, int desiredTextLanguage);//возвращает строку из двумерного списка List
        int GetParagraphTextLength(int desiredTextLanguage);
        int SetParagraphText(string paragraphText, int paragraphCount, int desiredTextLanguage);
        int AddParagraphText(string paragraphText, int desiredTextLanguage);//тоже возвращает количество элементов
        int RemoveAtParagraphText(int paragraphCount, int desiredTextLanguage);//удаляет элемент списка с индексом paragraphCount

        string GetChapterName(int chapterCount, int desiredTextLanguage);
        int GetChapterNameLength(int desiredTextLanguage);
        int SetChapterName(string chapterNameWithNumber, int chapterCount, int desiredTextLanguage);
        int AddChapterName(string chapterNameWithNumber, int desiredTextLanguage);

        int GetChapterNumber(int chapterCount, int desiredTextLanguage);
        int GetChapterNumberLength(int desiredTextLanguage);
        int AddChapterNumber(int chapterNumberOnly, int desiredTextLanguage);

        string GetSymbolsCount(int i);
        int SetSymbolsCount(int symbolsCount, int i);
    }

    public class AllBookDataArrays : IAllBookData
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

        public AllBookDataArrays()//IMessageService service
        {
            //_messageService = service;

            filesQuantity = DeclarationConstants.FilesQuantity;

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
        //группа массива filesToDo
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
        //группа массива filesToSave
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
        //группа массива filesPath
        public string GetFilePath(int i)
        {
            return filesPath[i];
        }

        public int SetFilePath(string filePath, int i)
        {
            filesPath[i] = filePath;
            return (int)MethodFindResult.AllRight;
        }
        //группа массива selectedTexts
        public string GetSelectedText(int i)
        {
            return selectedTexts[i];
        }

        public int SetSelectedText(string selectedText, int i)
        {
            selectedTexts[i] = selectedText;
            return (int)MethodFindResult.AllRight;
        }
        //группа массива filesContents
        public string GetFileContent(int i)
        {
            return filesContents[i];
        }

        public int SetFileContent(string fileContent, int i)
        {
            filesContents[i] = fileContent;
            return (int)MethodFindResult.AllRight;
        }
        //группа массива Абзац текста - paragraphsTexts
        public string GetParagraphText(int paragraphCount, int desiredTextLanguage)
        {
            return paragraphsTexts[desiredTextLanguage][paragraphCount];            
        }

        public int GetParagraphTextLength(int desiredTextLanguage)
        {
            return paragraphsTexts[desiredTextLanguage].Count;
        }

        public int SetParagraphText(string paragraphText, int paragraphCount, int desiredTextLanguage)
        {
            paragraphsTexts[desiredTextLanguage][paragraphCount] = paragraphText;
            return 0;
        }

        public int AddParagraphText(string paragraphText, int desiredTextLanguage)
        {
            paragraphsTexts[desiredTextLanguage].Add(paragraphText);//добавление нового элемента в строку
            return paragraphsTexts[desiredTextLanguage].Count;
        }

        public int RemoveAtParagraphText(int paragraphCount, int desiredTextLanguage)
        {
            if (paragraphCount >= paragraphsTexts[desiredTextLanguage].Count)//сделать такие проверки во всех методах, придумать что-то с печатью (тревожное окно)
            {
                MessageBox.Show("запрошенный индекс = " + paragraphCount.ToString() + "\r\n" + "максимальный индекс = " + paragraphsTexts[desiredTextLanguage].Count.ToString(), "AllBookData", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (paragraphCount < 0)
            {
                MessageBox.Show("запрошенный индекс = " + paragraphCount.ToString(), "AllBookData", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            paragraphsTexts[desiredTextLanguage].RemoveAt(paragraphCount);//удаление элемента по индексу
            return paragraphsTexts[desiredTextLanguage].Count;//получение и возврат новой длины списка
        }
        //группа массива Главы имя - chaptersNamesWithNumbers
        public string GetChapterName(int chapterCount, int desiredTextLanguage)
        {
            if (chaptersNamesWithNumbers[desiredTextLanguage][chapterCount] != null) return chaptersNamesWithNumbers[desiredTextLanguage][chapterCount];
            return null;
        }

        public int GetChapterNameLength(int desiredTextLanguage)
        {
            return chaptersNamesWithNumbers[desiredTextLanguage].Count;
        }

        public int SetChapterName(string chapterNameWithNumber, int chapterCount, int desiredTextLanguage)
        {
            chaptersNamesWithNumbers[desiredTextLanguage][chapterCount] = chapterNameWithNumber;
            return 0;
        }

        public int AddChapterName(string chapterNameWithNumber, int desiredTextLanguage)
        {
            chaptersNamesWithNumbers[desiredTextLanguage].Add(chapterNameWithNumber);//добавление нового элемента в строку
            return chaptersNamesWithNumbers[desiredTextLanguage].Count;
        }
        //группа массива Главы Номер - chaptersNamesNumbersOnly
        public int GetChapterNumber(int chapterCount, int desiredTextLanguage)
        {
            if (chaptersNamesNumbersOnly[desiredTextLanguage][chapterCount] !=  0) return chaptersNamesNumbersOnly[desiredTextLanguage][chapterCount];
            return (int)MethodFindResult.NothingFound;
        }

        public int GetChapterNumberLength(int desiredTextLanguage)
        {
            return chaptersNamesNumbersOnly[desiredTextLanguage].Count;
        }

        public int AddChapterNumber(int chapterNumberOnly, int desiredTextLanguage)
        {
            chaptersNamesNumbersOnly[desiredTextLanguage].Add(chapterNumberOnly);//добавление нового элемента в строку
            return chaptersNamesNumbersOnly[desiredTextLanguage].Count;
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
