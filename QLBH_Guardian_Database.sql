-- ============================================================
-- QLBH_Guardian - Script tạo CSDL
-- Chạy trong SQL Server Management Studio (SSMS)
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'QLBH_Guardian')
    CREATE DATABASE QLBH_Guardian;
GO

USE QLBH_Guardian;
GO

-- ========================
-- VaiTro
-- ========================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='VaiTro')
CREATE TABLE VaiTro (
    maVaiTro   INT IDENTITY(1,1) PRIMARY KEY,
    tenVaiTro  NVARCHAR(100) NOT NULL
);

-- ========================
-- TaiKhoan
-- ========================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TaiKhoan')
CREATE TABLE TaiKhoan (
    maNguoiDung INT IDENTITY(1,1) PRIMARY KEY,
    username    NVARCHAR(50)  NOT NULL UNIQUE,
    [password]  NVARCHAR(255) NOT NULL,
    maVaiTro    INT REFERENCES VaiTro(maVaiTro),
    trangThai   NVARCHAR(20)  DEFAULT N'HoatDong'
);

-- ========================
-- DanhMuc
-- ========================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DanhMuc')
CREATE TABLE DanhMuc (
    maDanhMuc  INT IDENTITY(1,1) PRIMARY KEY,
    tenDanhMuc NVARCHAR(150) NOT NULL
);

-- ========================
-- NhanVien
-- ========================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='NhanVien')
CREATE TABLE NhanVien (
    maNV    INT IDENTITY(1,1) PRIMARY KEY,
    hoTen   NVARCHAR(150) NOT NULL,
    chucVu  NVARCHAR(100),
    sdt     NVARCHAR(20),
    email   NVARCHAR(100)
);

-- ========================
-- Kho
-- ========================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Kho')
CREATE TABLE Kho (
    maKho    INT IDENTITY(1,1) PRIMARY KEY,
    tenKho   NVARCHAR(150) NOT NULL,
    diaChi   NVARCHAR(255),
    maThuKho INT REFERENCES NhanVien(maNV)
);

-- ========================
-- KhachHang
-- ========================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='KhachHang')
CREATE TABLE KhachHang (
    maKH       INT IDENTITY(1,1) PRIMARY KEY,
    hoTen      NVARCHAR(150) NOT NULL,
    email      NVARCHAR(100),
    sdt        NVARCHAR(20),
    diaChi     NVARCHAR(255),
    loaiKH     NVARCHAR(50)  DEFAULT N'Moi',
    diemThuong INT           DEFAULT 0
);

-- ========================
-- SanPham
-- ========================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SanPham')
CREATE TABLE SanPham (
    maSP       INT IDENTITY(1,1) PRIMARY KEY,
    tenSP      NVARCHAR(200) NOT NULL,
    thuongHieu NVARCHAR(100),
    giaBan     DECIMAL(18,2) DEFAULT 0,
    moTa       NVARCHAR(500),
    hanSuDung  DATE,
    maDanhMuc  INT REFERENCES DanhMuc(maDanhMuc)
);

-- ========================
-- TonKho
-- ========================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TonKho')
CREATE TABLE TonKho (
    maTonKho   INT IDENTITY(1,1) PRIMARY KEY,
    maKho      INT REFERENCES Kho(maKho),
    maSP       INT REFERENCES SanPham(maSP),
    soLuongTon INT DEFAULT 0
);

-- ========================
-- DonHang
-- ========================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DonHang')
CREATE TABLE DonHang (
    maDonHang   INT IDENTITY(1,1) PRIMARY KEY,
    soDonHang   NVARCHAR(50),
    maKH        INT REFERENCES KhachHang(maKH),
    maNV        INT REFERENCES NhanVien(maNV),
    ngayDatHang DATETIME      DEFAULT GETDATE(),
    ngayGiaoHang DATETIME,
    trangThai   NVARCHAR(30)  DEFAULT N'BanNhap',
    tongTien    DECIMAL(18,2) DEFAULT 0,
    dienGiai    NVARCHAR(500),
    ghiChu      NVARCHAR(500)
);

-- ========================
-- ChiTietDonHang
-- ========================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ChiTietDonHang')
CREATE TABLE ChiTietDonHang (
    maChiTiet  INT IDENTITY(1,1) PRIMARY KEY,
    maDonHang  INT REFERENCES DonHang(maDonHang),
    maSP       INT REFERENCES SanPham(maSP),
    soLuong    INT           DEFAULT 1,
    donGia     DECIMAL(18,2) DEFAULT 0,
    chietKhau  DECIMAL(5,2)  DEFAULT 0
);

-- ========================
-- PhieuNhapHang
-- ========================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PhieuNhapHang')
CREATE TABLE PhieuNhapHang (
    maPhieuNhap INT IDENTITY(1,1) PRIMARY KEY,
    maKho       INT REFERENCES Kho(maKho),
    ngayNhap    DATETIME DEFAULT GETDATE(),
    ghiChu      NVARCHAR(500)
);

-- ========================
-- ChiTietPhieuNhap
-- ========================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ChiTietPhieuNhap')
CREATE TABLE ChiTietPhieuNhap (
    maChiTiet   INT IDENTITY(1,1) PRIMARY KEY,
    maPhieuNhap INT REFERENCES PhieuNhapHang(maPhieuNhap),
    maSP        INT REFERENCES SanPham(maSP),
    soLuong     INT           DEFAULT 1,
    donGia      DECIMAL(18,2) DEFAULT 0
);

-- ========================
-- View báo cáo doanh thu
-- ========================
GO
IF OBJECT_ID('vw_DoanhThuNgay') IS NOT NULL DROP VIEW vw_DoanhThuNgay;
GO
CREATE VIEW vw_DoanhThuNgay AS
SELECT
    CAST(dh.ngayDatHang AS DATE) AS NgayBan,
    COUNT(dh.maDonHang) AS SoDonHang,
    SUM(dh.tongTien) AS DoanhThu
FROM DonHang dh
WHERE dh.trangThai NOT IN (N'DaHuy')
GROUP BY CAST(dh.ngayDatHang AS DATE);
GO

-- ========================
-- Seed data mẫu
-- ========================
SET IDENTITY_INSERT VaiTro ON;
INSERT INTO VaiTro (maVaiTro, tenVaiTro) VALUES
    (1, N'Chủ cửa hàng'),
    (2, N'Nhân viên bán hàng'),
    (3, N'Thủ kho');
SET IDENTITY_INSERT VaiTro OFF;

INSERT INTO TaiKhoan (username, [password], maVaiTro, trangThai) VALUES
    ('admin', '123456', 1, N'HoatDong'),
    ('nhanvien1', '123456', 2, N'HoatDong');

INSERT INTO DanhMuc (tenDanhMuc) VALUES
    (N'Điện tử'), (N'Thực phẩm'), (N'Dụng cụ'), (N'Thời trang');

INSERT INTO NhanVien (hoTen, chucVu, sdt) VALUES
    (N'Nguyễn Văn An', N'Quản lý', N'0912345678'),
    (N'Trần Thị Bình', N'Nhân viên bán hàng', N'0987654321');

INSERT INTO Kho (tenKho, diaChi) VALUES
    (N'Kho Hà Nội', N'123 Cầu Giấy, Hà Nội'),
    (N'Kho TP.HCM', N'456 Quận 1, TP.HCM');

INSERT INTO KhachHang (hoTen, email, sdt, diaChi, loaiKH) VALUES
    (N'Công ty TNHH Thân Thiện',        'contact@thanthien.vn',  N'024.1234567', N'Hà Nội', N'VIP'),
    (N'Công ty cổ phần Hoa Sen',         'info@hoasen.vn',        N'028.7654321', N'TP.HCM', N'VIP'),
    (N'Doanh nghiệp tư nhân Minh Long',  'minhlong@gmail.com',    N'0901234567',  N'Đà Nẵng', N'Moi');

INSERT INTO SanPham (tenSP, thuongHieu, giaBan, maDanhMuc) VALUES
    (N'Điện thoại LG G4',   N'LG',      8899000, 1),
    (N'Bơm ly tâm',         N'Ebara',   20000000, 3),
    (N'Bơm đa tầng',        N'Grundfos',10000000, 3),
    (N'Áo sơ mi Nam',       N'May 10',  350000,  4);

INSERT INTO TonKho (maKho, maSP, soLuongTon) VALUES
    (1, 1, 50), (1, 2, 5), (1, 3, 8), (2, 1, 30), (2, 4, 100);

INSERT INTO DonHang (soDonHang, maKH, ngayDatHang, trangThai, tongTien, dienGiai) VALUES
    (N'DH00001', 1, GETDATE()-5, N'BanNhap',   1250000,  N'Đơn hàng bán mới cho Công ty TNHH Thân Thiện'),
    (N'DH00002', 2, GETDATE()-4, N'DeNghiGhi', 5000000,  N'Đơn hàng bán mới cho Công ty cổ phần Hoa Sen'),
    (N'DH00003', 3, GETDATE()-3, N'DaGhi',     2250000,  N'Đơn hàng bán mới cho Doanh nghiệp tư nhân Minh Long'),
    (N'DH00004', 1, GETDATE()-2, N'HoanThanh', 26697000, N'Đơn hàng bán mới cho Công ty TNHH Thân Thiện');

INSERT INTO ChiTietDonHang (maDonHang, maSP, soLuong, donGia, chietKhau) VALUES
    (1, 1, 1, 8899000, 5),
    (4, 2, 1, 20000000, 10),
    (4, 3, 1, 10000000, 10);

PRINT 'Database QLBH_Guardian đã được tạo và seed data thành công!';
