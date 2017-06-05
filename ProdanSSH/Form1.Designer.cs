namespace ProdanSSH
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.textIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textUser = new System.Windows.Forms.TextBox();
            this.textPass = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tray = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextBandeja = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.sairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.rtbOutput = new System.Windows.Forms.RichTextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnShootToKill = new System.Windows.Forms.Button();
            this.btnListproc = new System.Windows.Forms.Button();
            this.btnRestart = new System.Windows.Forms.Button();
            this.btnLimpa = new System.Windows.Forms.Button();
            this.btnVerifica = new System.Windows.Forms.Button();
            this.btnPutty = new System.Windows.Forms.Button();
            this.contextProcKill = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.groupBox1.SuspendLayout();
            this.contextBandeja.SuspendLayout();
            this.contextProcKill.SuspendLayout();
            this.SuspendLayout();
            // 
            // textIP
            // 
            this.textIP.BackColor = System.Drawing.Color.FloralWhite;
            this.textIP.Location = new System.Drawing.Point(4, 32);
            this.textIP.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textIP.Name = "textIP";
            this.textIP.Size = new System.Drawing.Size(95, 20);
            this.textIP.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Host";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(102, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Usuario";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(208, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Senha";
            // 
            // textUser
            // 
            this.textUser.BackColor = System.Drawing.Color.FloralWhite;
            this.textUser.Location = new System.Drawing.Point(105, 32);
            this.textUser.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textUser.Name = "textUser";
            this.textUser.Size = new System.Drawing.Size(100, 20);
            this.textUser.TabIndex = 3;
            // 
            // textPass
            // 
            this.textPass.BackColor = System.Drawing.Color.FloralWhite;
            this.textPass.Location = new System.Drawing.Point(211, 32);
            this.textPass.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textPass.Name = "textPass";
            this.textPass.Size = new System.Drawing.Size(104, 20);
            this.textPass.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textPass);
            this.groupBox1.Controls.Add(this.textIP);
            this.groupBox1.Controls.Add(this.textUser);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(8, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(347, 79);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // tray
            // 
            this.tray.ContextMenuStrip = this.contextBandeja;
            this.tray.Icon = ((System.Drawing.Icon)(resources.GetObject("tray.Icon")));
            this.tray.Text = "Linux Tools";
            this.tray.Visible = true;
            this.tray.DoubleClick += new System.EventHandler(this.tray_DoubleClick);
            // 
            // contextBandeja
            // 
            this.contextBandeja.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sairToolStripMenuItem});
            this.contextBandeja.Name = "contextMenuStrip1";
            this.contextBandeja.Size = new System.Drawing.Size(94, 26);
            // 
            // sairToolStripMenuItem
            // 
            this.sairToolStripMenuItem.Name = "sairToolStripMenuItem";
            this.sairToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.sairToolStripMenuItem.Text = "Sair";
            this.sairToolStripMenuItem.Click += new System.EventHandler(this.sairToolStripMenuItem_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox1.Location = new System.Drawing.Point(479, 344);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(174, 17);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "Verificar horário na inicialização";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // rtbOutput
            // 
            this.rtbOutput.BackColor = System.Drawing.SystemColors.InfoText;
            this.rtbOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbOutput.DetectUrls = false;
            this.rtbOutput.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbOutput.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rtbOutput.Location = new System.Drawing.Point(8, 90);
            this.rtbOutput.Name = "rtbOutput";
            this.rtbOutput.ReadOnly = true;
            this.rtbOutput.Size = new System.Drawing.Size(645, 242);
            this.rtbOutput.TabIndex = 9;
            this.rtbOutput.TabStop = false;
            this.rtbOutput.Text = "";
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.Color.Snow;
            this.progressBar1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.progressBar1.Location = new System.Drawing.Point(8, 370);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(645, 10);
            this.progressBar1.TabIndex = 13;
            this.progressBar1.Visible = false;
            // 
            // btnShootToKill
            // 
            this.btnShootToKill.BackColor = System.Drawing.SystemColors.Control;
            this.btnShootToKill.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShootToKill.Image = global::ProdanSSH.Properties.Resources.death2_16;
            this.btnShootToKill.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnShootToKill.Location = new System.Drawing.Point(659, 156);
            this.btnShootToKill.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnShootToKill.Name = "btnShootToKill";
            this.btnShootToKill.Size = new System.Drawing.Size(118, 25);
            this.btnShootToKill.TabIndex = 16;
            this.btnShootToKill.Text = "Matar Processo";
            this.btnShootToKill.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnShootToKill.UseVisualStyleBackColor = false;
            this.btnShootToKill.Click += new System.EventHandler(this.btnShootToKill_Click);
            // 
            // btnListproc
            // 
            this.btnListproc.BackColor = System.Drawing.SystemColors.Control;
            this.btnListproc.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnListproc.Image = global::ProdanSSH.Properties.Resources.Gears_icon;
            this.btnListproc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnListproc.Location = new System.Drawing.Point(659, 123);
            this.btnListproc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnListproc.Name = "btnListproc";
            this.btnListproc.Size = new System.Drawing.Size(118, 25);
            this.btnListproc.TabIndex = 15;
            this.btnListproc.Text = "Listar Processos";
            this.btnListproc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnListproc.UseVisualStyleBackColor = false;
            this.btnListproc.Click += new System.EventHandler(this.btnListproc_Click);
            // 
            // btnRestart
            // 
            this.btnRestart.BackColor = System.Drawing.SystemColors.Control;
            this.btnRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestart.Image = global::ProdanSSH.Properties.Resources.rest;
            this.btnRestart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRestart.Location = new System.Drawing.Point(661, 307);
            this.btnRestart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRestart.Name = "btnRestart";
            this.btnRestart.Size = new System.Drawing.Size(118, 25);
            this.btnRestart.TabIndex = 14;
            this.btnRestart.Text = "Reiniciar Serv.";
            this.btnRestart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRestart.UseVisualStyleBackColor = false;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);
            // 
            // btnLimpa
            // 
            this.btnLimpa.Image = global::ProdanSSH.Properties.Resources._1492562768_edit_clear;
            this.btnLimpa.Location = new System.Drawing.Point(80, 338);
            this.btnLimpa.Name = "btnLimpa";
            this.btnLimpa.Size = new System.Drawing.Size(27, 26);
            this.btnLimpa.TabIndex = 12;
            this.btnLimpa.UseVisualStyleBackColor = true;
            this.btnLimpa.Click += new System.EventHandler(this.btnLimpa_Click);
            // 
            // btnVerifica
            // 
            this.btnVerifica.BackColor = System.Drawing.SystemColors.Control;
            this.btnVerifica.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerifica.Image = global::ProdanSSH.Properties.Resources.conn;
            this.btnVerifica.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVerifica.Location = new System.Drawing.Point(659, 90);
            this.btnVerifica.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnVerifica.Name = "btnVerifica";
            this.btnVerifica.Size = new System.Drawing.Size(118, 25);
            this.btnVerifica.TabIndex = 5;
            this.btnVerifica.Text = "Verificar Horario";
            this.btnVerifica.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVerifica.UseVisualStyleBackColor = false;
            this.btnVerifica.Click += new System.EventHandler(this.btnVerifica_Click);
            // 
            // btnPutty
            // 
            this.btnPutty.Image = global::ProdanSSH.Properties.Resources.putty;
            this.btnPutty.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPutty.Location = new System.Drawing.Point(8, 338);
            this.btnPutty.Name = "btnPutty";
            this.btnPutty.Size = new System.Drawing.Size(66, 26);
            this.btnPutty.TabIndex = 11;
            this.btnPutty.Text = "PuTTY";
            this.btnPutty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPutty.UseVisualStyleBackColor = true;
            this.btnPutty.Click += new System.EventHandler(this.btnPutty_Click);
            // 
            // contextProcKill
            // 
            this.contextProcKill.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripTextBox1});
            this.contextProcKill.Name = "contextProcKill";
            this.contextProcKill.Size = new System.Drawing.Size(161, 29);
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
            this.toolStripTextBox1.Tag = "";
            this.toolStripTextBox1.ToolTipText = "Numero do Processo";
            this.toolStripTextBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripTextBox1_KeyDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Snow;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(789, 384);
            this.Controls.Add(this.btnShootToKill);
            this.Controls.Add(this.btnListproc);
            this.Controls.Add(this.btnRestart);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnLimpa);
            this.Controls.Add(this.btnVerifica);
            this.Controls.Add(this.btnPutty);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.rtbOutput);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ProdanSSH";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextBandeja.ResumeLayout(false);
            this.contextProcKill.ResumeLayout(false);
            this.contextProcKill.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textUser;
        private System.Windows.Forms.TextBox textPass;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnVerifica;
        private System.Windows.Forms.NotifyIcon tray;
        private System.Windows.Forms.ContextMenuStrip contextBandeja;
        private System.Windows.Forms.ToolStripMenuItem sairToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button btnPutty;
        private System.Windows.Forms.RichTextBox rtbOutput;
        private System.Windows.Forms.Button btnLimpa;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Button btnListproc;
        private System.Windows.Forms.Button btnShootToKill;
        private System.Windows.Forms.ContextMenuStrip contextProcKill;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
    }
}

