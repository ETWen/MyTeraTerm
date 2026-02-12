using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace MyTeraTerm
{
    public class PasswordDialog : Form
    {
        private Label lblPrompt;
        private TextBox txtPassword;
        private Button btnOK;
        private Button btnCancel;

        // 預設密碼的 SHA256 Hash (密碼: 4411)
        private const string CorrectPasswordHash = "8ed5f6c97c680e72cf3da76cc9ccbfb92c01ff74185d2883072fcc7be0d3fdb6";

        public bool IsAuthenticated { get; private set; }

        public PasswordDialog()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Form 設定
            Text = "Debug Console Authentication";
            Size = new Size(400, 180);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // 提示標籤
            lblPrompt = new Label
            {
                Text = "Enter debug password:",
                Location = new Point(20, 20),
                Size = new Size(350, 20),
                Font = new Font("Segoe UI", 9F)
            };

            // 密碼輸入框
            txtPassword = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(340, 25),
                PasswordChar = '●',
                Font = new Font("Segoe UI", 10F),
                TabIndex = 0
            };

            // 處理 Enter 鍵
            txtPassword.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    btnOK.PerformClick();
                }
            };

            // OK 按鈕
            btnOK = new Button
            {
                Text = "OK",
                Location = new Point(200, 95),
                Size = new Size(75, 30),
                TabIndex = 1,
                Font = new Font("Segoe UI", 9F)
            };
            btnOK.Click += BtnOK_Click;

            // Cancel 按鈕
            btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(285, 95),
                Size = new Size(75, 30),
                TabIndex = 2,
                Font = new Font("Segoe UI", 9F)
            };

            // 加入控制項
            Controls.Add(lblPrompt);
            Controls.Add(txtPassword);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);

            // 設定預設按鈕
            AcceptButton = btnOK;
            CancelButton = btnCancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            string inputHash = ComputeSha256Hash(txtPassword.Text);

            if (inputHash == CorrectPasswordHash)
            {
                IsAuthenticated = true;
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                IsAuthenticated = false;
                MessageBox.Show("Incorrect password!\n\nDebug console access denied.",
                              "Authentication Failed",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);

                // 清空密碼框並重新聚焦
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            txtPassword.Focus();
        }

        /// <summary>
        /// 計算字串的 SHA256 Hash
        /// </summary>
        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        /// 用於生成新密碼的 Hash（開發時使用）
        /// </summary>
        public static string GeneratePasswordHash(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}