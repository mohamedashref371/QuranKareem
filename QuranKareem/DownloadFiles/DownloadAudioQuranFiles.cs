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
        }

        private string path = "";
        private string directory = "";
        private Panel currentPanel;
        private Dictionary<string, Panel> panels = new Dictionary<string, Panel>();

        public void InitializeAudioFile(string path = "")
        {
            if (path != null && path.Trim() != "" && this.path != path)
            {
                if (currentPanel != null)
                {
                    currentPanel.Visible = false;
                    currentPanel = null;
                }
                directory = Directory.GetParent(path + "\\").Name;
                if (panels.ContainsKey(directory))
                {
                    currentPanel = panels[directory];
                    currentPanel.Visible = true;
                }
                else
                {
                    currentPanel = new Panel()
                    {
                        Location = new Point(0, 0),
                        ClientSize = fileViewPage.ClientSize ,
                        AutoScroll = true
                    };
                    fileViewPage.Controls.Add(currentPanel);
                    panels.Add(directory, currentPanel);

                    FileViewControl baseView = new FileViewControl(BaseCheckBox_CheckChanged, DownloadBaseBtn_Click)
                    {
                        Location = new Point(0, 10)
                    };
                    this.path = Path.GetFullPath(path);
                    currentPanel.Controls.Clear();
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
                    currentPanel.Visible = true;
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

        private void DownloadBtn_Click(object sender, EventArgs e)
        {
            FileDownloadingControl control = new FileDownloadingControl((FileViewControl)((Button)sender).Tag);
            flowLayoutPanel.Controls.Add(control);
        }

        private void DownloadBaseBtn_Click(object sender, EventArgs e)
        {
            FileViewControl fvc;
            for (int i = 1; i < currentPanel.Controls.Count; i++)
            {
                fvc = (FileViewControl)currentPanel.Controls[i];
                if (fvc.Checked)
                    fvc.Clicked(null, null);
            }
        }

        private void BaseCheckBox_CheckChanged(object sender, EventArgs e)
        {
            if (sender is CheckBox chBox)
                for (int i = 1; i < currentPanel.Controls.Count; i++)
                    ((FileViewControl)currentPanel.Controls[i]).Checked = chBox.Checked;
        }
    }
}
