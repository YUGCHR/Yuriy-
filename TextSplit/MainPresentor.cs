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
        
        private bool wasEnglishContentChange = false;
        private int filesQuantity;


        public MainPresentor(ITextSplitForm view, IFileManager manager, IMessageService service)
        {
            MessageBox.Show("Main Started", "Main in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _view = view;
            _manager = manager;
            _messageService = service;
            
            int[] FilesToDo = _view.FilesToDo;
            filesQuantity = FilesToDo.Length;
            int[] count = new int[filesQuantity];
            _view.SetSymbolCount(count);
            _manager.FilesToDo = FilesToDo;

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
                //string[] content = new string[filesQuantity];
                //string[] filePath = _view.FilePath;
                //int[] count = new int[filesQuantity];
                wasEnglishContentChange = false;
                bool[] isExist = new bool[filesQuantity];

                isExist = _manager.IsExist(filePath);

                for (int i = 0; i < filesQuantity; i++)
                {
                    _messageService.ShowTrace("Received filePath", filePath[i]); //traced
                    
                    if (!isExist[i])
                    {
                        _messageService.ShowExclamation("Selected file does not exist!");
                        return;
                    }                    
                }
                _view.FileContent = content;
                _view.SetSymbolCount(count);
                _manager.GetContent(filePath);
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
