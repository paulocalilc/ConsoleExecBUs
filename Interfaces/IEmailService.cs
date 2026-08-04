using ConsoleExecBUs.Domain;

namespace ConsoleExecBUs.Interfaces
{
    public interface IEmailService
    {
        void Enviar(string assunto, string corpo, List<string> destinatarios);
        string GerarCorpoRelatorio(List<BuStatus> dados);
        List<string> ObterDestinatarios();
    }
}
