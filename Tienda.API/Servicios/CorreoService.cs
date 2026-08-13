using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;
using Tienda.API.Configuracion;
using Tienda.Dominio.EntidadesTipadas;

namespace Tienda.API.Servicios.Correo;

public class CorreoService : ICorreoService
{
    private readonly CorreoSettings _settings;
    private readonly IWebHostEnvironment _env; // <-- para ubicar los archivos físicos de wwwroot

    public CorreoService(
        IOptions<CorreoSettings> settings,
        IWebHostEnvironment env)
    {
        _settings = settings.Value;
        _env = env;
    }

    public async Task EnviarFacturaAsync(
    string correoDestino,
    string nombreCliente,
    int pedidoId,
    decimal subtotal,
    decimal iva,
    decimal total,
    List<TItemFactura> items)
    {
        var mensaje = new MimeMessage();

        mensaje.From.Add(
            new MailboxAddress(
                _settings.NombreRemitente,
                _settings.Remitente
            )
        );

        mensaje.To.Add(
            new MailboxAddress(
                nombreCliente,
                correoDestino
            )
        );

        mensaje.Subject =
             mensaje.Subject = "Factura de compra - Tienda Tecnológica";

        var builder = new BodyBuilder();

        // Por cada producto, si tiene imagen la incrusto como recurso enlazado (cid:)
        // en vez de poner la URL directa. Así el correo funciona aunque el
        // backend esté corriendo en localhost (Gmail no puede "entrar" a tu PC,
        // pero sí puede mostrar un archivo que va pegado dentro del propio correo)
        var filas = new List<string>();

        foreach (var item in items)
        {
            string imgTag = "";

            if (!string.IsNullOrEmpty(item.ImagenUrl))
            {
                var rutaFisica = Path.Combine(
                    _env.WebRootPath,
                    item.ImagenUrl.Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(rutaFisica))
                {
                    var recurso = builder.LinkedResources.Add(rutaFisica);
                    recurso.ContentId = MimeUtils.GenerateMessageId();

                    imgTag = $"""<img src="cid:{recurso.ContentId}" width="60" style="vertical-align:middle; border-radius:4px; margin-right:10px;" />""";
                }
            }

            filas.Add($"""
            <tr>
                <td style="padding:8px 0;">
                    {imgTag}{item.Nombre}
                </td>
                <td style="text-align:center;">x{item.Cantidad}</td>
                <td style="text-align:right;">₡{(item.PrecioUnitario * item.Cantidad):N2}</td>
            </tr>
            """);
        }

        var filasProductos = string.Join("", filas);

        var cuerpo = $"""
        <html>
        <body style="font-family: Arial, sans-serif;">
            <h2>Tienda Tecnológica</h2>
            <p>Hola <strong>{nombreCliente}</strong>,</p>
            <p>Gracias por realizar tu compra.</p>
            <hr />
            

            <table style="width:100%; border-collapse: collapse;">
                {filasProductos}
                <tr><td colspan="3"><hr /></td></tr>
                <tr>
                    <td colspan="2">Subtotal</td>
                    <td style="text-align:right;">₡{subtotal:N2}</td>
                </tr>
                <tr>
                    <td colspan="2">IVA (13%)</td>
                    <td style="text-align:right;">₡{iva:N2}</td>
                </tr>
                <tr>
                    <td colspan="2"><strong>Total</strong></td>
                    <td style="text-align:right;"><strong>₡{total:N2}</strong></td>
                </tr>
            </table>

            <hr />
            <p>Este correo corresponde al comprobante de tu compra.</p>
            <p>Tienda Tecnológica</p>
        </body>
        </html>
        """;

        builder.HtmlBody = cuerpo;
        mensaje.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _settings.Servidor,
            _settings.Puerto,
            SecureSocketOptions.StartTls
        );

        await smtp.AuthenticateAsync(
            _settings.Usuario,
            _settings.Contrasena
        );

        await smtp.SendAsync(mensaje);

        await smtp.DisconnectAsync(true);
    }
}