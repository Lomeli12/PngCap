using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace PngCap {
    public partial class About : Form {
        public About() {
            InitializeComponent();
            aboutLabel.Text = PngCap.ABOUT;
        }
        void OpenSiteBtnClick(object sender, EventArgs e) {
            Process.Start(PngCap.HOMEPAGE);
        }
    }
}
