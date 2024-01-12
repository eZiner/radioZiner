using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace radioZiner
{
    public static class ConsoleUtil
    {
        // Found here: https://stackoverflow.com/questions/813086/can-i-send-a-ctrl-c-sigint-to-an-application-on-windows
        // ... and here: https://stanislavs.org/stopping-command-line-applications-programatically-with-ctrl-c-events-from-net/
        // Alternative: https://github.com/madelson/MedallionShell/tree/master

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool Add);

        delegate bool ConsoleCtrlDelegate(CtrlTypes CtrlType);

        // Enumerated type for the control messages sent to the handler routine
        enum CtrlTypes : uint
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GenerateConsoleCtrlEvent(CtrlTypes dwCtrlEvent, uint dwProcessGroupId);

        public static void StopProgram(Process proc)
        {
            //This does not require the console window to be visible.
            if (AttachConsole((uint)proc.Id))
            {
                // Disable Ctrl-C handling for our program
                SetConsoleCtrlHandler(null, true);
                GenerateConsoleCtrlEvent(CtrlTypes.CTRL_C_EVENT, 0);

                //Moved this command up on suggestion from Timothy Jannace (see comments below)
                FreeConsole();

                // Must wait here. If we don't and re-enable Ctrl-C
                // handling below too fast, we might terminate ourselves.
                proc.WaitForExit(2000);

                //Re-enable Ctrl-C handling or any subsequently started
                //programs will inherit the disabled state.
                SetConsoleCtrlHandler(null, false);
            }
        }

        public static Process StartProgram(String FileName, String Arguments)
        {
            Process p = new Process();
            if (File.Exists(FileName))
            {
                try
                {
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.FileName = FileName;
                    p.StartInfo.Arguments = Arguments;
                    p.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ": " + FileName + " " + Arguments);
                }
            }
            else
            {
                MessageBox.Show("Not found: " + FileName);
            }
            return p;
        }
    }
}
