using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSplitLibrary
{
    public enum FormFieldsNames : int { fld0EnglishContent = 0, fld1RussianContent = 1, fld2ResultContent = 2 };
    public enum OpenFormFieldNames : int { fld0EnglishFilePath = 0, fld1RussianFilePath = 1, fld2ResultFilePath = 2, fld2CreateResultFileName = 3 };
    public enum OpenFormButtonNames : int { butSelectEnglishFile = 0, butSelectRussianFile = 1, butSelectResultFile = 2 };
}
