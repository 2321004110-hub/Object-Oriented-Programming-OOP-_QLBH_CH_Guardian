using QLBH_Guardian.DataAccess;

namespace QLBH_Guardian.Forms
{
    public class FrmMain : Form
    {
        private Panel pnlSidebar = null!;
        private Panel pnlContent = null!;
        private Panel pnlTopBar = null!;
        private Label lblPageTitle = null!;
        private Panel pnlActiveNav = null!;
        private UserControl? _currentView = null;

        public FrmMain()
        {
            InitializeComponent();
            ShowView(new FrmBanHang(), "Bán hàng", null);
        }

        private void InitializeComponent()
        {
            this.Text = "QLBH Guardian 2026 - Cửa hàng Guardian khu vực TP.HCM";
            this.Size = new Size(1400, 820);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1100, 650);
            this.BackColor = Color.FromArgb(248, 249, 252);
            this.Font = new Font("Segoe UI", 9);

            BuildTopBar();
            BuildSidebar();
            BuildContent();
        }

        private void BuildTopBar()
        {
            pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.FromArgb(26, 80, 160)
            };

            var lblLogo = new Label
            {
                Text = "⊕  QLBH Guardian 2026",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(14, 10),
                AutoSize = true
            };

            lblPageTitle = new Label
            {
                Text = "Bán hàng",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(200, 220, 255),
                AutoSize = true
            };
            lblPageTitle.Location = new Point(280, 13);

            var lblUser = new Label
            {
                Text = "👤 Admin   |   " + DateTime.Now.ToString("dd/MM/yyyy"),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(200, 220, 255),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            this.Resize += (s, e) => lblUser.Location = new Point(this.Width - lblUser.Width - 20, 13);
            lblUser.Location = new Point(1260, 13);

            pnlTopBar.Controls.AddRange(new Control[] { lblLogo, lblPageTitle, lblUser });
            this.Controls.Add(pnlTopBar);
        }

        private void BuildSidebar()
        {
            pnlSidebar = new Panel
            {
                Location = new Point(0, 44),
                Width = 200,
                BackColor = Color.FromArgb(32, 40, 62),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
            };
            this.Resize += (s, e) => pnlSidebar.Height = this.ClientSize.Height - 44;
            pnlSidebar.Height = 760;

            var menuItems = new[]
            {
                ("🏠", "Tổng quan",    (Action)(() => ShowView(new FrmBanHang(), "Tổng quan", null))),
                ("🛒", "Bán hàng",     () => ShowView(new FrmBanHang(), "Bán hàng", null)),
                ("📋", "Đơn hàng",     () => ShowView(new FrmDonHang(), "Đơn hàng", null)),
                ("📦", "Tồn kho",      () => ShowView(new FrmTonKho(), "Tồn kho", null)),
                ("👥", "Khách hàng",   () => ShowView(new FrmBanHang(), "Khách hàng", null)),
                ("📊", "Báo cáo",      () => ShowView(new FrmTonKho(), "Báo cáo", null)),
                ("⚙️", "Cài đặt",      () => MessageBox.Show("Cài đặt hệ thống", "Thông báo")),
            };

            int y = 20;
            foreach (var (icon, title, action) in menuItems)
            {
                var pnlItem = new Panel
                {
                    Location = new Point(0, y),
                    Size = new Size(200, 44),
                    Cursor = Cursors.Hand,
                    BackColor = Color.Transparent
                };

                var lblIcon = new Label { Text = icon, Location = new Point(14, 12), AutoSize = true, ForeColor = Color.FromArgb(160, 180, 210), Font = new Font("Segoe UI", 11) };
                var lblText = new Label { Text = title, Location = new Point(44, 14), AutoSize = true, ForeColor = Color.FromArgb(190, 205, 230), Font = new Font("Segoe UI", 9) };

                pnlItem.Controls.AddRange(new Control[] { lblIcon, lblText });

                var localAction = action;
                var localTitle = title;
                var localPanel = pnlItem;

                pnlItem.MouseEnter += (s, e) => { if (pnlItem != pnlActiveNav) pnlItem.BackColor = Color.FromArgb(45, 55, 80); };
                pnlItem.MouseLeave += (s, e) => { if (pnlItem != pnlActiveNav) pnlItem.BackColor = Color.Transparent; };
                foreach (Control c in pnlItem.Controls)
                {
                    c.MouseEnter += (s, e) => { if (pnlItem != pnlActiveNav) pnlItem.BackColor = Color.FromArgb(45, 55, 80); };
                    c.MouseLeave += (s, e) => { if (pnlItem != pnlActiveNav) pnlItem.BackColor = Color.Transparent; };
                }
                pnlItem.Click += (s, e) => { SetActiveNav(localPanel, lblText); localAction(); };
                foreach (Control c in pnlItem.Controls) c.Click += (s, e) => { SetActiveNav(localPanel, lblText); localAction(); };

                if (y == 20) { pnlActiveNav = pnlItem; pnlItem.BackColor = Color.FromArgb(26, 115, 232); lblText.ForeColor = Color.White; lblIcon.ForeColor = Color.White; }

                pnlSidebar.Controls.Add(pnlItem);
                y += 44;
            }

            // Version badge
            var lblVer = new Label
            {
                Text = "v2026.1",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(90, 110, 140),
                Location = new Point(10, 680),
                AutoSize = true,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left
            };
            pnlSidebar.Controls.Add(lblVer);

            this.Controls.Add(pnlSidebar);
        }

        private void SetActiveNav(Panel panel, Label textLabel)
        {
            if (pnlActiveNav != null)
            {
                pnlActiveNav.BackColor = Color.Transparent;
                foreach (Control c in pnlActiveNav.Controls)
                {
                    c.ForeColor = c is Label l && l.Font.Size > 10
                        ? Color.FromArgb(160, 180, 210) : Color.FromArgb(190, 205, 230);
                }
            }
            pnlActiveNav = panel;
            panel.BackColor = Color.FromArgb(26, 115, 232);
            foreach (Control c in panel.Controls) c.ForeColor = Color.White;
        }

        private void BuildContent()
        {
            pnlContent = new Panel
            {
                Location = new Point(200, 44),
                BackColor = Color.FromArgb(248, 249, 252),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            this.Resize += (s, e) =>
            {
                pnlContent.Size = new Size(this.ClientSize.Width - 200, this.ClientSize.Height - 44);
            };
            pnlContent.Size = new Size(1200, 760);
            this.Controls.Add(pnlContent);
        }

        private void ShowView(UserControl view, string title, Panel? navPanel)
        {
            lblPageTitle.Text = title;
            _currentView?.Dispose();
            _currentView = view;
            view.Dock = DockStyle.Fill;
            pnlContent.Controls.Clear();
            pnlContent.Controls.Add(view);
        }

        // Static helper to style grids uniformly
        public static void StyleGrid(DataGridView grid, Color headerColor)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = headerColor == Color.White
                ? Color.FromArgb(250, 250, 252) : Color.FromArgb(26, 115, 232);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = headerColor == Color.White
                ? Color.FromArgb(70, 70, 90) : Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 255);
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            grid.DefaultCellStyle.Padding = new Padding(4, 2, 4, 2);
            grid.GridColor = Color.FromArgb(225, 228, 235);
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        }
    }
}
