using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class ColorComboBox : ComboBox
{

    public ColorComboBox()
    {
        List<object> data = typeof(Color).GetProperties()
            .Where(x => x.PropertyType == typeof(Color))
            .Select(x => x.GetValue(null)).ToList();
        data.Add(Color.FromName("Custom..."));
        DataSource = data;

        MaxDropDownItems = 10;
        IntegralHeight = false;
        DrawMode = DrawMode.OwnerDrawFixed;
        DropDownStyle = ComboBoxStyle.DropDownList;
        DrawItem += ColorComboBox_DrawItem;
        FormattingEnabled = true;

        // Handle custom color selection
        SelectedIndexChanged += ColorComboBox_SelectedIndexChanged;
    }

    private void ColorComboBox_DrawItem(object sender, DrawItemEventArgs e)
    {
        e.DrawBackground();
        if (e.Index >= 0)
        {
            //var text = GetItemText(Items[e.Index]).Replace(";", ",");
            var text = ((Color)Items[e.Index]).Name;
            if (text == "0") text = "Empty Color";
            if (text == "Custom..." || text == "Empty Color") // Handle custom item differently
            {
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
                TextRenderer.DrawText(e.Graphics, text, Font, e.Bounds, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
            else
            {
                var color = (Color)Items[e.Index];
                var rectSize = (int)(e.Bounds.Height / 1.5);
                var rectColor = new Rectangle(e.Bounds.Left + 1, e.Bounds.Top + 1, rectSize, rectSize);
                var rectText = Rectangle.FromLTRB(rectColor.Right + 2, e.Bounds.Top, e.Bounds.Right, e.Bounds.Bottom);

                using (var brush = new SolidBrush(color))
                    e.Graphics.FillRectangle(brush, rectColor);

                e.Graphics.DrawRectangle(Pens.Black, rectColor);
                TextRenderer.DrawText(e.Graphics, text, Font, rectText, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
        }
    }

    private void ColorComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (SelectedIndex >= 0 && ((Color)SelectedItem).Name == "Custom...")
        {
            Color clr;
            ColorDialog colorDialog = new ColorDialog();
            var result = colorDialog.ShowDialog();

            if (result == DialogResult.OK)
                clr = colorDialog.Color;
            else
                clr = Color.Empty;

            if (Items.Contains(clr))
                SelectedItem = clr;
            else
            {
                // Assuming DataSource is a List<object> or similar
                var newData = (List<object>)DataSource;
                newData.Insert(Items.Count - 1, clr);
                DataSource = null; // Clear the DataSource
                DataSource = newData; // Assign the modified data back to DataSource
                SelectedIndex = Items.Count - 2;
            }
        }
    }

    public Color SelectedColor
    {
        get => SelectedIndex >= 0 && SelectedIndex != Items.Count - 1 ?  (Color)SelectedItem : Color.Empty;
        set
        {
            if (Items.Contains(value))
            {
                SelectedItem = value;
            }
            else
            {
                // Assuming DataSource is a List<object> or similar
                var newData = (List<object>)DataSource;
                newData.Insert(Items.Count - 1, value);
                DataSource = null; // Clear the DataSource
                DataSource = newData; // Assign the modified data back to DataSource
                SelectedIndex = Items.Count - 2; // Select "Custom..." item
            }
        }
    }
}

// https://stackoverflow.com/questions/59007745/show-list-of-colors-in-combobox-color-picker
// The 'Custom' item and its supplies have been added by ChatGPT-3.5,
// I added and modified some lines.