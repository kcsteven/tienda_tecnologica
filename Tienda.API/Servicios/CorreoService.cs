using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Tienda.API.Configuracion;

namespace Tienda.API.Servicios.Correo;

public class CorreoService : ICorreoService
{
    private readonly CorreoSettings _settings;

    public CorreoService(
        IOptions<CorreoSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task EnviarFacturaAsync(
    string correoDestino,
    string nombreCliente,
    int pedidoId,
    decimal subtotal,
    decimal iva,
    decimal total)
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
            $"Factura de compra #{pedidoId} - Tienda Tecnológica";


        var cuerpo = $"""
        <html>

        <body style="font-family: Arial, sans-serif;">

            <h2>
                Tienda Tecnológica
            </h2>

            <p>
                Hola <strong>{nombreCliente}</strong>,
            </p>

            <p>
                Gracias por realizar tu compra.
            </p>

            <hr />

            <h3>
                Factura #{pedidoId}
            </h3>

            <table
                style="
                    width: 100%;
                    border-collapse: collapse;
                ">

                <tr>
                    <td>
                        Subtotal
                    </td>

                    <td style="text-align:right;">
                        ₡{subtotal:N2}
                    </td>
                </tr>

                <tr>
                    <td>
                        IVA (13%)
                    </td>

                    <td style="text-align:right;">
                        ₡{iva:N2}
                    </td>
                </tr>

                <tr>
                    <td>
                        <strong>Total</strong>
                    </td>

                    <td style="text-align:right;">
                        <strong>
                            ₡{total:N2}
                        </strong>
                    </td>
                </tr>

            </table>

            <hr />

            <p>
                Este correo corresponde al comprobante
                de tu compra.
            </p>

            <p>
                Tienda Tecnológica
            </p>

        </body>

        </html>
        """;


        var builder = new BodyBuilder
        {
            HtmlBody = cuerpo
        };

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