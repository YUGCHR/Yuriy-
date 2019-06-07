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
            
            

            countSentencesNumber = FindSeparatorsInText(textParagraph);//или получим другое значение - подумать

            return countSentencesNumber;
        }

        public int FindSeparatorsInText(string textParagraph)
        {
            List<List<char>> charsAllSeparators = new List<List<char>>
            {
                new List<char>(),
                new List<char>(),                
                new List<char>()
            };

            //проверять, сепараторы из одного символа или из двух, если из двух - то разделять на отдельные строки List - например
            //тогда все доставания констант и передачу их в массив char можно сделать в цикле - определения для метода GetConstantWhatNot тоже в массив и через него же доставать

            int sGroupCount = 3; //количество групп сепараторов
            //достать и сформировать временный массив сепараторов (разделителей предложений) - нулевой
            int charsSentenceSeparatorLength = GetConstantWhatNotLength("Sentence");
            string[] stringArraySentencesSeparators = new string[charsSentenceSeparatorLength];
            stringArraySentencesSeparators = GetConstantWhatNot("Sentence");//вариант многоточия из обычных точек надо обрабатывать отдельно - просто проверить, нет ли трех точек подряд
            string stringSentencesSeparators = String.Join("", stringArraySentencesSeparators);
            charsAllSeparators[0].AddRange(stringSentencesSeparators.ToCharArray());

            //достать и сформировать временный массив кавычек-скобок - первый
            int charsQuotesTypesLength = GetConstantWhatNotLength("Quotes");
            string[] stringArrayQuotesTypes = new string[charsQuotesTypesLength];
            stringArrayQuotesTypes = GetConstantWhatNot("Quotes");
            string stringQuotesTypes = String.Join("", stringArrayQuotesTypes);
            charsAllSeparators[1].AddRange(stringQuotesTypes.ToCharArray());

            //достать и сформировать временный массив откр-закр кавычек-скобок - второй и третий
            int charsBraketsTypesLength = GetConstantWhatNotLength("Brackets");
            string[] stringArrayBraketsTypes = new string[charsBraketsTypesLength];
            //string[] stringArrayBraketsClosingTypes = new string[charsBraketsTypesLength];
            stringArrayBraketsTypes = GetConstantWhatNot("Brackets");

            //for (int t = 0; t < charsBraketsTypesLength; t++)//разделяем строковые константы с двумя символами на открывающий и закрывающий 
            //{
            //    string currentChar = stringArrayBraketsOpeningTypes[t];
            //    stringArrayBraketsOpeningTypes[t] = currentChar[0].ToString();
            //    stringArrayBraketsClosingTypes[t] = currentChar[1].ToString();
            //}
            string stringBraketsTypes = String.Join("", stringArrayBraketsTypes);
            //string stringBraketsClosingTypes = String.Join("", stringArrayBraketsClosingTypes);
            charsAllSeparators[2].AddRange(stringBraketsTypes.ToCharArray());            
            //charsAllSeparators[3].AddRange(stringBraketsClosingTypes.ToCharArray());
            
            //разделить сепараторы на 3 группы по номеру и сделать временный массив на 3 значения, где хранить количество найденных сепараторов
            List<List<int>> allIndexResults = new List<List<int>>//три динамических массива - по группам сепараторов и три раза пройти IndexOfAny - и все это в отдельный метод
            {
                new List<int>(),//нулевая строка
                new List<int>(),//добавление еще одной строки для группы сепарараторов "кавычки"
                new List<int>() //добавление еще одной строки для группы сепарараторов «откр-закр»
            };
            //int charCount = 0;
            //foreach(char ch in textParagraph)
            //{
            //    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "ch[" + charCount.ToString() + "] -->" + ch.ToString(), CurrentClassName, 3);
            //    charCount++;
            //}

            int allSeparatorsQuantity = 0;
            int startFindIndex = 0;
            int[] separatorsQuantity = new int[sGroupCount];
            for (int sGroup = 0; sGroup < sGroupCount; sGroup++)
            {
                char[] charsCurrentGroupSeparators = charsAllSeparators[sGroup].ToArray();
                string printCharsCurrentGroupSeparators = new string(charsCurrentGroupSeparators);
                startFindIndex = 0;
                int indexResult = textParagraph.IndexOfAny(charsCurrentGroupSeparators, startFindIndex);//ищем первый по порядку сепаратор - для запуска while
                while (indexResult != -1)//если одни нашлись (уже не -1), собираем все остальные сепараторы, пока находятся
                {
                    allIndexResults[sGroup].Add(indexResult);//сохраняем индекс в массиве в нулевой строке
                    startFindIndex = indexResult + 1;//начинаем новый поиск с места найденных кавычек (наверное, тут надо добавить +1)
                    separatorsQuantity[sGroup]++; //считаем сепараторы
                    indexResult = textParagraph.IndexOfAny(charsCurrentGroupSeparators, startFindIndex);//Значение –1, если никакой символ не найдена. Индекс от нуля с начала строки, если любой символ найден
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "sGroup = " + sGroup.ToString() + strCRLF +
                        "separatorsQuantity = " + separatorsQuantity[sGroup].ToString() + strCRLF +
                        "printCharsCurrentGroupSeparators --> " + printCharsCurrentGroupSeparators + strCRLF +
                        "NEW startFindIndex (+ indexResultQuotes) = " + startFindIndex.ToString(), CurrentClassName, showMessagesLevel);
                }
            }
            //сначала проверяем содержание отк/закр разделителей
            //есть ли в абзаце что-то открывающее
            //int checkedSeparatorsGroup = 2;
            for (int checkedSeparatorsGroup = sGroupCount - 1; checkedSeparatorsGroup > 0; checkedSeparatorsGroup--)
            {
                if (allIndexResults[checkedSeparatorsGroup] != null)
                {
                    int removeSeparatorsCount = RemoveSentencesSeparatorsBeetweenQuotes(allIndexResults, checkedSeparatorsGroup);

                }
            }
            //краевые эффекты - не находит разделители в конце абзаца
            
            //кроме этого, надо нормировать текст - убрать пробелы между ?! " . - и другие варианты
            
            int allIndexResults0Count = allIndexResults[0].Count();//проверить следующий за сепаратором символ - там может быть кавычка (и вообще, проверить есть ли там пробел и заглавная буква, если последний - это не проверять)
            
            for (int i = allIndexResults0Count-1; i >= 0; i--)
            {
                if (allIndexResults[0][i] < 0)
                {
                    allIndexResults[0].RemoveAt(i);
                }
            }

            allIndexResults0Count = allIndexResults[0].Count();
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "allIndexResults = " + strCRLF +
                allIndexResults[0][0].ToString() + strCRLF +                 
                "allIndexResults0Count = " + allIndexResults0Count.ToString(), CurrentClassName, 3);

            int textParagraphLength = textParagraph.Length;
            allIndexResults0Count = allIndexResults[0].Count();
            int startIndexSentence = 0;
            int lengthSentence = 0;
            string[] paragraphSentences = new string[allIndexResults0Count];
            for (int i = 0; i < allIndexResults0Count; i++)
            {//string Substring (int startIndex, int length);                
                lengthSentence = allIndexResults[0][i] - startIndexSentence;
                paragraphSentences[i] = textParagraph.Substring(startIndexSentence, lengthSentence);
                startIndexSentence = startIndexSentence + lengthSentence;//и еще +1?

                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "textParagraph - " + textParagraph + strCRLF +
                            "paragraphSentences[" + i.ToString() + "] - " + paragraphSentences[i], CurrentClassName, 3);

            }


            int allSeparatorsQuantityMinus = 0;//временная проверка - потом убрать

            for (int i = 0; i < sGroupCount; i++)
            {
                int indexResultsGroupCount = allIndexResults[i].Count();
                for (int j = 0; j < indexResultsGroupCount; j++)
                {
                    if (allIndexResults[i][j] >= 0)
                    {
                        allSeparatorsQuantityMinus++;
                        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "allIndexResults[" + i.ToString() + "][" + j.ToString() + "] = " + allIndexResults[i][j].ToString() + strCRLF +
                            "allSeparatorsQuantityMinus " + allSeparatorsQuantityMinus.ToString(), CurrentClassName, showMessagesLevel);
                    }
                }
            }


            //собираем массив Quotes

            //Dictionary<int, string> searchResultQuotes = new Dictionary<int, string>(10);//начальная емкость списка - скажем 10, типа 5 пар кавычек
            //int quotesQuantity = searchResultQuotes.Count;//просто инициализовали allIndexesQuotesLength, ничего личного
            //Dictionary<int, string> searchResultSeparators = new Dictionary<int, string>(20);//начальная емкость списка - скажем, на 20 предложений                     
            //int separatorsQuantity = searchResultSeparators.Count;


            //int quotesTypesQuantity = SelectAllQuotesOrSeparatorsInText(textParagraph, searchResultQuotes, charsQuotesTypes);//нашли все кавычки, всех типов, занесли в словарь
            //int separatorsTypesQuantity = SelectAllQuotesOrSeparatorsInText(textParagraph, searchResultSeparators, charsSentencesSeparators);//нашли все сепараторы, всех типов, занесли в свой словарь

            //quotesQuantity = searchResultQuotes.Count;
            //separatorsQuantity = searchResultSeparators.Count;

            ////тут заносим все сепараторы из словаря во временный массив? 



            //if ((quotesQuantity & 1) == 0) //Console.WriteLine("Четное") - to check the quantity of quotes is even, if it is by contrast impair, will ask users or place the sentence in promlem list
            //{
            //    //int endIndexQuotes = 0;
            //    switch (quotesTypesQuantity)
            //    {
            //        case
            //        0:
            //            return (int)MethodFindResult.NothingFound;//не нашли никаких кавычек - чего тогда звали?
            //        case
            //        1://нашли один тип кавычек, вызываем анализ предложения
            //            int endIndexQuotes = CollectTextInQuotes(textParagraph, searchResultQuotes, quotesTypesQuantity, charsSentencesSeparators, charsSentenceSeparatorLength);//quotesTypesQuantity = 1
            //            return quotesQuantity;
            //        case
            //        2://нашли (наверное) сдвоенный тип кавычек, вызываем анализ предложения с открывающими и закрывающими кавычками                    
            //            //endIndexQuotes = CollectTextInQuotes(textParagraph, searchResultQuotes, quotesTypesQuantity); //quotesTypesQuantity = 1, вызываем метод обработки разных кавычек
            //            return quotesQuantity;
            //    }
            //}
            //else
            //{
            //    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "textParagraph -- > " + textParagraph + strCRLF + strCRLF +
            //        "Found Quotes Types Quantity is impair = " + quotesQuantity.ToString(), CurrentClassName, 3);// impair (общ.) наносить ущерб, (бур.) - нечётное число
            //    return quotesQuantity;
            //}
            ////если сюда попали, то число типов кавычек больше 2-х
            //_msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "textParagraph -- > " + textParagraph + strCRLF + strCRLF +
            //        "Found Quotes Types Quantity is too much = " + quotesTypesQuantity.ToString(), CurrentClassName, 3);
            allSeparatorsQuantity = allSeparatorsQuantityMinus;
            return allSeparatorsQuantity;
        }

        public int RemoveSentencesSeparatorsBeetweenQuotes(List<List<int>> allIndexResults, int checkedSeparatorsGroup)
        {
            int removeSeparatorsCount = 0;
            int allIndexResults0Count = allIndexResults[0].Count();
            int indexSentencesCount = allIndexResults0Count;
            int allIndexResults2Count = allIndexResults[checkedSeparatorsGroup].Count();//тут проверить, что четное!
                                                                   
            for (int s = 0; s < allIndexResults2Count - 1; s = s + 2)
            {//проверить наличие сепараторов нулевой группы между отк-закр и при наличии таковых - удалить
                int startIndexQuotes = allIndexResults[checkedSeparatorsGroup][s];
                int finishIndexQuotes = allIndexResults[checkedSeparatorsGroup][s + 1];

                if (finishIndexQuotes > startIndexQuotes)
                {//достать в цикле все индексы сепараторов предложений и проверить их на попадание в диапазон
                    for (int indexSentences = 0; indexSentences < indexSentencesCount; indexSentences++)
                    {
                        int currentIndexSentences = allIndexResults[0][indexSentences];
                        if (currentIndexSentences >= finishIndexQuotes - 2 && currentIndexSentences < finishIndexQuotes) allIndexResults[0][indexSentences] = finishIndexQuotes + 1;//переносим точку разделения предложений с сепаратора предложений на (за!) кавычки, чтобы ничего не терялось
                            if (currentIndexSentences > startIndexQuotes && currentIndexSentences < finishIndexQuotes - 2)//если последний символ перед кавычками - разделитель (и возможно с пробелом), то его оставляем
                        {
                            allIndexResults[0][indexSentences] = allIndexResults[0][indexSentences] * -1;//чтобы не менять количество разделителей, делаем его отрицательным (или надо идти с конца массива, если Remove ненужные)
                            removeSeparatorsCount++;
                            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Brakets Index = " + s.ToString() + strCRLF +
                                "startIndexQuotes = " + startIndexQuotes.ToString() + strCRLF +
                                "finishIndexQuotes = " + finishIndexQuotes.ToString(), CurrentClassName, showMessagesLevel);
                        }
                    }
                }
            }            
            return removeSeparatorsCount;
        }

        public int SelectAllQuotesOrSeparatorsInText(string textParagraph, Dictionary<int, string> searchResultQuotes, string[] charsQuotesOrSeparators)
        {
            int textParagraphLength = textParagraph.Length;
            int startFindIndex = 0;
            int finishFindIndex = textParagraphLength;
            int indexResultQuotes = 0;            
            int quotesTypesQuantity = 0;//общее количество найденных типов
            int quotesQuantity = 0;//общее количество найденных чего искали всех типов
            int charsQuotesOrSeparatorLength = charsQuotesOrSeparators.Length;
            for (int i = 0; i < charsQuotesOrSeparatorLength; i++)
            {
                startFindIndex = 0;
                finishFindIndex = textParagraphLength;//сброс точки старта и длины текста перед поиском нового варианта кавычек
                indexResultQuotes = textParagraph.IndexOf(charsQuotesOrSeparators[i], startFindIndex, finishFindIndex);//ищем, есть ли кавычки (первые по по порядку) - для запуска while

                if (indexResultQuotes > -1)
                {
                    quotesTypesQuantity++;
                }
                while (indexResultQuotes != -1)//если одни нашлись (уже не -1), собираем все остальные кавычки, пока находятся
                {
                    searchResultQuotes.Add(indexResultQuotes, charsQuotesOrSeparators[i]);//сохраняем индекс в тексте и тип найденных кавычек

                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "textParagraph [" + i.ToString() + "] -- > " + textParagraph + strCRLF + strCRLF +
                        "finishFindIndex (textLengh) = " + finishFindIndex.ToString() + strCRLF +
                        "startFindIndex (start of the search) = " + startFindIndex.ToString() + strCRLF +
                        "indexResultQuotes (quotes found on position No.) = " + indexResultQuotes.ToString() + strCRLF +
                        "charsQuotesSeparator[i] --> " + charsQuotesOrSeparators[i] + strCRLF +
                        "searchResultQuotes value on [" + indexResultQuotes.ToString() + "] = " + searchResultQuotes[indexResultQuotes], CurrentClassName, showMessagesLevel);

                    startFindIndex = indexResultQuotes + 1;//начинаем новый поиск с места найденных кавычек (наверное, тут надо добавить +1)
                    finishFindIndex = textParagraphLength - startFindIndex;//надо каждый раз вычитать последнюю найденную позицию из полной длины текста, а не остатка
                    quotesQuantity++; //считаем кавычки

                    indexResultQuotes = textParagraph.IndexOf(charsQuotesOrSeparators[i], startFindIndex, finishFindIndex);//Значение –1, если строка не найдена. Индекс от нуля с начала строки, если строка найдена
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "NEW finishFindIndex (- indexResultQuotes) = " + finishFindIndex.ToString() + strCRLF +
                        "NEW startFindIndex (+ indexResultQuotes) = " + startFindIndex.ToString(), CurrentClassName, showMessagesLevel);
                }
            }
            return quotesTypesQuantity;
        }

        public int CollectTextInQuotes(string textParagraph, Dictionary<int, string> searchResultQuotes, int quotesTypesQuantity, string[] charsSentencesSeparators, int charsSentenceSeparatorLength)
        {
            //проверить символы-сепараторы до и после закрывающих кавычек, а также между закрывающей и следующей открывающей, все как-то занести в общий массив
            //или собрать все символы-сепараторы в отдельный массив, а потом уже анализировать все вместе

            //добавить сюда константы формирования номера предложения

            int quotesQuantity = searchResultQuotes.Count;
            int[,] allPairsQuotes = new int[2, quotesQuantity/2];//в нулевой строке массива индекс (по тексту абзаца) открывающей кавычки, в первой - закрывающей

            string[] allSentencesInParagraph = new string[quotesQuantity];//спорная размерность массива, скорее, надо динамический
            int sentenceNumber = 0;//индекс массива allSentencesInParagraph

            int endIndexQuotes = (int)MethodFindResult.NothingFound; ;//если -1 - текст в кавычках не найден, придумать более подходящее название переменной

            switch (quotesTypesQuantity)
            {
                case
                0:
                    return (int)MethodFindResult.NothingFound;//не нашли никаких кавычек - чего тогда звали? не может быть такого варианта - можно убрать
                case
                1://нашли один тип кавычек
                    int forwardOrBacksparkQuote = 0;
                    int i = 0;
                    int j = 0;
                    foreach (int indexQuotes in searchResultQuotes.Keys)
                    {
                        forwardOrBacksparkQuote = i & 1;
                        allPairsQuotes[forwardOrBacksparkQuote, j] = indexQuotes;//записали открывающие и закрывающие кавычки в разные линейки массива
                        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "indexQuotes = " + indexQuotes.ToString() + strCRLF +
                            "i = " + i.ToString() + strCRLF +
                            "j = " + j.ToString() + strCRLF +
                            "forwardOrBacksparkQuote = " + forwardOrBacksparkQuote.ToString(), CurrentClassName, 3);
                        if ((i & 1) == 1) j++;
                        i++;
                    }
                    int forwardQuoteIndex = 0;//индекс открывающей кавычки
                    int backsparkQuoteIndex = 0;//индекс закрывающей кавычки
                    int indexResultSeparatorsBeforeQuoute = -1;//инициализация отрицанием
                    int indexResultSeparatorsAfterQuoute = -1;
                    //int indexResultSeparatorsNearQuoute = -1;
                    for (int n = 0; n < (quotesQuantity / 2); n++)
                    {                        
                        forwardQuoteIndex = allPairsQuotes[0, n];//достаем пару кавычек из массива - открывающую и закрывающую
                        backsparkQuoteIndex = allPairsQuotes[1, n];
                        //int startFindIndexNearQuoute = backsparkQuoteIndex - 1;//старт поиска за один символ до закрывающей кавычки
                        //int charCountForCheking = 3; //длина поиска - символ до кавычки, кавычка и символ после кавычки
                        for (int t = 0; t < charsSentenceSeparatorLength; t++)
                        {
                            indexResultSeparatorsBeforeQuoute = textParagraph.IndexOf(charsSentencesSeparators[t], forwardQuoteIndex, 1);//ищем сепараторы перед кавычкой
                            indexResultSeparatorsAfterQuoute = textParagraph.IndexOf(charsSentencesSeparators[t], forwardQuoteIndex+1, 1);//ищем сепараторы после кавычки
                        }
                        //проверяем сеператор сразу после кавычки - если есть, то переписываем предложение в окончательный массив по кавычке + символ-сепаратор после нее
                        if (indexResultSeparatorsAfterQuoute > 0)//нуль быть не может, потому что закрывающая кавычка и была найдена пара
                        {                            
                            allSentencesInParagraph[sentenceNumber] = textParagraph.Substring(forwardQuoteIndex, backsparkQuoteIndex + 1); //string substring = value.Substring(startIndex, length);
                        }
                        else//сеператор сразу после кавычки не найден - тогда проверяем, был ли сепаратор перед кавычкой
                        {
                            if (indexResultSeparatorsBeforeQuoute > 0)//если был сепаратор перед кавычкой, то переписываем найденное предложение, включая кавычку
                            {
                                allSentencesInParagraph[sentenceNumber] = textParagraph.Substring(forwardQuoteIndex, backsparkQuoteIndex); //string substring = value.Substring(startIndex, length);
                            }
                        }
                        

                    }
                    //pair.Key, pair.Value
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), 
                        "allPairsQuotes[0,0] = " + allPairsQuotes[0, 0].ToString() + strCRLF +
                        "allPairsQuotes[1,0] = " + allPairsQuotes[1, 0].ToString() + strCRLF + 
                        "allPairsQuotes[0,1] = " + allPairsQuotes[0, 1].ToString() + strCRLF +
                        "allPairsQuotes[1,1] = " + allPairsQuotes[1, 1].ToString() + strCRLF +
                        "allPairsQuotes[0,2] = " + allPairsQuotes[0, 2].ToString() + strCRLF +
                        "allPairsQuotes[1,2] = " + allPairsQuotes[1, 2].ToString() + strCRLF +                        
                        "quotesQuantity = " + quotesQuantity.ToString(), CurrentClassName, 3);
                    i++;
                    return quotesTypesQuantity; //endIndexQuotes;
                case
                2://нашли (наверное) сдвоенный тип кавычек - проверяем, так ли это (или раньше надо было проверить?)
                    
                    return endIndexQuotes;
            }


            //кусок абзаца от кавычки до кавычки проверить на окончания предложения, кроме последнего символа перед кавычкой
            //если нет окончаний, проверить последний символ перед кавычкой и первый после кавычки
            //если там есть, то закончить предложение, если нет - искать сепараторы до первого найденного, там закончить предложение - каши не дошли до следующей кавычки
            //если началась следующая - опять этот же метод - рекурсия, что ле?
            //если в кавычках несколько сепараторов, дня начала проверить, что на второй кавычке есть сепаратор, сразу до или после
            //если есть, то проверить, что во всех предложениях внутри кавычек меньше 5-ти слов
            //если меньше, собрать предложение и перейти к следующему
            //если больше - думать (может, не пяти слов, а 10-ти?)

            return quotesTypesQuantity;// endIndexQuotes; //-1 - текст в кавычках не найден - или вторые кавычки не найдены? в общем, засада и что-то надо делать
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

