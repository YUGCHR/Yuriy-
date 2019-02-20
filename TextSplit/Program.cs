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
            MessageBox.Show("Please remember - the quantity of the working files must be declared in the Form", "Program in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TextSplitForm form = new TextSplitForm();
            MessageService service = new MessageService();
            FileManager manager = new FileManager();
            MessageBox.Show("All Modules Declared", "Program in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MainPresentor presentor = new MainPresentor(form, manager, service);
            MessageBox.Show("Main Called", "Program in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Run(form);
            MessageBox.Show("Run Form Started", "Program in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
