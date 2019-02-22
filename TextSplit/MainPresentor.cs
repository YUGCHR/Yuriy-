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
        private readonly ITextSplitOpenForm _open;
        private readonly IFileManager _manager;
        private readonly IMessageService _messageService;


        private bool wasEnglishContentChange = false;
        private int filesQuantity;
        
        public MainPresentor(ITextSplitForm view, ITextSplitOpenForm open, IFileManager manager, IMessageService service)
        {
            MessageBox.Show("Main Started", "Main in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            _view = view;
            _open = open;
            _manager = manager;
            _messageService = service;

            _manager.FilesQuantity = Declaration.LanguagesQuantity;

            int[] filesToDo = _view.FilesToDo;
            filesQuantity = filesToDo.Length;
            int[] counts = new int[filesQuantity];
            _view.SetSymbolCount(counts, _view.FilesToDo);
            _manager.FilesToDo = filesToDo;

            MessageBox.Show(_view.FilesPath[0], " - FilePath[0] form", MessageBoxButtons.OK, MessageBoxIcon.Information);            
            MessageBox.Show(_view.FilesPath[1], " - FilePath[2] form", MessageBoxButtons.OK, MessageBoxIcon.Information);            
            MessageBox.Show(_view.FilesPath[2], " - FilePath[1] form", MessageBoxButtons.OK, MessageBoxIcon.Information);            
            _open.FilesPath = _view.FilesPath;
            MessageBox.Show(_open.FilesPath[0], " - FilePath[0] open", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(_open.FilesPath[1], " - FilePath[0] open", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show(_open.FilesPath[2], " - FilePath[2] open", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                int[] counts = new int[filesQuantity];
                wasEnglishContentChange = false;
                bool[] isFilesExist = new bool[filesQuantity];

                isFilesExist = _manager.IsFilesExist(_view.FilesPath);
                //_messageService.ShowTrace("Received filePath", filesPath[i]); //traced
                for (int i = 0; i < filesQuantity; i++)
                    if (!isFilesExist[i])
                    {
                        _messageService.ShowExclamation("Selected file does not exist!");
                        return;
                    }
                counts = _manager.GetSymbolCounts(_manager.GetContents(_view.FilesPath, _view.FilesToDo));
                _view.SetSymbolCount(counts, _view.FilesToDo);
                _view.FilesContent = _manager.GetContents(_view.FilesPath, _view.FilesToDo);
            }            
            catch (Exception ex)
            {
                _messageService.ShowError(ex.Message);
            }
        }

        void _view_EnglishContentChanged(object sender, EventArgs e)
        {
            string[] contents = _view.FilesContent;                        
            int[] counts = _manager.GetSymbolCounts(contents);
            _messageService.ShowTrace(wasEnglishContentChange.ToString(), " - EnglishContentWasChanged");
            _view.SetSymbolCount(counts, _view.FilesToDo);
            wasEnglishContentChange = true;//we need also the array here
        }
    }
}
