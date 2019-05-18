using System;
using System.Reflection;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TextSplitLibrary
{
    public interface IAnalysisLogicChapterDataArrays
    {
        int SetFoundWordsOfParagraph(string wordOfParagraph, int i);
        string GetFoundWordsOfParagraph(int i);
        int ClearFoundWordsOfParagraph();
        int GetFoundWordsOfParagraphLength();
        int SetFoundSymbolsOfParagraph(string symbolsOfParagraph, int i);
        string GetFoundSymbolsOfParagraph(int i);
        int ClearFoundSymbolsOfParagraph();
        int GetFoundSymbolsOfParagraphLength();

        string GetStringMarksChapterName(string markChapterName);
        int GetChapterNamesSamplesLength(int desiredTextLanguage);
        string GetChapterNamesSamples(int desiredTextLanguage, int i);

        int GetBaseKeyWordFormsQuantity();
        int GetChapterNamesVersionsCount(int m, int i);
        int SetChapterNamesVersionsCount(int m, int i, int countValue);
        int GetChapterSymbolsVersionsCount(int i);
        int SetChapterSymbolsVersionsCount(int i, int countValue);        
    }

    public class AnalysisLogicChapterDataArrays : IAnalysisLogicChapterDataArrays
    {
        private readonly IAllBookData _bookData;
        private readonly IMessageService _msgService;

        readonly private int showMessagesLevel;
        readonly private string strCRLF;

        private string[] foundWordsOfParagraph;
        readonly private string[,] chapterNamesSamples;
        readonly private string stringMarksChapterNameBegin;
        readonly private string stringMarksChapterNameEnd;
        readonly private char[] charsParagraphSeparator;
        readonly private char[] charsSentenceSeparator;

        private string[] foundSymbolsOfParagraph;
        private int[,] chapterNamesVersionsCount;
        private int[] chapterSymbolsVersionsCount;
        private char[] foundCharsSeparator;
        private readonly int baseKeyWordFormsQuantity;

        public AnalysisLogicChapterDataArrays(IAllBookData bookData, IMessageService msgService)
        {
            _bookData = bookData;
            _msgService = msgService;

            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;
            baseKeyWordFormsQuantity = 3;
            charsParagraphSeparator = new char[] { '\r', '\n' };
            charsSentenceSeparator = new char[] { '.', '!', '?' };
            stringMarksChapterNameBegin = "\u00A4\u00A4\u00A4\u00A4\u00A4";//¤¤¤¤¤ - метка строки перед началом названия главы
            stringMarksChapterNameEnd = "\u00A4\u00A4\u00A4";//¤¤¤ - метка строки после названия главы, еще \u00A7 - §, \u007E - ~, \u00B6 - ¶            
                                                             
            chapterNamesSamples = new string[,]//а номера глав бывают буквами! то мелочи, ключевые слова могуть быть из прописных букв, может быть дефис между словом и номером или другой символ
            { { "chapter", "paragraph", "section", "subhead", "part" },
                { "Глава ", "Параграф " , "Раздел ", "Подраздел ", "Часть " }, };
                    
            foundWordsOfParagraph = new string[10];//временное хранение найденных первых десяти слов абзаца
            foundSymbolsOfParagraph = new string[10];//временное хранение найденных групп спецсимволов перед ключевым словом главы
            foundCharsSeparator = new char[10];//временное хранение найденных вариантов разделителей
            chapterNamesVersionsCount = new int[3,GetChapterNamesSamplesLength(0)];
            chapterSymbolsVersionsCount = new int[GetChapterNamesSamplesLength(0)];
        }

        public int GetBaseKeyWordFormsQuantity()
        {
            return baseKeyWordFormsQuantity;
        }

        public int SetFoundWordsOfParagraph(string wordOfParagraph, int i)
        {
            if (i < GetFoundWordsOfParagraphLength())
            {
                foundWordsOfParagraph[i] = wordOfParagraph;
                return 0;
            }
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Warning in FoundWordsOfParagraph!" + strCRLF + "Attempt to WRITE with index outside the bounds of the array is detected" + strCRLF + "Requested I = " + i.ToString(), CurrentClassName, 3);
            return (int)MethodFindResult.NothingFound;
        }

        public string GetFoundWordsOfParagraph(int i)
        {
            //Assert.IsFalse(i < GetFoundWordsOfParagraphLength(), "Warning in FoundWordsOfParagraph!" + strCRLF + "Attempt to READ detected with the index besides the array" + strCRLF + "Requested I = " + i.ToString());
            if (i < GetFoundWordsOfParagraphLength())
            {
                return foundWordsOfParagraph[i];
            }            
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Warning in FoundWordsOfParagraph!" + strCRLF + "Attempt to READ detected with the index besides the array" + strCRLF + "Requested I = " + i.ToString(), CurrentClassName, 3);
            return null;
        }

        public int ClearFoundWordsOfParagraph()
        {
            Array.Clear(foundWordsOfParagraph, 0, GetFoundWordsOfParagraphLength());//проверить, что массив очистился?
            return 0;
        }

        public int GetFoundWordsOfParagraphLength()
        {
            int foundWordsOfParagraphLength = foundWordsOfParagraph.Length;
            return foundWordsOfParagraphLength;
        }

        public int SetFoundSymbolsOfParagraph(string symbolsOfParagraph, int i)
        {
            if (i < GetFoundSymbolsOfParagraphLength())
            {
                foundSymbolsOfParagraph[i] = symbolsOfParagraph;
                return 0;
            }
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Warning in FoundSymbolsOfParagraph!" + strCRLF + "Attempt to WRITE detected with the index besides the array" + strCRLF + "Requested I = " + i.ToString(), CurrentClassName, 3);
            return (int)MethodFindResult.NothingFound;
        }

        public string GetFoundSymbolsOfParagraph(int i)
        {
            if (i < GetFoundSymbolsOfParagraphLength())
            {
                return foundSymbolsOfParagraph[i];
            }
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Warning in FoundSymbolsOfParagraph!" + strCRLF + "Attempt to READ detected with the index besides the array" + strCRLF + "Requested I = " + i.ToString(), CurrentClassName, 3);
            return null;
        }

        public int ClearFoundSymbolsOfParagraph()
        {
            Array.Clear(foundSymbolsOfParagraph, 0, GetFoundSymbolsOfParagraphLength());//проверить, что массив очистился?
            return 0;
        }

        public int GetFoundSymbolsOfParagraphLength()
        {
            int foundSymbolsOfParagraphLength = foundSymbolsOfParagraph.Length;
            return foundSymbolsOfParagraphLength;
        }

        public string GetStringMarksChapterName(string markChapterName)
        {
            switch (markChapterName)
            {
                case
                "Begin":
                    return stringMarksChapterNameBegin;
                case
                "End":
                    return stringMarksChapterNameEnd;
            }
            return null;
        }

        public int GetChapterNamesSamplesLength(int desiredTextLanguage)
        {//в дальнейшем массив можно сделать динамическим и с разным количеством ключевых слов для разных языков, тогда получать язык при запросе
            return chapterNamesSamples.GetLength(1);
        }

        public string GetChapterNamesSamples(int desiredTextLanguage, int i)
        {//потом сделать динамический массив
            return chapterNamesSamples[desiredTextLanguage, i];
        }

        public int GetChapterNamesVersionsCount(int m, int i) //m - варианты форм, i - варианты ключевых слов
        {
            return chapterNamesVersionsCount[m, i];
        }

        public int SetChapterNamesVersionsCount(int m, int i, int countValue)
        {
            chapterNamesVersionsCount[m, i] = countValue;
            return 0;
        }        

        public int GetChapterSymbolsVersionsCount(int i)
        {
            return chapterSymbolsVersionsCount[i];
        }

        public int SetChapterSymbolsVersionsCount(int i, int countValue)
        {
            chapterSymbolsVersionsCount[i] = countValue;
            return 0;
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}
