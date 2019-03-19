using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;
//using System.Windows.Forms;

namespace TextSplit
{
    public interface IMainLogicCultivation
    {
        int isFilesExistCheckAndOpen();

        int SetSymbolsCountOnLabel(int theAffectedElementNumber);
        int IfSaveTextInFileFirtTime(int theAffectedElementNumber);
        int SaveTextInFile(int theAffectedElementNumber);

        //event EventHandler AnalyseInvokeTheMain;
    }

    class MainLogicCultivation : IMainLogicCultivation
    {
        private readonly IAllBookData _book;
        private readonly IMessageService _messageService;
        private readonly IFileManager _manager;

        readonly private int filesQuantity;
        readonly private int buttonNamesCountInLanguageGroup;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;

        //public event EventHandler AnalyseInvokeTheMain;

        public MainLogicCultivation(IAllBookData book, IMessageService service, IFileManager manager)
        {
            _book = book;
            _messageService = service;
            _manager = manager;

            filesQuantity = Declaration.FilesQuantity;
            buttonNamesCountInLanguageGroup = Declaration.ButtonNamesCountInLanguageGroup;
            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;
        }

        public int isFilesExistCheckAndOpen()
        {
            //textFieldsQuantity количество текстовых окон в форме OpenForm, возможно изменить на количество файлов - если открывать и файл результатов
            int theAffectedElementNumber = _book.WhatFileNeedToDo((int)WhatNeedDoWithFiles.ReadFileFirst);//спросили, на каком языке есть это значение
            if (theAffectedElementNumber != (int)MethodFindResult.NothingFound) //нашли - значение ReadFileFirst
            {
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                "ReadFileFirst ==> theAffectedElementNumber = " + theAffectedElementNumber.ToString() + strCRLF +
                "_book.GetFilePath(theAffectedElementNumber) = " + _book.GetFilePath(theAffectedElementNumber), CurrentClassName, showMessagesLevel);

                if (_manager.IsFileExist(_book.GetFilePath(theAffectedElementNumber)))
                {//если нужный файл существует (потом добавить try/catch на доступность)
                    int setFilesContentResult = _book.SetFileContent(_manager.GetContent(theAffectedElementNumber), theAffectedElementNumber);//загрузили весь текст из файла в массив
                    int setToDoResult = _book.SetFileToDo((int)WhatNeedDoWithFiles.ContinueProcessing, theAffectedElementNumber);//установили флаг, что можно работать дальше
                    return theAffectedElementNumber;
                }
            }
            _messageService.ShowExclamation("The source file does not exist, please select it!");
            return (int)MethodFindResult.NothingFound; //some file does not exist
        }

        public int SetSymbolsCountOnLabel(int theAffectedElementNumber)
        {
            int setToDoResult = _book.SetFileToDo((int)WhatNeedDoWithFiles.CountSymbols, theAffectedElementNumber);
            int valueSymbolsCount = _manager.GetSymbolsCount(theAffectedElementNumber);
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "valueSymbolsCount = " + valueSymbolsCount.ToString(), CurrentClassName, showMessagesLevel);
            int setSymbolsCountResult = _book.SetSymbolsCount(valueSymbolsCount, theAffectedElementNumber);
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "setSymbolsCountResult = " + setSymbolsCountResult.ToString(), CurrentClassName, showMessagesLevel);
            return setSymbolsCountResult;
        }

        public int IfSaveTextInFileFirtTime(int theAffectedElementNumber) //убрать все дополнительные методы в отдельный класс
        {
            //сообщить, что файл будет перезаписан и спросить - перезаписать, сделать и сохранить копию исходника или прекратить работу
            _messageService.ShowMessage("File will be rewritten!");
            //тут сделать этот выбор - сохранять или выйти из программы, чтобы сделать рабочую копию (или сделать копию автоматически отсюда)
            //типа return                        
            //потом присваиваем индикатору значение SaveFile, чтобы потом сохранить как обычно
            return (int)WhatNeedSaveFiles.SaveFile;
            //return (int)WhatNeedSaveFiles.CannotSaveFile; - на случай если решим закрывать программу
        }

        public int SaveTextInFile(int theAffectedElementNumber)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                "filesToSave [i] - " + _book.GetFileToSave(theAffectedElementNumber).ToString() + strCRLF +
                "filesPath [i] ==> " + _book.GetFilePath(theAffectedElementNumber) + strCRLF +
                "filesContent [i] ==> " + _book.GetFileContent(theAffectedElementNumber), CurrentClassName, showMessagesLevel);
            try
            {
                int fileSaveResult = _manager.SaveContent(theAffectedElementNumber);
                if (fileSaveResult == (int)WhatNeedSaveFiles.FileSavedSuccessfully)
                {
                    _messageService.ShowMessage("File saved sucessfully!");
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
