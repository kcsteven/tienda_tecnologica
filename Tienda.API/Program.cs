using AspNetCore.Localizer.Json.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using Tienda.AccesoDatos.Contexto;
using Tienda.API.Servicios.Usuarios;
using Tienda.AccesoDatos.Implementaciones;
using Tienda.Dominio.DTO;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.LogicaNegocio.Implementaciones;

var builder = WebApplication.CreateBuilder(args);

// la API revisa la misma configuración JWT que usará el emisor del token
var jwtClave = builder.Configuration["Jwt:Clave"];

if (string.IsNullOrWhiteSpace(jwtClave) || jwtClave.Length < 32)
{
    throw new InvalidOperationException("La configuración Jwt:Clave es obligatoria y debe tener al menos 32 caracteres.");
}

var jwtEmisor = string.IsNullOrWhiteSpace(builder.Configuration["Jwt:Emisor"])
    ? "Tienda.API"
    : builder.Configuration["Jwt:Emisor"]!;

var jwtAudiencia = string.IsNullOrWhiteSpace(builder.Configuration["Jwt:Audiencia"])
    ? "Tienda.Angular"
    : builder.Configuration["Jwt:Audiencia"]!;

var jwtDuracionConfigurada = builder.Configuration["Jwt:DuracionMinutos"];

if (!string.IsNullOrWhiteSpace(jwtDuracionConfigurada) &&
    (!int.TryParse(jwtDuracionConfigurada, NumberStyles.None, CultureInfo.InvariantCulture, out var jwtDuracionMinutos) ||
     jwtDuracionMinutos < 5 || jwtDuracionMinutos > 1440))
{
    throw new InvalidOperationException("La configuración Jwt:DuracionMinutos debe estar entre 5 y 1440.");
}

// Configuración de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// configurar niveles
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);



IdentityModelEventSource.ShowPII = false;

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

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtClave)),
            ValidateIssuer = true,
            ValidIssuer = jwtEmisor,
            ValidateAudience = true,
            ValidAudience = jwtAudiencia,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddScoped<IUnidadTrabajoEF, UnidadTrabajoEF>();
builder.Services.AddScoped<IRegistroClienteLN, RegistroClienteLN>();
builder.Services.AddScoped<IHashContrasena, HashContrasena>();
builder.Services.AddScoped<ITokenSesion, TokenSesion>();
builder.Services.AddScoped<IAccesoUsuarioLN, AccesoUsuarioLN>();
builder.Services.AddScoped<ICategoriaLN, CategoriaLN>();
builder.Services.AddScoped<ISubcategoriaLN, SubcategoriaLN>();
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

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())

{

    app.UseSwagger();

    app.UseSwaggerUI();

}

app.MapControllers();

app.Run();
