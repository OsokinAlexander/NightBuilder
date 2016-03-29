namespace NightBuilder
{
    partial class OperationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OperationForm));
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.sourceNameLabel = new System.Windows.Forms.Label();
            this.sourceNameTextBox = new System.Windows.Forms.TextBox();
            this.errorComboBox = new System.Windows.Forms.ComboBox();
            this.errorLabel = new System.Windows.Forms.Label();
            this.destinationTextBox = new System.Windows.Forms.TextBox();
            this.destinationLabel = new System.Windows.Forms.Label();
            this.adminCheckBox = new System.Windows.Forms.CheckBox();
            this.adminLabel = new System.Windows.Forms.Label();
            this.sqlServerTextBox = new System.Windows.Forms.TextBox();
            this.sqlServerLabel = new System.Windows.Forms.Label();
            this.dbLoginLabel = new System.Windows.Forms.Label();
            this.dbPasswordLabel = new System.Windows.Forms.Label();
            this.dbLoginTextBox = new System.Windows.Forms.TextBox();
            this.dbPasswordTextBox = new System.Windows.Forms.TextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.parentLabel = new System.Windows.Forms.Label();
            this.parentComboBox = new System.Windows.Forms.ComboBox();
            this.generateFolderCheckBox = new System.Windows.Forms.CheckBox();
            this.generateFolderLabel = new System.Windows.Forms.Label();
            this.selectSourceButton = new System.Windows.Forms.Button();
            this.selectDestinationButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // typeComboBox
            // 
            this.typeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Location = new System.Drawing.Point(135, 13);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(255, 21);
            this.typeComboBox.Sorted = true;
            this.typeComboBox.TabIndex = 0;
            this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.typeComboBox_SelectedIndexChanged);
            this.typeComboBox.Validated += new System.EventHandler(this.typeComboBox_Validated);
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(233, 250);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 8;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(314, 250);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Operation type";
            // 
            // sourceNameLabel
            // 
            this.sourceNameLabel.AutoSize = true;
            this.sourceNameLabel.Location = new System.Drawing.Point(10, 45);
            this.sourceNameLabel.Name = "sourceNameLabel";
            this.sourceNameLabel.Size = new System.Drawing.Size(41, 13);
            this.sourceNameLabel.TabIndex = 4;
            this.sourceNameLabel.Text = "Source";
            // 
            // sourceNameTextBox
            // 
            this.sourceNameTextBox.Location = new System.Drawing.Point(135, 45);
            this.sourceNameTextBox.Name = "sourceNameTextBox";
            this.sourceNameTextBox.Size = new System.Drawing.Size(254, 20);
            this.sourceNameTextBox.TabIndex = 1;
            this.sourceNameTextBox.Validated += new System.EventHandler(this.sourceNameTextBox_Validated);
            // 
            // errorComboBox
            // 
            this.errorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.errorComboBox.FormattingEnabled = true;
            this.errorComboBox.Location = new System.Drawing.Point(135, 97);
            this.errorComboBox.Name = "errorComboBox";
            this.errorComboBox.Size = new System.Drawing.Size(255, 21);
            this.errorComboBox.Sorted = true;
            this.errorComboBox.TabIndex = 3;
            this.errorComboBox.Validated += new System.EventHandler(this.errorComboBox_Validated);
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.Location = new System.Drawing.Point(10, 97);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(100, 13);
            this.errorLabel.TabIndex = 7;
            this.errorLabel.Text = "What do with error?";
            // 
            // destinationTextBox
            // 
            this.destinationTextBox.Location = new System.Drawing.Point(135, 71);
            this.destinationTextBox.Name = "destinationTextBox";
            this.destinationTextBox.Size = new System.Drawing.Size(254, 20);
            this.destinationTextBox.TabIndex = 2;
            this.destinationTextBox.Validated += new System.EventHandler(this.destinationTextBox_Validated);
            // 
            // destinationLabel
            // 
            this.destinationLabel.AutoSize = true;
            this.destinationLabel.Location = new System.Drawing.Point(10, 71);
            this.destinationLabel.Name = "destinationLabel";
            this.destinationLabel.Size = new System.Drawing.Size(60, 13);
            this.destinationLabel.TabIndex = 8;
            this.destinationLabel.Text = "Destination";
            // 
            // adminCheckBox
            // 
            this.adminCheckBox.AutoSize = true;
            this.adminCheckBox.Enabled = false;
            this.adminCheckBox.Location = new System.Drawing.Point(133, 255);
            this.adminCheckBox.Name = "adminCheckBox";
            this.adminCheckBox.Size = new System.Drawing.Size(15, 14);
            this.adminCheckBox.TabIndex = 4;
            this.adminCheckBox.UseVisualStyleBackColor = true;
            this.adminCheckBox.Visible = false;
            // 
            // adminLabel
            // 
            this.adminLabel.AutoSize = true;
            this.adminLabel.Enabled = false;
            this.adminLabel.Location = new System.Drawing.Point(10, 255);
            this.adminLabel.Name = "adminLabel";
            this.adminLabel.Size = new System.Drawing.Size(100, 13);
            this.adminLabel.TabIndex = 11;
            this.adminLabel.Text = "Administration rights";
            this.adminLabel.Visible = false;
            // 
            // sqlServerTextBox
            // 
            this.sqlServerTextBox.Location = new System.Drawing.Point(135, 127);
            this.sqlServerTextBox.Name = "sqlServerTextBox";
            this.sqlServerTextBox.Size = new System.Drawing.Size(256, 20);
            this.sqlServerTextBox.TabIndex = 5;
            this.sqlServerTextBox.Validated += new System.EventHandler(this.sqlServerTextBox_Validated);
            // 
            // sqlServerLabel
            // 
            this.sqlServerLabel.AutoSize = true;
            this.sqlServerLabel.Location = new System.Drawing.Point(10, 130);
            this.sqlServerLabel.Name = "sqlServerLabel";
            this.sqlServerLabel.Size = new System.Drawing.Size(62, 13);
            this.sqlServerLabel.TabIndex = 13;
            this.sqlServerLabel.Text = "SQL Server";
            // 
            // dbLoginLabel
            // 
            this.dbLoginLabel.AutoSize = true;
            this.dbLoginLabel.Location = new System.Drawing.Point(10, 156);
            this.dbLoginLabel.Name = "dbLoginLabel";
            this.dbLoginLabel.Size = new System.Drawing.Size(47, 13);
            this.dbLoginLabel.TabIndex = 14;
            this.dbLoginLabel.Text = "DB login";
            // 
            // dbPasswordLabel
            // 
            this.dbPasswordLabel.AutoSize = true;
            this.dbPasswordLabel.Location = new System.Drawing.Point(10, 182);
            this.dbPasswordLabel.Name = "dbPasswordLabel";
            this.dbPasswordLabel.Size = new System.Drawing.Size(70, 13);
            this.dbPasswordLabel.TabIndex = 15;
            this.dbPasswordLabel.Text = "DB password";
            // 
            // dbLoginTextBox
            // 
            this.dbLoginTextBox.Location = new System.Drawing.Point(135, 153);
            this.dbLoginTextBox.Name = "dbLoginTextBox";
            this.dbLoginTextBox.Size = new System.Drawing.Size(256, 20);
            this.dbLoginTextBox.TabIndex = 6;
            this.dbLoginTextBox.Validated += new System.EventHandler(this.dbLoginTextBox_Validated);
            // 
            // dbPasswordTextBox
            // 
            this.dbPasswordTextBox.Location = new System.Drawing.Point(135, 179);
            this.dbPasswordTextBox.Name = "dbPasswordTextBox";
            this.dbPasswordTextBox.PasswordChar = '*';
            this.dbPasswordTextBox.Size = new System.Drawing.Size(256, 20);
            this.dbPasswordTextBox.TabIndex = 7;
            this.dbPasswordTextBox.Validated += new System.EventHandler(this.dbPasswordTextBox_Validated);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // parentLabel
            // 
            this.parentLabel.AutoSize = true;
            this.parentLabel.Location = new System.Drawing.Point(10, 207);
            this.parentLabel.Name = "parentLabel";
            this.parentLabel.Size = new System.Drawing.Size(85, 13);
            this.parentLabel.TabIndex = 16;
            this.parentLabel.Text = "Parent operation";
            // 
            // parentComboBox
            // 
            this.parentComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parentComboBox.FormattingEnabled = true;
            this.parentComboBox.Location = new System.Drawing.Point(135, 207);
            this.parentComboBox.Name = "parentComboBox";
            this.parentComboBox.Size = new System.Drawing.Size(254, 21);
            this.parentComboBox.TabIndex = 8;
            this.parentComboBox.SelectedIndexChanged += new System.EventHandler(this.parentComboBox_SelectedIndexChanged);
            this.parentComboBox.Validated += new System.EventHandler(this.parentComboBox_Validated);
            // 
            // generateFolderCheckBox
            // 
            this.generateFolderCheckBox.AutoSize = true;
            this.generateFolderCheckBox.Location = new System.Drawing.Point(133, 235);
            this.generateFolderCheckBox.Name = "generateFolderCheckBox";
            this.generateFolderCheckBox.Size = new System.Drawing.Size(15, 14);
            this.generateFolderCheckBox.TabIndex = 17;
            this.generateFolderCheckBox.UseVisualStyleBackColor = true;
            // 
            // generateFolderLabel
            // 
            this.generateFolderLabel.AutoSize = true;
            this.generateFolderLabel.Location = new System.Drawing.Point(10, 235);
            this.generateFolderLabel.Name = "generateFolderLabel";
            this.generateFolderLabel.Size = new System.Drawing.Size(80, 13);
            this.generateFolderLabel.TabIndex = 18;
            this.generateFolderLabel.Text = "Generate folder";
            // 
            // selectSourceButton
            // 
            this.selectSourceButton.Location = new System.Drawing.Point(396, 44);
            this.selectSourceButton.Name = "selectSourceButton";
            this.selectSourceButton.Size = new System.Drawing.Size(27, 23);
            this.selectSourceButton.TabIndex = 19;
            this.selectSourceButton.Text = "...";
            this.selectSourceButton.UseVisualStyleBackColor = true;
            this.selectSourceButton.Click += new System.EventHandler(this.selectSourceButton_Click);
            // 
            // selectDestinationButton
            // 
            this.selectDestinationButton.Location = new System.Drawing.Point(396, 69);
            this.selectDestinationButton.Name = "selectDestinationButton";
            this.selectDestinationButton.Size = new System.Drawing.Size(27, 23);
            this.selectDestinationButton.TabIndex = 20;
            this.selectDestinationButton.Text = "...";
            this.selectDestinationButton.UseVisualStyleBackColor = true;
            this.selectDestinationButton.Click += new System.EventHandler(this.selectDestinationButton_Click);
            // 
            // OperationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 288);
            this.Controls.Add(this.selectDestinationButton);
            this.Controls.Add(this.selectSourceButton);
            this.Controls.Add(this.generateFolderLabel);
            this.Controls.Add(this.generateFolderCheckBox);
            this.Controls.Add(this.parentComboBox);
            this.Controls.Add(this.parentLabel);
            this.Controls.Add(this.dbPasswordTextBox);
            this.Controls.Add(this.dbLoginTextBox);
            this.Controls.Add(this.dbPasswordLabel);
            this.Controls.Add(this.dbLoginLabel);
            this.Controls.Add(this.sqlServerLabel);
            this.Controls.Add(this.sqlServerTextBox);
            this.Controls.Add(this.adminLabel);
            this.Controls.Add(this.adminCheckBox);
            this.Controls.Add(this.destinationTextBox);
            this.Controls.Add(this.destinationLabel);
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.errorComboBox);
            this.Controls.Add(this.sourceNameTextBox);
            this.Controls.Add(this.sourceNameLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.typeComboBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OperationForm";
            this.ShowInTaskbar = false;
            this.Text = "Operation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OperationForm_FormClosing);
            this.Load += new System.EventHandler(this.OperationForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label sourceNameLabel;
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.Label destinationLabel;
        private System.Windows.Forms.Label adminLabel;
        private System.Windows.Forms.TextBox sqlServerTextBox;
        private System.Windows.Forms.Label sqlServerLabel;
        private System.Windows.Forms.Label dbLoginLabel;
        private System.Windows.Forms.Label dbPasswordLabel;
        private System.Windows.Forms.TextBox dbLoginTextBox;
        private System.Windows.Forms.TextBox dbPasswordTextBox;
        private System.Windows.Forms.ComboBox errorComboBox;
        private System.Windows.Forms.TextBox destinationTextBox;
        private System.Windows.Forms.CheckBox adminCheckBox;
        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.TextBox sourceNameTextBox;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.ComboBox parentComboBox;
        private System.Windows.Forms.Label parentLabel;
        private System.Windows.Forms.Label generateFolderLabel;
        private System.Windows.Forms.CheckBox generateFolderCheckBox;
        private System.Windows.Forms.Button selectDestinationButton;
        private System.Windows.Forms.Button selectSourceButton;
    }
}