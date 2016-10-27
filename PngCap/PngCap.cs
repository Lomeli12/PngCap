using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using Utilities;

namespace PngCap {
    public sealed class PngCap {
        static readonly string TITLE = "PngCap";
        static readonly string SCREENSHOT_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "/ScreenShots";
        static readonly string CONFIG_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/" + TITLE;
        static readonly string CONFIG_FILE = CONFIG_FOLDER + "/config.cfg";
        static readonly Regex CLEAN_UP_REGEX = new Regex(@"[\\/]+", RegexOptions.Compiled);
        static string lastFile;
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
            notifyIcon.BalloonTipClicked += openClick;
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(PngCap));
            notifyIcon.Icon = (Icon)resources.GetObject("$this.Icon");
            notifyIcon.Text = "PngCap\nDouble-Click to open screenshot folder.";
            
            notifyMenu = new ContextMenu();
            setupContextMenu();
            notifyIcon.ContextMenu = notifyMenu;
        }
            
        static void setupContextMenu() {
            notifyMenu.MenuItems.Add(new MenuItem("Use 24-Hour Format", new EventHandler(enable24Hours)));
            notifyMenu.MenuItems.Add(new MenuItem("Disable Notification", new EventHandler(showNotification)));
            notifyMenu.MenuItems.Add("-");
            notifyMenu.MenuItems.Add(new MenuItem("Exit", new EventHandler(closeApp)));
            notifyMenu.MenuItems[0].Checked = use24hour;
            notifyMenu.MenuItems[1].Checked = notification;
        }
        
        static void keyHandler(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.PrintScreen && Clipboard.ContainsImage()) {
                if (!Directory.Exists(SCREENSHOT_FOLDER)) Directory.CreateDirectory(SCREENSHOT_FOLDER);
                var name = getTimeStampFileName();
                var fileName = SCREENSHOT_FOLDER + "/" + name;
                Clipboard.GetImage().Save(fileName, ImageFormat.Png);
                if (!notification) {
                    lastFile = fileName;
                    displayNotification(TITLE, "Saved screenshot as", name, "Click to open folder.");
                }
            }
        }
        
        static string getTimeStampFileName() {
            return use24hour ? string.Format("Screenshot_{0:yyyy-MM-dd_HH-mm-ss}.png", DateTime.Now) : string.Format("Screenshot_{0:yyyy-MM-dd_hh-mm-ss-tt}.png", DateTime.Now);
        }
        
        static void IconDoubleClick(object sender, EventArgs e) {
            Process.Start(SCREENSHOT_FOLDER);
        }
        
        static void closeApp(object sender, EventArgs e) {
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
        
        static void displayNotification(string title, params string[] message) {
            if (string.IsNullOrEmpty(title) || message == null || message.Length < 1) return;
            var msg = "";
            for (int i = 0; i < message.Length; i++) {
                msg += message[i];
                if (i != (message.Length - 1)) msg += "\n";
            }
            if (string.IsNullOrEmpty(msg)) return;
            
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipText = msg;
            notifyIcon.ShowBalloonTip(5);
        }
        
        static void openClick(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(lastFile)) return;
            browseToFile(lastFile);
            lastFile = null;
        }
        
        static void browseToFile(string file) {
            if (!File.Exists(lastFile)) return;
            var cleanPath = CLEAN_UP_REGEX.Replace(file, @"\");
            Process.Start("explorer.exe", string.Format("/select,\"{0}\"", cleanPath));
        }
    }
}
