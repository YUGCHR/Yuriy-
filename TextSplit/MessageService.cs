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
            bool haveDone = false;
            if (showLevel != 0)
            {
                int ii = tracePointValue.Length;
                MessageBox.Show(MethodBase.GetCurrentMethod().ToString()+ " ii = " + ii.ToString(), CurrentClassName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                string tracePointMessage = "";
                if (showLevel > 1)
                {
                    for (int i = 0; i < ii; i++)
                    {
                        MessageBox.Show(MethodBase.GetCurrentMethod().ToString() + " i = " + i.ToString(), CurrentClassName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                        string tracePointValueCut = "";// = tracePointValue[i].Substring(0, 10);//It is need to add Substring to the non-array method
                        int count = 0;
                        char charStop = '.';
                        foreach (char tracePointValueSymbol in tracePointValue[i])
                        {
                            if (!haveDone) haveDone = tracePointValueSymbol.Equals(charStop) | (count > 100);
                            MessageBox.Show(MethodBase.GetCurrentMethod().ToString() + "tracePointValueSymbol - count => " + tracePointValueSymbol.ToString() + count.ToString(), CurrentClassName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            count++;
                            MessageBox.Show(MethodBase.GetCurrentMethod().ToString() + "haveDone => " + haveDone.ToString(), CurrentClassName, MessageBoxButtons.OK, MessageBoxIcon.Information);

                            bool haveDoneOff = Char.IsControl(tracePointValueSymbol) & haveDone;

                                if (haveDoneOff)
                                {
                                    MessageBox.Show(MethodBase.GetCurrentMethod().ToString() + " tracePointValueSymbol - EOL = " + tracePointValueSymbol.ToString(), CurrentClassName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    break;
                                }
                                else
                                {
                                    MessageBox.Show(MethodBase.GetCurrentMethod().ToString() + " tracePointValueCut = " + tracePointValueCut, CurrentClassName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    tracePointValueCut = tracePointValueCut + tracePointValueSymbol;
                                    MessageBox.Show(MethodBase.GetCurrentMethod().ToString() + " tracePointValueCut = " + tracePointValueCut, CurrentClassName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    continue;
                                }                                                       
                        }

                        MessageBox.Show(MethodBase.GetCurrentMethod().ToString() + " tracePointValueCut = " + tracePointValueCut, CurrentClassName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        string[] tracePointArray = { tracePointMessage, tracePointValueCut };
                        tracePointMessage = String.Join("\r\n", tracePointArray);
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

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }
    }
}
