using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Anvil.Net
{
    public class CommonUI
    {
        public static UIStyle CommonStyle = new UIStyle
        {
            Button = new UIElement
            {
                Font = "SegoeUI",
                FontSize = 12f,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                ForeColor = Color.FromArgb(255, 255, 255),
                BackColor = Color.FromArgb(245, 181, 23),
                MouseOverBackColor = Color.FromArgb(245, 181, 23),
                MouseDownBackColor = Color.FromArgb(245, 181, 23),
                BorderSize = 1,
                BorderColor = Color.FromArgb(243, 156, 17),
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
    }
}
