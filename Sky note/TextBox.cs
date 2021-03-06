/*--------------------------------------------------------------------------------------------------------------------
 Copyright (C) 2021 Himber Sacha

 This program is free software: you can redistribute it and/or modify
 it under the +terms of the GNU General Public License as published by
 the Free Software Foundation, either version 2 of the License, or
 any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program.  If not, see https://www.gnu.org/licenses/gpl-2.0.html. 

--------------------------------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using Sky_UI;

namespace Sky_note
{
    [ObsoleteAttribute("This class is obsolete. Project abandoned.", false)]
    internal sealed class TextBox : Sky_UI.Rectangle
    {
        private System.Windows.Forms.TextBox TextBoxWriter = new System.Windows.Forms.TextBox();
        private Label nbLine = new Label();

        internal TextBox()
        {
            nbLine.BackColor = Color.FromArgb(64, 64, 64);
            nbLine.AutoSize = true;
            nbLine.ForeColor = Color.White;
            nbLine.Location = new Point(0, 0);
            nbLine.Font = new Font("Consolas", 11.25F, FontStyle.Regular, GraphicsUnit.Point);

            this.TextBoxWriter.BackColor = Color.FromArgb(64, 64, 64);
            this.TextBoxWriter.BorderStyle = BorderStyle.None;
            this.TextBoxWriter.Font = new Font("Consolas", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            this.TextBoxWriter.ForeColor = Color.White;
            this.TextBoxWriter.Location = new Point(0, 0);
            this.TextBoxWriter.Multiline = true;
            this.TextBoxWriter.Size = new Size(this.Width, this.Height);
            this.TextBoxWriter.ScrollBars = ScrollBars.None;
            this.TextBoxWriter.WordWrap = false;
            this.TextBoxWriter.MaxLength = int.MaxValue;
            this.TextBoxWriter.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; 
            this.TextBoxWriter.TextChanged += new EventHandler(TextBoxWriter_TextChanged);

            this.VScroll = true;
            this.HScroll = true;
            this.AutoScroll = true;
            this.BackColor = Color.FromArgb(64, 64, 64);
            this.Controls.Add(nbLine);
            this.Controls.Add(TextBoxWriter);
            this.Controls.SetChildIndex(nbLine, 0);
            this.Controls.SetChildIndex(TextBoxWriter, 0);
        }

        private void TextBoxWriter_TextChanged(object sender, EventArgs e)
        {
            nbLine.Text = string.Empty;

            for (int index = 0; index < TextBoxWriter.Lines.Length; index++)
            {
                nbLine.Text += index + "\n";
            }

            this.TextBoxWriter.Location = new Point(nbLine.Width + 3, 0);

            Size size = TextRenderer.MeasureText(TextBoxWriter.Text, TextBoxWriter.Font);
            TextBoxWriter.ClientSize = new Size(size.Width, size.Height);

            this.VerticalScroll.Value = TextBoxWriter.SelectionStart;
        }

        new public string Text
        {
            get
            {
                return TextBoxWriter.Text;
            }
            set
            {
                TextBoxWriter.Text = value;
            }
        }

        new public Color ForeColor
        {
            get
            {
                return TextBoxWriter.ForeColor;
            }
            set
            {
                TextBoxWriter.ForeColor = value;
            }
        }

        new public Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                TextBoxWriter.BackColor = value;
                nbLine.BackColor = value;
            }
        }
    }
}
