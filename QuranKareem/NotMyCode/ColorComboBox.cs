using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class ColorComboBox : ComboBox
{

    public ColorComboBox() => SetupComboBox();

    private void SetupComboBox()
    {
        List<object> data = typeof(Color).GetProperties()
            .Where(x => x.PropertyType == typeof(Color))
            .Select(x => x.GetValue(null)).ToList();
        data.RemoveAt(0);

        DataSource = data;

        MaxDropDownItems = 10;
        IntegralHeight = false;
        DrawMode = DrawMode.OwnerDrawFixed;
        DropDownStyle = ComboBoxStyle.DropDownList;
        DrawItem += ColorComboBox_DrawItem;
        FormattingEnabled = true;
    }

    private void ColorComboBox_DrawItem(object sender, DrawItemEventArgs e)
    {
        e.DrawBackground();
        if (e.Index >= 0)
        {
            var text = GetItemText(Items[e.Index]);
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

    public Color SelectedColor => SelectedIndex >= 0 ? (Color)SelectedItem : Color.Empty;
}

// https://stackoverflow.com/questions/59007745/show-list-of-colors-in-combobox-color-picker
// This class was created by ChatGPT 3.5