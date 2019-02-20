using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextSplit
{
    public interface IMessageService
    {
        void ShowMessage(string message);
        void ShowExclamation(string exclamation);
        void ShowError(string error);
        void ShowTrace(string tracePointNumber, string tracePointName);
    }
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

        public void ShowTrace(string tracePointNum, string tracePointNam)
        {
            string[] tracePointArray = { tracePointNum, tracePointNam };
            string tracePoint = String.Join(" - \r\n ", tracePointArray);
            MessageBox.Show(tracePoint, "Trace Point", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
