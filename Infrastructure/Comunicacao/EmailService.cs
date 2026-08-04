using ConsoleExecBUs.Interfaces;
using ConsoleExecBUs.Core;
using ConsoleExecBUs.Domain;
using System.Net.Mail;
using System.Text;

namespace ConsoleExecBUs.Aplication.Services
{
    class EmailService : IEmailService
    {
        private readonly ClasseInicializacao _ini;
        private readonly IDatabaseService _db;
        private readonly ILogger _logger;

        public EmailService(ClasseInicializacao ini, IDatabaseService db, ILogger logger)
        {
            _ini = ini;
            _db = db;
            _logger = logger;
        }

        public void Enviar(string assunto, string corpo, List<string> destinatarios)
        {
            var smtp = new SmtpClient("CLIENT_DO_SERVICO_EMAIL")
            {
                Port = 25
            };

            var mail = new MailMessage
            {
                From = new MailAddress("EMAIL_DE"),
                Subject = assunto,
                Body = corpo,
                IsBodyHtml = true
            };

            foreach (var dest in destinatarios)
            {
                mail.To.Add(dest);
            }

            smtp.Send(mail);
            _logger.CaixaTexto("inicio");
            _logger.CaixaTexto("Email enviado.");
            _logger.CaixaTexto("inicio");
        }

        public string GerarCorpoRelatorio(List<BuStatus> dados)
        {
            var sb = new StringBuilder();

            // Cabeçalho
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http-equiv='Content-Type' content='text/html; charset=ISO-8859-1' />");
            sb.AppendLine("<style type='text/css'>");
            sb.AppendLine("body { font-family: tahoma; font-size:13px; }");
            sb.AppendLine("table { border-collapse: collapse; }");
            sb.AppendLine("table, td { border: 1px solid black; text-align:center; padding:5px; }");
            sb.AppendLine("h1 { font-size:20px; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            // Introdução
            sb.AppendLine("<b>Oi</b>,<br><br>");
            sb.AppendLine("Segue relatório diário da execução das BUs.<br><br>");

            // Coloca o ano-mes e a data de execução
            sb.AppendLine($"<b>Executado para o ano-mês:</b> {_ini.AnoMes}.<br>");
            sb.AppendLine($"<b>Executado no dia:</b> {_ini.DataFormatada}.<br><br>");
            sb.AppendLine("<h1>Detalhamento BUs - UN: <i>Grandes Empresas</i></h1>");

            // Tabela
            sb.AppendLine("<table>");
            sb.AppendLine("<tr style='background-color:black; color:#92D050;'>");
            sb.AppendLine("<td><b>BU</b></td>");
            sb.AppendLine("<td><b>Indicador</b></td>");
            sb.AppendLine("<td><b>Quantidade de Registros</b></td>");
            sb.AppendLine("</tr>");

            int idx = 0;

            foreach (var item in dados)
            {
                // Define a cor: par ganha cinza claro (#F2F2F2), ímpar ganha branco (#FFFFFF)
                string corFundo = (idx % 2 == 0) ? "#F2F2F2" : "#FFFFFF";

                sb.AppendLine($"<tr style='background-color:{corFundo}; color:black;'>");
                sb.AppendLine($"<td><b>{item.NomeBU}</b></td>");
                sb.AppendLine($"<td>{item.Indicador}</td>");
                sb.AppendLine($"<td>{item.QtdeRegs}</td>");
                sb.AppendLine("</tr>");

                idx++;
            }

            sb.AppendLine("</table>");

            // Rodapé
            sb.AppendLine("<br><hr>");
            sb.AppendLine("<b>COLOQUE AQUI QUAL SUA GERENCIA E/OU DIRETORIA</b><br>");

            sb.AppendLine("<i>Esse email foi encaminhado automaticamente pelo sistema.</i>");

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        public List<string> ObterDestinatarios()
        {
            var lista = new List<string>();

            string sql = $@"
SELECT email_usuario 
FROM dbo.TABELA_ONDE_FICAM_OS_EMAILS
WHERE status_usuario = 1 
    AND status_alerta_email = 1";

            using (var reader = _db.ExecuteQuery(sql, "DB_PLANEMP"))
            {
                while (reader.Read())
                {
                    lista.Add(Convert.ToString(reader.GetValue(0)));
                }
            }

            return lista;
        }
    }
}