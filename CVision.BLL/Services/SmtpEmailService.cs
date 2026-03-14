using CVision.BLL.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace CVision.BLL.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _config;

    public SmtpEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var senderEmail = _config["EmailSettings:Sender"];
        var password = _config["EmailSettings:Password"];

        if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException("Email settings are missing in configuration.");
        }

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("CVision Admin", senderEmail));
        emailMessage.To.Add(new MailboxAddress(string.Empty, email));
        emailMessage.Subject = subject;

        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = message,
        };

        using var client = new SmtpClient();
        try
        {
            client.Timeout = 10000;

            await client.ConnectAsync("smtp.gmail.com", 465, true);
            await client.AuthenticateAsync(senderEmail, password);
            await client.SendAsync(emailMessage);
        }
        catch (Exception ex)
        {
            throw new Exception($"Помилка при відправці пошти на {email}", ex);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }

    public async Task SendConfirmationEmailAsync(string email, string confirmationLink)
    {
        var htmlBody = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='utf-8'>
            <style>
                .container {{
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    max-width: 500px;
                    margin: 40px auto;
                    padding: 0;
                    border-radius: 16px;
                    overflow: hidden;
                    box-shadow: 0 4px 20px rgba(0,0,0,0.1);
                    border: 1px solid #e1e8ed;
                }}
                .header {{
                    background: linear-gradient(135deg, #0984e3 0%, #6c5ce7 100%);
                    padding: 40px 20px;
                    text-align: center;
                    color: white;
                }}
                .content {{
                    padding: 40px 30px;
                    background-color: white;
                    text-align: center;
                    color: #2d3436;
                }}
                .button {{
                    display: inline-block;
                    padding: 16px 32px;
                    background-color: #0984e3;
                    color: #ffffff !important;
                    text-decoration: none;
                    border-radius: 12px;
                    font-weight: bold;
                    margin: 30px 0;
                    transition: background-color 0.3s;
                }}
                .footer {{
                    padding: 20px;
                    background-color: #f9f9f9;
                    text-align: center;
                    font-size: 12px;
                    color: #b2bec3;
                }}
                h1 {{ margin: 0; font-size: 28px; letter-spacing: 1px; }}
                p {{ line-height: 1.6; font-size: 16px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>CVision</h1>
                </div>
                <div class='content'>
                    <h2>Майже готово! 🚀</h2>
                    <p>Ми раді вітати вас у CVision. Щоб почати аналізувати резюме та керувати проектами, підтвердіть, будь ласка, свою електронну адресу.</p>
                    
                    <a href='{confirmationLink}' class='button'>Підтвердити пошту</a>
                    
                    <p style='font-size: 14px; color: #636e72;'>Якщо кнопка не працює, скопіюйте це посилання в браузер:</p>
                    <p style='font-size: 12px; word-break: break-all; color: #0984e3;'>{confirmationLink}</p>
                </div>
                <div class='footer'>
                    <p>© 2026 CVision Project • Future of Recruitment</p>
                    <p>Ви отримали цей лист, тому що зареєструвалися на нашому сервісі.</p>
                </div>
            </div>
        </body>
        </html>";

        await SendEmailAsync(email, "Підтвердження реєстрації в CVision", htmlBody);
    }
}