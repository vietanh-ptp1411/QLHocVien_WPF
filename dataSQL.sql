/* Tạo database nếu chưa có */
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'QLCLASS')
BEGIN
    CREATE DATABASE [QLCLASS];
END
GO
USE [QLCLASS];
GO

/* Học viên */
IF OBJECT_ID('dbo.HocVien','U') IS NULL
BEGIN
    CREATE TABLE dbo.HocVien
    (
        Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_HocVien PRIMARY KEY,
        MaHocVien   NVARCHAR(20)  NOT NULL CONSTRAINT UQ_HocVien_Ma UNIQUE,
        HoTen       NVARCHAR(100) NOT NULL,
        NgaySinh    DATE          NULL,
        Email       NVARCHAR(150) NOT NULL CONSTRAINT UQ_HocVien_Email UNIQUE,
        DienThoai   NVARCHAR(20)  NULL,
        DiaChi      NVARCHAR(255) NULL,
        TrangThai   BIT           NOT NULL CONSTRAINT DF_HocVien_TrangThai DEFAULT (1),  -- 1: hoạt động
        NgayTao     DATE          NOT NULL CONSTRAINT DF_HocVien_NgayTao    DEFAULT (GETDATE())
    );
END
GO

/* Giảng viên */
IF OBJECT_ID('dbo.GiangVien','U') IS NULL
BEGIN
    CREATE TABLE dbo.GiangVien
    (
        Id        INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_GiangVien PRIMARY KEY,
        HoTen     NVARCHAR(100) NOT NULL,
        Email     NVARCHAR(150) NOT NULL CONSTRAINT UQ_GiangVien_Email UNIQUE,
        DienThoai NVARCHAR(20)  NULL,
        ChuyenMon NVARCHAR(150) NULL,
        TrangThai BIT           NOT NULL CONSTRAINT DF_GiangVien_TrangThai DEFAULT (1)
    );
END
GO

/* Khóa học (đơn giá theo gói 10 buổi) */
IF OBJECT_ID('dbo.KhoaHoc','U') IS NULL
BEGIN
    CREATE TABLE dbo.KhoaHoc
    (
        Id            INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_KhoaHoc PRIMARY KEY,
        TenKhoa       NVARCHAR(150) NOT NULL,
        MoTa          NVARCHAR(MAX) NULL,
        DonGia10Buoi  DECIMAL(18,2) NOT NULL,
        TongSoBuoi    INT           NOT NULL,
        TrangThai     BIT           NOT NULL CONSTRAINT DF_KhoaHoc_TrangThai DEFAULT (1)
    );
END
GO

/* Lớp học */
IF OBJECT_ID('dbo.LopHoc','U') IS NULL
BEGIN
    CREATE TABLE dbo.LopHoc
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_LopHoc PRIMARY KEY,
        TenLop       NVARCHAR(150) NOT NULL,
        KhoaHocId    INT NOT NULL,
        GiangVienId  INT NOT NULL,
        NgayBatDau   DATE NOT NULL,
        NgayKetThuc  DATE NULL,
        LinkPhong    NVARCHAR(255) NULL,    -- Zoom/Meet

        CONSTRAINT FK_LopHoc_KhoaHoc   FOREIGN KEY (KhoaHocId)   REFERENCES dbo.KhoaHoc(Id),
        CONSTRAINT FK_LopHoc_GiangVien FOREIGN KEY (GiangVienId) REFERENCES dbo.GiangVien(Id)
    );
END
GO

/* Đăng ký lớp */
IF OBJECT_ID('dbo.DangKy','U') IS NULL
BEGIN
    CREATE TABLE dbo.DangKy
    (
        Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DangKy PRIMARY KEY,
        HocVienId   INT  NOT NULL,
        LopHocId    INT  NOT NULL,
        NgayDangKy  DATE NOT NULL CONSTRAINT DF_DangKy_NgayDangKy DEFAULT (GETDATE()),
        TrangThai   TINYINT NOT NULL CONSTRAINT DF_DangKy_TrangThai DEFAULT (1), -- 1: đang học, 2: bảo lưu, 0: hủy

        CONSTRAINT UQ_DangKy_HocVien_Lop UNIQUE (HocVienId, LopHocId),
        CONSTRAINT FK_DangKy_HocVien FOREIGN KEY (HocVienId) REFERENCES dbo.HocVien(Id),
        CONSTRAINT FK_DangKy_LopHoc  FOREIGN KEY (LopHocId)  REFERENCES dbo.LopHoc(Id)
    );
END
GO

/* Học phí (đóng theo kỳ 10 buổi) */
IF OBJECT_ID('dbo.HocPhi','U') IS NULL
BEGIN
    CREATE TABLE dbo.HocPhi
    (
        Id         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_HocPhi PRIMARY KEY,
        DangKyId   INT NOT NULL,
        KyThu      INT NOT NULL,                      -- 1,2,3...
        SoBuoi     INT NOT NULL CONSTRAINT DF_HocPhi_SoBuoi DEFAULT (10),
        SoTien     DECIMAL(18,2) NOT NULL,
        NgayDong   DATE NOT NULL CONSTRAINT DF_HocPhi_NgayDong DEFAULT (GETDATE()),
        GhiChu     NVARCHAR(255) NULL,

        CONSTRAINT UQ_HocPhi_DangKy_Ky UNIQUE (DangKyId, KyThu),
        CONSTRAINT FK_HocPhi_DangKy FOREIGN KEY (DangKyId) REFERENCES dbo.DangKy(Id)
    );
END
GO

/* Lịch buổi học */
IF OBJECT_ID('dbo.BuoiHoc','U') IS NULL
BEGIN
    CREATE TABLE dbo.BuoiHoc
    (
        Id               INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_BuoiHoc PRIMARY KEY,
        LopHocId         INT NOT NULL,
        SoThuTu          INT NOT NULL,                  -- buổi #1,#2...
        ThoiGianBatDau   DATETIME2(0) NOT NULL,
        ThoiGianKetThuc  DATETIME2(0) NOT NULL,
        ChuDe            NVARCHAR(200) NULL,

        CONSTRAINT FK_BuoiHoc_LopHoc FOREIGN KEY (LopHocId) REFERENCES dbo.LopHoc(Id)
    );
END
GO

/* Điểm danh */
IF OBJECT_ID('dbo.DiemDanh','U') IS NULL
BEGIN
    CREATE TABLE dbo.DiemDanh
    (
        Id         INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_DiemDanh PRIMARY KEY,
        BuoiHocId  INT NOT NULL,
        HocVienId  INT NOT NULL,
        CoMat      BIT NOT NULL,                        -- 1: có mặt, 0: vắng

        CONSTRAINT UQ_DiemDanh_Buoi_HV UNIQUE (BuoiHocId, HocVienId),
        CONSTRAINT FK_DiemDanh_BuoiHoc FOREIGN KEY (BuoiHocId) REFERENCES dbo.BuoiHoc(Id),
        CONSTRAINT FK_DiemDanh_HocVien FOREIGN KEY (HocVienId) REFERENCES dbo.HocVien(Id)
    );
END
GO

IF OBJECT_ID('dbo.Users','U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_NguoiDung PRIMARY KEY,
        TaiKhoan     NVARCHAR(100) NOT NULL CONSTRAINT UQ_NguoiDung_TaiKhoan UNIQUE,
        HoTen        NVARCHAR(150) NULL,
        Email        NVARCHAR(150) NULL,
        VaiTro       NVARCHAR(50)  NOT NULL CONSTRAINT DF_NguoiDung_VaiTro DEFAULT (N'User'), -- Admin/User
        MatKhau      NVARCHAR(50) NOT NULL,          
        TrangThai    BIT           NOT NULL CONSTRAINT DF_NguoiDung_TrangThai DEFAULT (1),
        NgayTao      DATETIME2(0)  NOT NULL CONSTRAINT DF_NguoiDung_NgayTao    DEFAULT (SYSDATETIME())
    );
END
GO



-- dữ liệu người dùng --
INSERT INTO GiangVien (HoTen, Email, DienThoai, ChuyenMon)
VALUES
 (N'Nguyễn Hải Dương',   'an.gv@example.com',   '0912000001', N'C#'),
 (N'Nguyễn Huy Ngọ',   'binh.gv@example.com', '0912000002', N'C#,WPF,Vision,..'),
 (N'Mai Văn Việt Anh',   'chi.gv@example.com',  '0912000003', N'Trợ giảng');

 INSERT INTO KhoaHoc (TenKhoa, MoTa, DonGia10Buoi, TongSoBuoi)
VALUES
 (N'C# Cơ bản',    N'Nhập môn C#',       1200000, 30),
 (N'C# Nâng cao',  N'Generic, async/await, LINQ nâng cao', 1500000, 40),
 (N'WPF MVVM',     N'WPF, Binding, MVVM Toolkit',          1400000, 30),
 (N'Vision',   N'Xử lý ảnh công nghiệp',          1000000, 30),
 (N'ASP.NET Core', N'Web MVC/API, EF Core, Identity',      1800000, 40);

 INSERT INTO LopHoc (TenLop, KhoaHocId, GiangVienId, NgayBatDau, NgayKetThuc, LinkPhong)
VALUES
 (N'C# Cơ bản - Tối 2-4-6',   1, 1, '2025-09-15', '2025-12-15', N'https://meet.example.com/csharp246'),
 (N'C# Nâng cao - Tối 3-5-7', 2, 1, '2025-09-16', '2025-12-20', N'https://meet.example.com/csharp-advanced'),
 (N'WPF MVVM - Cuối tuần',    3, 2, '2025-09-21', '2025-12-07', N'https://meet.example.com/wpf-weekend'),
 (N'SQL Server - Tối 3-5-7',  4, 1, '2025-09-16', '2025-11-30', N'https://meet.example.com/sql357'),
 (N'ASP.NET Core - Online',   5, 2, '2025-09-18', '2025-12-22', N'https://meet.example.com/aspnet-online');

 INSERT INTO DangKy (HocVienId, LopHocId, TrangThai)
VALUES
 (1, 6, 1), -- HV01 học lớp C# cơ bản
 (3, 3, 1), -- HV02 học lớp WPF MVVM
 (1, 4, 1); -- HV03 học lớp SQL Server

 INSERT INTO HocPhi (DangKyId, KyThu, SoBuoi, SoTien, GhiChu)
VALUES
 (5, 1, 10, 1200000, N'Đóng kỳ 1 C# Cơ bản'),
 (6, 1, 10, 1400000, N'Đóng kỳ 1 WPF'),
 (7, 1, 10, 1000000, N'Đóng kỳ 1 SQL');

 INSERT INTO BuoiHoc (LopHocId, SoThuTu, ThoiGianBatDau, ThoiGianKetThuc, ChuDe)
VALUES
 (3, 1, '2025-09-15 19:00', '2025-09-15 21:00', N'Giới thiệu C#'),
 (4, 1, '2025-09-16 19:00', '2025-09-16 21:00', N'Giới thiệu C# nâng cao'),
 (5, 1, '2025-09-21 08:30', '2025-09-21 11:30', N'Giới thiệu WPF & MVVM'),
 (6, 1, '2025-09-16 19:00', '2025-09-16 21:00', N'Tổng quan SQL Server'),
 (7, 1, '2025-09-18 19:00', '2025-09-18 21:00', N'Giới thiệu ASP.NET Core');


 INSERT INTO DiemDanh (BuoiHocId, HocVienId, CoMat)
VALUES
 (1, 1, 1),
 (2, 3, 1);


--/* -------- 2) THỦ TỤC TẠO NGƯỜI DÙNG -------- */
--CREATE OR ALTER PROCEDURE dbo.usp_NguoiDung_Tao
--    @TaiKhoan NVARCHAR(100),
--    @MatKhau  NVARCHAR(256),
--    @HoTen    NVARCHAR(150) = NULL,
--    @Email    NVARCHAR(150) = NULL,
--    @VaiTro   NVARCHAR(50)  = N'User'
--AS
--BEGIN
--    SET NOCOUNT ON;

--    IF EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE TaiKhoan = @TaiKhoan)
--    BEGIN
--        RAISERROR (N'Tài khoản đã tồn tại.', 16, 1);
--        RETURN;
--    END

--    DECLARE @Salt NVARCHAR(36) = CONVERT(NVARCHAR(36), NEWID());
--    DECLARE @Hash VARBINARY(32) = HASHBYTES('SHA2_256', CONVERT(VARBINARY(4000), @MatKhau + @Salt));

--    INSERT dbo.NguoiDung(TaiKhoan, HoTen, Email, VaiTro, Salt, MatKhauHash)
--    VALUES (@TaiKhoan, @HoTen, @Email, @VaiTro, @Salt, @Hash);
--END
--GO

--/* -------- 3) THỦ TỤC ĐĂNG NHẬP (KIỂM TRA MẬT KHẨU) -------- */
--CREATE OR ALTER PROCEDURE dbo.usp_NguoiDung_DangNhap
--    @TaiKhoan NVARCHAR(100),
--    @MatKhau  NVARCHAR(256)
--AS
--BEGIN
--    SET NOCOUNT ON;

--    DECLARE @Salt NVARCHAR(36);

--    SELECT TOP(1) @Salt = Salt
--    FROM dbo.NguoiDung
--    WHERE TaiKhoan = @TaiKhoan AND TrangThai = 1;

--    IF @Salt IS NULL
--    BEGIN
--        SELECT CAST(0 AS BIT) AS IsAuthenticated;
--        RETURN;
--    END

--    DECLARE @Hash VARBINARY(32) = HASHBYTES('SHA2_256', CONVERT(VARBINARY(4000), @MatKhau + @Salt));

--    IF EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE TaiKhoan = @TaiKhoan AND MatKhauHash = @Hash AND TrangThai = 1)
--    BEGIN
--        SELECT TOP(1)
--               CAST(1 AS BIT) AS IsAuthenticated,
--               Id, TaiKhoan, HoTen, Email, VaiTro
--        FROM dbo.NguoiDung
--        WHERE TaiKhoan = @TaiKhoan;
--    END
--    ELSE
--    BEGIN
--        SELECT CAST(0 AS BIT) AS IsAuthenticated;
--    END
--END
--GO

--/* -------- 4) THỦ TỤC ĐỔI MẬT KHẨU -------- */
--CREATE OR ALTER PROCEDURE dbo.usp_NguoiDung_DoiMatKhau
--    @TaiKhoan   NVARCHAR(100),
--    @MatKhauCu  NVARCHAR(256),
--    @MatKhauMoi NVARCHAR(256)
--AS
--BEGIN
--    SET NOCOUNT ON;

--    DECLARE @Salt NVARCHAR(36);
--    SELECT TOP(1) @Salt = Salt FROM dbo.NguoiDung WHERE TaiKhoan = @TaiKhoan AND TrangThai = 1;

--    IF @Salt IS NULL
--    BEGIN
--        RAISERROR (N'Tài khoản không tồn tại hoặc đã bị khóa.', 16, 1);
--        RETURN;
--    END

--    DECLARE @HashCu VARBINARY(32) = HASHBYTES('SHA2_256', CONVERT(VARBINARY(4000), @MatKhauCu + @Salt));

--    IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE TaiKhoan = @TaiKhoan AND MatKhauHash = @HashCu AND TrangThai = 1)
--    BEGIN
--        RAISERROR (N'Mật khẩu cũ không đúng.', 16, 1);
--        RETURN;
--    END

--    DECLARE @SaltMoi NVARCHAR(36) = CONVERT(NVARCHAR(36), NEWID());
--    DECLARE @HashMoi VARBINARY(32) = HASHBYTES('SHA2_256', CONVERT(VARBINARY(4000), @MatKhauMoi + @SaltMoi));

--    UPDATE dbo.NguoiDung
--    SET Salt = @SaltMoi, MatKhauHash = @HashMoi
--    WHERE TaiKhoan = @TaiKhoan;
--END
--GO

--/* -------- 5) SEED 1 TÀI KHOẢN ADMIN (admin / 123456) -------- */
--IF NOT EXISTS (SELECT 1 FROM dbo.NguoiDung WHERE TaiKhoan = N'admin')
--BEGIN
--    DECLARE @s NVARCHAR(36) = CONVERT(NVARCHAR(36), NEWID());
--    DECLARE @h VARBINARY(32) = HASHBYTES('SHA2_256', CONVERT(VARBINARY(4000), N'123456' + @s));
--    INSERT dbo.NguoiDung (TaiKhoan, HoTen, Email, VaiTro, Salt, MatKhauHash, TrangThai)
--    VALUES (N'admin', N'Quản trị', N'admin@example.com', N'Admin', @s, @h, 1);
--END
--GO