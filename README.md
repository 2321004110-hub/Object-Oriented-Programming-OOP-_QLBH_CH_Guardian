# QLBH Guardian 2026
# Phần mềm Quản lý Bán hàng — Phiên bản 2026

# Giới thiệu
Hệ thống quản lý bán hàng được xây dựng nhằm tin học hóa quy trình bán hàng tại chuỗi cửa hàng mỹ phẩm Guardian
Hỗ trợ vòng đời giao dịch từ đặt hàng, thanh toán, lập hóa đơn đến quản lý kho và báo cáo doanh thu

# Công nghệ
- Visual Studio
- C# .NET Windows Forms
- SQL Server T-SQL
- Enterprise Architect

# Cách chạy
1. Mở `QLBH_Guardian.sln` bằng Visual Studio
2. Chạy script `QLBH_Guardian_Database.sql` lên SQL Server để tạo CSDL
3. Chỉnh chuỗi kết nối trong `DataAccess/DatabaseHelper.cs`
4. Nhấn **F5** để build & chạy
5. Đăng nhập mặc định: `admin` / `123456`
6. Demo mode: Nếu chưa có SQL Server, chương trình tự hiển thị dữ liệu mẫu

# Chức năng chính
1. Đăng nhập, đăng ký, phân quyền theo vai trò
2. Tìm kiếm sản phẩm, quản lý giỏ hàng, đặt hàng
3. Thanh toán tiền mặt và online qua ngân hàng
4. Lập và in hóa đơn
5. Quản lý kho, nhập hàng, kiểm kê
6. Báo cáo thống kê doanh thu

# Kiến trúc
3 tầng: Giao diện - Nghiệp vụ - Truy cập dữ liệu - SQL

Mô hình BCE: Boundary - Control - Entity
