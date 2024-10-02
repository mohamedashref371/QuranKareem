using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuranKareem
{
    internal class FileDownloadingControl : Control
    {
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

        public void Initialize()
        {
            ClientSize = ControlSize;
            Controls.Add(fileName);
            Controls.Add(folderName);
            Controls.Add(status);
            Controls.Add(removeBtn);
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
                    case Status.Waiting:
                        status.Text = "ينتظر";
                        status.ForeColor = Color.FromArgb(200, 150, 0);
                        nextBtn.Enabled = true;
                        break;
                    case Status.Ready:
                        status.Text = "يستعد";
                        status.ForeColor = Color.FromArgb(0, 150, 200);
                        nextBtn.Enabled = false;
                        break;
                    case Status.Downloading:
                        status.Text = "جارٍ التحميل";
                        status.ForeColor = Color.FromArgb(0, 150, 200);
                        removeBtn.Enabled = false;
                        break;
                    case Status.Downloaded:
                        Parent.Controls.Remove(this);
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
            fileViewControl.Status = Status.Waiting;
            Initialize();
            Controls.Add(nextBtn);
            if (backColor.A == 255) BackColor = backColor;

            nextBtn.Click += NextBtn_Click;
            removeBtn.Click += RemoveBtn_Click;
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
            Parent.Controls.Remove(this);
        }
    }
}
