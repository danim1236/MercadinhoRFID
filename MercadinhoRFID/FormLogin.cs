using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MercadinhoRFID
{
    public partial class FormLogin : Form
    {
        public string User { get; set; }
        public string Senha { get; set; }

        public FormLogin()
        {
            InitializeComponent();
            textBoxUser.Text = string.Empty;
        }

        private void FormLoginLoad(object sender, EventArgs e)
        {
            textBoxSenha.Text = string.Empty;
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            User = textBoxUser.Text;
            Senha = textBoxSenha.Text;
        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            var form = new Form();
            var label = new Label();
            var textBox = new TextBox();
            var buttonOk = new Button();
            var buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x1B)
                DialogResult = DialogResult.Cancel;
        }

        public static bool TryLogin(string loginFileName)
        {
            var logins = 
                (from line in File.ReadAllLines(loginFileName)
                 let parts = line.Split(new[] {' ', '\t', ',', ';'}, StringSplitOptions.RemoveEmptyEntries)
                 where parts.Length == 2
                 select new {Key=parts[0], Value=parts[1]}).ToDictionary(_ => _.Key, _=>_.Value);
            var login = new FormLogin();
            if (login.ShowDialog() == DialogResult.OK)
            {
                if (logins.ContainsKey(login.User) && logins[login.User] == login.Senha)
                    return true;
            }
            return false;
        }
    }
}
