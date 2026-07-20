using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Ventas.Dominio.Entidades;

namespace Ventas.AccesoDatos.Contexto;

public partial class VentasContext : DbContext
{
    public VentasContext()
    {
    }

    public VentasContext(DbContextOptions<VentasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categoria> Categorias { get; set; }
    public virtual DbSet<Subcategoria> Subcategorias { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }
    public virtual DbSet<DetallesPedido> DetallesPedidos { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<SegPantalla> SegPantallas { get; set; }

    public virtual DbSet<SegPerfil> SegPerfils { get; set; }

    public virtual DbSet<SegPerfilXpantalla> SegPerfilXpantallas { get; set; }

    public virtual DbSet<SegUsuario> SegUsuarios { get; set; }

    public virtual DbSet<TipoCedula> TipoCedulas { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }
    public virtual DbSet<Proveedor> Proveedores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {

    }
    
#warning 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");


        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId);
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
       
        modelBuilder.Entity<DetallesPedido>(entity =>
        {
            entity.HasKey(e => e.DetalleId);

            entity.ToTable("DetallesPedido");

            entity.HasIndex(e => e.PedidoId, "IX_DetallesPedido_PedidoId");

            entity.HasIndex(e => e.ProductoId, "IX_DetallesPedido_ProductoId");

            entity.Property(e => e.Descuento).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.ProductoId).HasMaxLength(50);

            entity.HasOne(d => d.Pedido).WithMany(p => p.DetallesPedidos)
                .HasForeignKey(d => d.PedidoId)
                .HasConstraintName("FK_DetallesPedido_Pedido");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetallesPedidos)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetallesPedido_Producto");
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasIndex(e => e.ClienteId, "IX_Pedidos_ClienteId");

            entity.HasIndex(e => e.FechaPedido, "IX_Pedidos_Fecha");

            entity.Property(e => e.ActualizadoEn).HasPrecision(3);
            entity.Property(e => e.ActualizadoPor).HasMaxLength(50);
            entity.Property(e => e.CreadoEn)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Pedidos_CreadoEn");
            entity.Property(e => e.CreadoPor).HasMaxLength(50);
            entity.Property(e => e.FechaPedido)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Pedidos_Fecha");
            entity.Property(e => e.Moneda)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("CRC", "DF_Pedidos_Moneda");
            entity.Property(e => e.RowVer)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Total).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Pedidos_Clientes");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasIndex(e => e.CategoriaId, "IX_Productos_CategoriaId");

            entity.HasIndex(e => e.Nombre, "UQ_Productos_Nombre")
                .IsUnique()
                .HasFilter("([Activo]=(1))");

            entity.Property(e => e.ProductoId).HasMaxLength(50);
            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Productos_Activo");
            entity.Property(e => e.ActualizadoEn).HasPrecision(3);
            entity.Property(e => e.ActualizadoPor).HasMaxLength(50);
            entity.Property(e => e.CreadoEn)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_Productos_CreadoEn");
            entity.Property(e => e.CreadoPor).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(150);
            entity.Property(e => e.Precio).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.RowVer)
                .IsRowVersion()
                .IsConcurrencyToken();

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Productos_Categoria");
        });

        modelBuilder.Entity<SegPantalla>(entity =>
        {
            entity.HasKey(e => e.CodigoPantalla);

            entity.ToTable("SEG_Pantallas");

            entity.HasIndex(e => e.NombrePantalla, "UQ_SEG_Pantallas_Nombre").IsUnique();

            entity.Property(e => e.NombrePantalla).HasMaxLength(200);
        });

        modelBuilder.Entity<SegPerfil>(entity =>
        {
            entity.HasKey(e => e.CodigoPerfil);

            entity.ToTable("SEG_Perfil");

            entity.HasIndex(e => e.Descripcion, "UQ_SEG_Perfil_Descripcion").IsUnique();

            entity.Property(e => e.Descripcion).HasMaxLength(150);
        });

        modelBuilder.Entity<SegPerfilXpantalla>(entity =>
        {
            entity.HasKey(e => e.PerfilXpantallaId);

            entity.ToTable("SEG_PerfilXPantalla");

            entity.HasIndex(e => e.CodigoPantalla, "IX_SEG_PXP_Pantalla");

            entity.HasIndex(e => e.CodigoPerfil, "IX_SEG_PXP_Perfil");

            entity.HasIndex(e => new { e.CodigoPerfil, e.CodigoPantalla }, "UQ_SEG_PerfilPantalla").IsUnique();

            entity.Property(e => e.PerfilXpantallaId).HasColumnName("PerfilXPantallaId");

            entity.HasOne(d => d.CodigoPantallaNavigation).WithMany(p => p.SegPerfilXpantallas)
                .HasForeignKey(d => d.CodigoPantalla)
                .HasConstraintName("FK_SEG_PXP_Pantalla");

            entity.HasOne(d => d.CodigoPerfilNavigation).WithMany(p => p.SegPerfilXpantallas)
                .HasForeignKey(d => d.CodigoPerfil)
                .HasConstraintName("FK_SEG_PXP_Perfil");
        });

        modelBuilder.Entity<SegUsuario>(entity =>
        {
            entity.HasKey(e => e.Usuario);

            entity.ToTable("SEG_Usuario");

            entity.HasIndex(e => e.CedulaUsuario, "UQ_SEG_Usuario_Cedula").IsUnique();

            entity.HasIndex(e => e.Email, "UQ_SEG_Usuario_Email").IsUnique();

            entity.Property(e => e.Usuario).HasMaxLength(50);
            entity.Property(e => e.Apellidos).HasMaxLength(150);
            entity.Property(e => e.CedulaUsuario).HasMaxLength(25);
            entity.Property(e => e.Clave).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(250);
            entity.Property(e => e.Email).HasMaxLength(254);
            entity.Property(e => e.Estado).HasDefaultValue((byte)1, "DF_SEG_Usuario_Estado");
            entity.Property(e => e.FechaActualizacion)
                .HasPrecision(3)
                .HasDefaultValueSql("(sysutcdatetime())", "DF_SEG_Usuario_FechaAct");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(25);

            entity.HasOne(d => d.CodigoPerfilNavigation).WithMany(p => p.SegUsuarios)
                .HasForeignKey(d => d.CodigoPerfil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SEG_Usuario_Perfil");

            entity.HasOne(d => d.TipoCedula).WithMany(p => p.SegUsuarios)
                .HasForeignKey(d => d.TipoCedulaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SEG_Usuario_TipoCedula");
        });

        modelBuilder.Entity<TipoCedula>(entity =>
        {
            entity.HasKey(e => e.TipoCedula1);

            entity.ToTable("TipoCedula");

            entity.HasIndex(e => e.Descripcion, "UQ_TipoCedula_Descripcion").IsUnique();

            entity.Property(e => e.TipoCedula1).HasColumnName("TipoCedula");
            entity.Property(e => e.Descripcion).HasMaxLength(100);
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
