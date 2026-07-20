using AspNetCore.Localizer.Json.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using NLog.Web;
using System.Text.Json.Serialization;
using Ventas.AccesoDatos.Contexto;
using Ventas.AccesoDatos.Implementaciones;
using Ventas.Dominio.DTO;
using Ventas.Dominio.InterfacesAD;
using Ventas.Dominio.InterfazLN;
using Ventas.LogicaNegocio.Implementaciones;

var builder = WebApplication.CreateBuilder(args);

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


builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi//
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Logging.ClearProviders();

builder.Host.UseNLog();


var app = builder.Build();

app.UseCors("cors");

app.UseAuthentication(); app.UseAuthorization(); if (app.Environment.IsDevelopment())

{

    app.UseSwagger();

    app.UseSwaggerUI();

}

app.MapControllers();


//// Configure the HTTP request pipeline.//if (app.Environment.IsDevelopment())//{//    app.MapOpenApi();//}//app.UseHttpsRedirection();//app.UseAuthorization();//app.MapControllers();
app.Run();
