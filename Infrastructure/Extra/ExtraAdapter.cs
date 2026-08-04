using ConsoleExecBUs.Interfaces;
using Interop.EXTRA;
using static ConsoleExecBUs.Core.ClasseExtra;

namespace ConsoleExecBUs.Infrastructure.Extra
{
    public class ExtraAdapter : IExtraAdapter
    {
        public void SendKeys(string comando)
        {
            PropMyScreen.MyScreen.SendKeys(comando);
        }

        public void MoveCursor(short linha, short coluna)
        {
            PropMyScreen.MyScreen.MoveTo(linha, coluna);
        }

        public string ReadArea(short linIni, short colIni, short linFim, short colFim)
        {
            return PropMyScreen.MyScreen.Area(linIni, colIni, linFim, colFim, null, 1).Value.Trim();
        }

        public string PegarLinha(short linIni, short colIni, short tamanho)
        {
            return PropMyScreen.MyScreen.GetString(linIni, colIni, tamanho).Trim();
        }

        public string ReadScreen()
        {
            return PropMyScreen.MyScreen.Area(1, 1, 25, 80, null, 1).Value.Trim();
        }

        public void WaitHost(int tempo)
        {
            PropMyScreen.MyScreen.WaitHostQuiet(tempo);
        }

        public bool EstaProcessando()
        {
            return PropMyScreen.MyScreen.OIA.XStatus == 5;
        }

        public void AbrirSessao(string arquivoEdp)
        {
            PropObjExtra.OExtra ??= new ExtraSystem();

            PropSessoes.Sessoes ??= PropObjExtra.OExtra.Sessions;

            PropSessoes.Sessoes.Open(arquivoEdp);
        }

        public void SelecionarSessao(string arquivoEdp)
        {
            PropSessao.Sessao = PropSessoes.Sessoes.Item(arquivoEdp);

            if (PropSessao.Sessao == null)
            {
                throw new ArgumentException($"Sessão não encontrada: {arquivoEdp}");
            }
        }

        public void DefinirTelaAtual()
        {
            PropMyScreen.MyScreen = PropSessao.Sessao.Screen;

            if (PropMyScreen.MyScreen == null)
            {
                throw new ArgumentException("Screen não encontrado.");
            }
        }

        public void TornarVisivel()
        {
            if (PropSessao.Sessao.Visible == 0)
            {
                PropSessao.Sessao.Visible = 1;
            }
        }

        public bool EstaConectado()
        {
            return PropSessao.Sessao.Connected == 1;
        }

        public void Conectar()
        {
            PropSessao.Sessao.Connected = 1;
        }

        public void Desconectar()
        {
            PropSessao.Sessao.Connected = 0;
        }
    }
}
