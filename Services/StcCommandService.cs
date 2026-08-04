using ConsoleExecBUs.Interfaces;

namespace ConsoleExecBUs.Services
{
    public class StcCommandService : IStcCommandService
    {
        private readonly IExtraAdapter _extra;

        public StcCommandService(IExtraAdapter extra)
        {
            _extra = extra;
        }

        public void EnviarComando(string comando, int tempo = 50, int delay = 25, bool esperar = true)
        {
            _extra.SendKeys(comando);

            if (esperar)
                EsperarRetorno(tempo, delay);
        }

        public void MoverCursor(short linha, short coluna)
        {
            _extra.MoveCursor(linha, coluna);
        }

        public void EsperarRetorno(int tempo, int delay)
        {
            ProcessamentoStc();
            _extra.WaitHost(tempo);
            Thread.Sleep(delay);
        }

        public void ProcessamentoStc()
        {
            while (_extra.EstaProcessando()) { }
        }

    }
}
