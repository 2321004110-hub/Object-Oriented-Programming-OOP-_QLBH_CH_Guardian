using QLBH_Guardian.DataAccess;
using System.Data;

namespace QLBH_Guardian.Forms
{
   
    public class FrmDonHang : UserControl
    {
        private Panel pnlFilter = null!;
        private Panel pnlMain   = null!;
        private Panel pnlTopBar = null!;
        private DataGridView gridDonHang = null!;
        private Panel pnlPaging  = null!;
        private Panel pnlDetail  = null!;
        private Panel pnlHangHoa = null!;
        private Label lblDetailTitle = null!;
        private Button btnThem = null!, btnLamMoi = null!;
        private TextBox txtSearch = null!;
        private ComboBox cmbTrangThai = null!;
        private Label lblPaging = null!;
        private int _selectedDonHangId = 0;
        private int _pageSize = 10, _currentPage = 1, _totalRows = 0;

        public FrmDonHang()
        {
            InitializeComponent();
            LoadDonHang();
        }

        private void InitializeComponent()
        {
            this.BackColor = Color.FromArgb(248, 249, 252);
            BuildLayout();
        }

        private void BuildLayout()
        {
            // ── CRM top nav ───────────────────────────────────
            var pnlCRMTop = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.White };
            pnlCRMTop.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(215, 220, 230)), 0, pnlCRMTop.Height - 1, pnlCRMTop.Width, pnlCRMTop.Height - 1);

            var crmTabs = new[] { "Tổng quan", "Tiềm năng", "Chào hàng", "Liên hệ", "Khách hàng", "Cơ hội", "Đơn hàng", "Báo giá", "Hóa đơn", "Báo cáo", "Chiến dịch", "Khác" };
            int cx = 10;
            foreach (var tab in crmTabs)
            {
                var isActive = tab == "Đơn hàng";
                var lbl = new Label
                {
                    Text = tab,
                    Font = isActive ? new Font("Segoe UI", 9, FontStyle.Bold) : new Font("Segoe UI", 9),
                    ForeColor = isActive ? Color.FromArgb(26, 115, 232) : Color.FromArgb(60, 70, 90),
                    Location = new Point(cx, 15),
                    AutoSize = true,
                    Cursor = Cursors.Hand
                };
                pnlCRMTop.Controls.Add(lbl);
                if (isActive)
                {
                    var pnlUnder = new Panel { Location = new Point(cx, 44), Height = 3, BackColor = Color.FromArgb(26, 115, 232) };
                    pnlUnder.Width = lbl.PreferredSize.Width;
                    pnlCRMTop.Controls.Add(pnlUnder);
                }
                cx += lbl.PreferredSize.Width + 20;
            }
            this.Controls.Add(pnlCRMTop);

            // ── LEFT FILTER SIDEBAR ──────────────────────────
            pnlFilter = new Panel
            {
                Location = new Point(0, 50),
                Width = 215,
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
            };
            this.Resize += (s, e) => pnlFilter.Height = this.Height - 50;
            pnlFilter.Height = this.Height > 50 ? this.Height - 50 : 680;
            pnlFilter.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(215, 220, 230)), pnlFilter.Width - 1, 0, pnlFilter.Width - 1, pnlFilter.Height);

            var tabStrip = new TabControl { Location = new Point(10, 8), Size = new Size(192, 30), ItemSize = new Size(90, 24), Appearance = TabAppearance.Buttons };
            tabStrip.TabPages.Add(new TabPage("Bộ lọc"));
            tabStrip.TabPages.Add(new TabPage("Thống kê"));
            pnlFilter.Controls.Add(tabStrip);

            var lblSaved = new Label { Text = "BỘ LỌC ĐÃ LƯU", Font = new Font("Segoe UI", 7.5f, FontStyle.Bold), ForeColor = Color.Gray, Location = new Point(12, 48), AutoSize = true };
            pnlFilter.Controls.Add(lblSaved);
            var lbl1 = new Label { Text = "Tại Thủ Đức", Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(26,115,232), Location = new Point(12, 68), AutoSize = true, Cursor = Cursors.Hand };
            pnlFilter.Controls.Add(lbl1);

            var lblFlt = new Label { Text = "LỌC ĐƠN HÀNG", Font = new Font("Segoe UI", 7.5f, FontStyle.Bold), ForeColor = Color.Gray, Location = new Point(12, 100), AutoSize = true };
            var txtFlt = new TextBox { Location = new Point(12, 118), Size = new Size(187, 24), Font = new Font("Segoe UI", 9), PlaceholderText = "Tìm kiếm trường..." };
            pnlFilter.Controls.AddRange(new Control[] { lblFlt, txtFlt });

            string[] flds = { "Tình trạng ghi DS", "Số đơn hàng", "Diễn giải", "Giá trị đơn hàng", "Doanh số ghi nhận", "Ngày đặt hàng", "Ngày ghi sổ" };
            int fy = 148;
            foreach (var f in flds)
            {
                pnlFilter.Controls.Add(new CheckBox { Text = f, Font = new Font("Segoe UI", 9), Location = new Point(12, fy), AutoSize = true });
                fy += 26;
            }
            var lblMore = new Label { Text = "─────  Xem thêm  ─────", Font = new Font("Segoe UI", 8.5f), ForeColor = Color.FromArgb(26,115,232), Location = new Point(8, fy + 6), AutoSize = true, Cursor = Cursors.Hand };
            pnlFilter.Controls.Add(lblMore);
            this.Controls.Add(pnlFilter);

            // ── MAIN PANEL ──────────────────────────────────
            pnlMain = new Panel
            {
                Location = new Point(216, 50),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            pnlMain.Size = new Size(Math.Max(this.Width - 216, 800), Math.Max(this.Height - 50, 620));
            this.Resize += (s, e) => pnlMain.Size = new Size(this.Width - 216, this.Height - 50);

            BuildTopBar();
            BuildGrid();
            BuildPaging();
            BuildDetailPanel();

            this.Controls.Add(pnlMain);
        }

        private void BuildTopBar()
        {
            pnlTopBar = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = Color.White, Padding = new Padding(10) };
            pnlTopBar.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(215, 220, 230)), 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);

            var lblTitle = new Label
            {
                Text = "Tất cả đơn hàng  ▾",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(32, 40, 62),
                Location = new Point(12, 14),
                AutoSize = true
            };

            txtSearch = new TextBox { Location = new Point(240, 13), Size = new Size(220, 26), Font = new Font("Segoe UI", 9), PlaceholderText = "🔍 Tìm kiếm..." };
            txtSearch.TextChanged += (s, e) => { _currentPage = 1; LoadDonHang(); };

            cmbTrangThai = new ComboBox { Location = new Point(472, 13), Size = new Size(130, 26), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9) };
            cmbTrangThai.Items.AddRange(new object[] { "-- Tất cả --", "BanNhap", "DeNghiGhi", "DaGhi", "TuChoiGhi", "HoanThanh", "DaHuy" });
            cmbTrangThai.SelectedIndex = 0;
            cmbTrangThai.SelectedIndexChanged += (s, e) => { _currentPage = 1; LoadDonHang(); };

            btnThem = new Button { Text = "＋ Thêm", Size = new Size(100, 28), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(26,115,232), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnThem.FlatAppearance.BorderSize = 0;
            btnThem.Location = new Point(pnlTopBar.Width - 200, 12);
            pnlTopBar.Resize += (s, e) => btnThem.Location = new Point(pnlTopBar.Width - 200, 12);
            btnThem.Click += (s, e) => OpenNewDonHang();

            btnLamMoi = new Button { Text = "🔄", Size = new Size(34, 28), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(237,239,244), Font = new Font("Segoe UI", 10), Cursor = Cursors.Hand, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnLamMoi.Location = new Point(pnlTopBar.Width - 90, 12);
            pnlTopBar.Resize += (s, e) => btnLamMoi.Location = new Point(pnlTopBar.Width - 90, 12);
            btnLamMoi.Click += (s, e) => LoadDonHang();

            pnlTopBar.Controls.AddRange(new Control[] { lblTitle, txtSearch, cmbTrangThai, btnThem, btnLamMoi });
            pnlMain.Controls.Add(pnlTopBar);
        }

        private void BuildGrid()
        {
            gridDonHang = new DataGridView
            {
                Location = new Point(0, 52),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 38,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right
            };
            gridDonHang.Size = new Size(Math.Max(pnlMain.Width - 320, 400), Math.Max(pnlMain.Height - 52 - 44, 300));
            pnlMain.Resize += (s, e) =>
            {
                gridDonHang.Size = new Size(pnlMain.Width - 320, pnlMain.Height - 52 - 44);
                pnlDetail.Location = new Point(pnlMain.Width - 318, 52);
                pnlDetail.Size = new Size(318, pnlMain.Height - 52 - 44);
            };

            FrmMain.StyleGrid(gridDonHang, Color.White);
            gridDonHang.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(70, 80, 100);
            gridDonHang.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            gridDonHang.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);

            // Checkbox col
            gridDonHang.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Check", HeaderText = "", Width = 36 });

            var cols = new (string Name, string Header, int Width, bool Right)[]
            {
                ("TrangThai",     "Tình trạng ghi DS", 145, false),
                ("SoDonHang",     "Số đơn hàng",       108, false),
                ("DienGiai",      "Diễn giải",         290, false),
                ("GiaTriDonHang", "Giá trị đơn hàng",  130, true),
            };
            foreach (var (name, header, width, right) in cols)
            {
                var col = new DataGridViewTextBoxColumn { Name = name, HeaderText = header, Width = width };
                if (right) col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                gridDonHang.Columns.Add(col);
            }

            gridDonHang.CellFormatting    += GridDonHang_CellFormatting;
            gridDonHang.SelectionChanged  += GridDonHang_SelectionChanged;
            pnlMain.Controls.Add(gridDonHang);
        }

        private void GridDonHang_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= gridDonHang.Rows.Count) return;
            var tt = gridDonHang.Rows[e.RowIndex].Cells["TrangThai"]?.Value?.ToString() ?? "";
            var colName = gridDonHang.Columns[e.ColumnIndex].Name;

            if (colName == "TrangThai")
            {
                e.CellStyle.ForeColor = tt switch
                {
                    "DaGhi" or "HoanThanh" => Color.FromArgb(40, 167, 69),
                    "DeNghiGhi"            => Color.FromArgb(255, 143, 0),
                    "TuChoiGhi" or "DaHuy" => Color.FromArgb(220, 53, 69),
                    _                      => Color.FromArgb(80, 90, 110)
                };
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            if (colName == "SoDonHang")
            {
                e.CellStyle.ForeColor = Color.FromArgb(26, 115, 232);
                e.CellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
        }

        private void GridDonHang_SelectionChanged(object? sender, EventArgs e)
        {
            if (gridDonHang.SelectedRows.Count == 0) return;
            var row = gridDonHang.SelectedRows[0];
            if (row.Tag is int id) { _selectedDonHangId = id; LoadChiTiet(id); }
        }

        private void BuildPaging()
        {
            pnlPaging = new Panel { Dock = DockStyle.Bottom, Height = 44, BackColor = Color.White };
            pnlPaging.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(215, 220, 230)), 0, 0, pnlPaging.Width, 0);

            lblPaging = new Label { Text = "Tổng số: 0  |  1 đến 0", Font = new Font("Segoe UI", 9), ForeColor = Color.FromArgb(70,80,100), Location = new Point(12, 14), AutoSize = true };

            var cmbSize = new ComboBox { Location = new Point(260, 10), Size = new Size(58, 24), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 8.5f) };
            cmbSize.Items.AddRange(new object[] { "10", "25", "50", "100" }); cmbSize.SelectedIndex = 0;
            cmbSize.SelectedIndexChanged += (s, e) => { _pageSize = int.Parse(cmbSize.SelectedItem!.ToString()!); _currentPage = 1; LoadDonHang(); };

            Button NavBtn(string t, int x) { var b = new Button { Text = t, Size = new Size(28, 24), Location = new Point(x, 10), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9), Cursor = Cursors.Hand }; return b; }
            var btnFirst = NavBtn("|◄", 328); btnFirst.Click += (s, e) => { _currentPage = 1; LoadDonHang(); };
            var btnPrev  = NavBtn("◄",  360); btnPrev.Click  += (s, e) => { if (_currentPage > 1) { _currentPage--; LoadDonHang(); } };
            var btnNext  = NavBtn("►",  392); btnNext.Click  += (s, e) => { int t = (int)Math.Ceiling((double)_totalRows/_pageSize); if (_currentPage < t) { _currentPage++; LoadDonHang(); } };
            var btnLast  = NavBtn("►|", 424); btnLast.Click  += (s, e) => { _currentPage = (int)Math.Ceiling((double)_totalRows/_pageSize); LoadDonHang(); };

            pnlPaging.Controls.AddRange(new Control[] { lblPaging, cmbSize, btnFirst, btnPrev, btnNext, btnLast });
            pnlMain.Controls.Add(pnlPaging);
        }

        private void BuildDetailPanel()
        {
            pnlDetail = new Panel
            {
                Location = new Point(pnlMain.Width - 318, 52),
                Size = new Size(318, Math.Max(pnlMain.Height - 96, 400)),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
            };
            pnlDetail.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(210, 215, 225)), 0, 0, 0, pnlDetail.Height);

            lblDetailTitle = new Label { Text = "Thông tin hàng hóa  4", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(32,40,62), Location = new Point(15, 14), AutoSize = true };

            pnlHangHoa = new Panel { Location = new Point(0, 45), Size = new Size(318, 400), AutoScroll = true, BackColor = Color.White };
            pnlHangHoa.Controls.Add(new Label { Text = "Chọn một đơn hàng để xem chi tiết", Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, Location = new Point(15, 12), AutoSize = true });
            pnlDetail.Resize += (s, e) => pnlHangHoa.Size = new Size(pnlDetail.Width, pnlDetail.Height - 45);

            pnlDetail.Controls.AddRange(new Control[] { lblDetailTitle, pnlHangHoa });
            pnlMain.Controls.Add(pnlDetail);
        }

        private void LoadDonHang()
        {
            try
            {
                string ttF = cmbTrangThai.SelectedIndex > 0 ? $"AND dh.trangThai = N'{cmbTrangThai.SelectedItem}'" : "";
                string srF = !string.IsNullOrEmpty(txtSearch.Text)
                    ? $"AND (kh.hoTen LIKE N'%{txtSearch.Text}%' OR dh.soDonHang LIKE N'%{txtSearch.Text}%')" : "";

                var countRes = DatabaseHelper.ExecuteScalar($"SELECT COUNT(*) FROM DonHang dh LEFT JOIN KhachHang kh ON kh.maKH=dh.maKH WHERE 1=1 {ttF} {srF}");
                _totalRows = countRes != null ? Convert.ToInt32(countRes) : 0;

                int offset = (_currentPage - 1) * _pageSize;
                var sql = $@"SELECT dh.maDonHang,
                             dh.trangThai AS TrangThai,
                             ISNULL(dh.soDonHang, CONCAT(N'DH',FORMAT(dh.maDonHang,'00000'))) AS SoDonHang,
                             ISNULL(dh.dienGiai, CONCAT(N'Đơn hàng bán mới cho ', kh.hoTen)) AS DienGiai,
                             FORMAT(dh.tongTien,'N0') AS GiaTriDonHang
                         FROM DonHang dh LEFT JOIN KhachHang kh ON kh.maKH=dh.maKH
                         WHERE 1=1 {ttF} {srF}
                         ORDER BY dh.maDonHang DESC
                         OFFSET {offset} ROWS FETCH NEXT {_pageSize} ROWS ONLY";
                var dt = DatabaseHelper.ExecuteQuery(sql);
                gridDonHang.Rows.Clear();
                foreach (DataRow row in dt.Rows)
                {
                    int i = gridDonHang.Rows.Add(false, row["TrangThai"], row["SoDonHang"], row["DienGiai"], row["GiaTriDonHang"]);
                    gridDonHang.Rows[i].Tag = Convert.ToInt32(row["maDonHang"]);
                }
                int from = _totalRows == 0 ? 0 : offset + 1;
                int to   = Math.Min(offset + _pageSize, _totalRows);
                lblPaging.Text = $"Tổng số: {_totalRows}  |  {from} đến {to}";
            }
            catch { LoadDemoDonHang(); }
        }

        private void LoadDemoDonHang()
        {
            gridDonHang.Rows.Clear();
            var demos = new[]
            {
                ("BanNhap",   "DH00001","Đơn hàng bán mới cho Cửa hàng Guardian Bình Thạnh",       "1.250.000"),
                ("DeNghiGhi", "DH00002","Đơn hàng bán mới cho Cửa hàng Guardian Quận 1",        "5.000.000"),
                ("DaGhi",     "DH00003","Đơn hàng bán mới cho Cửa hàng Guardian Thủ Đức", "2.250.000"),
                ("TuChoiGhi", "DH00004","Đơn hàng bán mới cho Cửa hàng Guardian Quận 4",       "4.800.000"),
                ("BanNhap",   "DH00005","Đơn hàng bán mới cho Cửa hàng Guardian Nhà Bè",       "1.250.000"),
                ("DeNghiGhi", "DH00006","Đơn hàng bán mới cho Cửa hàng Guardian Thảo Điền",        "5.000.000"),
                ("DaGhi",     "DH00007","Đơn hàng bán mới cho Cửa hàng Guardian Quận 9", "2.250.000"),
                ("TuChoiGhi", "DH00008","Đơn hàng bán mới cho Cửa hàng Guardian Bình Chánh",       "4.800.000"),
                ("HoanThanh", "DH00009","Đơn hàng bán mới cho Cửa hàng Guardian Quận 7",       "1.250.000"),
            };
            int i = 1;
            foreach (var (tt, so, dg, gt) in demos)
            {
                int idx = gridDonHang.Rows.Add(false, tt, so, dg, gt);
                gridDonHang.Rows[idx].Tag = i++;
            }
            _totalRows = 9;
            lblPaging.Text = "Tổng số: 9  |  1 đến 9";
        }

        private void LoadChiTiet(int maDH)
        {
            pnlHangHoa.Controls.Clear();
            try
            {
                var sql = @"SELECT sp.tenSP, ctdh.soLuong, ctdh.donGia, ctdh.chietKhau,
                                   (ctdh.soLuong*ctdh.donGia) AS thanhTien, tk.soLuongTon
                            FROM ChiTietDonHang ctdh JOIN SanPham sp ON sp.maSP=ctdh.maSP
                            LEFT JOIN TonKho tk ON tk.maSP=ctdh.maSP
                            WHERE ctdh.maDonHang=@id";
                var dt = DatabaseHelper.ExecuteQuery(sql, new() { { "@id", maDH } });
                if (dt.Rows.Count == 0) { RenderDemoDetail(maDH); return; }
                RenderChiTiet(dt);
            }
            catch { RenderDemoDetail(maDH); }
        }

        private void RenderChiTiet(DataTable dt)
        {
            int y = 8; decimal total = 0; int num = 1;
            foreach (DataRow row in dt.Rows)
            {
                string name = row["tenSP"].ToString() ?? "";
                int qty = Convert.ToInt32(row["soLuong"]);
                decimal price = Convert.ToDecimal(row["donGia"]);
                decimal ck    = row["chietKhau"] != DBNull.Value ? Convert.ToDecimal(row["chietKhau"]) : 0;
                decimal tt    = Convert.ToDecimal(row["thanhTien"]);
                int ton       = row["soLuongTon"] != DBNull.Value ? Convert.ToInt32(row["soLuongTon"]) : 0;
                AddDetailCard(ref y, num++, name, qty, price, ck, tt, ton);
                total += tt;
            }
            AddDetailFooter(y, dt.Rows.Count, total);
            lblDetailTitle.Text = $"Thông tin hàng hóa  {dt.Rows.Count}";
        }

        private void RenderDemoDetail(int id)
        {
            var items = id % 3 == 0
                ? new[] { ("Serum dưỡng trắng Some By Mi", 50, 520000, 30m, 4), ("Serum dưỡng trắng Some By Mi", 1, 10000000m, 30m, 4) }
                : new[] { ("Kem ủ tóc Moroccan Oil", 3, 650000m, 5m, 2) };
            int y = 8; decimal total = 0; int num = 1;
            foreach (var (name, qty, price, ck, ton) in items)
            {
                decimal tt = qty * price * (1 - ck / 100m);
                AddDetailCard(ref y, num++, name, qty, price, ck, tt, ton);
                total += tt;
            }
            AddDetailFooter(y, items.Length, total);
            lblDetailTitle.Text = $"Thông tin hàng hóa  {items.Length}";
        }

        private void AddDetailCard(ref int y, int num, string name, int qty, decimal price, decimal ck, decimal tt, int ton)
        {
            var pnl = new Panel { Location = new Point(8, y), Size = new Size(295, 105), BackColor = Color.FromArgb(248, 249, 252) };
            pnl.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(210, 215, 225));
                e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, pnl.Width - 1, pnl.Height - 1));
            };
            pnl.Controls.Add(new Label { Text = $"#{num}", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(26,115,232), Location = new Point(8, 8), AutoSize = true });
            pnl.Controls.Add(new Label { Text = name, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(32,40,62), Location = new Point(8, 26), Size = new Size(275, 18) });
            pnl.Controls.Add(new Label { Text = $"{qty} Cái x {price:N0} = {qty * price:N0}", Font = new Font("Segoe UI", 8), ForeColor = Color.Gray, Location = new Point(8, 46), AutoSize = true });
            pnl.Controls.Add(new Label { Text = $"- {qty * price * ck / 100:N0}  {ck}%  + {tt * 0.1m:N0}  VAT 10%", Font = new Font("Segoe UI", 8), ForeColor = Color.Gray, Location = new Point(8, 62), AutoSize = true });
            pnl.Controls.Add(new Label { Text = $"{tt:N0}", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(52,168,83), Location = new Point(8, 80), AutoSize = true });
            pnl.Controls.Add(new Label { Text = $"📦 {ton} (KHO TP.HCM)", Font = new Font("Segoe UI", 8), ForeColor = Color.Gray, Location = new Point(155, 80), AutoSize = true });
            pnlHangHoa.Controls.Add(pnl);
            y += 113;
        }

        private void AddDetailFooter(int y, int count, decimal total)
        {
            var pnlF = new Panel { Location = new Point(8, y), Size = new Size(295, 54), BackColor = Color.White };
            pnlF.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Color.FromArgb(200,205,215)), 0, 0, pnlF.Width, 0);
            pnlF.Controls.Add(new Label { Text = $"Số lượng: {count}", Font = new Font("Segoe UI", 8.5f), ForeColor = Color.Gray, Location = new Point(6, 8), AutoSize = true });
            pnlF.Controls.Add(new Label { Text = "Số lượng giao: " + count, Font = new Font("Segoe UI", 8.5f), ForeColor = Color.Gray, Location = new Point(130, 8), AutoSize = true });
            pnlF.Controls.Add(new Label { Text = "Tổng tiền  ▲", Font = new Font("Segoe UI", 9), Location = new Point(6, 28), AutoSize = true });
            pnlF.Controls.Add(new Label { Text = $"{total:N0}", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(52,168,83), Location = new Point(170, 28), AutoSize = true });
            pnlHangHoa.Controls.Add(pnlF);
        }

        private void OpenNewDonHang()
        {
            var dlg = new FrmEditDonHang(0);
            if (dlg.ShowDialog() == DialogResult.OK) LoadDonHang();
        }
    }
}
