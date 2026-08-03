using AspNetCore.Localizer.Json.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using NLog.Web;
using System.Text.Json.Serialization;
using Tienda.AccesoDatos.Contexto;
using Tienda.AccesoDatos.Implementaciones;
using Tienda.Dominio.DTO;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.LogicaNegocio.Implementaciones;

var builder = WebApplication.CreateBuilder(args);

// DIAGNOSTICO TEMPORAL - borrar despues de resolver el problema
string rutaDiagnostico = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "diagnostico.txt");
string cadenaConexion = builder.Configuration.GetConnectionString("DefaultConnection");
string resultadoDiagnostico =
    "ENTORNO: " + builder.Environment.EnvironmentName + Environment.NewLine +
    "CONEXION USADA: " + cadenaConexion + Environment.NewLine;

try
{
    using (var conexion = new Microsoft.Data.SqlClient.SqlConnection(cadenaConexion))
    {
        conexion.Open();
        using (var cmd = new Microsoft.Data.SqlClient.SqlCommand(
            "SELECT DB_NAME(), @@SERVERNAME, (SELECT COUNT(*) FROM Subcategoria)", conexion))
        {
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    resultadoDiagnostico += "BASE DE DATOS REAL (ADO.NET directo): " + reader.GetString(0) + Environment.NewLine;
                    resultadoDiagnostico += "SERVIDOR REAL (ADO.NET directo): " + reader.GetString(1) + Environment.NewLine;
                    resultadoDiagnostico += "CONTEO SUBCATEGORIA (ADO.NET directo): " + reader.GetInt32(2) + Environment.NewLine;
                }
            }
        }
    }
}
catch (Exception ex)
{
    resultadoDiagnostico += "ERROR AL CONECTAR: " + ex.Message + Environment.NewLine;
}

File.WriteAllText(rutaDiagnostico, resultadoDiagnostico);
// FIN DIAGNOSTICO TEMPORAL

// DIAGNOSTICO 2 - ImagenProducto
string rutaDiagnostico2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "diagnostico2.txt");
string resultado2 = "";
try
{
    using (var conexion2 = new Microsoft.Data.SqlClient.SqlConnection(cadenaConexion))
    {
        conexion2.Open();
        using (var cmd2 = new Microsoft.Data.SqlClient.SqlCommand(
            "SELECT COUNT(*) FROM ImagenProducto WHERE ProductoId = 1", conexion2))
        {
            var conteo = cmd2.ExecuteScalar();
            resultado2 += "CONTEO IMAGENPRODUCTO ProductoId=1 (ADO.NET directo): " + conteo + Environment.NewLine;
        }
        using (var cmd3 = new Microsoft.Data.SqlClient.SqlCommand(
            "SELECT DB_NAME()", conexion2))
        {
            resultado2 += "BASE DE DATOS de esta segunda conexion: " + cmd3.ExecuteScalar() + Environment.NewLine;
        }
    }
}
catch (Exception ex)
{
    resultado2 += "ERROR: " + ex.Message + Environment.NewLine;
}
File.WriteAllText(rutaDiagnostico2, resultado2);
// FIN DIAGNOSTICO 2

// Configuración de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Opcional: configurar niveles
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);

// Add services to the container
//builder.Services.AddControllers();

IdentityModelEventSource.ShowPII = true;
//NUEVO
builder.Services.AddCors(options =>
{

    options.AddPolicy("cors",

                         builder =>
                         {

                             builder.AllowAnyOrigin()

                               .AllowAnyHeader()

                               .AllowAnyMethod();

                         });

});

//NUEVO
builder.Services.AddResponseCaching();


builder.Services.AddJsonLocalization(options => options.ResourcesPath = "Recursos"); builder.Services.AddDbContext<VentasContext>(options =>
            options.UseLazyLoadingProxies()

            .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMvc().AddJsonOptions(o =>
{

    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

    o.JsonSerializerOptions.MaxDepth = 0;

});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
           options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore

);

builder.Services.AddScoped<IUnidadTrabajoEF, UnidadTrabajoEF>();
builder.Services.AddScoped<ICategoriaLN, CategoriaLN>();
builder.Services.AddScoped<ISubcategoriaLN, SubcategoriaLN>();
builder.Services.AddScoped<IClienteLN, ClienteLN>();
builder.Services.AddScoped<IProductoLN, ProductoLN>();
builder.Services.AddScoped<IPedidoLN, PedidoLN>();
builder.Services.AddScoped<IMarcaLN, MarcaLN>();
builder.Services.AddScoped<IProveedorLN, ProveedorLN>();
builder.Services.AddScoped<IBodegaLN, BodegaLN>();
builder.Services.AddScoped<IDescuentoLN, DescuentoLN>();
builder.Services.AddScoped<IGarantiaLN, GarantiaLN>();
builder.Services.AddScoped<IEtiquetaLN, EtiquetaLN>();
builder.Services.AddScoped<IInventarioLN, InventarioLN>();
builder.Services.AddScoped<IProductoDescuentoLN, ProductoDescuentoLN>();
builder.Services.AddScoped<IImagenProductoLN, ImagenProductoLN>();
builder.Services.AddScoped<IProductoGarantiaLN, ProductoGarantiaLN>();
builder.Services.AddScoped<IProductoEtiquetaLN, ProductoEtiquetaLN>();
builder.Services.AddScoped<IPagoLN, PagoLN>();
builder.Services.AddScoped<IEnvioLN, EnvioLN>();
builder.Services.AddScoped<IResenaLN, ResenaLN>();
builder.Services.AddScoped<IListaDeseosLN, ListaDeseosLN>();
builder.Services.AddScoped<IDevolucionLN, DevolucionLN>();
builder.Services.AddScoped<IEspecificacionProductoLN, EspecificacionProductoLN>();

builder.Services.AddAuthorization();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Logging.ClearProviders();

builder.Host.UseNLog();


var app = builder.Build();

app.UseCors("cors");
app.UseStaticFiles();

app.UseAuthentication(); app.UseAuthorization(); if (app.Environment.IsDevelopment())

{

    app.UseSwagger();

    app.UseSwaggerUI();

}

app.MapControllers();



app.Run();