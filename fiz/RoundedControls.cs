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
    private Button toggleButton;
    private bool showPasswordToggle = false;
    private char passwordChar = '\0';

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public int BorderRadius
    {
        get => borderRadius;
        set
        {
            borderRadius = value;
            this.Invalidate();
        }
    }

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
                ShowPlaceholder();
            }
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public new char PasswordChar
    {
        get => passwordChar;
        set
        {
            passwordChar = value;
            if (!isPlaceholder)
            {
                base.PasswordChar = value;
            }
        }
    }

    [DefaultValue(false)]
    public bool ShowPasswordToggle
    {
        get => showPasswordToggle;
        set
        {
            showPasswordToggle = value;
            if (value && toggleButton == null)
            {
                CreateToggleButton();
            }
            else if (!value && toggleButton != null)
            {
                toggleButton.Dispose();
                toggleButton = null;
            }
        }
    }

    public RoundedTextBox()
    {
        this.BorderStyle = BorderStyle.None;
        this.Multiline = true;
        this.TextAlign = HorizontalAlignment.Left;
        this.Padding = new Padding(15, 0, 40, 0);
    }

    private void ShowPlaceholder()
    {
        base.PasswordChar = '\0';
        this.Text = placeholderText;
        this.ForeColor = Color.Gray;
        isPlaceholder = true;
    }

    private void HidePlaceholder()
    {
        this.Text = "";
        this.ForeColor = Color.Black;
        base.PasswordChar = passwordChar;
        isPlaceholder = false;
    }

    private void CreateToggleButton()
    {
        toggleButton = new Button();
        toggleButton.Text = "👁️";
        toggleButton.Size = new Size(30, 28);
        toggleButton.FlatStyle = FlatStyle.Flat;
        toggleButton.FlatAppearance.BorderSize = 0;
        toggleButton.BackColor = Color.Transparent;
        toggleButton.ForeColor = Color.Gray;
        toggleButton.Cursor = Cursors.Hand;
        toggleButton.TabIndex = this.TabIndex + 1;
        toggleButton.Click += ToggleButton_Click;
        toggleButton.MouseEnter += (s, e) => toggleButton.ForeColor = Color.DarkGray;
        toggleButton.MouseLeave += (s, e) => toggleButton.ForeColor = Color.Gray;

        UpdateToggleButtonPosition();

        if (this.Parent != null)
        {
            this.Parent.Controls.Add(toggleButton);
            toggleButton.BringToFront();
        }
    }

    private void UpdateToggleButtonPosition()
    {
        if (toggleButton != null)
        {
            toggleButton.Location = new Point(this.Right - toggleButton.Width - 5, this.Top + 3);
        }
    }

    private void ToggleButton_Click(object sender, EventArgs e)
    {
        if (isPlaceholder) return;

        if (base.PasswordChar == '\0')
        {
            base.PasswordChar = passwordChar;
            toggleButton.Text = "👁️";
        }
        else
        {
            base.PasswordChar = '\0';
            toggleButton.Text = "🙈";
        }
        this.Focus();
    }

    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        if (toggleButton != null && this.Parent != null)
        {
            if (!this.Parent.Controls.Contains(toggleButton))
            {
                this.Parent.Controls.Add(toggleButton);
                toggleButton.BringToFront();
            }
            UpdateToggleButtonPosition();
        }
    }

    protected override void OnLocationChanged(EventArgs e)
    {
        base.OnLocationChanged(e);
        UpdateToggleButtonPosition();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        UpdateToggleButtonPosition();
    }

    protected override void OnCreateControl()
    {
        base.OnCreateControl();
        if (!string.IsNullOrEmpty(placeholderText) && string.IsNullOrEmpty(this.Text))
        {
            ShowPlaceholder();
        }
    }

    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);

        if (!isPlaceholder && !string.IsNullOrEmpty(this.Text) && this.ForeColor == Color.Gray)
        {
            base.PasswordChar = '\0';
        }
    }

    protected override void OnEnter(EventArgs e)
    {
        base.OnEnter(e);
        if (isPlaceholder)
        {
            HidePlaceholder();
        }
    }

    protected override void OnLeave(EventArgs e)
    {
        base.OnLeave(e);
        if (string.IsNullOrEmpty(this.Text))
        {
            ShowPlaceholder();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && toggleButton != null)
        {
            toggleButton.Dispose();
            toggleButton = null;
        }
        base.Dispose(disposing);
    }
}

public class RoundedButton : Button
{
    private int borderRadius = 15;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public int BorderRadius
    {
        get => borderRadius;
        set
        {
            borderRadius = value;
            this.Invalidate();
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