using Gma.System.MouseKeyHook;
using System.Windows;
using Forms = System.Windows.Forms;
using LockCursorInMonitor.Interop;
using LockCursorInMonitor.Configurations;
using System.IO;
using System.Reflection;
using System;
using System.ComponentModel;

namespace LockCursorInMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
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
            ApplyConfigurations();
        }

        /// <summary>
        /// Applies saved settings to the UI so that the UI represents the saved settings of the user.
        /// </summary>
        public void ApplyConfigurations()
        {
            AppConfigs appConfigs = Configs.ConfigsTools.GetConfigs<AppConfigs>();
            ActivatedSwitch.IsOn = appConfigs.Activated;
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
            // Ctrl is pressed
            if (Native.GetKeyState(VirtualKeyStates.VK_CONTROL) < 0)
            {
                CursorLock.LockCursor();
            }
        }

        private void GlobalHookFocusChanged(object sender, Forms.MouseEventArgs e)
        {
            if (Native.GetKeyState(VirtualKeyStates.VK_CONTROL) < 0)
            {
                CursorLock.LockCursor();
            }
        }

        private void ActivatedSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            AppConfigs appConfigs = Configs.ConfigsTools.GetConfigs<AppConfigs>();
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

                appConfigs.Activated = true;
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

                appConfigs.Activated = false;
            }
            appConfigs.Save();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Setting cancel to true will cancel the close request so the application is not closed
            e.Cancel = true;
            this.Hide();

            base.OnClosing(e);
        }
    }
}
