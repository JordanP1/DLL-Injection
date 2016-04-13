namespace Controller
{
    partial class Controller
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
            this.listBoxProcessList = new System.Windows.Forms.ListBox();
            this.textBoxPrintString = new System.Windows.Forms.TextBox();
            this.buttonSendString = new System.Windows.Forms.Button();
            this.groupBoxProcessList = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanelController = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelPrintString = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelIncreaseIndex = new System.Windows.Forms.TableLayoutPanel();
            this.labelChangeIncrease = new System.Windows.Forms.Label();
            this.numericUpDownIncreaseIndex = new System.Windows.Forms.NumericUpDown();
            this.buttonTriggerIncrease = new System.Windows.Forms.Button();
            this.groupBoxProcessList.SuspendLayout();
            this.tableLayoutPanelController.SuspendLayout();
            this.tableLayoutPanelPrintString.SuspendLayout();
            this.tableLayoutPanelIncreaseIndex.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIncreaseIndex)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxProcessList
            // 
            this.listBoxProcessList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxProcessList.FormattingEnabled = true;
            this.listBoxProcessList.ItemHeight = 16;
            this.listBoxProcessList.Location = new System.Drawing.Point(3, 18);
            this.listBoxProcessList.Margin = new System.Windows.Forms.Padding(4);
            this.listBoxProcessList.Name = "listBoxProcessList";
            this.listBoxProcessList.Size = new System.Drawing.Size(378, 137);
            this.listBoxProcessList.TabIndex = 0;
            this.listBoxProcessList.SelectedIndexChanged += new System.EventHandler(this.listBoxProcessList_SelectedIndexChanged);
            // 
            // textBoxPrintString
            // 
            this.textBoxPrintString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPrintString.Location = new System.Drawing.Point(3, 4);
            this.textBoxPrintString.MaxLength = 512;
            this.textBoxPrintString.Name = "textBoxPrintString";
            this.textBoxPrintString.Size = new System.Drawing.Size(297, 22);
            this.textBoxPrintString.TabIndex = 0;
            this.textBoxPrintString.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxPrintString_KeyDown);
            // 
            // buttonSendString
            // 
            this.buttonSendString.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonSendString.Location = new System.Drawing.Point(306, 3);
            this.buttonSendString.Name = "buttonSendString";
            this.buttonSendString.Size = new System.Drawing.Size(75, 24);
            this.buttonSendString.TabIndex = 2;
            this.buttonSendString.Text = "Send";
            this.buttonSendString.UseVisualStyleBackColor = true;
            this.buttonSendString.Click += new System.EventHandler(this.buttonSendString_Click);
            // 
            // groupBoxProcessList
            // 
            this.groupBoxProcessList.Controls.Add(this.listBoxProcessList);
            this.groupBoxProcessList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxProcessList.Location = new System.Drawing.Point(3, 75);
            this.groupBoxProcessList.Name = "groupBoxProcessList";
            this.groupBoxProcessList.Size = new System.Drawing.Size(384, 158);
            this.groupBoxProcessList.TabIndex = 3;
            this.groupBoxProcessList.TabStop = false;
            this.groupBoxProcessList.Text = "Process List";
            // 
            // tableLayoutPanelController
            // 
            this.tableLayoutPanelController.ColumnCount = 1;
            this.tableLayoutPanelController.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelController.Controls.Add(this.groupBoxProcessList, 0, 2);
            this.tableLayoutPanelController.Controls.Add(this.tableLayoutPanelPrintString, 0, 0);
            this.tableLayoutPanelController.Controls.Add(this.tableLayoutPanelIncreaseIndex, 0, 1);
            this.tableLayoutPanelController.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelController.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelController.Name = "tableLayoutPanelController";
            this.tableLayoutPanelController.RowCount = 3;
            this.tableLayoutPanelController.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanelController.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanelController.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelController.Size = new System.Drawing.Size(390, 236);
            this.tableLayoutPanelController.TabIndex = 0;
            // 
            // tableLayoutPanelPrintString
            // 
            this.tableLayoutPanelPrintString.ColumnCount = 2;
            this.tableLayoutPanelPrintString.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelPrintString.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tableLayoutPanelPrintString.Controls.Add(this.textBoxPrintString, 0, 0);
            this.tableLayoutPanelPrintString.Controls.Add(this.buttonSendString, 1, 0);
            this.tableLayoutPanelPrintString.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelPrintString.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelPrintString.Name = "tableLayoutPanelPrintString";
            this.tableLayoutPanelPrintString.RowCount = 1;
            this.tableLayoutPanelPrintString.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelPrintString.Size = new System.Drawing.Size(384, 30);
            this.tableLayoutPanelPrintString.TabIndex = 0;
            // 
            // tableLayoutPanelIncreaseIndex
            // 
            this.tableLayoutPanelIncreaseIndex.ColumnCount = 3;
            this.tableLayoutPanelIncreaseIndex.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelIncreaseIndex.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 109F));
            this.tableLayoutPanelIncreaseIndex.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tableLayoutPanelIncreaseIndex.Controls.Add(this.labelChangeIncrease, 0, 0);
            this.tableLayoutPanelIncreaseIndex.Controls.Add(this.numericUpDownIncreaseIndex, 1, 0);
            this.tableLayoutPanelIncreaseIndex.Controls.Add(this.buttonTriggerIncrease, 2, 0);
            this.tableLayoutPanelIncreaseIndex.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelIncreaseIndex.Location = new System.Drawing.Point(3, 39);
            this.tableLayoutPanelIncreaseIndex.Name = "tableLayoutPanelIncreaseIndex";
            this.tableLayoutPanelIncreaseIndex.RowCount = 1;
            this.tableLayoutPanelIncreaseIndex.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelIncreaseIndex.Size = new System.Drawing.Size(384, 30);
            this.tableLayoutPanelIncreaseIndex.TabIndex = 4;
            // 
            // labelChangeIncrease
            // 
            this.labelChangeIncrease.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labelChangeIncrease.Location = new System.Drawing.Point(3, 6);
            this.labelChangeIncrease.Name = "labelChangeIncrease";
            this.labelChangeIncrease.Size = new System.Drawing.Size(188, 18);
            this.labelChangeIncrease.TabIndex = 0;
            this.labelChangeIncrease.Text = "Change increase value to:";
            this.labelChangeIncrease.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // numericUpDownIncreaseIndex
            // 
            this.numericUpDownIncreaseIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownIncreaseIndex.Location = new System.Drawing.Point(197, 4);
            this.numericUpDownIncreaseIndex.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDownIncreaseIndex.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numericUpDownIncreaseIndex.Name = "numericUpDownIncreaseIndex";
            this.numericUpDownIncreaseIndex.Size = new System.Drawing.Size(103, 22);
            this.numericUpDownIncreaseIndex.TabIndex = 1;
            this.numericUpDownIncreaseIndex.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDownIncreaseIndex.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownIncreaseIndex.ValueChanged += new System.EventHandler(this.numericUpDownIncreaseIndex_ValueChanged);
            // 
            // buttonTriggerIncrease
            // 
            this.buttonTriggerIncrease.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonTriggerIncrease.Location = new System.Drawing.Point(306, 3);
            this.buttonTriggerIncrease.Name = "buttonTriggerIncrease";
            this.buttonTriggerIncrease.Size = new System.Drawing.Size(75, 24);
            this.buttonTriggerIncrease.TabIndex = 2;
            this.buttonTriggerIncrease.Text = "Trigger";
            this.buttonTriggerIncrease.UseVisualStyleBackColor = true;
            this.buttonTriggerIncrease.Click += new System.EventHandler(this.buttonTriggerIncrease_Click);
            // 
            // Controller
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 236);
            this.Controls.Add(this.tableLayoutPanelController);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(213, 160);
            this.Name = "Controller";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Controller";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Controller_FormClosing);
            this.groupBoxProcessList.ResumeLayout(false);
            this.tableLayoutPanelController.ResumeLayout(false);
            this.tableLayoutPanelPrintString.ResumeLayout(false);
            this.tableLayoutPanelPrintString.PerformLayout();
            this.tableLayoutPanelIncreaseIndex.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownIncreaseIndex)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxProcessList;
        private System.Windows.Forms.TextBox textBoxPrintString;
        private System.Windows.Forms.Button buttonSendString;
        private System.Windows.Forms.GroupBox groupBoxProcessList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelController;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelPrintString;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelIncreaseIndex;
        private System.Windows.Forms.Label labelChangeIncrease;
        private System.Windows.Forms.NumericUpDown numericUpDownIncreaseIndex;
        private System.Windows.Forms.Button buttonTriggerIncrease;
    }
}

