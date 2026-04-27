using QLBH_Guardian.DataAccess;
using System.Data;

namespace QLBH_Guardian.Forms
{
    public class FrmBanHang : UserControl
    {
        // ── toolbar ──────────────────────────────────────────
        private Panel pnlToolbar = null!;
        private ComboBox cmbLoaiBH = null!;
        private CheckBox chkKiemPhieu = null!;
        private RadioButton rdoChuaThu = null!, rdoThuNgay = null!;
        private ComboBox cmbTienMat = null!;
        private CheckBox chkLapKemHD = null!;

        // ── tabs ─────────────────────────────────────────────
        private TabControl tabMain = null!;

        // ── form info ────────────────────────────────────────
        private TextBox txtMaKH = null!, txtTenKH = null!;
        private TextBox txtMaSoThue = null!, txtNguoiLienHe = null!;
        private TextBox txtDiaChi = null!, txtDienGiai = null!;
        private TextBox txtNVBanHang = null!;
        private TextBox txtThamChieu = null!;
        private DateTimePicker dtpNgayHachToan = null!, dtpNgayChungTu = null!;
        private TextBox txtSoChungTu = null!;

        // ── grid ─────────────────────────────────────────────
        private DataGridView gridHangHoa = null!;
        private DataTable _dtChiTiet = null!;

        // ── footer ───────────────────────────────────────────
        private Label lblSoDong = null!;
        private Label lblTongTienHang = null!, lblTienChietKhau = null!;
        private Label lblTienThue = null!, lblTongThanhToan = null!;

        // ── state ────────────────────────────────────────────
        private int _currentDonHangId = 0;
        private List<(int Id, string SoDH, string TenKH, decimal TongTien, string TrangThai)> _dsDonHang = new();
        private int _currentIndex = 0;

        public FrmBanHang()
        {
            InitializeComponent();
            LoadDonHang();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.Padding = new Padding(0);

            BuildToolbar();
            BuildTabContent();
            BuildFooter();
        }

        
        private void BuildToolbar()
        {
            pnlToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 112,
                BackColor = Color.White,
                Padding = new Padding(8, 6, 8, 6)
            };
            pnlToolbar.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(215, 220, 230)), 0, pnlToolbar.Height - 1, pnlToolbar.Width, pnlToolbar.Height - 1);

            // ── Row 1: title + type selector ─────────────────
            var lblTitle = new Label
            {
                Text = "Chứng từ bán hàng",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 80, 160),
                Location = new Point(10, 8),
                AutoSize = true
            };

            cmbLoaiBH = new ComboBox
            {
                Location = new Point(240, 9),
                Size = new Size(260, 26),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            cmbLoaiBH.Items.AddRange(new object[] {
                "1. Bán hàng hóa, dịch vụ trong nước",
                "2. Bán hàng xuất khẩu",
                "3. Bán hàng nội bộ"
            });
            cmbLoaiBH.SelectedIndex = 0;

            chkKiemPhieu = new CheckBox
            {
                Text = "Kiểm phiếu xuất kho",
                Location = new Point(510, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                Checked = true
            };

            // ── Row 2: action buttons ─────────────────────────
            var btnDefs = new (string Text, Color Back, Action Click)[]
            {
                ("◄ Trước",   Color.FromArgb(108,117,125), () => NavRecord(-1)),
                ("► Sau",     Color.FromArgb(108,117,125), () => NavRecord(1)),
                ("＋ Thêm",   Color.FromArgb(40,167,69),   () => NewRecord()),
                ("✎ Sửa",    Color.FromArgb(23,162,184),  () => EditRecord()),
                ("💾 Lưu",   Color.FromArgb(26,115,232),   () => SaveRecord()),
                ("✖ Xóa",    Color.FromArgb(220,53,69),   () => DeleteRecord()),
                ("↩ Hoàn",   Color.FromArgb(255,193,7),   () => LoadCurrentRecord()),
                ("📋 Cấp HĐ",Color.FromArgb(111,66,193),  () => LapHoaDon()),
                ("📤 Lập PX", Color.FromArgb(32,120,100),  () => MessageBox.Show("Lập phiếu xuất","Thông báo")),
                ("🖨 In",    Color.FromArgb(80,80,90),     () => InChungTu()),
            };

            int bx = 10;
            foreach (var (text, back, click) in btnDefs)
            {
                var btn = new Button
                {
                    Text = text,
                    Location = new Point(bx, 44),
                    Size = new Size(82, 30),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = back,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 8, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btn.FlatAppearance.BorderSize = 0;
                var localClick = click;
                btn.Click += (s, e) => localClick();
                pnlToolbar.Controls.Add(btn);
                bx += 84;
            }

            // ── Row 3: payment options ────────────────────────
            rdoChuaThu = new RadioButton { Text = "Chưa thu tiền", Location = new Point(10, 82), AutoSize = true, Font = new Font("Segoe UI", 9), Checked = true };
            rdoThuNgay  = new RadioButton { Text = "Thu tiền ngay", Location = new Point(120, 82), AutoSize = true, Font = new Font("Segoe UI", 9) };

            cmbTienMat = new ComboBox
            {
                Location = new Point(230, 80),
                Size = new Size(100, 24),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8.5f)
            };
            cmbTienMat.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản", "Thẻ" });
            cmbTienMat.SelectedIndex = 0;

            chkLapKemHD = new CheckBox { Text = "Lập kèm hóa đơn", Location = new Point(340, 82), AutoSize = true, Font = new Font("Segoe UI", 9) };

            pnlToolbar.Controls.AddRange(new Control[] { lblTitle, cmbLoaiBH, chkKiemPhieu, rdoChuaThu, rdoThuNgay, cmbTienMat, chkLapKemHD });
            this.Controls.Add(pnlToolbar);
        }

        // ════════════════════════════════════════════════════
        //  MAIN TABS  (Chứng từ ghi nợ | Phiếu xuất | Hóa đơn)
        // ════════════════════════════════════════════════════
        private void BuildTabContent()
        {
            tabMain = new TabControl
            {
                Location = new Point(0, 112),
                Font = new Font("Segoe UI", 9),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            this.Resize += (s, e) => { tabMain.Width = this.Width; tabMain.Height = this.Height - 112 - 102; };
            tabMain.Width  = this.Width  > 0 ? this.Width  : 1200;
            tabMain.Height = this.Height > 0 ? this.Height - 112 - 102 : 480;

            var tabCTGN = new TabPage("Chứng từ ghi nợ");
            tabMain.TabPages.Add(tabCTGN);
            tabMain.TabPages.Add(new TabPage("Phiếu xuất"));
            tabMain.TabPages.Add(new TabPage("Hóa đơn"));

            BuildTabCTGN(tabCTGN);
            this.Controls.Add(tabMain);
        }

        private void BuildTabCTGN(TabPage tab)
        {
            // ── Info section ─────────────────────────────────
            var pnlInfo = new Panel
            {
                Location = new Point(0, 0),
                BackColor = Color.FromArgb(250, 251, 253),
                Height = 215
            };
            tab.SizeChanged += (s, e) => pnlInfo.Width = tab.Width;
            pnlInfo.Width = tab.Width > 0 ? tab.Width : 1100;

            // GroupBox: Thông tin chung
            var grpChung = new GroupBox
            {
                Text = "Thông tin chung",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 80, 160),
                Location = new Point(6, 5),
                Size = new Size(680, 200)
            };
            AddInfoRow(grpChung, "Khách hàng:", 24, out txtMaKH, out txtTenKH, withPickBtn: true);
            AddSimpleRow(grpChung, "Mã số thuế:", 58, out txtMaSoThue, "Người liên hệ:", out txtNguoiLienHe);
            AddFullRow(grpChung, "Địa chỉ:", 92, out txtDiaChi);
            AddFullRow(grpChung, "Diễn giải:", 126, out txtDienGiai);
            AddNVRow(grpChung, 158, out txtNVBanHang, out txtThamChieu);
            pnlInfo.Controls.Add(grpChung);

            // GroupBox: Chứng từ
            var grpCT = new GroupBox
            {
                Text = "Chứng từ",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 80, 160),
                Location = new Point(692, 5),
                Size = new Size(310, 200)
            };
            grpCT.Controls.Add(new Label { Text = "Ngày hạch toán:", Location = new Point(10, 28), AutoSize = true, Font = new Font("Segoe UI", 9) });
            dtpNgayHachToan = new DateTimePicker { Location = new Point(135, 25), Size = new Size(155, 24), Font = new Font("Segoe UI", 9), Format = DateTimePickerFormat.Short };
            grpCT.Controls.Add(new Label { Text = "Ngày chứng từ:", Location = new Point(10, 60), AutoSize = true, Font = new Font("Segoe UI", 9) });
            dtpNgayChungTu  = new DateTimePicker { Location = new Point(135, 57), Size = new Size(155, 24), Font = new Font("Segoe UI", 9), Format = DateTimePickerFormat.Short };
            grpCT.Controls.Add(new Label { Text = "Số chứng từ:", Location = new Point(10, 92), AutoSize = true, Font = new Font("Segoe UI", 9) });
            txtSoChungTu = new TextBox { Location = new Point(135, 89), Size = new Size(155, 24), Font = new Font("Segoe UI", 9, FontStyle.Bold), ReadOnly = true, BackColor = Color.FromArgb(245, 246, 250) };
            grpCT.Controls.AddRange(new Control[] { dtpNgayHachToan, dtpNgayChungTu, txtSoChungTu });
            pnlInfo.Controls.Add(grpCT);

            tab.Controls.Add(pnlInfo);

            // ── Divider panel ────────────────────────────────
            var pnlDivider = new Panel { Location = new Point(0, 215), Height = 2, BackColor = Color.FromArgb(215, 220, 230) };
            tab.SizeChanged += (s, e) => pnlDivider.Width = tab.Width;
            pnlDivider.Width = tab.Width;
            tab.Controls.Add(pnlDivider);

            // ── Sub-tabs: 1.Hàng tiền … 5.Khác ──────────────
            var tabSub = new TabControl
            {
                Location = new Point(0, 217),
                Font = new Font("Segoe UI", 8.5f),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            tab.SizeChanged += (s, e) => { tabSub.Width = tab.Width; tabSub.Height = tab.Height - 217; };
            tabSub.Width  = tab.Width  > 0 ? tab.Width  : 1100;
            tabSub.Height = tab.Height > 0 ? tab.Height - 217 : 260;

            var tabHang = new TabPage("1. Hàng tiền");
            tabSub.TabPages.AddRange(new[] { tabHang, new TabPage("2. Thuế"), new TabPage("3. Giá vốn"), new TabPage("4. Thống kê"), new TabPage("5. Khác") });

            BuildGrid(tabHang);
            tab.Controls.Add(tabSub);
        }

        private void BuildGrid(TabPage tab)
        {
            // Right-side: loại tiền + tỷ giá
            var pnlGrdTop = new Panel { Dock = DockStyle.Top, Height = 28, BackColor = Color.FromArgb(240, 242, 248) };
            pnlGrdTop.Controls.Add(new Label { Text = "Loại tiền:", Location = new Point(600, 6), AutoSize = true, Font = new Font("Segoe UI", 9) });
            var cmbLoaiTien = new ComboBox { Location = new Point(658, 4), Size = new Size(60, 22), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9) };
            cmbLoaiTien.Items.AddRange(new object[] { "VND", "USD", "EUR" }); cmbLoaiTien.SelectedIndex = 0;
            pnlGrdTop.Controls.Add(cmbLoaiTien);
            pnlGrdTop.Controls.Add(new Label { Text = "Tỷ giá:", Location = new Point(726, 6), AutoSize = true, Font = new Font("Segoe UI", 9) });
            pnlGrdTop.Controls.Add(new Label { Text = "1,00", Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(773, 6), AutoSize = true });
            tab.Controls.Add(pnlGrdTop);

            gridHangHoa = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 32
            };
            FrmMain.StyleGrid(gridHangHoa, Color.FromArgb(26, 115, 232));

            _dtChiTiet = new DataTable();
            _dtChiTiet.Columns.AddRange(new[]
            {
                new DataColumn("MaHang",     typeof(string)),
                new DataColumn("TenHang",    typeof(string)),
                new DataColumn("TKCongNo",   typeof(string)),
                new DataColumn("TKDoanhThu", typeof(string)),
                new DataColumn("DVT",        typeof(string)),
                new DataColumn("SoLuong",    typeof(decimal)),
                new DataColumn("DonGia",     typeof(decimal)),
                new DataColumn("ThanhTien",  typeof(decimal)),
                new DataColumn("ChietKhau",  typeof(decimal)),
                new DataColumn("Ty",         typeof(decimal)),
                new DataColumn("MaSP_Hidden",typeof(int))
            });
            gridHangHoa.DataSource = _dtChiTiet;

            var colDefs = new (string Col, string Header, int W)[]
            {
                ("MaHang",    "Mã hàng",          90),
                ("TenHang",   "Tên hàng",         210),
                ("TKCongNo",  "TK công nợ/chi phí",130),
                ("TKDoanhThu","TK doanh thu",      100),
                ("DVT",       "ĐVT",               55),
                ("SoLuong",   "Số lượng",          80),
                ("DonGia",    "Đơn giá",           115),
                ("ThanhTien", "Thành tiền",        125),
                ("ChietKhau", "CK%",               55),
                ("Ty",        "Tỷ",                50),
                ("MaSP_Hidden","",                  0)
            };
            foreach (var (col, header, w) in colDefs)
            {
                if (!gridHangHoa.Columns.Contains(col)) continue;
                gridHangHoa.Columns[col].HeaderText = header;
                gridHangHoa.Columns[col].Width = w;
                if (col == "MaSP_Hidden") gridHangHoa.Columns[col].Visible = false;
            }

            gridHangHoa.CellEndEdit += (s, e) => UpdateTotals();
            tab.Controls.Add(gridHangHoa);

            // "Bấm vào đây để thêm mới" hint row — handled by AllowUserToAddRows
            // Bottom: Số dòng label
            lblSoDong = new Label { Text = "Số dòng = 0", Location = new Point(6, 4), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(32, 40, 62) };
            _dtChiTiet.RowChanged  += (s, e) => UpdateSoDong();
            _dtChiTiet.RowDeleted  += (s, e) => UpdateSoDong();
        }

        // ════════════════════════════════════════════════════
        //  FOOTER
        // ════════════════════════════════════════════════════
        private void BuildFooter()
        {
            var pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 100, BackColor = Color.White, Padding = new Padding(8) };
            pnlFooter.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(200, 205, 215)), 0, 0, pnlFooter.Width, 0);

            lblSoDong = new Label { Text = "Số dòng = 0", Location = new Point(10, 8), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            pnlFooter.Controls.Add(lblSoDong);

            void AddTotal(string label, int lx, int ly, ref Label valueLabel, Color color)
            {
                pnlFooter.Controls.Add(new Label { Text = label, Location = new Point(lx, ly), AutoSize = true, Font = new Font("Segoe UI", 9) });
                valueLabel = new Label { Text = "0", Location = new Point(lx + 150, ly), Size = new Size(155, 20), Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = color, TextAlign = ContentAlignment.MiddleRight };
                pnlFooter.Controls.Add(valueLabel);
            }

            AddTotal("Tổng tiền hàng:",    10, 36, ref lblTongTienHang!,  Color.FromArgb(32, 40, 62));
            AddTotal("Tiền chiết khấu:",   10, 60, ref lblTienChietKhau!, Color.FromArgb(255, 143, 0));
            AddTotal("Tiền thuế GTGT:",   360, 36, ref lblTienThue!,      Color.FromArgb(52, 168, 83));
            AddTotal("Tổng tiền thanh toán:", 360, 60, ref lblTongThanhToan!, Color.FromArgb(220, 53, 69));

            var btnPhanBo  = MakeFooterBtn("Phân bổ chiết khấu...", new Point(680, 32));
            var btnCongNo  = MakeFooterBtn("Xem công nợ",           new Point(680, 60));
            pnlFooter.Controls.AddRange(new Control[] { btnPhanBo, btnCongNo });

            this.Controls.Add(pnlFooter);

            var pnlHint = new Panel { Dock = DockStyle.Bottom, Height = 22, BackColor = Color.FromArgb(26, 80, 160) };
            pnlHint.Controls.Add(new Label { Text = "  F8 - Chọn chứng từ nhập, Ctrl+F2 - Xem số tồn", Font = new Font("Segoe UI", 7.5f), ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft });
            this.Controls.Add(pnlHint);
        }

        private static Button MakeFooterBtn(string text, Point loc)
        {
            var b = new Button
            {
                Text = text, Location = loc, Size = new Size(175, 26),
                FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(237, 239, 244),
                Font = new Font("Segoe UI", 8.5f), Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderColor = Color.FromArgb(210, 215, 225);
            return b;
        }

        // ════════════════════════════════════════════════════
        //  HELPER: row builders
        // ════════════════════════════════════════════════════
        private void AddInfoRow(GroupBox g, string lbl, int y, out TextBox ma, out TextBox ten, bool withPickBtn)
        {
            g.Controls.Add(new Label { Text = lbl, Location = new Point(10, y + 4), AutoSize = true, Font = new Font("Segoe UI", 9) });
            ma = new TextBox { Location = new Point(110, y), Size = new Size(115, 24), Font = new Font("Segoe UI", 9) };
            ten = new TextBox { Location = new Point(265, y), Size = new Size(390, 24), Font = new Font("Segoe UI", 9) };
            g.Controls.AddRange(new Control[] { ma, ten });

            if (withPickBtn)
            {
                var btn = new Button { Text = "▼", Location = new Point(227, y), Size = new Size(28, 24), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(26, 115, 232), ForeColor = Color.White };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += (s, e) => PickKhachHang();
                g.Controls.Add(btn);
                var btnPlus = new Button { Text = "+", Location = new Point(258, y), Size = new Size(24, 24), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White };
                btnPlus.FlatAppearance.BorderSize = 0;
                g.Controls.Add(btnPlus);
                ten.Location = new Point(286, y);
                ten.Size = new Size(382, 24);
            }
        }

        private void AddSimpleRow(GroupBox g, string l1, int y, out TextBox t1, string l2, out TextBox t2)
        {
            g.Controls.Add(new Label { Text = l1, Location = new Point(10, y + 4), AutoSize = true, Font = new Font("Segoe UI", 9) });
            t1 = new TextBox { Location = new Point(110, y), Size = new Size(140, 24), Font = new Font("Segoe UI", 9) };
            g.Controls.Add(new Label { Text = l2, Location = new Point(260, y + 4), AutoSize = true, Font = new Font("Segoe UI", 9) });
            t2 = new TextBox { Location = new Point(358, y), Size = new Size(308, 24), Font = new Font("Segoe UI", 9) };
            g.Controls.AddRange(new Control[] { t1, t2 });
        }

        private void AddFullRow(GroupBox g, string lbl, int y, out TextBox t)
        {
            g.Controls.Add(new Label { Text = lbl, Location = new Point(10, y + 4), AutoSize = true, Font = new Font("Segoe UI", 9) });
            t = new TextBox { Location = new Point(110, y), Size = new Size(556, 24), Font = new Font("Segoe UI", 9) };
            g.Controls.Add(t);
        }

        private void AddNVRow(GroupBox g, int y, out TextBox nv, out TextBox thamChieu)
        {
            g.Controls.Add(new Label { Text = "NV bán hàng:", Location = new Point(10, y + 4), AutoSize = true, Font = new Font("Segoe UI", 9) });
            nv = new TextBox { Location = new Point(110, y), Size = new Size(140, 24), Font = new Font("Segoe UI", 9) };
            var btnNV = new Button { Text = "+", Location = new Point(252, y), Size = new Size(24, 24), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(40, 167, 69), ForeColor = Color.White };
            btnNV.FlatAppearance.BorderSize = 0;
            g.Controls.Add(new Label { Text = "Tham chiếu:", Location = new Point(285, y + 4), AutoSize = true, Font = new Font("Segoe UI", 9) });
            thamChieu = new TextBox { Location = new Point(365, y), Size = new Size(120, 24), Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(26, 115, 232) };
            var btnRef = new Button { Text = "…", Location = new Point(487, y), Size = new Size(28, 24), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(240, 242, 248) };
            btnRef.FlatAppearance.BorderColor = Color.FromArgb(200, 205, 215);
            var btnPlus2 = new Button { Text = "+", Location = new Point(517, y), Size = new Size(24, 24), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(26, 115, 232), ForeColor = Color.White };
            btnPlus2.FlatAppearance.BorderSize = 0;
            g.Controls.AddRange(new Control[] { nv, btnNV, thamChieu, btnRef, btnPlus2 });
        }

        // ════════════════════════════════════════════════════
        //  DATA LOGIC
        // ════════════════════════════════════════════════════
        private void LoadDonHang()
        {
            try
            {
                var sql = @"SELECT dh.maDonHang, dh.soDonHang, kh.hoTen, dh.tongTien, dh.trangThai
                            FROM DonHang dh LEFT JOIN KhachHang kh ON kh.maKH = dh.maKH
                            ORDER BY dh.maDonHang DESC";
                var dt = DatabaseHelper.ExecuteQuery(sql);
                _dsDonHang.Clear();
                foreach (DataRow row in dt.Rows)
                    _dsDonHang.Add((
                        Convert.ToInt32(row["maDonHang"]),
                        row["soDonHang"]?.ToString() ?? "",
                        row["hoTen"]?.ToString() ?? "",
                        row["tongTien"] != DBNull.Value ? Convert.ToDecimal(row["tongTien"]) : 0,
                        row["trangThai"]?.ToString() ?? ""));

                if (_dsDonHang.Count > 0) { _currentIndex = 0; LoadCurrentRecord(); }
                else NewRecord();
            }
            catch { LoadDemoData(); }
        }

        private void LoadDemoData()
        {

            _dsDonHang.Clear();
            _dsDonHang.Add((19, "BH00019", "Đại lý Bình Thạnh", 112703338, "BanNhap"));
            _currentIndex = 0;
            LoadCurrentRecord();

        }

        private void LoadCurrentRecord()
        {
            if (_dsDonHang.Count == 0) { NewRecord(); return; }
            var dh = _dsDonHang[_currentIndex];
            _currentDonHangId = dh.Id;
            txtSoChungTu.Text = dh.SoDH.Length > 0 ? dh.SoDH : $"BH{dh.Id:D5}";
            txtTenKH.Text = dh.TenKH;

            try
            {
                var sql = @"SELECT dh2.maKH, kh.hoTen, kh.diaChi, dh2.ngayDatHang,
                                   dh2.tongTien, dh2.dienGiai, dh2.soDonHang
                            FROM DonHang dh2 LEFT JOIN KhachHang kh ON kh.maKH = dh2.maKH
                            WHERE dh2.maDonHang = @id";
                var dt = DatabaseHelper.ExecuteQuery(sql, new() { { "@id", _currentDonHangId } });
                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    txtMaKH.Text  = row["maKH"].ToString() ?? "";
                    txtTenKH.Text = row["hoTen"].ToString() ?? "";
                    txtDiaChi.Text    = row["diaChi"].ToString() ?? "";
                    txtDienGiai.Text  = row["dienGiai"].ToString() ?? "";
                    if (row["ngayDatHang"] != DBNull.Value)
                    {
                        dtpNgayHachToan.Value = Convert.ToDateTime(row["ngayDatHang"]);
                        dtpNgayChungTu.Value  = Convert.ToDateTime(row["ngayDatHang"]);
                    }
                    txtSoChungTu.Text = row["soDonHang"].ToString() ?? $"BH{_currentDonHangId:D5}";
                }
                LoadChiTiet(_currentDonHangId);
            }
            catch
            {
                // demo data
                txtMaKH.Text = "DL_BINHTHANH";
                txtTenKH.Text = "Đại lý Bình Thạnh";
                txtDiaChi.Text = "Xô Viết Nghệ Tĩnh, Bình Thạnh, TP.HCM";
                txtDienGiai.Text = "Bán hàng Đại lý Bình Thạnh";
                txtThamChieu.Text = "BG00001";
                txtSoChungTu.Text = "BH00019";
                dtpNgayHachToan.Value = new DateTime(2017, 1, 16);
                dtpNgayChungTu.Value  = new DateTime(2017, 1, 16);
                LoadDemoChiTiet();
            }
        }

        private void LoadDemoChiTiet()
        {
            _dtChiTiet.Rows.Clear();
            void AddRow(string ma, string ten, string tkCN, string tkDT, string dvt, decimal sl, decimal dg, decimal ck)
            {
                var r = _dtChiTiet.NewRow();
                r["MaHang"] = ma; r["TenHang"] = ten; r["TKCongNo"] = tkCN; r["TKDoanhThu"] = tkDT;
                r["DVT"] = dvt; r["SoLuong"] = sl; r["DonGia"] = dg;
                r["ThanhTien"] = sl * dg; r["ChietKhau"] = ck; r["Ty"] = 0m; r["MaSP_Hidden"] = 1;
                _dtChiTiet.Rows.Add(r);
            }
            AddRow("MASK1", "Mặt nạ đất sét Innisfree Jeju",       "131", "5111", "Hũ", 10, 195000, 0);
            AddRow("TONER1", "Toner Hada Labo Gokujyun 170ml", "131", "5111", "Chai", 7, 220000, 0);
            AddRow("BB5", "Xịt thơm body Bath & Body Works", "131", "5111", "Chai", 5, 320000, 0);
            UpdateTotals(); UpdateSoDong();
        }

        private void LoadChiTiet(int maDH)
        {
            try
            {
                var sql = @"SELECT ctdh.maSP AS MaHang, sp.tenSP AS TenHang,
                                   '131' AS TKCongNo, '5111' AS TKDoanhThu, N'Chiếc' AS DVT,
                                   ctdh.soLuong AS SoLuong, ctdh.donGia AS DonGia,
                                   (ctdh.soLuong * ctdh.donGia) AS ThanhTien,
                                   ctdh.chietKhau AS ChietKhau, 0 AS Ty, ctdh.maSP AS MaSP_Hidden
                            FROM ChiTietDonHang ctdh JOIN SanPham sp ON sp.maSP = ctdh.maSP
                            WHERE ctdh.maDonHang = @id";
                var dt = DatabaseHelper.ExecuteQuery(sql, new() { { "@id", maDH } });
                _dtChiTiet.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    var nr = _dtChiTiet.NewRow();
                    foreach (DataColumn col in _dtChiTiet.Columns)
                        if (dt.Columns.Contains(col.ColumnName)) nr[col] = row[col.ColumnName];
                    _dtChiTiet.Rows.Add(nr);
                }
                UpdateTotals(); UpdateSoDong();
            }
            catch { LoadDemoChiTiet(); }
        }

        private void NavRecord(int dir)
        {
            if (_dsDonHang.Count == 0) return;
            _currentIndex = Math.Clamp(_currentIndex + dir, 0, _dsDonHang.Count - 1);
            LoadCurrentRecord();
        }

        private void NewRecord()
        {
            _currentDonHangId = 0;
            txtMaKH.Text = txtTenKH.Text = txtMaSoThue.Text = "";
            txtNguoiLienHe.Text = txtDiaChi.Text = txtDienGiai.Text = txtNVBanHang.Text = "";
            txtThamChieu.Text = "";
            dtpNgayHachToan.Value = dtpNgayChungTu.Value = DateTime.Today;
            txtSoChungTu.Text = $"BH{DateTime.Now:yyyyMMddHHmm}";
            _dtChiTiet.Rows.Clear();
            UpdateTotals(); UpdateSoDong();
        }

        private void EditRecord() =>
            MessageBox.Show("Chế độ sửa đã được kích hoạt.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void SaveRecord()
        {
            if (string.IsNullOrWhiteSpace(txtMaKH.Text))
            { MessageBox.Show("Vui lòng chọn khách hàng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            try
            {
                if (!int.TryParse(txtMaKH.Text, out int maKH)) maKH = 1;
                decimal tongTien = _dtChiTiet.Rows.Cast<DataRow>()
                    .Where(r => r.RowState != DataRowState.Deleted && r["ThanhTien"] != DBNull.Value)
                    .Sum(r => Convert.ToDecimal(r["ThanhTien"]));

                if (_currentDonHangId == 0)
                {
                    var res = DatabaseHelper.ExecuteScalar(
                        "INSERT INTO DonHang (maKH,ngayDatHang,trangThai,tongTien,dienGiai) OUTPUT INSERTED.maDonHang VALUES (@maKH,@ngay,N'BanNhap',@tt,@dg)",
                        new() { {"@maKH",maKH},{"@ngay",dtpNgayChungTu.Value},{"@tt",tongTien},{"@dg",txtDienGiai.Text} });
                    _currentDonHangId = Convert.ToInt32(res);
                    txtSoChungTu.Text = $"BH{_currentDonHangId:D5}";
                    MessageBox.Show($"Đã lưu! Mã: {_currentDonHangId}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDonHang();
                }
                else
                {
                    DatabaseHelper.ExecuteNonQuery(
                        "UPDATE DonHang SET tongTien=@tt,dienGiai=@dg,ngayDatHang=@ngay WHERE maDonHang=@id",
                        new() { {"@tt",tongTien},{"@dg",txtDienGiai.Text},{"@ngay",dtpNgayChungTu.Value},{"@id",_currentDonHangId} });
                    MessageBox.Show("Đã cập nhật!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void DeleteRecord()
        {
            if (_currentDonHangId == 0) return;
            if (MessageBox.Show("Xóa chứng từ này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                DatabaseHelper.ExecuteNonQuery("DELETE FROM ChiTietDonHang WHERE maDonHang=@id", new() { {"@id",_currentDonHangId} });
                DatabaseHelper.ExecuteNonQuery("DELETE FROM DonHang WHERE maDonHang=@id", new() { {"@id",_currentDonHangId} });
                MessageBox.Show("Đã xóa!", "Thông báo");
                LoadDonHang();
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void LapHoaDon()
        {
            if (_currentDonHangId == 0) { MessageBox.Show("Chưa có chứng từ!", "Lỗi"); return; }
            MessageBox.Show($"Đã lập hóa đơn cho {txtSoChungTu.Text}", "Thông báo");
        }

        private void InChungTu() => MessageBox.Show("Chức năng in chứng từ.", "Thông báo");

        private void PickKhachHang()
        {
            try
            {
                var dt = DatabaseHelper.ExecuteQuery("SELECT maKH, hoTen, sdt, diaChi FROM KhachHang ORDER BY hoTen");
                var frm = new FrmPickItem("Chọn khách hàng", dt,
                    new[] { "maKH", "hoTen", "sdt", "diaChi" },
                    new[] { "Mã KH", "Họ tên", "SĐT", "Địa chỉ" });
                if (frm.ShowDialog() == DialogResult.OK && frm.SelectedRow != null)
                {
                    txtMaKH.Text  = frm.SelectedRow["maKH"].ToString() ?? "";
                    txtTenKH.Text = frm.SelectedRow["hoTen"].ToString() ?? "";
                    txtDiaChi.Text = frm.SelectedRow["diaChi"].ToString() ?? "";
                }
            }
            catch { MessageBox.Show("Không thể kết nối CSDL.", "Cảnh báo"); }
        }

        private void UpdateTotals()
        {
            decimal tongTien = 0, tongCK = 0;
            foreach (DataRow row in _dtChiTiet.Rows)
            {
                if (row.RowState == DataRowState.Deleted) continue;
                decimal tt = row["ThanhTien"] != DBNull.Value ? Convert.ToDecimal(row["ThanhTien"]) : 0;
                decimal ck = row["ChietKhau"]  != DBNull.Value ? Convert.ToDecimal(row["ChietKhau"]) : 0;
                tongTien += tt; tongCK += tt * ck / 100;
            }
            decimal thue = (tongTien - tongCK) * 0.1m;
            lblTongTienHang.Text  = tongTien.ToString("N0");
            lblTienChietKhau.Text = tongCK.ToString("N0");
            lblTienThue.Text      = thue.ToString("N0");
            lblTongThanhToan.Text = (tongTien - tongCK + thue).ToString("N0");
        }

        private void UpdateSoDong()
        {
            if (lblSoDong == null) return;
            int count = _dtChiTiet.Rows.Cast<DataRow>().Count(r => r.RowState != DataRowState.Deleted);
            lblSoDong.Text = $"Số dòng = {count}";
        }
    }
}
