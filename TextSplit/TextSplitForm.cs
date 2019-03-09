using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using TextSplitLibrary;
using System.Windows.Forms;


namespace TextSplit
{
    public interface ITextSplitForm
    {        
        int[] FilesToDo { get; set; }

        //event EventHandler FormOpenClick;
        event EventHandler FilesSaveClick;
        event EventHandler OpenTextSplitOpenForm;
        
        
        event EventHandler <FormClosingEventArgs> TextSplitFormClosing;
        //event EventHandler <FormClosingEventArgs> TextSplitOpenFormClosing;
    }

    public partial class TextSplitForm : Form, ITextSplitForm
    {
        private readonly IMessageService _messageService;
        private readonly ILogFileMessages _logs;

        public string[] FilesPath { get; set; }        
        public int[] FilesToDo { get; set; }
        public int[] counts;
        public Label[] lblSymbolsCount;

        private string LogBoxAllLines;

        readonly private string strCRLF;
        readonly private int filesQuantity = Declaration.FilesQuantity;
        readonly private int filesQuantityPlus = Declaration.ToDoQuantity;
        readonly private int showMessagesLevel = Declaration.ShowMessagesLevel;

        //public event EventHandler FormOpenClick;
        public event EventHandler FilesSaveClick;
        public event EventHandler OpenTextSplitOpenForm;
        
        //public event EventHandler TextSplitFormClosing;
        public event EventHandler<FormClosingEventArgs> TextSplitFormClosing;
        //public event EventHandler <FormClosingEventArgs> TextSplitOpenFormClosing;        

        public TextSplitForm(IMessageService service, ILogFileMessages logs)
        {            
            _messageService = service;
            _logs = logs;

            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " Started", CurrentClassName, showMessagesLevel);            
            InitializeComponent();
            
            butOpenForm.Click += new EventHandler(butOpenForm_Click);
            butSaveFiles.Click += butSaveFiles_Click;
           
            
            FormClosing += TextSplitForm_FormClosing;

            strCRLF = Declaration.StrCRLF;
            FilesToDo = new int[filesQuantityPlus];
            FilesPath = new string[filesQuantity];
            
    }

        #region Events forwarding // - obsolete
        private void TextSplitForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //MessageBox.Show(wasEnglishContentChange.ToString(), "It caught there, not here", MessageBoxButtons.OK, MessageBoxIcon.Information);
            TextSplitFormClosing(this, e);
            //MessageBox.Show(wasEnglishContentChange.ToString(), "We here again - last step", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //e.Cancel = wasEnglishContentChange;
            System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.AppendFormat("{0} = {1}", "CloseReason", e.CloseReason);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Cancel", e.Cancel);
            messageBoxCS.AppendLine();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " messageBoxCS = ", messageBoxCS.ToString(), CurrentClassName, showMessagesLevel);            
        }        

        void butOpenForm_Click(object sender, EventArgs e)//обрабатываем нажатие кнопки Open, которое означает открытие вспомогательной формы
        {
            //для начала проверить нет ли уже загруженных и измененных контентов и не начал ли строиться TextSplit - наверное, в мейне проверять
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " OpenTextSplitOpenForm will call now - " + OpenTextSplitOpenForm.ToString(), CurrentClassName, showMessagesLevel);
            if (OpenTextSplitOpenForm != null) OpenTextSplitOpenForm(this, EventArgs.Empty);                      
        }

        
        private void butSaveFiles_Click(object sender, EventArgs e)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " FilesSaveClick = ", FilesSaveClick.ToString(), CurrentClassName, showMessagesLevel);
            if (FilesSaveClick != null) FilesSaveClick(this, EventArgs.Empty);
        }


        #endregion

        public void SetLogBox(string LogBoxCurrentLineMessage, string LogBoxCurrentLineValue, int i)
        {
            string LogBoxCurrentLine = strCRLF + _logs.GetLogFileMessages(i) + strCRLF + LogBoxCurrentLineValue + strCRLF;
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " LogBoxCurrentLine = ", LogBoxCurrentLine, CurrentClassName, showMessagesLevel);
            
            SetLogBox(LogBoxCurrentLine, FilesPath[i], i);//Set the detail type of current action in OpenForm log textbox
            LogBoxAllLines = LogBoxAllLines + LogBoxCurrentLine;
            textBoxImplementation.Text = LogBoxAllLines;
            
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }        
    }
}
