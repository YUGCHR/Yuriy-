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
        private readonly ITextSplitForm _view; 
        private readonly IFileManager _manager;
        private readonly IMessageService _messageService;

        //private string _currentFilePath;
        private bool wasEnglishContentChange = false;
        private int i = 3;//Quantity of working files

        public MainPresentor(ITextSplitForm view, IFileManager manager, IMessageService service)
        {
            _view = view;
            _manager = manager;
            _messageService = service;

            int filesQuantity = _view.FilesQuantity;
            _manager.FilesQuantity = filesQuantity;
            int[] count = new int[3];
            _view.SetSymbolCount(count[]);            

            _view.EnglishContentChanged += new EventHandler (_view_EnglishContentChanged);
            _view.FilesOpenClick += new EventHandler (_view_FilesOpenClick);
            _view.FilesSaveClick += new EventHandler (_view_FilesSaveClick);            
            _view.TextSplitFormClosing += new EventHandler<FormClosingEventArgs>(_view_TextSplitFormClosing);
        }

        private void _view_TextSplitFormClosing(object sender, FormClosingEventArgs e)
        {            
            _messageService.ShowTrace("Closing attempt catched", "!!!");
            //var formArgs = (FormClosingEventArgs)e;
            e.Cancel = wasEnglishContentChange;
            //_view.WasEnglishContentChange = wasEnglishContentChange;
            _messageService.ShowTrace(wasEnglishContentChange.ToString(), " - Returned - Was English Content Change?");            
        }

        private void _view_FilesSaveClick(object sender, EventArgs e)
        {
            try
            {
                //string content = _view.FileContent;
                //_manager.SaveContent(content, _currentFilePath);
                _messageService.ShowMessage("File saved sucessfully!");
                wasEnglishContentChange = false;
                _messageService.ShowTrace(wasEnglishContentChange.ToString(), " - EnglishContentSaved!");
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
                string[] filePath = _view.FilePath;
                wasEnglishContentChange = false;

                _messageService.ShowTrace("Received filePath", filePath[0]); //traced
                bool isExist = _manager.IsExist(filePath[0]);
                if (!isExist)
                {
                    _messageService.ShowExclamation("Selected file does not exist!");
                    return;
                }

                //_currentFilePath = filePath[0];


                string[] content = new string[i];
                int[] count = new int[i];
                    _manager.GetContent(filePath[0]);
                
                    
                    //_manager.GetSymbolCount(content[]);

                _view.FileContent = content;
                _view.SetSymbolCount(count[]);
            }
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        void _view_EnglishContentChanged(object sender, EventArgs e)
        {
            string[] content = _view.FileContent;                        
            //int[] count = _manager.GetSymbolCount(content[]);
            _messageService.ShowTrace(wasEnglishContentChange.ToString(), " - EnglishContentWasChanged");
            //_view.SetSymbolCount(count[]);
            wasEnglishContentChange = true;
        }
    }
}
