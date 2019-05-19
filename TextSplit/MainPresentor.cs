using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;
using System.Windows.Forms;

namespace TextSplit
{
    public class MainPresentor
    {
        private readonly IAllBookData _book;
        private readonly ITextSplitForm _view;
        private ITextSplitOpenForm _open;        
        private readonly IMessageService _messageService;
        private readonly IAllBookAnalysis _analysis;        
        private readonly ILoadTextToDataBase _load;
        private readonly IMainLogicCultivation _logic;

        private bool wasEnglishContentChange = false;
        readonly private string strCRLF;
        readonly private int filesQuantity;
        readonly private int textFieldsQuantity;
        readonly private int buttonNamesCountInLanguageGroup;
        readonly private int showMessagesLevel;

        public MainPresentor(ITextSplitForm view, ITextSplitOpenForm open, IMessageService message, IAllBookAnalysis analysis, ILoadTextToDataBase load, IAllBookData book, IMainLogicCultivation logic)
        {
            _book = book;
            _view = view;            
            _messageService = message;
            _analysis = analysis;
            _load = load;
            _logic = logic;

            filesQuantity = DeclarationConstants.FilesQuantity;
            textFieldsQuantity = DeclarationConstants.TextFieldsQuantity;
            buttonNamesCountInLanguageGroup = DeclarationConstants.ButtonNamesCountInLanguageGroup;
            showMessagesLevel = DeclarationConstants.ShowMessagesLevel;
            strCRLF = DeclarationConstants.StrCRLF;

            string mainStart = "******************************************************************************************************************************************* \r\n";//Log-file separator
            _messageService.ShowTrace(mainStart + MethodBase.GetCurrentMethod().ToString(), " Started", CurrentClassName, showMessagesLevel);

            _view.OpenTextSplitOpenForm += new EventHandler(_view_OpenTextSplitOpenForm);            
            _view.TextSplitFormClosing += new EventHandler<FormClosingEventArgs>(_view_TextSplitFormClosing);
            _analysis.AnalyseInvokeTheMain += _analysis_AnalyseInvokeTheMain;
            // All EventHandlers for _open (TextSplitOpenForm) are located inside _view_OpenTextSplitOpenForm method
        }

        private void _analysis_AnalyseInvokeTheMain(object sender, EventArgs e)//тут просим пользователя выделить название любой главы
        {   //вызвали только для того, чтобы поменять имя на кнопке (самому TextBookAnalysis делать это не положено)
            //узнать, с каким языком вызвали - теперь для этого есть int indexToDo = _book.WhatFileNeedToDo(int whatNeedToDo);
            int invokeFrom = _book.WhatFileNeedToDo((int)WhatNeedDoWithFiles.SelectChapterName);//надо придумать, как сюда сообщить, зачем его вызвали - чтобы был универсальным
            if (invokeFrom != (int)MethodFindResult.NothingFound) //нашли - тогда поменять название на кнопке Analyse/Select на Select Chapter Name
            {
                int placeButton = invokeFrom + buttonNamesCountInLanguageGroup; // Place of the button the same as changed field (English or Russian) - для группы Analyse/Select смещаем выбор на соседнюю кнопку в том же языке (окне)                    
                int changeOnButtonTextResult = _open.ChangeOnButtonText(placeButton, (int)ButtonName.SelectChapterName, true);
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "changeOnButtonTextResult ==> " + changeOnButtonTextResult.ToString(), CurrentClassName, showMessagesLevel);                
            }            
        }

        private void _open_LoadEnglishToDataBase(object sender, EventArgs e)
        {   // обобщить метод для любого языка
            // разрешать запускать только после сохранения текста - гасить кнопку или менять название - Save на dB Load
            int ID_Language = 0;//работа с английской таблицей базы - может, надо убрать, по ToDo ясно, с кем работаем (но это не точно)
            
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                "filesToDo[English] must be CountSymbols here ==> " + 
                Enum.GetNames(typeof(WhatNeedDoWithFiles))[(int)TableLanguagesContent.English], 
                CurrentClassName, showMessagesLevel);

            if (_book.GetFileToDo((int)TableLanguagesContent.English) == (int)WhatNeedDoWithFiles.CountSymbols)//check is English content filled in the filesContent - CountSymbols was done
            {
                //ID_Language = -1; //временный костыль для заполнения таблицы, потом перевесить в настройки главной формы, для обычной работы пока что просто закомментировать
                int insertPortionResult = _load.PortionTextForDataBase(ID_Language); // - ID_Language можно тоже не передавать
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + strCRLF + " insertPortionResult ==> ", insertPortionResult.ToString(), CurrentClassName, showMessagesLevel);
            }            
        }

        private void _view_TextSplitFormClosing(object sender, FormClosingEventArgs e)
        {            
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " Closing attempt catched", CurrentClassName, showMessagesLevel);
            //var formArgs = (FormClosingEventArgs)e;
            e.Cancel = wasEnglishContentChange;
            //_view.WasEnglishContentChange = wasEnglishContentChange;
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " wasEnglishContentChange = ", wasEnglishContentChange.ToString(), CurrentClassName, showMessagesLevel);
        }

        void _view_OpenTextSplitOpenForm(object sender, EventArgs e)//обрабатываем нажатие кнопки Open, которое означает открытие вспомогательной формы
        {            
            TextSplitOpenForm openForm = new TextSplitOpenForm(_book, _messageService);

            _open = openForm;
            _open.OpenFileClick += new EventHandler(_open_OpenFileClick);
            _open.ContentChanged += new EventHandler(_open_ContentChanged);
            _open.SaveFileClick += new EventHandler(_open_SaveFileClick);
            _open.TimeToAnalyseTextBook += new EventHandler(_open_TimeToAnalyseTextBook);
            _open.LoadEnglishToDataBase += _open_LoadEnglishToDataBase;

            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "openForm will start now", CurrentClassName, showMessagesLevel);
            openForm.Show();
        }        
        
        private void _open_OpenFileClick(object sender, EventArgs e)
        {//вынести весь блок try/catch со внешнюю логику
            try
            {                
                int theAffectedElementNumber = _logic.isFilesExistCheckAndOpen();//там проверили ToDo, узнали, какую кнопку нажали, получили и вернули сюда номер элемента, с которым произведено действие
                //еще в том же методе проверили наличие файла по номеру FilePath и открыли его в массив FileContent - и теперь он там есть
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "theAffectedElementNumber = " + theAffectedElementNumber.ToString(), CurrentClassName, showMessagesLevel);
                if (_book.GetFileToDo(theAffectedElementNumber) == (int)WhatNeedDoWithFiles.ContinueProcessing)//добавить else WittingIncomplete)//если в элементе сказано, что все хорошо, отправляем данные в форму
                {
                    _open.SetFileContent(theAffectedElementNumber);//открытый в isFilesExistCheckAndOpen отправили в текстовое поле формы                                        
                    //change ButtonName from Open to Save - кнопка серая - текст только загружен и не изменялся                    
                    int changeOnButtonTextResult = _open.ChangeOnButtonText(theAffectedElementNumber, (int)ButtonName.SaveFile, false);//вызывается метод смены названия на кнопке и меняется на нужное
                    //change ButtonName AnalyseText и зажечь
                    int placeButton = theAffectedElementNumber + buttonNamesCountInLanguageGroup; // Place of the button the same as changed field (English or Russian) - смещаем выбор на соседнюю кнопку в том же языке (окне)                    
                    changeOnButtonTextResult = _open.ChangeOnButtonText(placeButton, (int)ButtonName.AnalyseText, true);//вызывается метод смены названия на кнопке и меняется на нужное

                    int SetSymbolsCountResult = _logic.SetSymbolsCountOnLabel(theAffectedElementNumber);//посчитали количество символов
                    _open.SetSymbolCount(theAffectedElementNumber);//записали количество символов в поле формы
                }
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }        
        
        void _open_ContentChanged(object sender, EventArgs e)
        {
            int theAffectedElementNumber = _book.WhatFileNeedToDo((int)WhatNeedDoWithFiles.ContentChanged);//спросили, на каком языке есть это значение
            if (theAffectedElementNumber != (int)MethodFindResult.NothingFound) //нашли - значение ContentChanged
            {
                // Place of the button the same as changed field (English or Russian) - ButtonName leave Save and активировать кнопку - текст изменился и его можно сохранить                    
                int changeOnButtonTextResult = _open.ChangeOnButtonText(theAffectedElementNumber, (int)ButtonName.SaveFile, true);
                //надо как-то проверять результаты переименования кнопки (отдельный метод прочитать текущие названия на всех кнопках?)
                //и погасить кнопку AnalyseText - пока текст не записан в файл (хотя, это не обязательно
                int placeButton = theAffectedElementNumber + buttonNamesCountInLanguageGroup; // Place of the button the same as changed field (English or Russian) - смещаем выбор на соседнюю кнопку в том же языке (окне)                    
                changeOnButtonTextResult = _open.ChangeOnButtonText(placeButton, (int)ButtonName.AnalyseText, false);//вызывается метод смены названия на кнопке и меняется на нужное

                int SetSymbolsCountResult = _logic.SetSymbolsCountOnLabel(theAffectedElementNumber);
                _open.SetSymbolCount(theAffectedElementNumber);
            }
        }        

        private void _open_SaveFileClick(object sender, EventArgs e)
        {            
            int theAffectedElementNumber = _book.WhatFileNeedToSave((int)WhatNeedSaveFiles.SaveFileFirst);//проверяем насчет сохранения в первый раз
            if (theAffectedElementNumber != (int)MethodFindResult.NothingFound) //нашли - значение SaveFileFirst - сохраняем в первый раз, надо спросить
            {
                int prepareFileSaveResult = _logic.IfSaveTextInFileFirtTime(theAffectedElementNumber);//предупреждение о перезаписи и возможные действия                
                _book.SetFileToSave(prepareFileSaveResult, theAffectedElementNumber);//записали признак обычного сохранения в индикатор (или признак выхода из программы)
            }
            theAffectedElementNumber = _book.WhatFileNeedToSave((int)WhatNeedSaveFiles.SaveFile);//теперь проверяем обычное сохранение
            if (theAffectedElementNumber != (int)MethodFindResult.NothingFound) //нашли - значение SaveFile
            {
                int fileSaveResult = _logic.SaveTextInFile(theAffectedElementNumber);//сохраняем файл
                if (fileSaveResult == (int)WhatNeedSaveFiles.FileSavedSuccessfully)
                {   //делаем кнопку Save серой                    
                    int changeOnButtonTextResult = _open.ChangeOnButtonText(theAffectedElementNumber, (int)ButtonName.SaveFile, false);                    
                    //и зажечь кнопку AnalyseText
                    int placeButton = theAffectedElementNumber + buttonNamesCountInLanguageGroup; // Place of the button the same as changed field (English or Russian) - смещаем выбор на соседнюю кнопку в том же языке (окне)                    
                    changeOnButtonTextResult = _open.ChangeOnButtonText(placeButton, (int)ButtonName.AnalyseText, true);//вызывается метод смены названия на кнопке и меняется на нужное
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                        "theAffectedElementNumber = " + theAffectedElementNumber.ToString() + strCRLF +
                        "prepareFileSaveResult = " + fileSaveResult.ToString(), CurrentClassName, showMessagesLevel);

                    _book.SetFileToSave((int)WhatNeedSaveFiles.FileSavedSuccessfully, theAffectedElementNumber);//сохранили в индикатор успешное сохранение
                }                
                else _book.SetFileToSave((int)WhatNeedSaveFiles.StopProcessing, theAffectedElementNumber);//если сохранение не получилось, то сохранили в индикатор завершение работы
            }
        }

        private void _open_TimeToAnalyseTextBook(object sender, EventArgs e)//пока не используется?
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Start", CurrentClassName, showMessagesLevel);
            // где-то тут погасить кнопку, но какую надпись - непонятно
            int analysisResult = _analysis.AnalyseTextBook();//еще можно было бы сразу анализировать текст на стандартные слова нумерации глав, а уже потом приставать к пользователю
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}