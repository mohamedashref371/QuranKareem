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
        Items.AddRange(data.ToArray());
        
        MaxDropDownItems = 10;
        IntegralHeight = false;
        DrawMode = DrawMode.OwnerDrawFixed;
        DropDownStyle = ComboBoxStyle.DropDownList;
        DrawItem += ColorComboBox_DrawItem;
        FormattingEnabled = true;

        // Handle custom color selection
        SelectedIndexChanged += ColorComboBox_SelectedIndexChanged;

        SelectedIndex = 0;
    }

    private void ColorComboBox_DrawItem(object sender, DrawItemEventArgs e)
    {
        e.DrawBackground();
        if (e.Index >= 0)
        {
            //string text = GetItemText(Items[e.Index]).Replace(";", ",");
            string text = ((Color)Items[e.Index]).Name;
            if (text == "0") text = "EmptyColor";
            if (text == "Custom..." || text == "EmptyColor") // Handle custom item differently
            {
                e.Graphics.FillRectangle(Brushes.Transparent, e.Bounds);
                TextRenderer.DrawText(e.Graphics, text, Font, e.Bounds, ForeColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
            else
            {
                Color color = (Color)Items[e.Index];
                int rectSize = (int)(e.Bounds.Height / 1.3);
                var rectColor = new Rectangle(e.Bounds.Left + 1, e.Bounds.Top + 1, rectSize, rectSize);
                Rectangle rectText = Rectangle.FromLTRB(rectColor.Right + 2, e.Bounds.Top, e.Bounds.Right, e.Bounds.Bottom);

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
            using (ColorDialog colorDialog = new ColorDialog())
            {
                DialogResult result = colorDialog.ShowDialog();

                if (result == DialogResult.OK)
                    clr = colorDialog.Color;
                else
                    clr = Color.Empty;
            }
            
            if (Items.Contains(clr))
                SelectedItem = clr;
            else
            {
                Items.Insert(Items.Count - 1, clr);
                SelectedIndex = Items.Count - 2;
            }
        }
    }

    public Color SelectedColor
    {
        get => SelectedIndex >= 0 && SelectedIndex != Items.Count - 1 ?  (Color)SelectedItem : Color.Empty;
        set
        {
            int index = Items.IndexOf(value);
            if (index >= 0 && index != Items.Count - 1)
            {
                SelectedItem = value;
            }
            else if (Items.Count != 0)
            {
                Items.Insert(Items.Count - 1, value);
                SelectedIndex = Items.Count - 2;
            }
        }
    }
}

// Thanks https://stackoverflow.com/questions/59007745/show-list-of-colors-in-combobox-color-picker
// and ChatGPT-3.5 for helping me set up this control.