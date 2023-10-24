using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Notepad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int WM_GETTEXT = 0x000D;
        private const int WM_GETTEXTLENGTH = 0x000E;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        private string notepadText = "";
        private IntPtr notepadHandle = IntPtr.Zero;
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {

            if (notepadHandle == IntPtr.Zero)
            {
                notepadHandle = FindNotepad();
            }

            if (notepadHandle != IntPtr.Zero)
            {
                int length = SendMessage(notepadHandle, WM_GETTEXTLENGTH, 0, null);
                var sb = new StringBuilder(length + 1);
                SendMessage(notepadHandle, WM_GETTEXT, sb.Capacity, sb);
                string newText = sb.ToString();

                if (newText != notepadText)
                {
                    notepadText = newText;
                    textBox.Text = notepadText;
                }
            }
            else
            {
                MessageBox.Show("NOTEPAD НЕ ЗАПУЩЕН!");
            }
        }

        private IntPtr FindNotepad()
        {
            IntPtr mainWindowHandle = FindWindow("Notepad", null);
            if (mainWindowHandle != IntPtr.Zero)
            {
                return FindWindowEx(mainWindowHandle, IntPtr.Zero, "Edit", null);
            }
            return IntPtr.Zero;
        }
    }
}
