using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;
using System.Windows.Forms;

namespace TextSplit
{
    public interface ITextSplitOpenForm
    {        
        string[] GetFilesPath();
        void SetFilesToDo(int[] filesToDo);
        event EventHandler AllOpenFilesClick;
        //event EventHandler FormOpenClick;
        //event EventHandler<FormClosingEventArgs> TextSplitOpenFormClosing;
    }

    public partial class TextSplitOpenForm : Form, ITextSplitOpenForm
    {
        private readonly IMessageService _messageService;
        private readonly ILogFileMessages _logs;

        public string[] FilesPath;// { get; set; }        
        public int[] FilesToDo;// { get; set; }

        readonly private int filesQuantity;
        readonly private int filesQuantityPlus;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;
        readonly private int iBreakpointManager;

        private string LogBoxAllLines;

        public event EventHandler AllOpenFilesClick;
        //public event EventHandler FormOpenClick;
        //public event EventHandler<FormClosingEventArgs> TextSplitOpenFormClosing;

        public TextSplitOpenForm(IMessageService service, ILogFileMessages logs)
        {            
            _messageService = service;
            _logs = logs;
            filesQuantity = Declaration.FilesQuantity; //the length of the all Files___ arrays (except FilesToDo)
            filesQuantityPlus = Declaration.ToDoQuantity; //the length of the FilesToDo array (+1 for BreakpointManager)
            iBreakpointManager = filesQuantityPlus-1; //index of BreakpointManager in the FilesToDo array
            showMessagesLevel = Declaration.ShowMessagesLevel;
            strCRLF = Declaration.StrCRLF;

            InitializeComponent();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "InitializeComponent", CurrentClassName, showMessagesLevel);

            FilesToDo = new int[filesQuantityPlus];
            FilesPath = new string[filesQuantity];
            
            butAllFilesOpen.Click += new EventHandler (butAllFilesOpen_Click);
            butSelectEnglishFile.Click += butSelectFile_Click;
            butSelectRussianFile.Click += butSelectFile_Click;
            butSelectResultFile.Click += butSelectFile_Click;

            //butCreateResultFile.Click += butSelectEnglishFile_Click; it needs to clear up
            //fld2CreateResultFileName - result file name field

            FormClosing += TextSplitOpenForm_FormClosing;            
        }

        public string[] GetFilesPath()
        {            
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " FilesPath[] = ", FilesPath, CurrentClassName, showMessagesLevel);
            return FilesPath;
        }

        public void SetFilesToDo(int[] filesToDo)
        {
            FilesToDo = filesToDo;
        }

        private void TextSplitOpenForm_FormClosing(object sender, FormClosingEventArgs e)//it needs to clear up
        {            
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Start", CurrentClassName, showMessagesLevel);
            //TextSplitOpenFormClosing(this, e);            
            //e.Cancel = wasEnglishContentChange;
            System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.AppendFormat("{0} = {1}", "CloseReason", e.CloseReason);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Cancel", e.Cancel);
            messageBoxCS.AppendLine();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + "FormClosing Event", messageBoxCS.ToString(), CurrentClassName, showMessagesLevel);            
        }

        void butAllFilesOpen_Click(object sender, EventArgs e)
        {
            if (AllOpenFilesClick != null) AllOpenFilesClick(this, EventArgs.Empty);            
            int BreakpointManager = FilesToDo[iBreakpointManager];            
            if (BreakpointManager != (int)WhatNeedDoWithFiles.WittingIncomplete) this.Close();//if we have received all data from OpenForm we can close it
        }        

        private void butSelectFile_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            string ButtonName = button.Name;            

            for (int i = 0; i < filesQuantity; i++)
            {
                string currentOpenFormButtonName = Enum.GetNames(typeof(OpenFormButtonNames))[i];
                if (ButtonName == currentOpenFormButtonName)
                {                    
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Filter = "Text files|*.txt|All files|*.*";

                    if (dlg.ShowDialog() == DialogResult.OK)
                    {                        
                        FilesPath[i] = dlg.FileName;
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " FilesPath[i] = ", FilesPath[i] + " [" + i.ToString() + "]", CurrentClassName, showMessagesLevel);                        
                    }
                    statusBottomLabel.Text = Enum.GetNames(typeof(OpenFormProgressStatusMessages))[i] + " - " + FilesPath[i];//Set the short type of current action in the status bar - delete?
                    string LogBoxCurrentLine = "";//it does not use here, need to make Overload of SetLogBox (and add with ToDo one)
                    SetLogBox(LogBoxCurrentLine, FilesPath[i], i);//Set the detail type of current action in OpenForm log textbox
                    FilesToDo[i] = (int)WhatNeedDoWithFiles.PassThrough;//file maybe exists but we will check this more clearly - delete?
                }
            }            
        }

        public void SetLogBox(string LogBoxCurrentLineMessage, string LogBoxCurrentLineValue, int i)
        {            
            string LogBoxCurrentLine = strCRLF + _logs.GetLogFileMessages(i) + strCRLF + LogBoxCurrentLineValue + strCRLF;             
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " LogBoxCurrentLine = ", LogBoxCurrentLine, CurrentClassName, showMessagesLevel);
            LogBoxAllLines = LogBoxAllLines + LogBoxCurrentLine;
            textBoxImplementation.Text = LogBoxAllLines;
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
