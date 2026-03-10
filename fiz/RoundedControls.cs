using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class RoundedTextBox : TextBox
{
    public int BorderRadius { get; set; } = 15;
    public new string PlaceholderText { get; set; } = "";
    private bool isPlaceholder = false;

    public RoundedTextBox()
    {
        this.BorderStyle = BorderStyle.None;
        this.Multiline = true;
        this.TextAlign = HorizontalAlignment.Left; // Слева
        this.Padding = new Padding(15, 0, 0, 0); // Отступ слева
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        // Показываем placeholder сразу при создании
        if (!string.IsNullOrEmpty(PlaceholderText) && string.IsNullOrEmpty(this.Text))
        {
            this.Text = PlaceholderText;
            this.ForeColor = Color.Gray;
            isPlaceholder = true;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        GraphicsPath path = new GraphicsPath();
        int diameter = BorderRadius * 2;

        path.AddArc(0, 0, diameter, diameter, 180, 90);
        path.AddArc(this.Width - diameter, 0, diameter, diameter, 270, 90);
        path.AddArc(this.Width - diameter, this.Height - diameter, diameter, diameter, 0, 90);
        path.AddArc(0, this.Height - diameter, diameter, diameter, 90, 90);
        path.CloseAllFigures();

        this.Region = new Region(path);
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
    }

    protected override void OnEnter(EventArgs e)
    {
        base.OnEnter(e);
        // При входе убираем placeholder
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
        // При выходе показываем placeholder если пусто
        if (string.IsNullOrEmpty(this.Text))
        {
            this.Text = PlaceholderText;
            this.ForeColor = Color.Gray;
            isPlaceholder = true;
        }
    }
}

public class RoundedButton : Button
{
    public int BorderRadius { get; set; } = 15;

    protected override void OnPaint(PaintEventArgs e)
    {
        GraphicsPath path = new GraphicsPath();
        int diameter = BorderRadius * 2;

        path.AddArc(0, 0, diameter, diameter, 180, 90);
        path.AddArc(this.Width - diameter, 0, diameter, diameter, 270, 90);
        path.AddArc(this.Width - diameter, this.Height - diameter, diameter, diameter, 0, 90);
        path.AddArc(0, this.Height - diameter, diameter, diameter, 90, 90);
        path.CloseAllFigures();

        this.Region = new Region(path);
        base.OnPaint(e);
    }
}