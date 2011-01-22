namespace KinectTest2.Sandbox
{
    partial class JointForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.CheckBox pin;
        private System.Windows.Forms.CheckBox motorEnabled;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox motorTorque;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox motorSpeed;

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
            this.pin = new System.Windows.Forms.CheckBox();
            this.motorEnabled = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.motorSpeed = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.motorTorque = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pin
            // 
            this.pin.AutoSize = true;
            this.pin.Location = new System.Drawing.Point(12, 12);
            this.pin.Name = "pin";
            this.pin.Size = new System.Drawing.Size(114, 17);
            this.pin.TabIndex = 0;
            this.pin.Text = "Pin to Background";
            this.pin.UseVisualStyleBackColor = true;
            // 
            // motorEnabled
            // 
            this.motorEnabled.AutoSize = true;
            this.motorEnabled.Location = new System.Drawing.Point(13, 37);
            this.motorEnabled.Name = "motorEnabled";
            this.motorEnabled.Size = new System.Drawing.Size(89, 17);
            this.motorEnabled.TabIndex = 1;
            this.motorEnabled.Text = "Enable Motor";
            this.motorEnabled.UseVisualStyleBackColor = true;
            this.motorEnabled.CheckedChanged += new System.EventHandler(this.motorEnabled_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Motor Speed";
            // 
            // motorSpeed
            // 
            this.motorSpeed.Location = new System.Drawing.Point(114, 64);
            this.motorSpeed.Name = "motorSpeed";
            this.motorSpeed.Size = new System.Drawing.Size(100, 20);
            this.motorSpeed.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Max Motor Torque";
            // 
            // motorTorque
            // 
            this.motorTorque.Location = new System.Drawing.Point(114, 90);
            this.motorTorque.Name = "motorTorque";
            this.motorTorque.Size = new System.Drawing.Size(100, 20);
            this.motorTorque.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(36, 129);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(162, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Shift-click in game to place";
            // 
            // JointForm
            // 
            this.ClientSize = new System.Drawing.Size(242, 151);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.motorTorque);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.motorSpeed);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.motorEnabled);
            this.Controls.Add(this.pin);
            this.Name = "JointForm";
            this.Text = "Create Joint";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        
    }
}