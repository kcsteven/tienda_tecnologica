USE [TiendaTecnologicaDB];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @IdentityInsertTabla sysname = NULL;

    IF OBJECT_ID(N'dbo.Categoria', N'U') IS NULL
    BEGIN
        THROW 52001, N'No existe la tabla dbo.Categoria.', 1;
    END;
    IF OBJECT_ID(N'dbo.Subcategoria', N'U') IS NULL
    BEGIN
        THROW 52002, N'No existe la tabla dbo.Subcategoria.', 1;
    END;
    IF OBJECT_ID(N'dbo.Marca', N'U') IS NULL
    BEGIN
        THROW 52003, N'No existe la tabla dbo.Marca.', 1;
    END;
    IF OBJECT_ID(N'dbo.Proveedor', N'U') IS NULL
    BEGIN
        THROW 52004, N'No existe la tabla dbo.Proveedor.', 1;
    END;
    IF OBJECT_ID(N'dbo.Producto', N'U') IS NULL
    BEGIN
        THROW 52005, N'No existe la tabla dbo.Producto.', 1;
    END;
    IF OBJECT_ID(N'dbo.ImagenProducto', N'U') IS NULL
    BEGIN
        THROW 52006, N'No existe la tabla dbo.ImagenProducto.', 1;
    END;
    IF OBJECT_ID(N'dbo.Garantia', N'U') IS NULL
    BEGIN
        THROW 52007, N'No existe la tabla dbo.Garantia.', 1;
    END;
    IF OBJECT_ID(N'dbo.ProductoGarantia', N'U') IS NULL
    BEGIN
        THROW 52008, N'No existe la tabla dbo.ProductoGarantia.', 1;
    END;
    IF OBJECT_ID(N'dbo.Bodega', N'U') IS NULL
    BEGIN
        THROW 52009, N'No existe la tabla dbo.Bodega.', 1;
    END;
    IF OBJECT_ID(N'dbo.Inventario', N'U') IS NULL
    BEGIN
        THROW 52010, N'No existe la tabla dbo.Inventario.', 1;
    END;

    IF OBJECT_ID(N'dbo.EspecificacionProducto', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.EspecificacionProducto
        (
            EspecificacionId int IDENTITY(1,1) NOT NULL
                CONSTRAINT PK_EspecificacionProducto PRIMARY KEY,
            ProductoId int NOT NULL,
            Etiqueta varchar(50) NOT NULL,
            Valor varchar(200) NOT NULL,
            Orden int NOT NULL
                CONSTRAINT DF_EspecificacionProducto_Orden DEFAULT (0),
            CONSTRAINT FK_EspecificacionProducto_Producto
                FOREIGN KEY (ProductoId) REFERENCES dbo.Producto (ProductoId)
        );
    END;

    IF EXISTS (SELECT 1 FROM dbo.Categoria)
    BEGIN
        THROW 52101, N'La tabla dbo.Categoria ya contiene datos.', 1;
    END;
    IF EXISTS (SELECT 1 FROM dbo.Subcategoria)
    BEGIN
        THROW 52102, N'La tabla dbo.Subcategoria ya contiene datos.', 1;
    END;
    IF EXISTS (SELECT 1 FROM dbo.Marca)
    BEGIN
        THROW 52103, N'La tabla dbo.Marca ya contiene datos.', 1;
    END;
    IF EXISTS (SELECT 1 FROM dbo.Proveedor)
    BEGIN
        THROW 52104, N'La tabla dbo.Proveedor ya contiene datos.', 1;
    END;
    IF EXISTS (SELECT 1 FROM dbo.Producto)
    BEGIN
        THROW 52105, N'La tabla dbo.Producto ya contiene datos.', 1;
    END;
    IF EXISTS (SELECT 1 FROM dbo.ImagenProducto)
    BEGIN
        THROW 52106, N'La tabla dbo.ImagenProducto ya contiene datos.', 1;
    END;
    IF EXISTS (SELECT 1 FROM dbo.EspecificacionProducto)
    BEGIN
        THROW 52107, N'La tabla dbo.EspecificacionProducto ya contiene datos.', 1;
    END;
    IF EXISTS (SELECT 1 FROM dbo.Garantia)
    BEGIN
        THROW 52108, N'La tabla dbo.Garantia ya contiene datos.', 1;
    END;
    IF EXISTS (SELECT 1 FROM dbo.ProductoGarantia)
    BEGIN
        THROW 52109, N'La tabla dbo.ProductoGarantia ya contiene datos.', 1;
    END;
    IF EXISTS (SELECT 1 FROM dbo.Bodega)
    BEGIN
        THROW 52110, N'La tabla dbo.Bodega ya contiene datos.', 1;
    END;
    IF EXISTS (SELECT 1 FROM dbo.Inventario)
    BEGIN
        THROW 52111, N'La tabla dbo.Inventario ya contiene datos.', 1;
    END;

    SET IDENTITY_INSERT dbo.Categoria ON;
    SET @IdentityInsertTabla = N'dbo.Categoria';
    INSERT [dbo].[Categoria] ([CategoriaId], [Nombre], [Descripcion], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (1, N'Laptops', N'Computadoras portátiles', 1, CAST(N'2026-07-27T00:25:36.983' AS DateTime), N'admin', NULL, NULL)
    INSERT [dbo].[Categoria] ([CategoriaId], [Nombre], [Descripcion], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (2, N'Celulares', N'Teléfonos inteligentes', 1, CAST(N'2026-08-01T16:06:13.660' AS DateTime), NULL, NULL, NULL)
    INSERT [dbo].[Categoria] ([CategoriaId], [Nombre], [Descripcion], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (3, N'Audífonos', N'Audífonos y auriculares', 1, CAST(N'2026-08-01T16:06:13.660' AS DateTime), NULL, NULL, NULL)
    INSERT [dbo].[Categoria] ([CategoriaId], [Nombre], [Descripcion], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (5, N'Periféricos', N'Accesorios de computadora', 1, CAST(N'2026-08-01T16:06:13.660' AS DateTime), NULL, NULL, NULL)
    SET IDENTITY_INSERT dbo.Categoria OFF;
    SET @IdentityInsertTabla = NULL;

    SET IDENTITY_INSERT dbo.Subcategoria ON;
    SET @IdentityInsertTabla = N'dbo.Subcategoria';
    INSERT [dbo].[Subcategoria] ([SubcategoriaId], [CategoriaId], [Nombre], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (1, 2, N'Smartphones', 1, CAST(N'2026-08-01T18:05:12.0970000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Subcategoria] ([SubcategoriaId], [CategoriaId], [Nombre], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (2, 3, N'Audífonos', 1, CAST(N'2026-08-01T18:05:12.0970000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Subcategoria] ([SubcategoriaId], [CategoriaId], [Nombre], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (4, 5, N'Mouse', 1, CAST(N'2026-08-01T18:05:12.0970000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Subcategoria] ([SubcategoriaId], [CategoriaId], [Nombre], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (5, 5, N'Teclado', 1, CAST(N'2026-08-01T18:05:12.0970000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Subcategoria] ([SubcategoriaId], [CategoriaId], [Nombre], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (11, 1, N'Laptops', 1, CAST(N'2026-08-01T18:05:12.0970000' AS DateTime2), NULL, NULL, NULL)
    SET IDENTITY_INSERT dbo.Subcategoria OFF;
    SET @IdentityInsertTabla = NULL;

    SET IDENTITY_INSERT dbo.Marca ON;
    SET @IdentityInsertTabla = N'dbo.Marca';
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (1, N'Lenovo', N'China', 1, CAST(N'2026-08-01T18:05:12.1160000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (2, N'ASUS', N'Taiwán', 1, CAST(N'2026-08-01T18:05:12.1160000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (3, N'Acer', N'Taiwán', 1, CAST(N'2026-08-02T16:36:49.1950000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (4, N'HP', N'Estados Unidos', 1, CAST(N'2026-08-02T16:36:49.1950000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (5, N'Dynabook', N'Japón', 1, CAST(N'2026-08-02T18:25:44.2290000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (6, N'Dell', N'Estados Unidos', 1, CAST(N'2026-08-02T18:25:44.2310000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (7, N'MSI', N'Taiwán', 1, CAST(N'2026-08-03T19:33:09.3620000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (8, N'Gigabyte', N'Taiwán', 1, CAST(N'2026-08-03T19:39:10.6230000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (9, N'Apple', N'Estados Unidos', 1, CAST(N'2026-08-03T19:42:31.2090000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (10, N'Honor', N'China', 1, CAST(N'2026-08-04T21:23:56.1090000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (11, N'Xiaomi', N'China', 1, CAST(N'2026-08-04T21:34:01.1860000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (12, N'Realme', N'China', 1, CAST(N'2026-08-04T21:38:23.6470000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (13, N'Samsung', N'Corea del Sur', 1, CAST(N'2026-08-04T21:40:53.5410000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (14, N'Infinix', N'China', 1, CAST(N'2026-08-04T21:46:31.6730000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (15, N'Oppo', N'China', 1, CAST(N'2026-08-04T21:49:13.7740000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (16, N'Nothing', N'Reino Unido', 1, CAST(N'2026-08-04T21:54:22.6280000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (17, N'RedMagic', N'China', 1, CAST(N'2026-08-04T21:57:07.7790000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (18, N'Skullcandy', N'Estados Unidos', 1, CAST(N'2026-08-04T22:02:24.9480000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (19, N'Maxell', N'Japón', 1, CAST(N'2026-08-04T22:06:42.7570000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (20, N'SoundCore', N'China', 1, CAST(N'2026-08-04T22:08:36.2160000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (21, N'Energy Sistem', N'España', 1, CAST(N'2026-08-05T12:51:22.3390000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (22, N'UNNO TEKNO', N'Estados Unidos', 1, CAST(N'2026-08-05T13:03:26.9430000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (23, N'Xtech', N'Estados Unidos', 1, CAST(N'2026-08-05T15:31:28.6450000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (24, N'Argom', N'Estados Unidos', 1, CAST(N'2026-08-05T15:34:12.7540000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (25, N'Logitech', N'Suiza', 1, CAST(N'2026-08-05T15:37:23.5880000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (26, N'Razer', N'Estados Unidos', 1, CAST(N'2026-08-05T15:46:22.1200000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (27, N'Corsair', N'Estados Unidos', 1, CAST(N'2026-08-05T15:50:23.4680000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (28, N'SteelSeries', N'Dinamarca', 1, CAST(N'2026-08-05T15:52:53.5770000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Marca] ([MarcaId], [Nombre], [PaisOrigen], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (29, N'Ducky', N'Taiwán', 1, CAST(N'2026-08-05T17:00:00.0140000' AS DateTime2), NULL, NULL, NULL)
    SET IDENTITY_INSERT dbo.Marca OFF;
    SET @IdentityInsertTabla = NULL;

    SET IDENTITY_INSERT dbo.Proveedor ON;
    SET @IdentityInsertTabla = N'dbo.Proveedor';
    INSERT [dbo].[Proveedor] ([ProveedorId], [Nombre], [Telefono], [Email], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (1, N'Distribuidora CR', N'2222-2222', N'ventas@distcr.com', 1, CAST(N'2026-08-01T18:05:12.1210000' AS DateTime2), NULL, NULL, NULL)
    SET IDENTITY_INSERT dbo.Proveedor OFF;
    SET @IdentityInsertTabla = NULL;

    SET IDENTITY_INSERT dbo.Garantia ON;
    SET @IdentityInsertTabla = N'dbo.Garantia';
    INSERT [dbo].[Garantia] ([GarantiaId], [Nombre], [Meses], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (1, N'Garantía estándar', 12, 1, CAST(N'2026-08-03T21:40:15.9790000' AS DateTime2), NULL, NULL, NULL)
    SET IDENTITY_INSERT dbo.Garantia OFF;
    SET @IdentityInsertTabla = NULL;

    SET IDENTITY_INSERT dbo.Producto ON;
    SET @IdentityInsertTabla = N'dbo.Producto';
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (1, N'Lenovo IdeaPad Slim 3 15IAN8', N'15.6" FHD - Intel - 8GB RAM', CAST(199000.00 AS Decimal(10, 2)), CAST(150000.00 AS Decimal(10, 2)), 11, 1, 1, 1, CAST(N'2026-08-01T18:05:12.1100000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (2, N'ASUS VivoBook - Ryzen 3 3250 - 15.6" - FHD - 8GB', N'Laptop ASUS VivoBook con procesador Ryzen 3, 8GB RAM, pantalla FHD', CAST(214900.00 AS Decimal(10, 2)), CAST(170000.00 AS Decimal(10, 2)), 11, 2, 1, 1, CAST(N'2026-08-02T16:23:52.6560000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (3, N'Acer Aspire Go 15 - i5 120U', N'Laptop Acer Aspire Go 15 con procesador Intel Core i5 120U', CAST(229000.00 AS Decimal(10, 2)), CAST(180000.00 AS Decimal(10, 2)), 11, 3, 1, 1, CAST(N'2026-08-02T16:36:49.1950000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (4, N'HP 15t-fd000 - 15.6" - I7 1355U', N'Laptop HP 15t-fd000 con procesador Intel Core i7 1355U, pantalla 15.6"', CAST(349000.00 AS Decimal(10, 2)), CAST(280000.00 AS Decimal(10, 2)), 11, 4, 1, 1, CAST(N'2026-08-02T16:36:49.1960000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (5, N'Dynabook Satellite Pro C50-K - i7 1355U', N'Laptop Dynabook Satellite Pro C50-K con procesador Intel Core i7 1355U', CAST(319000.00 AS Decimal(10, 2)), CAST(255000.00 AS Decimal(10, 2)), 11, 5, 1, 1, CAST(N'2026-08-02T18:25:44.2310000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (6, N'Dell 15 DC15250 - 15.6 Pulgadas - FHD - 120Hz - Intel Core I5 1334U', N'Laptop Dell 15 DC15250 con procesador Intel Core i5 1334U, pantalla FHD 120Hz', CAST(289000.00 AS Decimal(10, 2)), CAST(230000.00 AS Decimal(10, 2)), 11, 6, 1, 1, CAST(N'2026-08-02T18:25:44.2320000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (7, N'MSI Thin A15 B7UC - Ryzen 7 7735HS', N'Laptop gaming MSI Thin A15 con procesador AMD Ryzen 7 7735HS (8 núcleos), gráfica NVIDIA GeForce RTX 3050 4GB, pantalla 15.6" FHD 144Hz, 16GB RAM DDR5, SSD 512GB', CAST(549000.00 AS Decimal(10, 2)), CAST(440000.00 AS Decimal(10, 2)), 11, 7, 1, 1, CAST(N'2026-08-03T19:33:09.3650000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (8, N'Gigabyte Gaming A16 - R7 260 - 16"', N'Laptop gaming Gigabyte Gaming A16 con procesador AMD Ryzen 7 260 (8 núcleos, hasta 5.1GHz), pantalla 16" IPS, gráfica dedicada NVIDIA GeForce RTX', CAST(999000.00 AS Decimal(10, 2)), CAST(800000.00 AS Decimal(10, 2)), 11, 8, 1, 1, CAST(N'2026-08-03T19:39:10.6240000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (9, N'Apple MacBook Air 13" - Chip M4', N'MacBook Air de 13 pulgadas con chip M4 de Apple (CPU 10 núcleos, GPU 8 núcleos), pantalla Liquid Retina, hasta 18 horas de batería, 256GB SSD', CAST(749000.00 AS Decimal(10, 2)), CAST(600000.00 AS Decimal(10, 2)), 11, 9, 1, 1, CAST(N'2026-08-03T19:42:31.2090000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (10, N'HP Omen Max 16 - Intel Core Ultra 9 275HX - 16" - WUXGA - IPS', N'Laptop gaming de gama alta HP Omen Max 16 con procesador Intel Core Ultra 9 275HX (24 núcleos), pantalla 16" WUXGA IPS 165Hz, gráfica dedicada NVIDIA GeForce RTX, hasta 64GB RAM DDR5', CAST(1590000.00 AS Decimal(10, 2)), CAST(1300000.00 AS Decimal(10, 2)), 11, 4, 1, 1, CAST(N'2026-08-03T19:46:17.4050000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (12, N'Honor Play10 – Dual Sim – 3GB – 64GB – Negro Medianoche', N'Smartphone Dual Sim, 3GB RAM, 64GB de almacenamiento, color Negro Medianoche', CAST(94900.00 AS Decimal(10, 2)), CAST(68000.00 AS Decimal(10, 2)), 1, 10, 1, 1, CAST(N'2026-08-04T21:24:11.5340000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (13, N'Xiaomi POCO C81 PRO – 4GB – 64GB – Negro', N'Smartphone 4GB RAM, 64GB de almacenamiento, color Negro', CAST(79000.00 AS Decimal(10, 2)), CAST(58000.00 AS Decimal(10, 2)), 1, 11, 1, 1, CAST(N'2026-08-04T21:34:15.0080000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (14, N'Realme C21Y – 4GB – 64GB – Cross Black', N'Smartphone 4GB RAM, 64GB de almacenamiento, color Cross Black', CAST(69900.00 AS Decimal(10, 2)), CAST(50000.00 AS Decimal(10, 2)), 1, 12, 1, 1, CAST(N'2026-08-04T21:38:41.0730000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (15, N'Samsung A07 – Verde – 4GB – 128GB', N'Smartphone 4GB RAM, 128GB de almacenamiento, color Verde', CAST(119900.00 AS Decimal(10, 2)), CAST(88000.00 AS Decimal(10, 2)), 1, 13, 1, 1, CAST(N'2026-08-04T21:41:01.3590000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (16, N'Xiaomi Redmi Note 15 Pro – 12GB – 512GB – Gris Titanio', N'Smartphone 12GB RAM, 512GB de almacenamiento, color Gris Titanio', CAST(249900.00 AS Decimal(10, 2)), CAST(185000.00 AS Decimal(10, 2)), 1, 11, 1, 1, CAST(N'2026-08-04T21:43:59.6210000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (17, N'Infinix Note 60 – 8GB – 256GB – Mist Titanium', N'Smartphone 8GB RAM, 256GB de almacenamiento, color Mist Titanium', CAST(159900.00 AS Decimal(10, 2)), CAST(118000.00 AS Decimal(10, 2)), 1, 14, 1, 1, CAST(N'2026-08-04T21:46:31.6740000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (18, N'Oppo Reno13 F 5G – 12GB – 256GB – Morado', N'Smartphone 5G, 12GB RAM, 256GB de almacenamiento, color Morado', CAST(199900.00 AS Decimal(10, 2)), CAST(148000.00 AS Decimal(10, 2)), 1, 15, 1, 1, CAST(N'2026-08-04T21:49:13.7750000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (19, N'Honor Magic8 Lite – Dual SIM – 8GB – 256GB – Forest Green', N'Smartphone Dual SIM, 8GB RAM, 256GB de almacenamiento, color Forest Green', CAST(179900.00 AS Decimal(10, 2)), CAST(132000.00 AS Decimal(10, 2)), 1, 10, 1, 1, CAST(N'2026-08-04T21:51:10.9490000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (20, N'Nothing 4a – 8GB – 128GB – Silver', N'Smartphone 8GB RAM, 128GB de almacenamiento, color Silver', CAST(139900.00 AS Decimal(10, 2)), CAST(103000.00 AS Decimal(10, 2)), 1, 16, 1, 1, CAST(N'2026-08-04T21:54:32.2960000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (21, N'RedMagic 11 Air – 16GB – 512GB – Prism', N'Smartphone gaming 16GB RAM, 512GB de almacenamiento, color Prism', CAST(329900.00 AS Decimal(10, 2)), CAST(245000.00 AS Decimal(10, 2)), 1, 17, 1, 1, CAST(N'2026-08-04T21:57:07.7800000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (22, N'Skullcandy JIB – USB C – Blanco – S2JMY', N'Audífonos con conexión USB-C, color Blanco, modelo S2JMY', CAST(9900.00 AS Decimal(10, 2)), CAST(6500.00 AS Decimal(10, 2)), 2, 18, 1, 1, CAST(N'2026-08-04T22:02:24.9500000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (23, N'Maxell ELIO – Wireless – Negro/Gris – EB-BTTWS23', N'Audífonos inalámbricos, color Negro/Gris, modelo EB-BTTWS23', CAST(14900.00 AS Decimal(10, 2)), CAST(10000.00 AS Decimal(10, 2)), 2, 19, 1, 1, CAST(N'2026-08-04T22:06:42.7570000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (24, N'SoundCore Audífonos K20i – Negro', N'Audífonos inalámbricos, color Negro, modelo K20i', CAST(11900.00 AS Decimal(10, 2)), CAST(8000.00 AS Decimal(10, 2)), 2, 20, 1, 1, CAST(N'2026-08-04T22:08:36.2170000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (25, N'Energy Sistem StreetMusic – Rosado', N'Audífonos inalámbricos Energy Sistem StreetMusic, color rosado, con sonido de alta calidad y diseño ligero para uso diario.', CAST(14900.00 AS Decimal(10, 2)), CAST(9800.00 AS Decimal(10, 2)), 2, 21, 1, 1, CAST(N'2026-08-05T12:51:22.3400000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (26, N'Energy Sistem Style Space – Bluetooth – Gris – 490042', N'Audífonos inalámbricos Bluetooth Energy Sistem Style Space, color gris, diseño ergonómico y sonido de alta fidelidad para uso diario.', CAST(18900.00 AS Decimal(10, 2)), CAST(12600.00 AS Decimal(10, 2)), 2, 21, 1, 1, CAST(N'2026-08-05T12:59:57.2660000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (27, N'Audífonos UNNO TEKNO ZEN AIR – Blanco – HS7515WT', N'Audífonos inalámbricos UNNO TEKNO ZEN AIR con tecnología Bluetooth, color blanco, diseño compacto y sonido nítido para uso diario.', CAST(17900.00 AS Decimal(10, 2)), CAST(11800.00 AS Decimal(10, 2)), 2, 22, 1, 1, CAST(N'2026-08-05T13:03:26.9440000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (28, N'Mouse UNNO TEKNO Trans Optical – Negro', N'Mouse óptico USB UNNO TEKNO Trans Optical, color negro, diseño ergonómico y sensor de alta precisión para uso diario.', CAST(6900.00 AS Decimal(10, 2)), CAST(4300.00 AS Decimal(10, 2)), 4, 22, 1, 1, CAST(N'2026-08-05T14:07:51.4190000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (29, N'Mouse Xtech Galos – Negro Azul', N'Mouse inalámbrico Xtech Galos con sensor óptico, diseño ambidiestro y resolución ajustable de hasta 1600 DPI. Ideal para oficina, estudio y uso diario.', CAST(5900.00 AS Decimal(10, 2)), CAST(3600.00 AS Decimal(10, 2)), 4, 23, 1, 1, CAST(N'2026-08-05T15:31:28.6460000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (30, N'Mouse Argom MS32 – Rojo', N'Mouse inalámbrico Argom MS32 color rojo con conexión de 2.4 GHz, diseño ergonómico de 6 botones y resolución ajustable para un control preciso.', CAST(4900.00 AS Decimal(10, 2)), CAST(3100.00 AS Decimal(10, 2)), 4, 24, 1, 1, CAST(N'2026-08-05T15:34:12.7540000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (31, N'Mouse Logitech G203 LIGHTSYNC RGB – Blanco', N'Mouse gamer con cable Logitech G203 LIGHTSYNC RGB, sensor óptico de alta precisión de hasta 8000 DPI, iluminación RGB personalizable y diseño de 6 botones para un rendimiento preciso en juegos.', CAST(16900.00 AS Decimal(10, 2)), CAST(11800.00 AS Decimal(10, 2)), 4, 25, 1, 1, CAST(N'2026-08-05T15:37:23.5880000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (32, N'Mouse Razer DeathAdder Essential – Negro', N'Mouse gaming ergonómico Razer DeathAdder Essential con sensor óptico de hasta 6400 DPI, cinco botones programables, switches mecánicos Razer y conexión USB cableada. Diseñado para jugadores que buscan precisión, comodidad y durabilidad.', CAST(19900.00 AS Decimal(10, 2)), CAST(13000.00 AS Decimal(10, 2)), 4, 26, 1, 1, CAST(N'2026-08-05T15:46:22.1210000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (33, N'Mouse Corsair Harpoon RGB Pro – Negro', N'Mouse gaming Corsair Harpoon RGB Pro con diseño ergonómico, sensor óptico de 12000 DPI, seis botones programables, iluminación RGB y conexión USB cableada. Diseñado para juegos FPS y MOBA ofreciendo precisión, comodidad y respuesta rápida.', CAST(24900.00 AS Decimal(10, 2)), CAST(16500.00 AS Decimal(10, 2)), 4, 27, 1, 1, CAST(N'2026-08-05T15:50:23.4690000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (34, N'Mouse SteelSeries Rival 5 – Negro', N'Mouse gaming SteelSeries Rival 5 con sensor óptico TrueMove Air de 18000 CPI, diseño ergonómico para diestros, nueve botones programables, iluminación RGB PrismSync y switches mecánicos de alta durabilidad. Ideal para juegos FPS, MOBA y Battle Royale.', CAST(39900.00 AS Decimal(10, 2)), CAST(27000.00 AS Decimal(10, 2)), 4, 28, 1, 1, CAST(N'2026-08-05T15:52:53.5780000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (35, N'Mouse Razer Basilisk V3 Pro 35K – Blanco', N'Mouse gaming inalámbrico premium Razer Basilisk V3 Pro 35K con sensor óptico Focus Pro 35K Gen-2, conectividad HyperSpeed Wireless, Bluetooth y USB-C. Cuenta con iluminación Chroma RGB, rueda HyperScroll Tilt y diseño ergonómico para diestros con múltiples botones programables.', CAST(99900.00 AS Decimal(10, 2)), CAST(72000.00 AS Decimal(10, 2)), 4, 26, 1, 1, CAST(N'2026-08-05T15:55:41.3180000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (36, N'Razer Ornata V3X – Inglés', N'Teclado gaming membrana, distribución en inglés', CAST(24900.00 AS Decimal(10, 2)), CAST(17500.00 AS Decimal(10, 2)), 5, 26, 1, 1, CAST(N'2026-08-05T16:07:38.3050000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (37, N'Teclado Corsair K55 Core RGB', N'Teclado gaming Corsair K55 Core RGB con switches de membrana silenciosos, retroiluminación RGB de 10 zonas personalizable mediante Corsair iCUE, controles multimedia dedicados, tasa de sondeo de 1000 Hz y diseño resistente a derrames. Ideal para gaming y uso diario.', CAST(34900.00 AS Decimal(10, 2)), CAST(24500.00 AS Decimal(10, 2)), 5, 27, 1, 1, CAST(N'2026-08-05T16:56:32.8070000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (38, N'Teclado Ducky Project Tinker Barebone Negro', N'Teclado mecánico barebone Ducky Project Tinker en color negro. Cuenta con PCB hot-swap compatible con switches de 3 y 5 pines, estructura gasket mount para una experiencia de escritura más cómoda, iluminación RGB, conexión USB-C desmontable y compatibilidad con QMK/VIA. No incluye switches ni keycaps.', CAST(25000.00 AS Decimal(10, 2)), CAST(18000.00 AS Decimal(10, 2)), 5, 29, 1, 1, CAST(N'2026-08-05T17:00:00.0160000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (39, N'Teclado MSI Forge GK600 TKL Wireless Inglés 75%', N'Teclado gaming mecánico MSI Forge GK600 TKL Wireless con distribución en inglés (ANSI), formato compacto 75%, switches mecánicos lineales hot-swap, conectividad inalámbrica 2.4 GHz, Bluetooth y USB-C, iluminación RGB de 20 efectos, keycaps PBT de alta resistencia y pantalla integrada para visualizar información del teclado.', CAST(59900.00 AS Decimal(10, 2)), CAST(43500.00 AS Decimal(10, 2)), 5, 7, 1, 1, CAST(N'2026-08-05T17:02:05.0460000' AS DateTime2), NULL, NULL, NULL)
    INSERT [dbo].[Producto] ([ProductoId], [Nombre], [Descripcion], [Precio], [CostoCompra], [SubcategoriaId], [MarcaId], [ProveedorId], [Activo], [CreadoEn], [CreadoPor], [ActualizadoEn], [ActualizadoPor]) VALUES (40, N'Teclado SteelSeries Apex Pro TKL Wireless Gen 3 Negro', N'Teclado gaming mecánico inalámbrico SteelSeries Apex Pro TKL Wireless Gen 3 con switches magnéticos OmniPoint 3.0 de accionamiento ajustable, conectividad inalámbrica Quantum 2.0 (2.4 GHz y Bluetooth), pantalla OLED inteligente, iluminación RGB por tecla, Rapid Trigger, Rapid Tap y estructura de aluminio premium. Diseñado para jugadores competitivos que buscan el máximo rendimiento.', CAST(159900.00 AS Decimal(10, 2)), CAST(122000.00 AS Decimal(10, 2)), 5, 28, 1, 1, CAST(N'2026-08-05T17:04:07.3700000' AS DateTime2), NULL, NULL, NULL)
    SET IDENTITY_INSERT dbo.Producto OFF;
    SET @IdentityInsertTabla = NULL;

    SET IDENTITY_INSERT dbo.ImagenProducto ON;
    SET @IdentityInsertTabla = N'dbo.ImagenProducto';
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (4, 1, N'images/productos/lenovo-1.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (5, 2, N'images/productos/asus-vivobook-1.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (6, 3, N'images/productos/acer-aspire-go-1.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (7, 4, N'images/productos/hp-15t-1.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (8, 5, N'images/productos/dynabook-satellite-1.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (9, 6, N'images/productos/dell-15-dc15250-1.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (10, 7, N'images/productos/msi-thin-a15-1.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (11, 8, N'images/productos/gigabyte-gaming-a16-1.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (12, 9, N'images/productos/macbook-air-m4-1.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (13, 10, N'images/productos/hp-omen-max-16-1.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (14, 12, N'images/productos/honor-play10.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (15, 13, N'images/productos/xiaomi-poco-c81-pro.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (16, 14, N'images/productos/realme-c21y.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (17, 15, N'images/productos/samsung-a07.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (18, 16, N'images/productos/xiaomi-redmi-note15-pro.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (19, 17, N'images/productos/infinix-note60.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (20, 18, N'images/productos/oppo-reno13f.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (21, 19, N'images/productos/honor-magic8-lite.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (22, 20, N'images/productos/nothing-4a.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (23, 21, N'images/productos/redmagic-11-air.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (24, 22, N'images/productos/skullcandy-jib-usbc.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (25, 23, N'images/productos/maxell-elio.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (26, 24, N'images/productos/soundcore-k20i.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (27, 25, N'images/productos/energy-streetmusic-rosado.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (28, 26, N'images/productos/energy-style-space-gris-490042.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (29, 27, N'images/productos/unno-tekno-zen-air-blanco-hs7515wt.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (30, 28, N'images/productos/unno-tekno-trans-optical-negro.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (31, 29, N'images/productos/xtech-galos-negro-azul.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (32, 30, N'images/productos/argom-ms32-rojo.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (33, 31, N'images/productos/logitech-g203-lightsync-rgb-blanco.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (34, 32, N'images/productos/razer-deathadder-essential-negro.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (35, 33, N'images/productos/corsair-harpoon-rgb-pro-negro.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (36, 34, N'images/productos/steelseries-rival-5-negro.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (37, 35, N'images/productos/razer-basilisk-v3-pro-35k-blanco.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (38, 36, N'images/productos/razer-ornata-v3x.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (39, 37, N'images/productos/corsair-k55-core-rgb.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (40, 38, N'images/productos/ducky-project-tinker-barebone-negro.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (41, 39, N'images/productos/msi-forge-gk600-tkl-wireless-ingles-75.jpg')
    INSERT [dbo].[ImagenProducto] ([ImagenId], [ProductoId], [RutaImagen]) VALUES (42, 40, N'images/productos/steelseries-apex-pro-tkl-wireless-gen3-negro.jpg')
    SET IDENTITY_INSERT dbo.ImagenProducto OFF;
    SET @IdentityInsertTabla = NULL;

    SET IDENTITY_INSERT dbo.EspecificacionProducto ON;
    SET @IdentityInsertTabla = N'dbo.EspecificacionProducto';
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (1, 1, N'Pantalla', N'15.6 pulgadas – 1920 x 1080 resolución', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (2, 1, N'Procesador', N'Intel Core i3 N305', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (3, 1, N'Memoria', N'8GB DDR5', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (4, 1, N'Gráficos', N'Intel UHD', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (5, 1, N'Disco SSD', N'128GB', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (6, 1, N'Conectividad', N'Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (7, 1, N'Sistema Operativo', N'Windows 11', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (8, 2, N'Pantalla', N'15.6 pulgadas – 1920 x 1080 resolución', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (9, 2, N'Procesador', N'AMD Ryzen 3 3250', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (10, 2, N'Memoria', N'8GB DDR4', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (11, 2, N'Gráficos', N'AMD Radeon', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (12, 2, N'Disco SSD', N'512GB', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (13, 2, N'Conectividad', N'Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (14, 2, N'Sistema Operativo', N'Windows 11', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (15, 3, N'Pantalla', N'15.6 pulgadas – 1920 x 1080 resolución', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (16, 3, N'Procesador', N'Intel Core i5-120U', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (17, 3, N'Memoria', N'16GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (18, 3, N'Gráficos', N'Intel Graphics', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (19, 3, N'Disco SSD', N'512GB', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (20, 3, N'Conectividad', N'Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (21, 3, N'Sistema Operativo', N'Windows 11', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (22, 4, N'Pantalla', N'15.6 pulgadas – 1920 x 1080 resolución', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (23, 4, N'Procesador', N'Intel Core i7-1355U', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (24, 4, N'Memoria', N'8GB DDR4', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (25, 4, N'Gráficos', N'Intel Iris Xe', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (26, 4, N'Disco SSD', N'512GB', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (27, 4, N'Conectividad', N'Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (28, 4, N'Sistema Operativo', N'Windows 11', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (29, 5, N'Pantalla', N'15.6 pulgadas – 1920 x 1080 resolución', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (30, 5, N'Procesador', N'Intel Core i7-1355U', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (31, 5, N'Memoria', N'16GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (32, 5, N'Gráficos', N'Intel Iris Xe', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (33, 5, N'Disco SSD', N'512GB', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (34, 5, N'Conectividad', N'Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (35, 5, N'Sistema Operativo', N'Windows 11', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (36, 6, N'Pantalla', N'15.6 pulgadas – FHD – 120Hz', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (37, 6, N'Procesador', N'Intel Core i5-1334U', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (38, 6, N'Memoria', N'16GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (39, 6, N'Gráficos', N'Intel Iris Xe', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (40, 6, N'Disco SSD', N'512GB', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (41, 6, N'Conectividad', N'Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (42, 6, N'Sistema Operativo', N'Windows 11', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (43, 7, N'Pantalla', N'15.6 pulgadas – 144Hz', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (44, 7, N'Procesador', N'AMD Ryzen 7 7735HS', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (45, 7, N'Memoria', N'16GB DDR5', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (46, 7, N'Gráficos', N'NVIDIA GeForce RTX 3050', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (47, 7, N'Disco SSD', N'512GB', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (48, 7, N'Conectividad', N'Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (49, 7, N'Sistema Operativo', N'Windows 11', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (50, 8, N'Pantalla', N'16 pulgadas – 165Hz', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (51, 8, N'Procesador', N'AMD Ryzen AI 7 260', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (52, 8, N'Memoria', N'16GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (53, 8, N'Gráficos', N'NVIDIA GeForce RTX 5050', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (54, 8, N'Disco SSD', N'1TB', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (55, 8, N'Conectividad', N'Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (56, 8, N'Sistema Operativo', N'Windows 11', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (57, 9, N'Pantalla', N'13 pulgadas – Liquid Retina', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (58, 9, N'Procesador', N'Apple M5', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (59, 9, N'Memoria', N'16GB unificada', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (60, 9, N'Gráficos', N'GPU integrada Apple', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (61, 9, N'Disco SSD', N'256GB', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (62, 9, N'Conectividad', N'Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (63, 9, N'Sistema Operativo', N'macOS', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (64, 10, N'Pantalla', N'16 pulgadas – WUXGA – IPS', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (65, 10, N'Procesador', N'Intel Core Ultra 9 275HX', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (66, 10, N'Memoria', N'32GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (67, 10, N'Gráficos', N'NVIDIA GeForce RTX 5080', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (68, 10, N'Disco SSD', N'1TB', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (69, 10, N'Conectividad', N'Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (70, 10, N'Sistema Operativo', N'Windows 11', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (71, 12, N'Pantalla', N'6.56 pulgadas – HD+', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (72, 12, N'Memoria RAM', N'3GB', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (73, 12, N'Almacenamiento', N'64GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (74, 12, N'Cámara', N'Cámara trasera + frontal', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (75, 12, N'Batería', N'5200 mAh', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (76, 12, N'Conectividad', N'Dual Sim – Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (77, 12, N'Color', N'Negro Medianoche', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (78, 13, N'Pantalla', N'6.74 pulgadas – HD+', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (79, 13, N'Memoria RAM', N'4GB', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (80, 13, N'Almacenamiento', N'64GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (81, 13, N'Cámara', N'Cámara trasera + frontal', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (82, 13, N'Batería', N'5160 mAh', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (83, 13, N'Conectividad', N'Dual Sim – Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (84, 13, N'Color', N'Negro', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (85, 14, N'Pantalla', N'6.5 pulgadas – HD+', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (86, 14, N'Memoria RAM', N'4GB', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (87, 14, N'Almacenamiento', N'64GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (88, 14, N'Cámara', N'Cámara trasera + frontal', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (89, 14, N'Batería', N'5000 mAh', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (90, 14, N'Conectividad', N'Dual Sim – Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (91, 14, N'Color', N'Cross Black', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (92, 15, N'Pantalla', N'6.7 pulgadas – HD+', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (93, 15, N'Memoria RAM', N'4GB', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (94, 15, N'Almacenamiento', N'128GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (95, 15, N'Cámara', N'Cámara trasera + frontal', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (96, 15, N'Batería', N'5000 mAh', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (97, 15, N'Conectividad', N'Dual Sim – Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (98, 15, N'Color', N'Verde', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (99, 16, N'Pantalla', N'6.77 pulgadas – AMOLED FHD+', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (100, 16, N'Memoria RAM', N'12GB', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (101, 16, N'Almacenamiento', N'512GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (102, 16, N'Cámara', N'Cámara trasera + frontal', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (103, 16, N'Batería', N'5500 mAh', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (104, 16, N'Conectividad', N'Dual Sim – Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (105, 16, N'Color', N'Gris Titanio', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (106, 17, N'Pantalla', N'6.78 pulgadas – AMOLED FHD+', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (107, 17, N'Memoria RAM', N'8GB', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (108, 17, N'Almacenamiento', N'256GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (109, 17, N'Cámara', N'Cámara trasera + frontal', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (110, 17, N'Batería', N'5200 mAh', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (111, 17, N'Conectividad', N'Dual Sim – Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (112, 17, N'Color', N'Mist Titanium', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (113, 18, N'Pantalla', N'6.67 pulgadas – AMOLED FHD+', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (114, 18, N'Memoria RAM', N'12GB', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (115, 18, N'Almacenamiento', N'256GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (116, 18, N'Cámara', N'Cámara trasera + frontal', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (117, 18, N'Batería', N'5800 mAh', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (118, 18, N'Conectividad', N'Dual Sim – 5G – Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (119, 18, N'Color', N'Morado', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (120, 19, N'Pantalla', N'6.7 pulgadas – AMOLED FHD+', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (121, 19, N'Memoria RAM', N'8GB', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (122, 19, N'Almacenamiento', N'256GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (123, 19, N'Cámara', N'Cámara trasera + frontal', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (124, 19, N'Batería', N'5300 mAh', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (125, 19, N'Conectividad', N'Dual Sim – Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (126, 19, N'Color', N'Forest Green', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (127, 20, N'Pantalla', N'6.77 pulgadas – AMOLED FHD+', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (128, 20, N'Memoria RAM', N'8GB', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (129, 20, N'Almacenamiento', N'128GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (130, 20, N'Cámara', N'Cámara trasera + frontal', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (131, 20, N'Batería', N'5000 mAh', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (132, 20, N'Conectividad', N'Dual Sim – Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (133, 20, N'Color', N'Silver', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (134, 21, N'Pantalla', N'6.8 pulgadas – AMOLED 144Hz', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (135, 21, N'Memoria RAM', N'16GB', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (136, 21, N'Almacenamiento', N'512GB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (137, 21, N'Cámara', N'Cámara trasera + frontal', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (138, 21, N'Batería', N'6500 mAh', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (139, 21, N'Conectividad', N'Dual Sim – 5G – Wi-Fi – Bluetooth', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (140, 21, N'Color', N'Prism', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (141, 22, N'Tipo', N'Auriculares con cable (in-ear)', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (142, 22, N'Conector', N'USB-C', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (143, 22, N'Micrófono', N'Integrado', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (144, 22, N'Controles', N'Botón de llamada y volumen en el cable', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (145, 22, N'Color', N'Blanco', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (146, 22, N'Modelo', N'S2JMY', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (147, 23, N'Tipo', N'Auriculares inalámbricos (in-ear)', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (148, 23, N'Conectividad', N'Bluetooth', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (149, 23, N'Batería', N'Estuche de carga incluido', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (150, 23, N'Micrófono', N'Integrado', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (151, 23, N'Color', N'Negro/Gris', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (152, 23, N'Modelo', N'EB-BTTWS23', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (153, 24, N'Tipo', N'Auriculares inalámbricos (in-ear)', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (154, 24, N'Conectividad', N'Bluetooth', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (155, 24, N'Batería', N'Estuche de carga incluido', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (156, 24, N'Micrófono', N'Integrado', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (157, 24, N'Color', N'Negro', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (158, 24, N'Modelo', N'K20i', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (159, 25, N'Tipo', N'Audífonos inalámbricos', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (160, 25, N'Conectividad', N'Bluetooth 5.0', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (161, 25, N'Batería', N'Hasta 20 horas de reproducción', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (162, 25, N'Micrófono', N'Integrado', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (163, 25, N'Color', N'Rosado', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (164, 25, N'Modelo', N'StreetMusic', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (165, 26, N'Tipo', N'Audífonos inalámbricos', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (166, 26, N'Conectividad', N'Bluetooth 5.0', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (167, 26, N'Batería', N'Hasta 25 horas de reproducción', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (168, 26, N'Micrófono', N'Integrado', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (169, 26, N'Color', N'Gris', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (170, 26, N'Modelo', N'Style Space', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (171, 26, N'Código', N'490042', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (172, 27, N'Tipo', N'Audífonos inalámbricos', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (173, 27, N'Conectividad', N'Bluetooth 5.0', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (174, 27, N'Batería', N'Hasta 20 horas de reproducción', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (175, 27, N'Micrófono', N'Integrado', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (176, 27, N'Color', N'Blanco', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (177, 27, N'Modelo', N'ZEN AIR', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (178, 27, N'Código', N'HS7515WT', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (179, 28, N'Tipo', N'Mouse óptico', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (180, 28, N'Conectividad', N'USB', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (181, 28, N'Sensor', N'Óptico', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (182, 28, N'Botones', N'3 botones con rueda de desplazamiento', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (183, 28, N'Color', N'Negro', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (184, 28, N'Modelo', N'Trans Optical', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (185, 29, N'Tipo', N'Mouse inalámbrico de 4 botones', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (186, 29, N'Tecnología', N'Sensor óptico', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (187, 29, N'Resolución', N'1000 / 1200 / 1600 DPI', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (188, 29, N'Conectividad', N'Inalámbrica 2.4 GHz mediante nano receptor USB', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (189, 29, N'Alcance', N'Hasta 10 metros', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (190, 29, N'Alimentación', N'2 baterías AAA incluidas', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (191, 29, N'Color', N'Negro / Azul', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (192, 29, N'Modelo', N'Galos (XTM-310BL)', 8)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (193, 30, N'Tipo', N'Mouse inalámbrico', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (194, 30, N'Conectividad', N'Inalámbrica 2.4 GHz', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (195, 30, N'Sensor', N'Óptico', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (196, 30, N'Resolución', N'800 / 1600 DPI', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (197, 30, N'Botones', N'6 botones con rueda de desplazamiento', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (198, 30, N'Alimentación', N'2 baterías AAA', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (199, 30, N'Alcance', N'Hasta 10 metros', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (200, 30, N'Color', N'Rojo', 8)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (201, 30, N'Modelo', N'MS32 (ARG-MS-0032R)', 9)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (202, 31, N'Tipo', N'Mouse gamer con cable', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (203, 31, N'Conectividad', N'USB', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (204, 31, N'Sensor', N'Óptico para gaming', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (205, 31, N'Resolución', N'200 - 8000 DPI', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (206, 31, N'Botones', N'6 botones programables', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (207, 31, N'Iluminación', N'RGB LIGHTSYNC personalizable', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (208, 31, N'Frecuencia de respuesta', N'1000 Hz (1 ms)', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (209, 31, N'Color', N'Blanco', 8)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (210, 31, N'Modelo', N'G203 LIGHTSYNC RGB', 9)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (211, 32, N'Tipo', N'Mouse gaming ergonómico con cable', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (212, 32, N'Sensor', N'Sensor óptico Razer 5G Advanced', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (213, 32, N'Resolución', N'Hasta 6400 DPI ajustables', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (214, 32, N'Conectividad', N'USB cableado', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (215, 32, N'Botones', N'5 botones programables', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (216, 32, N'Switches', N'Switches mecánicos Razer con vida útil de hasta 10 millones de clics', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (217, 32, N'Diseño', N'Ergonómico para mano derecha', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (218, 32, N'Color', N'Negro', 8)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (219, 32, N'Modelo', N'RZ01-03850100-R3U1', 9)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (220, 33, N'Tipo', N'Mouse gaming ergonómico con cable', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (221, 33, N'Sensor', N'Sensor óptico de alta precisión', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (222, 33, N'Resolución', N'Hasta 12000 DPI ajustables', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (223, 33, N'Conectividad', N'USB cableado', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (224, 33, N'Botones', N'6 botones programables', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (225, 33, N'Switches', N'Switches Omron con duración de hasta 20 millones de clics', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (226, 33, N'Iluminación', N'RGB dinámico de una zona compatible con Corsair iCUE', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (227, 33, N'Peso', N'Aproximadamente 85 g', 8)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (228, 33, N'Color', N'Negro', 9)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (229, 33, N'Modelo', N'CH-9301111-NA', 10)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (230, 34, N'Tipo', N'Mouse gaming ergonómico con cable', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (231, 34, N'Sensor', N'Sensor óptico SteelSeries TrueMove Air', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (232, 34, N'Resolución', N'Hasta 18000 CPI/DPI ajustables', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (233, 34, N'Conectividad', N'USB cableado con cable Super Mesh', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (234, 34, N'Botones', N'9 botones programables con 5 botones laterales de acción rápida', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (235, 34, N'Switches', N'Switches mecánicos Golden Micro IP54 con duración de hasta 80 millones de clics', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (236, 34, N'Iluminación', N'RGB PrismSync de 10 zonas con 16.8 millones de colores', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (237, 34, N'Peso', N'Aproximadamente 85 gramos', 8)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (238, 34, N'Compatibilidad', N'Windows, macOS y Linux mediante software SteelSeries GG', 9)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (239, 34, N'Color', N'Negro mate', 10)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (240, 34, N'Modelo', N'62551', 11)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (241, 35, N'Tipo', N'Mouse gaming ergonómico inalámbrico', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (242, 35, N'Sensor', N'Razer Focus Pro 35K Optical Sensor Gen-2', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (243, 35, N'Resolución', N'Hasta 35000 DPI ajustables', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (244, 35, N'Conectividad', N'Razer HyperSpeed Wireless 2.4 GHz, Bluetooth y USB-C', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (245, 35, N'Botones', N'11 botones programables', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (246, 35, N'Switches', N'Razer Optical Mouse Switches Gen-3 con duración de hasta 90 millones de clics', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (247, 35, N'Iluminación', N'Razer Chroma RGB multizona', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (248, 35, N'Rueda', N'Razer HyperScroll Tilt Wheel con modos táctil y libre', 8)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (249, 35, N'Peso', N'Aproximadamente 112 gramos', 9)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (250, 35, N'Memoria', N'Memoria interna para hasta 5 perfiles', 10)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (251, 35, N'Color', N'Blanco (Phantom White)', 11)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (252, 35, N'Modelo', N'RZ01-05240200-R3U1', 12)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (253, 36, N'Tipo de switch', N'Membrana mecánica', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (254, 36, N'Iluminación', N'Retroiluminación RGB Chroma', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (255, 36, N'Conexión', N'Cable USB', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (256, 36, N'Distribución', N'Inglés (QWERTY US)', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (257, 36, N'Reposamuñecas', N'Incluido, extraíble', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (258, 36, N'Compatibilidad', N'Windows / consolas (según puerto USB)', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (259, 37, N'Tipo de switch', N'Membrana (Rubber Dome)', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (260, 37, N'Iluminación', N'RGB de 10 zonas personalizable mediante Corsair iCUE', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (261, 37, N'Conexión', N'USB 2.0 cableado', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (262, 37, N'Distribución', N'Inglés (QWERTY US)', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (263, 37, N'Controles multimedia', N'Sí, dedicados', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (264, 37, N'Frecuencia de sondeo', N'1000 Hz', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (265, 37, N'Resistencia', N'Resistente a derrames de hasta 300 ml', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (266, 37, N'Compatibilidad', N'Windows 10/11 y macOS', 8)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (267, 38, N'Tipo de switch', N'Barebone (no incluye switches)', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (268, 38, N'Iluminación', N'RGB personalizable', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (269, 38, N'Conexión', N'USB-C cableado desmontable', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (270, 38, N'Distribución', N'Inglés (ANSI US)', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (271, 38, N'Compatibilidad de switches', N'Hot-swap para switches mecánicos de 3 y 5 pines', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (272, 38, N'Estructura', N'Gasket Mount', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (273, 38, N'Compatibilidad', N'Windows, macOS, Linux (QMK/VIA)', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (274, 39, N'Tipo de switch', N'Mecánico lineal hot-swap (compatible con switches de 5 pines)', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (275, 39, N'Iluminación', N'RGB con 20 efectos personalizables', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (276, 39, N'Conexión', N'2.4 GHz inalámbrico, Bluetooth y USB-C cableado', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (277, 39, N'Distribución', N'Inglés (ANSI US)', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (278, 39, N'Formato', N'75% (83 teclas)', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (279, 39, N'Keycaps', N'PBT sublimadas de alta durabilidad', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (280, 39, N'Pantalla', N'Pantalla integrada de 1.06"', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (281, 39, N'Compatibilidad', N'Windows 10/11 y macOS 11 o superior', 8)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (282, 40, N'Tipo de switch', N'OmniPoint 3.0 HyperMagnetic (Hall Effect)', 1)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (283, 40, N'Iluminación', N'RGB PrismSync por tecla', 2)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (284, 40, N'Conexión', N'2.4 GHz inalámbrico, Bluetooth 5.0 y USB-C cableado', 3)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (285, 40, N'Distribución', N'Inglés (ANSI US)', 4)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (286, 40, N'Formato', N'TKL (80%)', 5)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (287, 40, N'Pantalla', N'OLED Smart Display integrada', 6)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (288, 40, N'Funciones', N'Rapid Trigger, Rapid Tap, Dual Action Keys y accionamiento ajustable de 0.1 a 4.0 mm', 7)
    INSERT [dbo].[EspecificacionProducto] ([EspecificacionId], [ProductoId], [Etiqueta], [Valor], [Orden]) VALUES (289, 40, N'Compatibilidad', N'Windows, macOS, Xbox y PlayStation', 8)
    SET IDENTITY_INSERT dbo.EspecificacionProducto OFF;
    SET @IdentityInsertTabla = NULL;

    SET IDENTITY_INSERT dbo.ProductoGarantia ON;
    SET @IdentityInsertTabla = N'dbo.ProductoGarantia';
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (1, 1, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (2, 2, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (3, 3, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (4, 4, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (5, 5, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (6, 6, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (7, 7, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (8, 8, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (9, 9, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (10, 10, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (11, 12, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (12, 13, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (13, 14, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (14, 15, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (15, 16, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (16, 17, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (17, 18, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (18, 19, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (19, 20, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (20, 21, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (21, 22, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (22, 23, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (23, 24, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (24, 25, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (25, 26, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (26, 27, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (27, 28, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (28, 29, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (29, 30, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (30, 31, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (31, 32, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (32, 33, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (33, 34, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (34, 35, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (35, 36, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (36, 37, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (37, 38, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (38, 39, 1)
    INSERT [dbo].[ProductoGarantia] ([ProductoGarantiaId], [ProductoId], [GarantiaId]) VALUES (39, 40, 1)
    SET IDENTITY_INSERT dbo.ProductoGarantia OFF;
    SET @IdentityInsertTabla = NULL;

    DECLARE @BodegaCreada TABLE (BodegaId int NOT NULL);

    INSERT dbo.Bodega (Nombre, Ubicacion, Activo, CreadoEn)
        OUTPUT INSERTED.BodegaId INTO @BodegaCreada (BodegaId)
        VALUES (N'Bodega Principal', NULL, 1, SYSUTCDATETIME());

    DECLARE @BodegaId int = (SELECT BodegaId FROM @BodegaCreada);

    INSERT dbo.Inventario (ProductoId, BodegaId, Cantidad)
        SELECT ProductoId, @BodegaId, 10
        FROM dbo.Producto;

    IF (SELECT COUNT_BIG(*) FROM dbo.Categoria) <> 4
    BEGIN
        THROW 52201, N'El conteo final de Categoria no es 4.', 1;
    END;
    IF (SELECT COUNT_BIG(*) FROM dbo.Subcategoria) <> 5
    BEGIN
        THROW 52202, N'El conteo final de Subcategoria no es 5.', 1;
    END;
    IF (SELECT COUNT_BIG(*) FROM dbo.Marca) <> 29
    BEGIN
        THROW 52203, N'El conteo final de Marca no es 29.', 1;
    END;
    IF (SELECT COUNT_BIG(*) FROM dbo.Proveedor) <> 1
    BEGIN
        THROW 52204, N'El conteo final de Proveedor no es 1.', 1;
    END;
    IF (SELECT COUNT_BIG(*) FROM dbo.Producto) <> 39
    BEGIN
        THROW 52205, N'El conteo final de Producto no es 39.', 1;
    END;
    IF (SELECT COUNT_BIG(*) FROM dbo.ImagenProducto) <> 39
    BEGIN
        THROW 52206, N'El conteo final de ImagenProducto no es 39.', 1;
    END;
    IF (SELECT COUNT_BIG(*) FROM dbo.EspecificacionProducto) <> 289
    BEGIN
        THROW 52207, N'El conteo final de EspecificacionProducto no es 289.', 1;
    END;
    IF (SELECT COUNT_BIG(*) FROM dbo.Garantia) <> 1
    BEGIN
        THROW 52208, N'El conteo final de Garantia no es 1.', 1;
    END;
    IF (SELECT COUNT_BIG(*) FROM dbo.ProductoGarantia) <> 39
    BEGIN
        THROW 52209, N'El conteo final de ProductoGarantia no es 39.', 1;
    END;
    IF (SELECT COUNT_BIG(*) FROM dbo.Bodega) <> 1
    BEGIN
        THROW 52210, N'El conteo final de Bodega no es 1.', 1;
    END;
    IF (SELECT COUNT_BIG(*) FROM dbo.Inventario) <> 39
    BEGIN
        THROW 52211, N'El conteo final de Inventario no es 39.', 1;
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @IdentityInsertTabla IS NOT NULL
    BEGIN
        BEGIN TRY
            DECLARE @ApagarIdentityInsert nvarchar(300) =
                N'SET IDENTITY_INSERT ' + @IdentityInsertTabla + N' OFF;';
            EXEC sys.sp_executesql @ApagarIdentityInsert;
        END TRY
        BEGIN CATCH
            SET @IdentityInsertTabla = NULL;
        END CATCH;
    END;

    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO
