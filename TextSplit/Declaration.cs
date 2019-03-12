using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSplit
{
    static class Declaration
    {
        public const int FilesQuantity = 3; //the length of the all Files___ arrays (except FilesToDo)
        public const int ToDoQuantity = FilesQuantity + 1; //the length of the FilesToDo array (+1 for BreakpointManager)
        public const int FilesQuantityPlus = FilesQuantity + 1; //the length of the FilesToDo array (+1 for BreakpointManager)
        public const int ResultFileNumber = FilesQuantity - 1;
        public const int TextFieldsQuantity = FilesQuantity - 1;////quantity of the text fields in the Open Form
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
    

    //0 - Languages - cannot insert records
    //1 - Chapters - Columns - ID, ID_Language, int Chapter, nvchar10 Chapter_name
    //2 - Paragraphs - Columns - ID, ID_Language, ID_Chapter, int Paragraph, nvchar10 Paragraph_name
    //3 - Sentences - Columns - ID, ID_Language, ID_Chapter, ID_Paragraph, int Sentence, ntext Sentence_name
    public enum TablesNamesNumbers : int { Languages = 0, Chapters = 1, Paragraphs = 2, Sentences = 3 };
}


