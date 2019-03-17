using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSplitLibrary
{
    public static class Declaration
    {
        public const int FilesQuantity = 3; //the length of the all Files___ arrays (except FilesToDo)
        public const int ToDoQuantity = FilesQuantity + 1; //the length of the FilesToDo array (+1 for BreakpointManager)
        public const int FilesQuantityPlus = FilesQuantity + 1; //the length of the FilesToDo array (+1 for BreakpointManager)
        public const int ResultFileNumber = FilesQuantity - 1;
        public const int TextFieldsQuantity = 2;//количество текстовых окон в форме OpenForm (используется в Main - в isFilesExistCheckAndOpen)
        public const int ButtonNamesCountInLanguageGroup = 2;//количество названий для одной кнопки - перенести на прямое измерение массива или enum
        public const int IBreakpointManager = FilesQuantityPlus - 1; //index of BreakpointManager in the FilesToDo array
        public const string StrCRLF = "\r\n";
        public const string ResultFileName = "sampleResultTextDoc";
        public const int ShowMessagesLevel = 0;//0 - no messages, 1 - Trace messages, 2 - Value messages, -1 - Print mesaages...

        //Database structure and constants
        //0 - Languages - cannot insert records (ID, ID_Language, nvchar10 Language_name)
        //1 - Chapters - Columns - ID, ID_Language, int Chapter, nvchar10 Chapter_name 
        //2 - Paragraphs - Columns - ID, ID_Language, ID_Chapter, int Paragraph, nvchar10 Paragraph_name
        //3 - Sentences - Columns - ID, ID_Language, ID_Chapter, ID_Paragraph, int Sentence, ntext Sentence_name
        public static readonly string[] DataBaseTableNames = new string[] { "Languages", "Chapters", "Paragraphs", "Sentences" };

    }

    public enum ButtonName : int { OpenFile = 0, SaveFile = 1, AnalyseText = 2, SelectChapterName = 3, Reserved = 4 };

    public enum TextBoxImplementationMessages : int { EnglishFilePathSelected = 0, RussianFilePathSelected = 1, ResultFilePathSelected = 2, ResultFileCreated = 3 };

    public enum TablesNamesNumbers : int { Languages = 0, Chapters = 1, Paragraphs = 2, Sentences = 3 }; 

    public enum TableLanguagesContent : int { English = 0, Russian = 1, Result = 2 };

    public enum WhatNeedDoWithFiles : int
    {
        PassThrough = 0,
        ReadFileFirst = 1,
        ContentChanged = 2,
        AnalyseText = 3,
        CountSymbols = 4,
        CountSentences = 5,
        WittingIncomplete = 7,
        ContinueProcessing = 8,
        StopProcessing = 9,
        Reserved10 = 10,
        SelectChapterName = 11,
        Reserved12 = 12,
        AnalyseChapterName = 13
    };

    public enum WhatNeedSaveFiles : int
    {
        PassThrough = 0,
        SaveFileFirst = 1,        
        SaveFile = 2,
        FileSavedSuccessfully = 3,
        CannotSaveFile = 4,
        Reserved5 = 5,
        Reserved6 = 6,
        WittingIncomplete = 7,
        ContinueProcessing = 8,
        StopProcessing = 9
    };

    //public enum SavingResults : int
    //{
    //    PassThrough = 0,
    //    FileSavedSuccessfully = 1,
    //    CannotRead = 2,
    //    Reserved3 = 3,
    //    CannotWrite = 4,
    //    Reserved4 = 5,
    //    WittingIncomplete = 6,
    //    ContinueProcessing = 6,
    //    StopProcessing = 8,
    //};

    public enum WhatNeedDoWithTables : int
    {
        PassThrough = 0,
        ReadRecord = 1,
        ReadAllRecord = 2,
        Reserved3 = 3,
        InsertRecord = 4,
        DeleteRecord = 5,
        ClearTable = 7,
        ContinueProcessing = 8,
        StopProcessing = 9
    };

    public enum TablesProcessingResult : int
    {
        Successfully = 0,
        CannotRead = 1,
        Reserved2 = 2,
        CannotInsert = 3,
        UsingWrongTableName = 4
    };
}


