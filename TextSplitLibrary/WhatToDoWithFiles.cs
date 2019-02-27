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
}
