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
        int GetIntConstant(string needConstantnName);
        string[] GetStringArrConstant(string needConstantnName);
    }












    public interface IAnalysisLogicDataConstant
    {
        
    }

    public class AnalysisLogicDataConstant<T> : IAnalysisLogicDataConstant
    {
        public T ReturnType { get; set; }
        public string WordToGetValue { get; set; }
        public int ConstantLength { get; set; }

        AnalysisLogicDataConstant<int[]> totalDigitsQuantity = new AnalysisLogicDataConstant<int[]> { WordToGetValue = "", ConstantLength = 0 };
        AnalysisLogicDataConstant<string[]> SentenceSeparators = new AnalysisLogicDataConstant<string[]> { WordToGetValue = "", ConstantLength = 0 };

        public AnalysisLogicDataConstant()
        {
            SentenceSeparators.ReturnType = new string[] { ".", "…", "!", "?", ";" };
            SentenceSeparators.ConstantLength = SentenceSeparators.ReturnType.Length;
            SentenceSeparators.WordToGetValue = "SentenceSeparators";
            

            totalDigitsQuantity.ReturnType = new int[] { 3, 5, 5 };
            totalDigitsQuantity.ConstantLength = totalDigitsQuantity.ReturnType.Length;
            totalDigitsQuantity.WordToGetValue = "totalDigitsQuantity";
        }

        

    }
















    public class AnalysisLogicDataArrays : IAnalysisLogicDataArrays
    {
        private readonly IAllBookData _bookData;
        private readonly IMessageService _msgService;

        readonly private int showMessagesLevel;
        readonly private int showMessagesLocal;
        readonly private string strCRLF;
        readonly private int chapterNumberTotalDigits;
        readonly private int paragraptNumberTotalDigits;
        readonly private int sentenceNumberTotalDigits;

        private string[] foundWordsOfParagraph;
        readonly private string[,] chapterNamesSamples;//два следующих массива собираются заменить этот двумерный
        readonly private string[] chapterNamesSamplesLanguage0;
        readonly private string[] chapterNamesSamplesLanguage1;
        readonly private string[] chapterMark;
        readonly private string[] stringMarksChapterEnd;
        readonly private string[] stringMarksParagraphBegin;
        readonly private string[] stringMarksParagraphEnd;
        readonly private string[] stringMarksSentenceBegin;
        readonly private string[] stringMarksSentenceEnd;
        readonly private string[] charsParagraphSeparator;
        readonly private string[] charsSentenceSeparators;
        readonly private string[] charsQuotesSeparator;
        readonly private string[] charsBracketsSeparator;
        readonly private string[] charsEllipsisToChange1;
        readonly private string[] charsEllipsisToChange2;
        readonly private string[] charsGroupsSeparators;
        readonly private string[] numbersOfGroupsNames;
        readonly private string[] allDigits;

        private string[] foundSymbolsOfParagraph;
        private int[,] chapterNamesVersionsCount;
        private int[] chapterSymbolsVersionsCount;
        private char[] foundCharsSeparator;
        private readonly int baseKeyWordFormsCount;

        public AnalysisLogicDataArrays(IAllBookData bookData, IMessageService msgService)
        {
            _bookData = bookData;
            _msgService = msgService;

            showMessagesLevel = DeclarationConstants.ShowMessagesLevel;
            strCRLF = DeclarationConstants.StrCRLF;
            showMessagesLocal = showMessagesLevel;
            showMessagesLocal = 3; //локальные печати класса выводятся на экран
            baseKeyWordFormsCount = 3;
            chapterNumberTotalDigits = 3;
            paragraptNumberTotalDigits = 5;
            sentenceNumberTotalDigits = 5;
            charsParagraphSeparator = new string[] { "\r\n" };//в строковом массиве - чтобы получать все константы одним методом

            //старый вариант, надо понемногу удалять
            charsSentenceSeparators = new string[] { ".", "…", "!", "?", ";" };//…\u2026 (Horizontal Ellipsis) ⁇\u2047 ⁈\u2048 ⁉\u2049 ‼\u203C
            charsQuotesSeparator = new string[] { "\u0022", "/", "\u02BA", "\u02EE", "\u02DD" };// "\u0022 ʺ\u02BA ˮ\u02EE ˝\u02DD - кавычки и скобки без деления на открывающие и закрывающие
            charsBracketsSeparator = new string[] { "()", "[]", "{}", "«»", "<>" };// - кавычки и скобки открывающие и закрывающие - «\u00AB »\u00BB

            charsEllipsisToChange1 = new string[] { "“”", "...", "!!!", "!!", "?!!", "?!", "!!?", "!?", "???", "??" };
            charsEllipsisToChange2 = new string[] { "«»", "…", "\u203C", "\u203C", "\u2048", "\u2048", "\u2049", "\u2049", "\u2047", "\u2047" };

            charsGroupsSeparators = new string[] { ".…!?;", "\u0022\u002F\u02BA\u02EE\u02DD", "()[]{}«»<>" };//…\u2026 (Horizontal Ellipsis) (\u002F - /) ⁇\u2047 ⁈\u2048 ⁉\u2049 ‼\u203C
            numbersOfGroupsNames = new string[] { "Sentence", "Quotes", "Brackets" }; //номера групп сепараторов для получения их значений в цикле

            allDigits = new string[] { "0123456789" };

            chapterMark = new string[] { "\u00A4\u00A4\u00A4\u00A4\u00A4", "\u00A4\u00A4\u00A4" };//¤¤¤¤¤ - метка строки перед началом названия главы, ¤¤¤ - метка строки после названия главы, еще \u007E - ~
            stringMarksChapterEnd = new string[] { "\u00A4\u00A4\u00A4" };//¤¤¤ - метка строки после названия главы, еще \u007E - ~
            stringMarksParagraphBegin = new string[] { "\u00A7\u00A7\u00A7\u00A7\u00A7" };//§§§§§ - метка строки перед началом абзаца
            stringMarksParagraphEnd = new string[] { "\u00A7\u00A7\u00A7" };//§§§ - метка строки после абзаца
            stringMarksSentenceBegin = new string[] { "\u00B6\u00B6\u00B6\u00B6\u00B6" };//¶¶¶¶¶ - метка строки перед началом предложния
            stringMarksSentenceEnd = new string[] { "\u00B6\u00B6\u00B6" };//¶¶¶ - метка строки после конца предложения            

            chapterNamesSamples = new string[,]//а номера глав бывают буквами!
            { { "chapter", "paragraph", "section", "subhead", "part" },
                { "глава", "параграф" , "раздел", "подраздел", "часть" }, };//можно разделить на два отдельных массива и тоже передавать через метод сепараторов - выбирая потом язык (прибавляя цифру языка к строке выбора)

            chapterNamesSamplesLanguage0 = new string[] { "chapter", "paragraph", "section", "subhead", "part" };
            chapterNamesSamplesLanguage1 = new string[] { "глава", "параграф", "абзац", "история", "сказание", "раздел", "подраздел", "часть" };

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

        public int GetIntConstant(string needConstantnName)//хорошо бы проверить, что не null
        {
            switch (needConstantnName)
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
                "EllipsisToChange":
                    //можно прямо тут сравнить длины charsEllipsisToChange1 и charsEllipsisToChange2 и, если сопадают возвращать что-то одно (все равно эту переменную спросят только пару раз за все время)
                    return charsEllipsisToChange1.Length;//EllipsisToChange                
                case
                "Groups":
                    return charsGroupsSeparators.Length;
                case
                "GroupsNumbers":
                    return numbersOfGroupsNames.Length;
                case
                "Paragraph":
                    return charsParagraphSeparator.Length;
                case
                "ChapterMark":
                    return chapterMark.Length;
                case
                "ChapterTotalDigits":
                    return chapterNumberTotalDigits;
                case
                "ParagraphBegin":
                    return stringMarksParagraphBegin.Length;
                case
                "ParagraphEnd"://заменить на ParagraphTotalDigits
                    return stringMarksParagraphEnd.Length;//заменить на paragraptNumberTotalDigits
                case
                "SentenceBegin":
                    return stringMarksSentenceBegin.Length;
                case
                "SentenceEnd"://заменить на SentenceTotalDigits
                    return stringMarksSentenceEnd.Length;//заменить на sentenceNumberTotalDigits
                case                    
                "NamesSamples0":
                    return chapterNamesSamplesLanguage0.Length;
                case                    
                "NamesSamples1":
                    return chapterNamesSamplesLanguage1.Length;
                case
                "baseKeyWordFormsCount":
                    return baseKeyWordFormsCount;
            }
            return 0;
        }

        public string[] GetStringArrConstant(string needConstantnName)//сделать три перегрузки - с выдачей разных типов?
        {//Отличия только типами возвращаемых значений методами недостаточно для перегрузки, но если методы отличаются параметрами, тогда перегружаемые методы могут иметь и различные типы возвращаемых значений
            switch (needConstantnName)
            {//тогда вариант с FSM - три разных метода с возвратом разных типов и определять, какой вызвать по типу используемой константы
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
                "EllipsisToChange1":                    
                    return charsEllipsisToChange1;
                case
                "EllipsisToChange2":
                    return charsEllipsisToChange2;
                case
                "Groups":
                    return charsGroupsSeparators;
                case
                "GroupsNumbers":
                    return numbersOfGroupsNames;
                case
                "AllDigitsIn0":
                    return allDigits;
                case
                "Paragraph":
                    return charsParagraphSeparator;
                case
                "ChapterMark":
                    return chapterMark;
                case
                "Free1":
                    return stringMarksChapterEnd;
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
            return baseKeyWordFormsCount;
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
