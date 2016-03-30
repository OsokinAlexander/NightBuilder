using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace NightBuilder
{
    /// <summary>
    /// Форма создания/редактирования операции.
    /// </summary>
    public partial class OperationForm : Form
    {
        public Operation operation;

        public OperationForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Получить номер в выпадающем списке для типа операции
        /// </summary>
        /// <param name="type"> тип операции </param>
        /// <returns> номер </returns>
        private int GetOperationTypeIndex(Operation.OperationTypes type)
        {
            for (int i = 0; i < typeComboBox.Items.Count; i++ )
            {
                Operation.OperationTypes itemType = ((EnumWrapper<Operation.OperationTypes>)typeComboBox.Items[i]).GetConst();
                if (itemType == type)
                {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// Получить номер в выпадающем списке для типа реагирования на ошибку.
        /// </summary>
        /// <param name="type"> тип реагирования на ошибку </param>
        /// <returns> номер </returns>
        private int GetErrorTypeIndex(Operation.ErrorTypes error)
        {
            for (int i = 0; i < errorComboBox.Items.Count; i++)
            {
                Operation.ErrorTypes errorType = ((EnumWrapper<Operation.ErrorTypes>)errorComboBox.Items[i]).GetConst();
                if (errorType == error)
                {
                    return i;
                }
            }
            return 0;
        }

        /// <summary>
        /// Обработать событие загрузки формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OperationForm_Load(object sender, EventArgs e)
        {
            typeComboBox.Items.AddRange(Operation.GetOperationTypes().ToArray());
            errorComboBox.Items.AddRange(Operation.GetErrorTypes().ToArray());
            typeComboBox.SelectedIndex = 0;
            errorComboBox.SelectedIndex = 0;
            parentComboBox.SelectedIndex = 0;
            if (operation != null)
            {
                typeComboBox.SelectedIndex = GetOperationTypeIndex(operation.type);
                errorComboBox.SelectedIndex = GetErrorTypeIndex(operation.errorType);
                adminCheckBox.Checked = operation.isAdmin;
                sourceNameTextBox.Text = operation.sourceName;
                destinationTextBox.Text = operation.destinationName;
                sqlServerTextBox.Text = operation.sqlServer;
                dbLoginTextBox.Text = operation.dbLogin;
                dbPasswordTextBox.Text = operation.dbPassword;
                for (int i = 0; i < parentComboBox.Items.Count; i++)
                {
                    string itemName = parentComboBox.Items[i].ToString();
                    int pos = itemName.IndexOf(operation.parentIndex + 1 + " ");
                    if (pos == 0)
                    {
                        parentComboBox.SelectedIndex = i;
                    }
                }
                generateFolderCheckBox.Checked = operation.isGenerateBackupFolder;
                operation = null;
            }
        }

        /// <summary>
        /// Задать выпадающий список родительских операций.
        /// </summary>
        /// <param name="operations"> операция </param>
        /// <param name="currentIndex"> индекс редактируемой операции, которую нужно исключить из списка </param>
        public void SetParentOperationList(ArrayList operations, int currentIndex)
        {
            int i = 1;
            parentComboBox.Items.Add(new EnumWrapper<Operation>(null, ""));
            foreach (Operation oper in operations)
            {
                if (currentIndex != i)
                {
                    if (oper.type == Operation.OperationTypes.CONNECT_DB || //oper.type == Operation.OperationTypes.BACKUP_FILE || oper.type == Operation.OperationTypes.BACKUP_FOLDER ||
                        oper.type == Operation.OperationTypes.GEN_FOLDER )//|| oper.type == Operation.OperationTypes.BACKUP_FILES_IN_FOLDER || oper.type == Operation.OperationTypes.COPY_FILES_IN_FOLDER)
                    {
                        int index = parentComboBox.Items.Add(new EnumWrapper<Operation>(oper, i + " " + Operation.ToString(oper.type)));
                    }
                }
                i++;
            }
        }

        /// <summary>
        /// Обработать корректность выбранных данных для типа операции.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void typeComboBox_Validated(object sender, EventArgs e)
        {
            CheckComboBox(sender, typeComboBox, "Select operation type");
        }
        
        /// <summary>
        /// Задать правильное отображение об ошибке ввода данных для выпадающего списка.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="comboBox"> выпадающий список </param>
        /// <param name="errorMsg"> сообщение об ошибке </param>
        private void CheckComboBox(object sender, ComboBox comboBox, string errorMsg)
        {
            if ((sender as ComboBox).SelectedIndex == -1)
            {
                errorProvider.SetError(errorComboBox, errorMsg);
            }
            else
            {
                errorProvider.SetError(errorComboBox, string.Empty);
            }
        }

        /// <summary>
        /// Задать правильное отображение об ошибке ввода данных для текстового поля.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="textBox"> текстовое поле </param>
        /// <param name="errorMsg"> сообщение об ошибке </param>
        private void CheckTextBox(object sender, TextBox textBox, string errorMsg)
        {
            if ((sender as TextBox).Text.Equals(""))
            {
                errorProvider.SetError(textBox, errorMsg);
            }
            else
            {
                errorProvider.SetError(textBox, string.Empty);
            }
        }

        /// <summary>
        /// Обработать корректность выбранных данных для источника операции.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sourceNameTextBox_Validated(object sender, EventArgs e)
        {
            CheckTextBox(sender, sourceNameTextBox, "Set source");
        }

        /// <summary>
        /// Обработать корректность выбранных данных для цели операции.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void destinationTextBox_Validated(object sender, EventArgs e)
        {
            CheckTextBox(sender, destinationTextBox, "Set destination");
        }

        /// <summary>
        /// Обработать корректность выбранных данных для типа реагирования на ошибку.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void errorComboBox_Validated(object sender, EventArgs e)
        {
            CheckComboBox(sender, errorComboBox, "Select error type");
        }

        /// <summary>
        /// Обработать корректность выбранных данных для адреса SQL-сервера.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sqlServerTextBox_Validated(object sender, EventArgs e)
        {
            CheckTextBox(sender, sqlServerTextBox, "Set SQL Server");
        }

        /// <summary>
        /// Обработать корректность выбранных данных для логина к БД.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dbLoginTextBox_Validated(object sender, EventArgs e)
        {
            CheckTextBox(sender, dbLoginTextBox, "Set DB login");
        }

        /// <summary>
        /// Обработать корректность выбранных данных для пароля к БД.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dbPasswordTextBox_Validated(object sender, EventArgs e)
        {
            CheckTextBox(sender, dbPasswordTextBox, "Set DB password");
        }

        /// <summary>
        /// Обработать корректность выбранных данных для родительской операции.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void parentComboBox_Validated(object sender, EventArgs e)
        {
            if (parentComboBox.SelectedIndex <= 0)
            {
                return;
            }
            Operation.OperationTypes type = ((EnumWrapper<Operation.OperationTypes>)typeComboBox.SelectedItem).GetConst();
            Operation parent = ((EnumWrapper<Operation>)parentComboBox.SelectedItem).GetConst();

            errorProvider.SetError(parentComboBox, string.Empty);
            if (((type == Operation.OperationTypes.BACKUP_DB || type == Operation.OperationTypes.RUN_SQL_QUERY || type == Operation.OperationTypes.RUN_SQL_QUERIES_IN_FOLDER) && parent.type != Operation.OperationTypes.CONNECT_DB) || 
                ((type == Operation.OperationTypes.BACKUP_FILE || type == Operation.OperationTypes.BACKUP_FOLDER) && parent.type == Operation.OperationTypes.CONNECT_DB))
            {
                errorProvider.SetError(parentComboBox, "Error parent operation.");
            }
        }

        /// <summary>
        /// Обработать событие закрытия формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OperationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                string sourceName = sourceNameTextBox.Text;
                string destinationName = destinationTextBox.Text;
                bool isAdmin = adminCheckBox.Checked;
                string sqlServerName = sqlServerTextBox.Text;
                string dbLogin = dbLoginTextBox.Text;
                string dbPassword = dbPasswordTextBox.Text;
                int parentIndex = -1;
                bool isGenerateBackupFolder = generateFolderCheckBox.Checked;

                Operation.OperationTypes type = Operation.OperationTypes.EMPTY;
                if (typeComboBox.SelectedIndex != -1)
                {
                    type = ((EnumWrapper<Operation.OperationTypes>)typeComboBox.SelectedItem).GetConst();
                }
                Operation.ErrorTypes error = Operation.ErrorTypes.EMPTY;
                if (errorComboBox.SelectedIndex != -1)
                {
                    error = ((EnumWrapper<Operation.ErrorTypes>)errorComboBox.SelectedItem).GetConst();
                }
                if (parentComboBox.SelectedIndex > 0)
                {
                    string str = ((EnumWrapper<Operation>)parentComboBox.SelectedItem).ToString();
                    str = str.Substring(0, str.IndexOf(' '));
                    parentIndex = int.Parse(str) - 1;
                }
                if ((sourceName.Equals("") && destinationName.Equals("")) || type == Operation.OperationTypes.EMPTY || error == Operation.ErrorTypes.EMPTY)
                {
                    MessageBox.Show("Set missing data", "Data error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Cancel = true;
                }
                if (operation == null)
                {
                    operation = new Operation(type, sourceName, error, isAdmin, destinationName, sqlServerName, dbLogin, dbPassword, parentIndex, isGenerateBackupFolder);
                }
            }
        }

        /// <summary>
        /// Обработать событие выбора типа операции.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Operation.OperationTypes type = ((EnumWrapper<Operation.OperationTypes>)typeComboBox.SelectedItem).GetConst();

            sourceNameLabel.Enabled = true;
            sourceNameTextBox.Enabled = true;
            destinationLabel.Enabled = true;
            destinationTextBox.Enabled = true;
            sqlServerLabel.Enabled = false;
            sqlServerTextBox.Enabled = false;
            dbLoginLabel.Enabled = false;
            dbLoginTextBox.Enabled = false;
            dbPasswordLabel.Enabled = false;
            dbPasswordTextBox.Enabled = false;
            generateFolderLabel.Enabled = false;
            generateFolderCheckBox.Enabled = false;
            parentComboBox.Enabled = false;
            parentLabel.Enabled = false;
            selectSourceButton.Enabled = false;
            selectDestinationButton.Enabled = false;
            switch (type)
            {
                case Operation.OperationTypes.BACKUP_DB:
                    sourceNameLabel.Text = "What DB backup?";
                    destinationLabel.Text = "Where do backup?";
                    parentComboBox.Enabled = true;
                    parentLabel.Enabled = true;
                    selectDestinationButton.Enabled = true;
                    break;
                case Operation.OperationTypes.BACKUP_FILE:
                    sourceNameLabel.Text = "What file backup?";
                    destinationLabel.Text = "Where do backup?";
                    generateFolderLabel.Enabled = true;
                    generateFolderCheckBox.Enabled = true;
                    parentComboBox.Enabled = true;
                    parentLabel.Enabled = true;
                    selectSourceButton.Enabled = true;
                    selectDestinationButton.Enabled = true;
                    break;
                case Operation.OperationTypes.BACKUP_FOLDER:
                //case Operation.OperationTypes.BACKUP_FILES_IN_FOLDER:
                    sourceNameLabel.Text = "What folder backup?";
                    destinationLabel.Text = "Where do backup?";
                    generateFolderLabel.Enabled = true;
                    generateFolderCheckBox.Enabled = true;
                    parentComboBox.Enabled = true;
                    parentLabel.Enabled = true;
                    selectSourceButton.Enabled = true;
                    selectDestinationButton.Enabled = true;
                    break;
                case Operation.OperationTypes.CONNECT_DB:
                    sourceNameLabel.Text = "DB name";
                    destinationLabel.Enabled = false;
                    destinationTextBox.Enabled = false;
                    sqlServerLabel.Enabled = true;
                    sqlServerTextBox.Enabled = true;
                    dbLoginLabel.Enabled = true;
                    dbLoginTextBox.Enabled = true;
                    dbPasswordLabel.Enabled = true;
                    dbPasswordTextBox.Enabled = true;
                    break;
                case Operation.OperationTypes.COPY_FILE:
                    sourceNameLabel.Text = "What file copy?";
                    destinationLabel.Text = "Where copy?";
                    selectSourceButton.Enabled = true;
                    selectDestinationButton.Enabled = true;
                    break;
                case Operation.OperationTypes.COPY_FOLDER:
                //case Operation.OperationTypes.COPY_FILES_IN_FOLDER:
                    sourceNameLabel.Text = "What folder copy?";
                    destinationLabel.Text = "Where copy?";
                    selectSourceButton.Enabled = true;
                    selectDestinationButton.Enabled = true;
                    break;
                case Operation.OperationTypes.DELETE_FILE:
                    sourceNameLabel.Text = "What file delete?";
                    selectSourceButton.Enabled = true;
                    destinationLabel.Enabled = false;
                    destinationTextBox.Enabled = false;
                    break;
                case Operation.OperationTypes.DELETE_FOLDER:
                    sourceNameLabel.Text = "What folder delete?";
                    selectSourceButton.Enabled = true;
                    destinationLabel.Enabled = false;
                    destinationTextBox.Enabled = false;
                    break;
                case Operation.OperationTypes.RUN_PROCESS:
                case Operation.OperationTypes.STOP_PROCESS:
                    sourceNameLabel.Text = "Process name";
                    destinationLabel.Enabled = false;
                    destinationTextBox.Enabled = false;
                    break;
                case Operation.OperationTypes.RUN_SERVICE:
                case Operation.OperationTypes.STOP_SERVICE:
                    sourceNameLabel.Text = "Service name";
                    destinationLabel.Enabled = false;
                    destinationTextBox.Enabled = false;
                    break;
                case Operation.OperationTypes.RUN_SQL_QUERY:
                    sourceNameLabel.Text = "SQL file name";
                    destinationLabel.Enabled = false;
                    destinationTextBox.Enabled = false;
                    parentComboBox.Enabled = true;
                    parentLabel.Enabled = true;
                    selectSourceButton.Enabled = true;
                    break;
                case Operation.OperationTypes.RUN_SQL_QUERIES_IN_FOLDER:
                    sourceNameLabel.Text = "SQL dir";
                    destinationLabel.Enabled = false;
                    destinationTextBox.Enabled = false;
                    parentComboBox.Enabled = true;
                    parentLabel.Enabled = true;
                    selectSourceButton.Enabled = true;
                    break;
                case Operation.OperationTypes.RUN_TASK:
                case Operation.OperationTypes.STOP_TASK:
                    sourceNameLabel.Text = "Task name";
                    destinationLabel.Enabled = false;
                    destinationTextBox.Enabled = false;
                    break;
                case Operation.OperationTypes.STOP_SCENARIO:
                    sourceNameLabel.Text = "Why to stop?";
                    destinationLabel.Enabled = false;
                    destinationTextBox.Enabled = false;
                    break;
                case Operation.OperationTypes.CREATE_VAR:
                    sourceNameLabel.Text = "Name";
                    destinationLabel.Text = "Value";
                    break;
                case Operation.OperationTypes.GEN_FOLDER:
                    destinationLabel.Text = "Where generate?";
                    sourceNameLabel.Enabled = false;
                    sourceNameTextBox.Enabled = false;
                    selectDestinationButton.Enabled = true;
                    //selectSourceButton.Enabled = true;
                    //destinationLabel.Enabled = false;
                    //destinationTextBox.Enabled = false;
                    break;
            }
        }

        /// <summary>
        /// Обработать событие выбора родительской операции.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void parentComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Operation.OperationTypes type = ((EnumWrapper<Operation.OperationTypes>)typeComboBox.SelectedItem).GetConst();

            if (!(type == Operation.OperationTypes.BACKUP_FILE || type == Operation.OperationTypes.BACKUP_FOLDER))
            {
                return;
            }
            if (parentComboBox.SelectedIndex > 0)
            {
                generateFolderLabel.Enabled = false;
                generateFolderCheckBox.Enabled = false;
                destinationLabel.Enabled = false;
                destinationTextBox.Enabled = false;
                selectDestinationButton.Enabled = false;
            }
            else
            {
                generateFolderLabel.Enabled = true;
                generateFolderCheckBox.Enabled = true;
                destinationLabel.Enabled = true;
                destinationTextBox.Enabled = true;
                selectDestinationButton.Enabled = true;
            }
        }

        /// <summary>
        /// Открыть диалог выбора файла или папки.
        /// </summary>
        /// <param name="textBox"> текстовое поле </param>
        /// <param name="isFile"> признак, что нужен файловый диалог </param>
        /// <param name="filter"> фильтр выбора </param>
        private void OpenFileOrFolderDialog(TextBox textBox, bool isFile, string filter = "")
        {
            if (isFile)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                int length = filter.Length;
                filter += "|All files|*.*";
                if (length == 0)
                {
                    filter = filter.Substring(1);
                }
                dialog.Filter = filter;
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                textBox.Text = dialog.FileName;
            }
            else
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                textBox.Text = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// Обработать событие выбора источника операции через диалог.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectSourceButton_Click(object sender, EventArgs e)
        {
            bool isFile = true;
            string filter = "";
            Operation.OperationTypes type = ((EnumWrapper<Operation.OperationTypes>)typeComboBox.SelectedItem).GetConst();
            if (type == Operation.OperationTypes.RUN_SQL_QUERY)
            {
                filter = "SQL file|*.sql";
            }

            if (type == Operation.OperationTypes.BACKUP_FOLDER || type == Operation.OperationTypes.COPY_FOLDER || 
                type == Operation.OperationTypes.RUN_SQL_QUERIES_IN_FOLDER || type == Operation.OperationTypes.DELETE_FOLDER ||
                //type == Operation.OperationTypes.COPY_FILES_IN_FOLDER || type == Operation.OperationTypes.BACKUP_FILES_IN_FOLDER ||
                type == Operation.OperationTypes.GEN_FOLDER)
            {
                isFile = false;
            }
            OpenFileOrFolderDialog(sourceNameTextBox, isFile, filter);
        }

        /// <summary>
        /// Обработать событие выбора цели операции через диалог.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectDestinationButton_Click(object sender, EventArgs e)
        {
            OpenFileOrFolderDialog(destinationTextBox, false);
        }
    }
}
