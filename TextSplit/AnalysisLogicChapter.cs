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

    class AnalysisLogicChapter : IAnalysisLogicChapter
    {
        private readonly IAllBookData _book;
        private readonly IMessageService _messageService;

        readonly private int filesQuantity;
        readonly private int textFieldsQuantity;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;        

        private string[,] chapterNamesSamples;
        private readonly char[] charsParagraphSeparator;
        private readonly char[] charsSentenceSeparator;
        private readonly int chapterNamesSamplesCount;
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

            //maxKeyNameLength = new int[textFieldsQuantity];
            charsParagraphSeparator = new char[] { '\r', '\n' };
            charsSentenceSeparator = new char[] { '.', '!', '?' };

            //проверить типовые названия глав (для разных языков свои) - сделать метод универсальным и для частей тоже?
            chapterNamesSamples = new string[,]
            { { "Chapter ", "Paragraph ", "Section ", "Subhead ", "Part " },
                { "Глава ", "Параграф " , "Раздел ", "Подраздел ", "Часть " }, };//а номера глав бывают буквами!
            chapterNamesSamplesCount = chapterNamesSamples.GetLength(1); //при добавлении значений проверить присвоение длины!            
        }

        public int ChapterNameAnalysis(int desiredTextLanguage)//ищем все названия глав, складываем их по массивам, узнаем их количество и возвращаем его
        {
            string firstWordOfParagraph = "";
            int firstNumberOfParagraph = 0;
            int keyWordChapterName = 0;            
            //bool normalizeEmptyParagraphsFlag = false;//флаг пустой текущей строки, true - если пустая
            
            int maxKeyNameLength = ChapterKeyNamesAnalysis(desiredTextLanguage);//непонятно, зачем это нужно, но пока пусть будет
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "maxKeyNameLength = " + maxKeyNameLength.ToString(), CurrentClassName, 3);

            int paragraphTextLength = _book.GetParagraphTextLength(desiredTextLanguage);
            
            for (int i = 1; i < paragraphTextLength; i++)//перебираем все абзацы текста, начиная с второго - в первом главы быть не должно
            {
                string currentParagraph = _book.GetParagraphText(i, desiredTextLanguage);

                if (!String.IsNullOrWhiteSpace(currentParagraph))//если текущая строка не пустая, получаем и смотрим предыдущий абзац
                {
                    string previousParagraph = _book.GetParagraphText((i - 1), desiredTextLanguage);

                    if (String.IsNullOrWhiteSpace(previousParagraph))
                    {//начало анализа строки (абзаца)
                        firstWordOfParagraph = FirstWordOfParagraphSearch(currentParagraph);//находим первое слово или цифры в текущем параграфе
                        bool successSearchNumber = Int32.TryParse(firstWordOfParagraph, out firstNumberOfParagraph);
                        if (successSearchNumber)
                        {//первое слово - цифра, тогда проверяем остальной текст на номера глав без ключевых слов - искать просто по возрастанию - ждать следующего попадания сюда
                            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                                "firstWordOfParagraph - " + firstWordOfParagraph + strCRLF +
                                "successSearchNumber - " + successSearchNumber.ToString() + strCRLF +
                                "firstNumberOfParagraph = " + firstNumberOfParagraph.ToString() + strCRLF, CurrentClassName, 3);
                        }
                        else
                        {
                            keyWordChapterName = FirstWordOfParagraphCompare(firstWordOfParagraph, desiredTextLanguage);//проверяем первое слово на совпадение с ключевыми
                            if (keyWordChapterName >= 0)
                            {
                                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                                    "Paragraph Number = " + i.ToString() + strCRLF +
                                    "ChapterName found - " + strCRLF + firstWordOfParagraph, CurrentClassName, 3);
                                //нашли название главы, теперь как-то найти номер сразу за названием
                            }
                        }
                    }
                }
                //firstWordOfParagraph = "";
                //firstNumberOfParagraph = "";
                //flagWordStarted = 0;
                //flagNumberStarted = 0;
            }
            return 0;
        }

        string FirstWordOfParagraphSearch(string currentParagraph)//поиск первого слова или цифры в строке - с пробелом после него - разделить метод на выделение первого (или любого другого) слова абзаца и его анализ
        {
            string firstWordOfParagraph = "";            
            int flagWordStarted = 0;
            
            foreach (char charOfChapterNumber in currentParagraph) //добываем первое слово или цифру из абзаца - до любого спецсимвола, игнорируя все спецсимволы до начала слова
            {
                if (Char.IsLetterOrDigit(charOfChapterNumber))//слабое место, что может быть комбинация букв и цифр - протестировать этот вариант
                {
                    firstWordOfParagraph = firstWordOfParagraph + charOfChapterNumber;//нашли начало слова (после возможных спецсимволов в начале строки) и собираем слово, пока идут буквы (или цифры)
                    flagWordStarted++;
                }
                else
                {//слово кончилось (или еще не началось)
                    if (flagWordStarted > 0)
                    {//слово точно кончилось
                        if (charOfChapterNumber == ' ')
                        {//нашли пробел после него, прибавляем его к слову для совпадения с ключевыми словами
                            return firstWordOfParagraph + charOfChapterNumber;
                        }
                        else
                        {//после слова непонятно что за символ - что делать, тоже непонятно, возвращаем пока этот символ
                            return charOfChapterNumber.ToString();
                        }
                    }
                }
            }
            return null;//вообще не нашли букв и цифр в абзаце - и так, наверное, бывает
        }
        
        int FirstWordOfParagraphCompare(string firstWordOfParagraph, int desiredTextLanguage)//проверяем первое слово на совпадение с ключевыми
        {
            for (int n = 0; n < chapterNamesSamplesCount; n++)
            {
                //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "n = " + n.ToString() + strCRLF +
                //    "firstWordOfParagraph = " + firstWordOfParagraph.ToString() + strCRLF +
                //    "chapterNamesSamplesCount = " + chapterNamesSamplesCount.ToString() + strCRLF +
                //    chapterNamesSamples[desiredTextLanguage, n], CurrentClassName, 3);

                bool currentfirstWordOfParagraph = firstWordOfParagraph.Contains(chapterNamesSamples[desiredTextLanguage, n]);//проверяем, содержатся ли стандартные называния глав в строке - с пробелом после слова
                if (currentfirstWordOfParagraph)
                {
                    //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "n = " + n.ToString() + strCRLF +
                    //    "firstWordOfParagraph - " + firstWordOfParagraph + strCRLF +
                    //    "ChapterName found - " + strCRLF + firstWordOfParagraph, CurrentClassName, 3);
                    //нашли название главы, возвращаем индекс массива ключевых слов
                    return n;
                }
            }
            return (int)MethodFindResult.NothingFound;
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
        {
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


