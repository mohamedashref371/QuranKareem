using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuranKareem
{
    internal class FileDownloadingControl : Control
    {


        private readonly FileViewControl fileViewControl;
        public FileDownloadingControl(FileViewControl fileViewControl)
        {
            this.fileViewControl = fileViewControl;
        }
    }
}
