using QLBH_Guardian.DataAccess;

namespace QLBH_Guardian.Forms
{
    public class FrmLogin : Form
    {
        private TextBox txtUsername = null!, txtPassword = null!;
        private Button btnLogin = null!;
        private Label lblStatus = null!;

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Đăng nhập - QLBH Guardian 2026";
            this.Size = new Size(440, 540);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(248, 249, 252);
            this.Font = new Font("Segoe UI", 9);

            // Header banner
            var pnlBanner = new Panel
            {
                Dock = DockStyle.Top,
                Height = 160,
                BackColor = Color.FromArgb(26, 80, 160)
            };

            var lblLogo = new Label
            {
                Text = "⊕",
                Font = new Font("Segoe UI", 40),
                ForeColor = Color.FromArgb(100, 180, 255),
                Location = new Point(170, 20),
                AutoSize = true
            };

            var lblTitle = new Label
            {
                Text = "QLBH Guardian 2026",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(95, 98),
                AutoSize = true
            };

            var lblSub = new Label
            {
                Text = "Phần mềm Quản lý Bán hàng",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(190, 210, 240),
                Location = new Point(128, 124),
                AutoSize = true
            };

            pnlBanner.Controls.AddRange(new Control[] { lblLogo, lblTitle, lblSub });
            this.Controls.Add(pnlBanner);

            // Login card
            var pnlCard = new Panel
            {
                Location = new Point(30, 185),
                Size = new Size(378, 290),
                BackColor = Color.White
            };
            pnlCard.Paint += (s, e) =>
            {
                var r = new Rectangle(0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
                using var pen = new Pen(Color.FromArgb(220, 225, 235));
                e.Graphics.DrawRectangle(pen, r);
            };

            var lblLogin = new Label
            {
                Text = "Đăng nhập hệ thống",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(32, 40, 62),
                Location = new Point(20, 18),
                AutoSize = true
            };

            var lblUser = new Label { Text = "Tên đăng nhập:", Location = new Point(20, 60), AutoSize = true, Font = new Font("Segoe UI", 9) };
            txtUsername = new TextBox
            {
                Location = new Point(20, 80),
                Size = new Size(334, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "admin"
            };

            var lblPass = new Label { Text = "Mật khẩu:", Location = new Point(20, 122), AutoSize = true, Font = new Font("Segoe UI", 9) };
            txtPassword = new TextBox
            {
                Location = new Point(20, 142),
                Size = new Size(334, 30),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '●',
                Text = "123456"
            };
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) DoLogin(); };

            lblStatus = new Label
            {
                Location = new Point(20, 182),
                Size = new Size(334, 20),
                ForeColor = Color.FromArgb(220, 53, 69),
                Font = new Font("Segoe UI", 8.5f),
                Text = ""
            };

            btnLogin = new Button
            {
                Text = "ĐĂNG NHẬP",
                Location = new Point(20, 210),
                Size = new Size(334, 42),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(26, 115, 232),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += (s, e) => DoLogin();

            pnlCard.Controls.AddRange(new Control[] { lblLogin, lblUser, txtUsername, lblPass, txtPassword, lblStatus, btnLogin });
            this.Controls.Add(pnlCard);

            var lblCopy = new Label
            {
                Text = "© 2026 Guardian Software. All rights reserved.",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                Location = new Point(95, 490),
                AutoSize = true
            };
            this.Controls.Add(lblCopy);
        }

        private void DoLogin()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                lblStatus.Text = "⚠ Vui lòng nhập tên đăng nhập!";
                return;
            }

            btnLogin.Text = "Đang đăng nhập...";
            btnLogin.Enabled = false;

            try
            {
                bool ok = DatabaseHelper.CheckLogin(txtUsername.Text, txtPassword.Text);
                if (ok)
                {
                    var main = new FrmMain();
                    main.Show();
                    this.Hide();
                    main.FormClosed += (s, e) => this.Close();
                }
                else
                {
                    lblStatus.Text = "⚠ Tên đăng nhập hoặc mật khẩu không đúng!";
                    btnLogin.Text = "ĐĂNG NHẬP";
                    btnLogin.Enabled = true;
                }
            }
            catch
            {
                // DB not available — open main directly in demo mode
                var main = new FrmMain();
                main.Show();
                this.Hide();
                main.FormClosed += (s, e) => this.Close();
            }
        }
    }
}
