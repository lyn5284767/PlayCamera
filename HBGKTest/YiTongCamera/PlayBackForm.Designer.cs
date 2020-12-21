namespace HBGKTest.YiTongCamera
{
    partial class PlayBackForm
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
            this.trackBarLocalPlayPos = new System.Windows.Forms.TrackBar();
            this.pictureBoxVideoWnd = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLocalPlayPos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideoWnd)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBarLocalPlayPos
            // 
            this.trackBarLocalPlayPos.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.trackBarLocalPlayPos.Location = new System.Drawing.Point(0, 381);
            this.trackBarLocalPlayPos.Name = "trackBarLocalPlayPos";
            this.trackBarLocalPlayPos.Size = new System.Drawing.Size(800, 69);
            this.trackBarLocalPlayPos.TabIndex = 0;
            this.trackBarLocalPlayPos.Scroll += new System.EventHandler(this.trackBarLocalPlayPos_Scroll);
            // 
            // pictureBoxVideoWnd
            // 
            this.pictureBoxVideoWnd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxVideoWnd.Location = new System.Drawing.Point(0, 0);
            this.pictureBoxVideoWnd.Name = "pictureBoxVideoWnd";
            this.pictureBoxVideoWnd.Size = new System.Drawing.Size(800, 381);
            this.pictureBoxVideoWnd.TabIndex = 1;
            this.pictureBoxVideoWnd.TabStop = false;
            // 
            // PlayBackForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pictureBoxVideoWnd);
            this.Controls.Add(this.trackBarLocalPlayPos);
            this.Name = "PlayBackForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "一通摄像头回放";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLocalPlayPos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideoWnd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBarLocalPlayPos;
        private System.Windows.Forms.PictureBox pictureBoxVideoWnd;
    }
}