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

        //string GetStringMarksChapterName(string BeginOrEnd);
        //string GetStringMarksParagraphName(string BeginOrEnd);
        //string GetStringMarksSentenceName(string BeginOrEnd);
        //
        int GetChapterNamesSamplesLength(int desiredTextLanguage);
        string GetChapterNamesSamples(int desiredTextLanguage, int i);
        //
        int GetBaseKeyWordFormsQuantity();
        int GetChapterNamesVersionsCount(int m, int i);
        int SetChapterNamesVersionsCount(int m, int i, int countValue);
        int GetChapterSymbolsVersionsCount(int i);
        int SetChapterSymbolsVersionsCount(int i, int countValue);

        //int GetCharsParagraphSeparatorLength();
        //string[] GetCharsParagraphSeparator(int index);
        int GetConstantWhatNotLength(string WhatNot);
        string[] GetConstantWhatNot(string WhatNot);
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
        readonly private string[] stringMarksChapterNameBegin;
        readonly private string[] stringMarksChapterNameEnd;
        readonly private string[] stringMarksParagraphBegin;
        readonly private string[] stringMarksParagraphEnd;
        readonly private string[] stringMarksSentenceBegin;
        readonly private string[] stringMarksSentenceEnd;
        readonly private string[] charsParagraphSeparator;
        readonly private string[] charsSentenceSeparators;
        readonly private string[] charsQuotesSeparator;
        readonly private string[] charsBracketsSeparator;
        readonly private string[] numbersOfGroupsNames; 

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
            charsParagraphSeparator = new string[] { "\r\n" };//в строковом массиве - чтобы получать все константы одним методом

            charsSentenceSeparators = new string[] { ".", "…", "!", "?", ";" };//…\u2026 (Horizontal Ellipsis) ⁇\u2047 ⁈\u2048 ⁉\u2049 ‼\u203C
            charsQuotesSeparator = new string[] { "\u0022", "/", "\u02BA", "\u02EE", "\u02DD" };// "\u0022 ʺ\u02BA ˮ\u02EE ˝\u02DD - кавычки и скобки без деления на открывающие и закрывающие
            charsBracketsSeparator = new string[] { "()", "[]", "{}", "«»", "<>" };// - кавычки и скобки открывающие и закрывающие - «\u00AB »\u00BB
            numbersOfGroupsNames = new string[] { "Sentence", "Quotes", "Brackets" }; //номера групп сепараторов для получения их значений в цикле

            stringMarksChapterNameBegin = new string[] { "\u00A4\u00A4\u00A4\u00A4\u00A4" };//¤¤¤¤¤ - метка строки перед началом названия главы
            stringMarksChapterNameEnd = new string[] { "\u00A4\u00A4\u00A4" };//¤¤¤ - метка строки после названия главы, еще \u007E - ~
            stringMarksParagraphBegin = new string[] { "\u00A7\u00A7\u00A7\u00A7\u00A7" };//§§§§§ - метка строки перед началом абзаца
            stringMarksParagraphEnd = new string[] { "\u00A7\u00A7\u00A7" };//§§§ - метка строки после абзаца
            stringMarksSentenceBegin = new string[] { "\u00B6\u00B6\u00B6\u00B6\u00B6" };//¶¶¶¶¶ - метка строки перед началом предложния
            stringMarksSentenceEnd = new string[] { "\u00B6\u00B6\u00B6" };//¶¶¶ - метка строки после конца предложения            

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

        public int GetChapterNamesSamplesLength(int desiredTextLanguage)
        {//в дальнейшем массив можно сделать динамическим и с разным количеством ключевых слов для разных языков, тогда получать язык при запросе
            return chapterNamesSamples.GetLength(1);
        }

        public string GetChapterNamesSamples(int desiredTextLanguage, int i)
        {//потом сделать динамический массив
            return chapterNamesSamples[desiredTextLanguage, i];
        }

        public int GetConstantWhatNotLength(string WhatNot)//хорошо бы проверить, что не null
        {
            switch (WhatNot)
            {
                case
                "Sentence":
                    return charsSentenceSeparators.Length;                
                case
                "Quotes":
                    return charsQuotesSeparator.Length;
                case
                "Brackets":
                    return charsBracketsSeparator.Length;
                case
                "GroupsNumbers":
                    return numbersOfGroupsNames.Length;
                case
                "Paragraph":
                    return charsParagraphSeparator.Length;
                case
                "ChapterBegin":
                    return stringMarksChapterNameBegin.Length;
                case
                "ChapterEnd":
                    return stringMarksChapterNameEnd.Length;
                case
                "ParagraphBegin":
                    return stringMarksParagraphBegin.Length;
                case
                "ParagraphEnd":
                    return stringMarksParagraphEnd.Length;
                case
                "SentenceBegin":
                    return stringMarksSentenceBegin.Length;
                case
                "SentenceEnd":
                    return stringMarksSentenceEnd.Length;
                case                    
                "NamesSamples0":
                    return chapterNamesSamplesLanguage0.Length;
                case                    
                "NamesSamples1":
                    return chapterNamesSamplesLanguage1.Length;
            }
            return 0;
        }

        public string[] GetConstantWhatNot(string WhatNot)
        {
            switch (WhatNot)
            {
                case
                "Sentence":
                    return charsSentenceSeparators;                
                case
                "Quotes":
                    return charsQuotesSeparator;
                case
                "Brackets":
                    return charsBracketsSeparator;
                case
                "GroupsNumbers":
                    return numbersOfGroupsNames;
                case
                "Paragraph":
                    return charsParagraphSeparator;
                case
                "ChapterBegin":
                    return stringMarksChapterNameBegin;
                case
                "ChapterEnd":
                    return stringMarksChapterNameEnd;
                case
                "ParagraphBegin":
                    return stringMarksParagraphBegin;
                case
                "ParagraphEnd":
                    return stringMarksParagraphEnd;
                case
                "SentenceBegin":
                    return stringMarksSentenceBegin;
                case
                "SentenceEnd":
                    return stringMarksSentenceEnd;
                case                    
                "NamesSamples0":
                    return chapterNamesSamplesLanguage0;
                case                    
                "NamesSamples1":
                    return chapterNamesSamplesLanguage1;
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
