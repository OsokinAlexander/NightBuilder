using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Xml;
using System.Threading;
using System.Xml.Serialization;

namespace NightBuilder
{
    /// <summary>
    /// Основная форма утилиты.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Список операций сценария.
        /// </summary>
        private ArrayList operations = new ArrayList();
        /// <summary>
        /// Поток (нить), в котором будут выполняться операции.
        /// </summary>
        private Thread thread;
        /// <summary>
        /// Признак сохранения сценария в файл.
        /// </summary>
        private bool isSavedInFile = false;
        /// <summary>
        /// Длина заголовка главной формы.
        /// </summary>
        private int titleLength = 0;
        /// <summary>
        /// Полный путь к файлу, в котором хранится сценарий.
        /// </summary>
        private string savedFile = "";
        /// <summary>
        /// Буферная операция, которую держим в памяти при копировании.
        /// </summary>
        private Operation copyOperation = null;

        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обработать двойной клик мыши по ячейке таблицы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            EditOperation(e.RowIndex, true);
        }

        /// <summary>
        /// Редактировать операцию.
        /// </summary>
        /// <param name="index"> номер операции в списке </param>
        /// <param name="isEdit"> признак, что операция редактируется, а не создаётся заново </param>
        private void EditOperation(int index, bool isEdit)
        {
            OperationForm form = new OperationForm();
            if (isEdit)
            {
                form.operation = new Operation((Operation)operations[index]);
            }
            form.SetParentOperationList(operations, index + 1);
            DialogResult result = form.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }
            if (form.operation.parentIndex >= 0)
            {
                form.operation.SetParent((Operation)operations[form.operation.parentIndex]);
            }
            if (!isEdit)
            {
                if (index == -1)
                {
                    operations.Add(form.operation);
                    index = dataGridView.Rows.Add();
                }
                else
                {
                    operations.Insert(index, form.operation);
                    dataGridView.Rows.Insert(index, 1);
                }
            }
            else
            {
                operations.RemoveAt(index);
                operations.Insert(index, form.operation);
            }
            foreach (Operation oper in operations)
            {
                if (oper.parentIndex == index)
                {
                    oper.SetParent((Operation)operations[index]);
                }
            }
            SetDataGridViewRow(index);
            
            ClearStatusImages(0);
            isSavedInFile = false;
        }

        /// <summary>
        /// Заполнить строку таблицы на основе операции.
        /// </summary>
        /// <param name="index"> номер операции в списке </param>
        private void SetDataGridViewRow(int index)
        {
            dataGridView.Rows[index].Cells["Status"].Value = Properties.Resources.Clear_16;
            dataGridView.Rows[index].Cells["Number"].Value = index + 1;
            dataGridView.Rows[index].Cells["OperationType"].Value = Operation.ToString(((Operation)operations[index]).type);
            dataGridView.Rows[index].Cells["Details"].Value = ((Operation)operations[index]).ToString();
            if (((Operation)operations[index]).parentIndex >= 0)
            {
                dataGridView.Rows[index].Cells["ParentStep"].Value = ((Operation)operations[index]).parentIndex + 1;
            }
            else
            {
                dataGridView.Rows[index].Cells["ParentStep"].Value = "";
            }
            dataGridView.Rows[index].Cells["ErrorAction"].Value = Operation.ToString(((Operation)operations[index]).errorType);
            dataGridView.CurrentCell = dataGridView[0, index];
        }

        /// <summary>
        /// Обработать клик по кнопке "Новая операция".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newButton_Click(object sender, EventArgs e)
        {
            int index = -1;
            if (dataGridView.CurrentRow != null)
            {
                index = dataGridView.CurrentRow.Index + 1;
            }
            EditOperation(index, false);
        }

        /// <summary>
        /// Обработать клик по кнопке "Редактировать операцию".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editButton_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null)
            {
                return;
            }
            int index = dataGridView.CurrentRow.Index;
            if (index >= 0)
            {
                EditOperation(index, true);
            }
        }

        /// <summary>
        /// Выполнить операции из списка в отдельном потоке.
        /// </summary>
        private void ExecuteOperations(int startStep)
        {
            bool isError = false;
            bool isStop = false;
            Dictionary<string, string> variables = new Dictionary<string, string>();
            foreach (Operation oper in operations)
            {
                if (oper.type == Operation.OperationTypes.CREATE_VAR)
                {
                    variables.Add(oper.sourceName.ToUpper(), oper.destinationName);
                }
            }
            int i = startStep;
            for (; i < operations.Count; i++)
            {
                ActionResult result = ((Operation)operations[i]).Execute(variables);
                if (!result.IsEnd)
                {
                    dataGridView.Rows[i].Cells["Status"].Value = Properties.Resources.Stop_16;
                    dataGridView.Rows[i].Cells["Error"].Value = result.Error;
                    dataGridView.Rows[i].Tag = result.ErrorList;
                    if (result.ErrorList != null && result.ErrorList.Count > 0)
                    {
                        dataGridView.Rows[i].Cells["ShowErrorList"].Value = result.ErrorList.Count + " errors";
                    }
                    if (((Operation)operations[i]).IsStopOnError())
                    {
                        isError = true;
                        break;
                    }
                }
                else
                {
                    dataGridView.Rows[i].Cells["Status"].Value = Properties.Resources.Start_16;
                    dataGridView.Rows[i].Cells["Error"].Value = "";
                    dataGridView.Rows[i].Tag = null;
                    if (((Operation)operations[i]).type == Operation.OperationTypes.STOP_SCENARIO)
                    {
                        isStop = true;
                        break;
                    }
                }
                Thread.Sleep(0);
            }
            // Анонимный делегат при окончании работы потока.
            this.BeginInvoke(
               new Action(() =>
               {
                   this.Cursor = Cursors.Default;
                   dataGridView.Cursor = Cursors.Default;
                   saveButton.Enabled = true;
                   loadButton.Enabled = true;
                   clearButton.Enabled = true;
                   newButton.Enabled = true;
                   editButton.Enabled = true;
                   deleteButton.Enabled = true;
                   upButton.Enabled = true;
                   downButton.Enabled = true;
                   if (isStop)
                   {
                       if (i + 1 < dataGridView.RowCount)
                       {
                           i++;
                       }
                   }
                   else if (!isError)
                   {
                       i--;
                   }
                   dataGridView.CurrentCell = dataGridView[0, i];
               }));
            
        }

        /// <summary>
        /// Очистить иконки операций в списке.
        /// </summary>
        private void ClearStatusImages(int startStep)
        {
            for (int i = startStep; i < dataGridView.Rows.Count; i++)
            {
                dataGridView.Rows[i].Cells["Status"].Value = Properties.Resources.Clear_16;
                dataGridView.Rows[i].Cells["ShowErrorList"].Value = "";
            }
        }

        /// <summary>
        /// Начать выполнение списка операций с требуемого шага.
        /// </summary>
        /// <param name="startStep"> номер начального шага </param>
        private void StartOperations(int startStep)
        {
            this.Cursor = Cursors.WaitCursor;
            dataGridView.Cursor = Cursors.WaitCursor;
            saveButton.Enabled = false;
            loadButton.Enabled = false;
            clearButton.Enabled = false;
            newButton.Enabled = false;
            editButton.Enabled = false;
            deleteButton.Enabled = false;
            upButton.Enabled = false;
            downButton.Enabled = false;
            ClearStatusImages(startStep);
            thread = new Thread(() => ExecuteOperations(startStep));
            thread.Start();
        }

        /// <summary>
        /// Обработать клик по кнопке "Начать выполнение сценария".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startButton_Click(object sender, EventArgs e)
        {
            StartOperations(0);
        }

        /// <summary>
        /// Обработать клик по кнопке "Начать выполнение сценария с шага".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startFromButton_Click(object sender, EventArgs e)
        {
            StartOperations(dataGridView.CurrentRow.Index);
        }

        /// <summary>
        /// Обработать клик по кнопке "Остановить выполнение сценария".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
            dataGridView.Cursor = Cursors.Default;
            thread.Abort();
            saveButton.Enabled = true;
            loadButton.Enabled = true;
            clearButton.Enabled = true;
            newButton.Enabled = true;
            editButton.Enabled = true;
            deleteButton.Enabled = true;
            upButton.Enabled = true;
            downButton.Enabled = true;
        }

        /// <summary>
        /// Задать заголовок главной формы.
        /// </summary>
        /// <param name="fileName"></param>
        private void setMainFormHeader(string fileName)
        {
            isSavedInFile = true;
            savedFile = fileName;
            if (titleLength == 0)
            {
                titleLength = ActiveForm.Text.Length;
                ActiveForm.Text += " - " + fileName;
            }
            else
            {
                ActiveForm.Text = ActiveForm.Text.Substring(0, titleLength) + " - " + fileName;
            }
        }

        /// <summary>
        /// Сохранить сценарий в файл.
        /// </summary>
        /// <param name="isNewFile"> признак, что нужно создать новый файл </param>
        private void SaveScenarioToFile(bool isNewFile)
        {
            string fileName = savedFile;
            if (isNewFile)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Night build file|*.nbuild|All files|*.*";
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                fileName = dialog.FileName;
            }
            Type[] types = new Type[2];
            types[0] = typeof(ArrayList);
            types[1] = typeof(Operation);
            XmlSerializer writer = new XmlSerializer(typeof(ArrayList), types);
            FileStream file = File.Create(fileName);
            writer.Serialize(file, operations);
            file.Close();
            setMainFormHeader(fileName);
        }

        /// <summary>
        /// Обработать клик по кнопке "Сохранить сценарий".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveScenarioToFile(savedFile.Length == 0);
        }

        /// <summary>
        /// Обработать клик по кнопке "Загрузить сценарий".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Night build file|*.nbuild|All files|*.*";
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string fileName = dialog.FileName;
            Type[] types = new Type[2];
            types[0] = typeof(ArrayList);
            types[1] = typeof(Operation);
            XmlSerializer serializer = new XmlSerializer(typeof(ArrayList), types);
            FileStream file = File.Open(fileName, FileMode.Open);
            operations = (ArrayList)serializer.Deserialize(file);
            file.Close();
            setMainFormHeader(fileName);
            foreach (Operation oper in operations)
            {
                if (oper.parentIndex >= 0)
                {
                    oper.SetParent((Operation)operations[oper.parentIndex]);
                }
                oper.SetBackupFolder();
            }
            FillDataGridView();
        }

        /// <summary>
        /// Заполнить таблицу данными из списка операций.
        /// </summary>
        private void FillDataGridView()
        {
            dataGridView.Rows.Clear();
            foreach (Operation op in operations)
            {
                int index = dataGridView.Rows.Add();
                dataGridView.Rows[index].Cells["Status"].Value = Properties.Resources.Clear_16;
                dataGridView.Rows[index].Cells["Number"].Value = index + 1;
                dataGridView.Rows[index].Cells["OperationType"].Value = Operation.ToString(op.type);
                dataGridView.Rows[index].Cells["Details"].Value = op.ToString();
                if (op.parentIndex >= 0)
                {
                    dataGridView.Rows[index].Cells["ParentStep"].Value = op.parentIndex + 1;
                }
                dataGridView.Rows[index].Cells["ErrorAction"].Value = Operation.ToString(op.errorType);
            }
        }

        /// <summary>
        /// Проверить, является ли строка родительской операцией.
        /// </summary>
        /// <param name="index"> индекс операции в списке </param>
        /// <returns>Да/нет</returns>
        private bool IsParentOperation(int index)
        {
            foreach (Operation oper in operations)
            {
                if (oper.parentIndex == index)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Переместить запись в списке и таблице операций на одну позицию.
        /// </summary>
        /// <param name="isUp"> признак, что перемещение вверх </param>
        private void MoveRowInDataGridView(bool isUp)
        {
            if (dataGridView.CurrentRow == null)
            {
                return;
            }
            int index = dataGridView.CurrentRow.Index;
            if (index == 0 && isUp)
            {
                return;
            }
            if (index == dataGridView.Rows.Count - 1 && !isUp)
            {
                return;
            }
            if (index >= 0)
            {
                int newIndex = isUp ? index - 1 : index + 1;
                bool isCurrentParentOperation = IsParentOperation(index);
                bool isNextParentOperation = IsParentOperation(newIndex);
                Operation moveOperation = new Operation((Operation)operations[index]);
                operations.RemoveAt(index);
                operations.Insert(newIndex, moveOperation);

                foreach (Operation oper in operations)
                {
                    if (isCurrentParentOperation && oper.parentIndex == index)
                    {
                        oper.parentIndex = newIndex;
                        oper.SetParent((Operation)operations[newIndex]);
                    }
                    else if (isNextParentOperation && oper.parentIndex == newIndex)
                    {
                        oper.parentIndex = index;
                        oper.SetParent((Operation)operations[index]);
                    }
                }

                FillDataGridView();
                dataGridView.CurrentCell = dataGridView[0, newIndex];
                isSavedInFile = false;
            }
        }

        /// <summary>
        /// Обработать клик по кнопке "Переместить операцию вверх".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void upButton_Click(object sender, EventArgs e)
        {
            MoveRowInDataGridView(true);
        }

        /// <summary>
        /// Обработать клик по кнопке "Переместить операцию вниз".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void downButton_Click(object sender, EventArgs e)
        {
            MoveRowInDataGridView(false);
        }

        /// <summary>
        /// Обработать клик по кнопке "Очистить список операций".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearButton_Click(object sender, EventArgs e)
        {
            dataGridView.Rows.Clear();
            operations.Clear();
            isSavedInFile = false;
        }

        /// <summary>
        /// Обработать клик по кнопке "Удалить выбранную операцию".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null)
            {
                return;
            }
            int index = dataGridView.CurrentRow.Index;
            dataGridView.Rows.RemoveAt(index);
            operations.RemoveAt(index);
            for (int i = 0; i < operations.Count; i++)
            {
                dataGridView.Rows[i].Cells["Number"].Value = i + 1;
                int parentIndex = ((Operation)operations[i]).parentIndex;
                if (parentIndex == index)
                {
                    ((Operation)operations[i]).parentIndex = -1;
                    ((Operation)operations[i]).SetParent(null);
                    dataGridView.Rows[i].Cells["ParentStep"].Value = "";
                }
                if (parentIndex != -1 && parentIndex > index)
                {
                    parentIndex = ((Operation)operations[i]).parentIndex--;
                    ((Operation)operations[i]).SetParent((Operation)operations[parentIndex]);
                    dataGridView.Rows[i].Cells["ParentStep"].Value = parentIndex;
                }
            }
            isSavedInFile = false;
        }

        /// <summary>
        /// Установить активность кнопок старта и сохранения в зависимость от состояния таблицы операций.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableStartSaveButtons()
        {
            if (dataGridView.Rows.Count > 0)
            {
                startButton.Enabled = true;
                startFromButton.Enabled = true;
                stopButton.Enabled = true;
                saveButton.Enabled = true;
            }
            else
            {
                startButton.Enabled = false;
                startFromButton.Enabled = false;
                stopButton.Enabled = false;
                saveButton.Enabled = false;
            }
        }

        /// <summary>
        /// Обработать событие добавления строки в таблице.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            EnableStartSaveButtons();
        }

        /// <summary>
        /// Обработать событие удаления строки в таблице.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            EnableStartSaveButtons();
        }

        /// <summary>
        /// Обработать событие закрытия главной формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!(!isSavedInFile && dataGridView.Rows.Count > 0))
            {
                return;
            }
            DialogResult result = MessageBox.Show("Do want to save your scenario?", "Save scenario", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else if (result == DialogResult.Yes)
            {
                SaveScenarioToFile(savedFile.Length == 0);
            }
        }

        /// <summary>
        /// Обработать событие клика мышью по ячейке таблицы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 6)
            {
                return;
            }
            ArrayList errors = (ArrayList)dataGridView.CurrentRow.Tag;
            if (errors == null || errors.Count == 0)
            {
                return;
            }
            ErrorListForm form = new ErrorListForm();
            form.SetErrorList(errors);
            form.ShowDialog();
        }

        /// <summary>
        /// Обработать событие нажатия клавиш клавиатуры.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.F5 && startFromButton.Enabled)
            {
                startFromButton_Click(null, null);
            }
            else
            {
                if (e.KeyCode == Keys.F5 && startButton.Enabled)
                {
                    startButton_Click(null, null);
                }
            }
            if (e.KeyCode == Keys.F6 && stopButton.Enabled)
            {
                stopButton_Click(null, null);
            }
            if (e.Shift && e.Control && e.KeyCode == Keys.S && saveButton.Enabled)
            {
                SaveScenarioToFile(true);
            }
            else
            {
                if (e.Control && e.KeyCode == Keys.S && saveButton.Enabled)
                {
                    saveButton_Click(null, null);
                }
            }
            if (e.Control && e.KeyCode == Keys.O)
            {
                loadButton_Click(null, null);
            }
            if (e.Shift && e.KeyCode == Keys.C)
            {
                clearButton_Click(null, null);
            }
            if (e.Control && e.KeyCode == Keys.N)
            {
                newButton_Click(null, null);
            }
            if (e.Control && e.KeyCode == Keys.E)
            {
                editButton_Click(null, null);
            }
            if (e.KeyCode == Keys.Delete)
            {
                deleteButton_Click(null, null);
            }
            if (e.Control && e.KeyCode == Keys.U)
            {
                upButton_Click(null, null);
            }
            if (e.Control && e.KeyCode == Keys.D)
            {
                downButton_Click(null, null);
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                copyButton_Click(null, null);
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                pasteButton_Click(null, null);
            }
            if (e.Control && e.KeyCode == Keys.H)
            {
                //help
            }
        }

        /// <summary>
        /// Обработать событие нажатия кнопки "Копировать операцию".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyButton_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null){
                return;
            }
            copyOperation = (Operation)operations[dataGridView.CurrentRow.Index];
        }

        /// <summary>
        /// Обработать событие нажатия кнопки "Вставить операцию".
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pasteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow == null)
            {
                return;
            }
            int index = dataGridView.CurrentRow.Index + 1;
            operations.Insert(index, new Operation(copyOperation));
            for (int i = 0; i < operations.Count; i++)
            {
                int parentIndex = ((Operation)operations[i]).parentIndex;
                if (parentIndex != -1 && parentIndex >= index)
                {
                    parentIndex = ((Operation)operations[i]).parentIndex++;
                    ((Operation)operations[i]).SetParent((Operation)operations[parentIndex]);
                }
            }
            dataGridView.Rows.Insert(index, 1);
            FillDataGridView();
            dataGridView.CurrentCell = dataGridView[0, index];

            ClearStatusImages(0);
            isSavedInFile = false;
        }
    }
}
