using QLBH_Guardian.DataAccess;
using System.Data;

namespace QLBH_Guardian.Forms
{
    
    public class FrmTonKho : UserControl
    {
        private Panel pnlHeader = null!;
        private DataGridView gridTonKho = null!;
        private ComboBox cmbKho = null!;
        private DateTimePicker dtpTuNgay = null!, dtpDenNgay = null!;
        private Button btnXemBaoCao = null!, btnXuatExcel = null!, btnLamMoi = null!;
        private Label lblTenBaoCao = null!, lblChiNhanh = null!;

        public FrmTonKho()
        {
            InitializeComponent();
            LoadKho();
            LoadTonKho();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(245, 246, 250);
            BuildHeader();
            BuildGrid();
        }

        // ── Left sidebar (Kho menu) ──────────────────────────
        private void BuildHeader()
        {
            // ── Left nav panel ───────────────────────────────
            var pnlLeft = new Panel
            {
                Location = new Point(0, 0),
                Width = 180,
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom,
                BorderStyle = BorderStyle.None
            };
            this.Resize += (s, e) => pnlLeft.Height = this.Height;
            pnlLeft.Height = this.Height > 0 ? this.Height : 700;
            pnlLeft.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(215, 220, 230)), pnlLeft.Width - 1, 0, pnlLeft.Width - 1, pnlLeft.Height);

            var lblKhoTitle = new Label { Text = "Kho", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.FromArgb(32, 40, 62), Location = new Point(14, 14), AutoSize = true };
            pnlLeft.Controls.Add(lblKhoTitle);

            string[] navItems = { "Nhập kho", "Xuất kho", "Chuyển kho", "Lệnh sản xuất", "Lệnh lắp ráp, tháo dỡ", "Tính giá xuất kho" };
            int ny = 50;
            foreach (var item in navItems)
            {
                var lbl = new Label { Text = item, Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(26, 115, 232), Location = new Point(14, ny), AutoSize = true, Cursor = Cursors.Hand };
                pnlLeft.Controls.Add(lbl);
                ny += 28;
            }

            this.Controls.Add(pnlLeft);

            // ── Top toolbar ──────────────────────────────────
            pnlHeader = new Panel
            {
                Location = new Point(181, 0),
                Height = 100,
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Resize += (s, e) => pnlHeader.Width = this.Width - 181;
            pnlHeader.Width = this.Width > 181 ? this.Width - 181 : 900;
            pnlHeader.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(215, 220, 230)), 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);

            // Sub-nav tabs
            string[] subTabs = { "Nhập, xuất kho", "Chuyển kho", "Lệnh sản xuất", "Lắp ráp, tháo dỡ", "Kiểm kê" };
            int tx = 10;
            foreach (var tab in subTabs)
            {
                var btn = new Button
                {
                    Text = tab,
                    Location = new Point(tx, 8),
                    AutoSize = true,
                    Padding = new Padding(10, 3, 10, 3),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(240, 242, 248),
                    Font = new Font("Segoe UI", 8.5f),
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderColor = Color.FromArgb(205, 210, 220);
                pnlHeader.Controls.Add(btn);
                tx += btn.PreferredSize.Width + 8;
            }

            // Báo cáo link
            var lblBC = new Label { Text = "▶ Hướng dẫn khai thác hiệu quả phần mềm quản lý bán hàng", Font = new Font("Segoe UI", 8.5f), ForeColor = Color.FromArgb(26, 115, 232), Location = new Point(tx + 20, 12), AutoSize = true, Cursor = Cursors.Hand };
            pnlHeader.Controls.Add(lblBC);

            // Filter row
            var btnChonBC = new Button
            {
                Text = "Chọn báo cáo...",
                Location = new Point(10, 48),
                Size = new Size(130, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(237, 239, 244),
                Font = new Font("Segoe UI", 8.5f),
                Cursor = Cursors.Hand
            };
            btnChonBC.FlatAppearance.BorderColor = Color.FromArgb(200, 205, 215);
            pnlHeader.Controls.Add(btnChonBC);

            var lblKho = new Label { Text = "Kho:", Location = new Point(155, 52), AutoSize = true, Font = new Font("Segoe UI", 9) };
            cmbKho = new ComboBox { Location = new Point(183, 48), Size = new Size(165, 26), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9) };
            cmbKho.Items.Add("-- Tất cả kho --"); cmbKho.SelectedIndex = 0;

            var lblTu = new Label { Text = "Từ ngày:", Location = new Point(360, 52), AutoSize = true, Font = new Font("Segoe UI", 9) };
            dtpTuNgay = new DateTimePicker { Location = new Point(415, 48), Size = new Size(110, 26), Font = new Font("Segoe UI", 9), Format = DateTimePickerFormat.Short, Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1) };

            var lblDen = new Label { Text = "Đến ngày:", Location = new Point(535, 52), AutoSize = true, Font = new Font("Segoe UI", 9) };
            dtpDenNgay = new DateTimePicker { Location = new Point(596, 48), Size = new Size(110, 26), Font = new Font("Segoe UI", 9), Format = DateTimePickerFormat.Short, Value = DateTime.Today };

            btnXemBaoCao = new Button { Text = "Xem báo cáo", Location = new Point(716, 47), Size = new Size(115, 28), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(26, 115, 232), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            btnXemBaoCao.FlatAppearance.BorderSize = 0;
            btnXemBaoCao.Click += (s, e) => LoadTonKho();

            btnXuatExcel = new Button { Text = "Xuất khẩu", Location = new Point(840, 47), Size = new Size(95, 28), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand };
            btnXuatExcel.FlatAppearance.BorderSize = 0;

            btnLamMoi = new Button { Text = "🔄", Location = new Point(946, 47), Size = new Size(34, 28), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(237, 239, 244), Font = new Font("Segoe UI", 10), Cursor = Cursors.Hand };
            btnLamMoi.Click += (s, e) => LoadTonKho();

            pnlHeader.Controls.AddRange(new Control[] { lblKho, cmbKho, lblTu, dtpTuNgay, lblDen, dtpDenNgay, btnXemBaoCao, btnXuatExcel, btnLamMoi });
            this.Controls.Add(pnlHeader);

            // ── Report title ─────────────────────────────────
            var pnlTitle = new Panel
            {
                Location = new Point(181, 100),
                Height = 60,
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Resize += (s, e) => pnlTitle.Width = this.Width - 181;
            pnlTitle.Width = this.Width > 181 ? this.Width - 181 : 900;

            lblTenBaoCao = new Label
            {
                Text = "TỔNG HỢP TỒN KHO",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(32, 40, 62),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 34
            };
            lblChiNhanh = new Label
            {
                Text = $"Chi nhánh: Cửa hàng Guardian Quận 9; Tháng {DateTime.Today.Month:00} năm {DateTime.Today.Year}",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(80, 90, 110),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            pnlTitle.Controls.AddRange(new Control[] { lblTenBaoCao, lblChiNhanh });
            this.Controls.Add(pnlTitle);
        }

        private void BuildGrid()
        {
            gridTonKho = new DataGridView
            {
                Location = new Point(181, 160),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 36,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            this.Resize += (s, e) =>
            {
                gridTonKho.Width  = this.Width - 181;
                gridTonKho.Height = this.Height - 160;
            };
            gridTonKho.Width  = this.Width  > 181 ? this.Width  - 181 : 900;
            gridTonKho.Height = this.Height > 160 ? this.Height - 160 : 520;

            FrmMain.StyleGrid(gridTonKho, Color.FromArgb(26, 115, 232));

            // Grouped header rows — simulated with two header lines via ColumnHeader
            var cols = new (string Name, string Header, int Width, bool Right)[]
            {
                ("MaHang",      "Mã hàng",          90,  false),
                ("TenHang",     "Tên hàng",         160,  false),
                ("DVT",         "ĐVT",               50,  false),
                ("DKSoLuong",   "Đầu kỳ\nSố lượng", 90,  true),
                ("DKGiaTri",    "Đầu kỳ\nGiá trị",  110, true),
                ("NKSoLuong",   "Nhập kho\nSố lượng",90, true),
                ("NKGiaTri",    "Nhập kho\nGiá trị",110, true),
                ("XKSoLuong",   "Xuất kho\nSố lượng",90, true),
                ("XKGiaTri",    "Xuất kho\nGiá trị",110, true),
                ("CKSoLuong",   "Cuối kỳ\nSố lượng",90,  true),
                ("CKGiaTri",    "Cuối kỳ\nGiá trị", 110, true),
            };

            foreach (var (name, header, width, right) in cols)
            {
                var col = new DataGridViewTextBoxColumn { Name = name, HeaderText = header, Width = width };
                if (right) col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridTonKho.Columns.Add(col);
            }

            this.Controls.Add(gridTonKho);
        }

        private void LoadKho()
        {
            try
            {
                var dt = DatabaseHelper.ExecuteQuery("SELECT maKho, tenKho FROM Kho ORDER BY tenKho");
                cmbKho.Items.Clear();
                cmbKho.Items.Add("-- Tất cả kho --");
                foreach (DataRow row in dt.Rows) cmbKho.Items.Add(row["tenKho"].ToString() ?? "");
                cmbKho.SelectedIndex = 0;
            }
            catch { /* keep default */ }
        }

        private void LoadTonKho()
        {
            try
            {
                var sql = @"SELECT sp.maSP AS MaHang, sp.tenSP AS TenHang, sp.donViTinh AS DVT,
                                   ISNULL(tk.soLuongTon,0) AS CKSoLuong,
                                   ISNULL(tk.soLuongTon * sp.giaBan,0) AS CKGiaTri,
                                   0 AS DKSoLuong, 0 AS DKGiaTri,
                                   0 AS NKSoLuong, 0 AS NKGiaTri,
                                   0 AS XKSoLuong, 0 AS XKGiaTri
                            FROM SanPham sp
                            LEFT JOIN TonKho tk ON tk.maSP = sp.maSP
                            ORDER BY sp.tenSP";
                var dt = DatabaseHelper.ExecuteQuery(sql);
                PopulateGrid(dt);
            }
            catch { LoadDemoTonKho(); }
        }

        private void PopulateGrid(DataTable dt)
        {
            gridTonKho.Rows.Clear();
            foreach (DataRow row in dt.Rows)
            {
                int idx = gridTonKho.Rows.Add(
                    row["MaHang"], row["TenHang"], row["DVT"],
                    $"{Convert.ToDecimal(row["DKSoLuong"]):N2}", $"{Convert.ToDecimal(row["DKGiaTri"]):N0}",
                    $"{Convert.ToDecimal(row["NKSoLuong"]):N2}", $"{Convert.ToDecimal(row["NKGiaTri"]):N0}",
                    $"{Convert.ToDecimal(row["XKSoLuong"]):N2}", $"{Convert.ToDecimal(row["XKGiaTri"]):N0}",
                    $"{Convert.ToDecimal(row["CKSoLuong"]):N2}", $"{Convert.ToDecimal(row["CKGiaTri"]):N0}");
            }
            lblChiNhanh.Text = $"Chi nhánh: Cửa hàng Guardian Quận 9; Tháng {dtpTuNgay.Value.Month:00} năm {dtpTuNgay.Value.Year}";
            lblTenBaoCao.Text = $"TỔNG HỢP TỒN KHO";
        }

        private void LoadDemoTonKho()
        {
            gridTonKho.Rows.Clear();

            // Grouped header row for "Kho nguyên vật liệu"
            AddGroupRow("Tên kho : Kho nguyên vật liệu (10)", "114.300,00", "373.400.000", "120,00", "0", "7.029,60", "24.170.000", "107.390,40", "349.230.000");

            var items = new[]
            {
                ("NEU1", "Kem dưỡng ẩm Neutrogena Hydro Boost",            "Hũ", "300,00",    "350.000",     "0,00",   "0",          "0,00",    "0",          "300,00",    "350.000"),
                ("KL1",    "Serum Vitamin C Klairs 30ml",                 "Chai",   "1.000,00", "490.000", "0,00",   "0",          "0,00",    "0",          "1.000,00", "490.000"),
                ("LIP2", "Son kem lì Romand Blur Fudge 5g",             "Cây",  "2.000,00",  "280.000", "0,00",   "0",          "190,00",  "950.000",    "1.810,00",  "280.000"),
                ("LINER2","Kẻ mắt nước đen The Face Shop 7g",            "Cây",  "0,00",      "0",          "100,00", "0",          "145.000","0",          "(145.000)","0"),
                ("ST3",   "Sữa tắm dưỡng ẩm Dove 530ml",                "Chai",   "0,00",      "0",          "0,00",   "0",          "130.000",  "0",          "(130.000)",  "0"),
                ("DT3",   "Kem dưỡng thể Vaseline Cocoa Butter",                "Chai", "100,00","99.000", "0,00",   "0",          "0,00",    "0",          "100,00","99.000"),
                ("KM3",     "Lăn khử mùi Nivea Pearl & Beauty",                  "Chai",   "0,00",      "0",          "0,00",   "0",          "79.000",   "0",          "(79.000)",   "0"),
                ("DG4","Dầu gội Pantene dưỡng chất 650ml",            "Chai",    "0,00",      "0",          "0,00",   "0",          "155.000",  "0",          "(155.000)",  "0"),
                ("DX4","Dầu xả TRESemmé Keratin Smooth",    "Chai",    "1.000,00",  "145.000", "20,00",  "0",          "516,00",  "23.220.000", "504,00",    "21.780.000"),
                ("VCLS5", "Nước hoa Victoria Secret Love Spell",             "Chai",  "1.000,00",  "850.000","0,00",   "0",          "0,00",    "0",          "1.000,00",  "850.000.000"),
            };

            foreach (var (ma, ten, dvt, dksl, dkgt, nksl, nkgt, xksl, xkgt, cksl, ckgt) in items)
            {
                int idx = gridTonKho.Rows.Add(ma, ten, dvt, dksl, dkgt, nksl, nkgt, xksl, xkgt, cksl, ckgt);
                ColorNegativeRow(idx, cksl);
            }

            AddGroupRow("Tên kho : Kho thành phẩm (2)", "0,00", "0", "367,00", "0", "0,00", "0", "367,00", "0");
            gridTonKho.Rows.Add("VCLS5", "Nước hoa Victoria Secret Love Spell", "Chai", "0,00", "0", "192,00", "0", "0,00", "0", "192,00", "0");
            gridTonKho.Rows.Add("KL1", "Serum Vitamin C Klairs 30ml", "Chai", "0,00", "0", "175,00", "0", "0,00", "0", "175,00", "0");

            // Total row
            int totIdx = gridTonKho.Rows.Add("", "Số dòng = 12", "", "114.300,00", "373.400.000", "487,00", "0", "7.029,60", "24.170.000", "107.757,40", "349.230.000");
            gridTonKho.Rows[totIdx].DefaultCellStyle.BackColor = Color.FromArgb(240, 245, 255);
            gridTonKho.Rows[totIdx].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void AddGroupRow(string groupName, string dksl, string dkgt, string nksl, string nkgt, string xksl, string xkgt, string cksl, string ckgt)
        {
            int idx = gridTonKho.Rows.Add("", groupName, "", dksl, dkgt, nksl, nkgt, xksl, xkgt, cksl, ckgt);
            gridTonKho.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(225, 235, 255);
            gridTonKho.Rows[idx].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            gridTonKho.Rows[idx].DefaultCellStyle.ForeColor = Color.FromArgb(26, 60, 140);
        }

        private void ColorNegativeRow(int rowIdx, string cksl)
        {
            if (cksl.StartsWith("(") || cksl.StartsWith("-"))
                gridTonKho.Rows[rowIdx].DefaultCellStyle.ForeColor = Color.FromArgb(200, 40, 40);
        }
    }
}
