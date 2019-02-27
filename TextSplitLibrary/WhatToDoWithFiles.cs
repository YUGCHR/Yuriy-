using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSplitLibrary
{    
    public enum WhatNeedDoWithFiles : int { PassThrough, ReadFirst, ContentChanged, Write, CountSymbols, CountSentences, StopProcessing, ContinueProcessing };    
}
