using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Tienda.Dominio.Entidades;

namespace Tienda.AccesoDatos.Contexto;

public partial class VentasContext : DbContext
{
    public VentasContext()
    {
    }

    public VentasContext(DbContextOptions<VentasContext> options)
        : base(options)
    {
    }

    // ===== Catálogo (tu módulo) =====
    public virtual DbSet<Categoria> Categorias { get; set; }
    public virtual DbSet<Subcategoria> Subcategorias { get; set; }
    public virtual DbSet<Producto> Productos { get; set; }
    public virtual DbSet<Marca> Marcas { get; set; }
    public virtual DbSet<Proveedor> Proveedores { get; set; }
    public virtual DbSet<Bodega> Bodegas { get; set; }
    public virtual DbSet<Descuento> Descuentos { get; set; }
    public virtual DbSet<Garantia> Garantias { get; set; }
    public virtual DbSet<Etiqueta> Etiquetas { get; set; }
    public virtual DbSet<Inventario> Inventarios { get; set; }
    public virtual DbSet<ProductoDescuento> ProductoDescuentos { get; set; }
    public virtual DbSet<ImagenProducto> ImagenProductos { get; set; }
    public virtual DbSet<ProductoGarantia> ProductoGarantias { get; set; }
    public virtual DbSet<ProductoEtiqueta> ProductoEtiquetas { get; set; }

    // ===== Módulo de Personas/Usuarios/Clientes/Pedidos (le toca a tu compañero) =====
    // Temporalmente IGNORADO en OnModelCreating hasta que su módulo esté completo y correcto.
    // NO borrar estas líneas de DbSet: cuando su parte esté lista, solo hay que quitar
    // los modelBuilder.Ignore<...>() de abajo.
    public virtual DbSet<Cliente> Clientes { get; set; }
    public virtual DbSet<DetallesPedido> DetallesPedidos { get; set; }
    public virtual DbSet<Pedido> Pedidos { get; set; }
    public virtual DbSet<SegPantalla> SegPantallas { get; set; }
    public virtual DbSet<SegPerfil> SegPerfils { get; set; }
    public virtual DbSet<SegPerfilXpantalla> SegPerfilXpantallas { get; set; }
    public virtual DbSet<SegUsuario> SegUsuarios { get; set; }
    public virtual DbSet<TipoCedula> TipoCedulas { get; set; }
    public virtual DbSet<Pago> Pagos { get; set; }
    public virtual DbSet<Envio> Envios { get; set; }
    public virtual DbSet<Resena> Resenas { get; set; }
    public virtual DbSet<ListaDeseos> ListaDeseos { get; set; }
    public virtual DbSet<Devolucion> Devoluciones { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        // =====================================================================
        // TEMPORAL: se ignoran las entidades del módulo de Personas/Usuarios/
        // Clientes/Pedidos mientras ese módulo está incompleto/roto (ej. TipoCedula
        // tiene una colección mal tipada que rompe el modelo completo de EF).
        // Cuando esté listo y corregido, borrar estas líneas de Ignore<>().
        // =====================================================================
        modelBuilder.Ignore<Cliente>();
        modelBuilder.Ignore<DetallesPedido>();
        modelBuilder.Ignore<Pedido>();
        modelBuilder.Ignore<SegPantalla>();
        modelBuilder.Ignore<SegPerfil>();
        modelBuilder.Ignore<SegPerfilXpantalla>();
        modelBuilder.Ignore<SegUsuario>();
        modelBuilder.Ignore<TipoCedula>();
        modelBuilder.Ignore<Pago>();
        modelBuilder.Ignore<Envio>();
        modelBuilder.Ignore<Resena>();
        modelBuilder.Ignore<ListaDeseos>();
        modelBuilder.Ignore<Devolucion>();

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId);
            entity.ToTable("Categoria");
            entity.HasIndex(e => e.Nombre, "UQ_Categoria_Nombre").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Descripcion).HasMaxLength(255);
            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Categoria_Activo");
            entity.Property(e => e.CreadoEn).HasPrecision(3).HasDefaultValueSql("(sysutcdatetime())", "DF_Categoria_CreadoEn");
            entity.Property(e => e.CreadoPor).HasMaxLength(50);
            entity.Property(e => e.ActualizadoEn).HasPrecision(3);
            entity.Property(e => e.ActualizadoPor).HasMaxLength(50);
            entity.Property(e => e.RowVer).IsRowVersion().IsConcurrencyToken();
        });

        modelBuilder.Entity<Subcategoria>(entity =>
        {
            entity.HasKey(e => e.SubcategoriaId);
            entity.ToTable("Subcategoria");
            entity.HasIndex(e => e.CategoriaId, "IX_Subcategoria_CategoriaId");

            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Subcategoria_Activo");
            entity.Property(e => e.CreadoEn).HasPrecision(3).HasDefaultValueSql("(sysutcdatetime())", "DF_Subcategoria_CreadoEn");
            entity.Property(e => e.CreadoPor).HasMaxLength(50);
            entity.Property(e => e.ActualizadoEn).HasPrecision(3);
            entity.Property(e => e.ActualizadoPor).HasMaxLength(50);
            entity.Property(e => e.RowVer).IsRowVersion().IsConcurrencyToken();

            entity.HasOne(d => d.Categoria).WithMany(p => p.Subcategorias)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Subcategoria_Categoria");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoId);
            entity.ToTable("Producto");
            entity.HasIndex(e => e.SubcategoriaId, "IX_Producto_SubcategoriaId");
            entity.HasIndex(e => e.MarcaId, "IX_Producto_MarcaId");
            entity.HasIndex(e => e.ProveedorId, "IX_Producto_ProveedorId");

            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Descripcion).HasMaxLength(500);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CostoCompra).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Producto_Activo");
            entity.Property(e => e.CreadoEn).HasPrecision(3).HasDefaultValueSql("(sysutcdatetime())", "DF_Producto_CreadoEn");
            entity.Property(e => e.CreadoPor).HasMaxLength(50);
            entity.Property(e => e.ActualizadoEn).HasPrecision(3);
            entity.Property(e => e.ActualizadoPor).HasMaxLength(50);
            entity.Property(e => e.RowVer).IsRowVersion().IsConcurrencyToken();

            entity.HasOne(d => d.Subcategoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.SubcategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Subcategoria");

            entity.HasOne(d => d.Marca).WithMany(p => p.Productos)
                .HasForeignKey(d => d.MarcaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Marca");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Productos)
                .HasForeignKey(d => d.ProveedorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Proveedor");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.MarcaId);
            entity.HasIndex(e => e.Nombre, "UQ_Marca_Nombre").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PaisOrigen).HasMaxLength(100);
            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Marca_Activo");
            entity.Property(e => e.CreadoEn).HasPrecision(3).HasDefaultValueSql("(sysutcdatetime())", "DF_Marca_CreadoEn");
            entity.Property(e => e.CreadoPor).HasMaxLength(50);
            entity.Property(e => e.ActualizadoEn).HasPrecision(3);
            entity.Property(e => e.ActualizadoPor).HasMaxLength(50);
            entity.Property(e => e.RowVer).IsRowVersion().IsConcurrencyToken();
        });

        modelBuilder.Entity<Proveedor>(entity =>
        {
            entity.HasKey(e => e.ProveedorId);
            entity.HasIndex(e => e.Nombre, "UQ_Proveedor_Nombre").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Proveedor_Activo");
            entity.Property(e => e.CreadoEn).HasPrecision(3).HasDefaultValueSql("(sysutcdatetime())", "DF_Proveedor_CreadoEn");
            entity.Property(e => e.CreadoPor).HasMaxLength(50);
            entity.Property(e => e.ActualizadoEn).HasPrecision(3);
            entity.Property(e => e.ActualizadoPor).HasMaxLength(50);
            entity.Property(e => e.RowVer).IsRowVersion().IsConcurrencyToken();
        });

        modelBuilder.Entity<Bodega>(entity =>
        {
            entity.HasKey(e => e.BodegaId);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Ubicacion).HasMaxLength(200);
            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Bodega_Activo");
            entity.Property(e => e.CreadoEn).HasPrecision(3).HasDefaultValueSql("(sysutcdatetime())", "DF_Bodega_CreadoEn");
            entity.Property(e => e.CreadoPor).HasMaxLength(50);
            entity.Property(e => e.ActualizadoEn).HasPrecision(3);
            entity.Property(e => e.ActualizadoPor).HasMaxLength(50);
            entity.Property(e => e.RowVer).IsRowVersion().IsConcurrencyToken();
        });

        modelBuilder.Entity<Descuento>(entity =>
        {
            entity.HasKey(e => e.DescuentoId);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Porcentaje).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Descuento_Activo");
            entity.Property(e => e.CreadoEn).HasPrecision(3).HasDefaultValueSql("(sysutcdatetime())", "DF_Descuento_CreadoEn");
            entity.Property(e => e.CreadoPor).HasMaxLength(50);
            entity.Property(e => e.ActualizadoEn).HasPrecision(3);
            entity.Property(e => e.ActualizadoPor).HasMaxLength(50);
            entity.Property(e => e.RowVer).IsRowVersion().IsConcurrencyToken();
        });

        modelBuilder.Entity<Garantia>(entity =>
        {
            entity.HasKey(e => e.GarantiaId);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Garantia_Activo");
            entity.Property(e => e.CreadoEn).HasPrecision(3).HasDefaultValueSql("(sysutcdatetime())", "DF_Garantia_CreadoEn");
            entity.Property(e => e.CreadoPor).HasMaxLength(50);
            entity.Property(e => e.ActualizadoEn).HasPrecision(3);
            entity.Property(e => e.ActualizadoPor).HasMaxLength(50);
            entity.Property(e => e.RowVer).IsRowVersion().IsConcurrencyToken();
        });

        modelBuilder.Entity<Etiqueta>(entity =>
        {
            entity.HasKey(e => e.EtiquetaId);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Etiqueta_Activo");
            entity.Property(e => e.CreadoEn).HasPrecision(3).HasDefaultValueSql("(sysutcdatetime())", "DF_Etiqueta_CreadoEn");
            entity.Property(e => e.CreadoPor).HasMaxLength(50);
            entity.Property(e => e.ActualizadoEn).HasPrecision(3);
            entity.Property(e => e.ActualizadoPor).HasMaxLength(50);
            entity.Property(e => e.RowVer).IsRowVersion().IsConcurrencyToken();
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.InventarioId);
            entity.HasOne(d => d.Producto).WithMany(p => p.Inventarios)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_Producto");
            entity.HasOne(d => d.Bodega).WithMany(p => p.Inventarios)
                .HasForeignKey(d => d.BodegaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_Bodega");
        });

        modelBuilder.Entity<ProductoDescuento>(entity =>
        {
            entity.HasKey(e => e.ProductoDescuentoId);
            entity.HasOne(d => d.Producto).WithMany(p => p.ProductoDescuentos)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductoDescuento_Producto");
            entity.HasOne(d => d.Descuento).WithMany(p => p.ProductoDescuentos)
                .HasForeignKey(d => d.DescuentoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductoDescuento_Descuento");
        });

        modelBuilder.Entity<ImagenProducto>(entity =>
        {
            entity.HasKey(e => e.ImagenId);
            entity.ToTable("ImagenProducto");
            entity.Property(e => e.RutaImagen).HasMaxLength(255);
            entity.HasOne(d => d.Producto).WithMany(p => p.ImagenProductos)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImagenProducto_Producto");
        });

        modelBuilder.Entity<ProductoGarantia>(entity =>
        {
            entity.HasKey(e => e.ProductoGarantiaId);
            entity.HasOne(d => d.Producto).WithMany(p => p.ProductoGarantias)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductoGarantia_Producto");
            entity.HasOne(d => d.Garantia).WithMany(p => p.ProductoGarantias)
                .HasForeignKey(d => d.GarantiaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductoGarantia_Garantia");
        });

        modelBuilder.Entity<ProductoEtiqueta>(entity =>
        {
            entity.HasKey(e => e.ProductoEtiquetaId);
            entity.HasOne(d => d.Producto).WithMany(p => p.ProductoEtiquetas)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductoEtiqueta_Producto");
            entity.HasOne(d => d.Etiqueta).WithMany(p => p.ProductoEtiquetas)
                .HasForeignKey(d => d.EtiquetaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductoEtiqueta_Etiqueta");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}