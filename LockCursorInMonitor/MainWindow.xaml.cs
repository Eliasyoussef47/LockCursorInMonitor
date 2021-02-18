using Gma.System.MouseKeyHook;
using System.Windows;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System;

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

        private void GlobalHookKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey)
            {
                if (!CursorLock.Locked)
                {
                    CursorLock.LockCursor();
                }
            }
        }

        private void GlobalHookKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.LControlKey || e.KeyCode == Keys.RControlKey)
            {
                if (CursorLock.Locked)
                {
                    CursorLock.UnlockCursor();
                }
            }
        }

        private void GlobalHookDragStarted(object sender, MouseEventExtArgs e)
        {
            Trace.WriteLine(CursorLock.TraceCounter + ") DragStarted");
            //foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(e))
            //{
            //    string name = descriptor.Name;
            //    object value = descriptor.GetValue(e);
            //    Trace.WriteLine($"{name}={value}");
            //}
            CursorLock.Locked = false;
        }

        //private void GlobalHookMouseUp(object sender, MouseEventExtArgs e)
        //{
        //    Trace.WriteLine("MouseUp");
        //    foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(e))
        //    {
        //        string name = descriptor.Name;
        //        object value = descriptor.GetValue(e);
        //        Trace.WriteLine($"{name}={value}");
        //    }
        //    CursorLock.UnlockCursor();
        //}

        private void ActivatedSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ModernWpf.Controls.ToggleSwitch activatedSwitch = (ModernWpf.Controls.ToggleSwitch)sender;
            if (activatedSwitch.IsOn)
            {
                m_GlobalHook = Hook.GlobalEvents();

                m_GlobalHook.KeyDown += GlobalHookKeyDown;
                m_GlobalHook.KeyUp += GlobalHookKeyUp;
                m_GlobalHook.MouseDragStartedExt += GlobalHookDragStarted;
                //m_GlobalHook.MouseUpExt += GlobalHookMouseUp;
            }
            else
            {
                m_GlobalHook.KeyDown -= GlobalHookKeyDown;
                m_GlobalHook.KeyUp -= GlobalHookKeyUp;

                m_GlobalHook.Dispose();
            }
        }
    }
}
