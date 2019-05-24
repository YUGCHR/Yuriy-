using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;
using System.Collections;

namespace TextSplit
{
    public interface IAnalysisLogicSentences
    {
        int PrepareToDividePagagraphToSentences(int desiredTextLanguage, string currentParagraph, int currentChapterNumber, int currentParagraphNumber, int i);

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

        int GetConstantWhatNotLength(string ParagraphOrSentence) => _arrayAnalysis.GetConstantWhatNotLength(ParagraphOrSentence);
        string[] GetConstantWhatNot(string ParagraphOrSentence) => _arrayAnalysis.GetConstantWhatNot(ParagraphOrSentence);

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

        public int PrepareToDividePagagraphToSentences(int desiredTextLanguage, string currentParagraph, int currentChapterNumber, int currentParagraphNumber, int i)
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

        public int DividePagagraphToSentences(int desiredTextLanguage, string textParagraph, int currentChapterNumber, int currentParagraphNumber)
        {//в textParagraph получаем nextParagraph при вызове метода - следующий абзац с текстом после метки номера абзаца в пустой строке
            int countSentencesNumber = 0;
            int textParagraphLength = textParagraph.Length;
            //достать и сформировать временный массив разделителей предложений из _arrayAnalysis (через метод GetCharsSeparator)
            int charsSentenceSeparatorLength = GetConstantWhatNotLength("Sentence");
            string[] charsSentenceSeparator = new string[charsSentenceSeparatorLength];
            charsSentenceSeparator = GetConstantWhatNot("Sentence");//хорошо бы проверить, что не null

            return countSentencesNumber;
        }

        public int FindQuotesInText(string textParagraph)
        {//собираем массив Quotes
            int textParagraphLength = textParagraph.Length;
            int charsQuotesSeparatorLength = GetConstantWhatNotLength("Quotes");//хорошо бы проверить, что массив не null
            string[] charsQuotesSeparator = new string[charsQuotesSeparatorLength];//если будет нужен массив char[], то сделать так же, как с ParagraphSeparators - или сразу же так сделать для однообразия?
            charsQuotesSeparator = GetConstantWhatNot("Quotes");
            //char[] charArrQuotesSeparator = charsQuotesSeparator;
            Dictionary<int, string> searchResultQuotes = new Dictionary<int, string>(10);//начальная емкость списка - скажем 10, типа 5 пар кавычек
            //List<int> searchResultQuotes = new List<int>() 
            int startFindIndex = 0;
            int finishFindIndex = textParagraphLength;
            int indexResultQuotes = 0;
            int quotesTypesQuantity = 0;//общее количество найденных типов кавычек - в идеале должно быть 1 или 2 (если разные открывающие и закрывающие), если больше - пока не будем обрабатывать
            int quotesQuantity = 0;//общее количество найденных кавычек всех типов
            
            for (int i = 0; i < charsQuotesSeparatorLength; i++)
            {
                startFindIndex = 0;
                finishFindIndex = textParagraphLength;//сброс точки старта и длины текста перед поиском нового варианта кавычек
                indexResultQuotes = textParagraph.IndexOf(charsQuotesSeparator[i], startFindIndex, finishFindIndex);//ищем, есть ли кавычки (первые по по порядку) - для запуска while               

                if (indexResultQuotes > -1)
                {
                    quotesTypesQuantity++;                    
                }
                while (indexResultQuotes != -1)//если одни нашлись (уже не -1), собираем все остальные кавычки, пока находятся
                {
                    searchResultQuotes.Add(indexResultQuotes, charsQuotesSeparator[i]);//сохраняем индекс в тексте и тип найденных кавычек

                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "textParagraph [" + i.ToString() + "] -- > " + textParagraph + strCRLF + strCRLF +
                        "finishFindIndex (textLengh) = " + finishFindIndex.ToString() + strCRLF +
                        "startFindIndex (start of the search) = " + startFindIndex.ToString() + strCRLF +
                        "indexResultQuotes (quotes found on position No.) = " + indexResultQuotes.ToString() + strCRLF +
                        "charsQuotesSeparator[i] --> " + charsQuotesSeparator[i] + strCRLF +
                        "searchResultQuotes value on [" + indexResultQuotes.ToString() + "] = " + searchResultQuotes[indexResultQuotes], CurrentClassName, showMessagesLevel);                    

                    startFindIndex = indexResultQuotes + 1;//начинаем новый поиск с места найденных кавычек (наверное, тут надо добавить +1)
                    finishFindIndex = textParagraphLength - startFindIndex;//надо каждый раз вычитать последнюю найденную позицию из полной длины текста, а не остатка
                    quotesQuantity++; //считаем кавычки

                    indexResultQuotes = textParagraph.IndexOf(charsQuotesSeparator[i], startFindIndex, finishFindIndex);//Значение –1, если строка не найдена. Индекс от нуля с начала строки, если строка найдена
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "NEW finishFindIndex (- indexResultQuotes) = " + finishFindIndex.ToString() + strCRLF +
                        "NEW startFindIndex (+ indexResultQuotes) = " + startFindIndex.ToString(), CurrentClassName, showMessagesLevel);
                }
            }            

            switch (quotesTypesQuantity)
            {
                case
                0:
                    return (int)MethodFindResult.NothingFound;//не нашли никаких кавычек - чего тогда звали?
                case
                1://нашли один тип кавычек, вызываем анализ предложения
                    int endIndexQuotes = collectTextInQuotes(textParagraph, searchResultQuotes);
                    return quotesQuantity;
                case
                2://нашли (наверное) сдвоенный тип кавычек, вызываем анализ предложения с открывающими и закрывающими кавычками
                    //int endIndexQuotes = collectTextInQuotes(textParagraph, searchResultQuotes); //вызываем метод обработки разных кавычек
                    return quotesQuantity;
            }

            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "textParagraph -- > " + textParagraph + strCRLF + strCRLF +
                    "Found Quotes Types Quantity is too much = " + quotesTypesQuantity.ToString(), CurrentClassName, 3);

            return quotesQuantity;
        }        

        public int collectTextInQuotes(string textParagraph, Dictionary<int, string> searchResultQuotes)
        {//получили текст абзаца, найденные кавычки, позиция кавычек, длина текста (наверное, не нужна?)
            int endIndexQuotes = (int)MethodFindResult.NothingFound; ;//если -1 - текст в кавычках не найден, если найден - индекс закрывающих кавычек, потом искать начиная с него+1


            
            //ищем первый символ - букву, потом проверяем, что она заглавная


            return (int)MethodFindResult.NothingFound; //-1 - текст в кавычках не найден - или вторые кавычки не найдены? в общем, засада и что-то надо делать
        }

        //искать будем методом IndexOf(String, Int32, Int32) - Возвращает индекс с отсчетом от нуля первого вхождения значения указанной строки в данном экземпляре. Поиск начинается с указанной позиции знака; проверяется заданное количество позиций
        //еще лучше IndexOf(String, Int32) - Возвращает индекс с отсчетом от нуля первого вхождения значения указанной строки в данном экземпляре. Поиск начинается с указанной позиции знака
        //или можно поменять на IndexOf(String, Int32, StringComparison) - Возвращает индекс с отсчетом от нуля первого вхождения указанной строки в текущем объекте String. Параметры задают начальную позицию поиска в текущей строке и тип поиска
        //правила раздела текста на предложения -
        //начинаем нумерацию предложений - в этом абзаце и общую в тексте (может быть во всем тексте или метод вызвали для отдельной главы?)
        //тогда не надо перегонять в char - сразу ищем вхождения символов (сохраняем строку абзаца в массив char, вычисляем его длину и проверяем текст на разделители (включая варианты кавычек? или они отдельным массивом - хлопотнее, но выгоднее)
        //смотрим первый символ - str[0] - если это строчная буква - IsUpper(String, Int32) - , вызываем метод, собирающий все предложение, но пока делаем на месте - сохраняем посимвольно в строку
        //можно в начале сделать быструю проверку строки на все варианты кавычек, если кавычек нет, то и хорошо, а если есть, то какие-то одни конкретные (и сразу посчитать количество) и потом только их и искать (найденные кавычки = кавычки[index])
        //проверка содержания всех кавычек - 
        //bool contains = nextParagraph.Any(s => s.Contains(pattern)); - непонятно, как проверять сразу весь список кавычек
        //if (nextParagraph.Any(s => s.Contains(stringToCheck)))
        //{
        //    //do your stuff here
        //}
        //если первый символ - кавычки (во всех вариантах) вызываем метод определения строки в кавычках (как совместить кавычки с остальными разделителями в едином цикле перебора? или сделать два разных?)

        //если первый символ не прописная буква и не кавычка, то это может быть тире (то же самое, что и с прописной), цифра (непонятно, надо смотреть варианты) или что еще?
        //потом ищем символ-разделитель, который заканчивает предложение, после него смотрим, чтобы был пробел и следующая буква была строчной (варианты - тире, цифра?)
        //если встретили опять кавычки, но в середине строки, то тоже вызываем метод определения строки в кавычках, пусть сам разбирается, он умный (должен быть)
        //сначала ищем все варианты кавычек, текст в кавычках сразу сохраняем, как предложение, потом проверяем, какая буква после закрывающей кавычки, если строчная, то продолжаем искать разделитель

        //если встретился разделитель, опять же проверяем, что за ним строчная буква




        //_msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "CurrentParagraph [" + i.ToString() + "] -- > " + nextParagraph, CurrentClassName, 3);









    







public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}

