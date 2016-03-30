using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NightBuilder
{
    /// <summary>
    /// Форма, отображающая список ошибок.
    /// </summary>
    public partial class ErrorListForm : Form
    {
        public ErrorListForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Заполнить список ошибок на форме.
        /// </summary>
        /// <param name="errors"></param>
        public void SetErrorList(ArrayList errors)
        {
            foreach (string error in errors)
            {
                int index = errorDataGridView.Rows.Add();
                errorDataGridView.Rows[index].Cells["Number"].Value = index + 1;
                errorDataGridView.Rows[index].Cells["Error"].Value = error;
            }
        }
    }
}
