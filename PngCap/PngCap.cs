using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Utilities;

namespace PngCap {
    public sealed class PngCap {
        static readonly string TITLE = "PngCap";
        static readonly string SCREENSHOT_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "/ScreenShots";
        static readonly string CONFIG_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + TITLE;
        static readonly string CONFIG_FILE = CONFIG_FOLDER + "/config.cfg";
        static bool use24hour, notification;
        static NotifyIcon notifyIcon;
        static ContextMenu notifyMenu;
        static GlobalKeyboardHook ghook;
        static Config config;
        
        [STAThread]
        public static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            bool isFirstInstance;
            using (var mtx = new Mutex(true, TITLE, out isFirstInstance)) {
                if (isFirstInstance) {
                    loadConfig();
                    setupGlobalHooks();
                    setupTrayIcon();
                    Application.Run();
                    notifyIcon.Dispose();
                } else MessageBox.Show("PngCap is already running!");
            }
        }
        
        static void setupGlobalHooks() {
            ghook = new GlobalKeyboardHook();
            ghook.KeyUp += new KeyEventHandler(keyHandler);
            ghook.HookedKeys.Add(Keys.PrintScreen);
        }
        
        static void loadConfig() {
            if (!Directory.Exists(CONFIG_FOLDER)) Directory.CreateDirectory(CONFIG_FOLDER);
            config = new Config(CONFIG_FILE);
            use24hour = config.getBool("use24hour");
            notification = config.getBool("disableNotification");
        }
        
        static void setupTrayIcon() {
            notifyIcon = new NotifyIcon();
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += IconDoubleClick;
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(PngCap));
            notifyIcon.Icon = (Icon)resources.GetObject("$this.Icon");
            notifyIcon.Text = "PngCap\nDouble-Click to close.";
            
            notifyMenu = new ContextMenu(standardItems());
            notifyMenu.MenuItems[0].Checked = use24hour;
            notifyMenu.MenuItems[1].Checked = notification;
            notifyIcon.ContextMenu = notifyMenu;
        }
        
        static void keyHandler(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.PrintScreen && Clipboard.ContainsImage()) {
                if (!Directory.Exists(SCREENSHOT_FOLDER)) Directory.CreateDirectory(SCREENSHOT_FOLDER);
                var name = getTimeStampFileName();
                Clipboard.GetImage().Save(SCREENSHOT_FOLDER + "/" + name, ImageFormat.Png);
                if (!notification) displayNotification(TITLE, "Saved screenshot as", name);
            }
        }
        
        static string getTimeStampFileName() {
            return use24hour ? string.Format("ScreenShot-{0:yyyy-MM-dd_HH-mm-ss}.png", DateTime.Now) : string.Format("ScreenShot-{0:yyyy-MM-dd_hh-mm-ss-tt}.png", DateTime.Now);
        }
        
        static MenuItem[] standardItems() {
            return new MenuItem[] { 
                new MenuItem("Use 24-Hour Format", new EventHandler(enable24Hours)),
                new MenuItem("Disable Notification", new EventHandler(showNotification)) 
            };
        }
       
        static void IconDoubleClick(object sender, EventArgs e) {
            Application.Exit();
        }
       
        static void enable24Hours(object sender, EventArgs e) {
            notifyMenu.MenuItems[0].Checked = !notifyMenu.MenuItems[0].Checked;
            use24hour = notifyMenu.MenuItems[0].Checked;
            config.setBool("use24hour", use24hour);
        }
        
        static void showNotification(object sender, EventArgs e) {
            notifyMenu.MenuItems[1].Checked = !notifyMenu.MenuItems[1].Checked;
            notification = notifyMenu.MenuItems[1].Checked;
            config.setBool("disableNotification", notification);
        }
        
        static void displayNotification(string title, string mainLine, string second) {
            var notify = new NotifyIcon();
            notify.Visible = true;
            notify.BalloonTipTitle = title;
            notify.BalloonTipText = string.Format("{0}\n{1}", mainLine, second);
            notify.Icon = notifyIcon.Icon;
            notify.ShowBalloonTip(5);
            notify.Dispose();
        }
    }
}
