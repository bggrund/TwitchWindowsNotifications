﻿using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TwitchNotifications
{
    // Code below obtained from https://codereview.stackexchange.com/questions/127742/minimize-the-console-window-to-tray
    public static class MinimizeTray
    {

        #region pInvoke

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        private enum ShowWindowCommands
        {
            Hide = 0,
            Normal = 1,
            ShowMinimized = 2,
            Maximize = 3,
            ShowMaximized = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }

        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        private const uint SC_CLOSE = 0xF060;
        private const uint MF_ENABLED = 0x00000000;
        private const uint MF_DISABLED = 0x00000002;

        #endregion

        private static NotifyIcon Tray = default(NotifyIcon);
        private static IntPtr Me = default(IntPtr);

        public static void MinimizeToTray()
        {
            Console.Title = "TwitchNotifications";

            // Get The Console Window Handle
            Me = GetConsoleWindow();

            // Disable Close Button (X)
            //EnableMenuItem(GetSystemMenu(Me, false), SC_CLOSE, (uint)(MF_ENABLED | MF_DISABLED));

            MenuItem mExit = new MenuItem("Exit", Exit);
            ContextMenu Menu = new ContextMenu(new MenuItem[] { mExit });

            Tray = new NotifyIcon()
            {
                Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location),
                Visible = true,
                Text = "TwitchNotifications",
                ContextMenu = Menu
            };
            Tray.Click += DoubleClick; //changed to single click BGG
            //Tray.Visible = true;

            // Detect When The Console Window is Minimized and Hide it
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    WINDOWPLACEMENT wPlacement = new WINDOWPLACEMENT();
                    GetWindowPlacement(Me, ref wPlacement);
                    if (wPlacement.showCmd == (int)ShowWindowCommands.ShowMinimized)
                        ShowWindow(Me, (int)ShowWindowCommands.Hide);
                    // 1 ms Delay to Avoid High CPU Usage
                    Thread.Sleep(100);//changed to sleep BGG
                }
            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            ShowWindow(Me, (int)ShowWindowCommands.Hide);

            Application.Run();
        }

        public static void ShowConsole()
        {
            ShowWindow(Me, (int)ShowWindowCommands.Hide);
        }

        private static void DoubleClick(object sender, EventArgs e)
        {
            ShowWindow(Me, (int)ShowWindowCommands.Normal);
        }

        private static void Exit(object sender, EventArgs e)
        {
            Tray.Dispose();
            Application.Exit();
            Environment.Exit(0);
        }

        private static void Wait(int timeout)
        {
            using (AutoResetEvent AREv = new AutoResetEvent(false))
                AREv.WaitOne(timeout, true);
        }
    }
}
