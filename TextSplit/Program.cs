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
            MessageBox.Show("static void Main started", "Program in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FileManager manager = new FileManager(Declaration.LanguagesQuantity);
            MessageService service = new MessageService(manager);
            TextSplitOpenForm open = new TextSplitOpenForm(service);
            TextSplitForm form = new TextSplitForm(service, open);            
            MessageBox.Show("All Modules Declared", "Program in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MainPresentor presentor = new MainPresentor(form, open, manager, service);            
            MessageBox.Show("Main Called", "Program in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Run(form);            
        }
    }
}
