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
        private readonly IAnalysisLogicCultivation _alogic;
        private readonly IAnalysisLogicChapter _clogic;
        private readonly IAnalysisLogicParagraph _plogic;
        private readonly IAnalysisLogicSentences _slogic;

        readonly private int filesQuantity;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;

        private string[,] chapterNamesSamples;
        private readonly char[] charsParagraphSeparator;
        private readonly char[] charsSentenceSeparator;

        public event EventHandler AnalyseInvokeTheMain;

        public TextBookAnalysis(IAllBookData book, IMessageService service, IAnalysisLogicCultivation alogic, IAnalysisLogicChapter clogic, IAnalysisLogicParagraph plogic, IAnalysisLogicSentences slogic)
        {
            _book = book;
            _messageService = service;
            _alogic = alogic;//общая логика
            _clogic = clogic;//главы
            _plogic = plogic;//абзацы
            _slogic = slogic;//предложения

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

        public int AnalyseTextBook() // типа Main в логике анализа текста
        {
            int chapterCount = 0;
            int desiredTextLanguage = _alogic.GetDesiredTextLanguage();//возвращает номер языка, если на нем есть AnalyseText или AnalyseChapterName
            if (desiredTextLanguage == (int)MethodFindResult.NothingFound) return desiredTextLanguage;//типа, нечего анализировать
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "Start desiredTextLanguage = " + desiredTextLanguage.ToString(), CurrentClassName, showMessagesLevel);

            if (_book.GetFileToDo(desiredTextLanguage) == (int)WhatNeedDoWithFiles.AnalyseText)
            {   //если первоначальный анализ текста, то делим текст на абзацы, создем служебный массив с нумерацией пустых строк
                int portionBookTextResult = _plogic.PortionBookTextOnParagraphs(desiredTextLanguage);
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "portionBookTextResult = " + portionBookTextResult.ToString(), CurrentClassName, 3);

                int normalizeEmptyParagraphsResult = _plogic.normalizeEmptyParagraphs(desiredTextLanguage);//удаляем лишние пустые строки, оставляя только одну, результат - количество удаленных пустых строк
                
                //проверить, сколько получилось строк в тексте - что не ноль хотя бы
                //сначала ищем названия глав без подсказки
                chapterCount = _clogic.ChapterNameAnalysis(desiredTextLanguage);
                

                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Stop here ", CurrentClassName, 3);


                //следующий блок - в отдельный метод
                _book.SetFileToDo((int)WhatNeedDoWithFiles.SelectChapterName, desiredTextLanguage);//сообщаем Main, что надо поменять название на кнопке на Select
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                        "AnalyseInvokeTheMain with desiredTextLanguage = " + desiredTextLanguage.ToString(), CurrentClassName, showMessagesLevel);
                    if (AnalyseInvokeTheMain != null) AnalyseInvokeTheMain(this, EventArgs.Empty);//пошли узнавать у пользователя, как маркируются главы
                
                return portionBookTextResult;
            }
            if (_book.GetFileToDo(desiredTextLanguage) == (int)WhatNeedDoWithFiles.AnalyseChapterName) //анализ названия главы с полученной подсказкой от пользователя
            {//если получена подсказка от пользователя - текст названия главы, то изучаем полученный выбранный текст
                chapterCount = _clogic.UserSelectedChapterNameAnalysis(desiredTextLanguage);
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF + "Found Chapters count = " + chapterCount.ToString(), CurrentClassName, 3);
                return chapterCount;
            }
            return desiredTextLanguage;//типа, нечего анализировать
        }

        

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }

    }
}
