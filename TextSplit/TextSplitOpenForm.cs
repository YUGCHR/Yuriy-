﻿using System;
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
        public string[] FilesPath;// { get; set; }        
        public int[] FilesToDo;// { get; set; }

        readonly private int filesQuantity;
        readonly private int filesQuantityPlus;
        readonly private int iBreakpointManager;
        private int showMessagesLevel;

        public event EventHandler AllOpenFilesClick;
        //public event EventHandler FormOpenClick;
        //public event EventHandler<FormClosingEventArgs> TextSplitOpenFormClosing;

        public TextSplitOpenForm(IMessageService service)
        {
            _messageService = service;
            filesQuantity = Declaration.FilesQuantity; //the length of the all Files___ arrays (except FilesToDo)
            filesQuantityPlus = Declaration.ToDoQuantity; //the length of the FilesToDo array (+1 for BreakpointManager)
            iBreakpointManager = filesQuantityPlus-1; //index of BreakpointManager in the FilesToDo array
            showMessagesLevel = Declaration.ShowMessagesLevel;

            InitializeComponent();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "InitializeComponent", CurrentClassName, showMessagesLevel);

            FilesToDo = new int[filesQuantityPlus];
            FilesPath = new string[filesQuantity];
            //FilesContent = new string[filesQuantity];

            butAllFilesOpen.Click += new EventHandler (butAllFilesOpen_Click);
            butSelectEnglishFile.Click += butSelectEnglishFile_Click;
            butSelectRussianFile.Click += ButSelectRussianFile_Click;
            butSelectResultFile.Click += ButSelectResultFile_Click;
            butCreateResultFile.Click += butSelectEnglishFile_Click;
            FormClosing += TextSplitOpenForm_FormClosing;
            
            //fld2CreateResultFileName - result file name field
        }

        public string[] GetFilesPath()
        {
            FilesPath[0] = fld0EnglishFilePath.Text;
            FilesPath[1] = fld1RussianFilePath.Text;
            FilesPath[2] = fld2ResultFilePath.Text;
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " FilesPath[] = ", FilesPath, CurrentClassName, showMessagesLevel);
            return FilesPath;
        }

        public void SetFilesToDo(int[] filesToDo)
        {
            FilesToDo = filesToDo;
        }

        private void TextSplitOpenForm_FormClosing(object sender, FormClosingEventArgs e)
        {            
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Start", CurrentClassName, showMessagesLevel);
            //TextSplitOpenFormClosing(this, e);
            //MessageBox.Show(wasEnglishContentChange.ToString(), "We here again - last step", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Start", CurrentClassName, showMessagesLevel);       
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " fldEnglishFilePath = ", fld0EnglishFilePath.Text, CurrentClassName, showMessagesLevel);
            FilesPath[0] = fld0EnglishFilePath.Text;            
            FilesPath[1] = fld1RussianFilePath.Text;            
            FilesPath[2] = fld2ResultFilePath.Text;
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " FilesPath[] = ", FilesPath, CurrentClassName, showMessagesLevel);
            
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " FilesOpenClick = ", AllOpenFilesClick.ToString(), CurrentClassName, showMessagesLevel);
            if (AllOpenFilesClick != null) AllOpenFilesClick(this, EventArgs.Empty);
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " iBreakpointManager = " + iBreakpointManager.ToString(), CurrentClassName, 3);
            int BreakpointManager = FilesToDo[iBreakpointManager];
            if (BreakpointManager != (int)WhatNeedDoWithFiles.StopProcessing) this.Close();            
            //_messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "taki.Closed", CurrentClassName, showMessagesLevel);
            //this.parentForm.FilePath[1] = fldRussianFilePath.Text;
            //this.parentForm.FilePath[2] = fldResultFilePath.Text;
        }
        
        //public void FormOpenClick()
        //{

        //}

        //public void ManageFilesContent(string[] filesPath, string[] filesContent, int[] filesToDo)
        //{
        //    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " receiver filePath[] = ", filesPath, CurrentClassName, showMessagesLevel);
        //    FilesPath = filesPath;
        //    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " set FilePath[] = ", FilesPath, CurrentClassName, showMessagesLevel);
        //    FilesContent = filesContent;
        //    FilesToDo = filesToDo;
        //}

        private void butSelectEnglishFile_Click(object sender, EventArgs e)
        {            
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " FilePath[] = ", FilesPath, CurrentClassName, showMessagesLevel);
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files|*.txt|All files|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                fld0EnglishFilePath.Text = dlg.FileName;

                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " fldEnglishFilePath = ", fld0EnglishFilePath.Text, CurrentClassName, showMessagesLevel);                
            }
        }

        private void ButSelectRussianFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files|*.txt|All files|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                fld1RussianFilePath.Text = dlg.FileName;
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " fldRussianFilePath = ", fld1RussianFilePath.Text, CurrentClassName, showMessagesLevel);
            }

        }

        private void ButSelectResultFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files|*.txt|All files|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                fld2ResultFilePath.Text = dlg.FileName;
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + " fldResultFilePath = ", fld2ResultFilePath.Text, CurrentClassName, showMessagesLevel);
            }
        }       

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}
