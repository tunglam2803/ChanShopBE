IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(max) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Products] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [CategoryId] int NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Products_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] ON;
INSERT INTO [Categories] ([Id], [Name])
VALUES (1, N'Áo Nam'),
(2, N'Quần Tây');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CategoryId', N'Description', N'Name', N'Price') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] ON;
INSERT INTO [Products] ([Id], [CategoryId], [Description], [Name], [Price])
VALUES (1, 1, N'Sơ mi cao cấp', N'Áo Sơ mi Trắng', 250000.0),
(2, 2, N'Quần tôn dáng', N'Quần Âu Slimfit', 350000.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CategoryId', N'Description', N'Name', N'Price') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] OFF;
GO

CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260427112656_InitialCreate', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[Users].[Role]', N'Email', N'COLUMN';
GO

ALTER TABLE [Users] ADD [RefreshToken] nvarchar(max) NULL;
GO

ALTER TABLE [Users] ADD [RoleId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Users] ADD [TokenExpires] datetime2 NULL;
GO

CREATE TABLE [Carts] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Carts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Carts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [OrderDate] datetime2 NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [ProductImages] (
    [Id] int NOT NULL IDENTITY,
    [Url] nvarchar(max) NOT NULL,
    [ProductId] int NOT NULL,
    CONSTRAINT [PK_ProductImages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProductImages_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [CartItems] (
    [Id] int NOT NULL IDENTITY,
    [CartId] int NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    CONSTRAINT [PK_CartItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CartItems_Carts_CartId] FOREIGN KEY ([CartId]) REFERENCES [Carts] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CartItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [OrderItems] (
    [Id] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    [PriceAtPurchase] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);
GO

CREATE INDEX [IX_CartItems_CartId] ON [CartItems] ([CartId]);
GO

CREATE INDEX [IX_CartItems_ProductId] ON [CartItems] ([ProductId]);
GO

CREATE INDEX [IX_Carts_UserId] ON [Carts] ([UserId]);
GO

CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
GO

CREATE INDEX [IX_OrderItems_ProductId] ON [OrderItems] ([ProductId]);
GO

CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId]);
GO

CREATE INDEX [IX_ProductImages_ProductId] ON [ProductImages] ([ProductId]);
GO

ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260427143508_InitialDb', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([Id], [Name])
VALUES (1, N'Admin'),
(2, N'User');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260427144923_SeedRolesFix', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DELETE FROM [Roles]
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

ALTER TABLE [Products] ADD [ImageUrl] nvarchar(max) NULL;
GO

ALTER TABLE [Products] ADD [StockQuantity] int NOT NULL DEFAULT 0;
GO

UPDATE [Categories] SET [Name] = N'Quần '
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] ON;
INSERT INTO [Categories] ([Id], [Name])
VALUES (3, N'Phụ Kiện');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name') AND [object_id] = OBJECT_ID(N'[Categories]'))
    SET IDENTITY_INSERT [Categories] OFF;
GO

UPDATE [Products] SET [Description] = N'Hàng cao cấp chữ nổi cực nét', [ImageUrl] = N'/images/0609905f-e19d-4626-90b9-19b39fc25169.jpg', [Name] = N'Áo LV xám chữ nổi', [Price] = 1250000.0, [StockQuantity] = 50
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Products] SET [CategoryId] = 1, [Description] = N'Phong cách boy phố trẻ trung', [ImageUrl] = N'/images/356b1fa5-1389-4d4c-91bf-47927aad56c8.jpg', [Name] = N'Áo len tay dài boy phố', [Price] = 450000.0, [StockQuantity] = 30
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CategoryId', N'Description', N'ImageUrl', N'Name', N'Price', N'StockQuantity') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] ON;
INSERT INTO [Products] ([Id], [CategoryId], [Description], [ImageUrl], [Name], [Price], [StockQuantity])
VALUES (3, 1, N'Sơ mi họa tiết sang chảnh', N'/images/c8835fcc-32a7-4902-a0a4-99a27087baa5.jpg', N'Áo sơ mi Gucci', 950000.0, 20),
(4, 1, N'Thương hiệu Mikenko chính hãng', N'/images/cf7bc697-39ad-44f7-b9d3-97629d60d46a.jpg', N'Áo Mikenko đen hình nổi', 550000.0, 45),
(5, 1, N'Phiên bản giới hạn cho fan MU', N'/images/9bab3251-da03-4fd3-973f-ead6791e7973.jpg', N'Áo Adidas Man United bản đặc biệt', 850000.0, 15),
(8, 2, N'Chất jean co giãn tốt', N'/images/0e66918c-3e38-48e0-a528-01d436e997fe.jpg', N'Quần jean đen trơn', 380000.0, 100),
(9, 2, N'Phong cách bụi bặm', N'/images/f0b1ebd7-133d-4596-868b-ec80974961f6.jpg', N'Quần jean rách gối vẩy sơn', 420000.0, 40),
(10, 2, N'Dáng suông thoải mái', N'/images/2ae057f9-3f68-4549-bf1a-67945ab121fc.jpg', N'Quần ống rộng túi hộp', 350000.0, 60),
(6, 3, N'Balo Jordan thời trang', N'/images/c17f15fa-5691-4a80-acad-46e77a7e70ac.jpg', N'Balo JD Trắng', 650000.0, 25),
(7, 3, N'Họa tiết monogram kinh điển', N'/images/ed492643-cecc-4871-ab43-c377e059fc40.jpg', N'Balo LV Nâu', 1500000.0, 10);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CategoryId', N'Description', N'ImageUrl', N'Name', N'Price', N'StockQuantity') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260428094423_UpdateFullData', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Contacts] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Contacts] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260428102434_InitialContactAndFixAllDecimals', N'8.0.0');
GO

COMMIT;
GO

USE CuaHangQuanAo;
GO

-- Xem tất cả admin user
SELECT Id, Username, Email, RoleId, TokenExpires FROM Users WHERE Username = 'admin';
GO

-- Xóa tất cả admin cũ, chỉ giữ cái Id lớn nhất (mới nhất)
DELETE FROM Users 
WHERE Username = 'admin' 
AND Id NOT IN (SELECT MAX(Id) FROM Users WHERE Username = 'admin');
GO

-- Xác nhận còn 1 admin duy nhất
SELECT Id, Username, Email, RoleId, TokenExpires FROM Users WHERE Username = 'admin';
GO