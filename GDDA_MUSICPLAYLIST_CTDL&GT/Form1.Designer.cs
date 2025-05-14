namespace GDDA_MUSICPLAYLIST_CTDL_GT
{
    partial class Form1
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btSORT = new System.Windows.Forms.Button();
            this.btADD = new System.Windows.Forms.Button();
            this.btREMOVE = new System.Windows.Forms.Button();
            this.btSWAP = new System.Windows.Forms.Button();
            this.btINSERT = new System.Windows.Forms.Button();
            this.lblTOTALTIME = new System.Windows.Forms.Label();
            this.lbCURRENTTIME = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.btPREVIOUS = new System.Windows.Forms.Button();
            this.btNEXT = new System.Windows.Forms.Button();
            this.btPLAY = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.splitContainer1.Panel1.Controls.Add(this.btSORT);
            this.splitContainer1.Panel1.Controls.Add(this.btADD);
            this.splitContainer1.Panel1.Controls.Add(this.btREMOVE);
            this.splitContainer1.Panel1.Controls.Add(this.btSWAP);
            this.splitContainer1.Panel1.Controls.Add(this.btINSERT);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.splitContainer1.Panel2.Controls.Add(this.lblTOTALTIME);
            this.splitContainer1.Panel2.Controls.Add(this.lbCURRENTTIME);
            this.splitContainer1.Panel2.Controls.Add(this.trackBar1);
            this.splitContainer1.Panel2.Controls.Add(this.btPREVIOUS);
            this.splitContainer1.Panel2.Controls.Add(this.btNEXT);
            this.splitContainer1.Panel2.Controls.Add(this.btPLAY);
            this.splitContainer1.Panel2.Controls.Add(this.listBox1);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 266;
            this.splitContainer1.TabIndex = 0;
            // 
            // btSORT
            // 
            this.btSORT.Location = new System.Drawing.Point(64, 337);
            this.btSORT.Name = "btSORT";
            this.btSORT.Size = new System.Drawing.Size(138, 37);
            this.btSORT.TabIndex = 5;
            this.btSORT.Text = "SORT";
            this.btSORT.UseVisualStyleBackColor = true;
            this.btSORT.Click += new System.EventHandler(this.btSORT_Click);
            // 
            // btADD
            // 
            this.btADD.Location = new System.Drawing.Point(64, 50);
            this.btADD.Name = "btADD";
            this.btADD.Size = new System.Drawing.Size(138, 37);
            this.btADD.TabIndex = 4;
            this.btADD.Text = "ADD";
            this.btADD.UseVisualStyleBackColor = true;
            this.btADD.Click += new System.EventHandler(this.btADD_Click);
            // 
            // btREMOVE
            // 
            this.btREMOVE.Location = new System.Drawing.Point(64, 123);
            this.btREMOVE.Name = "btREMOVE";
            this.btREMOVE.Size = new System.Drawing.Size(138, 37);
            this.btREMOVE.TabIndex = 3;
            this.btREMOVE.Text = "REMOVE";
            this.btREMOVE.UseVisualStyleBackColor = true;
            this.btREMOVE.Click += new System.EventHandler(this.btREMOVE_Click);
            // 
            // btSWAP
            // 
            this.btSWAP.Location = new System.Drawing.Point(64, 260);
            this.btSWAP.Name = "btSWAP";
            this.btSWAP.Size = new System.Drawing.Size(138, 37);
            this.btSWAP.TabIndex = 2;
            this.btSWAP.Text = "SWAP";
            this.btSWAP.UseVisualStyleBackColor = true;
            this.btSWAP.Click += new System.EventHandler(this.btSWAP_Click);
            // 
            // btINSERT
            // 
            this.btINSERT.Location = new System.Drawing.Point(64, 190);
            this.btINSERT.Name = "btINSERT";
            this.btINSERT.Size = new System.Drawing.Size(138, 37);
            this.btINSERT.TabIndex = 1;
            this.btINSERT.Text = "INSERT";
            this.btINSERT.UseVisualStyleBackColor = true;
            this.btINSERT.Click += new System.EventHandler(this.btINSERT_Click);
            // 
            // lblTOTALTIME
            // 
            this.lblTOTALTIME.AutoSize = true;
            this.lblTOTALTIME.Location = new System.Drawing.Point(461, 315);
            this.lblTOTALTIME.Name = "lblTOTALTIME";
            this.lblTOTALTIME.Size = new System.Drawing.Size(34, 13);
            this.lblTOTALTIME.TabIndex = 11;
            this.lblTOTALTIME.Text = "00:00";
            // 
            // lbCURRENTTIME
            // 
            this.lbCURRENTTIME.AutoSize = true;
            this.lbCURRENTTIME.Location = new System.Drawing.Point(30, 315);
            this.lbCURRENTTIME.Name = "lbCURRENTTIME";
            this.lbCURRENTTIME.Size = new System.Drawing.Size(34, 13);
            this.lbCURRENTTIME.TabIndex = 10;
            this.lbCURRENTTIME.Text = "00:00";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(74, 315);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(390, 45);
            this.trackBar1.TabIndex = 9;
            // 
            // btPREVIOUS
            // 
            this.btPREVIOUS.Location = new System.Drawing.Point(103, 366);
            this.btPREVIOUS.Name = "btPREVIOUS";
            this.btPREVIOUS.Size = new System.Drawing.Size(76, 37);
            this.btPREVIOUS.TabIndex = 8;
            this.btPREVIOUS.Text = "⏪";
            this.btPREVIOUS.UseVisualStyleBackColor = true;
            this.btPREVIOUS.Click += new System.EventHandler(this.btPREVIOUS_Click);
            // 
            // btNEXT
            // 
            this.btNEXT.Location = new System.Drawing.Point(329, 366);
            this.btNEXT.Name = "btNEXT";
            this.btNEXT.Size = new System.Drawing.Size(76, 37);
            this.btNEXT.TabIndex = 7;
            this.btNEXT.Text = "⏩";
            this.btNEXT.UseVisualStyleBackColor = true;
            this.btNEXT.Click += new System.EventHandler(this.btNEXT_Click);
            // 
            // btPLAY
            // 
            this.btPLAY.Location = new System.Drawing.Point(185, 366);
            this.btPLAY.Name = "btPLAY";
            this.btPLAY.Size = new System.Drawing.Size(138, 37);
            this.btPLAY.TabIndex = 6;
            this.btPLAY.Text = "▶";
            this.btPLAY.UseVisualStyleBackColor = true;
            this.btPLAY.Click += new System.EventHandler(this.btPLAY_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(3, 6);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(524, 303);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btSORT;
        private System.Windows.Forms.Button btADD;
        private System.Windows.Forms.Button btREMOVE;
        private System.Windows.Forms.Button btSWAP;
        private System.Windows.Forms.Button btINSERT;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Button btPREVIOUS;
        private System.Windows.Forms.Button btNEXT;
        private System.Windows.Forms.Button btPLAY;
        private System.Windows.Forms.Label lblTOTALTIME;
        private System.Windows.Forms.Label lbCURRENTTIME;
        private System.Windows.Forms.Timer timer1;
    }
}

