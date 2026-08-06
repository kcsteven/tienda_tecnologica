USE [TiendaTecnologicaDB];
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    /* Verificaciones previas para índices únicos. */
    IF EXISTS (
        SELECT 1
        FROM dbo.Persona
        WHERE Email IS NOT NULL
        GROUP BY Email
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 51001, N'No se puede crear el índice único de Persona.Email: existen correos duplicados.', 1;
    END;

    IF EXISTS (
        SELECT 1
        FROM dbo.Usuario
        GROUP BY NombreUsuario
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 51002, N'No se puede crear el índice único de Usuario.NombreUsuario: existen nombres de usuario duplicados.', 1;
    END;

    IF EXISTS (
        SELECT 1
        FROM dbo.Usuario
        GROUP BY PersonaId
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 51003, N'No se puede crear el índice único de Usuario.PersonaId: una persona tiene más de un usuario.', 1;
    END;

    IF EXISTS (
        SELECT 1
        FROM dbo.Cliente
        GROUP BY PersonaId
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 51004, N'No se puede crear el índice único de Cliente.PersonaId: una persona tiene más de un cliente.', 1;
    END;

    IF EXISTS (
        SELECT 1
        FROM dbo.Empleado
        GROUP BY PersonaId
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 51005, N'No se puede crear el índice único de Empleado.PersonaId: una persona tiene más de un empleado.', 1;
    END;

    IF EXISTS (
        SELECT 1
        FROM dbo.Rol
        GROUP BY Nombre
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 51006, N'No se puede crear el índice único de Rol.Nombre: existen roles duplicados.', 1;
    END;

    IF EXISTS (
        SELECT 1
        FROM dbo.TipoDocumento
        GROUP BY Nombre
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 51007, N'No se puede crear el índice único de TipoDocumento.Nombre: existen tipos de documento duplicados.', 1;
    END;

    IF EXISTS (
        SELECT 1
        FROM dbo.EstadoPedido
        GROUP BY Nombre
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 51008, N'No se puede crear el índice único de EstadoPedido.Nombre: existen estados de pedido duplicados.', 1;
    END;

    IF EXISTS (
        SELECT 1
        FROM dbo.MetodoPago
        GROUP BY Nombre
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 51009, N'No se puede crear el índice único de MetodoPago.Nombre: existen métodos de pago duplicados.', 1;
    END;

    IF EXISTS (
        SELECT 1
        FROM dbo.Inventario
        GROUP BY ProductoId, BodegaId
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 51010, N'No se puede crear el índice único de Inventario: existen combinaciones ProductoId/BodegaId duplicadas.', 1;
    END;

    IF EXISTS (
        SELECT 1
        FROM dbo.DireccionCliente
        WHERE EsPrincipal = 1
        GROUP BY ClienteId
        HAVING COUNT(*) > 1
    )
    BEGIN
        THROW 51011, N'No se puede limitar la dirección principal: existen clientes con más de una dirección principal.', 1;
    END;

    /* Verificaciones previas para restricciones CHECK. */
    IF EXISTS (SELECT 1 FROM dbo.Inventario WHERE Cantidad < 0)
    BEGIN
        THROW 51020, N'No se puede agregar CK_Inventario_Cantidad_NoNegativa: existen cantidades negativas.', 1;
    END;

    IF EXISTS (SELECT 1 FROM dbo.Producto WHERE Precio <= 0)
    BEGIN
        THROW 51021, N'No se puede agregar CK_Producto_Precio_Positivo: existen precios menores o iguales a cero.', 1;
    END;

    IF EXISTS (SELECT 1 FROM dbo.Producto WHERE CostoCompra < 0)
    BEGIN
        THROW 51022, N'No se puede agregar CK_Producto_CostoCompra_NoNegativo: existen costos de compra negativos.', 1;
    END;

    IF EXISTS (SELECT 1 FROM dbo.DetallePedido WHERE Cantidad <= 0)
    BEGIN
        THROW 51023, N'No se puede agregar CK_DetallePedido_Cantidad_Positiva: existen cantidades menores o iguales a cero.', 1;
    END;

    IF EXISTS (SELECT 1 FROM dbo.DetallePedido WHERE PrecioUnitario < 0)
    BEGIN
        THROW 51024, N'No se puede agregar CK_DetallePedido_PrecioUnitario_NoNegativo: existen precios unitarios negativos.', 1;
    END;

    IF EXISTS (SELECT 1 FROM dbo.Pedido WHERE Total < 0)
    BEGIN
        THROW 51025, N'No se puede agregar CK_Pedido_Total_NoNegativo: existen totales negativos.', 1;
    END;

    IF EXISTS (SELECT 1 FROM dbo.Pago WHERE Monto < 0)
    BEGIN
        THROW 51026, N'No se puede agregar CK_Pago_Monto_NoNegativo: existen montos negativos.', 1;
    END;

    /* Datos base; las claves se resuelven por Nombre y nunca por un ID fijo. */
    INSERT INTO dbo.Rol (Nombre)
    SELECT valores.Nombre
    FROM (VALUES
        (CONVERT(varchar(50), N'Cliente')),
        (CONVERT(varchar(50), N'Empleado'))
    ) AS valores(Nombre)
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.Rol existente
        WHERE existente.Nombre = valores.Nombre
    );

    INSERT INTO dbo.TipoDocumento (Nombre)
    SELECT valores.Nombre
    FROM (VALUES
        (CONVERT(varchar(50), N'Cedula de identidad costarricense')),
        (CONVERT(varchar(50), N'DIMEX')),
        (CONVERT(varchar(50), N'Pasaporte'))
    ) AS valores(Nombre)
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.TipoDocumento existente
        WHERE existente.Nombre = valores.Nombre
    );

    INSERT INTO dbo.MetodoPago (Nombre)
    SELECT valores.Nombre
    FROM (VALUES
        (CONVERT(varchar(50), N'Tarjeta')),
        (CONVERT(varchar(50), N'PayPal'))
    ) AS valores(Nombre)
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.MetodoPago existente
        WHERE existente.Nombre = valores.Nombre
    );

    INSERT INTO dbo.EstadoPedido (Nombre)
    SELECT valores.Nombre
    FROM (VALUES
        (CONVERT(varchar(50), N'Pendiente')),
        (CONVERT(varchar(50), N'Pagado')),
        (CONVERT(varchar(50), N'En preparación')),
        (CONVERT(varchar(50), N'Enviado')),
        (CONVERT(varchar(50), N'Entregado')),
        (CONVERT(varchar(50), N'Cancelado'))
    ) AS valores(Nombre)
    WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.EstadoPedido existente
        WHERE existente.Nombre = valores.Nombre
    );

    /* Índices únicos. */
    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.Persona') AND name = N'UX_Persona_Email'
    )
    BEGIN
        CREATE UNIQUE INDEX UX_Persona_Email
            ON dbo.Persona (Email)
            WHERE Email IS NOT NULL;
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.Usuario') AND name = N'UX_Usuario_NombreUsuario'
    )
    BEGIN
        CREATE UNIQUE INDEX UX_Usuario_NombreUsuario ON dbo.Usuario (NombreUsuario);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.Usuario') AND name = N'UX_Usuario_PersonaId'
    )
    BEGIN
        CREATE UNIQUE INDEX UX_Usuario_PersonaId ON dbo.Usuario (PersonaId);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.Cliente') AND name = N'UX_Cliente_PersonaId'
    )
    BEGIN
        CREATE UNIQUE INDEX UX_Cliente_PersonaId ON dbo.Cliente (PersonaId);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.Empleado') AND name = N'UX_Empleado_PersonaId'
    )
    BEGIN
        CREATE UNIQUE INDEX UX_Empleado_PersonaId ON dbo.Empleado (PersonaId);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.Rol') AND name = N'UX_Rol_Nombre'
    )
    BEGIN
        CREATE UNIQUE INDEX UX_Rol_Nombre ON dbo.Rol (Nombre);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.TipoDocumento') AND name = N'UX_TipoDocumento_Nombre'
    )
    BEGIN
        CREATE UNIQUE INDEX UX_TipoDocumento_Nombre ON dbo.TipoDocumento (Nombre);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.EstadoPedido') AND name = N'UX_EstadoPedido_Nombre'
    )
    BEGIN
        CREATE UNIQUE INDEX UX_EstadoPedido_Nombre ON dbo.EstadoPedido (Nombre);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.MetodoPago') AND name = N'UX_MetodoPago_Nombre'
    )
    BEGIN
        CREATE UNIQUE INDEX UX_MetodoPago_Nombre ON dbo.MetodoPago (Nombre);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.Inventario') AND name = N'UX_Inventario_Producto_Bodega'
    )
    BEGIN
        CREATE UNIQUE INDEX UX_Inventario_Producto_Bodega ON dbo.Inventario (ProductoId, BodegaId);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.DireccionCliente') AND name = N'UX_DireccionCliente_Principal'
    )
    BEGIN
        CREATE UNIQUE INDEX UX_DireccionCliente_Principal
            ON dbo.DireccionCliente (ClienteId)
            WHERE EsPrincipal = 1;
    END;

    /* Restricciones CHECK. */
    IF NOT EXISTS (
        SELECT 1 FROM sys.check_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Inventario') AND name = N'CK_Inventario_Cantidad_NoNegativa'
    )
    BEGIN
        ALTER TABLE dbo.Inventario WITH CHECK
            ADD CONSTRAINT CK_Inventario_Cantidad_NoNegativa CHECK (Cantidad >= 0);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.check_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Producto') AND name = N'CK_Producto_Precio_Positivo'
    )
    BEGIN
        ALTER TABLE dbo.Producto WITH CHECK
            ADD CONSTRAINT CK_Producto_Precio_Positivo CHECK (Precio > 0);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.check_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Producto') AND name = N'CK_Producto_CostoCompra_NoNegativo'
    )
    BEGIN
        ALTER TABLE dbo.Producto WITH CHECK
            ADD CONSTRAINT CK_Producto_CostoCompra_NoNegativo CHECK (CostoCompra IS NULL OR CostoCompra >= 0);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.check_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.DetallePedido') AND name = N'CK_DetallePedido_Cantidad_Positiva'
    )
    BEGIN
        ALTER TABLE dbo.DetallePedido WITH CHECK
            ADD CONSTRAINT CK_DetallePedido_Cantidad_Positiva CHECK (Cantidad > 0);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.check_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.DetallePedido') AND name = N'CK_DetallePedido_PrecioUnitario_NoNegativo'
    )
    BEGIN
        ALTER TABLE dbo.DetallePedido WITH CHECK
            ADD CONSTRAINT CK_DetallePedido_PrecioUnitario_NoNegativo CHECK (PrecioUnitario >= 0);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.check_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Pedido') AND name = N'CK_Pedido_Total_NoNegativo'
    )
    BEGIN
        ALTER TABLE dbo.Pedido WITH CHECK
            ADD CONSTRAINT CK_Pedido_Total_NoNegativo CHECK (Total >= 0);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM sys.check_constraints
        WHERE parent_object_id = OBJECT_ID(N'dbo.Pago') AND name = N'CK_Pago_Monto_NoNegativo'
    )
    BEGIN
        ALTER TABLE dbo.Pago WITH CHECK
            ADD CONSTRAINT CK_Pago_Monto_NoNegativo CHECK (Monto >= 0);
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END;

    THROW;
END CATCH;
GO
