using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using TextSplitLibrary;

namespace TextSplit
{
    public interface IAnalysisLogicChapter
    {
        string ChapterNameAnalysis(int desiredTextLanguage);
        int UserSelectedChapterNameAnalysis(int desiredTextLanguage);       
        string FindDigitsInChapterName(string paragraphWihtChapterName);
    }

    public class AnalysisLogicChapter : IAnalysisLogicChapter
    {
        private readonly IAllBookData _bookData;
        private readonly IMessageService _msgService;
        private readonly IAnalysisLogicCultivation _analysisLogic;
        private readonly IAnalysisLogicDataArrays _arrayAnalysis;

        delegate bool IsOrNotEqual(char x);
        delegate bool DoElseConditions(string x, int i, int j);
        delegate string TransduceWord(string baseKeyWord);

        int GetParagraphTextLength(int desiredTextLanguage) => _bookData.GetParagraphTextLength(desiredTextLanguage);
        string GetParagraphText(int paragraphCount, int desiredTextLanguage) => _bookData.GetParagraphText(paragraphCount, desiredTextLanguage);
        int SetParagraphText(string paragraphText, int paragraphCount, int desiredTextLanguage) => _bookData.SetParagraphText(paragraphText, paragraphCount, desiredTextLanguage);

        string GetChapterName(int chapterCount, int desiredTextLanguage) => _bookData.GetChapterName(chapterCount, desiredTextLanguage);
        int GetChapterNameLength(int desiredTextLanguage) => _bookData.GetChapterNameLength(desiredTextLanguage);
        int AddChapterName(string chapterNameWithNumber, int desiredTextLanguage) => _bookData.AddChapterName(chapterNameWithNumber, desiredTextLanguage);

        int GetChapterNumber(int chapterCount, int desiredTextLanguage) => _bookData.GetChapterNumber(chapterCount, desiredTextLanguage);
        int GetChapterNumberLength(int desiredTextLanguage) => _bookData.GetChapterNumberLength(desiredTextLanguage);
        int AddChapterNumber(int chapterNumberOnly, int desiredTextLanguage) => _bookData.AddChapterNumber(chapterNumberOnly, desiredTextLanguage);

        int GetConstantWhatNotLength(string WhatNot) => _arrayAnalysis.GetConstantWhatNotLength(WhatNot);
        string[] GetConstantWhatNot(string WhatNot) => _arrayAnalysis.GetConstantWhatNot(WhatNot);
        int GetChapterNamesSamplesLength(int desiredTextLanguage) => _arrayAnalysis.GetChapterNamesSamplesLength(desiredTextLanguage);
        string GetChapterNamesSamples(int desiredTextLanguage, int i) => _arrayAnalysis.GetChapterNamesSamples(desiredTextLanguage, i);

        int GetFoundWordsOfParagraphLength() => GetFoundWordsOfParagraphLength();
        string GetFoundWordsOfParagraph(int i) => _arrayAnalysis.GetFoundWordsOfParagraph(i);
        int SetFoundWordsOfParagraph(string wordOfParagraph, int i) => _arrayAnalysis.SetFoundWordsOfParagraph(wordOfParagraph, i);
        //int ClearFoundWordsOfParagraph();

        int GetFoundSymbolsOfParagraphLength() => _arrayAnalysis.GetFoundSymbolsOfParagraphLength();
        int SetFoundSymbolsOfParagraph(string symbolsOfParagraph, int i) => _arrayAnalysis.SetFoundSymbolsOfParagraph(symbolsOfParagraph, i);
        string GetFoundSymbolsOfParagraph(int i) => _arrayAnalysis.GetFoundSymbolsOfParagraph(i);
        //int ClearFoundSymbolsOfParagraph();
               
        int GetBaseKeyWordFormsQuantity() => _arrayAnalysis.GetBaseKeyWordFormsQuantity();        
        int GetChapterNamesVersionsCount(int m, int i) => _arrayAnalysis.GetChapterNamesVersionsCount(m, i);
        int SetChapterNamesVersionsCount(int m, int i, int countValue) => _arrayAnalysis.SetChapterNamesVersionsCount(m, i, countValue);
        int GetChapterSymbolsVersionsCount(int i) => _arrayAnalysis.GetChapterSymbolsVersionsCount(i);
        int SetChapterSymbolsVersionsCount(int i, int countValue) => _arrayAnalysis.SetChapterSymbolsVersionsCount(i, countValue);

        string AddSome00ToIntNumber(string currentChapterNumberToFind, int totalDigitsQuantity) =>_analysisLogic.AddSome00ToIntNumber(currentChapterNumberToFind, totalDigitsQuantity);

        private readonly int showMessagesLevel;
        private readonly string strCRLF;

        public AnalysisLogicChapter(IAllBookData bookData, IMessageService msgService, IAnalysisLogicCultivation analysisLogic, IAnalysisLogicDataArrays arrayAnalysis)
        {
            _bookData = bookData;
            _msgService = msgService;
            _analysisLogic = analysisLogic;//общая логика
            _arrayAnalysis = arrayAnalysis;

            showMessagesLevel = DeclarationConstants.ShowMessagesLevel;
            strCRLF = DeclarationConstants.StrCRLF;            
        }

        public string ChapterNameAnalysis(int desiredTextLanguage)//Main здесь - ищем все названия и номера глав, создаем метку в фиксированном формате и записываем ее обратно в массив перед оригинальным называнием главы
        {
            int paragraphTextLength = GetParagraphTextLength(desiredTextLanguage); // это вместо _bookData.GetParagraphTextLength(desiredTextLanguage);
            int baseKeyWordFormsQuantity = GetBaseKeyWordFormsQuantity();
            int[] chapterNameIsDigitsOnly = new int[paragraphTextLength];
            string[] allTextWithChapterNames = new string[paragraphTextLength];

            int maxKeyNameLength = ChapterKeyNamesAnalysis(desiredTextLanguage);//ищем слово с макс.символов из ключевых (непонятно, зачем это нужно, но пока пусть будет)

            for (int i = 0; i < paragraphTextLength; i++)//перебираем все абзацы текста
            {
                string currentParagraph = GetParagraphText(i, desiredTextLanguage);//_bookData.GetParagraphText(i, desiredTextLanguage);
                FirstTenGroupsChecked(currentParagraph, chapterNameIsDigitsOnly, i, desiredTextLanguage);
            }           

            //for (int m = 0; m < baseKeyWordFormsQuantity; m++)
            //{
            //    //в этом массиве значения показывают, сколько раз какое ключевое слово встретилось в тексте
            //    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Must be found 52 the same chapters name" + strCRLF +            
            //        "ChapterNamesVersionsCount[0] --> " + GetChapterNamesVersionsCount(m, 0) + strCRLF +            
            //        "ChapterNamesVersionsCount[1] --> " + GetChapterNamesVersionsCount(m, 1) + strCRLF +            
            //        "ChapterNamesVersionsCount[2] --> " + GetChapterNamesVersionsCount(m, 2) + strCRLF +            
            //        "ChapterNamesVersionsCount[3] --> " + GetChapterNamesVersionsCount(m, 3) + strCRLF +            
            //        "ChapterNamesVersionsCount[4] --> " + GetChapterNamesVersionsCount(m, 4), CurrentClassName, showMessagesLevel);
            //}            

            int increasedChapterNumbers = IsChapterNumbersIncreased(chapterNameIsDigitsOnly, desiredTextLanguage);//выделяем монотонно возрастающие номера глав и считаем количество
            
            string keyWordFounfForm = KeyWordFormFound(desiredTextLanguage);//возвращаем готовую форму найденного ключевого слова из массива образцов GetChapterNamesSamples и сохраненной статистики поиска

            string lastFoundChapterNumberInMarkFormat = TextBookDivideOnChapter(chapterNameIsDigitsOnly, increasedChapterNumbers, keyWordFounfForm, desiredTextLanguage);//делим текст на главы, ставим метки с собственными номерами глав (совпадающими по значению с исходными)

            return lastFoundChapterNumberInMarkFormat;
        }

        public string TextBookDivideOnChapter(int[] chapterNameIsDigitsOnly, int increasedChapterNumbers, string keyWordFounfForm, int desiredTextLanguage)
        {//разделили текст на главы - перед каждой главой поставили специальную метку вида ¤¤¤¤¤KeyWord-NNN-¤¤¤, где KeyWord - в той же форме, в какой в тексте, а NNN - три цифры номера главы с лидирующими нулями
            string lastFoundChapterNumberInMarkFormat = ""; //вернем маркировку последней главы
            int totalDigitsQuantity = 3; //для номера главы используем 3 цифры (до 99, должно хватить) - перенести в AnalysisLogicDataArrays
            int paragraphTextLength = GetParagraphTextLength(desiredTextLanguage);            
            int previousChapterNameIsDigitsOnly = chapterNameIsDigitsOnly[0] - 1;//получить первый (предположительно? - проверить на разных текстах) первый номер главы и отнять 1 для запуска цикла            
            //List<string> workParagraphText = new List<string>();
            for (int i = 1; i < paragraphTextLength; i++)
            {//из массива всего текста доставать абзац, проверять на название главы, если названия нет, присвоить метку и номер абзаца - но лучше в отдельный метод

                string currentParagraph = GetParagraphText(i, desiredTextLanguage);
                int chapterNumberLength = GetChapterNumberLength(desiredTextLanguage);

                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "CurrentParagraph with i = " + i.ToString() + "  -- > " + currentParagraph + strCRLF +
                    "paragraphTextLength = " + paragraphTextLength.ToString() + strCRLF +
                    "chapterNumberLength = " + chapterNumberLength.ToString(), CurrentClassName, showMessagesLevel);
                int foundNumberInChapter = GetChapterNumber(i, desiredTextLanguage);

                if (foundNumberInChapter > 0)
                {
                    if (currentParagraph.Contains(keyWordFounfForm))
                    {
                        int correctedChapterNameIsDigitsOnly = chapterNameIsDigitsOnly[i] - 1;
                        string currentChapterNumberToFind = correctedChapterNameIsDigitsOnly.ToString();
                        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "CurrentParagraph with i = " + i.ToString() + "  -- > " + currentParagraph + strCRLF +
                                "correctedChapterNameIsDigitsOnly = " + correctedChapterNameIsDigitsOnly.ToString() + strCRLF +
                                " == previousChapterNameIsDigitsOnly = " + previousChapterNameIsDigitsOnly.ToString() + strCRLF +
                                " (" + currentChapterNumberToFind + ")" + strCRLF +
                                "foundNumberInParagraph [" + i.ToString() + "] --> " + foundNumberInChapter.ToString(), CurrentClassName, showMessagesLevel);
                        if (currentParagraph.Contains(currentChapterNumberToFind))
                        {
                            bool isChapterNumbersIncreased = (previousChapterNameIsDigitsOnly + 1) == correctedChapterNameIsDigitsOnly;
                            if (isChapterNumbersIncreased)
                            {//нашли строку с названием и номером главы
                                string currentChapterNumberToFind000 = AddSome00ToIntNumber(currentChapterNumberToFind, totalDigitsQuantity);
                                if(currentChapterNumberToFind000 == null)
                                {
                                    return null;             
                                }
                                string markChapterBegin = GetConstantWhatNot("ChapterBegin")[0];
                                string markChapterEnd = GetConstantWhatNot("ChapterEnd")[0];
                                string chapterTextMarks = markChapterBegin + currentChapterNumberToFind000 + markChapterEnd + "-" + keyWordFounfForm + "-";
                                SetParagraphText(chapterTextMarks, i - 1, desiredTextLanguage);//проверить, что i больше 0, иначе некуда заносить - ЗДЕСЬ запись SetParagraphText! - записываем номер главы в строку перед именем главы! проверить, что будет, если добавить перевод строки? ничего хорошего - потом не найти маркировку, так как строка теперь начинается не маркой
                                string currentMarkChapter = GetParagraphText(i - 1, desiredTextLanguage);//после тестов убрать вместе с печатью
                                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "CurrentParagraph with i = " + i.ToString() + "  -- > " + currentParagraph + strCRLF +
                                "chapterTextMarks " + chapterTextMarks + strCRLF +
                                "currentMarkChapter" + currentMarkChapter + strCRLF +
                                "foundNumberInChapter [" + i.ToString() + "] --> " + foundNumberInChapter.ToString(), CurrentClassName, showMessagesLevel);
                                lastFoundChapterNumberInMarkFormat = chapterTextMarks;
                                previousChapterNameIsDigitsOnly = correctedChapterNameIsDigitsOnly;
                            }
                        }
                    }
                }
            }
            return lastFoundChapterNumberInMarkFormat;//вернуть уточненное количество глав
        }

        public string KeyWordFormFound(int desiredTextLanguage)//возвращаем готовую форму найденного ключевого слова из массива образцов GetChapterNamesSamples и сохраненной статистики поиска
        {
            int baseKeyWordFormsQuantity = GetBaseKeyWordFormsQuantity();
            TransduceWord[] TransduceKeyWord = new TransduceWord[baseKeyWordFormsQuantity];
            TransduceKeyWord[0] = TransduceKeyWordToTitleCase;
            TransduceKeyWord[1] = TransduceKeyWordToUpper;
            TransduceKeyWord[2] = TransduceKeyWordToLower;
            
            int keyWordIndex = 0;//i
            int keyWordForm = 0;//m
            int keyWordCount = GetChapterNamesVersionsCount(keyWordForm, keyWordIndex);
            int getChapterNamesSamplesLength = GetChapterNamesSamplesLength(desiredTextLanguage);
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "keyWordCount = " + keyWordCount.ToString(), CurrentClassName, showMessagesLevel);
            for (int i = 1; i < getChapterNamesSamplesLength; i++)
            {
                for (int m = 0; m < baseKeyWordFormsQuantity; m++)
                {
                    int currentChapterNamesVersionsCount = GetChapterNamesVersionsCount(m, i);
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "keyWordCount = " + keyWordCount.ToString() + strCRLF +
                            "currentChapterNamesVersionsCount = " + currentChapterNamesVersionsCount.ToString() + strCRLF +
                            "i = " + i.ToString() + strCRLF +
                            "m = " + m.ToString(), CurrentClassName, showMessagesLevel);
                    if (currentChapterNamesVersionsCount > keyWordCount)//keyWordCount - уже найденное самое большое, currentChapterNamesVersionsCount - текущий претендент на самое большое
                    {
                        keyWordCount = currentChapterNamesVersionsCount;
                        keyWordIndex = i;
                        keyWordForm = m;
                        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "keyWordCount = " + keyWordCount.ToString() + strCRLF +                        
                            "currentChapterNamesVersionsCount = " + currentChapterNamesVersionsCount.ToString() + strCRLF +
                            "keyWordIndex = " + keyWordIndex.ToString() + strCRLF +                        
                            "keyWordForm = " + keyWordForm.ToString(), CurrentClassName, showMessagesLevel);
                    }
                }
            }            
            string keyWordBaseForm = GetChapterNamesSamples(desiredTextLanguage, keyWordIndex);//получаем базовое ключевое слово - строчное
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "keyWordIndex = " + keyWordIndex.ToString() + strCRLF +
                "keyWordForm = " + keyWordForm.ToString() + strCRLF +
                "keyWordBaseForm = " + keyWordBaseForm, CurrentClassName, showMessagesLevel);
            string keyWordFoundForm = TransduceKeyWord[keyWordForm](keyWordBaseForm);
            return keyWordFoundForm;
        }

        public int IsChapterNumbersIncreased(int[] chapterNameIsDigitsOnly, int desiredTextLanguage)
        {//выкинуть нули и лишние цифры из номеров глав, проверить, что остальные цифры идут по порядку, посчитать количество номеров глав
            int paragraphTextLength = _bookData.GetParagraphTextLength(desiredTextLanguage);
            int[] workChapterNameIsDigitsOnly = new int[paragraphTextLength];
            int increasedChapterNumbers = 0;
            int working = -1;
            //_msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "paragraphTextLength = " + paragraphTextLength.ToString(), CurrentClassName, 3);
            for (int i = 0; i < paragraphTextLength; i++)
            {
                if (chapterNameIsDigitsOnly[i] != 0)
                {
                    //_msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "chapterNameIsDigitsOnly = " + chapterNameIsDigitsOnly[i].ToString(), CurrentClassName, 3);
                    workChapterNameIsDigitsOnly[increasedChapterNumbers] = chapterNameIsDigitsOnly[i] - 1;

                    if (working == (workChapterNameIsDigitsOnly[increasedChapterNumbers] - 1))
                    {
                        working = workChapterNameIsDigitsOnly[increasedChapterNumbers];
                        increasedChapterNumbers++;
                    }
                }
            }            
            return increasedChapterNumbers;
        }

        public void FirstTenGroupsChecked(string currentParagraph, int[] chapterNameIsDigitsOnly, int iParagraphNumber, int desiredTextLanguage)//придумать внятное название
        {//начало анализа строки (абзаца)            
            int firstNumberOfParagraph = 0;
            int keyWordChapterName = 0;
            int foundNumberInParagraph = 0;
            
            int foundWordsOfParagraphCount = WordsOfParagraphSearch(currentParagraph);//выделяем в текущем параграфе первые 10 групп (сохраняются в массиве-методе), получаем количество слов-чисел + лидирующая группа символов

            for (int j = 0; j < foundWordsOfParagraphCount; j++)//перебираем все полученные слова - ищем сначала числа и затем ключевые слова (на самом деле одновременно)
            {//если есть число заносим в его массив int номеров глав, потом разберемся, что там, а всю строку - в массив названий глав, если нашлось ключевое слово - прибавляем счетчик ключевых слов

                string currentWordOfParagraph = GetFoundWordsOfParagraph(j); //_arrayChapter.GetFoundWordsOfParagraph(j);
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "currentWordOfParagraph = " + currentWordOfParagraph, CurrentClassName, showMessagesLevel);
                if (currentWordOfParagraph != null)
                {
                    bool successSearchNumber = Int32.TryParse(currentWordOfParagraph, out firstNumberOfParagraph);//проверяем найденное слово, что оно число - ищем номера глав
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "currentWordOfParagraph = " + currentWordOfParagraph + strCRLF + "is digit = " + successSearchNumber.ToString(), CurrentClassName, showMessagesLevel);
                    if (successSearchNumber)//складываем найденные номера глав, потом проверим на монотонное возрастание и выкинем лишние
                    {
                        chapterNameIsDigitsOnly[iParagraphNumber] = firstNumberOfParagraph + 1; //(+1 - чтобы избавиться от нулевой главы, бывают и такие, потом не забыть отнять)
                        foundNumberInParagraph = iParagraphNumber;//найден номер в абзаце, надо записать индекс абзаца в массив 
                        int chaptersNamesWithNumbersLength = AddChapterName(currentParagraph, desiredTextLanguage);//заносим всю строку в массив названий глав

                        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                        "currentParagraph = " + currentParagraph + strCRLF +
                        "Names Length = " + chaptersNamesWithNumbersLength.ToString() + strCRLF +
                        "iParagraphNumber = " + iParagraphNumber.ToString() + "    and j = " + j.ToString() + strCRLF +
                        "firstNumberOfParagraph = " + firstNumberOfParagraph.ToString(), CurrentClassName, showMessagesLevel);
                       
                    }
                    else
                    {//количество найденных ключевых слов сохраняем в массиве _arrayChapter.IncrementOfChapterNamesVersionsCount(n)
                        keyWordChapterName = CheckWordOfParagraphCompare(currentWordOfParagraph, j, desiredTextLanguage);//проверяем текущее слово на совпадение с ключевыми                        
                    }//индекс найденного слова совпадает с индексом наибольшего значения в массиве и должен совпадать по значению с количеством найденных глав - проверка
                }
            }            
            int chaptersNamesNumbersOnlysLength = AddChapterNumber(foundNumberInParagraph, desiredTextLanguage);//сохраняем индексы массива абзацев c цифрами, остальные значения - нули            
        }

        public int CheckWordOfParagraphCompare(string currentWordOfParagraph, int jWordNumber, int desiredTextLanguage)//проверяем полученное слово на совпадение с ключевыми из массива
        {
            int baseKeyWordFormsQuantity = GetBaseKeyWordFormsQuantity();
            TransduceWord[] TransduceKeyWord = new TransduceWord[baseKeyWordFormsQuantity];
            TransduceKeyWord[0] = TransduceKeyWordToTitleCase;
            TransduceKeyWord[1] = TransduceKeyWordToUpper;
            TransduceKeyWord[2] = TransduceKeyWordToLower;
            
            int chapterNamesSamplesLength = GetChapterNamesSamplesLength(desiredTextLanguage);
            int[,] tempArrChapterNamesSamples = new int[baseKeyWordFormsQuantity, chapterNamesSamplesLength];//перебрать 3 возможные формы заглавия глав - все строчные, все прописные, первая прописная - такое же количество методов у делегата TransduceKeyWord

            for (int n = 0; n < chapterNamesSamplesLength; n++)//тут перебираем базовые формы ключевых слов
            {
                for (int m = 0; m < baseKeyWordFormsQuantity; m++)//тут перебираем варианты написания ключевых слов
                {
                    string baseKeyWord = GetChapterNamesSamples(desiredTextLanguage, n);
                    string currentKeyWordForm = TransduceKeyWord[m](baseKeyWord);
                    
                    bool currentWordOfParagraphCheck = currentWordOfParagraph.Contains(currentKeyWordForm);//проверяем, содержатся ли стандартные называния глав в строке - если в другое слово входит ключевое, то тоже находит - исправить
                    if (currentWordOfParagraphCheck)
                    {//надо еще проверять вариант, когда все буквы прописные (или часть?) - проще всего текущее слово перевести в строчные и ключевые сделать все из строчных
                        
                        int incrementFactor = GetChapterNamesVersionsCount(m, n); //сюда прибавляем количество встреченных ключевых слов, в ряд, в зависимости от найденной формы ключевого слова - чтобы потом выбрать похожее по количеству с номерами (не менее количества номеров, больше можно)
                        incrementFactor++;
                        SetChapterNamesVersionsCount(m, n, incrementFactor);
                        //нашли название главы
                        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "ChapterNamesVersionsCount(n) = " + GetChapterNamesVersionsCount(m, n).ToString() + strCRLF + 
                            "n = " + n.ToString() + "   /m = " + n.ToString(), CurrentClassName, showMessagesLevel);

                        if (jWordNumber > 0)
                        {
                            SymbolsFoundBeforeKeyWord(jWordNumber); //найденное ключевое слово не первое в строке, перед ним были какие-то символы, ищем их и сохраняем, чтобы потом было проще искать названия глав
                        }
                        return n;//возвращаем индекс найденного ключевого слова из массива
                    }
                }
            }
            return (int)MethodFindResult.NothingFound;//не нашли...
        }

        private string TransduceKeyWordToTitleCase(string baseKeyWord)
        {
            //проверять baseKeyWord чтобы не null и останавливать с диагностикой
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            string transduceWord = ti.ToTitleCase(baseKeyWord);
            return transduceWord;
        }

        private string TransduceKeyWordToUpper(string baseKeyWord)
        {
            string transduceWord = baseKeyWord.ToUpper();
            return transduceWord;
        }

        private string TransduceKeyWordToLower(string baseKeyWord)
        {            
            return baseKeyWord;
        }

        public void SymbolsFoundBeforeKeyWord(int j)//передать и получить все параметры
        {//найденное ключевое слово не первое, перед ним были какие-то символы - ищем их и складываем количество встретившихся групп в массив chapterSymbolsVersionsCount
         //потом сравнить с числом найденных глав и, если совпадает, то найден постоянная группа в названии главы - но зачем она?
            for (int m = 0; m < j; m++)//записываем группы символов до встреченного ключевого слова (текущее j)
            {
                if (GetFoundWordsOfParagraph(m) == GetFoundSymbolsOfParagraph(m))
                {
                    int incrementFactor = GetChapterSymbolsVersionsCount(m); //сюда прибавляем количество встреченных ключевых слов, в ряд, в зависимости от найденной формы ключевого слова - чтобы потом выбрать похожее по количеству с номерами (не менее количества номеров, больше можно)
                    incrementFactor++;
                    SetChapterSymbolsVersionsCount(m, incrementFactor);

                    //_arrayChapter.IncrementOfChapterSymbolsVersionsCount(m);//если новое значение равно предыдущему, то увеличиваем счетчик
                }
                SetFoundSymbolsOfParagraph(GetFoundWordsOfParagraph(m), m);// - это что такое тут делаем?
            //подумать - если левая группа испортит переменную, что потом будет? по идее через пару правильных групп счет восстановится                                                                      
            //заносить все равно надо, потому что вдруг уже первая группа будет неправильная - потом она постепенно заменится правильной
            //но вообще не очень правильно - надо все же все значения занести в массив и потом анализировать - исключения выкинуть и найти такое же количество групп, как и глав
            //здесь надо занести в массив (из 10-ти элементов) номер слова названия главы - по порядку в строке, т.е. есть ли перед названием лишние спецсимволы и другое
            }
        }

        public int WordsOfParagraphSearch(string currentParagraph)
        {//метод выделяет из строки (абзаца текста) первые десять (или больше - по размерности передаваемого массива) слов или чисел (и сохраняет лидирующую группу спецсимволов)
            if (String.IsNullOrWhiteSpace(currentParagraph))
            {
                return (int)MethodFindResult.NothingFound;//пустая строка без слов, вернули -1 для ясности
            }            
            string currentParagraphWithSingleBlanks = RemoveMoreThenOneBlank(currentParagraph);//убрать сдвоенные пробелы из строки
            currentParagraph = currentParagraphWithSingleBlanks;

            _arrayAnalysis.ClearFoundWordsOfParagraph();//можно передать контрольное число, подтверждающее, что массив можно очистить
            int foundWordsCount = 0;

            IsOrNotEqual isEqual = IsEqual;//check condition is current charArrayOfChapterNumber[] LetterOrDigit
            IsOrNotEqual isNotEqual = IsNotEqual;//check condition is NOT LetterOrDigit

            DoElseConditions stringIsNotNull = StringIsNotNull;//check condition is current word not null
            DoElseConditions jSubIisAboveOne = JsubIisAboveOne;//check condition is j-i > 1

            ////разделяем абзац на слова или числа и на скопления спецсимволов (если больше одного подряд)
            char[] charArrayOfChapterNumber = currentParagraph.ToCharArray();
            int charArrayOfChapterNumberLength = charArrayOfChapterNumber.Length;
            for (int i = 0; i < charArrayOfChapterNumberLength; i++)
            {
                if (foundWordsCount < _arrayAnalysis.GetFoundWordsOfParagraphLength())
                {
                    if (Char.IsLetterOrDigit(charArrayOfChapterNumber[i]))
                    {//проверка, есть ли цепочка букв-цифр
                        int j = SymbolGroupSaving(charArrayOfChapterNumber, foundWordsCount, i, isEqual, stringIsNotNull, 0);//0 - не надо вычитать единицу
                        foundWordsCount++;
                        i = j;
                    }
                    else
                    {//проверка, есть ли цепочка спецсимволов
                        int j = SymbolGroupSaving(charArrayOfChapterNumber, foundWordsCount, i, isNotEqual, jSubIisAboveOne, 1);//1 - надо вычитать единицу в одном месте
                        if (j - i > 1)
                        {
                            foundWordsCount++;//кстати - в методе тоже такое же сравнение, можно ли совместить?
                        }
                        i = j;
                    }
                }
            }
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "foundWordsCount = " + foundWordsCount.ToString() + strCRLF +
            "currentParagraph --> " + currentParagraph + strCRLF +
            "foundWordsOfParagraph[0] --> " + _arrayAnalysis.GetFoundWordsOfParagraph(0) + strCRLF +
            "foundWordsOfParagraph[1] --> " + _arrayAnalysis.GetFoundWordsOfParagraph(1) + strCRLF +
            "foundWordsOfParagraph[2] --> " + _arrayAnalysis.GetFoundWordsOfParagraph(2) + strCRLF +
            "foundWordsOfParagraph[3] --> " + _arrayAnalysis.GetFoundWordsOfParagraph(3) + strCRLF +
            "foundWordsOfParagraph[4] --> " + _arrayAnalysis.GetFoundWordsOfParagraph(4), CurrentClassName, showMessagesLevel);
            return foundWordsCount;
        }

        private int SymbolGroupSaving(char[] charArrayOfChapterNumber, int foundWordsCount, int i, IsOrNotEqual currentIf, DoElseConditions currentDoElse, int doMinusOne)//убрать все массивы из параметров всех методов
        {//метод вызывается для проверки, есть ли цепочка букв-цифр или спецсимволов (используется для WordsOfParagraphSearch, тестируется вместе с ним)
            string wordOfParagraph = "";
            int currentCharIndex = 0;
            int charArrayOfChapterNumberLength = charArrayOfChapterNumber.Length;

            for (int j = i; j < charArrayOfChapterNumberLength; j++)
            {
                currentCharIndex = j;
                bool resultIf = currentIf(charArrayOfChapterNumber[j]);//выбираем сравнение - символ буква-цифра и НЕ буква-цифра
                if (resultIf)
                {
                    wordOfParagraph = wordOfParagraph + charArrayOfChapterNumber[j];
                }
                else
                {
                    bool resultElse = currentDoElse(wordOfParagraph, i, j);//выбираем сравнение - непустое слово или j-i>1 - это означает, что найдена лидирующая группа спецсимволов, которую надо сохранить, как слово
                    if (resultElse)                    
                    {
                        _arrayAnalysis.SetFoundWordsOfParagraph(wordOfParagraph, foundWordsCount);
                    }
                    return currentCharIndex - doMinusOne;//вычитаем 1 или нет - по ситуации (вычитать надо когда?)
                }
            }
            bool resultIsNullOrEmpty = StringIsNotNull(wordOfParagraph, 0, 0);//в методе используется только первый аргумент, два других для совпадения вызова в делегате - ой, криво!
            if(resultIsNullOrEmpty)            
            {
                _arrayAnalysis.SetFoundWordsOfParagraph(wordOfParagraph, foundWordsCount);
            }
            return currentCharIndex;
        }

       
        private bool IsEqual(char x)
        {
            bool result = Char.IsLetterOrDigit(x);
            return result;                
        }

        private bool IsNotEqual(char x)
        {
            bool result = !Char.IsLetterOrDigit(x);
            return result;
        }

        private bool StringIsNotNull(string wordOfParagraph, int j, int i)
        {
            bool result = !string.IsNullOrEmpty(wordOfParagraph);
            return result;
        }

        private bool JsubIisAboveOne(string wordOfParagraph, int j, int i)
        {
            bool result = (j - i > 1);
            return result;
        }
        
        public string RemoveMoreThenOneBlank(string currentParagraph)
        {
            string currentParagraphWithSingleBlanks = "";
            char previousCharOfCurrentParagraph = '¶';
            
            int resultSymbols = 0;
            int sourceSymbols = 0;

            foreach (char charOfcurrentParagraph in currentParagraph)
            {
                if (charOfcurrentParagraph == ' ' && previousCharOfCurrentParagraph == ' ')
                { }
                else
                {
                    currentParagraphWithSingleBlanks = currentParagraphWithSingleBlanks + charOfcurrentParagraph;
                    resultSymbols++;
                }
                previousCharOfCurrentParagraph = charOfcurrentParagraph;
                //singleBlankSpaceDetected = true;
                sourceSymbols++;
            }
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), 
                "foreach sourceSymbols = " + sourceSymbols.ToString() + strCRLF +
                "resultSymbols = " + resultSymbols.ToString() + strCRLF +
                "Source string --> " + currentParagraph + strCRLF +
                "Result string --> " + currentParagraphWithSingleBlanks, CurrentClassName, showMessagesLevel);
            return currentParagraphWithSingleBlanks;
        }
        
        public string FindDigitsInChapterName(string paragraphWihtChapterName)
        {
            //в цикле проверяем символ на цифру, если да, то записываем в строку и ставим флаг, что цифра
            //если следующий символ тоже цифра, то поддерживаем флаг и записываем в строку, если нет - ?
            //string digitsOfChapterNumber = FindDigitsInChapterName(currentParagraph);//получили строку с цифрами в названии главы            

            string digitsOfChapterNumber = "";
            char[] isStringChapterNumber = paragraphWihtChapterName.ToCharArray();
            int digitAtChapterNameCount = 0;
            bool isDigitAtChapterName = false;

            foreach (char charOfChapterNumber in isStringChapterNumber)
            {
                isDigitAtChapterName = Char.IsDigit(charOfChapterNumber);
                if (isDigitAtChapterName)
                {
                    digitsOfChapterNumber = digitsOfChapterNumber + charOfChapterNumber.ToString();
                    digitAtChapterNameCount++;
                }
                else
                {
                    if (digitAtChapterNameCount != 0) return digitsOfChapterNumber;
                }
            }
            if (digitAtChapterNameCount != 0) return digitsOfChapterNumber;
            else return null;
        }
        
        public int ChapterKeyNamesAnalysis(int desiredTextLanguage)
        {
            int maxKeyNameLength = 0;
            int previousKeyNameLength = 0;

            for (int n = 0; n < _arrayAnalysis.GetChapterNamesSamplesLength(desiredTextLanguage); n++)
            {
                string currentKeyName = _arrayAnalysis.GetChapterNamesSamples(desiredTextLanguage, n);
                int currentKeyNameLength = currentKeyName.Length;
                if (n > 0 && currentKeyNameLength > previousKeyNameLength)
                {
                    maxKeyNameLength = currentKeyNameLength;
                }
                previousKeyNameLength = currentKeyNameLength;
            }
            return maxKeyNameLength;
        }

        public int UserSelectedChapterNameAnalysis(int desiredTextLanguage)//метод, когда не получился анализ глав со стандартными названиями, тогда просим подсказки у пользователя
        {//пока не используется, потом придется переписать 
            string userSelectedText = _bookData.GetSelectedText(desiredTextLanguage);//получили припрятанный фрагмент, выбранный пользователем - предположительно вид нумерации глав

            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "In the TextBox " + desiredTextLanguage.ToString() + strCRLF +
               "User Selected the following Text ==> " + userSelectedText, CurrentClassName, showMessagesLevel);

            int lengthUserSelectedText = userSelectedText.Length;//считаем, что длина больше нуля, иначе не попали бы сюда
            //сначала ищем полученный фрагмент в тексте, чтобы добыть и проверить предыдущие (и последующие) строки - т.е. абзацы
            int findChapterResult = 0; //FindParagraphTextNumber(userSelectedText, desiredTextLanguage, 0);//в результате получим номер элемента, в котором нашелся фрагмент
            if (findChapterResult > 0)//в то, что название главы в первой строке книги, мы не верим
            {
                string previousParagraphChapterName = _bookData.GetParagraphText(findChapterResult - 1, desiredTextLanguage);
                string paragraphWihtChapterName = _bookData.GetParagraphText(findChapterResult, desiredTextLanguage);
                //string nextParagraphChapterName = _book.GetParagraphText(findChapterResult + 1, desiredTextLanguage);

                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "findChapterResult = " + findChapterResult.ToString() + strCRLF +
                    "previousParagraphChapterName - " + previousParagraphChapterName + strCRLF +
                    "paragraphWihtChapterName - " + paragraphWihtChapterName + strCRLF +
                    "nextParagraphChapterName - " + "nextParagraphChapterName", CurrentClassName, showMessagesLevel);
                //тут надо убедиться, что предыдущая (и желательно последующая) строки - пустые, если последующая не пустая - запросить доп.подтверждение у пользователя

                //а пока проверим, совпадает ли фрагмент с найденной строкой и есть ли в нем цифры
                if (userSelectedText == paragraphWihtChapterName)
                {
                    string digitsOfChapterNumber = FindDigitsInChapterName(paragraphWihtChapterName);//получили строку с цифрами в названии главы

                    if (digitsOfChapterNumber.Length > 0)//если хоть одна цифра нашлась, выделяем названия глав без цифр (типа - Глава )
                    {
                        char[] DigitsOfChapterNumber = digitsOfChapterNumber.ToCharArray();
                        string chapterNameWithoutDigits = paragraphWihtChapterName.Trim(DigitsOfChapterNumber);
                        //проверить все строки (абзацы) текста - есть ли там еще chapterNameWithoutDigits с увеличивающимися номерами
                        int paragraphTextLength = _bookData.GetParagraphTextLength(desiredTextLanguage);
                        int startParagraphTextNumber = 0;
                        int chaptersMaxNumber = 0;
                        int AddChapterNumberLength = 0;
                        for (int i = 0; i < paragraphTextLength; i++)
                        {
                            int findChapterNameWithoutDigitsResult = 0;//FindParagraphTextNumber(chapterNameWithoutDigits, desiredTextLanguage, startParagraphTextNumber);//в результате получим номера элементов, в которых нашлась "Глава"
                            if (findChapterNameWithoutDigitsResult > 0)
                            {
                                //тут массив названий из book, заполняемый полными названиями глав (с номерами)                            
                                string foundChapterName = _bookData.GetParagraphText(findChapterNameWithoutDigitsResult, desiredTextLanguage);//достаем текст абзаца, номер которого получили от FindParagraphTextNumber - там имя главы
                                int AddChapterNameLength = _bookData.AddChapterName(foundChapterName, desiredTextLanguage);//полное имя главы в массив имен глав

                                int chapterNumber = Convert.ToInt32(FindDigitsInChapterName(foundChapterName));//получили строку с цифрами в названии главы
                                AddChapterNumberLength = _bookData.AddChapterNumber(chapterNumber, desiredTextLanguage);

                                if (findChapterNameWithoutDigitsResult < paragraphTextLength) startParagraphTextNumber = findChapterNameWithoutDigitsResult + 1;//на следующем круге начинаем искать после предыдущего нахождения
                                                                                                                                                                //после выхода взять массив названий и проанализировать его на возрастание номеров глав
                                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "i = " + i.ToString() + "and paragraphTextLength = " + paragraphTextLength.ToString() + strCRLF +
                                    "startParagraphTextNumber = " + startParagraphTextNumber.ToString() + strCRLF +
                                    "foundChapterName - " + foundChapterName + strCRLF +
                                    "chapterNumber - " + chapterNumber.ToString(), CurrentClassName, 3);
                                chaptersMaxNumber++; // счетчик найденных глав, чтобы проверить идут ли номера глав с последовательным возрастанием - сделать по другому - если главы с 0 или с 1
                            }
                        }
                        if ((chaptersMaxNumber - AddChapterNumberLength) >= 0)//нашлось хоть одно имя главы
                        {
                            return chaptersMaxNumber;
                        }
                    }
                }
            }
            return 0;
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
        
    }

}
//
//_msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "BEFORE IF currentParagraph = " + currentParagraph + "   i = " + i.ToString(), CurrentClassName, 3);
//if (!String.IsNullOrWhiteSpace(currentParagraph))//если текущая строка не пустая, получаем и смотрим предыдущий абзац
//{
//    string previousParagraph = GetParagraphText((i - 1), desiredTextLanguage);
//    //если предыдущая пустая, то начинаем анализ строки (абзаца), выбираем первые 10 групп символов (по пробелу) и ищем в них название (ключевые слова) и номера глав
//    if (String.IsNullOrWhiteSpace(previousParagraph))//метод вызывается не на каждый абзац, а в нем массив для каждого абзаца!
//    {
//        //_msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "IN IF currentParagraph = " + currentParagraph + "   i = " + i.ToString(), CurrentClassName, 3);
//
//int SymbolGroupSaving1(char[] charArrayOfChapterNumber, string[] foundWordsOfParagraph, int foundWordsCount, int i, IsOrNotEqual currentIf, DoElseCircumstance currentDoElse, int doMinusOne)
//{
//    string wordOfParagraph = "";
//    int currentCharIndex = 0;
//    int charArrayOfChapterNumberLength = charArrayOfChapterNumber.Length;

//    for (int j = i; j < charArrayOfChapterNumberLength; j++)
//    {
//        currentCharIndex = j;
//        bool result = currentIf(charArrayOfChapterNumber[j]);                
//        if (result)
//        {
//            wordOfParagraph = wordOfParagraph + charArrayOfChapterNumber[j];
//        }
//        else
//        {
//            bool result1 = currentDoElse(wordOfParagraph, i, j);
//            if (result1)
//                //if (j - i > 1)
//            {
//                foundWordsOfParagraph[foundWordsCount] = wordOfParagraph;
//            }
//            return currentCharIndex - doMinusOne;
//        }
//    }
//    return currentCharIndex;
//}
//
//foreach (char charOfChapterNumber in currentParagraph)
//{               
//    if (i < findWordsCount)//если массив еще не заполнен, заполняем (если будет переполняться, вычесть 1)
//    {
//        if (Char.IsLetterOrDigit(charOfChapterNumber))//слабое место, что может быть комбинация букв и цифр - протестировать этот вариант
//        {
//            if (flagSymbolsStarted > 1)
//            {//найдена цепочка спецсимволов больше одного подряд (она только что завершилась)
//                foundWordsOfParagraph[i] = symbolsOfParagraph;                            
//                symbolsOfParagraph = "";
//                i++;
//            }
//            flagSymbolsStarted = 0; //цепочка символов прервалась, сбрасываем счетчик                    
//            wordOfParagraph = wordOfParagraph + charOfChapterNumber;//нашли начало слова (после возможных спецсимволов в начале строки) и собираем слово, пока идут буквы (или цифры)
//            flagWordStarted++;
//        }
//        else
//        {//слово кончилось (или еще не началось)
//            if (flagWordStarted > 0)
//            {
//                if (charOfChapterNumber == ' ')
//                {//нашли пробел после него, прибавляем его к слову для совпадения с ключевыми словами - сомнительное действие, надо предусмотреть дефис и прочие варианты
//                    foundWordsOfParagraph[i] = wordOfParagraph + charOfChapterNumber;                                
//                    wordOfParagraph = "";
//                    i++;
//                }                            
//            }
//            flagWordStarted = 0; //цепочка букв или цифр прервалась, сбрасываем счетчик                        
//            symbolsOfParagraph = symbolsOfParagraph + charOfChapterNumber;
//            flagSymbolsStarted++;                        
//        }
//    }
//    else
//    {
//        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "else i = " + i.ToString() + strCRLF +
//            "foundWordsOfParagraph[0] --> " + foundWordsOfParagraph[0] + strCRLF +
//            "foundWordsOfParagraph[1] --> " + foundWordsOfParagraph[1] + strCRLF +
//            "foundWordsOfParagraph[2] --> " + foundWordsOfParagraph[2] + strCRLF +
//            "foundWordsOfParagraph[3] --> " + foundWordsOfParagraph[3] + strCRLF +
//            "foundWordsOfParagraph[4] --> " + foundWordsOfParagraph[4] + strCRLF +
//            "flagWordStarted - " + flagWordStarted.ToString(), CurrentClassName, 3); 
//        return i;
//    }
//}
//if (flagWordStarted > 0)
//{
//    foundWordsOfParagraph[i] = wordOfParagraph;
//    i++;
//}
//if (flagSymbolsStarted > 1)
//{
//    foundWordsOfParagraph[i] = symbolsOfParagraph;
//    i++;
//}
//_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "foreach i = " + i.ToString() + strCRLF +
//            "foundWordsOfParagraph[0] --> " + foundWordsOfParagraph[0] + strCRLF +
//            "foundWordsOfParagraph[1] --> " + foundWordsOfParagraph[1] + strCRLF +
//            "foundWordsOfParagraph[2] --> " + foundWordsOfParagraph[2] + strCRLF +
//            "foundWordsOfParagraph[3] --> " + foundWordsOfParagraph[3] + strCRLF +
//            "foundWordsOfParagraph[4] --> " + foundWordsOfParagraph[4] + strCRLF +
//            "flagWordStarted - " + flagWordStarted.ToString(), CurrentClassName, 3);
//
//пусть он работает в обратном порядке (удалите с самого высокого индекса на самый низкий)
//List<T>.RemoveAll(Predicate<T>) или LINQ для замены исходного списка новым списком путем фильтрации элементов 
//Когда вы вызываете RemoveAt для удаления элемента, остальные элементы в списке перенумеровываются, чтобы заменить удаленный элемент. 
//Например, если вы удаляете элемент по индексу 3, элемент в индексе 4 перемещается в позицию 3. Поэтому удаляйте элементы из хвоста массива!
//
//private int CurrentParagraphsPortioned(int desiredTextLanguage)
//{
//    int ID_Chapter = -1;// - gimp stick?
//    int ID_Paragraph = 0;
//    int ID_Sentenses = 0;

//    int iSentStep = 0;

//    int previousParagraphLength = -1;//start flag value - before Paragraph exists

//    foreach (string currentParagraph in currentTextParagraphsPortioned)//place in currentParagraph each Paragraph
//    {
//        int lengthOfCurrentParagraph = currentParagraph.Length;//count symbols quantity in the current Paragraph - придумать более осмысленый параметр
//        string paragraph_name = lengthOfCurrentParagraph.ToString();//(for test only)

//        #region Chapters 
//        //ID_Chapter = -1; on the first pass of the foreach
//        //find Chapters and set Chapters ID and name
//        bool isPreviousLineBlank = previousParagraphLength == 0;//check if previous line was blank
//        if (iSentStep > 0)//if not first step
//        {
//            ID_Chapter = FindChapterNumber(currentParagraph, desiredTextLanguage, ID_Chapter, isPreviousLineBlank);
//        }
//        #endregion
//        #region Paragraphs & Sentences
//        if (ID_Chapter >= 0)//if the first Chapter has not inserted in dB yet, we cannot insert the Paragraph
//        {
//            Array.Clear(dataBaseTableToDo, 0, dataBaseTableToDo.Length);
//            dataBaseTableToDo[(int)TablesNamesNumbers.Paragraphs] = (int)WhatNeedDoWithTables.InsertRecord;// - массив объявлен вне метода и не передан в метод
//            //dataBaseTableNames[(int)TablesNamesNumbers.Paragraphs] - искать имя таблицы по индексу dataBaseTableToDo который равен InsertRecord
//            int insertResultParagraphs = _data.InsertRecordInTable(dataBaseTableNames, dataBaseTableToDo, ID_Paragraph, desiredTextLanguage, ID_Chapter, lengthOfCurrentParagraph, paragraph_name);//Insert Record in Table Paragraphs
//            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " insertResult after Paragraphs ==> ", insertResultParagraphs.ToString(), CurrentClassName, showMessagesLevel);
//            string[] currentParagraphSentences = currentParagraph.Split(charsSentenceSeparator);//portioned current paragraph on sentences - съел все точки и прочие окончания предложений!
//                                                                                                // - написать свое разделение на предложния с учетом кавычек и всяких p.m. 
//                                                                                                // - показать пользователю проблемные места в поле редактора формы - точки без последующих пробелов и т.д. 
//                                                                                                // - чтобы пользователь мог написать маски для обработки
//            ID_Sentenses = PortionParagraphOnSentences(currentParagraphSentences, desiredTextLanguage, ID_Chapter, ID_Paragraph, ID_Sentenses);
//            ID_Paragraph++;
//        }
//        #endregion

//        iSentStep++;//count of paragraphs
//        previousParagraphLength = currentParagraph.Length;//count symbols quantity in the current Paragraph - will be in previous Paragraph on the next pass

//        // to print in file
//        //int textParagraphesCount = textParagraphs.Count;
//        //string[] textParagraphesArray = new string[textParagraphesCount];
//        //textParagraphesArray = textParagraphs.ToArray();
//        //_manager.WriteToFilePathPlus(textParagraphesArray, filesPath[(int)TableLanguagesContent.English], "001");
//    }
//    return 0;
//}


