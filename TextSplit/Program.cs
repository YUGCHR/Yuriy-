using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TextSplitLibrary;

namespace TextSplit
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TextSplitForm form = new TextSplitForm();
            MessageService service = new MessageService();
            FileManager manager = new FileManager();

            MainPresentor presentor = new MainPresentor(form, manager, service);

            Application.Run(form);

        }
    }
}
