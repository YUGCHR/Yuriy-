using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextSplitLibrary;

namespace TextSplit
{    
    public class MessageService: IMessageService
    {
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
            if (showLevel > 0)
            {
                string[] tracePointArray = { tracePointName, tracePointValue };
                string tracePointMessage = String.Join(" - \r\n ", tracePointArray);
                MessageBox.Show(tracePointMessage, tracePointPlace, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        public void ShowTrace(string tracePointName, string[] tracePointValue, string tracePointPlace, int showLevel)
        {
            int ii = tracePointValue.Length;            
            string tracePointMessage = "";
            if (showLevel > 1)
            {
                for (int i = 0; i < ii; i++)
                {
                    string[] tracePointArray = { tracePointMessage, tracePointValue[i] };                    
                    tracePointMessage = String.Join("\r\n", tracePointArray);                    
                }
                ShowTrace(tracePointName, tracePointMessage, tracePointPlace, showLevel);                
            }

        }
    }
}
