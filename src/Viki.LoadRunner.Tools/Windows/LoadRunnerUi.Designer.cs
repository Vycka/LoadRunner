namespace Viki.LoadRunner.Tools.Windows
{
    partial class LoadRunnerUi
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
            this._startButton = new System.Windows.Forms.Button();
            this.resultsTextBox = new System.Windows.Forms.RichTextBox();
            this._backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this._stopButton = new System.Windows.Forms.Button();
            this.tbErrors = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // _startButton
            // 
            this._startButton.Location = new System.Drawing.Point(12, 12);
            this._startButton.Name = "_startButton";
            this._startButton.Size = new System.Drawing.Size(75, 23);
            this._startButton.TabIndex = 0;
            this._startButton.Text = "Start";
            this._startButton.UseVisualStyleBackColor = true;
            this._startButton.Click += new System.EventHandler(this._startButton_Click);
            // 
            // resultsTextBox
            // 
            this.resultsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.resultsTextBox.Location = new System.Drawing.Point(12, 41);
            this.resultsTextBox.Name = "resultsTextBox";
            this.resultsTextBox.Size = new System.Drawing.Size(321, 270);
            this.resultsTextBox.TabIndex = 2;
            this.resultsTextBox.Text = "";
            // 
            // _backgroundWorker1
            // 
            this._backgroundWorker1.WorkerSupportsCancellation = true;
            this._backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this._backgroundWorker1_DoWork);
            // 
            // _stopButton
            // 
            this._stopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._stopButton.Enabled = false;
            this._stopButton.Location = new System.Drawing.Point(537, 12);
            this._stopButton.Name = "_stopButton";
            this._stopButton.Size = new System.Drawing.Size(75, 23);
            this._stopButton.TabIndex = 3;
            this._stopButton.Text = "Stop";
            this._stopButton.UseVisualStyleBackColor = true;
            this._stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // tbErrors
            // 
            this.tbErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbErrors.Location = new System.Drawing.Point(339, 41);
            this.tbErrors.Name = "tbErrors";
            this.tbErrors.Size = new System.Drawing.Size(273, 270);
            this.tbErrors.TabIndex = 4;
            this.tbErrors.Text = "";
            // 
            // LoadRunnerUi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 324);
            this.Controls.Add(this.tbErrors);
            this.Controls.Add(this._stopButton);
            this.Controls.Add(this.resultsTextBox);
            this.Controls.Add(this._startButton);
            this.Name = "LoadRunnerUi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LoadRunnerUi";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _startButton;
        private System.Windows.Forms.RichTextBox resultsTextBox;
        private System.ComponentModel.BackgroundWorker _backgroundWorker1;
        private System.Windows.Forms.Button _stopButton;
        private System.Windows.Forms.RichTextBox tbErrors;
    }
}