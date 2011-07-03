namespace KinectRagdoll.Sandbox
{
    partial class PowerupForm
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
            this.jetpack = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.peashooters = new System.Windows.Forms.RadioButton();
            this.spidersilk = new System.Windows.Forms.RadioButton();
            this.birdflap = new System.Windows.Forms.CheckBox();
            this.apply = new System.Windows.Forms.Button();
            this.remove = new System.Windows.Forms.Button();
            this.noShoot = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // jetpack
            // 
            this.jetpack.AutoSize = true;
            this.jetpack.Location = new System.Drawing.Point(12, 19);
            this.jetpack.Name = "jetpack";
            this.jetpack.Size = new System.Drawing.Size(64, 17);
            this.jetpack.TabIndex = 0;
            this.jetpack.Text = "Jetpack";
            this.jetpack.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.birdflap);
            this.groupBox1.Controls.Add(this.jetpack);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(255, 117);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Equipment";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.noShoot);
            this.groupBox2.Controls.Add(this.peashooters);
            this.groupBox2.Controls.Add(this.spidersilk);
            this.groupBox2.Location = new System.Drawing.Point(6, 65);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(243, 43);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "shootin\'";
            // 
            // peashooters
            // 
            this.peashooters.AutoSize = true;
            this.peashooters.Location = new System.Drawing.Point(145, 19);
            this.peashooters.Name = "peashooters";
            this.peashooters.Size = new System.Drawing.Size(89, 17);
            this.peashooters.TabIndex = 3;
            this.peashooters.TabStop = true;
            this.peashooters.Text = "Pea Shooters";
            this.peashooters.UseVisualStyleBackColor = true;
            // 
            // spidersilk
            // 
            this.spidersilk.AutoSize = true;
            this.spidersilk.Location = new System.Drawing.Point(64, 19);
            this.spidersilk.Name = "spidersilk";
            this.spidersilk.Size = new System.Drawing.Size(75, 17);
            this.spidersilk.TabIndex = 2;
            this.spidersilk.TabStop = true;
            this.spidersilk.Text = "Spider Silk";
            this.spidersilk.UseVisualStyleBackColor = true;
            // 
            // birdflap
            // 
            this.birdflap.AutoSize = true;
            this.birdflap.Location = new System.Drawing.Point(12, 42);
            this.birdflap.Name = "birdflap";
            this.birdflap.Size = new System.Drawing.Size(67, 17);
            this.birdflap.TabIndex = 2;
            this.birdflap.Text = "Bird Flap";
            this.birdflap.UseVisualStyleBackColor = true;
            // 
            // apply
            // 
            this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.apply.Location = new System.Drawing.Point(192, 135);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(75, 23);
            this.apply.TabIndex = 2;
            this.apply.Text = "Apply";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // remove
            // 
            this.remove.Location = new System.Drawing.Point(12, 135);
            this.remove.Name = "remove";
            this.remove.Size = new System.Drawing.Size(75, 23);
            this.remove.TabIndex = 3;
            this.remove.Text = "Remove";
            this.remove.UseVisualStyleBackColor = true;
            this.remove.Click += new System.EventHandler(this.remove_Click);
            // 
            // noShoot
            // 
            this.noShoot.AutoSize = true;
            this.noShoot.Location = new System.Drawing.Point(7, 19);
            this.noShoot.Name = "noShoot";
            this.noShoot.Size = new System.Drawing.Size(51, 17);
            this.noShoot.TabIndex = 4;
            this.noShoot.TabStop = true;
            this.noShoot.Text = "None";
            this.noShoot.UseVisualStyleBackColor = true;
            // 
            // PowerupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 170);
            this.Controls.Add(this.remove);
            this.Controls.Add(this.apply);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PowerupForm";
            this.Text = "Powerup Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox jetpack;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton peashooters;
        private System.Windows.Forms.RadioButton spidersilk;
        private System.Windows.Forms.CheckBox birdflap;
        private System.Windows.Forms.Button apply;
        private System.Windows.Forms.Button remove;
        private System.Windows.Forms.RadioButton noShoot;
    }
}