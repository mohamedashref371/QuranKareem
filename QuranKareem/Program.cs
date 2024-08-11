using System;
using System.Windows.Forms;

namespace QuranKareem
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AssemblyResolve.AssemblyResolveEventHandler();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string errorMessage = "حدث خطأ غير متوقع، سيتم إغلاق البرنامج";
            Application.ThreadException += (sender, e) => MessageBox.Show(errorMessage);
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => MessageBox.Show(errorMessage);

            try
            {
                System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName + Application.CompanyName, out bool createdNew);
                if (createdNew /* لمنع فتح نسختين من البرنامج */)
                {
                    Application.Run(new Form1());
                    mutex.ReleaseMutex();
                }
                else MessageBox.Show("هناك نسخة من البرنامج مفتوحة");
            }
            catch { MessageBox.Show(errorMessage); }
        }
    }
}
