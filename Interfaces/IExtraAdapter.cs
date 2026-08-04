namespace ConsoleExecBUs.Interfaces
{
    public interface IExtraAdapter
    {
        void SendKeys(string comando);

        void MoveCursor(short linha, short coluna);

        string PegarLinha(short linIni, short colIni, short tamanho);

        string ReadArea(short linIni, short colIni, short linFim, short colFim);

        string ReadScreen();

        void WaitHost(int tempo);

        bool EstaProcessando();

        void AbrirSessao(string arquivoEdp);

        void SelecionarSessao(string arquivoEdp);

        void DefinirTelaAtual();

        void TornarVisivel();

        bool EstaConectado();

        void Conectar();

        void Desconectar();
    }
}
