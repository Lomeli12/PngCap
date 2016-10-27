
namespace PngCap
{
    partial class About
    {
        /// <summary>
        /// Designer variable used to keep track of non-visual components.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label aboutLabel;
        private System.Windows.Forms.Button openSiteBtn;
        
        /// <summary>
        /// Disposes resources used by the form.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        
        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.aboutLabel = new System.Windows.Forms.Label();
            this.openSiteBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // aboutLabel
            // 
            this.aboutLabel.Location = new System.Drawing.Point(12, 9);
            this.aboutLabel.Name = "aboutLabel";
            this.aboutLabel.Size = new System.Drawing.Size(222, 58);
            this.aboutLabel.TabIndex = 0;
            this.aboutLabel.Text = "PngCap v1.0.1\r\nCopyright © 2016 Anthony Lomeli\r\nhttps://lomeli12.net/";
            // 
            // openSiteBtn
            // 
            this.openSiteBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.openSiteBtn.Location = new System.Drawing.Point(12, 70);
            this.openSiteBtn.Name = "openSiteBtn";
            this.openSiteBtn.Size = new System.Drawing.Size(222, 30);
            this.openSiteBtn.TabIndex = 1;
            this.openSiteBtn.Text = "Open Website";
            this.openSiteBtn.UseVisualStyleBackColor = true;
            this.openSiteBtn.Click += new System.EventHandler(this.OpenSiteBtnClick);
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(248, 112);
            this.Controls.Add(this.openSiteBtn);
            this.Controls.Add(this.aboutLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            this.ResumeLayout(false);

        }
    }
}
