using System;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.Web;

namespace ProjetoEcommerceGabrielMonteiro.Models
{
    public static class EmailService
    {
        public static bool EnviarEmailConfirmacao(string emailDestino, int pedidoId, List<ItemCarrinho> itens, decimal total)
        {
            try
            {
                // Configurações do e-mail (usando Gmail como exemplo)
                var fromAddress = new MailAddress("seu-email@gmail.com", "E-commerce GabrielMonteiro");
                var toAddress = new MailAddress(emailDestino);
                const string fromPassword = "sua-senha-app"; // Senha de app do Gmail
                const string subject = "Confirmação de Pedido - E-commerce GabrielMonteiro";

                // Corpo do e-mail
                string body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #007bff;'>✅ Pedido Confirmado!</h2>
                    <p>Olá, obrigado por sua compra em nosso e-commerce!</p>
                    
                    <h3>📋 Resumo do Pedido #{pedidoId}</h3>
                    <table border='1' cellpadding='8' style='border-collapse: collapse; width: 100%;'>
                        <tr style='background-color: #f8f9fa;'>
                            <th>Produto</th>
                            <th>Quantidade</th>
                            <th>Preço Unitário</th>
                            <th>Subtotal</th>
                        </tr>";

                foreach (var item in itens)
                {
                    body += $@"
                        <tr>
                            <td>{item.Nome}</td>
                            <td>{item.Quantidade}</td>
                            <td>R$ {item.Preco:N2}</td>
                            <td>R$ {item.Subtotal:N2}</td>
                        </tr>";
                }

                body += $@"
                        <tr style='font-weight: bold; background-color: #e9ecef;'>
                            <td colspan='3' style='text-align: right;'>Total:</td>
                            <td>R$ {total:N2}</td>
                        </tr>
                    </table>
                    
                    <br/>
                    <p><strong>📦 Status do pedido:</strong> Em processamento</p>
                    <p><strong>⏰ Previsão de entrega:</strong> 5-7 dias úteis</p>
                    
                    <br/>
                    <p>Atenciosamente,<br/>
                    <strong>Equipe E-commerce GabrielMonteiro</strong></p>
                    
                    <hr style='border: 1px solid #dee2e6;'/>
                    <p style='color: #6c757d; font-size: 12px;'>
                        Este é um e-mail automático, por favor não responda.
                    </p>
                </body>
                </html>";

                // Configurar SMTP
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                // Criar mensagem
                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                // Enviar e-mail
                smtp.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                // Em produção, você logaria esse erro
                System.Diagnostics.Debug.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
                return false;
            }
        }
    }
}