﻿using System;
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
            
            TextSplitOpenForm open = new TextSplitOpenForm();
            MessageService service = new MessageService();
            TextSplitForm form = new TextSplitForm(service, open);            
            FileManager manager = new FileManager(service, Declaration.LanguagesQuantity);
            MessageBox.Show("All Modules Declared", "Program in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MainPresentor presentor = new MainPresentor(form, open, manager, service);
            MessageBox.Show("Main Called", "Program in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Run(form);
            MessageBox.Show("Run Form Started", "Program in progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
