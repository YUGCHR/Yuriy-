using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextSplit
{
    public interface ITextSplitForm
    {
        string[] FilesPath { get; set; }
        string[] FilesContent { get; set; }        
        int[] FilesToDo { get; set; }
        void SetSymbolCount(int[] counts, int[] filesToDo);
          
        event EventHandler FilesOpenClick;
        event EventHandler FilesSaveClick;
        event EventHandler EnglishContentChanged;
        //event EventHandler TextSplitFormClosing;
        event EventHandler <FormClosingEventArgs> TextSplitFormClosing;
    }

    public partial class TextSplitForm : Form, ITextSplitForm
    {               
        public string[] FilesPath { get; set; }
        public string[] FilesContent { get; set; }
        public Label[] lblSymbolsCount;
        public int[] FilesToDo { get; set; }
        public int[] counts;
        public int filesQuantity = 3;//Please remember - the quantity of the working files must be declared here

        public TextSplitForm()
        {
            MessageBox.Show("Form Started", "Form in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            InitializeComponent();

            butOpenFiles.Click += new EventHandler(butOpenFiles_Click);
            butSaveFiles.Click += butSaveFiles_Click;
            fldEnglishContent.TextChanged += fldEnglishContent_TextChanged;
            butSelectEnglishFile.Click += butSelectEnglishFile_Click;
            numFont.ValueChanged += numFont_ValueChanged;

            FormClosing += TextSplitForm_FormClosing;            
            FilesPath = new string[filesQuantity];
            FilesContent = new string[filesQuantity];
            FilesToDo = new int[filesQuantity];            
            lblSymbolsCount = new Label[] { lblSymbolCount1, lblSymbolCount2, lblSymbolCount3 };
        }

        #region Events forwarding
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
            MessageBox.Show(messageBoxCS.ToString(), "FormClosing Event");
        }
                
        void butOpenFiles_Click(object sender, EventArgs e)
        {
            MessageBox.Show("butOpenFiles_Click - Started", "butOpenFiles_Click", MessageBoxButtons.OK, MessageBoxIcon.Information);
            TextSplitOpenForm openForm = new TextSplitOpenForm(this);
            MessageBox.Show("openForm.Show will start now", "openForm.Show", MessageBoxButtons.OK, MessageBoxIcon.Information);
            openForm.Show();
            MessageBox.Show("after openForm.Show now", "openForm.Show", MessageBoxButtons.OK, MessageBoxIcon.Information);            
        }

        private void butSaveFiles_Click(object sender, EventArgs e)
        {
            if (FilesSaveClick != null) FilesSaveClick(this, EventArgs.Empty);
        }

        private void fldEnglishContent_TextChanged(object sender, EventArgs e)
        {
            if (EnglishContentChanged != null) EnglishContentChanged(this, EventArgs.Empty);
        }
        #endregion

        #region Interface ITextSplitForm
        //public string EnglishContent - source version get/set without arrays
        //{
        //    get { return fldEnglishContent.Text; }
        //    set { fldEnglishContent.Text = value; }
        //}

        public void SetSymbolCount(int[] count, int[] filesToDo)
        {
            for (int i = 0; i < filesQuantity; i++)
            {
                if (filesToDo[i] != 0)
                {
                    MessageBox.Show(count[i].ToString(), "SetSymbolCount", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    lblSymbolsCount[i].Text = count[i].ToString();
                }                
            }            
        }

        //public bool WasEnglishContentChange
        //{
        //    set { wasEnglishContentChange = value; }
        //}


        public event EventHandler FilesOpenClick;
        public event EventHandler FilesSaveClick;
        public event EventHandler EnglishContentChanged;
        //public event EventHandler TextSplitFormClosing;
        public event EventHandler<FormClosingEventArgs> TextSplitFormClosing;
        #endregion

        private void butSelectEnglishFile_Click(object sender, EventArgs e)//сразу открыть файлы?
        {
            //OpenFileDialog dlg = new OpenFileDialog();
            //dlg.Filter = "Text files|*.txt|All files|*.*";

            //if (dlg.ShowDialog() == DialogResult.OK)
            //{
            MessageBox.Show(FilesPath[0], "FilePathArray received", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fldEnglishFilePath.Text = FilesPath[0];
            MessageBox.Show(fldEnglishFilePath.Text, "EnglishFilePath received", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //if (FilesOpenClick != null)
            FilesOpenClick(this, EventArgs.Empty);            
        }

        private void numFont_ValueChanged(object sender, EventArgs e)
        {            
            {
                fldEnglishContent.Font = new Font("Calibri", (float)numFont.Value);
            }
        }
    }
}
