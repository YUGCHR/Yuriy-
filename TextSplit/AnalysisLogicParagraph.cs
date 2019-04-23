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

        int FindParagraphTextNumber(string userSelectedText, int desiredTextLanguage, int startParagraphTextNumber);//plogic        
        int PortionBookTextOnParagraphs(int desiredTextLanguage);//plogic
        int normalizeEmptyParagraphs(int desiredTextLanguage);//plogic

        //event EventHandler AnalyseInvokeTheMain;
    }

    class AnalysisLogicParagraph : IAnalysisLogicParagraph
    {
        private readonly IAllBookData _book;
        private readonly IMessageService _messageService;

        readonly private int filesQuantity;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;

        private string[,] chapterNamesSamples;
        private readonly char[] charsParagraphSeparator;
        private readonly char[] charsSentenceSeparator;

        //public event EventHandler AnalyseInvokeTheMain;

        public AnalysisLogicParagraph(IAllBookData book, IMessageService service)
        {
            _book = book;
            _messageService = service;

            filesQuantity = Declaration.FilesQuantity;
            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;

            charsParagraphSeparator = new char[] { '\r', '\n' };
            charsSentenceSeparator = new char[] { '.', '!', '?' };

            //проверить типовые названия глав (для разных языков свои) - сделать метод универсальным и для частей тоже?
            chapterNamesSamples = new string[,]
            { { "Chapter ", "Paragraph ", "Section ", "Subhead ", "Part " },
                { "Глава ", "Параграф " , "Раздел ", "Подраздел ", "Часть " }, };//а номера глав бывают буквами!
        }        

        public int FindParagraphTextNumber(string userSelectedText, int desiredTextLanguage, int startParagraphTextNumber)
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
            return -1;
        }  

        public int PortionBookTextOnParagraphs(int desiredTextLanguage)//делит текст на абзацы по EOL, сохраняет в List в AllBookData
        {
            string textToAnalyse = _book.GetFileContent(desiredTextLanguage);
            //можно заранее определить размерность массива, если отказаться от динамических - 
            //int cnt = 0;
            //foreach (char c in test) { if (c == '&') cnt++; }

            string[] TextOnParagraphsPortioned = textToAnalyse.Split(charsParagraphSeparator);//portioned all book content in the ParagraphsArray via EOL            
                                                                                              //потом тут можно написать свой метод деления на абзацы (но это не точно)
            int textOnParagraphsPortionedLength = TextOnParagraphsPortioned.Length;//узнали количество абзацев после Split

            int addParagraphTextCount = 0;
            //int addParagraphNumberEmptyTextCount = 0; - нет нужды, все равно не возвратить в return
            for (int i = 0; i < textOnParagraphsPortionedLength; i++)//загружаем получившиеся абзацы в динамический массив, потом сравниваем длину массивов
            {
                addParagraphTextCount = _book.AddParagraphText(TextOnParagraphsPortioned[i], desiredTextLanguage);//также возвращает количество уже существующих элементов
                //if (String.IsNullOrWhiteSpace(TextOnParagraphsPortioned[i]))
                //{   //если текущий абзац - пустая строка, записываем номер абзаца в служебный массив
                //    int addParagraphNumberEmptyTextCount = _book.AddParagraphNumberEmptyText(i, desiredTextLanguage);
                //}
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

        public int normalizeEmptyParagraphs(int desiredTextLanguage)//удаляет идущие подряд пустые строки (абзацы) оставляя только одну
        {
            //нормируем пустые строки - идущие 2 подряд заменяем одной, пока не останется только по одной пустой строке

            int paragraphTextLength = _book.GetParagraphTextLength(desiredTextLanguage);
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                "paragraphTextLength = " + paragraphTextLength.ToString() + strCRLF, CurrentClassName, 3);

            bool normalizeEmptyParagraphsFlag = false;//флаг пустой текущей строки, true - если пустая
            int emplyParagraphCount = 0;
            int normalizeEmptyParagraphsResult = 0;            

            for (int n = 0; n < paragraphTextLength; n++)//запускаем раз за разом нормализацию (удаление двух подряд пустых строк)
            {
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                        "Cycle Normalize with step n = " + n.ToString() + strCRLF +
                        "счетчик удаленных строк до сброса = " + normalizeEmptyParagraphsResult.ToString(), CurrentClassName, showMessagesLevel);
                normalizeEmptyParagraphsResult = 0;

                paragraphTextLength = _book.GetParagraphTextLength(desiredTextLanguage); //узнали исходное количество абзацев для старта цикла

                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                    "normalizeEmptyParagraphsFlag is " + normalizeEmptyParagraphsFlag.ToString() + strCRLF, CurrentClassName, showMessagesLevel);
                normalizeEmptyParagraphsFlag = false;//сбрасываем флаг перед циклом

                for (int i = paragraphTextLength - 1; i > 0; i--)//выбираем по очереди номера пустых строк, начиная с последней(-1, то шо нумерация массива с нуля - мелочь, а засада)
                {
                    string paragraphText = _book.GetParagraphText(i, desiredTextLanguage);//получили номер строки                    
                    if (String.IsNullOrWhiteSpace(paragraphText))//если текущая строка пустая, смотрим предыдущую строку
                    {
                        emplyParagraphCount++;//счетчик исходных пустых строк
                        if (normalizeEmptyParagraphsFlag)//если предыдущая строка была тоже пустой, удаляем текущую и сбрасываем флаг
                        {
                            int addParagraphTextCount = _book.RemoveAtParagraphText(i, desiredTextLanguage);//удаляет элемент списка с индексом i
                            normalizeEmptyParagraphsFlag = false;
                            normalizeEmptyParagraphsResult++;//счетчик удаленых пустых строк
                        }
                        else normalizeEmptyParagraphsFlag = true;//если текущая строка пустая, а предыдущая не пустая - ставим флаг

                    }
                    else normalizeEmptyParagraphsFlag = false;//если текущая строка не пустая - сбрасываем флаг

                }
                paragraphTextLength = _book.GetParagraphTextLength(desiredTextLanguage);
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
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
