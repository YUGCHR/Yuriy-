using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TextSplitLibrary;
using System.Windows.Forms;

namespace TextSplit
{
    public interface ITextSplitOpenForm
    {
        int SetFileContent(int theAffectedElementNumber);
        int SetSymbolCount(int i);        
        int ChangeOnButtonText(int placeButton, int textButton, bool flagButtonEnable);
        int setStatusBottomLabel(int j, string textForStatusBottomLabel);        

        event EventHandler ContentChanged;
        event EventHandler OpenFileClick;
        event EventHandler SaveFileClick;
        event EventHandler TimeToAnalyseTextBook;
        event EventHandler LoadEnglishToDataBase;
        
        //event EventHandler<FormClosingEventArgs> TextSplitOpenFormClosing;
    }

    public partial class TextSplitOpenForm : Form, ITextSplitOpenForm
    {
        private readonly ISharedDataAccess _book;
        private readonly IMessageService _messageService;

        private Label[] lblSymbolsCount;
        private TextBox[] txtBoxFilesPath;
        private TextBox[] txtBoxFilesContent;
        private Button[] butMF;
        private string[,] buttonDuty;
        //private string[,] butMfOpenSaveLanguageNames;

        readonly private int buttonLanguages;
        readonly private int buttonTextsQuantity;
        readonly private int textFieldsQuantity;
        readonly private int buttonNamesCountInLanguageGroup;
        readonly private int showMessagesLevel;
        readonly private string strCRLF;       

        public event EventHandler ContentChanged;
        public event EventHandler OpenFileClick;
        public event EventHandler SaveFileClick;
        public event EventHandler TimeToAnalyseTextBook;
        public event EventHandler LoadEnglishToDataBase;        

        //public event EventHandler<FormClosingEventArgs> TextSplitOpenFormClosing;
        
        public enum FieldNames : int { fld0EnglishContent = 0, fld1RussianContent = 1, fld2ResultContent = 2 };

        //enum ButtonNames must be the same as butMF = new Button[]
        //длина enum LeftButtons используется в переменной buttonLanguages 
        public enum LeftButtons : int { butLeftTextBoxLeftSide = 0, butRightTextBoxLeftSide = 1 };// LeftButtons - это левый кнопки в обеих окных, т.е. 0 - Endlish, 1 - Russian
        public enum RightButtons : int { butLeftTextBoxRightSide = 0, butRightTextBoxRightSide = 1 };//RightButtons - это кнопки с правой стороны обеих окон, т.е. 0 - Endlish, 1 - Russian
        //индекс для массива имен кнопок
        public enum ButtonPlace : int { EnglishFile = 0, RussianFile = 1, EnglishText = 2, RussianText = 3, Reserved = 4 };//смысл, что номера с File - кнопки для работы с файлами, Text - с текстами

        public enum ProgressStatusMessages : int { EnglishFilePathSelected = 0, RussianFilePathSelected = 1, ResultFilePathSelected = 2, ResultFileCreated = 3 };        

        public TextSplitOpenForm(ISharedDataAccess book, IMessageService service)
        {
            _book = book;
            _messageService = service;

            textFieldsQuantity = DConst.TextFieldsQuantity;//количество текстовых окон - пока глобальное (нужно в Main)
            buttonLanguages = Enum.GetNames(typeof(LeftButtons)).Length; //количество языков на кнопках
            buttonTextsQuantity = 2 + 2;//количество вариантов текста на кнопках под текстовыми окнами
            buttonNamesCountInLanguageGroup = DConst.ButtonNamesCountInLanguageGroup;
            showMessagesLevel = DConst.ShowMessagesLevel;
            strCRLF = DConst.StrCRLF;

            InitializeComponent();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "InitializeComponent in TextSplitOpenForm", CurrentClassName, showMessagesLevel);

            lblSymbolsCount = new Label[] { lblSymbolCount1, lblSymbolCount2 };//, lblSymbolCount3 };
            txtBoxFilesPath = new TextBox[] { fld0EnglishFilePath, fld1RussianFilePath };//, fld2ResultFilePath  };
            txtBoxFilesContent = new TextBox[] { fld0EnglishContent, fld1RussianContent };//, fld2ResultFileContent  };
            butMF = new Button[] { butLeftTextBoxLeftSide, butRightTextBoxLeftSide, butLeftTextBoxRightSide, butRightTextBoxRightSide };

            buttonDuty = new string[,]
            { { "Open English File", "Save English File", "Analyse English Text", "Select Chapter Name" },
                { "Open Russian File", "Save Russian File", "Analyse Russian Text", "Select Chapter Name" }, };

            butMF[(int)ButtonPlace.EnglishFile].Click += butMF_Click;
            butMF[(int)ButtonPlace.RussianFile].Click += butMF_Click;
            butMF[(int)ButtonPlace.EnglishText].Click += butMF_Click;
            butMF[(int)ButtonPlace.RussianText].Click += butMF_Click;

            txtBoxFilesContent[(int)ButtonPlace.EnglishFile].TextChanged += fldContent_TextChanged;//потом, возможно, поменять English/Russian на Left/Right
            txtBoxFilesContent[(int)ButtonPlace.RussianFile].TextChanged += fldContent_TextChanged;
            
            numEnglishFont.SelectedIndexChanged += numEnglishFont_SelectedIndexChanged;

            //butCreateResultFile.Click += butSelectEnglishFile_Click; it needs to clear up
            //fld2CreateResultFileName - result file name field
            //fld2ResultContent.TextChanged += fldContent_TextChanged;

            FormClosing += TextSplitOpenForm_FormClosing;

            //- первоначальная установка названий кнопок на старте            
            int[] textButton = new int[] { (int)ButtonName.OpenFile, (int)ButtonName.OpenFile, (int)ButtonName.AnalyseText, (int)ButtonName.AnalyseText }; // the second index for buttonDuty = new string[,] 
            bool[] flagButtonEnable = new bool[] { true, true, false, false };
            int[] placeButton = (int[]) Enum.GetValues(typeof(ButtonPlace));
            ButtonTextsOnStart(placeButton, textButton, flagButtonEnable);
        }

        private void ButtonTextsOnStart(int[] placeButton, int[] textButton, bool[] flagButtonEnable)
        {
            for(int i = 0; i < buttonTextsQuantity; i++)
            {
                int changeOnButtonTextResult = ChangeOnButtonText(placeButton[i], textButton[i], flagButtonEnable[i]);
            }
        }        

        public int ChangeOnButtonText(int placeButton, int textButton, bool flagButtonEnable)//mfButtonPlace - 0 - English, 1 -Russian, mfButtonText - 0 - Open, 1 - Save (or Analyse/Load)
        {
            //butLeftTextBoxLeftSide = 0/Endlish, butRightTextBoxLeftSide = 1/Russian
            //butLeftTextBoxRightSide = 2/Endlish, butRightTextBoxRightSide = 3/Russian
            //placeButton = EnglishFile = 0, RussianFile = 1, EnglishText = 2, RussianText = 3, Reserved = 4
            //               mfButtonPlace 
            //ButtonText - Open English File, Save English File, Analyse English Text, English Text to dB
            //           - Open Russian File, Save Russian File, Analyse Russian Text, Russian Text to dB
            int placeGroupButtons = 0;

            // выделили номер языка из места кнопки в placeGroupButtons - 0/Endlish, 1/Russian
            if (placeButton < buttonLanguages) placeGroupButtons = placeButton;
            else placeGroupButtons = placeButton - buttonNamesCountInLanguageGroup;
            
            if (textButton < buttonTextsQuantity) 
            {
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                    "on the button " + butMF[placeButton].Name + strCRLF +
                    "the text is " + butMF[placeButton].Text + strCRLF +
                    " set name " + buttonDuty[placeGroupButtons, textButton] + strCRLF +
                    " enabled = " + flagButtonEnable, CurrentClassName, showMessagesLevel);

                if (butMF[placeButton].Text != buttonDuty[placeGroupButtons, textButton])//если название надо менять, то меняем
                {
                    butMF[placeButton].Text = buttonDuty[placeGroupButtons, textButton];//установили требуемое название на нужной кнопке
                }
                butMF[placeButton].Enabled = flagButtonEnable;//сделали кнопку Enabled или нет

                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                    "on the button " + butMF[placeButton].Name + strCRLF +
                    " set name " + buttonDuty[placeGroupButtons, textButton] + strCRLF +
                    " enabled = " + flagButtonEnable, CurrentClassName, showMessagesLevel);

                return placeButton + textButton;
            }
            else
            {
                _messageService.ShowExclamation("Your requests exceed reasonable limits!");                
                return -1;
            }
        }

        private void ButEnglishToDataBase_Click(object sender, EventArgs e)//выбирать кнопки из массива
        {
            if (LoadEnglishToDataBase != null) LoadEnglishToDataBase(this, EventArgs.Empty);
        }

        private void TextSplitOpenForm_FormClosing(object sender, FormClosingEventArgs e)//it needs to clear up
        {            
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Start", CurrentClassName, showMessagesLevel);
            //TextSplitOpenFormClosing(this, e);            
            //e.Cancel = wasEnglishContentChange;
            System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.AppendFormat("{0} = {1}", "CloseReason", e.CloseReason);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Cancel", e.Cancel);
            messageBoxCS.AppendLine();
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString() + "FormClosing Event", messageBoxCS.ToString(), CurrentClassName, showMessagesLevel);            
        }

        private void butMF_Click(object sender, EventArgs e)     // алгоритм выбора действия по нажатым кнопкам
        {                                                           // 
            Button button = sender as Button;                       // кнопка нажата
            string ButtonName = button.Name;                        // получили место нажатой кнопки
            string ButtonText = button.Text;                        // получили имя нажатой кнопки 
            
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(),
                    "on the button ==> " + ButtonName + strCRLF +
                    " with the text ==> " + ButtonText, CurrentClassName, showMessagesLevel);

            int buttonPlace = ButClickLabelNow(ButtonName, ButtonText);      // вызвали обработчик
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "здесь надо остановиться", CurrentClassName, showMessagesLevel);
            //тут пока окончание цепочки обработки нажатия кнопок
        }        

        private int ButClickLabelNow(string buttonName, string buttonText)// алгоритм выбора метода по нажатым кнопкам
        {                                                           
            string currentLeftButtonPlace = "";
            string currentRightButtonPlace = "";
            string currentButtonName = "";
            
            var listButtonMethods = new List<Action<int>>();            
            listButtonMethods.Add(PathForOpenFile);       // listOpenFormButtonMethods[0]            
            listButtonMethods.Add(SaveFileFromTextBox);   // listOpenFormButtonMethods[1]            
            listButtonMethods.Add(AnalyseTextBook);       // listOpenFormButtonMethods[2]            
            listButtonMethods.Add(UserSelectChapterName); // listOpenFormButtonMethods[3]

            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " buttonLanguages = " + buttonLanguages.ToString(), CurrentClassName, showMessagesLevel);
            for (int i = 0; i < buttonLanguages; i++)//0 - Left-Left, 1 - Right-Left, 2 - Left-Right, 3 - Right-Right
            {                                                         // цикл 1 начат                
                currentLeftButtonPlace = Enum.GetNames(typeof(LeftButtons))[i];  // фактически i - индекс языка
                currentRightButtonPlace = Enum.GetNames(typeof(RightButtons))[i];// две переменные для определения, кнопка с каким языком была нажата 

                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " i = " + i.ToString() + strCRLF + 
                    " buttonName ==> " + buttonName + strCRLF +
                    " currentLeftButtonPlace ==> " + currentLeftButtonPlace + strCRLF +
                    " currentRightButtonPlace ==> " + currentRightButtonPlace + strCRLF, CurrentClassName, showMessagesLevel);

                if (buttonName == currentLeftButtonPlace || buttonName == currentRightButtonPlace) // если совпало с ЯЗЫКОМ нажатой кнопки - начали цикл 2, если нет - проход 2 цикла 1 с другим языком
                { 
                    for (int j = 0; j < buttonTextsQuantity; j++)     // выбираем (номер) метода для обработки
                    {                                                 // цикл 2 начат                
                        currentButtonName = buttonDuty[i, j]; // проход 0 цикла 2 - выбрали имя Open для проверки имени кнопки 
                                                                      // проход 1 цикла 2 - выбрали имя Save для проверки имени кнопки, проход 2 - Analise и 4 - LoadToBase                                                                         
                        _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                            " i = " + i.ToString() + strCRLF  + 
                            " j = " + j.ToString() + strCRLF + 
                            " Button Name ==> " + currentButtonName, CurrentClassName, showMessagesLevel);

                        if (buttonText == currentButtonName)  // если имя нажатой кнопки совпало с именем из двумерного массива, то - 
                        {                  //[j] - выбираем метод из массива List методов, а (i) - аргумент, передаваемый в метод (фактически язык 0 - Englis, 1 - Russian)
                            listButtonMethods[j](i);//только тут мы знаем i - чтобы передать его в Main, заносим в методах в FilesToDo[i] или в FilesToSave[i]
                            return i;//возвращаем что-нибудь - возможно признак завершения работы при неудачном Save
                        }
                    }
                }
            }
            return -1;//means encounter - все пропало
        }

        private void PathForOpenFile(int j) // listOpenFormButtonMethods[0] - нажата одна из кнопок и на ней было написано Open, j хранит язык кнопки (0 - English)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Method2 PathForOpenFile fetched j = " + j.ToString(), CurrentClassName, showMessagesLevel);
            string currentFilePath = "";
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text files|*.txt|All files|*.*";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                currentFilePath = dlg.FileName;
                _book.SetFilePath(currentFilePath, j);
                _book.SetFileToDo((int)WhatNeedDoWithFiles.ReadFileFirst, j); // записали в нужный элемент ToDo значение, что надо открывать файл
            }
            int setResult = setStatusBottomLabel(j, currentFilePath);
            if (OpenFileClick != null) OpenFileClick(this, EventArgs.Empty);//вызываем Main (сказать поменять надписи на кнопках - показать погасший Save)
        }

        private void SaveFileFromTextBox(int j) // listOpenFormButtonMethods[1] - нажата одна из кнопок и на ней было написано Save, j хранит язык кнопки (0 - English)
        {//определить, первый раз сохраняют или нет - по признаку FileWasSavedSuccessfully и поставить указание - либо SaveFileFirst, либо SaveFile
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Method2 SaveFileFromTextBox fetched j = " + j.ToString(), CurrentClassName, showMessagesLevel);
            if (_book.GetFileToSave(j) == (int)WhatNeedSaveFiles.FileSavedSuccessfully) // проверяем значение индикатора, что файл уже сохранялся
            {
                int test = _book.SetFileToSave((int)WhatNeedSaveFiles.SaveFile, j);//молча сохраняем файл (в Main)
            }
            else
            {
                int test = _book.SetFileToSave((int)WhatNeedSaveFiles.SaveFileFirst, j);//спрашиваем о перезаписи (в Main)
            }
            int setResult = setStatusBottomLabel(j, _book.GetFilePath(j));//Set the short type of current action in the status bar
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " FilesToSave[j] = " + _book.GetFileToSave(j).ToString(), CurrentClassName, showMessagesLevel);
            if (SaveFileClick != null) SaveFileClick(this, EventArgs.Empty);
            // тут проверить индикатор, сохранилось успешно или появился признак завершения работы?
            // проще сразу же воззвать к событию закрытия формы в Main
        }

        public int setStatusBottomLabel(int j, string textForStatusBottomLabel)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " setStatusBottomLabel with j = " + j.ToString() + strCRLF +
                " set to Status the following text = " + textForStatusBottomLabel, CurrentClassName, showMessagesLevel);
            txtBoxFilesPath[j].Text = textForStatusBottomLabel; //записали путь и имя файла в поле над текстом файла
            statusBottomLabel.Text = Enum.GetNames(typeof(ProgressStatusMessages))[j] + " - " + textForStatusBottomLabel;//Set the short type of current action in the status bar            
            
            return j;
        }

        private void AnalyseTextBook(int j) // listOpenFormButtonMethods[2] - нажата одна из кнопок и на ней было написано Analyse, j хранит язык кнопки (0 - English)
        {
            int test = _book.SetFileToDo((int)WhatNeedDoWithFiles.AnalyseText, j);//сохраняем номер языка в ToDo
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Method3 AnalyseTextBook fetched j = " + j.ToString(), CurrentClassName, showMessagesLevel);
            if (TimeToAnalyseTextBook != null) TimeToAnalyseTextBook(this, EventArgs.Empty);
        }

        private void UserSelectChapterName(int j) // listOpenFormButtonMethods[3] - нажата одна из кнопок и на ней было написано Load, i хранит язык кнопки (0 - English)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Method4 UserSelectChapterName fetched j = " + j.ToString(), CurrentClassName, showMessagesLevel);
            //получить выделенный пользователем текст - предположительно имя главы
            TextBox currentTexBox = txtBoxFilesContent[j];
            string userSelectedText = currentTexBox.SelectedText;
            if (userSelectedText.Length > 0)
            {
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "In the TextBox " + currentTexBox.Name + strCRLF +
                "User Selected the following Text ==> " + userSelectedText, CurrentClassName, showMessagesLevel);
            
                int test = _book.SetSelectedText(userSelectedText, j);//сохраняем выбранный пользователем фрагмент
                test = _book.SetFileToDo((int)WhatNeedDoWithFiles.AnalyseChapterName, j);//сохраняем номер языка в ToDo
                int placeButton = j + buttonNamesCountInLanguageGroup; // Place of the button the same as changed field (English or Russian) - смещаем выбор на соседнюю кнопку в том же языке (окне)
                ChangeOnButtonText(placeButton, (int)ButtonName.AnalyseText, false);
                if (TimeToAnalyseTextBook != null) TimeToAnalyseTextBook(this, EventArgs.Empty);
            }
            else _messageService.ShowExclamation("Please, select any Chapter name with number in the text!");
        }            

        private void fldContent_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string TextBoxName = textBox.Name;
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), "Text Changed in TextBox - " + TextBoxName, CurrentClassName, showMessagesLevel);

            for (int i = 0; i < textFieldsQuantity; i++)
            {
                string currentFormFieldName = Enum.GetNames(typeof(FieldNames))[i];

                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), strCRLF +
                    "TextBoxName " + strCRLF +
                     TextBoxName + " must be == " + currentFormFieldName + strCRLF +
                    "currentFormFieldName /i = " + i.ToString(), CurrentClassName, showMessagesLevel);

                if (TextBoxName == currentFormFieldName)
                {
                    int setToDoResult = _book.SetFileToDo((int)WhatNeedDoWithFiles.ContentChanged, i); 
                    _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), " FilesToDo[i] - " + _book.GetFileToDo(i).ToString() + strCRLF + 
                        "sent to setToDoResult = " + setToDoResult, CurrentClassName, showMessagesLevel);
                    int setContentResult = _book.SetFileContent(textBox.Text, i);
                }
            }
            
            if (ContentChanged != null) ContentChanged(this, EventArgs.Empty);
        }

        public int SetFileContent(int theAffectedElementNumber)
        {
            txtBoxFilesContent[theAffectedElementNumber].Text = _book.GetFileContent(theAffectedElementNumber);
            numEnglishFont_FirstSet(theAffectedElementNumber);
            return (int)WhatNeedSaveFiles.FileSavedSuccessfully;
        }

        public int SetSymbolCount(int i)
        {
            _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), 
                " SetSymbolCount fetched i = " + i.ToString() + strCRLF +
                "_book.GetFileToDo(i) (must be 4) = " + _book.GetFileToDo(i).ToString(), CurrentClassName, showMessagesLevel);

            if (_book.GetFileToDo(i) == (int)WhatNeedDoWithFiles.CountSymbols)
            {
                _messageService.ShowTrace(MethodBase.GetCurrentMethod().ToString(), 
                    "lblSymbolsCount[i] ==> " + lblSymbolsCount[i].Name + strCRLF +
                    "SymbolsCount(i) = " + _book.GetSymbolsCount(i).ToString(), CurrentClassName, showMessagesLevel);
                lblSymbolsCount[i].Text = _book.GetSymbolsCount(i);
                return (int)MethodFindResult.AllRight;
            }
            return (int)MethodFindResult.NothingFound;
        }        

        private void numEnglishFont_FirstSet(int theAffectedElementNumber)
        {
            var font = txtBoxFilesContent[theAffectedElementNumber].Font;            
            numEnglishFont.SelectedIndex = Convert.ToInt32(font.Size);
        }

        private void numEnglishFont_SelectedIndexChanged(object sender, EventArgs e)//переделать в поля из массива
        {
            {
                fld0EnglishContent.Font = new Font("Tahoma", numEnglishFont.SelectedIndex);
            }
        } 

        public static string CurrentClassName
        {
            get { return MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
		
	}
}
