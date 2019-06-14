using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;

namespace TextSplit
{
    public interface IAnalysisLogicCultivation
    {
        int GetDesiredTextLanguage();
        bool FindTextPartMarker(string currentParagraph, string stringMarkBegin);
        int FindTextPartNumber(string currentParagraph, string stringMarkBegin, int totalDigitsQuantity);
        string CreatePartTextMarks(string stringMarkBegin, string stringMarkEnd, int currentUpperNumber, int enumerateCurrentCount, string sentenceTextMarksWithOtherNumbers);
        string AddSome00ToIntNumber(string currentNumberToFind, int totalDigitsQuantity);
        //event EventHandler AnalyseInvokeTheMain;
    }

    public class AnalysisLogicCultivation : IAnalysisLogicCultivation
    {
        private readonly IAllBookData _bookData;
        private readonly IMessageService _msgService;
        private readonly IAnalysisLogicDataArrays _arrayAnalysis;
        readonly private int filesQuantity;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;

        int GetConstantWhatNotLength(string WhatNot) => _arrayAnalysis.GetConstantWhatNotLength(WhatNot);
        string[] GetConstantWhatNot(string WhatNot) => _arrayAnalysis.GetConstantWhatNot(WhatNot);

        public AnalysisLogicCultivation(IAllBookData bookData, IMessageService msgService, IAnalysisLogicDataArrays arrayAnalysis)
        {
            _bookData = bookData;
            _msgService = msgService;
            _arrayAnalysis = arrayAnalysis;            

            filesQuantity = DeclarationConstants.FilesQuantity;
            showMessagesLevel = DeclarationConstants.ShowMessagesLevel;
            strCRLF = DeclarationConstants.StrCRLF;            
        }
        
        public int GetDesiredTextLanguage()
        {
            int desiredTextLanguage = (int)MethodFindResult.NothingFound;

            for (int i = 0; i < filesQuantity; i++)//пока остается цикл - все же один вместо двух, если вызывать специальный метод 2 раза
            {
                int iDesiredTextLanguage = _bookData.GetFileToDo(i);
                if (iDesiredTextLanguage == (int)WhatNeedDoWithFiles.AnalyseText) desiredTextLanguage = i;
                if (iDesiredTextLanguage == (int)WhatNeedDoWithFiles.AnalyseChapterName) desiredTextLanguage = i;
            }
            return desiredTextLanguage;
        }

        public bool FindTextPartMarker(string currentParagraph, string stringMarkBegin)//если найден маркер главы, сбрасываем счетчик абзацев на 1 (кстати, почему не на 0?)
        {
            string symbolsMarkBegin = GetConstantWhatNot(stringMarkBegin)[0];//расписать подробнее, что используем только нулевую ячейку массива            
            bool foundPartMark = currentParagraph.StartsWith(symbolsMarkBegin);//тут можно искать маркировку не во всем абзаце, а только в заданном месте, где она должна быть - пока вариант, ищем в начале            
            return foundPartMark;
        }

        public int FindTextPartNumber(string currentParagraph, string stringMarkBegin, int totalDigitsQuantity)
        {
            //найти и выделить номер главы
            int currentPartNumber = -1;//чтобы не спутать с нулевым индексом на выходе, -1 - ничего нет (совсем ничего)
            string symbolsMarkBegin = GetConstantWhatNot(stringMarkBegin)[0];//расписать подробнее, что используем только нулевую ячейку массива
            int symbolsMarkBeginLength = symbolsMarkBegin.Length;
            bool partNumberFound = Int32.TryParse(currentParagraph.Substring(symbolsMarkBeginLength, totalDigitsQuantity), out currentPartNumber);//вместо 3 взять totalDigitsQuantity для главы
            if (partNumberFound)
            {
                return currentPartNumber;
            }
            else
            {
                //что-то пошло не так, остановиться - System.Diagnostics.Debug.Assert(partNumberFound, "Stop here - partNumberFound did not find!");
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "STOP HERE - currentPartNumber did not find in currentParagraph!" + strCRLF +
                    "currentParagraph = " + currentParagraph + " -->" + strCRLF +
                    "stringMarkBegin --> " + stringMarkBegin + strCRLF +                                
                    "totalDigitsQuantity = " + totalDigitsQuantity.ToString(), CurrentClassName, 3);                
                return (int)MethodFindResult.NothingFound;
            }            
        }

        //надо убрать метод в общий класс AnalysisLogicCultivation и сделать его общим с нумерацией глав/абзаца - этот пока для предложений
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





        public string AddSome00ToIntNumber(string currentNumberToFind, int totalDigitsQuantity)
        {
            //string currentChapterNumberToFind000 = "";
            int currentChapterNumberLength = currentNumberToFind.Length;
            int add00Digits = totalDigitsQuantity - currentChapterNumberLength;
            if(add00Digits <= 0)
            {
                return null;
            }
            for(int i = 0; i < add00Digits; i++)
            {
                currentNumberToFind = "0" + currentNumberToFind;
            }
            //switch (currentChapterNumberLength)
            //{
            //    case 1:
            //        return currentChapterNumberToFind000 = "00" + currentNumberToFind;
            //    case 2:
            //        return currentChapterNumberToFind000 = "0" + currentNumberToFind;
            //}
            return currentNumberToFind;
        }

        public string SaveTextToFile(int desiredTextLanguage)
        {
            string hashFile = "";

            return hashFile;
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}
//
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
