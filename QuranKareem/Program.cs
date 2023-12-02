using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            AppDomain.CurrentDomain.AssemblyResolve += ChatGPTResponses.CurrentDomain_AssemblyResolve;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
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
            catch { MessageBox.Show("حدث خطأ غير متوقع، سيتم إغلاق البرنامج"); }
        }
    }
}
