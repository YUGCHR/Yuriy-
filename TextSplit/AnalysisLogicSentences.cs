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
        int DividePagagraphToSentencesAndEnumerate(int desiredTextLanguage);

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

        bool FindTextPartMarker(string currentParagraph, string stringMarkBegin) => _analysisLogic.FindTextPartMarker(currentParagraph, stringMarkBegin);
        int FindTextPartNumber(string currentParagraph, string stringMarkBegin, int totalDigitsQuantity) => _analysisLogic.FindTextPartNumber(currentParagraph, stringMarkBegin, totalDigitsQuantity);

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

        public int DividePagagraphToSentencesAndEnumerate(int desiredTextLanguage)
        {
            int totalSentencesCount = 0;
            int countSentencesNumber = 0;

            List<List<char>> charsAllDelimiters = new List<List<char>> { new List<char>(), new List<char>(), new List<char>() };//временный массив для хранения всех групп разделителей в виде char[] для IndexOfAny            
            //заполнили List разделителями из констант
            int sGroupCount = ConstanstListFillCharsDelimiters(charsAllDelimiters);//вернули количество групп разделителей (предложений, кавычки, скобки)

            //выбрать по очереди все абзацы, найти номер главы (а так же предисловие), потом в главе найти номер абзаца, достать абзац, получить и сравнить его номер (со счетчиком) и начать работать с предложениями
            int paragraphTextLength = GetParagraphTextLength(desiredTextLanguage);//нет, главу искать не будем, сразу ищем абзац - в его номере уже есть номер главы
            for (int currentParagraphIndex = 0; currentParagraphIndex < paragraphTextLength; currentParagraphIndex++)//перебираем все абзацы текста - тут надо запустить большой FSM и вызывать все методы отсюда
            {
                string currentParagraph = GetParagraphText(currentParagraphIndex, desiredTextLanguage);

                //состояние - есть маркер абзаца или нет, действие если есть - вызов PrepareToDividePagagraphToSentences, если нет - перейти к следующему абзацу

                bool foundParagraphMark = FindTextPartMarker(currentParagraph, "ParagraphBegin");//проверяем начало абзаца на маркер абзаца, если есть, то надо вызвать подготовку деления абзаца (она возьмет следующий абзац для деления на предложения)
                if (foundParagraphMark)
                {
                    string sentenceTextMarksWithOtherNumbers = FindPagagrapNumberForSentenceNumber(paragraphTextLength, currentParagraph, currentParagraphIndex);
                    if (sentenceTextMarksWithOtherNumbers != null)//если null, то текст закончился
                    {
                        string nextParagraph = GetParagraphText(currentParagraphIndex + 1, desiredTextLanguage);
                        
                        string[] paragraphSentences = SelectActionByDelimitersGroupState(nextParagraph, charsAllDelimiters, sGroupCount);////здесь делим текст на предложения! и вызываем селектор методов обработки состояний - согласно найденному состоянию

                        paragraphSentences = EnumerateDividedSentences(sentenceTextMarksWithOtherNumbers, paragraphSentences);//пронумеровали разделенные предложения - еще в том же массиве

                        //здесь предложения уже поделенные и с номерами, теперь слить их опять вместе, чтобы записать на то же самое место (хотя логичнее сделать List с предложениями - или структуру типа базы данных из List)
                        if (paragraphSentences != null)
                        {
                            currentParagraph = string.Join(strCRLF, paragraphSentences);//добавить ли в конце еще один перевод строки?
                            countSentencesNumber = paragraphSentences.Length;
                            SetParagraphText(currentParagraph, currentParagraphIndex + 1, desiredTextLanguage);//ЗДЕСЬ запись SetParagraphText! - записываем абзац с пронумерованными предложениями на старое место! проверить, что попадаем на нужное место, а не в предыдущую ячейку
                            totalSentencesCount = totalSentencesCount + countSentencesNumber;
                        }
                    }
                }
            }
            return totalSentencesCount;
        }

        private int ConstanstListFillCharsDelimiters(List<List<char>> charsAllDelimiters)
        {//вариант многоточия из обычных точек надо обрабатывать отдельно - просто проверить, нет ли трех точек подряд
            int sGroupCount = GetConstantWhatNotLength("Groups"); //получили количество групп разделителей - длина массива слов для получения констант разделителей
            string[] numbersOfGroupsNames = GetConstantWhatNot("Groups"); //new string[sGroupCount];            
            int allDelimitersCount = 0;//только для контроля правильности сбора разделителей - поставим Assert? сейчас всего 20 разделителей - а константах, а не в предложениях

            for (int g = 0; g < sGroupCount; g++)
            {
                string stringCurrentDelimiters = numbersOfGroupsNames[g];
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "stringCurrentDelimiters -> " + stringCurrentDelimiters, CurrentClassName, showMessagesLevel);
                charsAllDelimiters[g].AddRange(stringCurrentDelimiters.ToCharArray());
                allDelimitersCount = allDelimitersCount + charsAllDelimiters[g].Count;
            }
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "allDelimitersCount = " + allDelimitersCount.ToString(), CurrentClassName, showMessagesLevel);
            System.Diagnostics.Debug.Assert(allDelimitersCount == 20, "The total Delimiters count is WRONG!");
            return sGroupCount;
        }

        public string FindPagagrapNumberForSentenceNumber(int paragraphTextLength, string currentParagraph, int currentParagraphIndex)//когда заменять на FSM, надо позаботиться, чтобы desiredTextLanguage и currentParagraphIndex были доступны в самом низу
        {
            int totalDigitsQuantity5 = 5;//для номера абзаца - перенести в AnalysisLogicDataArrays
            int currentParagraphNumber = 0;            
            if ((currentParagraphIndex + 1) < paragraphTextLength)//на всякий случай проверим, что не уткнемся в конец файла
            {
                //§§§§§00003§§§-Paragraph-of-Chapter-3 - формат номера абзаца такой - взять currentParagraph, убрать позиции по длине ParagraphBegin + totalDigitsQuantity5 + ParagraphEnd, останется -Paragraph-of-Chapter-3

                currentParagraphNumber = FindTextPartNumber(currentParagraph, "ParagraphBegin", totalDigitsQuantity5);//тут уже знаем, что в начале абзаца есть нужный маркер и сразу ищем номер //ищем номер главы, перенести totalDigitsQuantity3 внутрь метода                

                if (currentParagraphNumber > 0) //избегаем предисловия, его как-нибудь потом поделим добавив else
                {
                    //тут сформируем всю маркировку для предложений, кроме собственно номера предложения - вместо 23 считать количество символов, как указано выше
                    string sentenceTextMarksWithOtherNumbers = currentParagraph.Remove(0, 23);//Возвращает новую строку, в которой было удалено указанное число символов в указанной позиции.
                    sentenceTextMarksWithOtherNumbers = "-of-Paragraph-" + currentParagraphNumber.ToString() + sentenceTextMarksWithOtherNumbers;//должно получиться -Paragraph-3-of-Chapter-3                    
                    return sentenceTextMarksWithOtherNumbers;
                }
            }
            return null;//сюда попадем, только если закончился общий текст
        }

        public string[] EnumerateDividedSentences(string sentenceTextMarksWithOtherNumbers, string[] paragraphSentences) //в textParagraph получаем nextParagraph при вызове метода - следующий абзац с текстом после метки номера абзаца в пустой строке
        {
            //лучше достать-получить метку абзаца из предыдущего            
            int countSentencesNumbers = paragraphSentences.Length;
            int currentSentenceNumber = 0;
            int currentChapterNumber = 0;//0 - означает первую главу (-1 - предисловие), в данном случае это не используется, так как достается номер главы из сформированного номера абзаца
            string stringToPutMarkBegin = "SentenceBegin";
            string stringToPutMarkEnd = "SentenceEnd";
            //теперь к каждому предложению сгенерировать номер, добавить спереди метку и номер, добавить EOL и метку в конце и потом все сложить обратно во внешний массив ParagraphText                                                                               
            //генерация номера и метки очень похожа во всех трех случаях - потом можно было сделать единым методом
            for (int i = 0; i < countSentencesNumbers; i++)
            {
                currentSentenceNumber = i + 1; //будем нумеровать с первого номера, а не с нулевого
                string sentenceTextMarks = CreatePartTextMarks(stringToPutMarkBegin, stringToPutMarkEnd, currentChapterNumber, currentSentenceNumber, sentenceTextMarksWithOtherNumbers);//создали базовую маркировку и номер текущего предложения - ¶¶¶¶¶00001¶¶¶-Paragraph-3-of-Chapter-3                
                paragraphSentences[i] = sentenceTextMarks + strCRLF + paragraphSentences[i] + strCRLF;
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "paragraphSentences[" + i.ToString() + "] --> " + strCRLF + paragraphSentences[i], CurrentClassName, showMessagesLevel);
            }
            return paragraphSentences;
        }

        public string CreatePartTextMarks(string stringMarkBegin, string stringMarkEnd, int currentUpperNumber, int enumerateCurrentCount, string sentenceTextMarksWithOtherNumbers)//сделать общим методом с созданием номера параграфа и убрать в дополнения
        {
            int totalDigitsQuantity5 = 5;//для номера предложения используем 5 цифр (до 999, должно хватить) - перенести в AnalysisLogicDataArrays
            string markPartTextBegin = GetConstantWhatNot(stringMarkBegin)[0];
            string markPartTextEnd = GetConstantWhatNot(stringMarkEnd)[0];

            if (currentUpperNumber < 0)//номера главы еще нет, а текст есть - предисловие
            {
                string partTextMark = markPartTextBegin + "Introduction" + markPartTextEnd + "-" + "Sentence" + "-";//создаем маркировку введения/предисловия - пока не будет использовано
                return partTextMark;
            }
            else
            {
                string currentPartNumberSrting = enumerateCurrentCount.ToString();
                string currentPartNumberToFind00 = AddSome00ToIntNumber(currentPartNumberSrting, totalDigitsQuantity5);
                string partTextMarks = markPartTextBegin + currentPartNumberToFind00 + markPartTextEnd + sentenceTextMarksWithOtherNumbers;//если общий метод, то тут еще прибавить сформированый хвост со старшими номерами
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "READY partTextMarks = " + partTextMarks, CurrentClassName, showMessagesLevel);
                return partTextMarks;
            }
        }        

        public string[] SelectActionByDelimitersGroupState(string textParagraph, List<List<char>> charsAllDelimiters, int sGroupCount)//выбираем, какой метод вызвать для обработки состояния
        {
            List<List<int>> allIndexResults = new List<List<int>> { new List<int>(), new List<int>(), new List<int>() };//временный массив для хранения индексов найденных в тексте разделителей
            //заполнили List индексами найденных разделителей

            //int foundMAxDelimitersGroups = ResultListFillDelimitersIndexes(textParagraph, charsAllDelimiters, allIndexResults, sGroupCount);//поиск старшей группы разделителей (кавычек) в тексте
            int foundMAxDelimitersGroups = 0;
            int startFindIndex = 0;
            int[] DelimitersQuantity = new int[sGroupCount];
            int[] foundDelimitersGroups = new int[sGroupCount];

            for (int sGroup = 0; sGroup < sGroupCount; sGroup++)
            {
                char[] charsCurrentGroupDelimiters = charsAllDelimiters[sGroup].ToArray();
                string printCharsCurrentGroupDelimiters = new string(charsCurrentGroupDelimiters);
                startFindIndex = 0;
                int indexResult = textParagraph.IndexOfAny(charsCurrentGroupDelimiters, startFindIndex);//ищем первый по порядку разделитель - для запуска while
                while (indexResult != -1)//если одни нашлись (уже не -1), собираем все остальные разделительы, пока находятся
                {
                    allIndexResults[sGroup].Add(indexResult);//сохраняем индекс в массиве в нулевой строке
                    startFindIndex = indexResult + 1;//начинаем новый поиск с места найденных кавычек (наверное, тут надо добавить +1)
                    DelimitersQuantity[sGroup]++; //считаем разделители
                    indexResult = textParagraph.IndexOfAny(charsCurrentGroupDelimiters, startFindIndex);//Значение –1, если никакой символ не найдена. Индекс от нуля с начала строки, если любой символ найден
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "sGroup = " + sGroup.ToString() + strCRLF +
                        "DelimitersQuantity = " + DelimitersQuantity[sGroup].ToString() + strCRLF +
                        "printCharsCurrentGroupDelimiters --> " + printCharsCurrentGroupDelimiters + strCRLF +
                        "NEW startFindIndex (+ indexResultQuotes) = " + startFindIndex.ToString(), CurrentClassName, showMessagesLevel);
                }
                if (DelimitersQuantity[sGroup] > 0)
                {
                    foundMAxDelimitersGroups = sGroup;//максимальный номер группы разделителей - пока что для совместимости
                    foundDelimitersGroups[sGroup] = sGroup;//а вообще массив с найденными количествами разделителей по группам - хотя и этого не надо - можно же порыться в allIndexResults - и вообще, создавать дополнительные строки только, если есть соотвествующие группы
                }
            }
                //не рассмотрен вариант нахождения и группы 2 и группы 1 - после обработки группы 2 надо вернуться опять сюда
                if (foundMAxDelimitersGroups < 0)
            {
                return null;//была пустая строка - тоже не надо возвращать - будет просто пустой allIndexResults, может, даже не созданный? кстати, тогда FindSentencesDelimitersBeetweenQuotes вообще вызывать и не надо - проходим мимо
            }            
                    int[] SentenceDelimitersIndexesArray = FindSentencesDelimitersBeetweenQuotes(textParagraph, allIndexResults, foundDelimitersGroups);//foundDelimitersGroups может быть 0, 1 или 2 - если два, то потом надо как-то рассмотреть вариант и с 1
                    string[] paragraphSentences = DivideTextToSentencesByDelimiters(textParagraph, SentenceDelimitersIndexesArray);//разделили текст на предложения согласно оставшимся разделителям
                    return paragraphSentences;            
        }

        //сделать простые примеры с точным расположением серараторов, написать все возможные ситуации обработки разделителей (FSM)
        public int[] FindSentencesDelimitersBeetweenQuotes(string textParagraph, List<List<int>> allIndexResults, int[] foundDelimitersGroups)//метод вызывется для проверки попадания точек (разделителей предложений) внутрь кавычек/скобок, тип кавычек - простые или откр-закр определяется checkedDelimitersGroup
        {
            int checkedDelimitersGroup = 0;

            int sGroupCount = foundDelimitersGroups.Length;//вместо того, что смотреть во всякие foundDelimitersGroups, надо посмотреть, сколько строк есть у allIndexResults - и сразу все понятно из первых рук
            for (int sGroup = 0; sGroup < sGroupCount; sGroup++)
            {
                if (foundDelimitersGroups[sGroup] > 0)
                {
                    checkedDelimitersGroup = foundDelimitersGroups[sGroup];
                }
            }

            int positiveCurrentIndexOfDotPosition = 0;
            int SentenceDelimitersIndexesCount = allIndexResults[0].Count();            

            int[] tempAllIndexResultsCheckedDelimitersGroup = allIndexResults[checkedDelimitersGroup].ToArray();//временный массив индесов текущей группы кавычек

            int[] temp0IndexResultsCheckedDelimitersGroup = allIndexResults[0].ToArray();//временный сохраненный (исходный) массив индесов текущей разделителей
            if (checkedDelimitersGroup == 0)
            {
                return temp0IndexResultsCheckedDelimitersGroup;
            }
            int[] tempSentenceDelimitersIndexesArray = allIndexResults[0].ToArray();//временно рабочий массив индесов разделителей предложений - для сохранения результатов - вроде бы один и тот же массив, а объединить не получается

            int currentOFAllIndexResultsCount = tempAllIndexResultsCheckedDelimitersGroup.Count();//получили общее количество разделителей указанной в checkedDelimitersGroup группы

            bool evenQuotesCount = IsCurrentGroupDelimitersCountEven(textParagraph,currentOFAllIndexResultsCount);//результат пока не используем - если кавычек нечетное количество, то при проверке сейчас остановит Assert, а потом - позовем пользователя сделать четное, то есть в любом случае, считаем, что стало четное (хотя проверить все же стоит?)
                                                                                                    
            //проверяем наличие разделителей нулевой группы (.?!;) между парами кавычек/скобок и при наличии таковых - удаляем (с осторожностью на правых краях)
            for (int currentNumberQuotesPair = 0; currentNumberQuotesPair < currentOFAllIndexResultsCount - 1; currentNumberQuotesPair = currentNumberQuotesPair + 2)//выбираем номер по порядку пары кавычек из общего количества кавычек, точнее выбираем номер открывающей кавычки и перекакиваем через одну в следюущем цикле
            {
                int startIndexQuotes = tempAllIndexResultsCheckedDelimitersGroup[currentNumberQuotesPair];//получаем индекс открывающей кавычки текущей по порядку пары
                int finishIndexQuotes = tempAllIndexResultsCheckedDelimitersGroup[currentNumberQuotesPair + 1];//получаем индекс закрывающей                
                positiveCurrentIndexOfDotPosition = 0;
                for (int forCurrentIndexOfDotPosition = 0; forCurrentIndexOfDotPosition < SentenceDelimitersIndexesCount; forCurrentIndexOfDotPosition++)//достаем в цикле все индексы разделителей предложений и проверяем их на попадание в диапазон между кавычками
                {
                    int currentRestDelimitersIndex = RemoveSentencesDelimitersBeetweenQuotes(temp0IndexResultsCheckedDelimitersGroup, forCurrentIndexOfDotPosition, startIndexQuotes, finishIndexQuotes);//временный метод для сохранения старой логики                    

                    if (currentRestDelimitersIndex > 0)
                    {
                        tempSentenceDelimitersIndexesArray[positiveCurrentIndexOfDotPosition] = currentRestDelimitersIndex;//заполнение массива - если бы знать заранее его длину, то можно было бы обойтись без временного - подумать над этим
                        positiveCurrentIndexOfDotPosition++;
                    }
                }                
            }            
            int[] SentenceDelimitersIndexesArray = tempSentenceDelimitersIndexesArray.Take(positiveCurrentIndexOfDotPosition).ToArray();//настоящий (выходной) временный массив (упакованный без нулей и прочего) - теперь известна его длина, а раньше был временно-временный - вместо отрицательных значений нули, хотя и все в конце            
            return SentenceDelimitersIndexesArray;//возвращать полезнее число оставшихся разделителей - кому нужны уже удаленные? а возвращать новый маленький массив индексов - еще полезнее, еще его надо сразу сжать - выкинуть отрицательные - уже успешно сжали
        }
        
        public int RemoveSentencesDelimitersBeetweenQuotes(int[] temp0IndexResultsCheckedDelimitersGroup, int forCurrentIndexOfDotPosition, int startIndexQuotes, int finishIndexQuotes)
        {
            int currentRestDelimitersIndex = 0;
            int maxShiftLastDotBeforeRightQuote = 3;//параметр сдвига правой точки за закрывающую кавычку, здесь взять максимальный (и получить его из констант), но потом надо рассмотреть все случаи - 1. точка перед самой кавычкой, 2. пробел между ними 3. больше знаков - например многоточие 
            int currentIndexOfDotPosition = temp0IndexResultsCheckedDelimitersGroup[forCurrentIndexOfDotPosition];
            
            //варианты для switch - расстояние от точки до кавычки - 1 символ, 2, 3, 4, 5 или больше (5 - это многоточие из точек и пробел между ним и кавычками)             

            bool action1 = currentIndexOfDotPosition > startIndexQuotes;//сначала смотрим, больше ли индекс точки первой кавычки - если нет, то сразу выходим
            if(action1)
            {
                bool action2 = currentIndexOfDotPosition < finishIndexQuotes;//потом смотрим, меньше ли индекс точки второй кавычки - если нет, то сразу выходим
                if (action2)
                {
                    int rightQuoteZone = finishIndexQuotes - maxShiftLastDotBeforeRightQuote;//вычисляем границу критической зоны правой кавычки
                    bool action3 = currentIndexOfDotPosition > rightQuoteZone;//и главный выбор - попадает ли индекс точки до зоны второй кавычки или в зону
                    if(action3)
                    {//попадает в зону, тут потом рассмотрим разные варианты расстояний до кавычки (когда пройдет старый тест)
                        temp0IndexResultsCheckedDelimitersGroup[forCurrentIndexOfDotPosition] = finishIndexQuotes;// + 1; переносим точку на место кавычки (возможно +1, но не факт)
                        currentRestDelimitersIndex = finishIndexQuotes;                        
                        return currentRestDelimitersIndex;
                    }
                    else
                    {//попадает до зоны - делаем индекс отрицательным для последующего удаления из массива
                        temp0IndexResultsCheckedDelimitersGroup[forCurrentIndexOfDotPosition] = temp0IndexResultsCheckedDelimitersGroup[forCurrentIndexOfDotPosition] * -1;
                        currentRestDelimitersIndex = -1;
                        return currentRestDelimitersIndex;
                    }
                }
            }
            currentRestDelimitersIndex = currentIndexOfDotPosition; //тут просто выходим из условий - ничего не делаем, возвращаем исходный индекс
            return currentRestDelimitersIndex;
        }

        public bool IsCurrentGroupDelimitersCountEven(string textParagraph, int currentOFAllIndexResultsCount)
        {
            bool evenQuotesCount = (currentOFAllIndexResultsCount & 1) == 0;//true, если allIndexResults2Count - четное // if(a&1==0) Console.WriteLine("Четное")
            if (!evenQuotesCount)
            {
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Current textParagraph with unpair quotes is - " + textParagraph + strCRLF +
                    "allIndexResults2Count = " + currentOFAllIndexResultsCount.ToString() + strCRLF +                        
                    "The Quotes Quantity in NOT EVEN!" + evenQuotesCount.ToString(), CurrentClassName, 3);
                //string currentParagraph = GetParagraphText(i, desiredTextLanguage); //надо бы выводить или абзац, в котором беда или хотя бы его индекс (но можно в вызывающем методе, правда, там тоже всего этого нет) - когда дадут доступ, вызовем
            }
            System.Diagnostics.Debug.Assert(evenQuotesCount, "The Quotes Quantity in NOT EVEN");//тут проверили, что кавычек-скобок четное количество, если нечетное, то потом будем звать пользователя
            return evenQuotesCount;
        }

        public string[] DivideTextToSentencesByDelimiters(string textParagraph, int[] SentenceDelimitersIndexesArray)
        {            
            int SentenceDelimitersIndexesArrayLength = SentenceDelimitersIndexesArray.Length;
            string[] paragraphSentences = new string[SentenceDelimitersIndexesArrayLength];//временный массив для хранения свежеподеленных предложений
            int textParagraphLength = textParagraph.Length;
            int textParagraphLengthFromSentences = 1;// - по дороге похоже потерялся пробел на границе предложений, но это не выход
            int startIndexSentence = 0;
            int lengthSentence = 0;
            int checkLengthOfLastSentence = 0; //сдвинем все разделители на 1 вправо, чтобы не терялись точки - но последний сдвигать нельзя, проверяем длину и отрезаем

            for (int i = 0; i < SentenceDelimitersIndexesArrayLength; i++)
            {
                lengthSentence = SentenceDelimitersIndexesArray[i] - startIndexSentence + 2;//где-то здесь теряются точки в конце предложений, попробуем поставить +1 (но нет, наверное, надо не здесь, а сдвинуть индекс разделительа на 1 вверх)
                checkLengthOfLastSentence = startIndexSentence + lengthSentence;
                bool exceptionWillCome = checkLengthOfLastSentence >= textParagraphLength;

                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "startIndexSentence = " + startIndexSentence.ToString() + strCRLF +
                    "lengthSentence = " + lengthSentence.ToString() + strCRLF +
                    "chechLengthLastSentence = " + checkLengthOfLastSentence.ToString() + strCRLF +
                    "textParagraphLength = " + textParagraphLength.ToString() + strCRLF +
                    "exceptionWillCome = " + exceptionWillCome.ToString(), CurrentClassName, showMessagesLevel);

                if (exceptionWillCome)
                {
                    lengthSentence = textParagraphLength - startIndexSentence;
                }
                paragraphSentences[i] = textParagraph.Substring(startIndexSentence, lengthSentence);//string Substring (int startIndex, int length)
                startIndexSentence = startIndexSentence + lengthSentence;
                textParagraphLengthFromSentences = textParagraphLengthFromSentences + lengthSentence;

                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "textParagraph - " + textParagraph + strCRLF +
                    "textParagraphLength = " + textParagraphLength.ToString() + strCRLF +
                    "paragraphSentences[" + i.ToString() + "] - " + paragraphSentences[i] + strCRLF +
                    "textParagraphLengthFromSentences = " + textParagraphLengthFromSentences.ToString(), CurrentClassName, showMessagesLevel);
            }
            textParagraphLengthFromSentences = textParagraphLengthFromSentences - 1; //вычитаем единицу, которую не удалось прибавить к последнему предложению
            if (textParagraphLengthFromSentences != textParagraphLength)
            {
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "textParagraphLength = " + textParagraphLength.ToString() + strCRLF +
                        "The paragraph length is NOT EQUAL to sentences length sum!" + strCRLF +
                        "textParagraphLengthFromSentences = " + textParagraphLengthFromSentences.ToString(), CurrentClassName, showMessagesLevel);//сюда поставить переменную или метод аварийного сообщения
            }
            //System.Diagnostics.Debug.Assert(textParagraphLengthFromSentences == textParagraphLength, "The paragraph length is NOT EQUAL to sentences length sum!");//можно убрать
            return paragraphSentences;//возвращаем массив разделенных предложений
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}

//public int ResultListFillDelimitersIndexes(string textParagraph, List<List<char>> charsAllDelimiters, List<List<int>> allIndexResults, int sGroupCount)
//{             
//    int startFindIndex = 0;
//    int[] DelimitersQuantity = new int[sGroupCount];
//    int delimitersGroupsState = 0;

//    for (int sGroup = 0; sGroup < sGroupCount; sGroup++)
//    {
//        char[] charsCurrentGroupDelimiters = charsAllDelimiters[sGroup].ToArray();
//        string printCharsCurrentGroupDelimiters = new string(charsCurrentGroupDelimiters);
//        startFindIndex = 0;
//        int indexResult = textParagraph.IndexOfAny(charsCurrentGroupDelimiters, startFindIndex);//ищем первый по порядку разделитель - для запуска while
//        while (indexResult != -1)//если одни нашлись (уже не -1), собираем все остальные разделительы, пока находятся
//        {
//            allIndexResults[sGroup].Add(indexResult);//сохраняем индекс в массиве в нулевой строке
//            startFindIndex = indexResult + 1;//начинаем новый поиск с места найденных кавычек (наверное, тут надо добавить +1)
//            DelimitersQuantity[sGroup]++; //считаем разделители
//            indexResult = textParagraph.IndexOfAny(charsCurrentGroupDelimiters, startFindIndex);//Значение –1, если никакой символ не найдена. Индекс от нуля с начала строки, если любой символ найден
//            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "sGroup = " + sGroup.ToString() + strCRLF +
//                "DelimitersQuantity = " + DelimitersQuantity[sGroup].ToString() + strCRLF +
//                "printCharsCurrentGroupDelimiters --> " + printCharsCurrentGroupDelimiters + strCRLF +
//                "NEW startFindIndex (+ indexResultQuotes) = " + startFindIndex.ToString(), CurrentClassName, showMessagesLevel);
//        }                
//        if (DelimitersQuantity[sGroup] > 0)
//        {
//            delimitersGroupsState = sGroup;
//        }                
//    }                
//    return delimitersGroupsState;
//}
//string stringDelimitersGroupsState = null;
//int allDelimitersQuantity = 0;//только для тестов - общее количество найденных разделителей в примерах
//allDelimitersQuantity = allDelimitersQuantity + DelimitersQuantity[sGroup];
//if (DelimitersQuantity[0] > 0)
//{
//    bool successStringToInt = Int32.TryParse(stringDelimitersGroupsState, out delimitersGroupsState);
//    if (!successStringToInt)
//    {
//        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "String with delimitersGroupsState --> " + stringDelimitersGroupsState, CurrentClassName, 3);
//    }
//    System.Diagnostics.Debug.Assert(successStringToInt, "что за фигня?, в строке не числовые символы");
//}      
//switch (delimitersGroupsState)
//{
//    case
//    0:// - найдена только нулевая группа                    
//int[] SentenceDelimitersIndexesArray = allIndexResults[0].ToArray();
//case
//10:// - найдена нулевая и первая
//    SentenceDelimitersIndexesArray = FindSentencesDelimitersBeetweenQuotes(textParagraph, allIndexResults, foundDelimitersGroups);//foundDelimitersGroups = 1
//    paragraphSentences = DivideTextToSentencesByDelimiters(textParagraph, SentenceDelimitersIndexesArray);//разделили текст на предложения согласно оставшимся разделителям
//    return paragraphSentences;
//case
//20:// - найдена нулевая и вторая - обе группы кавычек(1 и 2) обрабатываются одинаково - скорее всего, одним и тем же методом
//    SentenceDelimitersIndexesArray = FindSentencesDelimitersBeetweenQuotes(textParagraph, allIndexResults, foundDelimitersGroups);//foundDelimitersGroups = 2
//    paragraphSentences = DivideTextToSentencesByDelimiters(textParagraph, SentenceDelimitersIndexesArray);//разделили текст на предложения согласно оставшимся разделителям
//    return paragraphSentences;
//case
//21:// - найдена первая и вторая - не имеет смысла, но допустим
//    actionsResults = -12;//или еще чему-то приравнять
//    return actionsResults;
//case
//210:// - найдены все три группы (тяжелый случай, скорее всего скобки и кавычки)
//    actionsResults = CommandProcess120();
//    return actionsResults;
//}
//return null;
//рассмотреть для начала два варианта - точка находится до "зоны правой кавычки" и в этой зоне - в отдельный метод
//int removeDelimitersCount = RemoveSentencesDelimitersBeetweenQuotes(allIndexResults, checkedDelimitersGroup);//удалили все точки(?!) между кавычками-скобками
//bool currentDotPositionInQuotesZone = (currentIndexOfDotPosition > (finishIndexQuotes - maxShiftLastDotBeforeRightQuote)) && (currentIndexOfDotPosition < finishIndexQuotes);//точка попадает в зону кавычек - разделить на 2 случая? точка вплотную к кавычке и точка на символ от кавычки
//if (currentDotPositionInQuotesZone)//если точка(?!) находится близко к закрывающим кавычкам (между ними еще может быть пробел), то принимаем меры - переносим точку на место кавычки (возможно +1, но не факт)
//{                
//    temp0IndexResultsCheckedDelimitersGroup[forCurrentIndexOfDotPosition] = finishIndexQuotes;// + 1;
//    currentRestDelimitersIndex = finishIndexQuotes;
//    return currentRestDelimitersIndex;
//}

//bool currentDotPositionBetweenQuotes = (currentIndexOfDotPosition > startIndexQuotes) && (currentIndexOfDotPosition < finishIndexQuotes);//точка попадает между кавычек - до зоны правой кавычки ( - maxShiftLastDotBeforeRightQuote)
//if (currentDotPositionBetweenQuotes)//точка попадает между кавычек, удаляем ее
//{
//    temp0IndexResultsCheckedDelimitersGroup[forCurrentIndexOfDotPosition] = temp0IndexResultsCheckedDelimitersGroup[forCurrentIndexOfDotPosition] * -1;
//    currentRestDelimitersIndex = -1;
//    return currentRestDelimitersIndex;
//}
//public string[] FSM1(string textParagraph, List<List<char>> charsAllDelimiters, int sGroupCount) //запускаем машину состояний - поиск разделителей, их анализ, потом деление текста на предложения
//{
//    List<List<int>> allIndexResults = new List<List<int>> { new List<int>(), new List<int>(), new List<int>() };//временный массив для хранения индексов найденных в тексте разделителей
//    //заполнили List индексами найденных разделителей
//    int delimitersGroupsState = ResultListFillDelimitersIndexes(textParagraph, charsAllDelimiters, allIndexResults, sGroupCount);//FSM - запуск вычисления состояния по массиву разделителей

//    //string[] paragraphSentences = SelectActionByDelimitersGroupState(textParagraph, delimitersGroupsState, charsAllDelimiters, allIndexResults, sGroupCount);//вызываем селектор методов обработки состояний - согласно найденному состоянию

//    return paragraphSentences;
//}        
//тут-то и вызываем метод дележки - передавая ему текст и собранные номера главы и абзаца
//allIndexResults0Count = allIndexResults[0].Count();//заново посчитали длину массива индексов точек(?!) после децимации
//countSentencesNumber = allIndexResults0Count;//фактически количество предложений должно быть равно окончательному количеству разделителей
//allIndexResults[0][forCurrentIndexOfDotPosition] = allIndexResults[0][forCurrentIndexOfDotPosition] * -1;//чтобы не менять количество разделителей на ходу, делаем его отрицательным (или надо идти с конца массива, если Remove ненужные)
//theRestDelimitersCount++;//изменить способ подсчета (или добавить?)
//тут избавиться от списка allIndexResults - его время вышло и перейти к простому временному массиву
//int allIndexResults0Count = allIndexResults[0].Count();//потом еще надо будет проверить следующий за разделителем символ - там может быть кавычка (и вообще, проверить есть ли там пробел и заглавная буква, если последний - это не проверять)            
//for (int i = allIndexResults0Count - 1; i >= 0; i--)
//{
//    if (allIndexResults[0][i] < 0)
//    {
//        allIndexResults[0].RemoveAt(i);//удаляем ставшие отрицательные индексы - для удобства
//    }
//}
//allIndexResults0Count = allIndexResults[0].Count();//заново посчитали длину массива индексов точек(?!) после децимации
//int countSentencesNumber = allIndexResults0Count;//фактически количество предложений должно быть равно окончательному количеству разделителей
//string[] paragraphSentences = DivideTextToSentencesByDelimiters(textParagraph, allIndexResults, allIndexResults0Count);//разделили текст на предложения согласно оставшимся разделителям
//проверяем наличие разделителей каждой группы, начиная с последней (кавычки окр-закр) - по сути, проверяем сначала 2-ю группу, потом 1-ю
//for (int checkedDelimitersGroup = sGroupCount - 1; checkedDelimitersGroup > 0; checkedDelimitersGroup--)
//{
//    if (allIndexResults[checkedDelimitersGroup] != null)
//    {
//        int removeDelimitersCount = RemoveSentencesDelimitersBeetweenQuotes(allIndexResults, checkedDelimitersGroup);//удалили все точки(?!) между кавычками-скобками
//    }
//}
//краевые эффекты - не находит разделители в конце абзаца, кроме этого, возможно, надо нормировать текст - убрать пробелы между ?! " . - и другие варианты
//теряем точку в конце обычного предложения без кавычек
//тут надо рассмотреть случаи p.m. и аналогичную фигню с левыми точками - проверять пробелы после точки и смотреть на заглавную букву (или EOL)
//if (finishIndexQuotes > startIndexQuotes)//проверили, что закрывающая правее, чем открывающая - надо?
//int newCurrentIndexOfDotPosition = Method4(result, finishIndexQuotes, currentIndexOfDotPosition);//метод-переключатель состояния FSM, только вот выдача нуля явно лишняя - наверное, надо присваивать allIndexResults[0][forCurrentIndexOfDotPosition] внутри него
//if (newCurrentIndexOfDotPosition != 0) allIndexResults[0][forCurrentIndexOfDotPosition] = newCurrentIndexOfDotPosition;
//переносим точку разделения предложений с разделителя предложений на (за!) кавычки, чтобы ничего не терялось
//-1 - прошли возможный пробел, -2 - попали на кавычки, -3 - вышли за кавычки (но надо проверять пробелы и другие знаки между точками и кавычками - если многоточие например)
//
//
//результат не используем - возвращать что-то другое? например, последняя группа найденных разделителей
//что вернуть-то? количество предложений = метод дележки(текст, номер главы, номер абзаца)
//проверяем, что текущий абзац - это номер абзаца, можно его достать еще раз через currentParagraphIndex и сравнить с полученным - что находимся в нужном месте
//string controlParagraph = GetParagraphText(currentParagraphIndex, desiredTextLanguage);
//bool theFollowingNeedToDivide = String.Equals(currentParagraph, controlParagraph);//тут правильно было бы добавить CultureInfo.CreateSpecificCulture(cultureName), но наверное не надо, строки должны совпадать в точности
//if (theFollowingNeedToDivide)
//тут надо решить, что дальше делать с полученными предложениями - вообще, надо бы их вернуть в DividePagagraphToSentences и пусть он там с ними разбирается, но с временным массивом сложности - его размерность становится известна только тут
//public int SelectAllQuotesOrDelimitersInText(string textParagraph, Dictionary<int, string> searchResultQuotes, string[] charsQuotesOrDelimiters)
//{
//    int textParagraphLength = textParagraph.Length;
//    int startFindIndex = 0;
//    int finishFindIndex = textParagraphLength;
//    int indexResultQuotes = 0;            
//    int quotesTypesQuantity = 0;//общее количество найденных типов
//    int quotesQuantity = 0;//общее количество найденных чего искали всех типов
//    int charsQuotesOrSeparatorLength = charsQuotesOrDelimiters.Length;
//    for (int i = 0; i < charsQuotesOrSeparatorLength; i++)
//    {
//        startFindIndex = 0;
//        finishFindIndex = textParagraphLength;//сброс точки старта и длины текста перед поиском нового варианта кавычек
//        indexResultQuotes = textParagraph.IndexOf(charsQuotesOrDelimiters[i], startFindIndex, finishFindIndex);//ищем, есть ли кавычки (первые по по порядку) - для запуска while

//        if (indexResultQuotes > -1)
//        {
//            quotesTypesQuantity++;
//        }
//        while (indexResultQuotes != -1)//если одни нашлись (уже не -1), собираем все остальные кавычки, пока находятся
//        {
//            searchResultQuotes.Add(indexResultQuotes, charsQuotesOrDelimiters[i]);//сохраняем индекс в тексте и тип найденных кавычек

//            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "textParagraph [" + i.ToString() + "] -- > " + textParagraph + strCRLF + strCRLF +
//                "finishFindIndex (textLengh) = " + finishFindIndex.ToString() + strCRLF +
//                "startFindIndex (start of the search) = " + startFindIndex.ToString() + strCRLF +
//                "indexResultQuotes (quotes found on position No.) = " + indexResultQuotes.ToString() + strCRLF +
//                "charsQuotesSeparator[i] --> " + charsQuotesOrDelimiters[i] + strCRLF +
//                "searchResultQuotes value on [" + indexResultQuotes.ToString() + "] = " + searchResultQuotes[indexResultQuotes], CurrentClassName, showMessagesLevel);

//            startFindIndex = indexResultQuotes + 1;//начинаем новый поиск с места найденных кавычек (наверное, тут надо добавить +1)
//            finishFindIndex = textParagraphLength - startFindIndex;//надо каждый раз вычитать последнюю найденную позицию из полной длины текста, а не остатка
//            quotesQuantity++; //считаем кавычки

//            indexResultQuotes = textParagraph.IndexOf(charsQuotesOrDelimiters[i], startFindIndex, finishFindIndex);//Значение –1, если строка не найдена. Индекс от нуля с начала строки, если строка найдена
//            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "NEW finishFindIndex (- indexResultQuotes) = " + finishFindIndex.ToString() + strCRLF +
//                "NEW startFindIndex (+ indexResultQuotes) = " + startFindIndex.ToString(), CurrentClassName, showMessagesLevel);
//        }
//    }
//    return quotesTypesQuantity;
//}

//public int CollectTextInQuotes(string textParagraph, Dictionary<int, string> searchResultQuotes, int quotesTypesQuantity, string[] charsSentencesDelimiters, int charsSentenceSeparatorLength)
//{
//    //проверить символы-разделительы до и после закрывающих кавычек, а также между закрывающей и следующей открывающей, все как-то занести в общий массив
//    //или собрать все символы-разделительы в отдельный массив, а потом уже анализировать все вместе

//    //добавить сюда константы формирования номера предложения

//    int quotesQuantity = searchResultQuotes.Count;
//    int[,] allPairsQuotes = new int[2, quotesQuantity/2];//в нулевой строке массива индекс (по тексту абзаца) открывающей кавычки, в первой - закрывающей

//    string[] allSentencesInParagraph = new string[quotesQuantity];//спорная размерность массива, скорее, надо динамический
//    int sentenceNumber = 0;//индекс массива allSentencesInParagraph

//    int endIndexQuotes = (int)MethodFindResult.NothingFound; ;//если -1 - текст в кавычках не найден, придумать более подходящее название переменной

//    switch (quotesTypesQuantity)
//    {
//        case
//        0:
//            return (int)MethodFindResult.NothingFound;//не нашли никаких кавычек - чего тогда звали? не может быть такого варианта - можно убрать
//        case
//        1://нашли один тип кавычек
//            int forwardOrBacksparkQuote = 0;
//            int i = 0;
//            int j = 0;
//            foreach (int indexQuotes in searchResultQuotes.Keys)
//            {
//                forwardOrBacksparkQuote = i & 1;
//                allPairsQuotes[forwardOrBacksparkQuote, j] = indexQuotes;//записали открывающие и закрывающие кавычки в разные линейки массива
//                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "indexQuotes = " + indexQuotes.ToString() + strCRLF +
//                    "i = " + i.ToString() + strCRLF +
//                    "j = " + j.ToString() + strCRLF +
//                    "forwardOrBacksparkQuote = " + forwardOrBacksparkQuote.ToString(), CurrentClassName, 3);
//                if ((i & 1) == 1) j++;
//                i++;
//            }
//            int forwardQuoteIndex = 0;//индекс открывающей кавычки
//            int backsparkQuoteIndex = 0;//индекс закрывающей кавычки
//            int indexResultDelimitersBeforeQuoute = -1;//инициализация отрицанием
//            int indexResultDelimitersAfterQuoute = -1;
//            //int indexResultDelimitersNearQuoute = -1;
//            for (int n = 0; n < (quotesQuantity / 2); n++)
//            {                        
//                forwardQuoteIndex = allPairsQuotes[0, n];//достаем пару кавычек из массива - открывающую и закрывающую
//                backsparkQuoteIndex = allPairsQuotes[1, n];
//                //int startFindIndexNearQuoute = backsparkQuoteIndex - 1;//старт поиска за один символ до закрывающей кавычки
//                //int charCountForCheking = 3; //длина поиска - символ до кавычки, кавычка и символ после кавычки
//                for (int t = 0; t < charsSentenceSeparatorLength; t++)
//                {
//                    indexResultDelimitersBeforeQuoute = textParagraph.IndexOf(charsSentencesDelimiters[t], forwardQuoteIndex, 1);//ищем разделительы перед кавычкой
//                    indexResultDelimitersAfterQuoute = textParagraph.IndexOf(charsSentencesDelimiters[t], forwardQuoteIndex+1, 1);//ищем разделительы после кавычки
//                }
//                //проверяем сеператор сразу после кавычки - если есть, то переписываем предложение в окончательный массив по кавычке + символ-разделитель после нее
//                if (indexResultDelimitersAfterQuoute > 0)//нуль быть не может, потому что закрывающая кавычка и была найдена пара
//                {                            
//                    allSentencesInParagraph[sentenceNumber] = textParagraph.Substring(forwardQuoteIndex, backsparkQuoteIndex + 1); //string substring = value.Substring(startIndex, length);
//                }
//                else//сеператор сразу после кавычки не найден - тогда проверяем, был ли разделитель перед кавычкой
//                {
//                    if (indexResultDelimitersBeforeQuoute > 0)//если был разделитель перед кавычкой, то переписываем найденное предложение, включая кавычку
//                    {
//                        allSentencesInParagraph[sentenceNumber] = textParagraph.Substring(forwardQuoteIndex, backsparkQuoteIndex); //string substring = value.Substring(startIndex, length);
//                    }
//                }


//            }
//            //pair.Key, pair.Value
//            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), 
//                "allPairsQuotes[0,0] = " + allPairsQuotes[0, 0].ToString() + strCRLF +
//                "allPairsQuotes[1,0] = " + allPairsQuotes[1, 0].ToString() + strCRLF + 
//                "allPairsQuotes[0,1] = " + allPairsQuotes[0, 1].ToString() + strCRLF +
//                "allPairsQuotes[1,1] = " + allPairsQuotes[1, 1].ToString() + strCRLF +
//                "allPairsQuotes[0,2] = " + allPairsQuotes[0, 2].ToString() + strCRLF +
//                "allPairsQuotes[1,2] = " + allPairsQuotes[1, 2].ToString() + strCRLF +                        
//                "quotesQuantity = " + quotesQuantity.ToString(), CurrentClassName, 3);
//            i++;
//            return quotesTypesQuantity; //endIndexQuotes;
//        case
//        2://нашли (наверное) сдвоенный тип кавычек - проверяем, так ли это (или раньше надо было проверить?)

//            return endIndexQuotes;
//    }


//    //кусок абзаца от кавычки до кавычки проверить на окончания предложения, кроме последнего символа перед кавычкой
//    //если нет окончаний, проверить последний символ перед кавычкой и первый после кавычки
//    //если там есть, то закончить предложение, если нет - искать разделительы до первого найденного, там закончить предложение - каши не дошли до следующей кавычки
//    //если началась следующая - опять этот же метод - рекурсия, что ле?
//    //если в кавычках несколько разделителей, дня начала проверить, что на второй кавычке есть разделитель, сразу до или после
//    //если есть, то проверить, что во всех предложениях внутри кавычек меньше 5-ти слов
//    //если меньше, собрать предложение и перейти к следующему
//    //если больше - думать (может, не пяти слов, а 10-ти?)

//    return quotesTypesQuantity;// endIndexQuotes; //-1 - текст в кавычках не найден - или вторые кавычки не найдены? в общем, засада и что-то надо делать
//}
//три динамических массива - по группам разделителей и три раза пройти IndexOfAny - и все это в отдельный метод
//проверять, разделительы из одного символа или из двух, если из двух - то разделять на отдельные строки List - например - все еще проще, слили и разделили в массив char
//тогда все доставания констант и передачу их в массив char можно сделать в цикле - определения для метода GetConstantWhatNot тоже в массив и через него же доставать - уст.
//разделить разделительы на 3 группы по номеру и сделать временный массив на 3 значения, где хранить количество найденных разделителей
//достать и сформировать временный массив разделителей (разделителей предложений) - нулевой
//int charsSentenceSeparatorLength = GetConstantWhatNotLength("Sentence");
//string[] stringArraySentencesDelimiters = new string[charsSentenceSeparatorLength];
//stringArraySentencesDelimiters = GetConstantWhatNot("Sentence");//вариант многоточия из обычных точек надо обрабатывать отдельно - просто проверить, нет ли трех точек подряд
//string stringSentencesDelimiters = String.Join("", stringArraySentencesDelimiters);
//charsAllDelimiters[0].AddRange(stringSentencesDelimiters.ToCharArray());
//достать и сформировать временный массив кавычек-скобок - первый
//int charsQuotesTypesLength = GetConstantWhatNotLength("Quotes");
//string[] stringArrayQuotesTypes = new string[charsQuotesTypesLength];
//stringArrayQuotesTypes = GetConstantWhatNot("Quotes");
//string stringQuotesTypes = String.Join("", stringArrayQuotesTypes);
//charsAllDelimiters[1].AddRange(stringQuotesTypes.ToCharArray());
//достать и сформировать временный массив откр-закр кавычек-скобок - второй и третий
//int charsBraketsTypesLength = GetConstantWhatNotLength("Brackets");
//string[] stringArrayBraketsTypes = new string[charsBraketsTypesLength];
//string[] stringArrayBraketsClosingTypes = new string[charsBraketsTypesLength];
//stringArrayBraketsTypes = GetConstantWhatNot("Brackets");
//for (int t = 0; t < charsBraketsTypesLength; t++)//разделяем строковые константы с двумя символами на открывающий и закрывающий 
//{
//    string currentChar = stringArrayBraketsOpeningTypes[t];
//    stringArrayBraketsOpeningTypes[t] = currentChar[0].ToString();
//    stringArrayBraketsClosingTypes[t] = currentChar[1].ToString();
//}
//string stringBraketsTypes = String.Join("", stringArrayBraketsTypes);
//string stringBraketsClosingTypes = String.Join("", stringArrayBraketsClosingTypes);
//charsAllDelimiters[2].AddRange(stringBraketsTypes.ToCharArray());            
//charsAllDelimiters[3].AddRange(stringBraketsClosingTypes.ToCharArray());
//int charCount = 0;
//foreach(char ch in textParagraph)
//{
//    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "ch[" + charCount.ToString() + "] -->" + ch.ToString(), CurrentClassName, 3);
//    charCount++;
//}
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




