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
        private readonly IFileManager _manager;
        private readonly IMessageService _messageService;
        private readonly ITextBookAnalysis _analysis;        
        private readonly ILoadTextToDataBase _load;

        private bool wasEnglishContentChange = false;
        readonly private string strCRLF;        
        readonly private int textFieldsQuantity;        
        readonly private int showMessagesLevel;

        public MainPresentor(ITextSplitForm view, ITextSplitOpenForm open, IFileManager manager, IMessageService service, ITextBookAnalysis analysis, ILoadTextToDataBase load, IAllBookData book)
        {
            _book = book;
            _view = view;            
            _manager = manager;
            _messageService = service;
            _analysis = analysis;
            _load = load;
            
            textFieldsQuantity = Declaration.TextFieldsQuantity;            
            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;

            string mainStart = "******************************************************************************************************************************************* \r\n";//Log-file separator
            _messageService.ShowTrace(mainStart + MethodBase.GetCurrentMethod().ToString(), " Started", CurrentClassName, showMessagesLevel);

            _view.OpenTextSplitOpenForm += new EventHandler(_view_OpenTextSplitOpenForm);            
            _view.TextSplitFormClosing += new EventHandler<FormClosingEventArgs>(_view_TextSplitFormClosing);
            // All EventHandlers for _open (TextSplitOpenForm) are located inside _view_OpenTextSplitOpenForm method
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
            TextSplitOpenForm openForm = new TextSplitOpenForm(_messageService, _book);

            _open = openForm;
            _open.OpenFileClick += new EventHandler(_open_OpenFileClick);
            _open.ContentChanged += new EventHandler(_open_ContentChanged);
            _open.SaveFileClick += new EventHandler(_open_SaveFileClick);
            _open.LoadEnglishToDataBase += _open_LoadEnglishToDataBase;

            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "openForm will start now", CurrentClassName, showMessagesLevel);
            openForm.Show();
        }        
        
        private void _open_OpenFileClick(object sender, EventArgs e)
        {
            try
            {                
                int theAffectedElementNumber = isFilesExistCheckAndOpen();//там проверили ToDo, узнали, какую кнопку нажали, получили и вернули сюда номер элемента, с которым произведено действие
                //еще в том же методе проверили наличие файла по номеру FilePath и открыли его в массив FileContent - и теперь он там есть
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "theAffectedElementNumber = " + theAffectedElementNumber.ToString(), CurrentClassName, showMessagesLevel);
                if (_book.GetFileToDo(theAffectedElementNumber) == (int)WhatNeedDoWithFiles.ContinueProcessing)//добавить else WittingIncomplete)//если в элементе сказано, что все хорошо, отправляем данные в форму
                {
                    _open.SetFileContent(theAffectedElementNumber);//открытый в isFilesExistCheckAndOpen отправили в текстовое поле формы
                    
                    int mfButtonPlace = theAffectedElementNumber; // Place of the button which was pressed (English or Russian)
                    int mfButtonText = 1; //change ButtonName from Open to Save
                    bool mfButtonEnableFlag = false; //кнопка серая - текст только загружен и не изменялся

                    int checkOnButtonTextResult = mfButtonPlace + mfButtonText;
                    int changeOnButtonTextResult = _open.ChangeOnButtonText(mfButtonPlace, mfButtonText, mfButtonEnableFlag);//вызывается метод смены названия на кнопке и меняется на нужное
                    //if (changeOnButtonTextResult == checkOnButtonTextResult) - allright

                    int SetSymbolsCountResult = SetSymbolsCountOnLabel(theAffectedElementNumber);
                }
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        int isFilesExistCheckAndOpen()
        {
            //textFieldsQuantity количество текстовых окон в форме OpenForm, возможно изменить на количество файлов - если открывать и файл результатов
            for (int i = 0; i < textFieldsQuantity; i++)//если добавится textBox для Result, то сделать <=
            {                
                int theAffectedElementNumber = _book.GetFileToDo(i);

                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                    "isFilesExistCheckAndOpen ==> i = " + i.ToString() + strCRLF + 
                    "theAffectedElementNumber = " + Enum.GetNames(typeof(WhatNeedDoWithFiles))[theAffectedElementNumber], CurrentClassName, showMessagesLevel);                

                if (theAffectedElementNumber == (int)WhatNeedDoWithFiles.ReadFileFirst)
                {
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                    "ReadFileFirst ==> i = " + i.ToString() + strCRLF +
                    "_book.GetFilePath(i) = " + _book.GetFilePath(i), CurrentClassName, showMessagesLevel);

                    if (_manager.IsFileExist(_book.GetFilePath(i)))
                    {                           
                        int setFilesContentResult = _book.SetFileContent(_manager.GetContent(i), i);
                        int setToDoResult = _book.SetFileToDo((int)WhatNeedDoWithFiles.ContinueProcessing, i);
                        return i;
                    }                    
                }
            }
            _messageService.ShowExclamation("The source file does not exist, please select it!");            
            return -1; //some file does not exist
        }
        
        void _open_ContentChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < textFieldsQuantity; i++)
            {
                int WhatNeedDoWith = _book.GetFileToDo(i);
                if (WhatNeedDoWith == (int)WhatNeedDoWithFiles.ContentChanged)
                {                   
                    int mfButtonPlace = i; // Place of the button the same as changed field (English or Russian)
                    int mfButtonText = 1; // ButtonName leave Save
                    bool mfButtonEnableFlag = true; //активировать кнопку - текст изменился и его можно сохранить                    
                    int checkOnButtonTextResult = mfButtonPlace + mfButtonText; // типа, контроль
                    int changeOnButtonTextResult = _open.ChangeOnButtonText(mfButtonPlace, mfButtonText, mfButtonEnableFlag);
                    //if (changeOnButtonTextResult == checkOnButtonTextResult) - allright

                    int SetSymbolsCountResult = SetSymbolsCountOnLabel(i);
                }                
            }
        }

        private int SetSymbolsCountOnLabel(int i)
        {
            int setToDoResult = _book.SetFileToDo((int)WhatNeedDoWithFiles.CountSymbols, i);
            int valueSymbolsCount = _manager.GetSymbolsCount(i);
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "valueSymbolsCount = " + valueSymbolsCount.ToString(), CurrentClassName, showMessagesLevel);
            int setSymbolsCountResult = _book.SetSymbolsCount(valueSymbolsCount, i);
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "setSymbolsCountResult = " + setSymbolsCountResult.ToString(), CurrentClassName, showMessagesLevel);            
            return _open.SetSymbolCount(i);
        }

        private void _open_SaveFileClick(object sender, EventArgs e)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Start", CurrentClassName, showMessagesLevel);
            int prepareFileSaveResult = PrepareSaveTextInFile();
            //if (fileSaveResult == (int)WhatNeedSaveFiles.CannotSaveFile) - все пропало
        }

        private int PrepareSaveTextInFile()
        {
            //filesToSave = _open.GetFilesToSave();
            for (int i = 0; i < textFieldsQuantity; i++)
            {
                int theAffectedElementNumber = _book.GetFileToSave(i);
                if (theAffectedElementNumber == (int)WhatNeedSaveFiles.SaveFileFirst)
                {
                    //сообщить, что файл будет перезаписан и если не копия, то пусть идет и делает сам копию, то шо нефиг (после первого сохранения больше не спрашивать)
                    _messageService.ShowMessage("File will be rewritten!");
                    //тут сделать выбор - сохранять или выйти из программы, чтобы сделать рабочую копию (или сделать копию автоматически отсюда)
                    //типа return                        
                    int fileSaveResult = SaveTextInFile(i);
                    if (fileSaveResult == (int)WhatNeedSaveFiles.FileSavedSuccessfully) return (int)WhatNeedSaveFiles.FileSavedSuccessfully;
                    return (int)WhatNeedSaveFiles.CannotSaveFile;
                }
                if (theAffectedElementNumber == (int)WhatNeedSaveFiles.SaveFile)
                {
                    int fileSaveResult = SaveTextInFile(i);
                    if (fileSaveResult == (int)WhatNeedSaveFiles.FileSavedSuccessfully) return (int)WhatNeedSaveFiles.FileSavedSuccessfully;
                    return (int)WhatNeedSaveFiles.CannotSaveFile;                    
                }
            }
            return (int)WhatNeedSaveFiles.CannotSaveFile;
        }

        private int SaveTextInFile(int i)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "SaveTextInFile fetched i = " + i.ToString(), CurrentClassName, showMessagesLevel);

            int setToSaveResult = _book.SetFileToSave((int)WhatNeedSaveFiles.FileSavedSuccessfully, i);

            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),                
                "filesToSave [i] - " + _book.GetFileToSave(i).ToString() + strCRLF +
                "filesPath [i] ==> " + _book.GetFilePath(i).ToString() + strCRLF +
                "filesContent [i] ==> " + _book.GetFileContent(i).ToString(), CurrentClassName, showMessagesLevel);
            try
            {
                int fileSaveResult = _manager.SaveContent(i);
                if (fileSaveResult == (int)WhatNeedSaveFiles.FileSavedSuccessfully)
                {
                    _messageService.ShowMessage("File saved sucessfully!");
                    //тут сделать кнопку Save серой
                    int mfButtonPlace = i; // Place of the button the same as changed field (English or Russian)
                    int mfButtonText = 1; // ButtonName leave Save
                    bool mfButtonEnableFlag = false; //текст сохранен, кнопку погасили
                    int checkOnButtonTextResult = mfButtonPlace + mfButtonText; // типа, контроль

                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                    "mfButtonPlace = " + mfButtonPlace.ToString() + strCRLF +
                    "mfButtonText = " + mfButtonText.ToString() + strCRLF +
                    "mfButtonEnableFlag ==> " + mfButtonEnableFlag.ToString() + strCRLF, CurrentClassName, showMessagesLevel);

                    int changeOnButtonTextResult = _open.ChangeOnButtonText(mfButtonPlace, mfButtonText, mfButtonEnableFlag);
                    //if (changeOnButtonTextResult == checkOnButtonTextResult) - allright                

                    return (int)WhatNeedSaveFiles.FileSavedSuccessfully;
                }
                else
                {                    
                    return (int)WhatNeedSaveFiles.CannotSaveFile;
                }
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
                return (int)WhatNeedSaveFiles.CannotSaveFile;
            }
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}