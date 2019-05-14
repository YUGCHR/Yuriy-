using System;
using System.Reflection;
using TextSplitLibrary;

namespace TextSplit
{
    public interface IAnalysisLogicChapter
    {
        int ChapterNameAnalysis(int desiredTextLanguage);
        int UserSelectedChapterNameAnalysis(int desiredTextLanguage);       
        string FindDigitsInChapterName(string paragraphWihtChapterName);
    }

    public class AnalysisLogicChapter : IAnalysisLogicChapter
    {
        private readonly IAllBookData _bookData;
        private readonly IMessageService _msgService;
        private readonly IAnalysisLogicChapterDataArrays _arrayChapter;

        delegate bool IsOrNotEqual(char x);
        delegate bool DoElseConditions(string x, int i, int j);

        readonly private int showMessagesLevel;
        readonly private string strCRLF;

        public AnalysisLogicChapter(IAllBookData bookData, IMessageService msgService, IAnalysisLogicChapterDataArrays arrayChapter)
        {
            _bookData = bookData;
            _msgService = msgService;
            _arrayChapter = arrayChapter;

            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;
        }

        public int ChapterNameAnalysis(int desiredTextLanguage)//Main здесь - ищем все названия глав, складываем их по массивам, узнаем их количество и возвращаем его
        {            
            int paragraphTextLength = _bookData.GetParagraphTextLength(desiredTextLanguage);
            int[] chapterNameIsDigitsOnly = new int[paragraphTextLength];
            string[] allTextWithChapterNames = new string[paragraphTextLength];

            int maxKeyNameLength = ChapterKeyNamesAnalysis(desiredTextLanguage);//ищем слово с макс.символов из ключевых (непонятно, зачем это нужно, но пока пусть будет)
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                "Mark of begining Chapter --> " + _arrayChapter.GetStringMarksChapterName("Begin") + strCRLF +
                "Mark of the end of Chapter --> " + _arrayChapter.GetStringMarksChapterName("End") + strCRLF +
                "maxKeyNameLength = " + maxKeyNameLength.ToString() + strCRLF +
                "chapterNamesSamplesCount = " + _arrayChapter.GetChapterNamesSamplesLength(desiredTextLanguage), CurrentClassName, 3);//тестовая печать будущей маркировки глав, абзацев и прочего - просто проверка символов

            for (int i = 1; i < paragraphTextLength; i++)//перебираем все абзацы текста, начиная с второго - в первом главы быть не должно
            {
                string currentParagraph = _bookData.GetParagraphText(i, desiredTextLanguage);

                if (!String.IsNullOrWhiteSpace(currentParagraph))//если текущая строка не пустая, получаем и смотрим предыдущий абзац
                {
                    string previousParagraph = _bookData.GetParagraphText((i - 1), desiredTextLanguage);
                    //если предыдущая пустая, то начинаем анализ строки (абзаца), выбираем первые 10 групп символов (по пробелу) и ищем в них название (ключевые слова) и номера глав
                    if (String.IsNullOrWhiteSpace(previousParagraph))
                    {
                        FirstTenGroupsChecked(currentParagraph, chapterNameIsDigitsOnly, i, desiredTextLanguage);//преобразовать массив chapterNameIsDigitsOnly тоже в метод доступа к массиву?
                    }
                }
            }
            //в этом массиве значения показывают, сколько раз какое ключевое слово встретилось в тексте
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Must be found 52 the same chapters name" + strCRLF +
            "ChapterNamesVersionsCount[0] --> " + _arrayChapter.GetChapterNamesVersionsCount(0) + strCRLF +
            "ChapterNamesVersionsCount[1] --> " + _arrayChapter.GetChapterNamesVersionsCount(1) + strCRLF +
            "ChapterNamesVersionsCount[2] --> " + _arrayChapter.GetChapterNamesVersionsCount(2) + strCRLF +
            "ChapterNamesVersionsCount[3] --> " + _arrayChapter.GetChapterNamesVersionsCount(3) + strCRLF +
            "ChapterNamesVersionsCount[4] --> " + _arrayChapter.GetChapterNamesVersionsCount(4), CurrentClassName, showMessagesLevel);

            for (int i = 0; i < _arrayChapter.GetChapterNamesSamplesLength(desiredTextLanguage); i++)
            {
                //проверяем содержимое chapterNameIsDigitsOnly и chapterNamesVersionsCount
                if(_arrayChapter.GetChapterNamesVersionsCount(i) > 0) _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "chapterNamesVersionsCount - " + _arrayChapter.GetChapterNamesVersionsCount(i).ToString(), CurrentClassName, 3);
            }
            
            int increasedChapterNumbers = IsChapterNumbersIncreased(chapterNameIsDigitsOnly, desiredTextLanguage);
            
            //тут уже 52 раза найдено ключевое слово и найдено 52 номера глав (для контрольного текста)
            //теперь заново анализировать текст и искать уже известные названия глав (и в известном количестве)
            //перебирать абзацы, записывать их во временный массив и помечать названия глав - проблема вставить дополнительные строки для разметки

            for (int i = 0; i < paragraphTextLength; i++)
            {
                //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "t = " + t.ToString() + strCRLF + "workChapterNameIsDigitsOnly - " + workChapterNameIsDigitsOnly[i].ToString(), CurrentClassName, 3);
                //тут переписываем массив _book.GetParagraphText(i, desiredTextLanguage) во временный массив allTextWithChapterNames
                //allTextWithChapterNames[i] = _book.GetParagraphText(i, desiredTextLanguage);
                
            }
            //обнулить массив _book.GetParagraphText(i, desiredTextLanguage)
            //


            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "increasedChapterNumbers = " + increasedChapterNumbers.ToString(), CurrentClassName, 3);
            return increasedChapterNumbers;
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
            //for (int i = 0; i < paragraphTextLength; i++)
            //{
            //    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "workChapterNameIsDigitsOnly = " + workChapterNameIsDigitsOnly[i], CurrentClassName, 3);
            //}
            return increasedChapterNumbers;
        }

        public void FirstTenGroupsChecked(string currentParagraph, int[] chapterNameIsDigitsOnly, int iParagraphNumber, int desiredTextLanguage)//придумать внятное название
        {//начало анализа строки (абзаца)
            int firstNumberOfParagraph = 0;
            int keyWordChapterName = 0;
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "currentParagraph = " + currentParagraph, CurrentClassName, showMessagesLevel);
            int foundWordsOfParagraphCount = WordsOfParagraphSearch(currentParagraph);//выделяем в текущем параграфе первые 10 групп (сохраняются в массиве-методе), получаем количество слов-чисел + лидирующая группа символов

            for (int j = 0; j < foundWordsOfParagraphCount; j++)//перебираем все полученные слова - ищем числа и ключевые слова
            {
                string currentWordOfParagraph = _arrayChapter.GetFoundWordsOfParagraph(j);
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "currentWordOfParagraph = " + currentWordOfParagraph, CurrentClassName, showMessagesLevel);
                if (currentWordOfParagraph != null)
                {
                    bool successSearchNumber = Int32.TryParse(currentWordOfParagraph, out firstNumberOfParagraph);//проверяем найденное слово, что оно число - ищем номера глав
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "currentWordOfParagraph = " + currentWordOfParagraph + strCRLF + "is digit = " + successSearchNumber.ToString(), CurrentClassName, showMessagesLevel);
                    if (successSearchNumber)
                    {//складываем найденные номера глав, потом проверим на монотонное возрастание и выкинем лишние
                        chapterNameIsDigitsOnly[iParagraphNumber] = firstNumberOfParagraph + 1; //(+1 - чтобы избавиться от нулевой главы, бывают и такие, потом не забыть отнять)
                        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "firstNumberOfParagraph = " + firstNumberOfParagraph.ToString(), CurrentClassName, showMessagesLevel);
                        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "chapterNameIsDigitsOnly[iParagraphNumber] = " + chapterNameIsDigitsOnly[iParagraphNumber].ToString(), CurrentClassName, showMessagesLevel);
                    }
                    else
                    {//количество найденных ключевых слов сохраняем в массиве _arrayChapter.IncrementOfChapterNamesVersionsCount(n), индекс найденного слова совпадает с индексом наибольшего значения в массиве
                        keyWordChapterName = CheckWordOfParagraphCompare(currentWordOfParagraph, j, desiredTextLanguage);//проверяем текущее слово на совпадение с ключевыми                        
                    }
                }
            }            
        }

        public int CheckWordOfParagraphCompare(string currentWordOfParagraph, int jWordNumber, int desiredTextLanguage)//проверяем полученное слово на совпадение с ключевыми из массива
        {
            currentWordOfParagraph = currentWordOfParagraph + " ";//добавили пробел для совпадения с ключевыми словами - потом это убрать, текущие слова все равно проверяются по спецсимволу за ними - может быть и тире
            for (int n = 0; n < _arrayChapter.GetChapterNamesSamplesLength(desiredTextLanguage); n++)
            {
                bool currentWordOfParagraphCheck = currentWordOfParagraph.Contains(_arrayChapter.GetChapterNamesSamples(desiredTextLanguage, n));//проверяем, содержатся ли стандартные называния глав в строке - с пробелом после слова (может заменить на Equals)
                if (currentWordOfParagraphCheck)
                {//надо еще проверять вариант, когда все буквы прописные (или часть?) - проще всего текущее слово перевести в строчные и ключевые сделать все из строчных
                    _arrayChapter.IncrementOfChapterNamesVersionsCount(n); //сюда прибавляем количество встреченных ключевых слов, чтобы потом выбрать похожее по количеству с номерами (не менее количества номеров, больше можно)

                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "ChapterNamesVersionsCount(n) = " + _arrayChapter.GetChapterNamesVersionsCount(n).ToString() + strCRLF + "n = " + n.ToString(), CurrentClassName, showMessagesLevel);

                    //нашли название главы, теперь как-то найти номер сразу за названием - похоже, номер ищется автоматически - в следующей группе символов за ключевым словом
                    if (jWordNumber > 0) SymbolsFoundBeforeKeyWord(jWordNumber); //найденное ключевое слово не первое в строке, перед ним были какие-то символы, ищем их и сохраняем, чтобы потом было проще искать названия глав
                    return n;//возвращаем индекс найденного ключевого слова из массива
                }
            }
            return (int)MethodFindResult.NothingFound;//не нашли...
        }

        public void SymbolsFoundBeforeKeyWord(int j)//передать и получить все параметры
        {//найденное ключевое слово не первое, перед ним были какие-то символы - ищем их и складываем количество встретившихся групп в массив chapterSymbolsVersionsCount
         //потом сравнить с числом найденных глав и, если совпадает, то найден постоянная группа в названии главы - но зачем она?
            for (int m = 0; m < j; m++)//записываем группы символов до встреченного ключевого слова (текущее j)
            {
                if (_arrayChapter.GetFoundWordsOfParagraph(m) == _arrayChapter.GetFoundSymbolsOfParagraph(m))
                {
                    _arrayChapter.IncrementOfChapterSymbolsVersionsCount(m);//если новое значение равно предыдущему, то увеличиваем счетчик
                }
                _arrayChapter.SetFoundSymbolsOfParagraph(_arrayChapter.GetFoundWordsOfParagraph(m), m);
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

            _arrayChapter.ClearFoundWordsOfParagraph();//можно передать контрольное число, подтверждающее, что массив можно очистить
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
                if (foundWordsCount < _arrayChapter.GetFoundWordsOfParagraphLength())
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
            "foundWordsOfParagraph[0] --> " + _arrayChapter.GetFoundWordsOfParagraph(0) + strCRLF +
            "foundWordsOfParagraph[1] --> " + _arrayChapter.GetFoundWordsOfParagraph(1) + strCRLF +
            "foundWordsOfParagraph[2] --> " + _arrayChapter.GetFoundWordsOfParagraph(2) + strCRLF +
            "foundWordsOfParagraph[3] --> " + _arrayChapter.GetFoundWordsOfParagraph(3) + strCRLF +
            "foundWordsOfParagraph[4] --> " + _arrayChapter.GetFoundWordsOfParagraph(4), CurrentClassName, showMessagesLevel);
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
                        _arrayChapter.SetFoundWordsOfParagraph(wordOfParagraph, foundWordsCount);
                    }
                    return currentCharIndex - doMinusOne;//вычитаем 1 или нет - по ситуации (вычитать надо когда?)
                }
            }
            bool resultIsNullOrEmpty = StringIsNotNull(wordOfParagraph, 0, 0);//в методе используется только первый аргумент, два других для совпадения вызова в делегате - ой, криво!
            if(resultIsNullOrEmpty)            
            {
                _arrayChapter.SetFoundWordsOfParagraph(wordOfParagraph, foundWordsCount);
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

            for (int n = 0; n < _arrayChapter.GetChapterNamesSamplesLength(desiredTextLanguage); n++)
            {
                string currentKeyName = _arrayChapter.GetChapterNamesSamples(desiredTextLanguage, n);
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


