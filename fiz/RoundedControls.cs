using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class RoundedTextBox : TextBox
{
    private int borderRadius = 15;
    private string placeholderText = "";
    private bool isPlaceholder = false;

    // ✅ Добавлен атрибут для скрытия от дизайнера
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public int BorderRadius
    {
        get => borderRadius;
        set
        {
            borderRadius = value;
            this.Invalidate(); // Перерисовать контрол
        }
    }

    // ✅ Добавлен атрибут для скрытия от дизайнера
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public new string PlaceholderText
    {
        get => placeholderText;
        set
        {
            placeholderText = value;
            if (string.IsNullOrEmpty(this.Text) && !string.IsNullOrEmpty(value))
            {
                this.Text = value;
                this.ForeColor = Color.Gray;
                isPlaceholder = true;
            }
        }
    }

    public RoundedTextBox()
    {
        this.BorderStyle = BorderStyle.None;
        this.Multiline = true;
        this.TextAlign = HorizontalAlignment.Left;
        this.Padding = new Padding(15, 0, 0, 0);
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        if (!string.IsNullOrEmpty(placeholderText) && string.IsNullOrEmpty(this.Text))
        {
            this.Text = placeholderText;
            this.ForeColor = Color.Gray;
            isPlaceholder = true;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        GraphicsPath path = new GraphicsPath();
        int diameter = borderRadius * 2;

        path.AddArc(0, 0, diameter, diameter, 180, 90);
        path.AddArc(this.Width - diameter, 0, diameter, diameter, 270, 90);
        path.AddArc(this.Width - diameter, this.Height - diameter, diameter, diameter, 0, 90);
        path.AddArc(0, this.Height - diameter, diameter, diameter, 90, 90);
        path.CloseAllFigures();

        this.Region = new Region(path);
    }

    protected override void OnEnter(EventArgs e)
    {
        base.OnEnter(e);
        if (isPlaceholder)
        {
            this.Text = "";
            this.ForeColor = Color.Black;
            isPlaceholder = false;
        }
    }

    protected override void OnLeave(EventArgs e)
    {
        base.OnLeave(e);
        if (string.IsNullOrEmpty(this.Text))
        {
            this.Text = placeholderText;
            this.ForeColor = Color.Gray;
            isPlaceholder = true;
        }
    }
}

public class RoundedButton : Button
{
    private int borderRadius = 15;

    // ✅ Добавлен атрибут для скрытия от дизайнера
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public int BorderRadius
    {
        get => borderRadius;
        set
        {
            borderRadius = value;
            this.Invalidate(); // Перерисовать контрол
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        GraphicsPath path = new GraphicsPath();
        int diameter = borderRadius * 2;

        path.AddArc(0, 0, diameter, diameter, 180, 90);
        path.AddArc(this.Width - diameter, 0, diameter, diameter, 270, 90);
        path.AddArc(this.Width - diameter, this.Height - diameter, diameter, diameter, 0, 90);
        path.AddArc(0, this.Height - diameter, diameter, diameter, 90, 90);
        path.CloseAllFigures();

        this.Region = new Region(path);
        base.OnPaint(e);
    }
}