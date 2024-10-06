using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QuranKareem
{
    public partial class DownloadAudioQuranFiles : Form
    {
        public DownloadAudioQuranFiles()
        {
            InitializeComponent();
            flowLayoutPanel.Controls.Add(new FileDownloadingControl());
        }

        private string path = "";
        private string directory = "";
        private Panel currentPanel;
        private Dictionary<string, Panel> panels = new Dictionary<string, Panel>();

        public void OpenDownloadingTab() => tabControl.SelectedTab = fileDownloadPage;

        public void InitializeAudioFile(string path = "")
        {
            tabControl.SelectedTab = fileViewPage;
            if (path != null && path.Trim() != "" && this.path != path)
            {
                if (currentPanel != null)
                {
                    currentPanel.Visible = false;
                    fileViewPage.Controls.Remove(currentPanel);
                    currentPanel = null;
                }
                directory = Directory.GetParent(path + "\\").Name;
                if (panels.ContainsKey(directory))
                {
                    currentPanel = panels[directory];
                    currentPanel.Visible = true;
                    fileViewPage.Controls.Add(currentPanel);
                }
                else
                {
                    currentPanel = new Panel()
                    {
                        Location = new Point(0, 0),
                        ClientSize = fileViewPage.ClientSize ,
                        AutoScroll = true,
                        Visible = true
                    };
                    fileViewPage.Controls.Add(currentPanel);
                    panels.Add(directory, currentPanel);

                    FileViewControl baseView = new FileViewControl(BaseCheckBox_CheckChanged, DownloadBaseBtn_Click)
                    {
                        Location = new Point(0, 10)
                    };
                    this.path = Path.GetFullPath(path);
                    currentPanel.Controls.Add(baseView);
                    string s = Path.Combine(path, "download links.txt");
                    if (File.Exists(s))
                    {
                        string[] links = File.ReadAllLines(s, Encoding.UTF8).Where(link => link.Trim() != "").ToArray();
#warning links.Length > 1
                        if (links.Length > 1)
                        {
                            InternalInitializeAudioFile(links);
                        }
                    }
                }
            }
        }
        private void InternalInitializeAudioFile(string[] links)
        {
            FileViewControl fvc;
            string fileName, fullPath;
            for (int i = 0; i < links.Length; i++)
            {
                fileName = Path.GetFileName(links[i]);
                fullPath = Path.Combine(path, fileName);
                fvc = new FileViewControl(links[i], fullPath, fileName, directory, File.Exists(fullPath))
                {
                    Location = new Point(0, FileViewControl.ControlSize.Height * (i + 1) + 15)
                };
                fvc.DownloadButtonClick += DownloadBtn_Click;
                currentPanel.Controls.Add(fvc);
            }
        }

        private Color GetColor()
        {
            Color clr;
            switch (flowLayoutPanel.Controls.Count % 8)
            {
                case 1:
                    clr = Color.FromArgb(250, 200, 200);
                    break;
                case 2:
                    clr = Color.FromArgb(250, 250, 200);
                    break;
                case 3:
                    clr = Color.FromArgb(200, 250, 200);
                    break;
                case 4:
                    clr = Color.FromArgb(200, 200, 200);
                    break;
                case 5:
                    clr = Color.FromArgb(200, 250, 250);
                    break;
                case 6:
                    clr = Color.FromArgb(200, 200, 250);
                    break;
                case 7:
                    clr = Color.FromArgb(250, 200, 250);
                    break;
                default:
                    clr = Color.FromArgb(250, 250, 250);
                    break;
            }
            return clr;
        }

        private void DownloadBtn_Click(object sender, EventArgs e)
        {
            FileDownloadingControl control = new FileDownloadingControl((FileViewControl)((Button)sender).Tag, GetColor());
            flowLayoutPanel.Controls.Add(control);
        }

        private void DownloadBaseBtn_Click(object sender, EventArgs e)
        {
            FileViewControl fvc;
            for (int i = 1; i < currentPanel.Controls.Count; i++)
            {
                fvc = (FileViewControl)currentPanel.Controls[i];
                if (fvc.Checked)
                    fvc.Clicked(sender, e);
            }
        }

        private void BaseCheckBox_CheckChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox chBox)
                for (int i = 1; i < currentPanel.Controls.Count; i++)
                    ((FileViewControl)currentPanel.Controls[i]).Checked = chBox.Checked;
        }

        private void DownloadAudioQuranFiles_FormClosing(object sender, FormClosingEventArgs e)
        {
            Owner = null;
            if (e.CloseReason != CloseReason.FormOwnerClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
