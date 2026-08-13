/*
    Actualiza el esquema de catalogo de TiendaTecnologicaDB.
    El script no inserta, actualiza ni elimina datos.
*/

USE [TiendaTecnologicaDB];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    /* Validar todas las tablas requeridas antes de modificar el esquema. */
    IF OBJECT_ID(N'dbo.Categoria', N'U') IS NULL
        THROW 52000, N'No existe la tabla requerida dbo.Categoria. No se realizaron cambios.', 1;

    IF OBJECT_ID(N'dbo.Subcategoria', N'U') IS NULL
        THROW 52001, N'No existe la tabla requerida dbo.Subcategoria. No se realizaron cambios.', 1;

    IF OBJECT_ID(N'dbo.Marca', N'U') IS NULL
        THROW 52002, N'No existe la tabla requerida dbo.Marca. No se realizaron cambios.', 1;

    IF OBJECT_ID(N'dbo.Proveedor', N'U') IS NULL
        THROW 52003, N'No existe la tabla requerida dbo.Proveedor. No se realizaron cambios.', 1;

    IF OBJECT_ID(N'dbo.Producto', N'U') IS NULL
        THROW 52004, N'No existe la tabla requerida dbo.Producto. No se realizaron cambios.', 1;

    IF OBJECT_ID(N'dbo.Garantia', N'U') IS NULL
        THROW 52005, N'No existe la tabla requerida dbo.Garantia. No se realizaron cambios.', 1;

    IF OBJECT_ID(N'dbo.Bodega', N'U') IS NULL
        THROW 52006, N'No existe la tabla requerida dbo.Bodega. No se realizaron cambios.', 1;

    /*
       Validar las columnas de auditoria que ya existan. Las columnas ausentes
       se agregan mas adelante; las incompatibles detienen toda la transaccion.
    */
    DECLARE @TablaIncompatible sysname = NULL;
    DECLARE @ColumnaIncompatible sysname = NULL;
    DECLARE @MensajeError nvarchar(2048) = NULL;

    SELECT TOP (1)
        @TablaIncompatible = Esperada.Tabla,
        @ColumnaIncompatible = Esperada.Columna
    FROM
    (
        VALUES
            (N'Categoria',    N'Activo',        104, NULL, NULL, 0),
            (N'Categoria',    N'CreadoEn',       42, NULL,    3, 0),
            (N'Categoria',    N'CreadoPor',     167,   50, NULL, 1),
            (N'Categoria',    N'ActualizadoEn',  42, NULL,    3, 1),
            (N'Categoria',    N'ActualizadoPor',167,   50, NULL, 1),
            (N'Categoria',    N'RowVer',        189, NULL, NULL, 0),
            (N'Subcategoria', N'Activo',        104, NULL, NULL, 0),
            (N'Subcategoria', N'CreadoEn',       42, NULL,    3, 0),
            (N'Subcategoria', N'CreadoPor',     167,   50, NULL, 1),
            (N'Subcategoria', N'ActualizadoEn',  42, NULL,    3, 1),
            (N'Subcategoria', N'ActualizadoPor',167,   50, NULL, 1),
            (N'Subcategoria', N'RowVer',        189, NULL, NULL, 0),
            (N'Marca',        N'Activo',        104, NULL, NULL, 0),
            (N'Marca',        N'CreadoEn',       42, NULL,    3, 0),
            (N'Marca',        N'CreadoPor',     167,   50, NULL, 1),
            (N'Marca',        N'ActualizadoEn',  42, NULL,    3, 1),
            (N'Marca',        N'ActualizadoPor',167,   50, NULL, 1),
            (N'Marca',        N'RowVer',        189, NULL, NULL, 0),
            (N'Proveedor',    N'Activo',        104, NULL, NULL, 0),
            (N'Proveedor',    N'CreadoEn',       42, NULL,    3, 0),
            (N'Proveedor',    N'CreadoPor',     167,   50, NULL, 1),
            (N'Proveedor',    N'ActualizadoEn',  42, NULL,    3, 1),
            (N'Proveedor',    N'ActualizadoPor',167,   50, NULL, 1),
            (N'Proveedor',    N'RowVer',        189, NULL, NULL, 0),
            (N'Producto',     N'Activo',        104, NULL, NULL, 0),
            (N'Producto',     N'CreadoEn',       42, NULL,    3, 0),
            (N'Producto',     N'CreadoPor',     167,   50, NULL, 1),
            (N'Producto',     N'ActualizadoEn',  42, NULL,    3, 1),
            (N'Producto',     N'ActualizadoPor',167,   50, NULL, 1),
            (N'Producto',     N'RowVer',        189, NULL, NULL, 0),
            (N'Garantia',     N'Activo',        104, NULL, NULL, 0),
            (N'Garantia',     N'CreadoEn',       42, NULL,    3, 0),
            (N'Garantia',     N'CreadoPor',     167,   50, NULL, 1),
            (N'Garantia',     N'ActualizadoEn',  42, NULL,    3, 1),
            (N'Garantia',     N'ActualizadoPor',167,   50, NULL, 1),
            (N'Garantia',     N'RowVer',        189, NULL, NULL, 0),
            (N'Bodega',       N'Activo',        104, NULL, NULL, 0),
            (N'Bodega',       N'CreadoEn',       42, NULL,    3, 0),
            (N'Bodega',       N'CreadoPor',     167,   50, NULL, 1),
            (N'Bodega',       N'ActualizadoEn',  42, NULL,    3, 1),
            (N'Bodega',       N'ActualizadoPor',167,   50, NULL, 1),
            (N'Bodega',       N'RowVer',        189, NULL, NULL, 0)
    ) AS Esperada (Tabla, Columna, TipoSistema, LongitudMaxima, Escala, EsNulable)
    LEFT JOIN sys.columns AS Columna
        ON Columna.object_id = OBJECT_ID(N'dbo.' + Esperada.Tabla, N'U')
       AND Columna.name = Esperada.Columna
    WHERE Columna.object_id IS NOT NULL
      AND
      (
          Columna.system_type_id <> Esperada.TipoSistema
          OR (Esperada.LongitudMaxima IS NOT NULL AND Columna.max_length <> Esperada.LongitudMaxima)
          OR (Esperada.Escala IS NOT NULL AND Columna.scale <> Esperada.Escala)
          OR Columna.is_nullable <> Esperada.EsNulable
      )
    ORDER BY Esperada.Tabla, Esperada.Columna;

    IF @TablaIncompatible IS NOT NULL
    BEGIN
        SET @MensajeError = N'La columna existente dbo.' + QUOTENAME(@TablaIncompatible)
            + N'.' + QUOTENAME(@ColumnaIncompatible)
            + N' no tiene el tipo o la nulabilidad esperados. No se realizaron cambios.';
        THROW 52010, @MensajeError, 1;
    END;

    /* Validar defaults existentes para no aceptar valores predeterminados incompatibles. */
    DECLARE @TablaDefaultIncompatible sysname = NULL;
    DECLARE @ColumnaDefaultIncompatible sysname = NULL;

    SELECT TOP (1)
        @TablaDefaultIncompatible = Esperada.Tabla,
        @ColumnaDefaultIncompatible = Esperada.Columna
    FROM
    (
        VALUES
            (N'Categoria',    N'Activo',   N'1'),
            (N'Categoria',    N'CreadoEn', N'sysutcdatetime'),
            (N'Subcategoria', N'Activo',   N'1'),
            (N'Subcategoria', N'CreadoEn', N'sysutcdatetime'),
            (N'Marca',        N'Activo',   N'1'),
            (N'Marca',        N'CreadoEn', N'sysutcdatetime'),
            (N'Proveedor',    N'Activo',   N'1'),
            (N'Proveedor',    N'CreadoEn', N'sysutcdatetime'),
            (N'Producto',     N'Activo',   N'1'),
            (N'Producto',     N'CreadoEn', N'sysutcdatetime'),
            (N'Garantia',     N'Activo',   N'1'),
            (N'Garantia',     N'CreadoEn', N'sysutcdatetime'),
            (N'Bodega',       N'Activo',   N'1'),
            (N'Bodega',       N'CreadoEn', N'sysutcdatetime')
    ) AS Esperada (Tabla, Columna, DefinicionEsperada)
    INNER JOIN sys.columns AS Columna
        ON Columna.object_id = OBJECT_ID(N'dbo.' + Esperada.Tabla, N'U')
       AND Columna.name = Esperada.Columna
    INNER JOIN sys.default_constraints AS RestriccionDefault
        ON RestriccionDefault.parent_object_id = Columna.object_id
       AND RestriccionDefault.parent_column_id = Columna.column_id
    CROSS APPLY
    (
        VALUES
        (
            LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(RestriccionDefault.definition,
                N' ', N''), N'(', N''), N')', N''), N'[', N''), N']', N''))
        )
    ) AS Normalizada (Definicion)
    WHERE
        (Esperada.Columna = N'Activo'
            AND Normalizada.Definicion NOT IN (N'1', N'cast1asbit', N'convertbit,1'))
        OR (Esperada.Columna = N'CreadoEn'
            AND Normalizada.Definicion <> Esperada.DefinicionEsperada)
    ORDER BY Esperada.Tabla, Esperada.Columna;

    IF @TablaDefaultIncompatible IS NOT NULL
    BEGIN
        SET @MensajeError = N'La restriccion DEFAULT existente en dbo.' + QUOTENAME(@TablaDefaultIncompatible)
            + N'.' + QUOTENAME(@ColumnaDefaultIncompatible)
            + N' no tiene el valor esperado. No se realizaron cambios.';
        THROW 52011, @MensajeError, 1;
    END;

    DECLARE @EspecificacionObjetoId int = OBJECT_ID(N'dbo.EspecificacionProducto', N'U');
    DECLARE @ProductoObjetoId int = OBJECT_ID(N'dbo.Producto', N'U');
    DECLARE @ProductoIdColumnaId int = COLUMNPROPERTY(@ProductoObjetoId, N'ProductoId', N'ColumnId');

    /* Si la tabla ya existe, validar su estructura antes de realizar cualquier DDL. */
    IF @EspecificacionObjetoId IS NOT NULL
    BEGIN
        SET @TablaIncompatible = NULL;
        SET @ColumnaIncompatible = NULL;

        SELECT TOP (1)
            @TablaIncompatible = N'EspecificacionProducto',
            @ColumnaIncompatible = Esperada.Columna
        FROM
        (
            VALUES
                (N'EspecificacionId',  56, NULL, NULL, 0, 1),
                (N'ProductoId',        56, NULL, NULL, 0, 0),
                (N'Etiqueta',         167,   50, NULL, 0, 0),
                (N'Valor',            167,  200, NULL, 0, 0),
                (N'Orden',             56, NULL, NULL, 0, 0)
        ) AS Esperada (Columna, TipoSistema, LongitudMaxima, Escala, EsNulable, EsIdentidad)
        LEFT JOIN sys.columns AS Columna
            ON Columna.object_id = @EspecificacionObjetoId
           AND Columna.name = Esperada.Columna
        WHERE Columna.object_id IS NULL
           OR Columna.system_type_id <> Esperada.TipoSistema
           OR (Esperada.LongitudMaxima IS NOT NULL AND Columna.max_length <> Esperada.LongitudMaxima)
           OR (Esperada.Escala IS NOT NULL AND Columna.scale <> Esperada.Escala)
           OR Columna.is_nullable <> Esperada.EsNulable
           OR Columna.is_identity <> Esperada.EsIdentidad
        ORDER BY Esperada.Columna;

        IF @TablaIncompatible IS NOT NULL
        BEGIN
            SET @MensajeError = N'La columna requerida dbo.[EspecificacionProducto].'
                + QUOTENAME(@ColumnaIncompatible)
                + N' no existe o es incompatible. No se realizaron cambios.';
            THROW 52012, @MensajeError, 1;
        END;

        IF EXISTS
        (
            SELECT 1
            FROM sys.default_constraints AS RestriccionDefault
            INNER JOIN sys.columns AS Columna
                ON Columna.object_id = RestriccionDefault.parent_object_id
               AND Columna.column_id = RestriccionDefault.parent_column_id
            CROSS APPLY
            (
                VALUES
                (
                    LOWER(REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(RestriccionDefault.definition,
                        N' ', N''), N'(', N''), N')', N''), N'[', N''), N']', N''))
                )
            ) AS Normalizada (Definicion)
            WHERE RestriccionDefault.parent_object_id = @EspecificacionObjetoId
              AND Columna.name = N'Orden'
              AND Normalizada.Definicion <> N'0'
        )
            THROW 52013, N'El DEFAULT existente de dbo.EspecificacionProducto.Orden no es compatible. No se realizaron cambios.', 1;

        IF EXISTS
        (
            SELECT 1
            FROM sys.key_constraints AS Llave
            WHERE Llave.parent_object_id = @EspecificacionObjetoId
              AND Llave.type = N'PK'
              AND
              (
                  (SELECT COUNT(*)
                   FROM sys.index_columns AS Indice
                   WHERE Indice.object_id = Llave.parent_object_id
                     AND Indice.index_id = Llave.unique_index_id
                     AND Indice.key_ordinal > 0) <> 1
                  OR NOT EXISTS
                  (
                      SELECT 1
                      FROM sys.index_columns AS Indice
                      INNER JOIN sys.columns AS Columna
                          ON Columna.object_id = Indice.object_id
                         AND Columna.column_id = Indice.column_id
                      WHERE Indice.object_id = Llave.parent_object_id
                        AND Indice.index_id = Llave.unique_index_id
                        AND Indice.key_ordinal = 1
                        AND Columna.name = N'EspecificacionId'
                  )
              )
        )
            THROW 52014, N'La llave primaria existente de dbo.EspecificacionProducto no es compatible. No se realizaron cambios.', 1;

        IF EXISTS
        (
            SELECT 1
            FROM sys.foreign_keys AS LlaveForanea
            INNER JOIN sys.foreign_key_columns AS ColumnaForanea
                ON ColumnaForanea.constraint_object_id = LlaveForanea.object_id
            WHERE LlaveForanea.parent_object_id = @EspecificacionObjetoId
              AND ColumnaForanea.parent_column_id = COLUMNPROPERTY(
                    @EspecificacionObjetoId, N'ProductoId', N'ColumnId')
              AND
              (
                  LlaveForanea.referenced_object_id <> @ProductoObjetoId
                  OR ColumnaForanea.referenced_column_id <> @ProductoIdColumnaId
                  OR (SELECT COUNT(*)
                      FROM sys.foreign_key_columns AS OtraColumna
                      WHERE OtraColumna.constraint_object_id = LlaveForanea.object_id) <> 1
              )
        )
            THROW 52015, N'La llave foranea existente de dbo.EspecificacionProducto.ProductoId no es compatible. No se realizaron cambios.', 1;
    END;

    /* Categoria */
    IF COL_LENGTH(N'dbo.Categoria', N'Activo') IS NULL
        ALTER TABLE dbo.Categoria ADD Activo bit NOT NULL CONSTRAINT DF_Categoria_Activo DEFAULT (1);
    ELSE IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Categoria', N'U')
          AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'dbo.Categoria', N'U'), N'Activo', N'ColumnId')
    )
        ALTER TABLE dbo.Categoria ADD CONSTRAINT DF_Categoria_Activo DEFAULT (1) FOR Activo;

    IF COL_LENGTH(N'dbo.Categoria', N'CreadoEn') IS NULL
        ALTER TABLE dbo.Categoria ADD CreadoEn datetime2(3) NOT NULL CONSTRAINT DF_Categoria_CreadoEn DEFAULT (SYSUTCDATETIME());
    ELSE IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Categoria', N'U')
          AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'dbo.Categoria', N'U'), N'CreadoEn', N'ColumnId')
    )
        ALTER TABLE dbo.Categoria ADD CONSTRAINT DF_Categoria_CreadoEn DEFAULT (SYSUTCDATETIME()) FOR CreadoEn;

    IF COL_LENGTH(N'dbo.Categoria', N'CreadoPor') IS NULL
        ALTER TABLE dbo.Categoria ADD CreadoPor varchar(50) NULL;
    IF COL_LENGTH(N'dbo.Categoria', N'ActualizadoEn') IS NULL
        ALTER TABLE dbo.Categoria ADD ActualizadoEn datetime2(3) NULL;
    IF COL_LENGTH(N'dbo.Categoria', N'ActualizadoPor') IS NULL
        ALTER TABLE dbo.Categoria ADD ActualizadoPor varchar(50) NULL;
    IF COL_LENGTH(N'dbo.Categoria', N'RowVer') IS NULL
        ALTER TABLE dbo.Categoria ADD RowVer rowversion NOT NULL;

    /* Subcategoria */
    IF COL_LENGTH(N'dbo.Subcategoria', N'Activo') IS NULL
        ALTER TABLE dbo.Subcategoria ADD Activo bit NOT NULL CONSTRAINT DF_Subcategoria_Activo DEFAULT (1);
    ELSE IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Subcategoria', N'U')
          AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'dbo.Subcategoria', N'U'), N'Activo', N'ColumnId')
    )
        ALTER TABLE dbo.Subcategoria ADD CONSTRAINT DF_Subcategoria_Activo DEFAULT (1) FOR Activo;

    IF COL_LENGTH(N'dbo.Subcategoria', N'CreadoEn') IS NULL
        ALTER TABLE dbo.Subcategoria ADD CreadoEn datetime2(3) NOT NULL CONSTRAINT DF_Subcategoria_CreadoEn DEFAULT (SYSUTCDATETIME());
    ELSE IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Subcategoria', N'U')
          AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'dbo.Subcategoria', N'U'), N'CreadoEn', N'ColumnId')
    )
        ALTER TABLE dbo.Subcategoria ADD CONSTRAINT DF_Subcategoria_CreadoEn DEFAULT (SYSUTCDATETIME()) FOR CreadoEn;

    IF COL_LENGTH(N'dbo.Subcategoria', N'CreadoPor') IS NULL
        ALTER TABLE dbo.Subcategoria ADD CreadoPor varchar(50) NULL;
    IF COL_LENGTH(N'dbo.Subcategoria', N'ActualizadoEn') IS NULL
        ALTER TABLE dbo.Subcategoria ADD ActualizadoEn datetime2(3) NULL;
    IF COL_LENGTH(N'dbo.Subcategoria', N'ActualizadoPor') IS NULL
        ALTER TABLE dbo.Subcategoria ADD ActualizadoPor varchar(50) NULL;
    IF COL_LENGTH(N'dbo.Subcategoria', N'RowVer') IS NULL
        ALTER TABLE dbo.Subcategoria ADD RowVer rowversion NOT NULL;

    /* Marca */
    IF COL_LENGTH(N'dbo.Marca', N'Activo') IS NULL
        ALTER TABLE dbo.Marca ADD Activo bit NOT NULL CONSTRAINT DF_Marca_Activo DEFAULT (1);
    ELSE IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Marca', N'U')
          AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'dbo.Marca', N'U'), N'Activo', N'ColumnId')
    )
        ALTER TABLE dbo.Marca ADD CONSTRAINT DF_Marca_Activo DEFAULT (1) FOR Activo;

    IF COL_LENGTH(N'dbo.Marca', N'CreadoEn') IS NULL
        ALTER TABLE dbo.Marca ADD CreadoEn datetime2(3) NOT NULL CONSTRAINT DF_Marca_CreadoEn DEFAULT (SYSUTCDATETIME());
    ELSE IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Marca', N'U')
          AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'dbo.Marca', N'U'), N'CreadoEn', N'ColumnId')
    )
        ALTER TABLE dbo.Marca ADD CONSTRAINT DF_Marca_CreadoEn DEFAULT (SYSUTCDATETIME()) FOR CreadoEn;

    IF COL_LENGTH(N'dbo.Marca', N'CreadoPor') IS NULL
        ALTER TABLE dbo.Marca ADD CreadoPor varchar(50) NULL;
    IF COL_LENGTH(N'dbo.Marca', N'ActualizadoEn') IS NULL
        ALTER TABLE dbo.Marca ADD ActualizadoEn datetime2(3) NULL;
    IF COL_LENGTH(N'dbo.Marca', N'ActualizadoPor') IS NULL
        ALTER TABLE dbo.Marca ADD ActualizadoPor varchar(50) NULL;
    IF COL_LENGTH(N'dbo.Marca', N'RowVer') IS NULL
        ALTER TABLE dbo.Marca ADD RowVer rowversion NOT NULL;

    /* Proveedor */
    IF COL_LENGTH(N'dbo.Proveedor', N'Activo') IS NULL
        ALTER TABLE dbo.Proveedor ADD Activo bit NOT NULL CONSTRAINT DF_Proveedor_Activo DEFAULT (1);
    ELSE IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Proveedor', N'U')
          AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'dbo.Proveedor', N'U'), N'Activo', N'ColumnId')
    )
        ALTER TABLE dbo.Proveedor ADD CONSTRAINT DF_Proveedor_Activo DEFAULT (1) FOR Activo;

    IF COL_LENGTH(N'dbo.Proveedor', N'CreadoEn') IS NULL
        ALTER TABLE dbo.Proveedor ADD CreadoEn datetime2(3) NOT NULL CONSTRAINT DF_Proveedor_CreadoEn DEFAULT (SYSUTCDATETIME());
    ELSE IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Proveedor', N'U')
          AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'dbo.Proveedor', N'U'), N'CreadoEn', N'ColumnId')
    )
        ALTER TABLE dbo.Proveedor ADD CONSTRAINT DF_Proveedor_CreadoEn DEFAULT (SYSUTCDATETIME()) FOR CreadoEn;

    IF COL_LENGTH(N'dbo.Proveedor', N'CreadoPor') IS NULL
        ALTER TABLE dbo.Proveedor ADD CreadoPor varchar(50) NULL;
    IF COL_LENGTH(N'dbo.Proveedor', N'ActualizadoEn') IS NULL
        ALTER TABLE dbo.Proveedor ADD ActualizadoEn datetime2(3) NULL;
    IF COL_LENGTH(N'dbo.Proveedor', N'ActualizadoPor') IS NULL
        ALTER TABLE dbo.Proveedor ADD ActualizadoPor varchar(50) NULL;
    IF COL_LENGTH(N'dbo.Proveedor', N'RowVer') IS NULL
        ALTER TABLE dbo.Proveedor ADD RowVer rowversion NOT NULL;

    /* Producto */
    IF COL_LENGTH(N'dbo.Producto', N'Activo') IS NULL
        ALTER TABLE dbo.Producto ADD Activo bit NOT NULL CONSTRAINT DF_Producto_Activo DEFAULT (1);
    ELSE IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Producto', N'U')
          AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'dbo.Producto', N'U'), N'Activo', N'ColumnId')
    )
        ALTER TABLE dbo.Producto ADD CONSTRAINT DF_Producto_Activo DEFAULT (1) FOR Activo;

    IF COL_LENGTH(N'dbo.Producto', N'CreadoEn') IS NULL
        ALTER TABLE dbo.Producto ADD CreadoEn datetime2(3) NOT NULL CONSTRAINT DF_Producto_CreadoEn DEFAULT (SYSUTCDATETIME());
    ELSE IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Producto', N'U')
          AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'dbo.Producto', N'U'), N'CreadoEn', N'ColumnId')
    )
        ALTER TABLE dbo.Producto ADD CONSTRAINT DF_Producto_CreadoEn DEFAULT (SYSUTCDATETIME()) FOR CreadoEn;

    IF COL_LENGTH(N'dbo.Producto', N'CreadoPor') IS NULL
        ALTER TABLE dbo.Producto ADD CreadoPor varchar(50) NULL;
    IF COL_LENGTH(N'dbo.Producto', N'ActualizadoEn') IS NULL
        ALTER TABLE dbo.Producto ADD ActualizadoEn datetime2(3) NULL;
    IF COL_LENGTH(N'dbo.Producto', N'ActualizadoPor') IS NULL
        ALTER TABLE dbo.Producto ADD ActualizadoPor varchar(50) NULL;
    IF COL_LENGTH(N'dbo.Producto', N'RowVer') IS NULL
        ALTER TABLE dbo.Producto ADD RowVer rowversion NOT NULL;

    /* Garantia */
    IF COL_LENGTH(N'dbo.Garantia', N'Activo') IS NULL
        ALTER TABLE dbo.Garantia ADD Activo bit NOT NULL CONSTRAINT DF_Garantia_Activo DEFAULT (1);
    ELSE IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Garantia', N'U')
          AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'dbo.Garantia', N'U'), N'Activo', N'ColumnId')
    )
        ALTER TABLE dbo.Garantia ADD CONSTRAINT DF_Garantia_Activo DEFAULT (1) FOR Activo;

    IF COL_LENGTH(N'dbo.Garantia', N'CreadoEn') IS NULL
        ALTER TABLE dbo.Garantia ADD CreadoEn datetime2(3) NOT NULL CONSTRAINT DF_Garantia_CreadoEn DEFAULT (SYSUTCDATETIME());
    ELSE IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Garantia', N'U')
          AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'dbo.Garantia', N'U'), N'CreadoEn', N'ColumnId')
    )
        ALTER TABLE dbo.Garantia ADD CONSTRAINT DF_Garantia_CreadoEn DEFAULT (SYSUTCDATETIME()) FOR CreadoEn;

    IF COL_LENGTH(N'dbo.Garantia', N'CreadoPor') IS NULL
        ALTER TABLE dbo.Garantia ADD CreadoPor varchar(50) NULL;
    IF COL_LENGTH(N'dbo.Garantia', N'ActualizadoEn') IS NULL
        ALTER TABLE dbo.Garantia ADD ActualizadoEn datetime2(3) NULL;
    IF COL_LENGTH(N'dbo.Garantia', N'ActualizadoPor') IS NULL
        ALTER TABLE dbo.Garantia ADD ActualizadoPor varchar(50) NULL;
    IF COL_LENGTH(N'dbo.Garantia', N'RowVer') IS NULL
        ALTER TABLE dbo.Garantia ADD RowVer rowversion NOT NULL;

    /* Bodega */
    IF COL_LENGTH(N'dbo.Bodega', N'Activo') IS NULL
        ALTER TABLE dbo.Bodega ADD Activo bit NOT NULL CONSTRAINT DF_Bodega_Activo DEFAULT (1);
    ELSE IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Bodega', N'U')
          AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'dbo.Bodega', N'U'), N'Activo', N'ColumnId')
    )
        ALTER TABLE dbo.Bodega ADD CONSTRAINT DF_Bodega_Activo DEFAULT (1) FOR Activo;

    IF COL_LENGTH(N'dbo.Bodega', N'CreadoEn') IS NULL
        ALTER TABLE dbo.Bodega ADD CreadoEn datetime2(3) NOT NULL CONSTRAINT DF_Bodega_CreadoEn DEFAULT (SYSUTCDATETIME());
    ELSE IF NOT EXISTS
    (
        SELECT 1 FROM sys.default_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Bodega', N'U')
          AND parent_column_id = COLUMNPROPERTY(OBJECT_ID(N'dbo.Bodega', N'U'), N'CreadoEn', N'ColumnId')
    )
        ALTER TABLE dbo.Bodega ADD CONSTRAINT DF_Bodega_CreadoEn DEFAULT (SYSUTCDATETIME()) FOR CreadoEn;

    IF COL_LENGTH(N'dbo.Bodega', N'CreadoPor') IS NULL
        ALTER TABLE dbo.Bodega ADD CreadoPor varchar(50) NULL;
    IF COL_LENGTH(N'dbo.Bodega', N'ActualizadoEn') IS NULL
        ALTER TABLE dbo.Bodega ADD ActualizadoEn datetime2(3) NULL;
    IF COL_LENGTH(N'dbo.Bodega', N'ActualizadoPor') IS NULL
        ALTER TABLE dbo.Bodega ADD ActualizadoPor varchar(50) NULL;
    IF COL_LENGTH(N'dbo.Bodega', N'RowVer') IS NULL
        ALTER TABLE dbo.Bodega ADD RowVer rowversion NOT NULL;

    /* Crear o completar las restricciones de EspecificacionProducto sin reemplazarla. */
    IF @EspecificacionObjetoId IS NULL
    BEGIN
        CREATE TABLE dbo.EspecificacionProducto
        (
            EspecificacionId int IDENTITY(1,1) NOT NULL,
            ProductoId int NOT NULL,
            Etiqueta varchar(50) NOT NULL,
            Valor varchar(200) NOT NULL,
            Orden int NOT NULL CONSTRAINT DF_EspecificacionProducto_Orden DEFAULT (0),
            CONSTRAINT PK_EspecificacionProducto PRIMARY KEY (EspecificacionId),
            CONSTRAINT FK_EspecificacionProducto_Producto
                FOREIGN KEY (ProductoId) REFERENCES dbo.Producto (ProductoId)
        );
    END
    ELSE
    BEGIN
        IF NOT EXISTS
        (
            SELECT 1
            FROM sys.default_constraints AS RestriccionDefault
            INNER JOIN sys.columns AS Columna
                ON Columna.object_id = RestriccionDefault.parent_object_id
               AND Columna.column_id = RestriccionDefault.parent_column_id
            WHERE RestriccionDefault.parent_object_id = @EspecificacionObjetoId
              AND Columna.name = N'Orden'
        )
            ALTER TABLE dbo.EspecificacionProducto
                ADD CONSTRAINT DF_EspecificacionProducto_Orden DEFAULT (0) FOR Orden;

        IF NOT EXISTS
        (
            SELECT 1
            FROM sys.key_constraints AS Llave
            WHERE Llave.parent_object_id = @EspecificacionObjetoId
              AND Llave.type = N'PK'
        )
            ALTER TABLE dbo.EspecificacionProducto
                ADD CONSTRAINT PK_EspecificacionProducto PRIMARY KEY (EspecificacionId);

        IF NOT EXISTS
        (
            SELECT 1
            FROM sys.foreign_keys AS LlaveForanea
            INNER JOIN sys.foreign_key_columns AS ColumnaForanea
                ON ColumnaForanea.constraint_object_id = LlaveForanea.object_id
            WHERE LlaveForanea.parent_object_id = @EspecificacionObjetoId
              AND LlaveForanea.referenced_object_id = @ProductoObjetoId
              AND ColumnaForanea.parent_column_id = COLUMNPROPERTY(
                    @EspecificacionObjetoId, N'ProductoId', N'ColumnId')
              AND ColumnaForanea.referenced_column_id = @ProductoIdColumnaId
              AND (SELECT COUNT(*)
                   FROM sys.foreign_key_columns AS OtraColumna
                   WHERE OtraColumna.constraint_object_id = LlaveForanea.object_id) = 1
        )
            ALTER TABLE dbo.EspecificacionProducto
                ADD CONSTRAINT FK_EspecificacionProducto_Producto
                    FOREIGN KEY (ProductoId) REFERENCES dbo.Producto (ProductoId);
    END;

    COMMIT TRANSACTION;

    PRINT N'Actualizacion del esquema de catalogo completada correctamente.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO
