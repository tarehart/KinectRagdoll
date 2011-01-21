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
            this.comboPlaceholder = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.Location = new System.Drawing.Point(12, 12);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(463, 359);
            this.grid.TabIndex = 0;
            this.grid.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.grid_SelectedGridItemChanged);
            // 
            // zoomIn
            // 
            this.zoomIn.Location = new System.Drawing.Point(13, 378);
            this.zoomIn.Name = "zoomIn";
            this.zoomIn.Size = new System.Drawing.Size(75, 23);
            this.zoomIn.TabIndex = 1;
            this.zoomIn.Text = "Zoom In";
            this.zoomIn.UseVisualStyleBackColor = true;
            this.zoomIn.Click += new System.EventHandler(this.zoomIn_Click);
            // 
            // zoomOut
            // 
            this.zoomOut.Location = new System.Drawing.Point(95, 378);
            this.zoomOut.Name = "zoomOut";
            this.zoomOut.Size = new System.Drawing.Size(75, 23);
            this.zoomOut.TabIndex = 2;
            this.zoomOut.Text = "Zoom Out";
            this.zoomOut.UseVisualStyleBackColor = true;
            this.zoomOut.Click += new System.EventHandler(this.zoomOut_Click);
            // 
            // comboPlaceholder
            // 
            this.comboPlaceholder.FormattingEnabled = true;
            this.comboPlaceholder.Location = new System.Drawing.Point(197, 378);
            this.comboPlaceholder.Name = "comboPlaceholder";
            this.comboPlaceholder.Size = new System.Drawing.Size(278, 21);
            this.comboPlaceholder.TabIndex = 3;
            // 
            // PropertyEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 429);
            this.Controls.Add(this.comboPlaceholder);
            this.Controls.Add(this.zoomOut);
            this.Controls.Add(this.zoomIn);
            this.Controls.Add(this.grid);
            this.Name = "PropertyEditorForm";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid grid;
        private System.Windows.Forms.Button zoomIn;
        private System.Windows.Forms.Button zoomOut;
        private System.Windows.Forms.ComboBox comboPlaceholder;
    }
}