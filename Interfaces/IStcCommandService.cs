namespace ConsoleExecBUs.Interfaces
{
    public interface IStcCommandService
    {
        void EnviarComando(string comando, int tempo = 50, int delay = 25, bool esperar = true);
        void EsperarRetorno(int tempo, int delay);
        void ProcessamentoStc();
        void MoverCursor(short linha, short coluna);

    }
}
