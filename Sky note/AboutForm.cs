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
    internal sealed class AboutForm : SkyForms
    {
        private Sky_UI.Rectangle rectangle1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Sky_UI.Button button1;
        private Label label1;

        internal AboutForm(Language language)
        {
            InitializeComponent(ref language);
        }

        private void InitializeComponent(ref Language language)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.rectangle1 = new Sky_UI.Rectangle();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new Sky_UI.Button();
            this.SuspendLayout();
            // 
            // rectangle1
            // 
            this.rectangle1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("256x256")));
            this.rectangle1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.rectangle1.Border = false;
            this.rectangle1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.rectangle1.BorderRadius = 0;
            this.rectangle1.BorderWidth = 3;
            this.rectangle1.Location = new System.Drawing.Point(3, 26);
            this.rectangle1.Name = "rectangle1";
            this.rectangle1.Size = new System.Drawing.Size(157, 134);
            this.rectangle1.TabIndex = 3;
            this.rectangle1.Text = "rectangle1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(187, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 31);
            this.label1.TabIndex = 4;
            this.label1.Text = "Sky note";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(157, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 51);
            this.label2.TabIndex = 5;
            if (language == Language.French)
            {
                this.label2.Text = "Version : 3.0.1\r\n\r\nDéveloppée par Sacha Himber";
            }
            else
            {
                this.label2.Text = "Version : 3.0.1\r\n\r\nDevelopped by Sacha Himber";
            }
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(6, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(336, 68);
            this.label3.TabIndex = 6;
            if (language == Language.French)
            {
                this.label3.Text = resources.GetString("label3.TextFR");
            }
            else
            {
                this.label3.Text = resources.GetString("label3.TextEN");
            }
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(6, 256);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(181, 34);
            this.label4.TabIndex = 7;
            if (language == Language.French)
            {
                this.label4.Text = "Librairies utilisées : Net 5.0.13, \r\nSky UI 3.0.1.";
            }
            else
            {
                this.label4.Text = "Libraries used : Net 5.0.13, \r\nSky UI 3.0.1.";
            }
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button1.Border = false;
            this.button1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button1.borderRadius = 5;
            this.button1.ID = 0;
            this.button1.Image = null;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.button1.Location = new System.Drawing.Point(273, 267);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 26);
            this.button1.TabIndex = 8;
            this.button1.Text = "OK";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // AboutForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.BorderColor = System.Drawing.Color.Indigo;
            this.ClientSize = new System.Drawing.Size(356, 269);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.rectangle1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Location = new Point(Screen.FromControl(this).WorkingArea.Width / 2 - this.Width / 2, Screen.FromControl(this).WorkingArea.Height / 2 - this.Height / 2);
            this.Name = "AboutForm";
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("rectangle1.BackgroundImage")));
            this.Redimensionnable = false;
            this.ButtonMaximizedVisible = false;
            if (language == Language.French)
            {
                this.Text = "Sky note - À propos";
            }
            else
            {
                this.Text = "Sky note - About";
            }
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.rectangle1, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
