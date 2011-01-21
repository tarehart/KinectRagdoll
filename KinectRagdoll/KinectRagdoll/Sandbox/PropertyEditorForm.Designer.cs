namespace KinectTest2.Sandbox
{
    partial class PropertyEditorForm
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
            this.grid = new System.Windows.Forms.PropertyGrid();
            this.zoomIn = new System.Windows.Forms.Button();
            this.zoomOut = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.selectWorld = new System.Windows.Forms.Button();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listLabel = new System.Windows.Forms.Label();
            this.multipleSelect = new System.Windows.Forms.CheckedListBox();
            this.deleteSelected = new System.Windows.Forms.Button();
            this.selectNone = new System.Windows.Forms.Button();
            this.selectAll = new System.Windows.Forms.Button();
            this.selectFromList = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.Location = new System.Drawing.Point(3, 3);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(588, 376);
            this.grid.TabIndex = 0;
            this.grid.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.grid_SelectedGridItemChanged);
            // 
            // zoomIn
            // 
            this.zoomIn.Location = new System.Drawing.Point(90, 3);
            this.zoomIn.Name = "zoomIn";
            this.zoomIn.Size = new System.Drawing.Size(95, 23);
            this.zoomIn.TabIndex = 1;
            this.zoomIn.Text = "Go to Selected";
            this.zoomIn.UseVisualStyleBackColor = true;
            this.zoomIn.Click += new System.EventHandler(this.zoomIn_Click);
            // 
            // zoomOut
            // 
            this.zoomOut.Location = new System.Drawing.Point(191, 3);
            this.zoomOut.Name = "zoomOut";
            this.zoomOut.Size = new System.Drawing.Size(75, 23);
            this.zoomOut.TabIndex = 2;
            this.zoomOut.Text = "Back";
            this.zoomOut.UseVisualStyleBackColor = true;
            this.zoomOut.Click += new System.EventHandler(this.zoomOut_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.selectWorld);
            this.splitContainer1.Panel1.Controls.Add(this.zoomIn);
            this.splitContainer1.Panel1.Controls.Add(this.zoomOut);
            this.splitContainer1.Panel1.Controls.Add(this.grid);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(594, 584);
            this.splitContainer1.SplitterDistance = 382;
            this.splitContainer1.TabIndex = 3;
            // 
            // selectWorld
            // 
            this.selectWorld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectWorld.Location = new System.Drawing.Point(499, 3);
            this.selectWorld.Name = "selectWorld";
            this.selectWorld.Size = new System.Drawing.Size(92, 23);
            this.selectWorld.TabIndex = 3;
            this.selectWorld.Text = "Select World";
            this.selectWorld.UseVisualStyleBackColor = true;
            this.selectWorld.Click += new System.EventHandler(this.selectWorld_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listLabel);
            this.splitContainer2.Panel1.Controls.Add(this.multipleSelect);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.deleteSelected);
            this.splitContainer2.Panel2.Controls.Add(this.selectNone);
            this.splitContainer2.Panel2.Controls.Add(this.selectAll);
            this.splitContainer2.Panel2.Controls.Add(this.selectFromList);
            this.splitContainer2.Size = new System.Drawing.Size(594, 198);
            this.splitContainer2.SplitterDistance = 450;
            this.splitContainer2.TabIndex = 0;
            // 
            // listLabel
            // 
            this.listLabel.AutoSize = true;
            this.listLabel.Location = new System.Drawing.Point(3, 7);
            this.listLabel.Name = "listLabel";
            this.listLabel.Size = new System.Drawing.Size(83, 13);
            this.listLabel.TabIndex = 1;
            this.listLabel.Text = "Multiple Edit List";
            // 
            // multipleSelect
            // 
            this.multipleSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.multipleSelect.CheckOnClick = true;
            this.multipleSelect.FormattingEnabled = true;
            this.multipleSelect.Location = new System.Drawing.Point(3, 25);
            this.multipleSelect.Name = "multipleSelect";
            this.multipleSelect.Size = new System.Drawing.Size(444, 169);
            this.multipleSelect.TabIndex = 0;
            this.multipleSelect.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.multipleSelect_ItemCheck);
            // 
            // deleteSelected
            // 
            this.deleteSelected.Location = new System.Drawing.Point(3, 75);
            this.deleteSelected.Name = "deleteSelected";
            this.deleteSelected.Size = new System.Drawing.Size(102, 23);
            this.deleteSelected.TabIndex = 3;
            this.deleteSelected.Text = "Delete Selected";
            this.deleteSelected.UseVisualStyleBackColor = true;
            this.deleteSelected.Click += new System.EventHandler(this.deleteSelected_Click);
            // 
            // selectNone
            // 
            this.selectNone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.selectNone.Location = new System.Drawing.Point(4, 171);
            this.selectNone.Name = "selectNone";
            this.selectNone.Size = new System.Drawing.Size(102, 23);
            this.selectNone.TabIndex = 2;
            this.selectNone.Text = "Select None";
            this.selectNone.UseVisualStyleBackColor = true;
            this.selectNone.Click += new System.EventHandler(this.selectNone_Click);
            // 
            // selectAll
            // 
            this.selectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.selectAll.Location = new System.Drawing.Point(4, 142);
            this.selectAll.Name = "selectAll";
            this.selectAll.Size = new System.Drawing.Size(102, 23);
            this.selectAll.TabIndex = 1;
            this.selectAll.Text = "Select All";
            this.selectAll.UseVisualStyleBackColor = true;
            this.selectAll.Click += new System.EventHandler(this.selectAll_Click);
            // 
            // selectFromList
            // 
            this.selectFromList.Location = new System.Drawing.Point(3, 25);
            this.selectFromList.Name = "selectFromList";
            this.selectFromList.Size = new System.Drawing.Size(103, 35);
            this.selectFromList.TabIndex = 0;
            this.selectFromList.Text = "Go to Selected";
            this.selectFromList.UseVisualStyleBackColor = true;
            this.selectFromList.Click += new System.EventHandler(this.selectFromList_Click);
            // 
            // PropertyEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 608);
            this.Controls.Add(this.splitContainer1);
            this.Name = "PropertyEditorForm";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid grid;
        private System.Windows.Forms.Button zoomIn;
        private System.Windows.Forms.Button zoomOut;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label listLabel;
        private System.Windows.Forms.CheckedListBox multipleSelect;
        private System.Windows.Forms.Button selectNone;
        private System.Windows.Forms.Button selectAll;
        private System.Windows.Forms.Button selectFromList;
        private System.Windows.Forms.Button selectWorld;
        private System.Windows.Forms.Button deleteSelected;
    }
}