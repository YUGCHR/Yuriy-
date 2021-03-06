﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextSplitLibrary;

namespace TextSplit
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //MessageBox.Show("static void Main started", "Program", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SharedDataAccess bookData = new SharedDataAccess();
            FileManager fileManager = new FileManager(bookData);
            LogFileMessages logFile = new LogFileMessages();
            MessageService msgService = new MessageService(fileManager);
            DataBaseAccessor dataAccess = new DataBaseAccessor(msgService);
            TextSplitForm viewForm = new TextSplitForm(msgService, logFile);
            TextSplitOpenForm openForm = new TextSplitOpenForm(bookData, msgService);

            AnalysisDataConstant<int[]> totalDigitsQuantity = new AnalysisDataConstant<int[]>();
            AnalysisDataConstant<string[]> SentenceSeparators = new AnalysisDataConstant<string[]>();

            TextAnalysisLogicExtension analysisLogic = new TextAnalysisLogicExtension(bookData, msgService);
            ChapterDividingAnalysis chapterLogic = new ChapterDividingAnalysis(bookData, msgService, analysisLogic);
            SentencesDividingAnalysis sentenceLogic = new SentencesDividingAnalysis(bookData, msgService, analysisLogic);
            noneAnalysisLogicParagraph paragraphLogic = new noneAnalysisLogicParagraph(bookData, msgService, analysisLogic);
            AllBookAnalysis analysisBook = new AllBookAnalysis(bookData, msgService, analysisLogic, chapterLogic, sentenceLogic);
            LoadTextToDataBase loadDataBase = new LoadTextToDataBase(bookData, dataAccess, msgService);
            MainLogicCultivation logicMain = new MainLogicCultivation(bookData, msgService, fileManager);
            MainPresentor presentorMain = new MainPresentor(viewForm, openForm, msgService, analysisBook, loadDataBase, bookData, logicMain);
            
            //MessageBox.Show("All Modules Declared", "Program", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //MessageBox.Show("Main Called", "Program", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Application.Run(viewForm);            
        }
    }
}
//Legacy code from all modules
//filesToDo[i] = isFilesExist[i] ? (int)WhatNeedDoWithFiles.ReadFirst : (int)WhatNeedDoWithFiles.PassThrough;//If PassThrough - say necessary file does not exist!
//Array.Clear(FilesToDo, 0, FilesToDo.Length);
//string[] currentFormFieldNames = Enum.GetNames(typeof(FormFieldNames));
//int i = Convert.ToInt32(TextBoxName.Substring(3, 1));//set apart the number of the textbox name - it is the 4-th symbol of the name
//public string EnglishContent - source version get/set without arrays
//{
//    get { return fldEnglishContent.Text; }
//    set { fldEnglishContent.Text = value; }
//}
//private void _open_TextSplitOpenFormClosing(object sender, FormClosingEventArgs e)
//{            
//    _messageService.ShowTrace("after TextSplitOpenFormClosing ", "(Closed)", CurrentClassName, showMessagesLevel);
//    //e.Cancel = wasEnglishContentChange;
//}
//
//private List<string> textParagraphs;
//textParagraphs = new List<string>();
//textParagraphs.Add(currentParagraph);//merge all Paragraphs in text to print to file (for test only)
//
//for (int i = 0; i < 20; i++)
//    Control currentOpenFormFieldName = this.Controls[15];
//
//string currentOpenFormFieldName = Enum.GetNames(typeof(OpenFormFieldNames))[i];                        
//
//OpenFileDialog dlg = new OpenFileDialog();
//    dlg.Filter = "Text files|*.txt|All files|*.*";
//    if (dlg.ShowDialog() == DialogResult.OK)
//        fld2ResultFilePath.Text = dlg.FileName;
//
//this.parentForm.FilePath[1] = fldRussianFilePath.Text;
//
//num.GetName(typeof(OpenFormProgressStatusMessages), i) + "\r\n" + LogBoxCurrentLineValue;
//
//MSSQLSERVER Server=localhost;Database=master;Trusted_Connection=True;
//
//