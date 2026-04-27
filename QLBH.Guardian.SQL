    CREATE DATABASE QLBH_Guardian
    COLLATE Vietnamese_CI_AS;
END
GO

USE QLBH_Guardian;
GO

--  XÓA CÁC BẢNG CŨ (theo thứ tự phụ thuộc khóa ngoại)
IF OBJECT_ID('ChiTietPhieuNhap',  'U') IS NOT NULL DROP TABLE ChiTietPhieuNhap;
IF OBJECT_ID('PhieuNhapHang',     'U') IS NOT NULL DROP TABLE PhieuNhapHang;
IF OBJECT_ID('KiemKe',            'U') IS NOT NULL DROP TABLE KiemKe;
IF OBJECT_ID('TonKho',            'U') IS NOT NULL DROP TABLE TonKho;
IF OBJECT_ID('Kho',               'U') IS NOT NULL DROP TABLE Kho;
IF OBJECT_ID('ChiTietDonHang',    'U') IS NOT NULL DROP TABLE ChiTietDonHang;
IF OBJECT_ID('ChiTietGioHang',    'U') IS NOT NULL DROP TABLE ChiTietGioHang;
IF OBJECT_ID('ThanhToanOnline',   'U') IS NOT NULL DROP TABLE ThanhToanOnline;
IF OBJECT_ID('ThanhToanTienMat',  'U') IS NOT NULL DROP TABLE ThanhToanTienMat;
IF OBJECT_ID('ThanhToan',         'U') IS NOT NULL DROP TABLE ThanhToan;
IF OBJECT_ID('HoaDon',            'U') IS NOT NULL DROP TABLE HoaDon;
IF OBJECT_ID('DonHang',           'U') IS NOT NULL DROP TABLE DonHang;
IF OBJECT_ID('GioHang',           'U') IS NOT NULL DROP TABLE GioHang;
IF OBJECT_ID('BaoCao',            'U') IS NOT NULL DROP TABLE BaoCao;
IF OBJECT_ID('SanPham',           'U') IS NOT NULL DROP TABLE SanPham;
IF OBJECT_ID('DanhMuc',           'U') IS NOT NULL DROP TABLE DanhMuc;
IF OBJECT_ID('KhachHang',         'U') IS NOT NULL DROP TABLE KhachHang;
IF OBJECT_ID('NhanVien',          'U') IS NOT NULL DROP TABLE NhanVien;
IF OBJECT_ID('TaiKhoan',          'U') IS NOT NULL DROP TABLE TaiKhoan;
IF OBJECT_ID('VaiTro',            'U') IS NOT NULL DROP TABLE VaiTro;
GO

--  1. VAI TRÒ (VaiTro)
--     Phân quyền: Chủ cửa hàng, Nhân viên bán hàng,
--                 Thủ kho, Nhân viên văn phòng, QLHT, Khách hàng
CREATE TABLE VaiTro (
    maVaiTro    INT             NOT NULL IDENTITY(1,1),
    tenVaiTro   NVARCHAR(100)   NOT NULL,
    moTa        NVARCHAR(255)   NULL,
    CONSTRAINT PK_VaiTro PRIMARY KEY (maVaiTro),
    CONSTRAINT UQ_VaiTro_Ten UNIQUE (tenVaiTro)
);
GO

--  2. TÀI KHOẢN (TaiKhoan)
CREATE TABLE TaiKhoan (
    maNguoiDung INT             NOT NULL IDENTITY(1,1),
    username    NVARCHAR(50)    NOT NULL,
    [password]  NVARCHAR(255)   NOT NULL,   -- lưu hash
    maVaiTro    INT             NOT NULL,
    trangThai   NVARCHAR(20)    NOT NULL
        CONSTRAINT DF_TaiKhoan_TrangThai DEFAULT N'HoatDong',
    CONSTRAINT PK_TaiKhoan       PRIMARY KEY (maNguoiDung),
    CONSTRAINT UQ_TaiKhoan_User  UNIQUE (username),
    CONSTRAINT FK_TaiKhoan_VaiTro
        FOREIGN KEY (maVaiTro) REFERENCES VaiTro(maVaiTro),
    CONSTRAINT CK_TaiKhoan_TrangThai
        CHECK (trangThai IN (N'HoatDong', N'KhoaTam', N'DaXoa'))
);
GO
--  3. KHÁCH HÀNG (KhachHang)
--     Liên kết 1-1 với TaiKhoan (maNguoiDung)
CREATE TABLE KhachHang (
    maKH        INT             NOT NULL IDENTITY(1,1),
    hoTen       NVARCHAR(150)   NOT NULL,
    email       NVARCHAR(150)   NOT NULL,
    sdt         VARCHAR(15)     NULL,
    diaChi      NVARCHAR(255)   NULL,
    loaiKH      NVARCHAR(30)    NOT NULL
        CONSTRAINT DF_KhachHang_LoaiKH DEFAULT N'Moi',
    diemThuong  INT             NOT NULL
        CONSTRAINT DF_KhachHang_DiemThuong DEFAULT 0,
    maNguoiDung INT             NULL,   -- NULL nếu khách vãng lai
    CONSTRAINT PK_KhachHang        PRIMARY KEY (maKH),
    CONSTRAINT UQ_KhachHang_Email  UNIQUE (email),
    CONSTRAINT FK_KhachHang_TK
        FOREIGN KEY (maNguoiDung) REFERENCES TaiKhoan(maNguoiDung),
    CONSTRAINT CK_KhachHang_LoaiKH
        CHECK (loaiKH IN (N'Moi', N'ThanThiet', N'VIP')),
    CONSTRAINT CK_KhachHang_Diem
        CHECK (diemThuong >= 0)
);
GO

--  4. NHÂN VIÊN (NhanVien)
--     Bao gồm: NV bán hàng, Thủ kho, NV văn phòng,
--              Chủ cửa hàng, QLHT
CREATE TABLE NhanVien (
    maNV        INT             NOT NULL IDENTITY(1,1),
    hoTen       NVARCHAR(150)   NOT NULL,
    chucVu      NVARCHAR(100)   NOT NULL,
    caDangLam   NVARCHAR(50)    NULL,
    sdt         VARCHAR(15)     NULL,
    email       NVARCHAR(150)   NULL,
    maNguoiDung INT             NOT NULL,
    CONSTRAINT PK_NhanVien PRIMARY KEY (maNV),
    CONSTRAINT FK_NhanVien_TK
        FOREIGN KEY (maNguoiDung) REFERENCES TaiKhoan(maNguoiDung),
    CONSTRAINT CK_NhanVien_ChucVu
        CHECK (chucVu IN (
            N'NhanVienBanHang', N'ThuKho',
            N'NhanVienVanPhong', N'ChuCuaHang',
            N'QuanLyHeThong'
        ))
);
GO

--  5. DANH MỤC SẢN PHẨM (DanhMuc)
CREATE TABLE DanhMuc (
    maDanhMuc   INT             NOT NULL IDENTITY(1,1),
    tenDanhMuc  NVARCHAR(100)   NOT NULL,
    moTa        NVARCHAR(255)   NULL,
    CONSTRAINT PK_DanhMuc      PRIMARY KEY (maDanhMuc),
    CONSTRAINT UQ_DanhMuc_Ten  UNIQUE (tenDanhMuc)
);
GO
--  6. SẢN PHẨM (SanPham)
CREATE TABLE SanPham (
    maSP        INT             NOT NULL IDENTITY(1,1),
    tenSP       NVARCHAR(200)   NOT NULL,
    thuongHieu  NVARCHAR(100)   NULL,
    giaBan      DECIMAL(18,2)   NOT NULL,
    moTa        NVARCHAR(500)   NULL,
    hanSuDung   DATE            NULL,
    maDanhMuc   INT             NOT NULL,
    CONSTRAINT PK_SanPham PRIMARY KEY (maSP),
    CONSTRAINT FK_SanPham_DanhMuc
        FOREIGN KEY (maDanhMuc) REFERENCES DanhMuc(maDanhMuc),
    CONSTRAINT CK_SanPham_GiaBan
        CHECK (giaBan > 0)
);
GO
--  7. KHO (Kho)
CREATE TABLE Kho (
    maKho       INT             NOT NULL IDENTITY(1,1),
    tenKho      NVARCHAR(100)   NOT NULL,
    diaChi      NVARCHAR(255)   NULL,
    maThuKho    INT             NULL,   -- FK → NhanVien
    CONSTRAINT PK_Kho PRIMARY KEY (maKho),
    CONSTRAINT FK_Kho_NhanVien
        FOREIGN KEY (maThuKho) REFERENCES NhanVien(maNV)
);
GO
--  8. TỒN KHO (TonKho)
CREATE TABLE TonKho (
    maTonKho        INT         NOT NULL IDENTITY(1,1),
    maKho           INT         NOT NULL,
    maSP            INT         NOT NULL,
    soLuongTon      INT         NOT NULL
        CONSTRAINT DF_TonKho_SoLuong DEFAULT 0,
    ngayCapNhat     DATETIME    NOT NULL
        CONSTRAINT DF_TonKho_NgayCapNhat DEFAULT GETDATE(),
    CONSTRAINT PK_TonKho PRIMARY KEY (maTonKho),
    CONSTRAINT UQ_TonKho_KhoSP UNIQUE (maKho, maSP),
    CONSTRAINT FK_TonKho_Kho
        FOREIGN KEY (maKho) REFERENCES Kho(maKho),
    CONSTRAINT FK_TonKho_SanPham
        FOREIGN KEY (maSP) REFERENCES SanPham(maSP),
    CONSTRAINT CK_TonKho_SoLuong
        CHECK (soLuongTon >= 0)
);
GO
--  9. KIỂM KÊ (KiemKe)
CREATE TABLE KiemKe (
    maKiemKe    INT             NOT NULL IDENTITY(1,1),
    maKho       INT             NOT NULL,
    ngayKiemKe  DATE            NOT NULL,
    maNhanVien  INT             NOT NULL,   -- Thủ kho thực hiện
    ghiChu      NVARCHAR(500)   NULL,
    CONSTRAINT PK_KiemKe PRIMARY KEY (maKiemKe),
    CONSTRAINT FK_KiemKe_Kho
        FOREIGN KEY (maKho) REFERENCES Kho(maKho),
    CONSTRAINT FK_KiemKe_NhanVien
        FOREIGN KEY (maNhanVien) REFERENCES NhanVien(maNV)
);
GO
-- 10. PHIẾU NHẬP HÀNG (PhieuNhapHang)
CREATE TABLE PhieuNhapHang (
    maPhieuNhap INT             NOT NULL IDENTITY(1,1),
    ngayNhap    DATETIME        NOT NULL
        CONSTRAINT DF_PhieuNhap_NgayNhap DEFAULT GETDATE(),
    maKho       INT             NOT NULL,
    maThuKho    INT             NOT NULL,
    tongTien    DECIMAL(18,2)   NOT NULL
        CONSTRAINT DF_PhieuNhap_TongTien DEFAULT 0,
    CONSTRAINT PK_PhieuNhapHang PRIMARY KEY (maPhieuNhap),
    CONSTRAINT FK_PhieuNhap_Kho
        FOREIGN KEY (maKho) REFERENCES Kho(maKho),
    CONSTRAINT FK_PhieuNhap_NhanVien
        FOREIGN KEY (maThuKho) REFERENCES NhanVien(maNV),
    CONSTRAINT CK_PhieuNhap_TongTien
        CHECK (tongTien >= 0)
);
GO
-- 11. CHI TIẾT PHIẾU NHẬP (ChiTietPhieuNhap)
CREATE TABLE ChiTietPhieuNhap (
    maChiTietPN INT             NOT NULL IDENTITY(1,1),
    maPhieuNhap INT             NOT NULL,
    maSP        INT             NOT NULL,
    soLuong     INT             NOT NULL,
    donGia      DECIMAL(18,2)   NOT NULL,
    CONSTRAINT PK_ChiTietPhieuNhap PRIMARY KEY (maChiTietPN),
    CONSTRAINT UQ_ChiTietPN UNIQUE (maPhieuNhap, maSP),
    CONSTRAINT FK_ChiTietPN_Phieu
        FOREIGN KEY (maPhieuNhap) REFERENCES PhieuNhapHang(maPhieuNhap),
    CONSTRAINT FK_ChiTietPN_SanPham
        FOREIGN KEY (maSP) REFERENCES SanPham(maSP),
    CONSTRAINT CK_ChiTietPN_SoLuong
        CHECK (soLuong > 0),
    CONSTRAINT CK_ChiTietPN_DonGia
        CHECK (donGia > 0)
);
GO
-- 12. GIỎ HÀNG (GioHang)
CREATE TABLE GioHang (
    maGioHang   INT             NOT NULL IDENTITY(1,1),
    maKH        INT             NOT NULL,
    ngayTao     DATETIME        NOT NULL
        CONSTRAINT DF_GioHang_NgayTao DEFAULT GETDATE(),
    trangThai   NVARCHAR(20)    NOT NULL
        CONSTRAINT DF_GioHang_TrangThai DEFAULT N'DangMua',
    CONSTRAINT PK_GioHang PRIMARY KEY (maGioHang),
    CONSTRAINT FK_GioHang_KhachHang
        FOREIGN KEY (maKH) REFERENCES KhachHang(maKH),
    CONSTRAINT CK_GioHang_TrangThai
        CHECK (trangThai IN (N'DangMua', N'DaDat', N'Huy'))
);
GO
-- 13. CHI TIẾT GIỎ HÀNG (ChiTietGioHang)
CREATE TABLE ChiTietGioHang (
    maChiTietGH INT             NOT NULL IDENTITY(1,1),
    maGioHang   INT             NOT NULL,
    maSP        INT             NOT NULL,
    soLuong     INT             NOT NULL,
    donGia      DECIMAL(18,2)   NOT NULL,
    CONSTRAINT PK_ChiTietGioHang PRIMARY KEY (maChiTietGH),
    CONSTRAINT UQ_ChiTietGH UNIQUE (maGioHang, maSP),
    CONSTRAINT FK_ChiTietGH_GioHang
        FOREIGN KEY (maGioHang) REFERENCES GioHang(maGioHang),
    CONSTRAINT FK_ChiTietGH_SanPham
        FOREIGN KEY (maSP) REFERENCES SanPham(maSP),
    CONSTRAINT CK_ChiTietGH_SoLuong
        CHECK (soLuong > 0),
    CONSTRAINT CK_ChiTietGH_DonGia
        CHECK (donGia > 0)
);
GO
-- 14. ĐƠN HÀNG (DonHang)
CREATE TABLE DonHang (
    maDonHang   INT             NOT NULL IDENTITY(1,1),
    maKH        INT             NOT NULL,
    maNV        INT             NULL,   -- NV xác nhận
    ngayDat     DATETIME        NOT NULL
        CONSTRAINT DF_DonHang_NgayDat DEFAULT GETDATE(),
    tongTien    DECIMAL(18,2)   NOT NULL,
    trangThai   NVARCHAR(30)    NOT NULL
        CONSTRAINT DF_DonHang_TrangThai DEFAULT N'ChoDuyet',
    maGioHang   INT             NULL,
    CONSTRAINT PK_DonHang PRIMARY KEY (maDonHang),
    CONSTRAINT FK_DonHang_KhachHang
        FOREIGN KEY (maKH) REFERENCES KhachHang(maKH),
    CONSTRAINT FK_DonHang_NhanVien
        FOREIGN KEY (maNV) REFERENCES NhanVien(maNV),
    CONSTRAINT FK_DonHang_GioHang
        FOREIGN KEY (maGioHang) REFERENCES GioHang(maGioHang),
    CONSTRAINT CK_DonHang_TrangThai
        CHECK (trangThai IN (
            N'ChoDuyet', N'DaXacNhan', N'DangGiao',
            N'DaGiao', N'DaHuy', N'HoanThanh'
        )),
    CONSTRAINT CK_DonHang_TongTien
        CHECK (tongTien >= 0)
);
GO
-- 15. CHI TIẾT ĐƠN HÀNG (ChiTietDonHang)
CREATE TABLE ChiTietDonHang (
    maChiTietDH INT             NOT NULL IDENTITY(1,1),
    maDonHang   INT             NOT NULL,
    maSP        INT             NOT NULL,
    soLuong     INT             NOT NULL,
    donGia      DECIMAL(18,2)   NOT NULL,
    CONSTRAINT PK_ChiTietDonHang PRIMARY KEY (maChiTietDH),
    CONSTRAINT UQ_ChiTietDH UNIQUE (maDonHang, maSP),
    CONSTRAINT FK_ChiTietDH_DonHang
        FOREIGN KEY (maDonHang) REFERENCES DonHang(maDonHang),
    CONSTRAINT FK_ChiTietDH_SanPham
        FOREIGN KEY (maSP) REFERENCES SanPham(maSP),
    CONSTRAINT CK_ChiTietDH_SoLuong
        CHECK (soLuong > 0),
    CONSTRAINT CK_ChiTietDH_DonGia
        CHECK (donGia > 0)
);
GO
-- 16. HÓA ĐƠN (HoaDon)
CREATE TABLE HoaDon (
    maHoaDon    INT             NOT NULL IDENTITY(1,1),
    maDonHang   INT             NOT NULL,
    maNV        INT             NOT NULL,   -- NV lập hóa đơn
    ngayLap     DATETIME        NOT NULL
        CONSTRAINT DF_HoaDon_NgayLap DEFAULT GETDATE(),
    tongTien    DECIMAL(18,2)   NOT NULL,
    trangThai   NVARCHAR(20)    NOT NULL
        CONSTRAINT DF_HoaDon_TrangThai DEFAULT N'DaLap',
    CONSTRAINT PK_HoaDon PRIMARY KEY (maHoaDon),
    CONSTRAINT UQ_HoaDon_DonHang UNIQUE (maDonHang),
    CONSTRAINT FK_HoaDon_DonHang
        FOREIGN KEY (maDonHang) REFERENCES DonHang(maDonHang),
    CONSTRAINT FK_HoaDon_NhanVien
        FOREIGN KEY (maNV) REFERENCES NhanVien(maNV),
    CONSTRAINT CK_HoaDon_TrangThai
        CHECK (trangThai IN (N'DaLap', N'DaIn', N'HuyBo')),
    CONSTRAINT CK_HoaDon_TongTien
        CHECK (tongTien >= 0)
);
GO
-- 17. THANH TOÁN (ThanhToan) - lớp cha
CREATE TABLE ThanhToan (
    maThanhToan INT             NOT NULL IDENTITY(1,1),
    maDonHang   INT             NOT NULL,
    soTien      DECIMAL(18,2)   NOT NULL,
    thoiGian    DATETIME        NOT NULL
        CONSTRAINT DF_ThanhToan_ThoiGian DEFAULT GETDATE(),
    trangThai   NVARCHAR(20)    NOT NULL
        CONSTRAINT DF_ThanhToan_TrangThai DEFAULT N'ChoPhuDuyet',
    loaiThanhToan NVARCHAR(20)  NOT NULL,
    CONSTRAINT PK_ThanhToan PRIMARY KEY (maThanhToan),
    CONSTRAINT FK_ThanhToan_DonHang
        FOREIGN KEY (maDonHang) REFERENCES DonHang(maDonHang),
    CONSTRAINT CK_ThanhToan_TrangThai
        CHECK (trangThai IN (
            N'ChoPhuDuyet', N'ThanhCong', N'ThatBai', N'Huy'
        )),
    CONSTRAINT CK_ThanhToan_Loai
        CHECK (loaiThanhToan IN (N'TienMat', N'Online')),
    CONSTRAINT CK_ThanhToan_SoTien
        CHECK (soTien > 0)
);
GO
-- 18. THANH TOÁN TIỀN MẶT (ThanhToanTienMat)
CREATE TABLE ThanhToanTienMat (
    maThanhToan INT             NOT NULL,
    soTienNhan  DECIMAL(18,2)   NOT NULL,
    soTienThoi  DECIMAL(18,2)   NOT NULL
        CONSTRAINT DF_TTM_SoTienThoi DEFAULT 0,
    CONSTRAINT PK_ThanhToanTM PRIMARY KEY (maThanhToan),
    CONSTRAINT FK_ThanhToanTM
        FOREIGN KEY (maThanhToan) REFERENCES ThanhToan(maThanhToan),
    CONSTRAINT CK_TTM_SoTienNhan
        CHECK (soTienNhan >= 0),
    CONSTRAINT CK_TTM_SoTienThoi
        CHECK (soTienThoi >= 0)
);
GO
-- 19. THANH TOÁN ONLINE (ThanhToanOnline)
CREATE TABLE ThanhToanOnline (
    maThanhToan INT             NOT NULL,
    nganHang    NVARCHAR(100)   NOT NULL,
    maNganHang  VARCHAR(50)     NOT NULL,   -- mã giao dịch ngân hàng
    maGiaoDich  VARCHAR(100)    NOT NULL,
    CONSTRAINT PK_ThanhToanOnline PRIMARY KEY (maThanhToan),
    CONSTRAINT FK_ThanhToanOnline
        FOREIGN KEY (maThanhToan) REFERENCES ThanhToan(maThanhToan),
    CONSTRAINT UQ_ThanhToanOnline_GD UNIQUE (maGiaoDich)
);
GO
-- 20. BÁO CÁO (BaoCao)
CREATE TABLE BaoCao (
    maBaoCao    INT             NOT NULL IDENTITY(1,1),
    loaiBaoCao  NVARCHAR(50)    NOT NULL,
    ngayLap     DATE            NOT NULL
        CONSTRAINT DF_BaoCao_NgayLap DEFAULT CAST(GETDATE() AS DATE),
    thoiGian    DATETIME        NOT NULL
        CONSTRAINT DF_BaoCao_ThoiGian DEFAULT GETDATE(),
    tongTien    DECIMAL(18,2)   NULL,
    maNhanVien  INT             NOT NULL,   -- NV lập báo cáo
    maKho       INT             NULL,       -- báo cáo kho (nếu có)
    CONSTRAINT PK_BaoCao PRIMARY KEY (maBaoCao),
    CONSTRAINT FK_BaoCao_NhanVien
        FOREIGN KEY (maNhanVien) REFERENCES NhanVien(maNV),
    CONSTRAINT FK_BaoCao_Kho
        FOREIGN KEY (maKho) REFERENCES Kho(maKho),
    CONSTRAINT CK_BaoCao_Loai
        CHECK (loaiBaoCao IN (
            N'DoanhThuNgay', N'DoanhThuThang', N'DoanhThuQuy',
            N'KiemKe', N'NhapHang', N'BanHang'
        ))
);
GO
--  INDEX HỖ TRỢ TỐC ĐỘ TRUY VẤN
CREATE INDEX IX_SanPham_DanhMuc  ON SanPham(maDanhMuc);
CREATE INDEX IX_DonHang_KhachHang ON DonHang(maKH);
CREATE INDEX IX_DonHang_TrangThai ON DonHang(trangThai);
CREATE INDEX IX_TonKho_Kho        ON TonKho(maKho);
CREATE INDEX IX_TonKho_SanPham    ON TonKho(maSP);
CREATE INDEX IX_HoaDon_NgayLap    ON HoaDon(ngayLap);
CREATE INDEX IX_ThanhToan_TrangThai ON ThanhToan(trangThai);
GO
--  DỮ LIỆU MẪU

-- VaiTro (6 vai trò)
INSERT INTO VaiTro (tenVaiTro, moTa) VALUES
(N'ChuCuaHang',         N'Chủ cửa hàng – xem báo cáo, kiểm tra chứng từ'),
(N'NhanVienBanHang',    N'Nhân viên bán hàng – lập hóa đơn, xác nhận đơn hàng'),
(N'ThuKho',             N'Thủ kho – quản lý kho, lập phiếu nhập'),
(N'NhanVienVanPhong',   N'Nhân viên văn phòng – quản lý đơn hàng, báo cáo'),
(N'QuanLyHeThong',      N'Quản lý hệ thống – phân quyền, sao lưu'),
(N'KhachHang',          N'Khách hàng – đặt hàng, thanh toán');
GO

--  TaiKhoan
INSERT INTO TaiKhoan (username, [password], maVaiTro, trangThai) VALUES
-- Chủ cửa hàng
(N'nguyen.chuCH',   N'$2b$12$HASH_001', 1, N'HoatDong'),
-- NV bán hàng (5 người)
(N'tran.nv1',       N'$2b$12$HASH_002', 2, N'HoatDong'),
(N'le.nv2',         N'$2b$12$HASH_003', 2, N'HoatDong'),
(N'pham.nv3',       N'$2b$12$HASH_004', 2, N'HoatDong'),
(N'hoang.nv4',      N'$2b$12$HASH_005', 2, N'HoatDong'),
(N'bui.nv5',        N'$2b$12$HASH_006', 2, N'HoatDong'),
-- Thủ kho (2 người)
(N'vu.thukho1',     N'$2b$12$HASH_007', 3, N'HoatDong'),
(N'do.thukho2',     N'$2b$12$HASH_008', 3, N'HoatDong'),
-- NV văn phòng (2 người)
(N'nguyen.nvvp1',   N'$2b$12$HASH_009', 4, N'HoatDong'),
(N'tran.nvvp2',     N'$2b$12$HASH_010', 4, N'HoatDong'),
-- QLHT
(N'admin.qlht',     N'$2b$12$HASH_011', 5, N'HoatDong'),
-- Khách hàng (9 người)
(N'kh.mylinh',      N'$2b$12$HASH_012', 6, N'HoatDong'),
(N'kh.thuytrang',   N'$2b$12$HASH_013', 6, N'HoatDong'),
(N'kh.bichvan',     N'$2b$12$HASH_014', 6, N'HoatDong'),
(N'kh.ngocmai',     N'$2b$12$HASH_015', 6, N'HoatDong'),
(N'kh.hoangnam',    N'$2b$12$HASH_016', 6, N'HoatDong'),
(N'kh.thanhha',     N'$2b$12$HASH_017', 6, N'HoatDong'),
(N'kh.kimchi',      N'$2b$12$HASH_018', 6, N'HoatDong'),
(N'kh.hongthu',     N'$2b$12$HASH_019', 6, N'KhoaTam'),
(N'kh.quynhnu',     N'$2b$12$HASH_020', 6, N'HoatDong');
GO

--  NhanVien
INSERT INTO NhanVien (hoTen, chucVu, caDangLam, sdt, email, maNguoiDung) VALUES
(N'Nguyễn Thị Lan',         N'ChuCuaHang',       NULL,      N'0901234567', N'lan.cch@guardian.vn',  1),
(N'Trần Văn Minh',          N'NhanVienBanHang',   N'Sáng',   N'0912345678', N'minh.nv1@guardian.vn', 2),
(N'Lê Thị Hoa',             N'NhanVienBanHang',   N'Chiều',  N'0923456789', N'hoa.nv2@guardian.vn',  3),
(N'Phạm Quốc Hùng',         N'NhanVienBanHang',   N'Sáng',   N'0934567890', N'hung.nv3@guardian.vn', 4),
(N'Hoàng Thị Yến',          N'NhanVienBanHang',   N'Chiều',  N'0945678901', N'yen.nv4@guardian.vn',  5),
(N'Bùi Văn Tùng',           N'NhanVienBanHang',   N'Tối',    N'0956789012', N'tung.nv5@guardian.vn', 6),
(N'Vũ Thị Ngọc',            N'ThuKho',            N'Sáng',   N'0967890123', N'ngoc.tk1@guardian.vn', 7),
(N'Đỗ Hữu Bình',            N'ThuKho',            N'Chiều',  N'0978901234', N'binh.tk2@guardian.vn', 8),
(N'Nguyễn Văn An',          N'NhanVienVanPhong',  NULL,      N'0989012345', N'an.vp1@guardian.vn',   9),
(N'Trần Thị Bảo',           N'NhanVienVanPhong',  NULL,      N'0990123456', N'bao.vp2@guardian.vn',  10),
(N'Lê Hoàng Admin',         N'QuanLyHeThong',     NULL,      N'0901122334', N'admin@guardian.vn',    11);
GO

--  KhachHang
INSERT INTO KhachHang (hoTen, email, sdt, diaChi, loaiKH, diemThuong, maNguoiDung) VALUES
(N'Võ Thị Mỹ Linh',     N'mylinh@gmail.com',    N'0901000001', N'12 Lê Lợi, Q.1, TP.HCM',                N'ThanThiet', 1250, 12),
(N'Nguyễn Thúy Trang',  N'thuytrang@gmail.com', N'0902000002', N'45 Nguyễn Huệ, Q.1, TP.HCM',            N'VIP',       3500, 13),
(N'Trần Bích Vân',       N'bichvan@gmail.com',   N'0903000003', N'78 Điện Biên Phủ, Q.3, TP.HCM',         N'Moi',       150,  14),
(N'Phan Ngọc Mai',       N'ngocmai@gmail.com',   N'0904000004', N'23 Cách Mạng Tháng 8, Q.10, TP.HCM',    N'ThanThiet', 870,  15),
(N'Lê Hoàng Nam',        N'hoangnam@gmail.com',  N'0905000005', N'56 Võ Văn Tần, Q.3, TP.HCM',            N'Moi',       0,    16),
(N'Đặng Thanh Hà',       N'thanhha@gmail.com',   N'0906000006', N'90 Trần Hưng Đạo, Q.5, TP.HCM',         N'ThanThiet', 620,  17),
(N'Hồ Kim Chi',          N'kimchi@gmail.com',    N'0907000007', N'34 Pasteur, Q.1, TP.HCM',               N'VIP',       5100, 18),
(N'Bùi Hồng Thu',        N'hongthu@gmail.com',   N'0908000008', N'67 Hai Bà Trưng, Q.1, TP.HCM',          N'Moi',       80,   19),
(N'Đinh Quỳnh Như',      N'quynhnu@gmail.com',   N'0909000009', N'101 Nguyễn Trãi, Q.5, TP.HCM',          N'ThanThiet', 420,  20),
-- 10 khách hàng không có tài khoản (khách vãng lai đã được tạo hồ sơ)
(N'Lý Thanh Xuân',       N'thanhxuan@yahoo.com', N'0910000010', N'15 Hùng Vương, Q.5, TP.HCM',            N'Moi',       0,    NULL),
(N'Mai Trọng Nghĩa',     N'trongnghia@yahoo.com',N'0911000011', N'29 Lý Tự Trọng, Q.1, TP.HCM',           N'Moi',       0,    NULL),
(N'Vương Diễm My',       N'diemmy@yahoo.com',    N'0912000012', N'88 Ngô Gia Tự, Q.10, TP.HCM',           N'Moi',       0,    NULL),
(N'Tô Khánh Hòa',        N'khanhoa@yahoo.com',   N'0913000013', N'42 An Dương Vương, Q.5, TP.HCM',        N'Moi',       0,    NULL),
(N'Cù Thị Phương',       N'phuong.cu@yahoo.com', N'0914000014', N'7 Trần Phú, Q.5, TP.HCM',               N'Moi',       0,    NULL),
(N'Ngô Bảo Châu',        N'baochau@hotmail.com', N'0915000015', N'53 Lê Thánh Tôn, Q.1, TP.HCM',          N'Moi',       0,    NULL),
(N'Trịnh Minh Tuấn',     N'minhTuan@hotmail.com',N'0916000016', N'11 Đinh Tiên Hoàng, Q.Bình Thạnh',      N'Moi',       0,    NULL),
(N'Điền Thanh Loan',     N'thanhloan@hotmail.com',N'0917000017',N'65 Xô Viết Nghệ Tĩnh, Q.Bình Thạnh',   N'Moi',       0,    NULL),
(N'Trang Minh Khoa',     N'minhkhoa@live.com',   N'0918000018', N'20 Bạch Đằng, Q.Bình Thạnh',            N'Moi',       0,    NULL),
(N'Lâm Kế Hoạch',        N'keholach@live.com',   N'0919000019', N'33 Phan Xích Long, Q.Phú Nhuận',        N'Moi',       0,    NULL),
(N'Dương Gia Linh',      N'gialinh@live.com',    N'0920000020', N'9 Hoàng Văn Thụ, Q.Phú Nhuận',          N'Moi',       0,    NULL);
GO

--  DanhMuc
INSERT INTO DanhMuc (tenDanhMuc, moTa) VALUES
(N'Chăm Sóc Da Mặt',    N'Kem dưỡng, serum, toner, mặt nạ...'),
(N'Trang Điểm',         N'Son môi, phấn, mascara, kẻ mắt...'),
(N'Chăm Sóc Cơ Thể',   N'Sữa tắm, kem dưỡng thể, lăn khử mùi...'),
(N'Chăm Sóc Tóc',      N'Dầu gội, dầu xả, kem ủ tóc...'),
(N'Nước Hoa & Xịt Thơm',N'Nước hoa, xịt thơm toàn thân...');
GO

--  SanPham
INSERT INTO SanPham (tenSP, thuongHieu, giaBan, moTa, hanSuDung, maDanhMuc) VALUES
(N'Kem dưỡng ẩm Neutrogena Hydro Boost', N'Neutrogena', 350000, N'Dưỡng ẩm 72h, không gây nhờn', '2026-12-31', 1),
(N'Serum Vitamin C Klairs 30ml',          N'Klairs',     490000, N'Làm sáng da, giảm thâm nám',  '2026-09-30', 1),
(N'Toner Hada Labo Gokujyun 170ml',       N'Hada Labo',  220000, N'Cấp ẩm sâu, làm mềm da',     '2027-03-31', 1),
(N'Mặt nạ đất sét Innisfree Jeju',        N'Innisfree',  195000, N'Làm sạch sâu lỗ chân lông',  '2026-06-30', 1),
(N'Son kem lì Romand Blur Fudge 5g',      N'Romand',     280000, N'Màu sắc rực rỡ, lên màu chuẩn','2027-01-31',2),
(N'Phấn phủ kiềm dầu L.A Girl 29g',      N'L.A Girl',   185000, N'Kiềm dầu 8 giờ, phủ nhẹ',    '2026-11-30', 2),
(N'Mascara chống thấm nước Maybelline',   N'Maybelline', 165000, N'Cong mi, chống lem',          '2025-12-31', 2),
(N'Kẻ mắt nước đen The Face Shop 7g',    N'The Face Shop',145000,N'Nét mảnh, lâu trôi',         '2026-08-31', 2),
(N'Sữa tắm dưỡng ẩm Dove 530ml',         N'Dove',       130000, N'Dưỡng ẩm, làm mịn da',       '2027-06-30', 3),
(N'Kem dưỡng thể Vaseline Cocoa Butter',  N'Vaseline',   99000,  N'Dưỡng ẩm sâu, bơ cacao',     '2027-12-31', 3),
(N'Lăn khử mùi Nivea Pearl & Beauty',    N'Nivea',      79000,  N'Khử mùi 48h, trắng da nách', '2027-09-30', 3),
(N'Tẩy tế bào chết body St. Ives 283g',  N'St. Ives',   175000, N'Tẩy sạch, mịn màng',         '2026-10-31', 3),
(N'Dầu gội Pantene dưỡng chất 650ml',    N'Pantene',    155000, N'Phục hồi, bóng mượt',        '2027-06-30', 4),
(N'Dầu xả TRESemmé Keratin Smooth',      N'TRESemmé',   145000, N'Chống xơ rối, giữ nếp',      '2027-03-31', 4),
(N'Kem ủ tóc Moroccan Oil 250ml',         N'Moroccan Oil',650000,N'Phục hồi hư tổn chuyên sâu', '2026-07-31', 4),
(N'Nước hoa Victoria Secret Love Spell',  N'Victoria Secret',850000,N'75ml, hương ngọt dịu',   '2028-01-31', 5),
(N'Xịt thơm body Bath & Body Works',     N'Bath & Body Works',320000,N'236ml, hương hoa nhẹ nhàng','2027-12-31',5),
(N'Nước hoa The Body Shop White Musk',   N'The Body Shop',420000,N'30ml EDT, hương xạ hương',  '2028-06-30', 5),
(N'Serum dưỡng trắng Some By Mi 30ml',   N'Some By Mi', 520000, N'Niacinamide 10%, giảm thâm', '2026-11-30', 1),
(N'Kem chống nắng Anessa SPF50+ 60ml',   N'Anessa',     680000, N'PA++++, chống nắng bền bỉ',  '2026-05-31', 1);
GO

--  Kho
INSERT INTO Kho (tenKho, diaChi, maThuKho) VALUES
(N'Kho Chính Q.1',          N'10 Bến Nghé, Q.1, TP.HCM',           7),
(N'Kho Phụ Q.3',            N'25 Võ Thị Sáu, Q.3, TP.HCM',         8),
(N'Kho Hàng Trưng Bày',     N'10 Bến Nghé, Q.1, TP.HCM (Tầng trệt)',7),
(N'Kho Dự Trữ Bình Thạnh',  N'50 Nơ Trang Long, Q.Bình Thạnh',     8),
(N'Kho Hàng Nhập Mới',      N'10 Bến Nghé, Q.1, TP.HCM (Tầng 1)',  7);
GO

--  TonKho
INSERT INTO TonKho (maKho, maSP, soLuongTon, ngayCapNhat) VALUES
(1, 1,  150, '2026-04-01'),
(1, 2,  80,  '2026-04-01'),
(1, 3,  200, '2026-04-01'),
(1, 4,  120, '2026-04-01'),
(1, 5,  300, '2026-04-01'),
(1, 6,  250, '2026-04-01'),
(1, 7,  180, '2026-04-01'),
(1, 8,  210, '2026-04-01'),
(1, 9,  500, '2026-04-01'),
(1, 10, 400, '2026-04-01'),
(1, 11, 350, '2026-04-01'),
(1, 12, 130, '2026-04-01'),
(2, 13, 160, '2026-04-01'),
(2, 14, 140, '2026-04-01'),
(2, 15, 60,  '2026-04-01'),
(2, 16, 45,  '2026-04-01'),
(2, 17, 90,  '2026-04-01'),
(2, 18, 55,  '2026-04-01'),
(2, 19, 75,  '2026-04-01'),
(2, 20, 100, '2026-04-01');
GO

--  GioHang
INSERT INTO GioHang (maKH, ngayTao, trangThai) VALUES
(1, '2026-04-10 09:00:00', N'DaDat'),
(2, '2026-04-11 10:30:00', N'DaDat'),
(3, '2026-04-12 14:00:00', N'DaDat'),
(4, '2026-04-13 11:00:00', N'DaDat'),
(5, '2026-04-14 16:00:00', N'DangMua');
GO

--  DonHang
INSERT INTO DonHang (maKH, maNV, ngayDat, tongTien, trangThai, maGioHang) VALUES
(1,  2, '2026-04-10 09:30:00', 840000,   N'HoanThanh',  1),
(2,  3, '2026-04-11 11:00:00', 1170000,  N'HoanThanh',  2),
(3,  4, '2026-04-12 14:30:00', 295000,   N'DaXacNhan',  3),
(4,  5, '2026-04-13 11:30:00', 675000,   N'DangGiao',   4),
(5,  2, '2026-04-14 16:30:00', 465000,   N'ChoDuyet',   NULL),
(6,  3, '2026-04-01 09:00:00', 1530000,  N'HoanThanh',  NULL),
(7,  4, '2026-04-02 10:00:00', 720000,   N'HoanThanh',  NULL),
(8,  5, '2026-04-03 13:00:00', 345000,   N'HoanThanh',  NULL),
(9,  2, '2026-04-04 15:00:00', 990000,   N'HoanThanh',  NULL),
(1,  3, '2026-04-05 09:30:00', 540000,   N'HoanThanh',  NULL),
(2,  4, '2026-04-06 10:30:00', 1350000,  N'HoanThanh',  NULL),
(3,  5, '2026-04-07 11:30:00', 390000,   N'DaHuy',      NULL),
(4,  2, '2026-04-08 14:30:00', 810000,   N'HoanThanh',  NULL),
(5,  3, '2026-04-09 15:30:00', 625000,   N'HoanThanh',  NULL),
(6,  4, '2026-04-10 09:00:00', 475000,   N'HoanThanh',  NULL),
(7,  5, '2026-04-10 11:00:00', 2100000,  N'HoanThanh',  NULL),
(8,  2, '2026-04-11 13:00:00', 560000,   N'DaXacNhan',  NULL),
(9,  3, '2026-04-12 14:00:00', 730000,   N'DangGiao',   NULL),
(10, 4, '2026-04-13 10:00:00', 450000,   N'DaXacNhan',  NULL),
(11, 5, '2026-04-14 12:00:00', 880000,   N'ChoDuyet',   NULL);
GO

--  ChiTietDonHang
INSERT INTO ChiTietDonHang (maDonHang, maSP, soLuong, donGia) VALUES
(1,  1,  1, 350000),
(1,  5,  1, 280000),
(1,  11, 1,  79000),  -- (cộng ~709000, đơn giản hóa thành 840000 khi có phụ phí)
(2,  2,  1, 490000),
(2,  3,  2, 220000),
(3,  6,  1, 185000),
(3,  11, 1,  79000),
(4,  16, 1, 850000),  -- Nước hoa
(4,  17, 1, 320000),  -- chỉ lấy 1 sp để demo
(5,  19, 1, 520000),
(5,  20, 1, 680000),  -- demo dòng 5
(6,  15, 1, 650000),
(6,  16, 1, 850000),
(7,  9,  2, 130000),
(7,  10, 2,  99000),
(8,  8,  1, 145000),
(8,  7,  1, 165000),
(9,  2,  1, 490000),
(9,  1,  1, 350000),
(10, 13, 1, 155000);
GO

--  HoaDon 
INSERT INTO HoaDon (maDonHang, maNV, ngayLap, tongTien, trangThai) VALUES
(1,  2, '2026-04-10 09:35:00', 840000,  N'DaIn'),
(2,  3, '2026-04-11 11:05:00', 1170000, N'DaIn'),
(3,  4, '2026-04-12 14:35:00', 295000,  N'DaLap'),
(4,  5, '2026-04-13 11:35:00', 675000,  N'DaLap'),
(6,  3, '2026-04-01 09:10:00', 1530000, N'DaIn'),
(7,  4, '2026-04-02 10:10:00', 720000,  N'DaIn'),
(8,  5, '2026-04-03 13:10:00', 345000,  N'DaIn'),
(9,  2, '2026-04-04 15:10:00', 990000,  N'DaIn'),
(10, 3, '2026-04-05 09:40:00', 540000,  N'DaIn'),
(11, 4, '2026-04-06 10:40:00', 1350000, N'DaIn'),
(13, 2, '2026-04-08 14:40:00', 810000,  N'DaIn'),
(14, 3, '2026-04-09 15:40:00', 625000,  N'DaIn'),
(15, 4, '2026-04-10 09:10:00', 475000,  N'DaIn'),
(16, 5, '2026-04-10 11:10:00', 2100000, N'DaIn'),
(17, 2, '2026-04-11 13:10:00', 560000,  N'DaLap'),
(18, 3, '2026-04-12 14:10:00', 730000,  N'DaLap'),
(19, 4, '2026-04-13 10:10:00', 450000,  N'DaLap'),
(20, 5, '2026-04-14 12:10:00', 880000,  N'DaLap');
GO

--  ThanhToan
INSERT INTO ThanhToan (maDonHang, soTien, thoiGian, trangThai, loaiThanhToan) VALUES
(1,  840000,  '2026-04-10 09:40:00', N'ThanhCong', N'TienMat'),
(2,  1170000, '2026-04-11 11:10:00', N'ThanhCong', N'Online'),
(3,  295000,  '2026-04-12 14:40:00', N'ThanhCong', N'TienMat'),
(4,  675000,  '2026-04-13 11:40:00', N'ThanhCong', N'Online'),
(6,  1530000, '2026-04-01 09:20:00', N'ThanhCong', N'TienMat'),
(7,  720000,  '2026-04-02 10:20:00', N'ThanhCong', N'Online'),
(8,  345000,  '2026-04-03 13:20:00', N'ThanhCong', N'TienMat'),
(9,  990000,  '2026-04-04 15:20:00', N'ThanhCong', N'Online'),
(10, 540000,  '2026-04-05 09:50:00', N'ThanhCong', N'TienMat'),
(11, 1350000, '2026-04-06 10:50:00', N'ThanhCong', N'Online'),
(13, 810000,  '2026-04-08 14:50:00', N'ThanhCong', N'TienMat'),
(14, 625000,  '2026-04-09 15:50:00', N'ThanhCong', N'Online'),
(15, 475000,  '2026-04-10 09:20:00', N'ThanhCong', N'TienMat'),
(16, 2100000, '2026-04-10 11:20:00', N'ThanhCong', N'Online'),
(17, 560000,  '2026-04-11 13:20:00', N'ChoPhuDuyet',N'Online'),
(18, 730000,  '2026-04-12 14:20:00', N'ChoPhuDuyet',N'Online'),
(19, 450000,  '2026-04-13 10:20:00', N'ChoPhuDuyet',N'Online'),
(20, 880000,  '2026-04-14 12:20:00', N'ChoPhuDuyet',N'TienMat'),
(5,  465000,  '2026-04-14 17:00:00', N'ChoPhuDuyet',N'TienMat'),
(12, 390000,  '2026-04-07 11:35:00', N'Huy',        N'TienMat');
GO

--  ThanhToanTienMat
INSERT INTO ThanhToanTienMat (maThanhToan, soTienNhan, soTienThoi) VALUES
(1,  900000,  60000),
(3,  300000,  5000),
(5,  600000,  135000),
(8,  400000,  55000),
(10, 600000,  60000);
GO

--  ThanhToanOnline
INSERT INTO ThanhToanOnline (maThanhToan, nganHang, maNganHang, maGiaoDich) VALUES
(2,  N'Vietcombank', N'VCB', N'VCB20260411110000001'),
(4,  N'BIDV',        N'BID', N'BID20260413114000001'),
(7,  N'Techcombank', N'TCB', N'TCB20260402102000001'),
(9,  N'Vietinbank',  N'VTB', N'VTB20260404152000001'),
(12, N'ACB',         N'ACB', N'ACB20260410112000001');
GO

--  PhieuNhapHang
INSERT INTO PhieuNhapHang (ngayNhap, maKho, maThuKho, tongTien) VALUES
('2026-03-01 08:00:00', 1, 7, 45000000),
('2026-03-15 08:00:00', 2, 8, 28000000),
('2026-04-01 08:00:00', 1, 7, 62000000),
('2026-04-08 09:00:00', 2, 8, 18000000),
('2026-04-13 08:00:00', 1, 7, 35000000);
GO

--  ChiTietPhieuNhap
INSERT INTO ChiTietPhieuNhap (maPhieuNhap, maSP, soLuong, donGia) VALUES
(1, 1,  200, 200000),
(1, 2,  100, 300000),
(1, 3,  300, 120000),
(1, 4,  150, 110000),
(1, 5,  400, 160000),
(2, 6,  300, 100000),
(2, 7,  250, 90000),
(2, 8,  300, 80000),
(2, 9,  600, 70000),
(2, 10, 500, 55000),
(3, 11, 400, 45000),
(3, 12, 200, 95000),
(3, 13, 200, 85000),
(3, 14, 180, 80000),
(3, 15, 80,  450000),
(4, 16, 60,  600000),
(4, 17, 120, 180000),
(4, 18, 70,  280000),
(5, 19, 100, 320000),
(5, 20, 120, 400000);
GO

--  KiemKe
INSERT INTO KiemKe (maKho, ngayKiemKe, maNhanVien, ghiChu) VALUES
(1, '2026-01-31', 7, N'Kiểm kê cuối tháng 1/2026 – khớp số liệu'),
(2, '2026-01-31', 8, N'Kiểm kê cuối tháng 1/2026 – chênh lệch nhỏ'),
(1, '2026-02-28', 7, N'Kiểm kê cuối tháng 2/2026 – khớp số liệu'),
(2, '2026-02-28', 8, N'Kiểm kê cuối tháng 2/2026'),
(1, '2026-03-31', 7, N'Kiểm kê cuối tháng 3/2026 – phát hiện 5 sp hết hạn');
GO

--  BaoCao
INSERT INTO BaoCao (loaiBaoCao, ngayLap, thoiGian, tongTien, maNhanVien, maKho) VALUES
(N'DoanhThuNgay',   '2026-04-14', '2026-04-14 23:59:00', 1345000,  9,  NULL),
(N'DoanhThuThang',  '2026-04-01', '2026-04-01 08:00:00', 18450000, 9,  NULL),
(N'DoanhThuQuy',    '2026-04-01', '2026-04-01 08:00:00', 52300000, 10, NULL),
(N'KiemKe',         '2026-03-31', '2026-03-31 17:00:00', NULL,     10, 1),
(N'NhapHang',       '2026-04-13', '2026-04-13 10:00:00', 35000000, 9,  1);
GO

--  VIEWS

-- View: Thống kê doanh thu theo ngày
CREATE OR ALTER VIEW vw_DoanhThuNgay AS
SELECT
    CAST(dh.ngayDat AS DATE)    AS ngay,
    COUNT(dh.maDonHang)         AS soDonHang,
    SUM(dh.tongTien)            AS doanhThu
FROM DonHang dh
WHERE dh.trangThai IN (N'HoanThanh', N'DaGiao')
GROUP BY CAST(dh.ngayDat AS DATE);
GO

-- View: Tồn kho hiện tại kèm thông tin sản phẩm
CREATE OR ALTER VIEW vw_TonKhoHienTai AS
SELECT
    k.tenKho,
    sp.tenSP,
    sp.thuongHieu,
    dm.tenDanhMuc,
    tk.soLuongTon,
    tk.ngayCapNhat,
    sp.giaBan,
    tk.soLuongTon * sp.giaBan AS giaTriTon
FROM TonKho tk
JOIN Kho     k  ON k.maKho     = tk.maKho
JOIN SanPham sp ON sp.maSP     = tk.maSP
JOIN DanhMuc dm ON dm.maDanhMuc= sp.maDanhMuc;
GO

-- View: Lịch sử mua hàng khách hàng
CREATE OR ALTER VIEW vw_LichSuMuaHang AS
SELECT
    kh.hoTen        AS tenKhachHang,
    kh.loaiKH,
    dh.maDonHang,
    dh.ngayDat,
    dh.tongTien,
    dh.trangThai    AS trangThaiDonHang,
    nv.hoTen        AS nhanVienXuLy
FROM DonHang dh
JOIN KhachHang kh ON kh.maKH   = dh.maKH
LEFT JOIN NhanVien nv ON nv.maNV= dh.maNV;
GO

--  STORED PROCEDURES

-- SP: Đặt hàng - tạo đơn hàng từ giỏ hàng
CREATE OR ALTER PROCEDURE sp_TaoDonHang
    @maKH       INT,
    @maNV       INT,
    @maGioHang  INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Tính tổng tiền từ giỏ hàng
        DECLARE @tongTien DECIMAL(18,2);
        SELECT @tongTien = SUM(soLuong * donGia)
        FROM ChiTietGioHang
        WHERE maGioHang = @maGioHang;

        IF @tongTien IS NULL OR @tongTien <= 0
        BEGIN
            RAISERROR(N'Giỏ hàng trống hoặc không hợp lệ.', 16, 1);
            RETURN;
        END

        -- Tạo đơn hàng
        INSERT INTO DonHang (maKH, maNV, ngayDat, tongTien, trangThai, maGioHang)
        VALUES (@maKH, @maNV, GETDATE(), @tongTien, N'ChoDuyet', @maGioHang);

        DECLARE @maDonHang INT = SCOPE_IDENTITY();

        -- Copy chi tiết giỏ hàng → chi tiết đơn hàng
        INSERT INTO ChiTietDonHang (maDonHang, maSP, soLuong, donGia)
        SELECT @maDonHang, maSP, soLuong, donGia
        FROM ChiTietGioHang
        WHERE maGioHang = @maGioHang;

        -- Cập nhật trạng thái giỏ hàng
        UPDATE GioHang SET trangThai = N'DaDat'
        WHERE maGioHang = @maGioHang;

        COMMIT TRANSACTION;
        SELECT @maDonHang AS maDonHangMoi;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

-- SP: Cập nhật tồn kho sau khi nhập hàng
CREATE OR ALTER PROCEDURE sp_CapNhatTonKhoNhap
    @maPhieuNhap INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @maKho INT;
        SELECT @maKho = maKho FROM PhieuNhapHang WHERE maPhieuNhap = @maPhieuNhap;

        -- Cập nhật hoặc thêm mới tồn kho
        MERGE TonKho AS target
        USING (
            SELECT maSP, SUM(soLuong) AS soLuong
            FROM ChiTietPhieuNhap
            WHERE maPhieuNhap = @maPhieuNhap
            GROUP BY maSP
        ) AS source ON target.maKho = @maKho AND target.maSP = source.maSP
        WHEN MATCHED THEN
            UPDATE SET soLuongTon = soLuongTon + source.soLuong,
                       ngayCapNhat = GETDATE()
        WHEN NOT MATCHED THEN
            INSERT (maKho, maSP, soLuongTon, ngayCapNhat)
            VALUES (@maKho, source.maSP, source.soLuong, GETDATE());

        COMMIT TRANSACTION;
        PRINT N'Cập nhật tồn kho thành công.';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;
GO

--  KIỂM TRA DỮ LIỆU ĐÃ INSERT
SELECT N'VaiTro'            AS [Bang], COUNT(*) AS [SoDong] FROM VaiTro
UNION ALL
SELECT N'TaiKhoan',   COUNT(*) FROM TaiKhoan
UNION ALL
SELECT N'NhanVien',   COUNT(*) FROM NhanVien
UNION ALL
SELECT N'KhachHang',  COUNT(*) FROM KhachHang
UNION ALL
SELECT N'DanhMuc',    COUNT(*) FROM DanhMuc
UNION ALL
SELECT N'SanPham',    COUNT(*) FROM SanPham
UNION ALL
SELECT N'Kho',        COUNT(*) FROM Kho
UNION ALL
SELECT N'TonKho',     COUNT(*) FROM TonKho
UNION ALL
SELECT N'GioHang',    COUNT(*) FROM GioHang
UNION ALL
SELECT N'DonHang',    COUNT(*) FROM DonHang
UNION ALL
SELECT N'ChiTietDonHang',  COUNT(*) FROM ChiTietDonHang
UNION ALL
SELECT N'HoaDon',     COUNT(*) FROM HoaDon
UNION ALL
SELECT N'ThanhToan',  COUNT(*) FROM ThanhToan
UNION ALL
SELECT N'ThanhToanTienMat', COUNT(*) FROM ThanhToanTienMat
UNION ALL
SELECT N'ThanhToanOnline',  COUNT(*) FROM ThanhToanOnline
UNION ALL
SELECT N'PhieuNhapHang',    COUNT(*) FROM PhieuNhapHang
UNION ALL
SELECT N'ChiTietPhieuNhap', COUNT(*) FROM ChiTietPhieuNhap
UNION ALL
SELECT N'KiemKe',     COUNT(*) FROM KiemKe
UNION ALL
SELECT N'BaoCao',     COUNT(*) FROM BaoCao;
GO

PRINT N'=== QLBH_Guardian database created successfully! ===';
GO
select*from dbo.BaoCao
select*from dbo.ChiTietDonHang
select*from dbo.ChiTietGioHang
select*from dbo.ChiTietPhieuNhap
select*from dbo.DanhMuc
select*from dbo.DonHang
select*from dbo.GioHang
select*from dbo.HoaDon
select*from dbo.KiemKe
select*from dbo.KhachHang
select*from dbo.Kho
select*from dbo.NhanVien
select*from dbo.PhieuNhapHang
select*from dbo.SanPham
select*from dbo.TaiKhoan
select*from dbo.TonKho
select*from dbo.ThanhToan
select*from dbo.ThanhToanOnline
select*from dbo.ThanhToanTienMat
select*from dbo.VaiTro
select*from dbo.vw_DoanhThuNgay
select*from dbo.vw_LichSuMuaHang
select*from dbo.vw_TonKhoHienTai

--Bảng tầm ảnh hưởng
--LOG chung cho hệ thống
CREATE TABLE NhatKyHeThong (
   maLog INT IDENTITY(1,1) PRIMARY KEY,
   tenBang NVARCHAR(100),
   hanhDong NVARCHAR(10), -- INSERT / UPDATE / DELETE
   khoaChinh NVARCHAR(100),
   thoiGian DATETIME DEFAULT GETDATE(),
   nguoiThucHien NVARCHAR(100) NULL
);
GO
--Trigger log dữ liệu
--Log đơn hàng
CREATE TRIGGER trg_Log_DonHang
ON DonHang
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    -- INSERT
    INSERT INTO NhatKyHeThong (tenBang, hanhDong, khoaChinh)
    SELECT 'DonHang', 'INSERT', CAST(maDonHang AS NVARCHAR)
    FROM inserted;
    -- DELETE
    INSERT INTO NhatKyHeThong (tenBang, hanhDong, khoaChinh)
    SELECT 'DonHang', 'DELETE', CAST(maDonHang AS NVARCHAR)
    FROM deleted;
    -- UPDATE
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
    BEGIN
        INSERT INTO NhatKyHeThong (tenBang, hanhDong, khoaChinh)
        SELECT 'DonHang', 'UPDATE', CAST(maDonHang AS NVARCHAR)
        FROM inserted;
    END
END;
GO
 --log sản phẩm
 CREATE TRIGGER trg_Log_SanPham
ON SanPham
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
   INSERT INTO NhatKyHeThong (tenBang, hanhDong, khoaChinh)
   SELECT 'SanPham', 'INSERT', CAST(maSP AS NVARCHAR)
   FROM inserted;
   INSERT INTO NhatKyHeThong (tenBang, hanhDong, khoaChinh)
   SELECT 'SanPham', 'DELETE', CAST(maSP AS NVARCHAR)
   FROM deleted;
   IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
   BEGIN
       INSERT INTO NhatKyHeThong (tenBang, hanhDong, khoaChinh)
       SELECT 'SanPham', 'UPDATE', CAST(maSP AS NVARCHAR)
       FROM inserted;
   END
END;
GO
--trigger trừ tồn kho khi bán hàng
CREATE TRIGGER trg_TruTonKho
ON ChiTietDonHang
AFTER INSERT
AS
BEGIN
    UPDATE tk
    SET tk.soLuongTon = tk.soLuongTon - i.soLuong,
        tk.ngayCapNhat = GETDATE()
    FROM TonKho tk
    JOIN inserted i ON tk.maSP = i.maSP;
    -- kiểm tra âm kho
    IF EXISTS (
        SELECT * FROM TonKho WHERE soLuongTon < 0
    )
    BEGIN
        ROLLBACK;
        RAISERROR(N'Không đủ hàng trong kho!', 16, 1);
    END
END;
GO
 --trigger cập nhật tổng tiền đơn hàng
 CREATE TRIGGER trg_UpdateTongTienDonHang
ON ChiTietDonHang
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    UPDATE dh
    SET tongTien = (
        SELECT SUM(soLuong * donGia)
        FROM ChiTietDonHang
        WHERE maDonHang = dh.maDonHang
    )
    FROM DonHang dh
    WHERE dh.maDonHang IN (
        SELECT maDonHang FROM inserted
        UNION
        SELECT maDonHang FROM deleted
    );
END;
GO
 --trigger tự động tạo hóa đơn khi hoàn thành
 CREATE TRIGGER trg_TaoHoaDon
ON DonHang
AFTER UPDATE
AS
BEGIN
   INSERT INTO HoaDon (maDonHang, maNV, tongTien)
   SELECT i.maDonHang, i.maNV, i.tongTien
   FROM inserted i
   WHERE i.trangThai = N'HoanThanh'
   AND NOT EXISTS (
       SELECT 1 FROM HoaDon hd WHERE hd.maDonHang = i.maDonHang
   );
END;
GO
--trigger kiểm tra thanh toán
CREATE TRIGGER trg_CheckThanhToan
ON ThanhToan
AFTER INSERT
AS
BEGIN
   IF EXISTS (
       SELECT 1
       FROM inserted i
       JOIN DonHang dh ON dh.maDonHang = i.maDonHang
       WHERE i.soTien < dh.tongTien
   )
   BEGIN
       ROLLBACK;
       RAISERROR(N'Số tiền thanh toán không đủ!', 16, 1);
   END
END;
GO
--bổ sung ràng buộc dữ liệu
--Khách hàng phải có số điện thoại, email
ALTER TABLE KhachHang
ALTER COLUMN sdt VARCHAR(15) NOT NULL;
ALTER TABLE KhachHang
ALTER COLUMN email NVARCHAR(150) NOT NULL;
 --Nhân viên phải có số điện thoại
 ALTER TABLE NhanVien
ALTER COLUMN sdt VARCHAR(15) NOT NULL;
--SDT chỉ chứa số
ALTER TABLE KhachHang
ADD CONSTRAINT CK_KH_SDT_Format
CHECK (sdt NOT LIKE '%[^0-9]%');
ALTER TABLE NhanVien
ADD CONSTRAINT CK_NV_SDT_Format
CHECK (sdt NOT LIKE '%[^0-9]%');
--Mẫu email hợp lệ
ALTER TABLE KhachHang
ADD CONSTRAINT CK_KH_Email_Format
CHECK (email LIKE '%@%.%');
ALTER TABLE NhanVien
ADD CONSTRAINT CK_NV_Email_Format
CHECK (email IS NULL OR email LIKE '%@%.%');
 --unique số điện thoại không trùng nhau
 ALTER TABLE KhachHang
ADD CONSTRAINT UQ_KH_SDT UNIQUE (sdt);

 ALTER TABLE NhanVien
ADD CONSTRAINT UQ_NV_SDT UNIQUE (sdt);
 --không cho tạo đơn nếu khách null
 ALTER TABLE DonHang
ADD CONSTRAINT CK_DonHang_KH
CHECK (maKH IS NOT NULL);
 --không cho giá = 0
 ALTER TABLE ChiTietDonHang
ADD CONSTRAINT CK_CTDH_GiaLonHon0
CHECK (donGia > 0);
--không cho thanh toán khi đơn bị hủy
CREATE TRIGGER trg_CheckTrangThaiDonHang
ON ThanhToan
AFTER INSERT
AS
BEGIN
   IF EXISTS (
       SELECT 1
       FROM inserted i
       JOIN DonHang dh ON dh.maDonHang = i.maDonHang
       WHERE dh.trangThai = N'DaHuy'
   )
   BEGIN
       ROLLBACK;
       RAISERROR(N'Không thể thanh toán đơn hàng đã huỷ!',16,1);
   END
END;
GO
--không cho tồn kho âm
ALTER TABLE TonKho
ADD CONSTRAINT CK_TonKho_KhongAm2
CHECK (soLuongTon >= 0);
--ngày hết hạn sử dụng > ngày hiện tại
ALTER TABLE SanPham
ADD CONSTRAINT CK_SanPham_HSD
CHECK (hanSuDung IS NULL OR hanSuDung > GETDATE());
 