using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;

namespace TextSplit
{
        public interface IAnalysisLogicChapter
    {
        int ChapterNameAnalysis(int desiredTextLanguage);//clogic
        int UserSelectedChapterNameAnalysis(int desiredTextLanguage);//clogic       
        string FindDigitsInChapterName(string paragraphWihtChapterName);//clogic        

        //event EventHandler AnalyseInvokeTheMain;
    }

    public class AnalysisLogicChapter : IAnalysisLogicChapter
    {
        private readonly IAllBookData _book;
        private readonly IMessageService _messageService;

        readonly private int filesQuantity;
        readonly private int textFieldsQuantity;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;
        readonly private string[,] chapterNamesSamples;
        readonly private string stringMarksChapterNameBegin;
        readonly private string stringMarksChapterNameEnd;
        readonly private char[] charsParagraphSeparator;
        readonly private char[] charsSentenceSeparator;
        readonly private int chapterNamesSamplesCount;

        private string[] foundWordsOfParagraph;
        private string[] foundSymbolsOfParagraph;
        //private string[] allTextWithChapterNames;
        private int[] chapterNamesVersionsCount;
        private int[] chapterSymbolsVersionsCount;
        private char[] foundCharsSeparator;
        //private readonly int [] maxKeyNameLength;
        //public event EventHandler AnalyseInvokeTheMain;

        public AnalysisLogicChapter(IAllBookData book, IMessageService service)
        {
            _book = book;
            _messageService = service;

            filesQuantity = Declaration.FilesQuantity;
            textFieldsQuantity = Declaration.TextFieldsQuantity;
            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;

            //потом перенести все константы в Declaration - разделить там все на разделы по классам
            charsParagraphSeparator = new char[] { '\r', '\n' };
            charsSentenceSeparator = new char[] { '.', '!', '?' };
            stringMarksChapterNameBegin = "\u00A4\u00A4\u00A4\u00A4\u00A4";//¤¤¤¤¤ - метка строки перед началом названия главы
            stringMarksChapterNameEnd = "\u00A4\u00A4\u00A4";//¤¤¤ - метка строки после названия главы, еще \u00A7 - §, \u007E - ~, \u00B6 - ¶            
            //проверить типовые названия глав (для разных языков свои) - сделать метод универсальным и для частей тоже? или некоторые методы метода - перенести их тогда в общую логику
            chapterNamesSamples = new string[,]
            { { "Chapter ", "Paragraph ", "Section ", "Subhead ", "Part " },
                { "Глава ", "Параграф " , "Раздел ", "Подраздел ", "Часть " }, };
            //а номера глав бывают буквами! то мелочи, ключевые слова могуть быть из прописных букв, может быть дефис между словом и номером или другой символ
            chapterNamesSamplesCount = chapterNamesSamples.GetLength(1); //при добавлении значений проверить присвоение длины!

            foundWordsOfParagraph = new string[10];//временное хранение найденных первых десяти слов абзаца
            foundSymbolsOfParagraph = new string[10];//временное хранение найденных групп спецсимволов перед ключевым словом главы
            foundCharsSeparator = new char[10];//временное хранение найденных вариантов разделителей
            chapterNamesVersionsCount = new int[chapterNamesSamplesCount];
            chapterSymbolsVersionsCount = new int[chapterNamesSamplesCount];
        }

        public int ChapterNameAnalysis(int desiredTextLanguage)//ищем все названия глав, складываем их по массивам, узнаем их количество и возвращаем его
        {
            int findWordsCount = foundWordsOfParagraph.Length;            
            int paragraphTextLength = _book.GetParagraphTextLength(desiredTextLanguage);
            int[] chapterNameIsDigitsOnly = new int[paragraphTextLength];
            string[] allTextWithChapterNames = new string[paragraphTextLength];

            int maxKeyNameLength = ChapterKeyNamesAnalysis(desiredTextLanguage);//непонятно, зачем это нужно, но пока пусть будет
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                "Mark of begining Chapter --> " + stringMarksChapterNameBegin + strCRLF +
                "Mark of the end of Chapter --> " + stringMarksChapterNameEnd + strCRLF +
                "maxKeyNameLength = " + maxKeyNameLength.ToString() + strCRLF +
                "chapterNamesSamplesCount = " + chapterNamesSamplesCount, CurrentClassName, 3);

            for (int i = 1; i < paragraphTextLength; i++)//перебираем все абзацы текста, начиная с второго - в первом главы быть не должно
            {
                string currentParagraph = _book.GetParagraphText(i, desiredTextLanguage);

                if (!String.IsNullOrWhiteSpace(currentParagraph))//если текущая строка не пустая, получаем и смотрим предыдущий абзац
                {
                    string previousParagraph = _book.GetParagraphText((i - 1), desiredTextLanguage);
                    //если предыдущая пустая, то начинаем анализ строки (абзаца), выбираем первые 10 групп символов (по пробелу) и ищем в них название (ключевые слова) и номера глав
                    if (String.IsNullOrWhiteSpace(previousParagraph)) FirstTenGroupsFound(currentParagraph, chapterNameIsDigitsOnly, i, desiredTextLanguage);
                }
            }

            for (int i = 0; i < chapterNamesSamplesCount; i++)
            {
                //проверяем содержимое chapterNameIsDigitsOnly и chapterNamesVersionsCount
                if(chapterNamesVersionsCount[i] > 0) _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "chapterNamesVersionsCount - " + chapterNamesVersionsCount[i].ToString(), CurrentClassName, 3);
            }
            
            int increasedChapterNumbers = isChapterNumbersIncreased(chapterNameIsDigitsOnly, desiredTextLanguage);
            
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


            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "increasedChapterNumbers = " + increasedChapterNumbers.ToString(), CurrentClassName, 3);
            return increasedChapterNumbers;
        }

        int isChapterNumbersIncreased(int[] chapterNameIsDigitsOnly, int desiredTextLanguage)
        {//выкинуть нули и лишние цифры из номеров глав, проверить, что остальные цифры идут по порядку, посчитать количество номеров глав
            int paragraphTextLength = _book.GetParagraphTextLength(desiredTextLanguage);
            int[] workChapterNameIsDigitsOnly = new int[paragraphTextLength];
            int increasedChapterNumbers = 0;
            int working = -1;
            for (int i = 0; i < paragraphTextLength; i++)
            {
                if (chapterNameIsDigitsOnly[i] != 0)
                {
                    workChapterNameIsDigitsOnly[increasedChapterNumbers] = chapterNameIsDigitsOnly[i] - 1;

                    if (working == (workChapterNameIsDigitsOnly[increasedChapterNumbers] - 1))
                    {
                        working = workChapterNameIsDigitsOnly[increasedChapterNumbers];
                        increasedChapterNumbers++;
                        //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "t = " + t.ToString(), CurrentClassName, 3);
                    }
                }
            }
            return increasedChapterNumbers;
        }

        void FirstTenGroupsFound(string currentParagraph, int[] chapterNameIsDigitsOnly, int i, int desiredTextLanguage)
        {//начало анализа строки (абзаца)
            int firstNumberOfParagraph = 0;
            int keyWordChapterName = 0;
            //int[] chapterNameIsDigitsOnly = new int[paragraphTextLength];
            int foundWordsOfParagraphCount = WordsOfParagraphSearch(currentParagraph, foundWordsOfParagraph);//выделяем в текущем параграфе первые 10 групп символов (слова, числа или группы спецсимволов)

            for (int j = 0; j <= foundWordsOfParagraphCount; j++)//перебираем все полученные слова из начала абзаца - ищем числа и ключевые слова
            {
                string testWordOfParagraph = foundWordsOfParagraph[j];
                if (testWordOfParagraph != null)
                {
                    bool successSearchNumber = Int32.TryParse(testWordOfParagraph, out firstNumberOfParagraph);
                    if (successSearchNumber) chapterNameIsDigitsOnly[i] = firstNumberOfParagraph + 1; //потом проверить количество и что номера монотонно возрастают (+1 - чтобы избавиться от нулевой главы, бывают и такие, потом не забыть отнять)                      
                                                                                                      //если слово - цифра, тогда проверяем остальной текст на номера глав без ключевых слов
                    else keyWordChapterName = TestWordOfParagraphCompare(testWordOfParagraph, j, desiredTextLanguage);//проверяем первые несколько слово на совпадение с ключевыми                                                                                                   
                }
            }            
        }        

        void SymbolsFoundBeforeKeyWord(int j)//передать и получить все параметры
        {//найденное ключевое слово не первое, перед ним были какие-то символы - ищем их и складываем количество встретившихся групп в массив chapterSymbolsVersionsCount
         //потом сравнить с числом найденных глав и, если совпадает, то найден постоянная группа в названии главы - но зачем она?
            for (int m = 0; m < j; m++)//записываем группы символов до встреченного ключевого слова (текущее j)
            {
                if (foundWordsOfParagraph[m] == foundSymbolsOfParagraph[m]) chapterSymbolsVersionsCount[m]++;//если новое значение равно предыдущему, то увеличиваем счетчик
                foundSymbolsOfParagraph[m] = foundWordsOfParagraph[m];
            //подумать - если левая группа испортит переменную, что потом будет? по идее через пару правильных групп счет восстановится                                                                      
            //заносить все равно надо, потому что вдруг уже первая группа будет неправильная - потом она постепенно заменится правильной
            //но вообще не очень правильно - надо все же все значения занести в массив и потом анализировать - исключения выкинуть и найти такое же количество групп, как и глав
            //здесь надо занести в массив (из 10-ти элементов) номер слова названия главы - по порядку в строке, т.е. есть ли перед названием лишние спецсимволы и другое
            }
        }

        public int WordsOfParagraphSearch(string currentParagraph, string[] foundWordsOfParagraph)
        {//метод выделяет из строки (абзаца текста) первые десять (или больше - по размерности передаваемого массива) слов или чисел (и, возможно, перечисляет все разделители)
            if (String.IsNullOrWhiteSpace(currentParagraph))
            {
                return (int)MethodFindResult.NothingFound;//пустая строка без слов, вернули -1 для ясности
            }
            int findWordsCount = foundWordsOfParagraph.Length;
            Array.Clear(foundWordsOfParagraph, 0, findWordsCount);
            string wordOfParagraph = "";
            string symbolsOfParagraph = "";
            int flagWordStarted = 0;
            int flagSymbolsStarted = 0;
            int i = 0;
            //разделяем абзац на слова или числа и на скопления спецсимволов (если больше одного подряд)
            foreach (char charOfChapterNumber in currentParagraph)
            {               
                if (i < findWordsCount - 1)//если массив еще не заполнен, заполняем (-1 - на всякий случай, чтобы не переполниться)
                {
                    if (Char.IsLetterOrDigit(charOfChapterNumber))//слабое место, что может быть комбинация букв и цифр - протестировать этот вариант
                    {
                        if (flagSymbolsStarted > 1)
                        {//найдена цепочка спецсимволов больше одного подряд (она только что завершилась)
                            foundWordsOfParagraph[i] = symbolsOfParagraph;                            
                            symbolsOfParagraph = "";
                            i++;
                        }
                        flagSymbolsStarted = 0; //цепочка символов прервалась, сбрасываем счетчик                    
                        wordOfParagraph = wordOfParagraph + charOfChapterNumber;//нашли начало слова (после возможных спецсимволов в начале строки) и собираем слово, пока идут буквы (или цифры)
                        flagWordStarted++;
                    }
                    else
                    {//слово кончилось (или еще не началось)
                        if (flagWordStarted > 0)
                        {
                            if (charOfChapterNumber == ' ')
                            {//нашли пробел после него, прибавляем его к слову для совпадения с ключевыми словами - сомнительное действие, надо предусмотреть дефис и прочие варианты
                                foundWordsOfParagraph[i] = wordOfParagraph + charOfChapterNumber;                                
                                wordOfParagraph = "";
                                i++;
                            }                            
                        }
                        flagWordStarted = 0; //цепочка букв или цифр прервалась, сбрасываем счетчик                        
                        symbolsOfParagraph = symbolsOfParagraph + charOfChapterNumber;
                        flagSymbolsStarted++;
                    }
                }
                else
                {
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "else i = " + i.ToString(), CurrentClassName, 3);
                    return i;
                }
            }
            if (flagWordStarted > 0) foundWordsOfParagraph[i] = wordOfParagraph;
            if (flagSymbolsStarted > 1) foundWordsOfParagraph[i] = symbolsOfParagraph;
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "foreach i = " + i.ToString(), CurrentClassName, 3);
            return i;
        }
        
        int TestWordOfParagraphCompare(string testWordOfParagraph, int j, int desiredTextLanguage)//проверяем полученное слово на совпадение с ключевыми из массива (его хорошо бы прямо передавать и мерять длину - вдруг потом в другой класс переедем?)
        {
            for (int n = 0; n < chapterNamesSamplesCount; n++)
            {
                bool currentfirstWordOfParagraph = testWordOfParagraph.Contains(chapterNamesSamples[desiredTextLanguage, n]);//проверяем, содержатся ли стандартные называния глав в строке - с пробелом после слова (может заменить на Equals)
                if (currentfirstWordOfParagraph)
                {//нашли название главы, возвращаем индекс массива ключевых слов
                    chapterNamesVersionsCount[n]++; //сюда прибавляем количество встреченных ключевых слов, чтобы потом выбрать похожее по количеству с номерами (не менее количества номеров, больше можно)                                        
                    //нашли название главы, теперь как-то найти номер сразу за названием - похоже, номер ищется автоматически - в следующей группе символов за ключевым словом
                    if (j > 0) SymbolsFoundBeforeKeyWord(j); //найденное ключевое слово не первое в строке, перед ним были какие-то символы, ищем их и сохраняем, чтобы потом было проще искать названия глав
                    return n;//можно уже возвращать что-то другое (наверное)
                }
            }
            return (int)MethodFindResult.NothingFound;//не нашли...
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
        
        int ChapterKeyNamesAnalysis(int desiredTextLanguage)
        {
            int maxKeyNameLength = 0;
            int previousKeyNameLength = 0;

            for (int n = 0; n < chapterNamesSamplesCount; n++)
            {
                string currentKeyName = chapterNamesSamples[desiredTextLanguage, n];
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
            string userSelectedText = _book.GetSelectedText(desiredTextLanguage);//получили припрятанный фрагмент, выбранный пользователем - предположительно вид нумерации глав

            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "In the TextBox " + desiredTextLanguage.ToString() + strCRLF +
               "User Selected the following Text ==> " + userSelectedText, CurrentClassName, showMessagesLevel);

            int lengthUserSelectedText = userSelectedText.Length;//считаем, что длина больше нуля, иначе не попали бы сюда
            //сначала ищем полученный фрагмент в тексте, чтобы добыть и проверить предыдущие (и последующие) строки - т.е. абзацы
            int findChapterResult = 0; //FindParagraphTextNumber(userSelectedText, desiredTextLanguage, 0);//в результате получим номер элемента, в котором нашелся фрагмент
            if (findChapterResult > 0)//в то, что название главы в первой строке книги, мы не верим
            {
                string previousParagraphChapterName = _book.GetParagraphText(findChapterResult - 1, desiredTextLanguage);
                string paragraphWihtChapterName = _book.GetParagraphText(findChapterResult, desiredTextLanguage);
                //string nextParagraphChapterName = _book.GetParagraphText(findChapterResult + 1, desiredTextLanguage);

                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "findChapterResult = " + findChapterResult.ToString() + strCRLF +
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
                        int paragraphTextLength = _book.GetParagraphTextLength(desiredTextLanguage);
                        int startParagraphTextNumber = 0;
                        int chaptersMaxNumber = 0;
                        int AddChapterNumberLength = 0;
                        for (int i = 0; i < paragraphTextLength; i++)
                        {
                            int findChapterNameWithoutDigitsResult = 0;//FindParagraphTextNumber(chapterNameWithoutDigits, desiredTextLanguage, startParagraphTextNumber);//в результате получим номера элементов, в которых нашлась "Глава"
                            if (findChapterNameWithoutDigitsResult > 0)
                            {
                                //тут массив названий из book, заполняемый полными названиями глав (с номерами)                            
                                string foundChapterName = _book.GetParagraphText(findChapterNameWithoutDigitsResult, desiredTextLanguage);//достаем текст абзаца, номер которого получили от FindParagraphTextNumber - там имя главы
                                int AddChapterNameLength = _book.AddChapterName(foundChapterName, desiredTextLanguage);//полное имя главы в массив имен глав

                                int chapterNumber = Convert.ToInt32(FindDigitsInChapterName(foundChapterName));//получили строку с цифрами в названии главы
                                AddChapterNumberLength = _book.AddChapterNumber(chapterNumber, desiredTextLanguage);

                                if (findChapterNameWithoutDigitsResult < paragraphTextLength) startParagraphTextNumber = findChapterNameWithoutDigitsResult + 1;//на следующем круге начинаем искать после предыдущего нахождения
                                                                                                                                                                //после выхода взять массив названий и проанализировать его на возрастание номеров глав
                                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "i = " + i.ToString() + "and paragraphTextLength = " + paragraphTextLength.ToString() + strCRLF +
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


