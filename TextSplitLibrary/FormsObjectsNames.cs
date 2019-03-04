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
}
