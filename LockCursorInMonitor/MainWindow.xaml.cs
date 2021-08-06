using Configs;
using Gma.System.MouseKeyHook;
using LockCursorInMonitor.Configurations;
using LockCursorInMonitor.Interop;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Forms = System.Windows.Forms;

namespace LockCursorInMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private IKeyboardMouseEvents m_GlobalHook;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CursorLockingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //ApplyConfigurations();
        }

        private void CursorLockingWindow_Initialized(object sender, EventArgs e)
        {
            ApplyConfigurations();

            #region Hide at startup
            bool runsAtStartup = RunsAtStartup();
            if (runsAtStartup)
            {
                CursorLockingWindow.Visibility = Visibility.Hidden;
            }
            #endregion
        }

        /// <summary>
        /// Applies saved settings to the UI so that the UI represents the saved settings of the user.
        /// </summary>
        public void ApplyConfigurations()
        {
            AppConfigs appConfigs = ConfigsTools.GetConfigs<AppConfigs>();
            ActivatedSwitch.IsOn = appConfigs.Activated;
            App app = (App)Application.Current;
            app.activatedMenuItem.Checked = ActivatedSwitch.IsOn;
            string appname = Assembly.GetEntryAssembly().GetName().Name;
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                string rkValue = (string)rk.GetValue(appname, "");
                if (rkValue != "")
                {
                    RunAtStartupSwitch.IsOn = true;
                }
                else
                {
                    RunAtStartupSwitch.IsOn = false;
                }
            }
        }

        private void GlobalHookCtrlDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Forms.Keys.LControlKey || e.KeyCode == Forms.Keys.RControlKey)
            {
                if (!CursorLock.Locked)
                {
                    CursorLock.LockCursor();
                }
            }
        }

        private void GlobalHookCtrlUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Forms.Keys.LControlKey || e.KeyCode == Forms.Keys.RControlKey)
            {
                if (CursorLock.Locked)
                {
                    CursorLock.UnlockCursor();
                }
            }
        }

        private void GlobalHookFocusChanged(object sender, MouseEventExtArgs e)
        {
            int ctrlKeyState = Native.GetKeyState(VirtualKeyStates.VK_CONTROL);
            // Ctrl is pressed.
            if (!CursorLock.Locked && ctrlKeyState < 0)
            {
                CursorLock.LockCursor();
            }
            // Ctrl not pressed.
            else if (CursorLock.Locked && ctrlKeyState >= 0)
            {
                CursorLock.UnlockCursor();
            }
        }

        private void GlobalHookFocusChanged(object sender, Forms.MouseEventArgs e)
        {
            int ctrlKeyState = Native.GetKeyState(VirtualKeyStates.VK_CONTROL);
            // Ctrl is pressed.
            if (ctrlKeyState < 0)
            {
                CursorLock.LockCursor();
            }
            // Ctrl not pressed.
            else if (ctrlKeyState >= 0)
            {
                CursorLock.UnlockCursor();
            }
        }

        private void ActivatedSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ModernWpf.Controls.ToggleSwitch activatedSwitch = (ModernWpf.Controls.ToggleSwitch)sender;
            if (activatedSwitch.IsOn)
            {
                m_GlobalHook = Hook.GlobalEvents();

                m_GlobalHook.KeyDown += GlobalHookCtrlDown;
                m_GlobalHook.KeyUp += GlobalHookCtrlUp;
                m_GlobalHook.MouseMove += GlobalHookFocusChanged;
                m_GlobalHook.MouseMoveExt += GlobalHookFocusChanged;
                m_GlobalHook.MouseDown += GlobalHookFocusChanged;
                m_GlobalHook.MouseDownExt += GlobalHookFocusChanged;
                m_GlobalHook.MouseDragStarted += GlobalHookFocusChanged;
                m_GlobalHook.MouseDragStartedExt += GlobalHookFocusChanged;
            }
            else
            {
                m_GlobalHook.KeyDown -= GlobalHookCtrlDown;
                m_GlobalHook.KeyUp -= GlobalHookCtrlUp;
                m_GlobalHook.MouseMove -= GlobalHookFocusChanged;
                m_GlobalHook.MouseMoveExt -= GlobalHookFocusChanged;
                m_GlobalHook.MouseDown -= GlobalHookFocusChanged;
                m_GlobalHook.MouseDownExt -= GlobalHookFocusChanged;
                m_GlobalHook.MouseDragStarted -= GlobalHookFocusChanged;
                m_GlobalHook.MouseDragStartedExt -= GlobalHookFocusChanged;

                m_GlobalHook.Dispose();
            }
            AppConfigs appConfigs = AppConfigs.GetConfigs();
            App app = (App)Application.Current;
            appConfigs.Activated = activatedSwitch.IsOn;
            app.activatedMenuItem.Checked = activatedSwitch.IsOn;
            appConfigs.Save();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Setting cancel to true will cancel the close request so the application is not closed.
            e.Cancel = true;
            this.Hide();

            base.OnClosing(e);
        }

        private void RunAtStartupSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ModernWpf.Controls.ToggleSwitch runAtStartupSwitch = (ModernWpf.Controls.ToggleSwitch)sender;
            string appname = Assembly.GetEntryAssembly().GetName().Name;
            string executablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (runAtStartupSwitch.IsOn)
                {
                    rk.SetValue(appname, executablePath);
                }
                else
                {
                    rk.DeleteValue(appname, false);
                }
            }
        }

        /// <summary>
        /// Checks if the app is registered in the registry to run on startup.
        /// </summary>
        /// <returns>True if the app will run at startup, false otherwise.</returns>
        public bool RunsAtStartup()
        {
            bool result = false;
            string appname = Assembly.GetEntryAssembly().GetName().Name;
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                string rkValue = (string)rk.GetValue(appname, "");
                if (rkValue != "")
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
