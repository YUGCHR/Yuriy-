using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;
using TextSplitLibrary;

namespace TextSplit
{
    public class MessageService : IMessageService
    {
        private readonly IFileManager _manager;

        public MessageService(IFileManager manager)
        {
            _manager = manager;
        }
            
        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowExclamation(string exclamation)
        {
            MessageBox.Show(exclamation, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        public void ShowError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowTrace(string tracePointName, string tracePointValue, string tracePointPlace, int showLevel)
        {            
            if (showLevel != 0)
            {                
                if (showLevel > 0)
                {
                    string[] tracePointArray = { tracePointName, tracePointValue };
                    string tracePointMessage = String.Join(" - \r\n ", tracePointArray);
                    MessageBox.Show(tracePointMessage, tracePointPlace, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {                    
                    _manager.AppendContent(tracePointName, tracePointValue, tracePointPlace);
                }
            }            
        }

        public string SaveTracedToFile(string tracedFileNameAddition, string tracedFileContent)
        {
            int i = 0;
            string tracedFilePathAndName = "";
            tracedFilePathAndName = tracedFileNameAddition;
            bool truePath = _manager.IsFileExist(tracedFilePathAndName);

            string pasHash = _manager.GetMd5Hash(tracedFileContent);
            int result =  _manager.SaveContent(tracedFilePathAndName, tracedFileContent, i);//i - no need yet

            return pasHash;
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}
