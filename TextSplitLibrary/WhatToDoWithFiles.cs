using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSplitLibrary
{    
    public enum WhatNeedDoWithFiles : int
    {
        PassThrough = 0,
        ReadFirst = 1,
        ContentChanged = 2,
        Write = 3,
        CountSymbols = 4,
        CountSentences = 5,
        WittingIncomplete = 7,
        ContinueProcessing = 8,
        StopProcessing = 9
    };

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
