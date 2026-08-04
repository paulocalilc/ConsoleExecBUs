namespace ConsoleExecBUs.Interfaces
{
    public interface ILogger
    {
        public bool GerarLog(string tipo, string mensagem, string classe, string detalhe);
        void CaixaTexto(string mensagem);
    }

}
