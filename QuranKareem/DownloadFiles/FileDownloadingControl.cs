using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuranKareem
{
    internal class FileDownloadingControl : Control
    {

        private readonly CheckBox nextBtn = new CheckBox
        {
            BackColor = Color.FromArgb(224, 224, 224),
            FlatStyle = FlatStyle.Popup,
            Font = new Font("Tahoma", 12F),
            Location = new Point(697, 9),
            Size = new Size(73, 32),
            TabIndex = 0,
            Text = "التالي",
            UseVisualStyleBackColor = true
        };

        private readonly Label fileName = new Label
        {
            Font = new Font("Tahoma", 12F),
            Location = new Point(506, 9),
            //RightToLeft = RightToLeft.Yes,
            Size = new Size(185, 32),
            TextAlign = ContentAlignment.MiddleCenter
        };

        private readonly Label folderName = new Label
        {
            Font = new Font("Tahoma", 12F),
            Location = new Point(240, 9),
            RightToLeft = RightToLeft.Yes,
            Size = new Size(260, 32),
            TextAlign = ContentAlignment.MiddleCenter
        };

        private readonly Label status = new Label
        {
            Font = new Font("Tahoma", 12F),
            Location = new Point(117, 9),
            RightToLeft = RightToLeft.Yes,
            Size = new Size(117, 32),
            TextAlign = ContentAlignment.MiddleCenter
        };

        private readonly Button remove = new Button
        {
            BackColor = Color.FromArgb(255, 192, 192),
            FlatStyle = FlatStyle.Popup,
            Font = new Font("Tahoma", 12F),
            Location = new Point(14, 9),
            RightToLeft = RightToLeft.Yes,
            Size = new Size(97, 32),
            TabIndex = 1,
            Text = "إزالة"
        };

        public static readonly Size ControlSize = new Size(782, 50);

        public void Initialize()
        {
            ClientSize = ControlSize;
            Controls.Add(nextBtn);
            Controls.Add(fileName);
            Controls.Add(folderName);
            Controls.Add(status);
            Controls.Add(remove);
        }

        private Status _status;
        public Status Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                switch (_status)
                {
                    case Status.NotExist:
                        status.Text = "";
                        status.ForeColor = Color.FromArgb(0, 0, 0);
                        break;
                    case Status.Waiting:
                        status.Text = "ينتظر";
                        status.ForeColor = Color.FromArgb(200, 150, 0);
                        break;
                    case Status.Downloading:
                        status.Text = "جارٍ التحميل";
                        status.ForeColor = Color.FromArgb(0, 150, 200);
                        break;
                    case Status.Downloaded:
                        status.Text = "تم التحميل";
                        status.ForeColor = Color.FromArgb(0, 0, 190);
                        break;
                    case Status.Exist:
                        status.Text = "موجود";
                        status.ForeColor = Color.FromArgb(0, 150, 0);
                        break;
                }
            }
        }

        public FileDownloadingControl()
        {
            fileName.Text = "إسم الملف";
            folderName.Text = "إسم المجلد";
            status.Text = "الحالة";
            Initialize();
        }

        private readonly FileViewControl fileViewControl;
        public FileDownloadingControl(FileViewControl fileViewControl)
        {
            this.fileViewControl = fileViewControl;
            fileName.Text = fileViewControl.FileName;
            folderName.Text = fileViewControl.FolderName;
            Status = Status.Waiting;
            Initialize();
        }
    }
}
