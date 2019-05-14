using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        int GetChapterNamesVersionsCount(int i);
        int SetChapterNamesVersionsCount(int i, int countValue);
        int GetChapterSymbolsVersionsCount(int i);
        int SetChapterSymbolsVersionsCount(int i, int countValue);
        int IncrementOfChapterNamesVersionsCount(int i);
        int IncrementOfChapterSymbolsVersionsCount(int i);
    }

    public class AnalysisLogicChapterDataArrays : IAnalysisLogicChapterDataArrays
    {
        private readonly IAllBookData _book;
        private readonly IMessageService _messageService;

        private string[] foundWordsOfParagraph;
        readonly private string[,] chapterNamesSamples;
        readonly private string stringMarksChapterNameBegin;
        readonly private string stringMarksChapterNameEnd;
        readonly private char[] charsParagraphSeparator;
        readonly private char[] charsSentenceSeparator;

        private string[] foundSymbolsOfParagraph;
        private int[] chapterNamesVersionsCount;
        private int[] chapterSymbolsVersionsCount;
        private char[] foundCharsSeparator;


        public AnalysisLogicChapterDataArrays(IAllBookData book, IMessageService service)
        {
            _book = book;
            _messageService = service;

            foundWordsOfParagraph = new string[10];//временное хранение найденных первых десяти слов абзаца

            charsParagraphSeparator = new char[] { '\r', '\n' };
            charsSentenceSeparator = new char[] { '.', '!', '?' };
            stringMarksChapterNameBegin = "\u00A4\u00A4\u00A4\u00A4\u00A4";//¤¤¤¤¤ - метка строки перед началом названия главы
            stringMarksChapterNameEnd = "\u00A4\u00A4\u00A4";//¤¤¤ - метка строки после названия главы, еще \u00A7 - §, \u007E - ~, \u00B6 - ¶            
                                                             //проверить типовые названия глав (для разных языков свои) - сделать метод универсальным и для частей тоже? или некоторые методы метода - перенести их тогда в общую логику
            chapterNamesSamples = new string[,]
            { { "Chapter ", "Paragraph ", "Section ", "Subhead ", "Part " },
                { "Глава ", "Параграф " , "Раздел ", "Подраздел ", "Часть " }, };
            //а номера глав бывают буквами! то мелочи, ключевые слова могуть быть из прописных букв, может быть дефис между словом и номером или другой символ        

            foundSymbolsOfParagraph = new string[10];//временное хранение найденных групп спецсимволов перед ключевым словом главы
            foundCharsSeparator = new char[10];//временное хранение найденных вариантов разделителей
            chapterNamesVersionsCount = new int[GetChapterNamesSamplesLength(0)];
            chapterSymbolsVersionsCount = new int[GetChapterNamesSamplesLength(0)];
        }


        public int SetFoundWordsOfParagraph(string wordOfParagraph, int i)
        {
            if (i < GetFoundWordsOfParagraphLength())
            {
                foundWordsOfParagraph[i] = wordOfParagraph;
                return 0;
            }
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Warning! Attempt to write detected with the index besides the array , I = " + i.ToString(), CurrentClassName, 3);
            return (int)MethodFindResult.NothingFound;
        }

        public string GetFoundWordsOfParagraph(int i)
        {
            if (i < GetFoundWordsOfParagraphLength())
            {
                return foundWordsOfParagraph[i];
            }
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Warning! Attempt to read detected with the index besides the array , I = " + i.ToString(), CurrentClassName, 3);
            return null;
        }

        public int ClearFoundWordsOfParagraph()
        {
            Array.Clear(foundWordsOfParagraph, 0, GetFoundWordsOfParagraphLength());//как проверить, что массив очистился?
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
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Warning! Attempt to write detected with the index besides the array , I = " + i.ToString(), CurrentClassName, 3);
            return (int)MethodFindResult.NothingFound;
        }

        public string GetFoundSymbolsOfParagraph(int i)
        {
            if (i < GetFoundSymbolsOfParagraphLength())
            {
                return foundSymbolsOfParagraph[i];
            }
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Warning! Attempt to read detected with the index besides the array , I = " + i.ToString(), CurrentClassName, 3);
            return null;
        }

        public int ClearFoundSymbolsOfParagraph()
        {
            Array.Clear(foundSymbolsOfParagraph, 0, GetFoundSymbolsOfParagraphLength());//как проверить, что массив очистился?
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

        public int GetChapterNamesVersionsCount(int i)
        {
            return chapterNamesVersionsCount[i];
        }

        public int SetChapterNamesVersionsCount(int i, int countValue)
        {
            chapterNamesVersionsCount[i] = countValue;
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

        public int IncrementOfChapterNamesVersionsCount(int i)
        {
            return chapterNamesVersionsCount[i]++;
        }

        public int IncrementOfChapterSymbolsVersionsCount(int i)
        {
            return chapterSymbolsVersionsCount[i]++;
        }
    }


}
