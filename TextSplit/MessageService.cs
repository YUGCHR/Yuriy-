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
                    //MessageBox.Show(tracePointMessage, tracePointPlace, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _manager.AppendContent(tracePointName, tracePointValue, tracePointPlace);
                }
            }
            
        }

        public void ShowTrace(string tracePointName, string[] tracePointValue, string tracePointPlace, int showLevel)
        {            
            if (showLevel != 0)
            {
                if (tracePointValue != null)
                {
                    int ii = tracePointValue.Length;
                    string tracePointMessage = "";
                    if (showLevel > 1)
                    {
                        for (int i = 0; i < ii; i++)
                        {
                            if (tracePointValue[i] != null)
                            {
                                string tracePointValueCut = "";
                                int count = 0;
                                foreach (char tracePointValueSymbol in tracePointValue[i])
                                {
                                    if (count < 100)
                                    {
                                        tracePointValueCut = tracePointValueCut + tracePointValueSymbol;
                                        count++;
                                    }
                                }
                                string[] tracePointArray = { tracePointMessage, tracePointValueCut };
                                tracePointMessage = String.Join("\r\n", tracePointArray);
                            }
                            
                        }
                        ShowTrace(tracePointName, tracePointMessage, tracePointPlace, showLevel);
                    }
                    else
                    {
                        MessageBox.Show(MethodBase.GetCurrentMethod().ToString() + " _manager.AppendContent will Call ", CurrentClassName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        _manager.AppendContent(tracePointName, tracePointValue, tracePointPlace);
                    }
                }
                

            }
        }

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}
