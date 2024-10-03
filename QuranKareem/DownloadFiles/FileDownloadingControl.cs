using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace QuranKareem
{
    internal class FileDownloadingControl : Control
    {
        private static FileDownloadingControl current = null;
        public static LinkedList<FileDownloadingControl> FilesList = new LinkedList<FileDownloadingControl>();

        private readonly Button nextBtn = new Button
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

        private readonly Button removeBtn = new Button
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
                    case Status.Waiting:
                        status.Text = "ينتظر";
                        status.ForeColor = Color.FromArgb(200, 150, 0);
                        nextBtn.Enabled = true;
                        FileViewControl.Status = Status.Waiting;
                        break;
                    case Status.Ready:
                        status.Text = "يستعد";
                        status.ForeColor = Color.FromArgb(0, 150, 200);
                        nextBtn.Enabled = false;
                        FileViewControl.Status = Status.Ready;
                        break;
                    case Status.Downloading:
                        status.Text = "جارٍ التحميل";
                        status.ForeColor = Color.FromArgb(0, 150, 200);
                        removeBtn.Enabled = false;
                        FileViewControl.Status = Status.Downloading;
                        break;
                    case Status.Downloaded:
                        Parent.Controls.Remove(this);
                        FileViewControl.Status = Status.Downloaded;
                        break;
                    case Status.Error:
                        Parent.Controls.Remove(this);
                        FileViewControl.Status = Status.Error;
                        break;
                }
            }
        }

        public void Initialize()
        {
            ClientSize = ControlSize;
            Controls.Add(fileName);
            Controls.Add(folderName);
            Controls.Add(status);
            Controls.Add(removeBtn);
        }

        public FileDownloadingControl()
        {
            fileName.Text = "إسم الملف";
            folderName.Text = "إسم المجلد";
            status.Text = "الحالة";
            removeBtn.Click += RemoveBaseBtn_Click;
            Initialize();
        }

        public readonly FileViewControl FileViewControl;
        public readonly LinkedListNode<FileDownloadingControl> Node;
        public FileDownloadingControl(FileViewControl fileViewControl, Color backColor)
        {
            FileViewControl = fileViewControl;
            Node = new LinkedListNode<FileDownloadingControl>(this);
            FilesList.AddLast(Node);

            fileName.Text = fileViewControl.FileName;
            folderName.Text = fileViewControl.FolderName;
            Status = Status.Waiting;
            Initialize();
            Controls.Add(nextBtn);
            if (backColor.A == 255) BackColor = backColor;

            nextBtn.Click += NextBtn_Click;
            removeBtn.Click += RemoveBtn_Click;

            if (current is null)
            {
                current = this;
                Task.Run(DownloadFiles);
            }
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            FilesList.First.Value.Status = Status.Waiting;
            FilesList.Remove(Node);
            FilesList.AddFirst(Node);
            Status = Status.Ready;
        }

        private void RemoveBtn_Click(object sender, EventArgs e)
        {
            FileViewControl.Status = File.Exists(FileViewControl.FilePath) ? Status.Exist : Status.NotExist;
            FilesList.Remove(Node);
            FilesList.First.Value.Status = Status.Ready;
            Parent.Controls.Remove(this);
        }

        private static void RemoveBaseBtn_Click(object sender, EventArgs e)
        {
            FileDownloadingControl fdc;
            while (FilesList.Count > 0)
            {
                fdc = FilesList.First.Value;
                fdc.Status = File.Exists(fdc.FileViewControl.FilePath) ? Status.Exist : Status.NotExist;
                FilesList.RemoveFirst();
                fdc.Parent.Controls.Remove(fdc);
            }
        }

        public static void DownloadFiles()
        {
            System.Net.WebClient client = new System.Net.WebClient();

            while (FilesList.Count > 0)
            {
                current = FilesList.First.Value;
                FilesList.Remove(FilesList.First);
                try
                {
                    current.Status = Status.Downloading;
                    client.DownloadFile(current.FileViewControl.FileLink, current.FileViewControl.FilePath);
                    current.Status = Status.Downloaded;
                }
                catch
                {
                    current.Status = Status.Error;
                }
            }

            current = null;
            client.Dispose();
        }
    }
}
