using QLBH_Guardian.DataAccess;
using System.Data;

namespace QLBH_Guardian.Forms
{
    public class FrmEditDonHang : Form
    {
        private int _maDonHang;
        private TextBox txtMaKH = null!, txtTenKH = null!, txtDienGiai = null!;
        private DateTimePicker dtpNgay = null!;
        private DataGridView gridChiTiet = null!;
        private DataTable _dtChiTiet = null!;
        private Label lblTongTien = null!;
        public DataRow? SelectedRow { get; private set; }

        public FrmEditDonHang(int maDonHang)
        {
            _maDonHang = maDonHang;
            InitializeComponent();
            if (maDonHang > 0) LoadDonHang();
        }

        private void InitializeComponent()
        {
            this.Text = _maDonHang == 0 ? "Thêm đơn hàng mới" : "Sửa đơn hàng";
            this.Size = new Size(780, 580);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 242, 245);

            // Header
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(26, 115, 232) };
            pnlTop.Controls.Add(new Label { Text = _maDonHang == 0 ? "  ＋ Thêm đơn hàng mới" : "  ✎ Sửa đơn hàng", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft });
            this.Controls.Add(pnlTop);

            // Form fields
            var pnlForm = new Panel { Location = new Point(10, 60), Size = new Size(750, 130), BackColor = Color.White };
            pnlForm.Paint += (s, e) =>
            {
                var r = new Rectangle(0, 0, pnlForm.Width - 1, pnlForm.Height - 1);
                using var pen = new Pen(Color.FromArgb(220, 220, 220));
                e.Graphics.DrawRectangle(pen, r);
            };

            // Row 1
            pnlForm.Controls.Add(new Label { Text = "Khách hàng:", Location = new Point(15, 20), AutoSize = true, Font = new Font("Segoe UI", 9) });
            txtMaKH = new TextBox { Location = new Point(110, 16), Size = new Size(80, 24), Font = new Font("Segoe UI", 9), PlaceholderText = "Mã KH" };
            var btnPickKH = new Button { Text = "▼", Location = new Point(193, 16), Size = new Size(26, 24), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(26, 115, 232), ForeColor = Color.White };
            btnPickKH.FlatAppearance.BorderSize = 0;
            btnPickKH.Click += (s, e) => PickKhachHang();
            txtTenKH = new TextBox { Location = new Point(225, 16), Size = new Size(300, 24), Font = new Font("Segoe UI", 9) };
            pnlForm.Controls.AddRange(new Control[] { txtMaKH, btnPickKH, txtTenKH });

            pnlForm.Controls.Add(new Label { Text = "Ngày:", Location = new Point(540, 20), AutoSize = true, Font = new Font("Segoe UI", 9) });
            dtpNgay = new DateTimePicker { Location = new Point(575, 16), Size = new Size(155, 24), Font = new Font("Segoe UI", 9), Format = DateTimePickerFormat.Short };
            pnlForm.Controls.Add(dtpNgay);

            // Row 2
            pnlForm.Controls.Add(new Label { Text = "Diễn giải:", Location = new Point(15, 58), AutoSize = true, Font = new Font("Segoe UI", 9) });
            txtDienGiai = new TextBox { Location = new Point(110, 54), Size = new Size(620, 24), Font = new Font("Segoe UI", 9) };
            pnlForm.Controls.Add(txtDienGiai);

            // Summary row
            pnlForm.Controls.Add(new Label { Text = "Tổng tiền:", Location = new Point(15, 96), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            lblTongTien = new Label { Text = "0 VNĐ", Location = new Point(110, 96), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(220, 53, 69) };
            pnlForm.Controls.Add(lblTongTien);

            this.Controls.Add(pnlForm);

            // Grid
            _dtChiTiet = new DataTable();
            _dtChiTiet.Columns.AddRange(new[]
            {
                new DataColumn("MaSP",      typeof(int)),
                new DataColumn("TenSP",     typeof(string)),
                new DataColumn("SoLuong",   typeof(decimal)),
                new DataColumn("DonGia",    typeof(decimal)),
                new DataColumn("ChietKhau", typeof(decimal)),
                new DataColumn("ThanhTien", typeof(decimal))
            });

            gridChiTiet = new DataGridView
            {
                Location = new Point(10, 200),
                Size = new Size(750, 280),
                DataSource = _dtChiTiet,
                AllowUserToAddRows = true,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 32,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            FrmMain.StyleGrid(gridChiTiet, Color.FromArgb(26, 115, 232));

            var colDefs = new[] { ("MaSP","Mã SP",70),("TenSP","Tên SP",220),("SoLuong","Số lượng",80),("DonGia","Đơn giá",110),("ChietKhau","CK%",60),("ThanhTien","Thành tiền",120) };
            foreach (var (col, hdr, w) in colDefs)
                if (gridChiTiet.Columns.Contains(col)) { gridChiTiet.Columns[col].HeaderText = hdr; gridChiTiet.Columns[col].Width = w; }

            gridChiTiet.CellEndEdit += (s, e) =>
            {
                _dtChiTiet.AcceptChanges();
                decimal total = _dtChiTiet.Rows.Cast<DataRow>()
                    .Sum(r => r["ThanhTien"] != DBNull.Value ? Convert.ToDecimal(r["ThanhTien"]) : 0);
                lblTongTien.Text = total.ToString("N0") + " VNĐ";
            };

            this.Controls.Add(gridChiTiet);

            // Buttons
            var btnLuu = new Button { Text = "💾 Lưu", Location = new Point(530, 490), Size = new Size(100, 34), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(26, 115, 232), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            btnLuu.FlatAppearance.BorderSize = 0;
            btnLuu.Click += BtnLuu_Click;

            var btnHuy = new Button { Text = "✖ Hủy", Location = new Point(640, 490), Size = new Size(100, 34), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };
            btnHuy.FlatAppearance.BorderSize = 0;
            btnHuy.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { btnLuu, btnHuy });
        }

        private void LoadDonHang()
        {
            try
            {
                var sql = "SELECT dh.*, kh.hoTen FROM DonHang dh LEFT JOIN KhachHang kh ON kh.maKH=dh.maKH WHERE dh.maDonHang=@id";
                var dt = DatabaseHelper.ExecuteQuery(sql, new() { {"@id",_maDonHang} });
                if (dt.Rows.Count > 0)
                {
                    var row = dt.Rows[0];
                    txtMaKH.Text = row["maKH"].ToString();
                    txtTenKH.Text = row["hoTen"].ToString() ?? "";
                    txtDienGiai.Text = row["dienGiai"].ToString() ?? "";
                    if (row["ngayDatHang"] != DBNull.Value) dtpNgay.Value = Convert.ToDateTime(row["ngayDatHang"]);
                }
            }
            catch { }
        }

        private void PickKhachHang()
        {
            try
            {
                var dt = DatabaseHelper.ExecuteQuery("SELECT maKH, hoTen, sdt, diaChi FROM KhachHang ORDER BY hoTen");
                var frm = new FrmPickItem("Chọn khách hàng", dt, new[] { "maKH","hoTen","sdt","diaChi" }, new[] { "Mã KH","Họ tên","SĐT","Địa chỉ" });
                if (frm.ShowDialog() == DialogResult.OK && frm.SelectedRow != null)
                {
                    txtMaKH.Text = frm.SelectedRow["maKH"].ToString();
                    txtTenKH.Text = frm.SelectedRow["hoTen"].ToString() ?? "";
                }
            }
            catch { MessageBox.Show("Không thể kết nối CSDL.", "Cảnh báo"); }
        }

        private void BtnLuu_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaKH.Text)) { MessageBox.Show("Vui lòng chọn khách hàng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            try
            {
                if (!int.TryParse(txtMaKH.Text, out int maKH)) maKH = 1;
                decimal tongTien = _dtChiTiet.Rows.Cast<DataRow>().Sum(r => r["ThanhTien"] != DBNull.Value ? Convert.ToDecimal(r["ThanhTien"]) : 0);

                if (_maDonHang == 0)
                    DatabaseHelper.ExecuteNonQuery(
                        "INSERT INTO DonHang (maKH, ngayDatHang, trangThai, tongTien, dienGiai) VALUES (@maKH,@ngay,N'BanNhap',@tt,@dg)",
                        new() { {"@maKH",maKH},{"@ngay",dtpNgay.Value},{"@tt",tongTien},{"@dg",txtDienGiai.Text} });
                else
                    DatabaseHelper.ExecuteNonQuery(
                        "UPDATE DonHang SET maKH=@maKH, ngayDatHang=@ngay, tongTien=@tt, dienGiai=@dg WHERE maDonHang=@id",
                        new() { {"@maKH",maKH},{"@ngay",dtpNgay.Value},{"@tt",tongTien},{"@dg",txtDienGiai.Text},{"@id",_maDonHang} });

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
