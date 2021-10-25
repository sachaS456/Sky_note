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
using Sky_framework;
using System.IO;
using Microsoft.Win32;
using Sky_Updater;

namespace Sky_note
{
    internal sealed class MainForm : SkyForms
    {
        private Sky_framework.Button ButtonSelectAll;
        private Sky_framework.Button ButtonEncode;
        private System.Windows.Forms.TextBox DefineLine;
        private Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private Sky_framework.Button ButtonFile;
        private MenuDeroulant MenuDeroulantFile = new MenuDeroulant();
        private MenuDeroulant MenuDeroulantEncoding = new MenuDeroulant();
        private Encoding encodingUsed = Encoding.UTF8;
        private Sky_framework.Button ButtonAbout;
        private ButtonCircular buttonCircular1;
        private Label label2;
        private ButtonCircular buttonCircular2;
        private string FileLoaded = string.Empty;
        private short Zoom = 100;
        private Language language;
        private Control c = new Control();
        private bool TextSaved = true;

        internal MainForm() : base()
        {
            if (System.Globalization.CultureInfo.CurrentCulture.Name == "fr-FR")
            {
                language = Language.French;
            }
            else
            {
                language = Language.English;
            }

            InitializeComponent();

            MenuDeroulantFile.Location = new Point(6, 50);
            MenuDeroulantFile.BorderRadius = 15;
            MenuDeroulantFile.Border = 0;
            MenuDeroulantFile.ShowSide = Side.Top;
            if (language == Language.French)
            {
                MenuDeroulantFile.SetButton(new string[6] { "Ouvrir", "Enregistrer sous", "Enregistrer", "Nouveau", "Nouvelle fenêtre", "Quitter" });
            }
            else
            {
                MenuDeroulantFile.SetButton(new string[6] { "Open", "Save as", "Save", "New", "New window", "Leave" });
            }
            MenuDeroulantFile.SetButtonClique(0, new MouseEventHandler(OpenFile_Click));
            MenuDeroulantFile.SetButtonClique(1, new MouseEventHandler(SaveAs_Click));
            MenuDeroulantFile.SetButtonClique(2, new MouseEventHandler(Save_Click));
            MenuDeroulantFile.SetButtonClique(3, new MouseEventHandler(New_Click));
            MenuDeroulantFile.SetButtonClique(4, new MouseEventHandler(NewWindow_Click));
            MenuDeroulantFile.SetButtonClique(5, new MouseEventHandler(Leave_Click));
            this.Controls.Add(MenuDeroulantFile);
            MenuDeroulantFile.BringToFront();

            MenuDeroulantEncoding.Location = new Point(770, 510);
            MenuDeroulantEncoding.BorderRadius = 15;
            MenuDeroulantEncoding.Border = 0;
            MenuDeroulantEncoding.ShowSide = Side.Bottom;
            MenuDeroulantEncoding.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            MenuDeroulantEncoding.SetButton(new string[6] { "UTF-8", "UTF-16 BE", "UTF-16 LE", "UTF-32", "ASCII", "ISO-8859-1" });
            MenuDeroulantEncoding.SetButtonClique(0, new MouseEventHandler(ButtonUTF8_Click));
            MenuDeroulantEncoding.SetButtonClique(1, new MouseEventHandler(ButtonUTF16BE_Click));
            MenuDeroulantEncoding.SetButtonClique(2, new MouseEventHandler(ButtonUTF16LE_Click));
            MenuDeroulantEncoding.SetButtonClique(3, new MouseEventHandler(ButtonUTF32_Click));
            MenuDeroulantEncoding.SetButtonClique(4, new MouseEventHandler(ButtonASCII_Click));
            MenuDeroulantEncoding.SetButtonClique(5, new MouseEventHandler(ButtonISO88591_Click));
            this.Controls.Add(MenuDeroulantEncoding);
            MenuDeroulantEncoding.BringToFront();

            HideMenuDeroulant();
            CheckUpdate();
            SelectText();
            this.Show();

            OpenFileOnStarting();
        }

        private async void OpenFileOnStarting()
        {
            if (Environment.GetCommandLineArgs().Last() != Application.ExecutablePath && Environment.GetCommandLineArgs().Last() != Application.StartupPath + @"Sky note.dll")
            {
                using (StreamReader streamReader = new StreamReader(new FileStream(Environment.GetCommandLineArgs().Last(), FileMode.Open, FileAccess.Read, 
                    FileShare.ReadWrite), EncodeAdapt(Environment.GetCommandLineArgs().Last())))
                {
                    textBox1.Text = String.Join(Environment.NewLine, (await streamReader.ReadToEndAsync()).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None));
                    encodingUsed = streamReader.CurrentEncoding;
                    ButtonEncode.Text = streamReader.CurrentEncoding.BodyName;
                    FileLoaded = Environment.GetCommandLineArgs().Last();
                    this.Text = "Sky note - " + FileLoaded;
                    streamReader.Close();
                }
            }
        }

        private async void CheckUpdate()
        {
            string CurrentVersion;

            if (Environment.Is64BitProcess)
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Sky note\Sky note setup x64.exe"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Sky note\Sky note setup x64.exe");
                }

                CurrentVersion = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Sky note", false).GetValue("DisplayVersion").ToString();
                if (await Sky_Updater.Update.CheckUpdateAsync("Sky note", CurrentVersion))
                {
                    this.KeyPreview = false;

                    do
                    {
                        await Task.Delay(10);
                    }
                    while (this.Opacity < 1);

                    BlurMainForm();
                    UpdateDetectDialogControl update = new UpdateDetectDialogControl(CurrentVersion, await Sky_Updater.Update.DownloadStringAsync("https://serie-sky.netlify.app/Download/Sky note/Version.txt"));
                    update.BringToFront();
                    update.Location = new Point(this.Width / 2 - update.Width / 2, this.Height / 2 - update.Height / 2);
                    update.Anchor = AnchorStyles.None;
                    update.ButtonUpdate += new EventBoolHandler(ActionButtonUpdate);
                    this.Controls.Add(update);
                    update.BringToFront();
                }
            }
            else
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Sky note\Sky note setup x86.exe"))
                {
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Sky note\Sky note setup x86.exe");
                }

                CurrentVersion = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Sky note", false).GetValue("DisplayVersion").ToString();
                if (await Sky_Updater.Update.CheckUpdateAsync("Sky note", CurrentVersion))
                {
                    this.KeyPreview = false;

                    do
                    {
                        await Task.Delay(10);
                    }
                    while (this.Opacity < 1);

                    BlurMainForm();
                    UpdateDetectDialogControl update = new UpdateDetectDialogControl(CurrentVersion, await Sky_Updater.Update.DownloadStringAsync("https://serie-sky.netlify.app/Download/Sky note/Version.txt"));
                    update.BringToFront();
                    update.Location = new Point(this.Width / 2 - update.Width / 2, this.Height / 2 - update.Height / 2);
                    update.Anchor = AnchorStyles.None;
                    update.ButtonUpdate += new EventBoolHandler(ActionButtonUpdate);
                    this.Controls.Add(update);
                    update.BringToFront();
                }
            }
        }

        private void ActionButtonUpdate(bool Download)
        {
            foreach (UpdateDetectDialogControl i in this.Controls.OfType<UpdateDetectDialogControl>())
            {
                i.Dispose();
                this.Controls.Remove(i);
            }

            if (Download == true)
            {
                DownloadUpdaterDialog update = new DownloadUpdaterDialog("Sky note");
                update.BringToFront();
                update.Location = new Point(this.Width / 2 - update.Width / 2, this.Height / 2 - update.Height / 2);
                update.Anchor = AnchorStyles.None;
                this.Controls.Add(update);
                update.BringToFront();
            }
            else
            {
                this.KeyPreview = true;
                UnBlurMainForm();
            }
        }

        private void BlurMainForm()
        {
            Bitmap b = new Bitmap(this.ClientRectangle.Width, this.ClientRectangle.Height);
            Graphics.FromImage(b).CopyFromScreen(this.Location.X + Border, this.Location.Y + 20, 0, 0, new Size(this.ClientRectangle.Size.Width - Border, this.ClientRectangle.Size.Height - Border),
                CopyPixelOperation.SourceCopy);
            c.BackgroundImage = b;
            c.Size = new Size(this.Width - Border * 2, this.Height - Border - 20);
            c.Location = new Point(Border, 20);
            c.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            c.BringToFront();
            c.Visible = true;
            this.Controls.Add(c);

            for (int index = 0; index < 20; index++)
            {
                c.BringToFront();
                Effect.GaussianBlur(ref b);
                c.BackgroundImage = b;
            }
        }

        private void UnBlurMainForm()
        {
            /*Bitmap b = (Bitmap)c.BackgroundImage;

            for (int index = 0; index < 20; index++)
            {
                c.BringToFront();
                Effect.GaussianBlurRemove(ref b);
                c.BackgroundImage = b;
            }*/

            this.Controls.Remove(c);
            c.BackgroundImage = null;
            this.Update();
        }


        private void ButtonUTF8_Click(object sender, MouseEventArgs e)
        {
            encodingUsed = Encoding.UTF8;
            ButtonEncode.Text = Encoding.UTF8.BodyName;
        }

        private void ButtonUTF16BE_Click(object sender, MouseEventArgs e)
        {
            encodingUsed = Encoding.BigEndianUnicode;
            ButtonEncode.Text = Encoding.BigEndianUnicode.BodyName;
        }

        private void ButtonUTF16LE_Click(object sender, MouseEventArgs e)
        {
            encodingUsed = Encoding.Unicode;
            ButtonEncode.Text = Encoding.Unicode.BodyName;
        }

        private void ButtonUTF32_Click(object sender, MouseEventArgs e)
        {
            encodingUsed = Encoding.UTF32;
            ButtonEncode.Text = Encoding.UTF32.BodyName;
        }

        private void ButtonASCII_Click(object sender, MouseEventArgs e)
        {
            encodingUsed = Encoding.ASCII;
            ButtonEncode.Text = Encoding.ASCII.BodyName;
        }

        private void ButtonISO88591_Click(object sender, MouseEventArgs e)
        {
            encodingUsed = Encoding.Latin1;
            ButtonEncode.Text = Encoding.Latin1.BodyName;
        }

        private async void HideMenuDeroulant()
        {
            await Task.Delay(0);

            for (int index = 0; index < this.Controls.Count; index++)
            {
                if (this.Controls[index] != MenuDeroulantFile && this.Controls[index] != MenuDeroulantEncoding)
                {
                    this.Controls[index].MouseClick += new MouseEventHandler(HideMenuDeroulantFile);
                }
            }

            this.MouseClick += new MouseEventHandler(HideMenuDeroulantFile);
        }

        private void HideMenuDeroulantFile(object sender, MouseEventArgs e)
        {
            MenuDeroulantFile.Hide();
            MenuDeroulantEncoding.Hide();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ButtonFile = new Sky_framework.Button();
            this.ButtonSelectAll = new Sky_framework.Button();
            this.ButtonEncode = new Sky_framework.Button();
            this.DefineLine = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.ButtonAbout = new Sky_framework.Button();
            this.buttonCircular1 = new Sky_framework.ButtonCircular();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonCircular2 = new Sky_framework.ButtonCircular();
            this.SuspendLayout();
            // 
            // ButtonFile
            // 
            this.ButtonFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ButtonFile.Border = false;
            this.ButtonFile.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ButtonFile.borderRadius = 0;
            this.ButtonFile.ID = 0;
            this.ButtonFile.Image = ((System.Drawing.Image)(resources.GetObject("ButtonFile.Image")));
            this.ButtonFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButtonFile.Location = new System.Drawing.Point(4, 22);
            this.ButtonFile.Name = "ButtonFile";
            this.ButtonFile.Size = new System.Drawing.Size(71, 24);
            this.ButtonFile.TabIndex = 3;
            if (language == Language.French)
            {
                this.ButtonFile.Text = "Fichier";
            }
            else
            {
                this.ButtonFile.Text = "File";
            }
            this.ButtonFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ButtonFile.Click += new System.EventHandler(this.ButtonFile_Click);
            // 
            // ButtonSelectAll
            // 
            this.ButtonSelectAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ButtonSelectAll.Border = false;
            this.ButtonSelectAll.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ButtonSelectAll.borderRadius = 0;
            this.ButtonSelectAll.ID = 0;
            this.ButtonSelectAll.Image = ((System.Drawing.Image)(resources.GetObject("ButtonSelectAll.Image")));
            this.ButtonSelectAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.ButtonSelectAll.Location = new System.Drawing.Point(81, 22);
            this.ButtonSelectAll.Name = "ButtonSelectAll";
            this.ButtonSelectAll.Size = new System.Drawing.Size(151, 24);
            this.ButtonSelectAll.TabIndex = 5;
            if (language == Language.French)
            {
                this.ButtonSelectAll.Text = "Tout sélectionner";
            }
            else
            {
                this.ButtonSelectAll.Text = "Select all";
            }
            this.ButtonSelectAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ButtonSelectAll.Click += new System.EventHandler(this.ButtonSelectAll_Click);
            // 
            // ButtonEncode
            // 
            this.ButtonEncode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonEncode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ButtonEncode.Border = false;
            this.ButtonEncode.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ButtonEncode.borderRadius = 0;
            this.ButtonEncode.ID = 0;
            this.ButtonEncode.Image = null;
            this.ButtonEncode.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ButtonEncode.Location = new System.Drawing.Point(833, 525);
            this.ButtonEncode.Name = "ButtonEncode";
            this.ButtonEncode.Size = new System.Drawing.Size(85, 22);
            this.ButtonEncode.TabIndex = 6;
            this.ButtonEncode.Text = "UTF-8";
            this.ButtonEncode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ButtonEncode.Click += new System.EventHandler(this.ButtonEncode_Click);
            // 
            // DefineLine
            // 
            this.DefineLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.DefineLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DefineLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DefineLine.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.DefineLine.ForeColor = System.Drawing.Color.White;
            this.DefineLine.Location = new System.Drawing.Point(773, 527);
            this.DefineLine.Name = "DefineLine";
            this.DefineLine.Size = new System.Drawing.Size(54, 22);
            this.DefineLine.TabIndex = 7;
            this.DefineLine.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DefineLine_KeyPress);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(724, 529);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 8;
            if (language == Language.French)
            {
                this.label1.Text = "Ligne :";
            }
            else
            {
                this.label1.Text = "Line :";
            }
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(2, 48);
            this.textBox1.MaxLength = 2147483647;
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(917, 477);
            this.textBox1.TabIndex = 9;
            this.textBox1.WordWrap = false;
            this.textBox1.TextChanged += new EventHandler(textBox1_TextChanged);
            // 
            // ButtonAbout
            // 
            this.ButtonAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonAbout.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ButtonAbout.BackgroundImage = ((System.Drawing.Icon)(resources.GetObject("Sky note"))).ToBitmap();
            this.ButtonAbout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ButtonAbout.Border = false;
            this.ButtonAbout.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ButtonAbout.borderRadius = 0;
            this.ButtonAbout.ID = 0;
            this.ButtonAbout.Image = null;
            this.ButtonAbout.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ButtonAbout.Location = new System.Drawing.Point(885, 22);
            this.ButtonAbout.Name = "ButtonAbout";
            this.ButtonAbout.Size = new System.Drawing.Size(32, 24);
            this.ButtonAbout.TabIndex = 10;
            this.ButtonAbout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ButtonAbout.Click += new System.EventHandler(this.ButtonAbout_Click);
            // 
            // buttonCircular1
            // 
            this.buttonCircular1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(40)))), ((int)(((byte)(170)))));
            this.buttonCircular1.Border = false;
            this.buttonCircular1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.buttonCircular1.borderRadius = 0;
            this.buttonCircular1.ID = 0;
            this.buttonCircular1.Image = null;
            this.buttonCircular1.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonCircular1.Location = new System.Drawing.Point(680, 527);
            this.buttonCircular1.Name = "buttonCircular1";
            this.buttonCircular1.Size = 20;
            this.buttonCircular1.TabIndex = 11;
            this.buttonCircular1.Text = "+";
            this.buttonCircular1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonCircular1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.buttonCircular1.Click += new System.EventHandler(this.buttonCircular1_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(595, 529);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 15);
            this.label2.TabIndex = 12;
            this.label2.Text = "Zoom : 100%";
            // 
            // buttonCircular2
            // 
            this.buttonCircular2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(40)))), ((int)(((byte)(170)))));
            this.buttonCircular2.Border = false;
            this.buttonCircular2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.buttonCircular2.borderRadius = 0;
            this.buttonCircular2.ID = 0;
            this.buttonCircular2.Image = null;
            this.buttonCircular2.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonCircular2.Location = new System.Drawing.Point(569, 527);
            this.buttonCircular2.Name = "buttonCircular2";
            this.buttonCircular2.Size = 20;
            this.buttonCircular2.TabIndex = 13;
            this.buttonCircular2.Text = "-";
            this.buttonCircular2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.buttonCircular2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.buttonCircular2.Click += new System.EventHandler(this.buttonCircular2_Click);
            // 
            // MainForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.BorderColor = System.Drawing.Color.Indigo;
            this.ButtonMaximizedVisible = true;
            this.ClientSize = new System.Drawing.Size(922, 552);
            this.MinimumSize = new Size(400, 250);
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(This_KeyDown);
            this.FormClosing += new FormClosingEventHandler(This_FormClosing);
            this.Controls.Add(this.buttonCircular2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCircular1);
            this.Controls.Add(this.ButtonAbout);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DefineLine);
            this.Controls.Add(this.ButtonEncode);
            this.Controls.Add(this.ButtonSelectAll);
            this.Controls.Add(this.ButtonFile);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "MainForm";
            this.Text = "Sky note";
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("Sky note")));
            this.Controls.SetChildIndex(this.ButtonFile, 0);
            this.Controls.SetChildIndex(this.ButtonSelectAll, 0);
            this.Controls.SetChildIndex(this.ButtonEncode, 0);
            this.Controls.SetChildIndex(this.DefineLine, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.textBox1, 0);
            this.Controls.SetChildIndex(this.ButtonAbout, 0);
            this.Controls.SetChildIndex(this.buttonCircular1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.buttonCircular2, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void This_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox1.Focused == false)
            {
                e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.O)
            {
                OpenFile();
                return;
            }

            if (e.Control && e.KeyCode == Keys.Q)
            {
                this.Close();
                return;
            }

            if (e.Control && e.KeyCode == Keys.N)
            {
                NewFile();
                return;
            }

            if (e.Control && e.KeyCode == Keys.S)
            {
                SaveFile();
                return;
            }

            if (e.Control && e.Shift && e.KeyCode == Keys.S)
            {
                SaveAsFile();
                return;
            }

            if (e.Control && e.Shift && e.KeyCode == Keys.N)
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = Application.ExecutablePath;
                process.Start();
                process.Close();
                process = null;
                return;
            }
        }

        private void ButtonSelectAll_Click(object sender, EventArgs e)
        {
            textBox1.Select();
            if (textBox1.SelectionLength > 0)
            {
                if (language == Language.French)
                {
                    ButtonSelectAll.Text = "Tout désélectionner";
                }
                else
                {
                    ButtonSelectAll.Text = "Unselect all";
                }

                textBox1.DeselectAll();
            }
            else
            {
                if (language == Language.French)
                {
                    ButtonSelectAll.Text = "Tout sélectionner";
                }
                else
                {
                    ButtonSelectAll.Text = "Select all";
                }

                textBox1.SelectAll();
            }
        }

        private async void SelectText()
        {
            while (this.Disposing == false && this.IsDisposed == false && textBox1.Disposing == false && textBox1.IsDisposed == false && textBox1 != null && this != null && ButtonSelectAll != null)
            {
                if (textBox1.SelectionLength > 0)
                {
                    if (language == Language.French)
                    {
                        if (ButtonSelectAll.Text != "Tout désélectionner")
                        {
                            ButtonSelectAll.Text = "Tout désélectionner";
                        }
                    }
                    else
                    {
                        if (ButtonSelectAll.Text != "Unselect all")
                        {
                            ButtonSelectAll.Text = "Unselect all";
                        }
                    }
                }
                else
                {
                    if (language == Language.French)
                    {
                        if (ButtonSelectAll.Text != "Tout sélectionner")
                        {
                            ButtonSelectAll.Text = "Tout sélectionner";
                        }
                    }
                    else
                    {
                        if (ButtonSelectAll.Text != "Select all")
                        {
                            ButtonSelectAll.Text = "Select all";
                        }
                    }
                }

                await Task.Delay(10);
            }
        }

        private void DefineLine_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '0':

                    return;

                case '1':

                    return;

                case '2':

                    return;

                case '3':

                    return;

                case '4':

                    return;

                case '5':

                    return;

                case '6':

                    return;

                case '7':

                    return;

                case '8':

                    return;

                case '9':

                    return;

                case (char)Keys.Back:

                    return;

                case (char)Keys.Return:
                    if (long.TryParse(DefineLine.Text, out long result) == true)
                    {
                        if (GoLine(result) == false)
                        {
                            DefineLine.Text = "1";
                        }
                    }
                    return;

                default:
                    e.Handled = true;
                    break;
            }
        }

        private bool GoLine(long line)
        {
            textBox1.Select();
            if (textBox1.Text != string.Empty && line <= textBox1.Lines.Length)
            {
                int seed = 0, pos = -1;

                line -= 1;

                if (line == 0)
                {
                    pos = 0;
                }
                else
                {
                    for (long i = 0; i < line; i++)
                    {
                        pos = textBox1.Text.IndexOf(Environment.NewLine, seed) + 2;
                        seed = pos;
                    }
                }

                if (pos != -1)
                {
                    textBox1.Select(pos, 0);
                }
                textBox1.ScrollToCaret();
                textBox1.Refresh();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ButtonEncode_Click(object sender, EventArgs e)
        {
            if (MenuDeroulantEncoding.View == false)
            {
                MenuDeroulantEncoding.Show();
            }
        }

        private void ButtonFile_Click(object sender, EventArgs e)
        {
            if (MenuDeroulantFile.View == false)
            {
                MenuDeroulantFile.Show();
            }
        }

        private void OpenFile()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                if (language == Language.French)
                {
                    dialog.Filter = "Tous les fichiers | *.*";
                }
                else
                {
                    dialog.Filter = "All the files | *.*";
                }
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamReader streamReader = new StreamReader(new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), EncodeAdapt(dialog.FileName)))
                    {
                        textBox1.Text = String.Join(Environment.NewLine, streamReader.ReadToEnd().Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None));
                        encodingUsed = streamReader.CurrentEncoding;
                        ButtonEncode.Text = streamReader.CurrentEncoding.BodyName;
                        FileLoaded = dialog.FileName;
                        this.Text = "Sky note - " + FileLoaded;
                        streamReader.Close();
                        TextSaved = true;
                    }
                }

                dialog.Dispose();
            }
        }

        private void OpenFile_Click(object sender, MouseEventArgs e)
        {
            OpenFile();
        }

        private Encoding EncodeAdapt(string FileName)
        {
            byte[] bytes = File.ReadAllBytes(FileName);

            string text = null;

            if (Encoding.UTF8.TryGetString(bytes, out text))
            {
                return Encoding.UTF8;
            }
            else if (Encoding.ASCII.TryGetString(bytes, out text))
            {
                return Encoding.ASCII;
            }
            else if (Encoding.Latin1.TryGetString(bytes, out text))
            {
                return Encoding.Latin1;
            }
            else if (Encoding.UTF32.TryGetString(bytes, out text))
            {
                return Encoding.UTF32;
            }
            else if (Encoding.BigEndianUnicode.TryGetString(bytes, out text))
            {
                return Encoding.BigEndianUnicode;
            }
            else if (Encoding.Unicode.TryGetString(bytes, out text))
            {
                return Encoding.Unicode;
            }
#pragma warning disable
            else if (Encoding.UTF7.TryGetString(bytes, out text))
            {
                if (language == Language.French)
                {
                    if (MessageBox.Show("Attention cet encodage n'est plus pris en charge. Pour votre sécurité il n'est pas recommandé de lire ce fichier! Voulez vous quand même le lire?", "Sky note",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        return Encoding.UTF7;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    if (MessageBox.Show("Please note that this encoding is no longer supported. For your safety it is not recommended to read this file! Do you want to read it anyway?", "Sky note",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        return Encoding.UTF7;
#warning
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                if (language == Language.French)
                {
                    MessageBox.Show("Ce fichier n'est pas pris en charge!", "Sky note", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("This file is not supported!", "Sky note", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return null;
            }
        }

        private void SaveAsFile()
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                if (language == Language.French)
                {
                    dialog.Filter = "Fichier texte | *.txt | Fichier log | *.log | Fichier config | *.ini | Tous les fichiers | *.*";
                }
                else
                {
                    dialog.Filter = "Text file | *.txt | Log file | *.log | Config file | *.ini | All the files | *.*";
                }

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter streamWriter = new StreamWriter(new FileStream(dialog.FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), encodingUsed))
                    {
                        streamWriter.Write(textBox1.Text);
                        FileLoaded = dialog.FileName;
                        streamWriter.Close();
                        TextSaved = true;
                    }
                }

                dialog.Dispose();
            }
        }

        private void SaveAs_Click(object sender, MouseEventArgs e)
        {
            SaveAsFile();
        }

        private void SaveFile()
        {
            if (FileLoaded == string.Empty)
            {
                SaveAsFile();
            }
            else
            {
                using (StreamWriter streamWriter = new StreamWriter(new FileStream(FileLoaded, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), encodingUsed))
                {
                    streamWriter.Write(textBox1.Text);
                    streamWriter.Close();
                    TextSaved = true;
                }
            }
        }

        private void Save_Click(object sender, MouseEventArgs e)
        {
            SaveFile();
        }

        private void NewFile()
        {
            if (language == Language.French)
            {
                if (MessageBox.Show("Attention, cela effacera les modifications non sauvegardées. Voulez vous créer un nouveau fichier texte?", "Sky note",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    FileLoaded = string.Empty;
                    textBox1.Text = string.Empty;
                }
            }
            else
            {
                if (MessageBox.Show("Warning that this will erase unsaved changes. Do you want to create a new text file?", "Sky note",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    FileLoaded = string.Empty;
                    textBox1.Text = string.Empty;
                }
            }
        }

        private void New_Click(object sender, MouseEventArgs e)
        {
            NewFile();
        }

        private void NewWindow_Click(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = Application.ExecutablePath;
            process.Start();
            process.Close();
            process = null;
        }

        private void Leave_Click(object sender, MouseEventArgs e)
        {
            this.Close();
        }

        private void ButtonAbout_Click(object sender, EventArgs e)
        {
            new AboutForm(language).Show();
        }

        private void buttonCircular2_Click(object sender, EventArgs e)
        {
            if (Zoom > 25)
            {
                Zoom -= 25;
            }

            label2.Text = "Zoom : " + Zoom;

            UpdateZoom();
        }

        private void UpdateZoom()
        {
            switch (Zoom)
            {
                case 25:
                    textBox1.Font = new Font("Consolas", 5.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                    break;

                case 50:
                    textBox1.Font = new Font("Consolas", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                    break;

                case 75:
                    textBox1.Font = new Font("Consolas", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                    break;

                case 100:
                    textBox1.Font = new Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                    break;

                case 125:
                    textBox1.Font = new Font("Consolas", 13.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                    break;

                case 150:
                    textBox1.Font = new Font("Consolas", 15.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                    break;

                case 175:
                    textBox1.Font = new Font("Consolas", 17.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                    break;

                case 200:
                    textBox1.Font = new Font("Consolas", 19.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                    break;

                case 225:
                    textBox1.Font = new Font("Consolas", 21.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                    break;

                case 250:
                    textBox1.Font = new Font("Consolas", 23.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                    break;
            }
        }

        private void buttonCircular1_Click(object sender, EventArgs e)
        {
            if (Zoom < 250)
            {
                Zoom += 25;
            }

            label2.Text = "Zoom : " + Zoom;

            UpdateZoom();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (TextSaved == true)
            {
                TextSaved = false;
            }
        }

        private void This_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (TextSaved == false)
            {
                if (language == Language.French)
                {
                    if (MessageBox.Show("Êtes vous sûr de vouloir fermer Sky note? Vos modifications vont être perdu.", "Sky note", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        e.Cancel = false;
                    }
                }
                else
                {
                    if (MessageBox.Show("Are you sure you want to close Sky note? Your changes will be lost.", "Sky note", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                        e.Cancel = false;
                    }
                }
            }
        }
    }

    internal enum Language
    {
        French,
        English
    }
}
