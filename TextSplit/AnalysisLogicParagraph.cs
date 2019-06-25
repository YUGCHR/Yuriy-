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
        int MarkAndEnumerateParagraphs(int desiredTextLanguage, string lastFoundChapterNumberInMarkFormat);//plogic        
        int PortionBookTextOnParagraphs(int desiredTextLanguage);//plogic        
    }

    public class AnalysisLogicParagraph : IAnalysisLogicParagraph
    {
        private readonly IAllBookData _bookData;
        private readonly IMessageService _msgService;
        private readonly IAnalysisLogicCultivation _analysisLogic;


        public AnalysisLogicParagraph(IAllBookData bookData, IMessageService msgService, IAnalysisLogicCultivation analysisLogic)
        {
            _bookData = bookData;
            _msgService = msgService;
            _analysisLogic = analysisLogic;//общая логика            
        }        

        public int MarkAndEnumerateParagraphs (int desiredTextLanguage, string lastFoundChapterNumberInMarkFormat)//ChapterNumberParagraphsIndexes - вычесть 1
        {
            //получив номер главы, в каждую пустую строку вставляем номер абзаца - §§§§§Paragraph-0000-§§§(-of-000) - где 000 берутся из номера главы ¤¤¤¤¤Chapter-000-¤¤¤
            int paragraphTextLength = _bookData.GetIntContent(desiredTextLanguage, "GetParagraphTextLength"); //вместо GetParagraphTextLength(desiredTextLanguage);
            //int ChapterNumberParagraphsIndexesLength = ChapterNumberParagraphsIndexes.Length;

            int totalParagraphsNumbersCount = 0;//всего пронумеровано абзацев - без названий глав




            //тут достанем номера глав и ключевое слово главы из служебного массива, а пример нумерации главы получили в параметрах - из примера сделаем заготовку нумерации абзаца




            //работаем с нулевой главой ¤¤¤¤¤000¤¤¤-Chapter-            
            string currentChapterNameParagraph = _bookData.GetStringContent(desiredTextLanguage, "GetParagraphText", 0);//достать название нулевой главы

            //сформировать метку главы для номера абзаца, выглядит так - of-Chapter-0 (§§§§§00002§§§-Paragraph-of-Chapter-0), номер главы выглядит так - ¤¤¤¤¤001¤¤¤-Chapter-

            //найдем ключевое слово главы
            int startSymbolOfChapterKeyWord = DConst.beginChapterMark.Length + DConst.chapterNumberTotalDigits + DConst.endChapterMark.Length;// ¤¤¤¤¤001¤¤¤ (5 + 3 + 3), останется -Chapter- до перевода строки            
            int carriageReturnIndex = currentChapterNameParagraph.IndexOf(DConst.StrCRLF);//найти номер символа перевода строки (на практике, в современном контексте записи в текстовый файл, вы всегда должны использовать \n )

            if (startSymbolOfChapterKeyWord > carriageReturnIndex)//если ключевого слова нет, может равняться
            {
                //остановить работу - что-то неправильно
            }
            int chapterKeyWordLength = carriageReturnIndex - startSymbolOfChapterKeyWord;

            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "currentChapterNameParagraph[" + 0.ToString() + "] = " + DConst.StrCRLF + currentChapterNameParagraph + DConst.StrCRLF +
                    "startSymbolOfChapterKeyWord = " + startSymbolOfChapterKeyWord.ToString() + DConst.StrCRLF +
                    "carriageReturnIndex = " + carriageReturnIndex.ToString() + DConst.StrCRLF +
                    "chapterKeyWordLength = " + chapterKeyWordLength.ToString(), CurrentClassName, DConst.ShowMessagesLevel);

            string chapterKeyWord = currentChapterNameParagraph.Substring(startSymbolOfChapterKeyWord, chapterKeyWordLength);//нашли ключевое слово главы (здесь -Chapter-)

            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "currentChapterNameParagraph[" + 0.ToString() + "] = " + DConst.StrCRLF + currentChapterNameParagraph + DConst.StrCRLF +
                        "startSymbolOfChapterKeyWord = " + startSymbolOfChapterKeyWord.ToString() + DConst.StrCRLF +
                        "carriageReturnIndex = " + carriageReturnIndex.ToString() + DConst.StrCRLF +
                        "chapterKeyWordLength = " + chapterKeyWordLength.ToString() + DConst.StrCRLF +
                        "chapterKeyWord - " + chapterKeyWord, CurrentClassName, DConst.ShowMessagesLevel);

            int startparagraphNumberIndex = 1;//на первом проходе зацепим нулевую главу, но без самой строки назыания главы, поэтому 1, а не 0 (в нулевую главу войдут заглавие, аннотацию, предисловие, пролог и что там еще бывает)            

            for (int chapterNumberIndex = 0; chapterNumberIndex <= ChapterNumberParagraphsIndexesLength; chapterNumberIndex++)//перебираем все индексы абзацев с номерами глав
            {
                int currentParagraphTextIndex = 0; //счетчик нумерации абзацев внутри главы (и он же сброс счетчика на следующем проходе)
                int endparagraphNumberIndex = paragraphTextLength;
                bool isLastChapterNumber = chapterNumberIndex == ChapterNumberParagraphsIndexesLength;

                if(!isLastChapterNumber)
                {
                    endparagraphNumberIndex = ChapterNumberParagraphsIndexes[chapterNumberIndex];//выбрать индекс первой главы для старта нумерации абзацев до первой главы (значения нулевого индекса массива индексов номеров глав) - типа абзацы нулевой главы
                }                

                for (int paragraphNumberIndex = startparagraphNumberIndex; paragraphNumberIndex < endparagraphNumberIndex; paragraphNumberIndex++)//перебираем все абзацы текста
                {
                    string textParagraph = _bookData.GetStringContent(desiredTextLanguage, "GetParagraphText", paragraphNumberIndex); //и следующий абзац с текстом - как проверить? только, что не пустой?
                    bool textParagraphIsEmpty = String.IsNullOrEmpty(textParagraph);
                    if (!textParagraphIsEmpty)
                    {
                        currentParagraphTextIndex++;//начинаем нумерацию с 1 (не забыть потом сбросить счетчик)
                        totalParagraphsNumbersCount++;

                        //формировать номера абзацев, помня про нулевую главу
                        string currentParagraphNumberToFind000 = _analysisLogic.AddSome00ToIntNumber(currentParagraphTextIndex.ToString(), DConst.paragraptNumberTotalDigits);
                        if (currentParagraphNumberToFind000 == null)
                        {
                            //return null;//лучше поставить Assert - и можно прямо в AddSome00ToIntNumber?
                        }
                        //писать маркировку с номерами прямо в абзац c текстом - добавить строку перед названием
                        string currentChapterNumberToFind000 = _analysisLogic.AddSome00ToIntNumber(chapterNumberIndex.ToString(), DConst.chapterNumberTotalDigits);//создать номер главы из индекса сохраненных индексов - как раз начинаются с нуля
                        string paragraphTextMarks = DConst.beginParagraphMark + currentParagraphNumberToFind000 + DConst.endParagraphMark + "-" + DConst.paragraphMarkNameLanguage[desiredTextLanguage] + "-of" + chapterKeyWord + currentChapterNumberToFind000;//¤¤¤¤¤001¤¤¤-Chapter- добавить номер главы от нулевой
                       
                        int emptyParagraphIndexBeforeText = paragraphNumberIndex - 1;
                        int setParagraphResult = _bookData.GetIntContent(desiredTextLanguage, "SetParagraphText", paragraphTextMarks, emptyParagraphIndexBeforeText );//int GetIntContent(desiredTextLanguage, SetParagraphText, stringToSet, indexCount) надо писать в индекс[i] из массива индексов, а не в сам i - currentParagraphChapterNumberIndex
                        string newCurrentParagraph = _bookData.GetStringContent(desiredTextLanguage, "GetParagraphText", emptyParagraphIndexBeforeText);

                        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "textParagraph[" + paragraphNumberIndex.ToString() + "] = " + DConst.StrCRLF + textParagraph + DConst.StrCRLF +
                            "startparagraphNumberIndex = " + startparagraphNumberIndex.ToString() + DConst.StrCRLF +
                            "endparagraphNumberIndex = " + endparagraphNumberIndex.ToString() + DConst.StrCRLF +
                            "ParagraphTextMarks - " + paragraphTextMarks + DConst.StrCRLF +
                            "newCurrentParagraph - " + newCurrentParagraph, CurrentClassName, DConst.ShowMessagesLevel);
                    }
                }
                startparagraphNumberIndex = endparagraphNumberIndex + 1;//конечный абзац станет начальным для следующего прохода (надо +1? похоже, что надо)
            }
            return totalParagraphsNumbersCount;//пока не очень понятно, зачем он нужен
        }
       
        public int PortionBookTextOnParagraphs(int desiredTextLanguage)//делит текст на абзацы по EOL, сохраняет в List в AllBookData
        {
            string textToAnalyse = _bookData.GetFileContent(desiredTextLanguage);
            //тут заменим все многоточия фирменным символом, а также другие составные знаки - выделить в метод NormalizeEllipsis например
            int changedEllipsisVariationCount = 0;
            int charsEllipsisToChangeLength = DConst.charsEllipsisToChange1.Length;           

            for (int ellipsisVariationToChange = 0; ellipsisVariationToChange < charsEllipsisToChangeLength; ellipsisVariationToChange++)
            {
                int ellipsisVariationIndexFound = textToAnalyse.IndexOf(DConst.charsEllipsisToChange1[ellipsisVariationToChange]);
                bool ellipsisVariationFound = ellipsisVariationIndexFound > 0;

                if (ellipsisVariationFound)
                {
                    string textToAnalyseEllipsisChanged = textToAnalyse.Replace(DConst.charsEllipsisToChange1[ellipsisVariationToChange], DConst.charsEllipsisToChange2[ellipsisVariationToChange]);//Возвращает новую строку, в которой все вхождения заданной строки(1) в текущем экземпляре заменены другой строкой(2)
                    textToAnalyse = textToAnalyseEllipsisChanged;//освобождаем переменную для следующего прохода
                    changedEllipsisVariationCount++;
                }
            }
            _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Found and changed Ellipsis Variation Count = " + changedEllipsisVariationCount.ToString(), CurrentClassName, DConst.ShowMessagesLevel);

            string[] TextOnParagraphsPortioned = textToAnalyse.Split(DConst.charsParagraphSeparator);//portioned all book content in the ParagraphsArray via EOL
            //потом тут можно написать свой метод деления на абзацы (или этот пусть делит по одному сепаратору) - но не нужно
            int textOnParagraphsPortionedLength = TextOnParagraphsPortioned.Length;
            int allParagraphsWithTextCount = 0;
            
            for (int i = 0; i < textOnParagraphsPortionedLength; i++)//загружаем получившиеся абзацы в динамический массив, потом сравниваем длину массивов
            {
                string currentTextParagraph = TextOnParagraphsPortioned[i];
                bool currentTextParagraphIsEmpty = String.IsNullOrEmpty(currentTextParagraph);

                if (!currentTextParagraphIsEmpty)
                {
                    _bookData.GetIntContent(desiredTextLanguage, "AddParagraphText", "", -1);//записываем пустую строку - индекс не нужен, записывается в конец
                    int addParagraphTextCount = _bookData.GetIntContent(desiredTextLanguage, "AddParagraphText", currentTextParagraph, -1);//также возвращает количество уже существующих элементов                
                    allParagraphsWithTextCount++;
                    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Current Paragraph[" + i.ToString() + "] --> " + TextOnParagraphsPortioned[i] + "---control<CR>---" + DConst.StrCRLF +
                        "currentTextParagraphIsEmpty = " + currentTextParagraphIsEmpty.ToString(), CurrentClassName, DConst.ShowMessagesLevel);
                }
            }            
            return allParagraphsWithTextCount;
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}

//int GetIntContent(int desiredTextLanguage, string needOperationName) => _bookData.GetIntContent(desiredTextLanguage, needOperationName);//перегрузка для получения длины двуязычных динамических массивов
//int GetIntContent(string needOperationName, string stringToSet, int indexCount) => _bookData.GetIntContent(needOperationName, stringToSet, indexCount);//перегрузка для записи обычных массивов
//int GetIntContent(int desiredTextLanguage, string needOperationName, int indexCount) => _bookData.GetIntContent(desiredTextLanguage, needOperationName, indexCount);//перегрузка для удаления элементов динамических массивов
//int GetIntContent(int desiredTextLanguage, string needOperationName, string stringToSet, int indexCount) => _bookData.GetIntContent(desiredTextLanguage, needOperationName, stringToSet, indexCount);

//string GetStringContent(string nameOfStringNeed, int indexCount) => _bookData.GetStringContent(nameOfStringNeed, indexCount);
//string GetStringContent(int desiredTextLanguage, string nameOfStringNeed, int indexCount) => _bookData.GetStringContent(desiredTextLanguage, nameOfStringNeed, indexCount);

//string AddSome00ToIntNumber(string currentChapterNumberToFind, int totalDigitsQuantity) => _analysisLogic.AddSome00ToIntNumber(currentChapterNumberToFind, totalDigitsQuantity);
//string carriageReturn = "\r\n";
//currentChapterNameParagraph = "¤¤¤¤¤001¤¤¤-Chapter-\r\n"; тестовая строка для проверки
//public int SetMarkInParagraphText(int desiredTextLanguage, string paragraphTextMarks, int i, int enumerateParagraphsInChapterCount)
//{            
//    SetParagraphText(paragraphTextMarks, i, desiredTextLanguage);//ЗДЕСЬ запись SetParagraphText! - записываем номер абзаца в пустую строку после имени главы! проверить, что будет, если добавить перевод строки (хорошо будет)
//    enumerateParagraphsInChapterCount++;
//    return enumerateParagraphsInChapterCount;
//}
//достанем метки начала и конца номера главы и абзаца
//string[] allChapterMarks = GetStringArrConstant("ChapterMark");
//string beginChapterMark = allChapterMarks[0];//получим маркировку начала номера (в данном случае главы)
//int endChapterMarkIndex = GetIntConstant("ChapterMark") - 1;
//string endChapterMark = allChapterMarks[endChapterMarkIndex];//получим маркировку конца номера
//int chapterTotalDigits = GetIntConstant("ChapterTotalDigits");//получим количество символов-цифр для номера главы в марке
//string[] allParagraphMarks = GetStringArrConstant("ParagraphMark");
//string beginParagraphMark = allParagraphMarks[0];
//int endParagraphMarkIndex = GetIntConstant("ParagraphMark") - 1;
//string endParagraphMark = allParagraphMarks[endParagraphMarkIndex];
//int paragraphTotalDigits = GetIntConstant("ParagraphTotalDigits");//получим количество символов-цифр для номера абзаца в марке
//string needConstantnName = "Paragraph" + desiredTextLanguage.ToString();//формирование ключевого слова запроса в зависимости от языка - можно языка прямо передавать параметром
//string paragraphKeyWord = GetStringArrConstant(needConstantnName)[0]; //ключевые слова брать из массива ключевых слов - скажем, из нулевой позиции - или сделать отдельные массивы, с учетом языка, рядом с ключевыми словами           
//старые методы из _bookData
//SetParagraphText(chapterTextMarks, currentParagraphIndex - 1, desiredTextLanguage);//проверить, что i больше 0, иначе некуда заносить - ЗДЕСЬ запись SetParagraphText! - записываем номер главы в строку перед именем главы! проверить, что будет, если добавить перевод строки? ничего хорошего - потом не найти маркировку, так как строка теперь начинается не маркой
//ParagraphTextMarks += DConst.StrCRLF + textParagraph;//дописываем маркировку главы к названию главы, добавив строку для нее 
//int enumerateParagraphsCount = 0;//можно было поставить 0 - номер главы еще не найден, нумерация абзацев не начиналась  
//int GetParagraphTextLength(int desiredTextLanguage) => _bookData.GetParagraphTextLength(desiredTextLanguage);
//string GetParagraphText(int paragraphCount, int desiredTextLanguage) => _bookData.GetParagraphText(paragraphCount, desiredTextLanguage);
//int SetParagraphText(string paragraphText, int paragraphCount, int desiredTextLanguage) => _bookData.SetParagraphText(paragraphText, paragraphCount, desiredTextLanguage);
//новые методы из _arrayAnalysis
//int GetIntConstant(string needConstantnName) => _arrayAnalysis.GetIntConstant(needConstantnName);
//string[] GetStringArrConstant(string needConstantnName) => _arrayAnalysis.GetStringArrConstant(needConstantnName);
//старые методы из _arrayAnalysis
//int GetConstantWhatNotLength(string needConstantnName) => _arrayAnalysis.GetIntConstant(needConstantnName);
//string[] GetConstantWhatNot(string needConstantnName) => _arrayAnalysis.GetStringArrConstant(needConstantnName);
//public event EventHandler AnalyseInvokeTheMain;
//SetParagraphText(chapterTextMarks, currentParagraphIndex - 1, desiredTextLanguage);//проверить, что i больше 0, иначе некуда заносить - ЗДЕСЬ запись SetParagraphText! - записываем номер главы в строку перед именем главы! проверить, что будет, если добавить перевод строки? ничего хорошего - потом не найти маркировку, так как строка теперь начинается не маркой
//ParagraphTextMarks += DConst.StrCRLF + textParagraph;//дописываем маркировку главы к названию главы, добавив строку для нее 
//int enumerateParagraphsCount = 0;//можно было поставить 0 - номер главы еще не найден, нумерация абзацев не начиналась         
//enumerateParagraphsTotalCount++;//тут было бы общее количество абзацев в книге (можно добавить последние несколько строчек и занести общее количество глав и абзацев) - но пока нет (кому надо будет - сам посчитает, чай, не баре)
//подходящий момент разделить целое на части... можно прямо тут делить абзацы на предложения - но нет, к сожалению
//int countSentencesInCurrentParagraph = PrepareToDividePagagraphToSentences(desiredTextLanguage, currentParagraph, currentChapterNumber, enumerateParagraphsInChapterCount, i);//только нужный абзац для дележки - на i+1
//int charsParagraphSeparatorLength = GetConstantWhatNotLength("ParagraphSeparator");            
//string[] charsParagraphSeparatorInString = GetStringArrConstant("ParagraphSeparator");
//char[] charsParagraphSeparator = charsParagraphSeparatorInString[0].ToCharArray();
//string[] charsEllipsisToChange1 = GetConstantWhatNot("EllipsisToChange1");
//string[] charsEllipsisToChange2 = GetConstantWhatNot("EllipsisToChange2");
//currentTextParagraph = "---control<CR>---" + TextOnParagraphsPortioned[i] + "---control<CR>---";
//public int NormalizeEmptyParagraphs(int desiredTextLanguage)//удаляет идущие подряд пустые строки (абзацы) оставляя только одну
//{//нормируем пустые строки - идущие 2 подряд заменяем одной, пока не останется только по одной пустой строке
//    int paragraphTextLength = GetIntContent(desiredTextLanguage, "GetParagraphTextLength");
//    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), DConst.StrCRLF +
//        "paragraphTextLength = " + paragraphTextLength.ToString() + DConst.StrCRLF, CurrentClassName, DConst.ShowMessagesLevel);
//    bool normalizeEmptyParagraphsFlag = false;//флаг пустой текущей строки, true - если пустая
//    int emplyParagraphCount = 0;
//    int normalizeEmptyParagraphsResult = 0;            
//    for (int n = 0; n < paragraphTextLength; n++)//запускаем раз за разом нормализацию (удаление двух подряд пустых строк)
//    {
//        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), DConst.StrCRLF +
//                "Cycle Normalize with step n = " + n.ToString() + DConst.StrCRLF +
//                "счетчик удаленных строк до сброса = " + normalizeEmptyParagraphsResult.ToString(), CurrentClassName, DConst.ShowMessagesLevel);
//        normalizeEmptyParagraphsResult = 0;
//        //paragraphTextLength = _bookData.GetParagraphTextLength(desiredTextLanguage); //узнали исходное количество абзацев для старта цикла
//        paragraphTextLength = GetIntContent(desiredTextLanguage, "GetParagraphTextLength");
//        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), DConst.StrCRLF +
//            "normalizeEmptyParagraphsFlag is " + normalizeEmptyParagraphsFlag.ToString() + DConst.StrCRLF, CurrentClassName, DConst.ShowMessagesLevel);
//        normalizeEmptyParagraphsFlag = false;//сбрасываем флаг перед циклом
//        for (int i = paragraphTextLength - 1; i > 0; i--)//выбираем по очереди номера пустых строк, начиная с последней(-1, то шо нумерация массива с нуля - мелочь, а засада)
//        {
//            //string paragraphText = _bookData.GetParagraphText(i, desiredTextLanguage);//получили номер строки
//            string paragraphText = GetStringContent(desiredTextLanguage, "GetParagraphText", i);
//            if (String.IsNullOrWhiteSpace(paragraphText))//если текущая строка пустая, смотрим предыдущую строку
//            {
//                emplyParagraphCount++;//счетчик исходных пустых строк
//                if (normalizeEmptyParagraphsFlag)//если предыдущая строка была тоже пустой, удаляем текущую и сбрасываем флаг
//                {
//                    //int addParagraphTextCount = _bookData.RemoveAtParagraphText(i, desiredTextLanguage);//удаляет элемент списка с индексом i
//                    int addParagraphTextCount = GetIntContent(desiredTextLanguage, "RemoveAtParagraphText", i);
//                    normalizeEmptyParagraphsFlag = false;
//                    normalizeEmptyParagraphsResult++;//счетчик удаленых пустых строк
//                }
//                else normalizeEmptyParagraphsFlag = true;//если текущая строка пустая, а предыдущая не пустая - ставим флаг
//            }
//            else normalizeEmptyParagraphsFlag = false;//если текущая строка не пустая - сбрасываем флаг
//        }
//        //paragraphTextLength = _bookData.GetParagraphTextLength(desiredTextLanguage);
//        paragraphTextLength = GetIntContent(desiredTextLanguage, "GetParagraphTextLength");
//        _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), DConst.StrCRLF +
//                    "было удалено пустых строк = " + normalizeEmptyParagraphsResult.ToString() + DConst.StrCRLF +
//                    "всего было найдено пустых строк = " + emplyParagraphCount.ToString() + DConst.StrCRLF +
//                    "длина текстового массива стала = " + paragraphTextLength.ToString() + DConst.StrCRLF, CurrentClassName, DConst.ShowMessagesLevel);
//        if (normalizeEmptyParagraphsResult == 0) break;
//    }
//    return normalizeEmptyParagraphsResult;
//}
//if (addParagraphTextCount == textOnParagraphsPortionedLength)
//{
//    return textOnParagraphsPortionedLength;
//}
//else
//{//длина массивов не совпала, показываем диагностику, потом добавить еще постоянное сообщение (не Trace) на тему
//    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), DConst.StrCRLF +
//        "Paragraphs count after Split" + DConst.StrCRLF +
//        "and" + DConst.StrCRLF +
//        "Last count in List paragraphsTexts" + DConst.StrCRLF +
//        "are not the same " + DConst.StrCRLF +
//        DConst.StrCRLF +
//        "Paragraphs count after Split = " + textOnParagraphsPortionedLength.ToString() + DConst.StrCRLF +
//        "last count in List paragraphsTexts = " + addParagraphTextCount.ToString(), CurrentClassName, 3);
//    return textOnParagraphsPortionedLength;
//}
//int paragraphTextLength = _bookData.GetParagraphTextLength(desiredTextLanguage);
//чтобы не спутать с нулевым индексом на выходе, -1 - ничего нет (совсем ничего)
//string nextParagraph = GetStringContent(desiredTextLanguage, "GetParagraphText", currentParagraphChapterNumberIndex + 1); //следующий абзац с пустой строкой - проверить
//for (int chapterNumberIndex = 0; chapterNumberIndex < ChapterNumberParagraphsIndexesLength; chapterNumberIndex++)//перебираем все абзацы текста
//{
//    int currentParagraphChapterNumberIndex = ChapterNumberParagraphsIndexes[chapterNumberIndex];
//    string currentChapterNameParagraph = GetStringContent(desiredTextLanguage, "GetParagraphText", currentParagraphChapterNumberIndex);
//    //string currentParagraph = GetParagraphText(i, desiredTextLanguage);
//    string nextParagraph = GetStringContent(desiredTextLanguage, "GetParagraphText", currentParagraphChapterNumberIndex + 1); //следующий абзац с пустой строкой - проверить
//    string textParagraph = GetStringContent(desiredTextLanguage, "GetParagraphText", currentParagraphChapterNumberIndex + 2); //и следующий абзац с текстом - как проверить? только, что не пустой?
//    _msgService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "currentChapterNameParagraph[" + currentParagraphChapterNumberIndex.ToString() + "] = " + DConst.StrCRLF + currentChapterNameParagraph + DConst.StrCRLF +
//            "startSymbolOfChapterKeyWord = " + startSymbolOfChapterKeyWord.ToString() + DConst.StrCRLF +
//            "carriageReturnIndex = " + carriageReturnIndex.ToString() + DConst.StrCRLF +
//            "chapterKeyWordLength = " + chapterKeyWordLength.ToString() + DConst.StrCRLF +
//            "chapterKeyWord - " + chapterKeyWord, CurrentClassName, 3);
//    //можно и не проверять, у нас и так есть все индексы с номерами глав, но можно и проверить - для чего?
//    bool foundChapterMark = FindTextPartMarker(currentParagraph, "ChapterBegin");//проверяем начало абзаца на маркер главы, если есть, то надо найти номер главы и сбросить нумерацию абзацев на 1
//    if (foundChapterMark)
//    {
//        enumerateParagraphsCount = 1;
//        currentChapterNumber = FindTextPartNumber(currentParagraph, "ChapterBegin", paragraphTotalDigits);//ищем номер главы, перенести totalDigitsQuantity внутрь метода
//    }
//    string paragraphTextMarks = CreateParagraphMarks(currentChapterNumber, enumerateParagraphsCount);
//    //сформирована маркировка абзаца, можно искать начало абзацев (пустые строки) и заносить (пустые строки перед главой уже заняты)
//    bool currentParagraphEmptyResult = string.IsNullOrEmpty(currentParagraph);
//    bool nextParagraphIsNotEmpty = !string.IsNullOrEmpty(nextParagraph);
//    if (currentParagraphEmptyResult && nextParagraphIsNotEmpty)
//    {
//        enumerateParagraphsCount = SetMarkInParagraphText(desiredTextLanguage, paragraphTextMarks, chapterNumberIndex, enumerateParagraphsCount);//записываем маркировку абзаца в пустую строку и прибавляем счетчик номера абзаца
//        //если в этой строке нашли номер главы, то как в нее можно записать номер абзаца? надо разобраться, что вообще происходит
//    }                
//}            
//int GetCharsSeparatorLength(string ParagraphOrSentence) => _arrayAnalysis.GetConstantWhatNotLength(ParagraphOrSentence);
//string[] GetCharsSeparator(string ParagraphOrSentence) => _arrayAnalysis.GetConstantWhatNot(ParagraphOrSentence);
//int PrepareToDividePagagraphToSentences(int desiredTextLanguage, string currentParagraph, int currentChapterNumber, int currentParagraphNumber, int i) => 
//    _sentenceAnalyser.PrepareToDividePagagraphToSentences(desiredTextLanguage, currentParagraph, currentChapterNumber, currentParagraphNumber, i);
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
