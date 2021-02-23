﻿using Gma.System.MouseKeyHook;
using System.Windows;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System;
using LockCursorInMonitor.Interop;

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

        private void GlobalHookCtrlDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey)
            {
                if (!CursorLock.Locked)
                {
                    CursorLock.LockCursor();
                }
            }
        }

        private void GlobalHookCtrlUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey)
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

        private void GlobalHookFocusChanged(object sender, MouseEventArgs e)
        {
            if (Native.GetKeyState(VirtualKeyStates.VK_CONTROL) < 0)
            {
                CursorLock.LockCursor();
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
        }
    }
}
