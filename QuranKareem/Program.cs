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

            Application.ThreadException += (sender, e) =>
                LogError(e.Exception.Message, e.Exception.StackTrace);
            
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Exception ex = (Exception)e.ExceptionObject;
                LogError(ex.Message, ex.StackTrace);
            };

            try
            {
                System.Threading.Mutex mutex = new System.Threading.Mutex(true, Application.ProductName + Application.CompanyName, out bool createdNew);
                if (createdNew)
                {
                    Application.Run(new Form1());
                    mutex.ReleaseMutex();
                }
                else MessageBox.Show("هناك نسخة من البرنامج مفتوحة");
            }
            catch (Exception ex)
            {
                LogError(ex.Message, ex.StackTrace);
                MessageBox.Show("حدث خطأ غير متوقع، سيتم إغلاق البرنامج");
            }
        }

        private static void LogError(string msg, string stack)
        {
            if (!System.IO.File.Exists("Errors.txt"))
                System.IO.File.CreateText("Errors.txt");

            System.IO.File.AppendAllText("Errors.txt", $"{DateTime.Now}\n{msg}\n{stack}\n------------------\n\n");
        }
    }
}
