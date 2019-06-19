using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSplitLibrary
{
    public interface IMessageService
    {
        void ShowMessage(string message);
        void ShowExclamation(string exclamation);
        void ShowError(string error);
        void ShowTrace(string tracePointName, string tracePointValue, string tracePointPlace, int showLevel);
        void ShowTrace(string tracePointName, string[] tracePointValue, string tracePointPlace, int showLevel);
        string SaveTracedToFile(string tracedFilePathAndName, string tracedFileContent);
    }
}
