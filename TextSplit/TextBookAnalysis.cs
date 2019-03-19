using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;

namespace TextSplit
{
    public interface ITextBookAnalysis
    {
        int AnalyseTextBook();        

        event EventHandler AnalyseInvokeTheMain;
    }

    class TextBookAnalysis : ITextBookAnalysis
    {
        private readonly IAllBookData _book;
        private readonly IMessageService _messageService;

        readonly private int filesQuantity;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;

        private string[,] chapterNamesSamples;
        private readonly char[] charsParagraphSeparator;
        private readonly char[] charsSentenceSeparator;

        public event EventHandler AnalyseInvokeTheMain;

        public TextBookAnalysis(IAllBookData book, IMessageService service)
        {
            _book = book;
            _messageService = service;

            filesQuantity = Declaration.FilesQuantity;
            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;

            charsParagraphSeparator = new char[] { '\r', '\n' };
            charsSentenceSeparator = new char[] { '.', '!', '?' };

            //проверить типовые названия глав (для разных языков свои)
            chapterNamesSamples = new string[,]
            { { "Chapter ", "Paragraph ", "Section ", "Subhead ", "Part " },
                { "Глава ", "Параграф " , "Раздел ", "Подраздел ", "Часть " }, };//а номера глав бывают буквами!
        }

        public int AnalyseTextBook() // типа Main в этом классе
        {   
            int desiredTextLanguage = GetDesiredTextLanguage();//возвращает номер языка, если на нем есть AnalyseText или AnalyseChapterName
            if (desiredTextLanguage == (int)MethodFindResult.NothingFound) return desiredTextLanguage;//типа, нечего анализировать
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "Start desiredTextLanguage = " + desiredTextLanguage.ToString(), CurrentClassName, showMessagesLevel);

            if (_book.GetFileToDo(desiredTextLanguage) == (int)WhatNeedDoWithFiles.AnalyseText)
            {//если первоначальный анализ текста, то делим текст на абзацы
                int portionBookTextResult = PortionBookTextOnParagraphs(desiredTextLanguage);
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "portionBookTextResult = " + portionBookTextResult.ToString(), CurrentClassName, showMessagesLevel);

                if (portionBookTextResult > 0)//найдено некоторое количество абзацев
                {




                    //в этот момент надо не лениться, а пойти и поискать тривиальные варианты названий глав, а только если не получилось, приставать к пользователю

                    //следующий блок - в отдельный метод
                    _book.SetFileToDo((int)WhatNeedDoWithFiles.SelectChapterName, desiredTextLanguage);//сообщаем Main, что надо поменять название на кнопке на Select
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                        "AnalyseInvokeTheMain with desiredTextLanguage = " + desiredTextLanguage.ToString(), CurrentClassName, showMessagesLevel);
                    if (AnalyseInvokeTheMain != null) AnalyseInvokeTheMain(this, EventArgs.Empty);//пошли узнавать у пользователя, как маркируются главы
                }
                return portionBookTextResult;
            }
            if (_book.GetFileToDo(desiredTextLanguage) == (int)WhatNeedDoWithFiles.AnalyseChapterName) //анализ названия главы с полученной подсказкой от пользователя
            {//если получена подсказка от пользователя - текст названия главы, то изучаем полученный выбранный текст
                int chapterCount = UserSelectedChapterNameAnalysis(desiredTextLanguage);
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "Found Chapters count = " + chapterCount.ToString(), CurrentClassName, 3);
                return chapterCount;
            }
            return desiredTextLanguage;//типа, нечего анализировать
        }

        private int SelectedChapterNameAnalysis(int desiredTextLanguage)//ищем все названия глав, складываем их по массивам, узнаем их количество и возвращаем его
        {
            string userSelectedText = _book.GetSelectedText(desiredTextLanguage);//получили припрятанный фрагмент, выбранный пользователем - предположительно вид нумерации глав

            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "In the TextBox " + desiredTextLanguage.ToString() + strCRLF +
               "User Selected the following Text ==> " + userSelectedText, CurrentClassName, showMessagesLevel);

            int lengthUserSelectedText = userSelectedText.Length;//считаем, что длина больше нуля, иначе не попали бы сюда
            //сначала ищем полученный фрагмент в тексте, чтобы добыть и проверить предыдущие (и последующие) строки - т.е. абзацы
            int findChapterResult = FindParagraphTextNumber(userSelectedText, desiredTextLanguage, 0);//в результате получим номер элемента, в котором нашелся фрагмент
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
                            int findChapterNameWithoutDigitsResult = FindParagraphTextNumber(chapterNameWithoutDigits, desiredTextLanguage, startParagraphTextNumber);//в результате получим номера элементов, в которых нашлась "Глава"
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

        private int UserSelectedChapterNameAnalysis(int desiredTextLanguage)//метод, когда не получился агализ глав со стандартными названиями, тогда просим подсказки у пользователя
        {
            string userSelectedText = _book.GetSelectedText(desiredTextLanguage);//получили припрятанный фрагмент, выбранный пользователем - предположительно вид нумерации глав
                
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "In the TextBox " + desiredTextLanguage.ToString() + strCRLF +
               "User Selected the following Text ==> " + userSelectedText, CurrentClassName, showMessagesLevel);

            int lengthUserSelectedText = userSelectedText.Length;//считаем, что длина больше нуля, иначе не попали бы сюда
            //сначала ищем полученный фрагмент в тексте, чтобы добыть и проверить предыдущие (и последующие) строки - т.е. абзацы
            int findChapterResult = FindParagraphTextNumber(userSelectedText, desiredTextLanguage, 0);//в результате получим номер элемента, в котором нашелся фрагмент
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
                            int findChapterNameWithoutDigitsResult = FindParagraphTextNumber(chapterNameWithoutDigits, desiredTextLanguage, startParagraphTextNumber);//в результате получим номера элементов, в которых нашлась "Глава"
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

        private int FindParagraphTextNumber(string userSelectedText, int desiredTextLanguage, int startParagraphTextNumber)
        {
            int paragraphTextLength = _book.GetParagraphTextLength(desiredTextLanguage);

            if (startParagraphTextNumber >= paragraphTextLength) return -1;

            if (startParagraphTextNumber < paragraphTextLength)
            {
                for (int i = startParagraphTextNumber; i < paragraphTextLength; i++)
                {
                    string currentParagraph = _book.GetParagraphText(i, desiredTextLanguage);
                    bool selectedTextFound = currentParagraph.Contains(userSelectedText);
                    if (selectedTextFound) return i;//возвращаем номер элемента, в котором нашелся фрагмент                
                }
            }            
            return - 1;
        }

        private string FindDigitsInChapterName(string paragraphWihtChapterName)
        {
            string digitsOfChapterNumber = "";
            char[] isStringChapterNumber = paragraphWihtChapterName.ToCharArray();//выделение цифр в строке - в отдельный метод
            foreach (char charOfChapterNumber in isStringChapterNumber)
            {
                bool isDigitAtChapterName = Char.IsDigit(charOfChapterNumber);
                if (isDigitAtChapterName) digitsOfChapterNumber = digitsOfChapterNumber + charOfChapterNumber.ToString();
            }
            return digitsOfChapterNumber;
        }        
        
        private int GetDesiredTextLanguage()
        {
            int desiredTextLanguage = (int)MethodFindResult.NothingFound;

            for (int i = 0; i < filesQuantity; i++)//пока остается цикл - все же один вместо двух, если вызывать специальный метод 2 раза
            {
                int iDesiredTextLanguage = _book.GetFileToDo(i);
                if (iDesiredTextLanguage == (int)WhatNeedDoWithFiles.AnalyseText) desiredTextLanguage = i;
                if (iDesiredTextLanguage == (int)WhatNeedDoWithFiles.AnalyseChapterName) desiredTextLanguage = i; 
            }
            return desiredTextLanguage;
        }

        private int PortionBookTextOnParagraphs(int desiredTextLanguage)//делит текст на абзацы по EOL, сохраняет в List в AllBookData
        {
            string textToAnalyse = _book.GetFileContent(desiredTextLanguage);
            //можно заранее определить размерность массива, если отказаться от динамических - 
            //int cnt = 0;
            //foreach (char c in test) { if (c == '&') cnt++; }

            string[] TextOnParagraphsPortioned = textToAnalyse.Split(charsParagraphSeparator);//portioned all book content in the ParagraphsArray via EOL            
                                                                                              //потом тут можно написать свой метод деления на абзацы (но это не точно)
            int textOnParagraphsPortionedLength = TextOnParagraphsPortioned.Length;//узнали количество абзацев после Split

            int addParagraphTextCount = 0;
            for (int i = 0; i < textOnParagraphsPortionedLength; i++)//загружаем получившиеся абзацы в динамический массив, потом сравниваем длину массивов
            {
                addParagraphTextCount = _book.AddParagraphText(TextOnParagraphsPortioned[i], desiredTextLanguage);//также возвращает количество уже существующих элементов

                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                    "TextOnParagraphsPortioned [i], where i = " + i.ToString() + strCRLF +
                    "addParagraphTextCount = " + addParagraphTextCount.ToString() + strCRLF +
                    "GetParagraphText ==> " + _book.GetParagraphText(i, desiredTextLanguage), CurrentClassName, showMessagesLevel); 
            }

            if (addParagraphTextCount == textOnParagraphsPortionedLength)
            {

                return textOnParagraphsPortionedLength;
            }
            else
            {//длина массивов не совпала, показываем диагностику, потом добавить еще постонное сообщение (не Trace) на тему
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                    "Paragraphs count after Split" + strCRLF + 
                    "and" + strCRLF + 
                    "Last count in List paragraphsTexts" + strCRLF + 
                    "are not the same " + strCRLF + 
                    strCRLF +
                    "Paragraphs count after Split = " + textOnParagraphsPortionedLength.ToString() + strCRLF +                
                    "last count in List paragraphsTexts = " + addParagraphTextCount.ToString(), CurrentClassName, 3);
                return textOnParagraphsPortionedLength;
            }            
        }

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

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }

    }
}
//    if (isPreviousLineBlank)//and if previous line was blank
//    {//we found the chapter begining
//        char[] isStringChapterNumber = currentParagraph.ToCharArray(0, 1);
//        bool isDigitAtSentence = Char.IsDigit(isStringChapterNumber[0]);//check is it start from digit - добавить распознавание ключевых слов от пользователя (глава т.д.)

//        if (isDigitAtSentence)//if yes - the chapter begining found
//        {//need to define chapter number here
//            id_Chapter++;//quantity of chapters and reset -1 to 0 on the first pass of sucessful chapter search
//            //from obsolete - if (id_Chapter < 0) id_Chapter = 0;//reset -1 to 0 on the first pass of sucessful chapter search
//            int chapter = isStringChapterNumber.Length;// - поставить какую-то осмысленную цифру - количество абзацев и/или предложений
//            string chapter_name = new string(isStringChapterNumber);//instead of - string Chapter_name = isStringChapterNumber.ToString(); - поставить присвоение названия главы
//            Array.Clear(dataBaseTableToDo, 0, dataBaseTableToDo.Length);
//            dataBaseTableToDo[(int)TablesNamesNumbers.Chapters] = (int)WhatNeedDoWithTables.InsertRecord;

//            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
//                strCRLF + "Chapter =  " + chapter.ToString() +
//                strCRLF + "Chapter_name - " + chapter_name, CurrentClassName, showMessagesLevel);

//            int insertRecordResult = _data.InsertRecordInTable(dataBaseTableNames, dataBaseTableToDo, id_Chapter, ID_Language, chapter, chapter_name);//Insert Record in Table Chapters
//            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " insertResult after Chapters ==> ", insertRecordResult.ToString(), CurrentClassName, showMessagesLevel);
//        }
//    }