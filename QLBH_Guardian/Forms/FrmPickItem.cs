using System.Data;

namespace QLBH_Guardian.Forms
{
    /// <summary>
    /// Generic lookup/pick dialog - used for selecting Customers, Products, etc.
    /// </summary>
    public class FrmPickItem : Form
    {
        private DataGridView grid = null!;
        private TextBox txtSearch = null!;
        private DataTable _dt;
        private string[] _columns;
        private string[] _headers;
        public DataRow? SelectedRow { get; private set; }

        public FrmPickItem(string title, DataTable dt, string[] columns, string[] headers)
        {
            _dt = dt;
            _columns = columns;
            _headers = headers;
            InitializeComponent(title);
        }

        private void InitializeComponent(string title)
        {
            this.Text = title;
            this.Size = new Size(680, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(240, 242, 245);

            // Header
            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(26, 115, 232) };
            pnlTop.Controls.Add(new Label { Text = $"  🔍 {title}", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft });

            // Search bar
            var pnlSearch = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Color.White, Padding = new Padding(10, 8, 10, 8) };
            txtSearch = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), PlaceholderText = "🔍 Tìm kiếm..." };
            txtSearch.TextChanged += (s, e) => FilterGrid();
            pnlSearch.Controls.Add(txtSearch);

            // Grid
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 9),
                ColumnHeadersHeight = 36,
                MultiSelect = false
            };
            FrmMain.StyleGrid(grid, Color.FromArgb(26, 115, 232));
            grid.CellDoubleClick += (s, e) => SelectAndClose();

            // Build view with only specified columns
            var view = new DataView(_dt);
            var displayDt = _dt.DefaultView.ToTable(false, _columns);
            for (int i = 0; i < _columns.Length && i < _headers.Length; i++)
                displayDt.Columns[_columns[i]].Caption = _headers[i];

            grid.DataSource = displayDt;
            for (int i = 0; i < grid.Columns.Count && i < _headers.Length; i++)
                grid.Columns[i].HeaderText = _headers[i];

            // Footer buttons
            var pnlBtn = new Panel { Dock = DockStyle.Bottom, Height = 44, BackColor = Color.White };
            pnlBtn.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Color.FromArgb(220, 220, 220)), 0, 0, pnlBtn.Width, 0);

            var btnChon = new Button { Text = "✔ Chọn", Location = new Point(480, 8), Size = new Size(90, 28), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(26, 115, 232), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Right | AnchorStyles.Top };
            btnChon.FlatAppearance.BorderSize = 0;
            btnChon.Click += (s, e) => SelectAndClose();

            var btnHuy = new Button { Text = "✖ Hủy", Location = new Point(578, 8), Size = new Size(90, 28), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(220, 53, 69), ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Cursor = Cursors.Hand, Anchor = AnchorStyles.Right | AnchorStyles.Top };
            btnHuy.FlatAppearance.BorderSize = 0;
            btnHuy.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            pnlBtn.Controls.AddRange(new Control[] { btnChon, btnHuy });

            this.Controls.Add(grid);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlTop);
            this.Controls.Add(pnlBtn);
        }

        private void FilterGrid()
        {
            if (grid.DataSource is DataTable dt)
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    dt.DefaultView.RowFilter = "";
                    return;
                }
                var parts = _columns.Select(c => $"CONVERT({c}, System.String) LIKE '%{txtSearch.Text}%'");
                dt.DefaultView.RowFilter = string.Join(" OR ", parts);
            }
        }

        private void SelectAndClose()
        {
            if (grid.SelectedRows.Count == 0) return;

            // Map selected display row back to original DataTable
            var selectedKey = grid.SelectedRows[0].Cells[0].Value;
            foreach (DataRow row in _dt.Rows)
            {
                if (row[_columns[0]].ToString() == selectedKey?.ToString())
                {
                    SelectedRow = row;
                    break;
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
