using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSplitLibrary;
using System.Windows.Forms;

namespace TextSplit
{
    public class MainPresentor
    {
        private int showLevel = 3;//0 - no message, 1 - Trace message, 2 - Value message, 3 - ???
        private readonly ITextSplitForm _view;
        private readonly ITextSplitOpenForm _open;
        private readonly IFileManager _manager;
        private readonly IMessageService _messageService;


        private bool wasEnglishContentChange = false;
        private int filesQuantity;

        private int[] filesToDo;
        private int[] counts;
        private string[] filesPath;
        private string[] filesContent;


        public MainPresentor(ITextSplitForm view, ITextSplitOpenForm open, IFileManager manager, IMessageService service)
        {
            MessageBox.Show("Main Started", "Main in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _view = view;
            _open = open;
            _manager = manager;
            _messageService = service;
            
            filesQuantity = Declaration.LanguagesQuantity;
            filesToDo = new int[filesQuantity];
            counts = new int[filesQuantity];
            _view.SetSymbolCount(counts, filesToDo);//move?

            filesPath = new string[filesQuantity];
            filesContent = new string[filesQuantity];            
            _view.ManageFilesContent(filesPath, filesContent, filesToDo);//move?

            _view.EnglishContentChanged += new EventHandler (_view_EnglishContentChanged);
            _view.FilesOpenClick += new EventHandler (_view_FilesOpenClick);
            _view.FilesSaveClick += new EventHandler (_view_FilesSaveClick);            
            _view.TextSplitFormClosing += new EventHandler<FormClosingEventArgs>(_view_TextSplitFormClosing);
            
        }

        private void _view_TextSplitFormClosing(object sender, FormClosingEventArgs e)
        {            
            _messageService.ShowTrace("Closing attempt", "catched", "Main - _view_TextSplitFormClosing", showLevel);
            //var formArgs = (FormClosingEventArgs)e;
            e.Cancel = wasEnglishContentChange;
            //_view.WasEnglishContentChange = wasEnglishContentChange;
            _messageService.ShowTrace("wasEnglishContentChange", wasEnglishContentChange.ToString(), "Main - _view_TextSplitFormClosing", showLevel);            
        }

        private void _view_FilesSaveClick(object sender, EventArgs e)
        {
            try
            {
                //string content = _view.FileContent;
                //_manager.SaveContent(content, _currentFilePath);
                _messageService.ShowMessage("File saved sucessfully!");
                wasEnglishContentChange = false;
                _messageService.ShowTrace("wasEnglishContentChange", wasEnglishContentChange.ToString(), "Main - _view_FilesSaveClick", showLevel);
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        private void _view_FilesOpenClick(object sender, EventArgs e)
        {
            try
            {                
                _messageService.ShowTrace("_view_FilesOpenClick", "started now", "Main", showLevel);
                filesPath = new string[] {"XXX-0", "XXX-1", "XXX-2"};//Testing of array delivery to Form
                _messageService.ShowTrace("filesPath", filesPath, "Main!!!!!!!!!!!!!!!", showLevel);                
                _view.ManageFilesContent(filesPath, filesContent, filesToDo);
                int[] counts = new int[filesQuantity];
                wasEnglishContentChange = false;
                bool[] isFilesExist = new bool[filesQuantity];

                isFilesExist = _manager.IsFilesExist(filesPath);

                for (int i = 0; i < filesQuantity; i++)
                { 
                    _messageService.ShowTrace("Received filePath", filesPath[i], "Main-_view_FilesOpenClick", showLevel); //traced

                if (!isFilesExist[i])
                    {                        
                        _messageService.ShowExclamation("Selected file does not exist!");
                        return;
                    }
                    //counts = _manager.GetSymbolCounts(_manager.GetContents(_view.FilesPath, _view.FilesToDo));
                    //_view.SetSymbolCount(counts, filesToDo);
                    //_view.FilesContent = _manager.GetContents(_view.FilesPath, _view.FilesToDo);
                }
            }            
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        void _view_EnglishContentChanged(object sender, EventArgs e)
        {
            //string[] contents = _view.FilesContent;                        
            //int[] counts = _manager.GetSymbolCounts(contents);
            _messageService.ShowTrace("wasEnglishContentChange", wasEnglishContentChange.ToString(), "Main - EnglishContentWasChanged", showLevel);
            //_view.SetSymbolCount(counts, _view.FilesToDo);
            wasEnglishContentChange = true;//we need also the array here
        }
    }
}
