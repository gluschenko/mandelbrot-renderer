//Fork: 21.03.2015
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Anvil.Net
{
    public struct UIStyle 
    {
        public UIElement Button;
        public UIElement Label;
        public UIElement Panel;
    }

    public struct UIElement
    {
        public string Font;
        public float FontSize;
        public FlatStyle FlatStyle;
        public bool UseVisualStyleBackColor;
        public Color ForeColor;
        public Color BackColor;
        public Color MouseOverBackColor;
        public Color MouseDownBackColor;
        public int BorderSize;
        public Color BorderColor;
    }

    public class UI
    {
        public static Dictionary<string, Control> Controls = new Dictionary<string, Control>();

        public static UIStyle DefaultStyle = new UIStyle {
            Button = new UIElement 
            {
                Font = "SegoeUI",
                FontSize = 12f,
                FlatStyle = FlatStyle.Standard,
                UseVisualStyleBackColor = true,
                ForeColor = SystemColors.ControlText,
                BackColor = SystemColors.Control,
                MouseOverBackColor = SystemColors.Control,
                MouseDownBackColor = SystemColors.Control,
                BorderSize = 0,
                BorderColor = SystemColors.Control,
            },
            Label = new UIElement
            {
                Font = "SegoeUI",
                FontSize = 12f,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = true,
                ForeColor = Color.FromArgb(255, 255, 255),
                BackColor = Color.FromArgb(222, 227, 233),
                MouseOverBackColor = Color.FromArgb(245, 181, 23),
                MouseDownBackColor = Color.FromArgb(245, 181, 23),
                BorderSize = 1,
                BorderColor = Color.FromArgb(243, 156, 17),
            },
            Panel = new UIElement
            {
                Font = "SegoeUI",
                FontSize = 12f,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = true,
                ForeColor = Color.FromArgb(255, 255, 255),
                BackColor = Color.FromArgb(222, 227, 233),
                MouseOverBackColor = Color.FromArgb(245, 181, 23),
                MouseDownBackColor = Color.FromArgb(245, 181, 23),
                BorderSize = 1,
                BorderColor = Color.FromArgb(243, 156, 17),
            },
        };

        public static UIStyle Style = DefaultStyle;

        ///

        public static AnchorStyles Anchor(bool top, bool left, bool bottom, bool right)
        {
            AnchorStyles a = new AnchorStyles();

            if (top) a = a | AnchorStyles.Top;
            if (left) a = a | AnchorStyles.Left;
            if (bottom) a = a | AnchorStyles.Bottom;
            if (right) a = a | AnchorStyles.Right;

            return a;
        }

        public static AnchorStyles DefaultAnchor = Anchor(true, true, false, false);
        public static AnchorStyles FullAnchor = Anchor(true, true, true, true);
        public static AnchorStyles NoAnchor = Anchor(false, false, false, false);

        ///

        public static void Start() 
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }

        public static void Append(Control UIObj, Control Target) 
        {
            if (!Controls.Keys.Contains<string>(UIObj.Name))
            {
                Controls.Add(UIObj.Name, UIObj);
                Target.Controls.Add(Controls[UIObj.Name]);
            }
            else 
            {
                Controls[UIObj.Name].Dispose();
                Controls[UIObj.Name] = UIObj;
                Target.Controls.Add(Controls[UIObj.Name]);
            }
        }

        public static Control.ControlCollection ControlsOf(Control UIObj) 
        {
            Panel CPanel = (Panel)UIObj;
            return CPanel.Controls;
        }

        /*public static void DestroyControls(Control Target)  //Бред
        {
            int Count = 0;

            foreach(var Pair in Controls)
            {
                string Key = Pair.Key;
                ///
                if (Pair.Value.Parent.Name == Target.Name) Controls.Remove(Key);
                ///
                Count++;
            }
        }*/


        ///

        public static Button CreateButton(Rect rect, AnchorStyles anchor, string name, string text, Action ClickEvent)
        {
            Button b = new Button();
            b.Location = new Point(rect.X, rect.Y);
            b.Size = new Size(rect.Width, rect.Height);
            b.Anchor = anchor;
            b.Name = name;
            b.Text = text;
            b.UseVisualStyleBackColor = Style.Button.UseVisualStyleBackColor;
            b.Font = new Font(Style.Button.Font, Style.Button.FontSize);
            b.TextAlign = ContentAlignment.MiddleCenter;
            b.FlatStyle = Style.Button.FlatStyle;
            b.BackColor = Style.Button.BackColor;
            b.ForeColor = Style.Button.ForeColor;
            b.FlatAppearance.BorderColor = Style.Button.BorderColor;
            b.FlatAppearance.BorderSize = Style.Button.BorderSize;
            b.FlatAppearance.MouseOverBackColor = Style.Button.MouseOverBackColor;
            b.FlatAppearance.MouseDownBackColor = Style.Button.MouseDownBackColor;

            ///
            b.Click += delegate //From India, with love ;-)
            {
                ClickEvent();
            };
            ///

            return b;
        }

        public static Panel CreatePanel(Rect rect, AnchorStyles anchor, string name)
        {
            Panel p = new Panel();
            p.Location = new Point(rect.X, rect.Y);
            p.Size = new Size(rect.Width, rect.Height);
            p.Name = name;
            p.AutoScroll = true;
            p.BackColor = Style.Panel.BackColor;
            //p.BorderStyle = BorderStyle.FixedSingle;
            p.Anchor = anchor;

            return p;
        }

        public static Label CreateLabel(Rect rect, AnchorStyles anchor, string name, string text, string font = "SegoeUI", int fontsize = 15) 
        {
            Label l = new Label();
            l.Location = new Point(rect.X, rect.Y);
            l.ForeColor = Color.FromArgb(73, 74, 69);
            l.Size = (rect.Width == 0 && rect.Height == 0)? new Size(fontsize * text.Length, fontsize * 2) : new Size(rect.Width, rect.Height);
            l.Name = name;
            l.Text = text;
            l.Font = new Font(font, fontsize);
            l.BackColor = Color.Transparent;
            l.Anchor = anchor;

            return l;
        }

        public static TextBox CreateTextBox(Rect rect, AnchorStyles anchor, string name, string text, int max = 1000000, string font = "SegoeUI", int fontsize = 12) 
        {
            TextBox t = new TextBox();
            t.Location = new Point(rect.X, rect.Y);
            t.Name = name;
            t.Text = text;
            t.MaxLength = max;
            t.Width = rect.Width;
            t.Height = rect.Height;
            t.Font = new Font(font, fontsize);
            t.ForeColor = Color.FromArgb(73, 74, 69);
            t.BackColor = Color.FromArgb(255, 255, 255);
            t.BorderStyle = BorderStyle.FixedSingle;
            t.Multiline = true;
            t.MaxLength = max;

            return t;
        }

        public static TextBox CreateTextField(Rect rect, AnchorStyles anchor, string name, string text, int max = 200, string font = "SegoeUI", int fontsize = 12)
        {
            TextBox t = new TextBox();
            t.Location = new Point(rect.X, rect.Y);
            t.Name = name;
            t.Text = text;
            t.MaxLength = max;
            t.Width = rect.Width;
            t.Height = rect.Height;
            t.Font = new Font(font, fontsize);
            t.ForeColor = Color.FromArgb(73, 74, 69);
            t.BackColor = Color.FromArgb(255, 255, 255);
            t.BorderStyle = BorderStyle.FixedSingle;
            //t.Multiline = true;
            t.MaxLength = max;

            return t;
        }

        public static CheckBox CreateCheckBox(Rect rect, AnchorStyles anchor, string name, string text, bool state, string font = "SegoeUI", int fontsize = 12) 
        {
            CheckBox c = new CheckBox();
            c.Location = new Point(rect.X, rect.Y);
            c.Name = name;
            c.Text = text;
            c.Width = rect.Width;
            c.Height = rect.Height;
            c.Font = new Font(font, fontsize);
            c.ForeColor = Color.FromArgb(73, 74, 69);
            c.Checked = state;

            return c;
        }

        public static PictureBox CreatePictureBox(Rect rect, AnchorStyles anchor, string name)
        {
            PictureBox p = new PictureBox();
            p.Location = new Point(rect.X, rect.Y);
            p.Size = new Size(rect.Width, rect.Height);
            p.Name = name;
            p.BackColor = Color.Black;
            p.Anchor = anchor;

            return p;
        }

        public static WebBrowser CreateWebBrowser(Rect rect, AnchorStyles anchor, string name) 
        {
            WebBrowser w = new WebBrowser();
            w.Name = name;
            w.Anchor = anchor;
            w.Location = new Point(rect.X, rect.Y);
            w.Size = new Size(rect.Width, rect.Height);

            return w;
        }

        public static Form CreateForm(Rect rect, string text) 
        {
            Form f = new Form();
            f.Size = new Size(rect.Width, rect.Height);
            f.MinimumSize = new Size(rect.Width, rect.Height);
            f.Text = text;
            f.StartPosition = FormStartPosition.WindowsDefaultLocation;
            f.AutoScaleMode = AutoScaleMode.Font;
            f.AutoScaleDimensions = new SizeF(6f, 13f);
            if (rect.X != 0 && rect.Y != 0) f.Location = new Point(rect.X, rect.Y);

            return f;
        }
    }

    public struct Rect
    {
        public int X, Y, Width, Height;

        public Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}