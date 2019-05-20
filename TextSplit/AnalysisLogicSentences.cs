using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;

namespace TextSplit
{
    public interface IAnalysisLogicSentences
    {
        int PrepareToDividePagagraph(int desiredTextLanguage, string currentParagraph, int currentChapterNumber, int currentParagraphNumber, int i);

        //event EventHandler AnalyseInvokeTheMain;
    }

    public class AnalysisLogicSentences : IAnalysisLogicSentences
    {
        private readonly IAllBookData _bookData;
        private readonly IMessageService _msgService;
        private readonly IAnalysisLogicCultivation _analysisLogic;
        private readonly IAnalysisLogicDataArrays _arrayAnalysis;        

        readonly private int filesQuantity;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;

        int GetParagraphTextLength(int desiredTextLanguage) => _bookData.GetParagraphTextLength(desiredTextLanguage);
        string GetParagraphText(int paragraphCount, int desiredTextLanguage) => _bookData.GetParagraphText(paragraphCount, desiredTextLanguage);
        int SetParagraphText(string paragraphText, int paragraphCount, int desiredTextLanguage) => _bookData.SetParagraphText(paragraphText, paragraphCount, desiredTextLanguage);

        int GetCharsSeparatorLength(string ParagraphOrSentence) => _arrayAnalysis.GetCharsSeparatorLength(ParagraphOrSentence);
        char[] GetCharsSeparator(string ParagraphOrSentence) => _arrayAnalysis.GetCharsSeparator(ParagraphOrSentence);

        string AddSome00ToIntNumber(string currentChapterNumberToFind, int totalDigitsQuantity) => _analysisLogic.AddSome00ToIntNumber(currentChapterNumberToFind, totalDigitsQuantity);

        //public event EventHandler AnalyseInvokeTheMain;

        public AnalysisLogicSentences(IAllBookData bookData, IMessageService msgService, IAnalysisLogicCultivation analysisLogic, IAnalysisLogicDataArrays arrayAnalysis)
        {
            _bookData = bookData;
            _msgService = msgService;
            _analysisLogic = analysisLogic;//общая логика
            _arrayAnalysis = arrayAnalysis;

            filesQuantity = DeclarationConstants.FilesQuantity;
            showMessagesLevel = DeclarationConstants.ShowMessagesLevel;
            strCRLF = DeclarationConstants.StrCRLF;            
        }

        public int PrepareToDividePagagraph(int desiredTextLanguage, string currentParagraph, int currentChapterNumber, int currentParagraphNumber, int i)
        {
            int countSentencesNumber = 0;
            int paragraphTextLength = GetParagraphTextLength(desiredTextLanguage);            
            //проверяем, что текущий абзац - это номер абзаца, можно его достать еще раз через i и сравнить с полученным - что находимся в нужном месте
            string controlParagraph = GetParagraphText(i, desiredTextLanguage);
            bool theFollowingNeedToDivide = String.Equals(currentParagraph, controlParagraph);//тут правильно было бы добавить CultureInfo.CreateSpecificCulture(cultureName), но наверное не надо, строки должны совпадать в точности
            if (theFollowingNeedToDivide)
            {//достать следующий абзац (i+1) - это будет текст, который надо делить на предложения, только проверить, что i не превысит длину массива текста
                if ((i + 1) < paragraphTextLength)
                {
                    string nextParagraph = GetParagraphText(i + 1, desiredTextLanguage);
                    //тут-то и вызываем метод дележки - передавая ему текст и собранные номера главы и абзаца
                    //что вернуть-то? количество предложений = метод дележки(текст, номер главы, номер абзаца)
                    countSentencesNumber = DividePagagraphToSentences(desiredTextLanguage, nextParagraph, currentChapterNumber, currentParagraphNumber);
                }
            }
            




            //_msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "CurrentParagraph [" + i.ToString() + "] -- > " + currentParagraph, CurrentClassName, 3);
                

            
            return countSentencesNumber;
        }

        public int DividePagagraphToSentences(int desiredTextLanguage, string nextParagraph, int currentChapterNumber, int currentParagraphNumber)
        {
            int countSentencesNumber = 0;            
            //достать и сформировать временный массив разделителей предложений из _arrayAnalysis (через метод GetCharsSeparator)
            int charsSentenceSeparatorLength = GetCharsSeparatorLength("Sentence");
            char[] charsSentenceSeparator = new char[charsSentenceSeparatorLength];
            charsSentenceSeparator = GetCharsSeparator("Sentence");//хорошо бы проверить, что не null
            //собираем массив Quotes
            int charsQuotesSeparatorLength = GetCharsSeparatorLength("Quotes");
            char[] charsQuotesSeparator = new char[charsQuotesSeparatorLength];
            charsQuotesSeparator = GetCharsSeparator("Sentence");//хорошо бы проверить, что не null

            //правила раздела текста на предложения -
            //начинаем нумерацию предложений - в этом абзаце и общую в тексте (может быть во всем тексте или метод вызвали для отдельной главы?)
            //сохраняем строку абзаца в массив char, вычисляем его длину и проверяем текст на разделители (включая варианты кавычек? или они отдельным массивом - хлопотнее, но выгоднее)
            //смотрим первый символ - если это строчная буква, вызываем метод, собирающий все предложение, но пока делаем на месте - сохраняем посимвольно в строку
            //если первый символ - кавычки (во всех вариантах) вызываем метод определения строки в кавычках (как совместить кавычки с остальными разделителями в едином цикле перебора? или сделать два разных?)
            //если первый символ не прописная буква и не кавычка, то это может быть тире (то же самое, что и с прописной), цифра (непонятно, надо смотреть варианты) или что еще?
            //потом ищем символ-разделитель, который заканчивает предложение, после него смотрим, чтобы был пробел и следующая буква была строчной (варианты - тире, цифра?)
            //если встретили опять кавычки, но в середине строки, то тоже вызываем метод определения строки в кавычках, пусть сам разбирается, он умный (должен быть)
            //сначала ищем все варианты кавычек, текст в кавычках сразу сохраняем, как предложение, потом проверяем, какая буква после закрывающей кавычки, если строчная, то продолжаем искать разделитель

            //если встретился разделитель, опять же проверяем, что за ним строчная буква




            //_msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "CurrentParagraph [" + i.ToString() + "] -- > " + nextParagraph, CurrentClassName, 3);



            return countSentencesNumber;
        }








        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}

