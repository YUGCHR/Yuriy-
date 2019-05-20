using System;
using System.Reflection;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TextSplitLibrary
{
    public interface IAnalysisLogicDataArrays
    {
        int SetFoundWordsOfParagraph(string wordOfParagraph, int i);
        string GetFoundWordsOfParagraph(int i);
        int ClearFoundWordsOfParagraph();
        int GetFoundWordsOfParagraphLength();
        int SetFoundSymbolsOfParagraph(string symbolsOfParagraph, int i);
        string GetFoundSymbolsOfParagraph(int i);
        int ClearFoundSymbolsOfParagraph();
        int GetFoundSymbolsOfParagraphLength();

        string GetStringMarksChapterName(string BeginOrEnd);
        string GetStringMarksParagraphName(string BeginOrEnd);
        string GetStringMarksSentenceName(string BeginOrEnd);

        int GetChapterNamesSamplesLength(int desiredTextLanguage);
        string GetChapterNamesSamples(int desiredTextLanguage, int i);

        int GetBaseKeyWordFormsQuantity();
        int GetChapterNamesVersionsCount(int m, int i);
        int SetChapterNamesVersionsCount(int m, int i, int countValue);
        int GetChapterSymbolsVersionsCount(int i);
        int SetChapterSymbolsVersionsCount(int i, int countValue);

        int GetCharsParagraphSeparatorLength();
        char GetCharsParagraphSeparator(int index);
        int GetCharsSeparatorLength(string ParagraphOrSentence);
        char[] GetCharsSeparator(string ParagraphOrSentence);
    }

    public class AnalysisLogicDataArrays : IAnalysisLogicDataArrays
    {
        private readonly IAllBookData _bookData;
        private readonly IMessageService _msgService;

        readonly private int showMessagesLevel;
        readonly private int showMessagesLocal;
        readonly private string strCRLF;

        private string[] foundWordsOfParagraph;
        readonly private string[,] chapterNamesSamples;//два следующих массива собираются заменить этот двумерный
        readonly private string[] chapterNamesSamplesLanguage0;
        readonly private string[] chapterNamesSamplesLanguage1;
        readonly private string stringMarksChapterNameBegin;
        readonly private string stringMarksChapterNameEnd;
        readonly private string stringMarksParagraphBegin;
        readonly private string stringMarksParagraphEnd;
        readonly private string stringMarksSentenceBegin;
        readonly private string stringMarksSentenceEnd;
        readonly private char[] charsParagraphSeparator;
        readonly private char[] charsSentenceSeparator;
        readonly private char[] charsQuotesSeparator;

        private string[] foundSymbolsOfParagraph;
        private int[,] chapterNamesVersionsCount;
        private int[] chapterSymbolsVersionsCount;
        private char[] foundCharsSeparator;
        private readonly int baseKeyWordFormsQuantity;

        public AnalysisLogicDataArrays(IAllBookData bookData, IMessageService msgService)
        {
            _bookData = bookData;
            _msgService = msgService;

            showMessagesLevel = DeclarationConstants.ShowMessagesLevel;
            strCRLF = DeclarationConstants.StrCRLF;
            showMessagesLocal = showMessagesLevel;
            showMessagesLocal = 3; //локальные печати класса выводятся на экран
            baseKeyWordFormsQuantity = 3;
            charsParagraphSeparator = new char[] { '\r', '\n' };//можно переделать все на строковый массив - чтобы передавать одним методом
            charsSentenceSeparator = new char[] { '.', '!', '?' };
            charsQuotesSeparator = new char[] { '.', '!', '?' };// "\U0022 «\U00AB »\U00BB ʺ\U02BA ˮ\U02EE ˝\U02DD 
            stringMarksChapterNameBegin = "\u00A4\u00A4\u00A4\u00A4\u00A4";//¤¤¤¤¤ - метка строки перед началом названия главы
            stringMarksChapterNameEnd = "\u00A4\u00A4\u00A4";//¤¤¤ - метка строки после названия главы, еще \u007E - ~
            stringMarksParagraphBegin = "\u00A7\u00A7\u00A7\u00A7\u00A7";//§§§§§ - метка строки перед началом абзаца
            stringMarksParagraphEnd = "\u00A7\u00A7\u00A7";//§§§ - метка строки после абзаца
            stringMarksSentenceBegin = "\u00B6\u00B6\u00B6\u00B6\u00B6";//¶¶¶¶¶ - метка строки перед началом предложния
            stringMarksSentenceEnd = "\u00B6\u00B6\u00B6";//¶¶¶ - метка строки после конца предложения

            chapterNamesSamples = new string[,]//а номера глав бывают буквами!
            { { "chapter", "paragraph", "section", "subhead", "part" },
                { "Глава ", "Параграф " , "Раздел ", "Подраздел ", "Часть " }, };//можно разделить на два отдельных массива и тоже передавать через метод сепараторов - выбирая потом язык (прибавляя цифру языка к строке выбора)

            chapterNamesSamplesLanguage0 = new string[] { "chapter", "paragraph", "section", "subhead", "part" };
            chapterNamesSamplesLanguage1 = new string[] { "Глава ", "Параграф " , "Раздел ", "Подраздел ", "Часть " };

            foundWordsOfParagraph = new string[10];//временное хранение найденных первых десяти слов абзаца
            foundSymbolsOfParagraph = new string[10];//временное хранение найденных групп спецсимволов перед ключевым словом главы
            foundCharsSeparator = new char[10];//временное хранение найденных вариантов разделителей
            chapterNamesVersionsCount = new int[3,GetChapterNamesSamplesLength(0)];
            chapterSymbolsVersionsCount = new int[GetChapterNamesSamplesLength(0)];
        }

        public int GetCharsParagraphSeparatorLength()
        {
            return charsParagraphSeparator.Length;
        }

        public char GetCharsParagraphSeparator(int index)
        {
            return charsParagraphSeparator[index];
        }

        public string GetStringMarksChapterName(string BeginOrEnd)
        {
            switch (BeginOrEnd)
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

        public string GetStringMarksParagraphName(string BeginOrEnd)
        {
            switch (BeginOrEnd)
            {
                case
                "Begin":
                    return stringMarksParagraphBegin;
                case
                "End":
                    return stringMarksParagraphEnd;
            }
            return null;
        }

        public string GetStringMarksSentenceName(string BeginOrEnd)
        {
            switch (BeginOrEnd)
            {
                case
                "Begin":
                    return stringMarksSentenceBegin;
                case
                "End":
                    return stringMarksSentenceEnd;
            }
            return null;
        }

        public int GetCharsSeparatorLength(string ParagraphOrSentence)
        {
            switch (ParagraphOrSentence)
            {
                case
                "Sentence":
                    return charsSentenceSeparator.Length;
                case
                "Paragraph":
                    return charsParagraphSeparator.Length;
                case
                "Quotes":
                    return charsQuotesSeparator.Length;
                    //case
                    //"NamesSamples0":
                    //    return chapterNamesSamplesLanguage0.Length;
                    //case
                    //"NamesSamples1":
                    //    return chapterNamesSamplesLanguage1.Length;
            }
            return 0;
        }

        public char[] GetCharsSeparator(string ParagraphOrSentence)
        {
            switch (ParagraphOrSentence)
            {
                case
                "Sentence":
                    return charsSentenceSeparator;
                case                
                "Paragraph":
                    return charsParagraphSeparator;
                case                    
                "Quotes":
                    return charsQuotesSeparator;
                //case
                //"NamesSamples0":
                //    return chapterNamesSamplesLanguage0.ToCharArray();
                //case
                //"NamesSamples1":
                //    return chapterNamesSamplesLanguage1;
            }
            return null;            
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
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Warning in FoundWordsOfParagraph!" + strCRLF + "Attempt to WRITE with index outside the bounds of the array is detected" + strCRLF + "Requested I = " + i.ToString(), CurrentClassName, showMessagesLocal);
            return (int)MethodFindResult.NothingFound;
        }

        public string GetFoundWordsOfParagraph(int i)
        {
            //Assert.IsFalse(i < GetFoundWordsOfParagraphLength(), "Warning in FoundWordsOfParagraph!" + strCRLF + "Attempt to READ detected with the index besides the array" + strCRLF + "Requested I = " + i.ToString());
            if (i < GetFoundWordsOfParagraphLength())
            {
                return foundWordsOfParagraph[i];
            }            
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Warning in FoundWordsOfParagraph!" + strCRLF + "Attempt to READ detected with the index besides the array" + strCRLF + "Requested I = " + i.ToString(), CurrentClassName, showMessagesLocal);
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
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Warning in FoundSymbolsOfParagraph!" + strCRLF + "Attempt to WRITE detected with the index besides the array" + strCRLF + "Requested I = " + i.ToString(), CurrentClassName, showMessagesLocal);
            return (int)MethodFindResult.NothingFound;
        }

        public string GetFoundSymbolsOfParagraph(int i)
        {
            if (i < GetFoundSymbolsOfParagraphLength())
            {
                return foundSymbolsOfParagraph[i];
            }
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Warning in FoundSymbolsOfParagraph!" + strCRLF + "Attempt to READ detected with the index besides the array" + strCRLF + "Requested I = " + i.ToString(), CurrentClassName, showMessagesLocal);
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
