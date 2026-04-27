namespace QLBH_Guardian.Models
{
    public class DonHang
    {
        public int MaDonHang { get; set; }
        public string SoDonHang { get; set; } = "";
        public int MaKH { get; set; }
        public string TenKhachHang { get; set; } = "";
        public int? MaNV { get; set; }
        public DateTime NgayDatHang { get; set; }
        public DateTime? NgayGiaoHang { get; set; }
        public string TrangThai { get; set; } = "BanNhap";
        public decimal TongTien { get; set; }
        public string DienGiai { get; set; } = "";
        public string GhiChu { get; set; } = "";
    }

    public class ChiTietDonHang
    {
        public int MaChiTiet { get; set; }
        public int MaDonHang { get; set; }
        public int MaSP { get; set; }
        public string TenSP { get; set; } = "";
        public string DonViTinh { get; set; } = "";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ChietKhau { get; set; }
        public decimal ThanhTien => SoLuong * DonGia * (1 - ChietKhau / 100);
    }

    public class SanPham
    {
        public int MaSP { get; set; }
        public string TenSP { get; set; } = "";
        public string ThuongHieu { get; set; } = "";
        public decimal GiaBan { get; set; }
        public string MoTa { get; set; } = "";
        public DateTime? HanSuDung { get; set; }
        public int MaDanhMuc { get; set; }
        public string TenDanhMuc { get; set; } = "";
    }

    public class KhachHang
    {
        public int MaKH { get; set; }
        public string HoTen { get; set; } = "";
        public string Email { get; set; } = "";
        public string Sdt { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public string LoaiKH { get; set; } = "Moi";
        public int DiemThuong { get; set; }
    }

    public class NhanVien
    {
        public int MaNV { get; set; }
        public string HoTen { get; set; } = "";
        public string ChucVu { get; set; } = "";
        public string Sdt { get; set; } = "";
        public string Email { get; set; } = "";
    }

    public class TonKho
    {
        public int MaTonKho { get; set; }
        public int MaKho { get; set; }
        public string TenKho { get; set; } = "";
        public int MaSP { get; set; }
        public string MaHang { get; set; } = "";
        public string TenHang { get; set; } = "";
        public string DonViTinh { get; set; } = "";
        public int SoLuongDauKy { get; set; }
        public decimal GiaTriDauKy { get; set; }
        public int SoLuongNhap { get; set; }
        public decimal GiaTriNhap { get; set; }
        public int SoLuongXuat { get; set; }
        public decimal GiaTriXuat { get; set; }
        public int SoLuongCuoiKy => SoLuongDauKy + SoLuongNhap - SoLuongXuat;
        public decimal GiaTriCuoiKy => GiaTriDauKy + GiaTriNhap - GiaTriXuat;
    }

    public class Kho
    {
        public int MaKho { get; set; }
        public string TenKho { get; set; } = "";
        public string DiaChi { get; set; } = "";
    }

    public class HoaDon
    {
        public int MaHoaDon { get; set; }
        public int MaDonHang { get; set; }
        public string SoHoaDon { get; set; } = "";
        public DateTime NgayLap { get; set; }
        public decimal TongTien { get; set; }
        public decimal TienThue { get; set; }
        public decimal TienChietKhau { get; set; }
        public decimal TongThanhToan => TongTien + TienThue - TienChietKhau;
    }
}
