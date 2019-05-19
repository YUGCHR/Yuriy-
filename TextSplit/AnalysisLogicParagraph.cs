using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;


namespace TextSplit
{
    public interface IAnalysisLogicParagraph
    {
        int markAndEnumerateParagraphs(string lastFoundChapterNumberInMarkFormat, int desiredTextLanguage);//plogic        
        int PortionBookTextOnParagraphs(int desiredTextLanguage);//plogic
        int normalizeEmptyParagraphs(int desiredTextLanguage);//plogic
        
        //event EventHandler AnalyseInvokeTheMain;
    }

    public class AnalysisLogicParagraph : IAnalysisLogicParagraph
    {
        private readonly IAllBookData _bookData;
        private readonly IMessageService _msgService;
        private readonly IAnalysisLogicCultivation _analysisLogic;
        private readonly IAnalysisLogicDataArrays _arrayAnalysis;

        readonly private int filesQuantity;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;
        
        private readonly int charsParagraphSeparatorLength;

        int GetParagraphTextLength(int desiredTextLanguage) => _bookData.GetParagraphTextLength(desiredTextLanguage);
        string GetParagraphText(int paragraphCount, int desiredTextLanguage) => _bookData.GetParagraphText(paragraphCount, desiredTextLanguage);
        int SetParagraphText(string paragraphText, int paragraphCount, int desiredTextLanguage) => _bookData.SetParagraphText(paragraphText, paragraphCount, desiredTextLanguage);

        int GetCharsParagraphSeparatorLength() => _arrayAnalysis.GetCharsParagraphSeparatorLength();
        char GetCharsParagraphSeparator(int index) => _arrayAnalysis.GetCharsParagraphSeparator(index);
        string GetStringMarksChapterName(string BeginOrEnd) => _arrayAnalysis.GetStringMarksChapterName(BeginOrEnd);
        string GetStringMarksParagraphName(string BeginOrEnd) => _arrayAnalysis.GetStringMarksParagraphName(BeginOrEnd);

        string AddSome00ToIntNumber(string currentChapterNumberToFind, int totalDigitsQuantity) => _analysisLogic.AddSome00ToIntNumber(currentChapterNumberToFind, totalDigitsQuantity);

        //public event EventHandler AnalyseInvokeTheMain;

        public AnalysisLogicParagraph(IAllBookData bookData, IMessageService msgService, IAnalysisLogicCultivation analysisLogic, IAnalysisLogicDataArrays arrayAnalysis)
        {
            _bookData = bookData;
            _msgService = msgService;
            _analysisLogic = analysisLogic;//общая логика
            _arrayAnalysis = arrayAnalysis;

            filesQuantity = DeclarationConstants.FilesQuantity;
            showMessagesLevel = DeclarationConstants.ShowMessagesLevel;
            strCRLF = DeclarationConstants.StrCRLF;

            charsParagraphSeparatorLength = GetCharsParagraphSeparatorLength();            
        }        

        public int markAndEnumerateParagraphs (string lastFoundChapterNumberInMarkFormat, int desiredTextLanguage)
        {
            int enumerateParagraphsInChapterCount = 1;
            int enumerateParagraphsTotalCount = 1;
            int currentChapterNumber =-1;
            int currentChapterNumberCount = 0;
            string paragraphTextMarks = "";
            string currentParagraphNumberSrting = "";
            int totalDigitsQuantity = 5; //для номера главы используем 5 цифр (до 999, должно хватить) - перенести в AnalysisLogicDataArrays
            int paragraphTextLength = GetParagraphTextLength(desiredTextLanguage);
            for (int i = 0; i < paragraphTextLength; i++)//перебираем все абзацы текста
            {
                string currentParagraph = GetParagraphText(i, desiredTextLanguage);
                //найти маркер клавы, выделить номер главы, сравнить со счетчиком, прибавить счетчик
                string chapterMarkBegin = GetStringMarksChapterName("Begin");
                int chapterMarkBeginLength = chapterMarkBegin.Length;
                bool foundChapterMark = currentParagraph.Contains(chapterMarkBegin);
                if (foundChapterMark)
                {
                    bool chapterNumberFound = Int32.TryParse(currentParagraph.Substring(chapterMarkBeginLength, 3), out currentChapterNumber);//вместо 3 взять totalDigitsQuantity для главы
                    if (chapterNumberFound)
                    {
                        currentChapterNumberCount++;
                        enumerateParagraphsInChapterCount = 1;//новая глава, нумерация абзацев с 1
                        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Current Paragraph [" + i.ToString() + "] --> " + currentParagraph + strCRLF +
                            "currentChapterNumber = " + currentChapterNumber.ToString() + strCRLF +
                            "currentChapterNumberCount = " + currentChapterNumberCount.ToString(), CurrentClassName, showMessagesLevel);
                    }
                    else
                    {
                        //что-то пошло не так, остановиться
                        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), lastFoundChapterNumberInMarkFormat + " - Stop here - markAndEnumerateParagraphs cannon do enumerate!", CurrentClassName, 3);
                        return (int)MethodFindResult.NothingFound;
                    }
                }

                if(currentChapterNumber < 0)
                {
                    paragraphTextMarks = GetStringMarksParagraphName("Begin") + "Introduction" + GetStringMarksParagraphName("End") + "-" + "Paragraph" + "-";//создаем маркировку введения/предисловия
                }
                else
                {
                    currentParagraphNumberSrting = enumerateParagraphsInChapterCount.ToString();
                    string currentParagraphNumberToFind00 =  AddSome00ToIntNumber(currentParagraphNumberSrting, totalDigitsQuantity);
                    paragraphTextMarks = GetStringMarksParagraphName("Begin") + currentParagraphNumberToFind00 + GetStringMarksParagraphName("End") + "-Paragraph-of-Chapter-" + currentChapterNumber.ToString();
                }

                //сформирована маркировка абзаца, можно искать начало абзацев (пустые строки) и заносить (пустые строки перед главой уже заняты)
                bool currentParagraphEmptyResult = string.IsNullOrEmpty(currentParagraph);
                if (currentParagraphEmptyResult)
                {
                    SetParagraphText(paragraphTextMarks, i, desiredTextLanguage);
                    enumerateParagraphsInChapterCount++;
                    enumerateParagraphsTotalCount++;//тут будет общее количество абзацев в книге (можно добавить последние несколько строчек и занести общее количество глав и абзацев)
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Paragraph must be EMPTY [" + i.ToString() + "] --> " + currentParagraph + strCRLF +
                            "paragraphTextMarks --> " + paragraphTextMarks + strCRLF +
                            "enumerateParagraphsCount++ = " + enumerateParagraphsInChapterCount.ToString(), CurrentClassName, showMessagesLevel);
                }

                //все, что до первого номера главы, маркировать предысловием (можно было бы нулевой главой, но нет)
                //получив номер главы, в каждую пустую строку вставляем номер абзаца - §§§§§Paragraph-0000-§§§(-of-000) - где 000 берутся из номера главы ¤¤¤¤¤Chapter-000-¤¤¤
            }
            //

            return enumerateParagraphsInChapterCount;
        }

        public int PortionBookTextOnParagraphs(int desiredTextLanguage)//делит текст на абзацы по EOL, сохраняет в List в AllBookData
        {
            char[] charsParagraphSeparator = new char[charsParagraphSeparatorLength];
            for (int i = 0; i < charsParagraphSeparatorLength; i++)
            {
                charsParagraphSeparator[i] = GetCharsParagraphSeparator(i);
            }
            string textToAnalyse = _bookData.GetFileContent(desiredTextLanguage);
            string[] TextOnParagraphsPortioned = textToAnalyse.Split(charsParagraphSeparator);//portioned all book content in the ParagraphsArray via EOL
            //потом тут можно написать свой метод деления на абзацы (или этот пусть делит по одному сепаратору)
            int textOnParagraphsPortionedLength = TextOnParagraphsPortioned.Length;
            int addParagraphTextCount = 0;
            
            for (int i = 0; i < textOnParagraphsPortionedLength; i++)//загружаем получившиеся абзацы в динамический массив, потом сравниваем длину массивов
            {
                addParagraphTextCount = _bookData.AddParagraphText(TextOnParagraphsPortioned[i], desiredTextLanguage);//также возвращает количество уже существующих элементов                

                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Current Paragraph --> " + TextOnParagraphsPortioned[i] + strCRLF +
                    "with i = " + i.ToString() + strCRLF +
                    "addParagraphTextCount = " + addParagraphTextCount.ToString(), CurrentClassName, showMessagesLevel);
            }

            if (addParagraphTextCount == textOnParagraphsPortionedLength)
            {
                return textOnParagraphsPortionedLength;
            }
            else
            {//длина массивов не совпала, показываем диагностику, потом добавить еще постоянное сообщение (не Trace) на тему
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
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

        public int normalizeEmptyParagraphs(int desiredTextLanguage)//удаляет идущие подряд пустые строки (абзацы) оставляя только одну
        {//нормируем пустые строки - идущие 2 подряд заменяем одной, пока не останется только по одной пустой строке

            int paragraphTextLength = _bookData.GetParagraphTextLength(desiredTextLanguage);
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                "paragraphTextLength = " + paragraphTextLength.ToString() + strCRLF, CurrentClassName, 3);

            bool normalizeEmptyParagraphsFlag = false;//флаг пустой текущей строки, true - если пустая
            int emplyParagraphCount = 0;
            int normalizeEmptyParagraphsResult = 0;            

            for (int n = 0; n < paragraphTextLength; n++)//запускаем раз за разом нормализацию (удаление двух подряд пустых строк)
            {
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                        "Cycle Normalize with step n = " + n.ToString() + strCRLF +
                        "счетчик удаленных строк до сброса = " + normalizeEmptyParagraphsResult.ToString(), CurrentClassName, showMessagesLevel);
                normalizeEmptyParagraphsResult = 0;

                paragraphTextLength = _bookData.GetParagraphTextLength(desiredTextLanguage); //узнали исходное количество абзацев для старта цикла

                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                    "normalizeEmptyParagraphsFlag is " + normalizeEmptyParagraphsFlag.ToString() + strCRLF, CurrentClassName, showMessagesLevel);
                normalizeEmptyParagraphsFlag = false;//сбрасываем флаг перед циклом

                for (int i = paragraphTextLength - 1; i > 0; i--)//выбираем по очереди номера пустых строк, начиная с последней(-1, то шо нумерация массива с нуля - мелочь, а засада)
                {
                    string paragraphText = _bookData.GetParagraphText(i, desiredTextLanguage);//получили номер строки                    
                    if (String.IsNullOrWhiteSpace(paragraphText))//если текущая строка пустая, смотрим предыдущую строку
                    {
                        emplyParagraphCount++;//счетчик исходных пустых строк
                        if (normalizeEmptyParagraphsFlag)//если предыдущая строка была тоже пустой, удаляем текущую и сбрасываем флаг
                        {
                            int addParagraphTextCount = _bookData.RemoveAtParagraphText(i, desiredTextLanguage);//удаляет элемент списка с индексом i
                            normalizeEmptyParagraphsFlag = false;
                            normalizeEmptyParagraphsResult++;//счетчик удаленых пустых строк
                        }
                        else normalizeEmptyParagraphsFlag = true;//если текущая строка пустая, а предыдущая не пустая - ставим флаг
                    }
                    else normalizeEmptyParagraphsFlag = false;//если текущая строка не пустая - сбрасываем флаг
                }
                paragraphTextLength = _bookData.GetParagraphTextLength(desiredTextLanguage);
                _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                            "было удалено пустых строк = " + normalizeEmptyParagraphsResult.ToString() + strCRLF +
                            "всего было найдено пустых строк = " + emplyParagraphCount.ToString() + strCRLF +
                            "длина текстового массива стала = " + paragraphTextLength.ToString() + strCRLF, CurrentClassName, showMessagesLevel);
                if (normalizeEmptyParagraphsResult == 0) break;
            }
            return normalizeEmptyParagraphsResult;
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}
//public int FindParagraphTextNumber(string userSelectedText, int desiredTextLanguage, int startParagraphTextNumber)
//{
//    int paragraphTextLength = _bookData.GetParagraphTextLength(desiredTextLanguage);

//    if (startParagraphTextNumber >= paragraphTextLength) return -1;

//    if (startParagraphTextNumber < paragraphTextLength)
//    {
//        for (int i = startParagraphTextNumber; i < paragraphTextLength; i++)
//        {
//            string currentParagraph = _bookData.GetParagraphText(i, desiredTextLanguage);
//            bool selectedTextFound = currentParagraph.Contains(userSelectedText);
//            if (selectedTextFound) return i;//возвращаем номер элемента, в котором нашелся фрагмент                
//        }
//    }
//    return -1;
//}
