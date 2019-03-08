using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSplitLibrary
{
    public enum FormFieldsNames : int { fld0EnglishContent = 0, fld1RussianContent = 1, fld2ResultContent = 2 };

    public enum OpenFormFieldNames : int { fld0EnglishFilePath = 0, fld1RussianFilePath = 1, fld2ResultFilePath = 2, fld2CreateResultFileName = 3 };
    public enum OpenFormButtonNames : int { butOpenEnglishFile = 0, butOpenRussianFile = 1, butOpenResultFile = 2 };

    public enum OpenFormProgressStatusMessages : int { EnglishFilePathSelected = 0, RussianFilePathSelected = 1, ResultFilePathSelected = 2, ResultFileCreated = 3 };
    public enum OpenFormTextBoxImplementationMessages : int { EnglishFilePathSelected = 0, RussianFilePathSelected = 1, ResultFilePathSelected = 2, ResultFileCreated = 3 };

    public enum TableLanguagesContent : int { English = 0, Russian = 1, Result = 2 };

    //0 - Languages - cannot insert records
    //1 - Chapters - Columns - ID, ID_Language, int Chapter, nvchar10 Chapter_name
    //2 - Paragraphs - Columns - ID, ID_Language, ID_Chapter, int Paragraph, nvchar10 Paragraph_name
    //3 - Sentences - Columns - ID, ID_Language, ID_Chapter, ID_Paragraph, int Sentence, ntext Sentence_name
    public enum TablesNamesNumbers : int { Languages = 0, Chapters = 1, Paragraphs = 2, Sentences = 3 };
}
