using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;

namespace LockCursorInMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly Forms.NotifyIcon notifyIcon;
        public Forms.ToolStripMenuItem activatedMenuItem;

        public App()
        {
            notifyIcon = new Forms.NotifyIcon();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            #region NotifyIcon
            // Get the icon of the app.
            using (Stream iconStream = GetResourceStream(
                new Uri("pack://application:,,,/assets/LockCursorInMonitor.ico")).Stream)
            {
                // Set the icon of the NotifyIcon as the icon of the app.
                notifyIcon.Icon = new System.Drawing.Icon(iconStream);
            }
            // Set the text as the name of the app.
            notifyIcon.Text = Assembly.GetEntryAssembly().GetName().Name;
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            // Activate toggle button.
            activatedMenuItem = new Forms.ToolStripMenuItem("Activated");
            activatedMenuItem.CheckOnClick = true;
            activatedMenuItem.CheckedChanged  += new EventHandler(activatedMenuItem_CheckedChanged);
            notifyIcon.ContextMenuStrip.Items.Add(activatedMenuItem);
            // Close app button.
            Forms.ToolStripMenuItem closeAppMenuItem = new Forms.ToolStripMenuItem("Exit");
            closeAppMenuItem.Click += CloseAppMenuItem_Click;
            notifyIcon.ContextMenuStrip.Items.Add(closeAppMenuItem);
            notifyIcon.Visible = true;
            #endregion
        }

        private void CloseAppMenuItem_Click(object sender, EventArgs e)
        {
            // Close the app.
            Current.Shutdown();
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            // Maximize the window and bring 
            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Activate();
            MainWindow.Show();
        }

        /// <summary>
        /// Runs when the button in the context menu is toggled to activate or deactivate the toggle switch that
        /// toggles the cursor locking.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void activatedMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            // 
            MainWindow wnd = (MainWindow)MainWindow;
            wnd.ActivatedSwitch.IsOn = ((Forms.ToolStripMenuItem)sender).Checked;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // Remove the 
            notifyIcon.Dispose();
        }
    }
}
