using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuranKareem
{
    internal class DiscriminatorControl : Control
    {
        public readonly int Id;
        private readonly CheckBox lightPageCheck, lightAyahCheck, lightWordCheck, nightPageCheck, nightAyahCheck, nightWordCheck;
        private readonly ColorComboBox lightPageColorComboBox, lightAyahColorComboBox, lightWordColorComboBox, nightPageColorComboBox, nightAyahColorComboBox, nightWordColorComboBox;
        public static readonly Size ControlSize = new Size(800, 260);

        #region properties
        public Color LightPageColor
        {
            get => lightPageCheck.Checked ? lightPageColorComboBox.SelectedColor : Color.Empty;
            set
            {
                if (value.IsEmpty)
                    lightPageCheck.Checked = false;
                else
                {
                    lightPageCheck.Checked = true;
                    lightPageColorComboBox.SelectedColor = value;
                }
            }
        }

        public Color LightAyahColor
        {
            get => lightAyahCheck.Checked ? lightAyahColorComboBox.SelectedColor : Color.Empty;
            set
            {
                if (value.IsEmpty)
                    lightAyahCheck.Checked = false;
                else
                {
                    lightAyahCheck.Checked = true;
                    lightAyahColorComboBox.SelectedColor = value;
                }
            }
        }
        public Color LightWordColor
        {
            get => lightWordCheck.Checked ? lightWordColorComboBox.SelectedColor : Color.Empty;
            set
            {
                if (value.IsEmpty)
                    lightWordCheck.Checked = false;
                else
                {
                    lightWordCheck.Checked = true;
                    lightWordColorComboBox.SelectedColor = value;
                }
            }
        }

        public Color NightPageColor
        {
            get => nightPageCheck.Checked ? nightPageColorComboBox.SelectedColor : Color.Empty;
            set
            {
                if (value.IsEmpty)
                    nightPageCheck.Checked = false;
                else
                {
                    nightPageCheck.Checked = true;
                    nightPageColorComboBox.SelectedColor = value;
                }
            }
        }
        public Color NightAyahColor
        {
            get => nightAyahCheck.Checked ? nightAyahColorComboBox.SelectedColor : Color.Empty;
            set
            {
                if (value.IsEmpty)
                    nightAyahCheck.Checked = false;
                else
                {
                    nightAyahCheck.Checked = true;
                    nightAyahColorComboBox.SelectedColor = value;
                }
            }
        }
        public Color NightWordColor
        {
            get => nightWordCheck.Checked ? nightWordColorComboBox.SelectedColor : Color.Empty;
            set
            {
                if (value.IsEmpty)
                    nightWordCheck.Checked = false;
                else
                {
                    nightWordCheck.Checked = true;
                    nightWordColorComboBox.SelectedColor = value;
                }
            }
        }
        #endregion

        public DiscriminatorControl(int id, string s)
        {
            Id = id;
            ClientSize = ControlSize;

            #region Labels
            Label word = new Label()
            {
                Font = new Font("Tahoma", 22F),
                Location = new Point(556, 9),
                RightToLeft = RightToLeft.Yes,
                Size = new Size(168, 241),
                TabIndex = 0,
                Text = s,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label light = new Label()
            {
                Font = new Font("Tahoma", 18F),
                Location = new Point(420, 9),
                RightToLeft = RightToLeft.Yes,
                Size = new Size(130, 114),
                TabIndex = 0,
                Text = "الوضع النهاري",
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label night = new Label()
            {
                Font = new Font("Tahoma", 18F),
                Location = new Point(420, 139),
                RightToLeft = RightToLeft.Yes,
                Size = new Size(130, 111),
                TabIndex = 0,
                Text = "الوضع الليلي",
                TextAlign = ContentAlignment.MiddleCenter
            };

            Controls.Add(word);
            Controls.Add(light);
            Controls.Add(night);
            #endregion

            #region ComboBoxes
            lightPageColorComboBox = new ColorComboBox()
            {
                Font = new Font("Tahoma", 16F),
                Location = new Point(73, 9),
                Size = new Size(237, 34),
                TabIndex = 2,
                Enabled = false
            };

            lightAyahColorComboBox = new ColorComboBox()
            {
                Font = new Font("Tahoma", 16F),
                Location = new Point(73, 49),
                Size = new Size(237, 34),
                TabIndex = 4,
                Enabled = false
            };

            lightWordColorComboBox = new ColorComboBox()
            {
                Font = new Font("Tahoma", 16F),
                Location = new Point(73, 89),
                Size = new Size(237, 34),
                TabIndex = 6,
                Enabled = false
            };


            nightPageColorComboBox = new ColorComboBox()
            {
                Font = new Font("Tahoma", 16F),
                Location = new Point(73, 136),
                Size = new Size(237, 34),
                TabIndex = 8,
                Enabled = false
            };

            nightAyahColorComboBox = new ColorComboBox()
            {
                Font = new Font("Tahoma", 16F),
                Location = new Point(73, 176),
                Size = new Size(237, 34),
                TabIndex = 10,
                Enabled = false
            };

            nightWordColorComboBox = new ColorComboBox()
            {
                Font = new Font("Tahoma", 16F),
                Location = new Point(73, 216),
                Size = new Size(237, 34),
                TabIndex = 12,
                Enabled = false
            };


            Controls.Add(lightPageColorComboBox);
            Controls.Add(lightAyahColorComboBox);
            Controls.Add(lightWordColorComboBox);

            Controls.Add(nightPageColorComboBox);
            Controls.Add(nightAyahColorComboBox);
            Controls.Add(nightWordColorComboBox);
            #endregion

            #region Checks
            lightPageCheck = new CheckBox()
            {
                AutoSize = true,
                Font = new Font("Tahoma", 16F),
                Location = new Point(316, 12),
                RightToLeft = RightToLeft.Yes,
                Size = new Size(98, 31),
                TabIndex = 1,
                Text = "الصفحة",
                TextAlign = ContentAlignment.MiddleCenter,
                UseVisualStyleBackColor = true,
                Tag = lightPageColorComboBox
            };

            lightAyahCheck = new CheckBox()
            {
                AutoSize = true,
                Font = new Font("Tahoma", 16F),
                Location = new Point(346, 50),
                RightToLeft = RightToLeft.Yes,
                Size = new Size(68, 31),
                TabIndex = 3,
                Text = "الآية",
                TextAlign = ContentAlignment.MiddleCenter,
                UseVisualStyleBackColor = true,
                Tag = lightAyahColorComboBox
            };

            lightWordCheck = new CheckBox()
            {
                AutoSize = true,
                Font = new Font("Tahoma", 16F),
                Location = new Point(326, 90),
                RightToLeft = RightToLeft.Yes,
                Size = new Size(88, 31),
                TabIndex = 5,
                Text = "الكلمة",
                TextAlign = ContentAlignment.MiddleCenter,
                UseVisualStyleBackColor = true,
                Tag = lightWordColorComboBox
            };


            nightPageCheck = new CheckBox()
            {
                AutoSize = true,
                Font = new Font("Tahoma", 16F),
                Location = new Point(316, 139),
                RightToLeft = RightToLeft.Yes,
                Size = new Size(98, 31),
                TabIndex = 7,
                Text = "الصفحة",
                TextAlign = ContentAlignment.MiddleCenter,
                UseVisualStyleBackColor = true,
                Tag = nightPageColorComboBox
            };

            nightAyahCheck = new CheckBox()
            {
                AutoSize = true,
                Font = new Font("Tahoma", 16F),
                Location = new Point(346, 177),
                RightToLeft = RightToLeft.Yes,
                Size = new Size(68, 31),
                TabIndex = 9,
                Text = "الآية",
                TextAlign = ContentAlignment.MiddleCenter,
                UseVisualStyleBackColor = true,
                Tag = nightAyahColorComboBox
            };

            nightWordCheck = new CheckBox()
            {
                AutoSize = true,
                Font = new Font("Tahoma", 16F),
                Location = new Point(326, 217),
                RightToLeft = RightToLeft.Yes,
                Size = new Size(88, 31),
                TabIndex = 11,
                Text = "الكلمة",
                TextAlign = ContentAlignment.MiddleCenter,
                UseVisualStyleBackColor = true,
                Tag = nightWordColorComboBox
            };

            lightPageCheck.CheckedChanged += CheckBoxes_CheckedChanged;
            lightAyahCheck.CheckedChanged += CheckBoxes_CheckedChanged;
            lightWordCheck.CheckedChanged += CheckBoxes_CheckedChanged;

            nightPageCheck.CheckedChanged += CheckBoxes_CheckedChanged;
            nightAyahCheck.CheckedChanged += CheckBoxes_CheckedChanged;
            nightWordCheck.CheckedChanged += CheckBoxes_CheckedChanged;

            Controls.Add(lightPageCheck);
            Controls.Add(lightAyahCheck);
            Controls.Add(lightWordCheck);

            Controls.Add(nightPageCheck);
            Controls.Add(nightAyahCheck);
            Controls.Add(nightWordCheck);
            #endregion

        }

        private void CheckBoxes_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            ((ColorComboBox)check.Tag).Enabled = check.Checked;
        }

    }
}
