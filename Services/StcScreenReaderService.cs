using ConsoleExecBUs.Interfaces;

namespace ConsoleExecBUs.Services
{
    public class StcScreenReaderService : IStcScreenReaderService
    {

        private readonly IExtraAdapter _extra;

        public StcScreenReaderService(IExtraAdapter extra)
        {
            _extra = extra;
        }

        public string ObterCliente()
        {
            return _extra.ReadArea(7, 19, 7, 80);
        }

        public string ObterCnpj()
        {
            return _extra.ReadArea(8, 21, 8, 37);
        }

        public string ObterProduto()
        {
            return _extra.ReadArea(5, 37, 5, 60);
        }

        public string ObterStatus()
        {
            return _extra.ReadArea(20, 56, 20, 64);
        }

        public string ObterUfA()
        {
            return _extra.ReadArea(12, 27, 12, 28);
        }

        public string ObterTipoPessoa()
        {
            return _extra.ReadArea(8, 54, 8, 78);
        }
        public string ObterTipoLogradA()
        {
            return _extra.ReadArea(12, 31, 12, 34);
        }

        public string ObterLogradA()
        {
            return _extra.ReadArea(12, 37, 12, 71);
        }

        public string ObterNumA()
        {
            return _extra.ReadArea(12, 72, 12, 78);
        }

        public string ObterBairroA()
        {
            return _extra.ReadArea(13, 21, 13, 40);
        }

        public string ObterModalidade()
        {
            return _extra.ReadArea(8, 56, 8, 80);
        }

        public string ObterLocalA()
        {
            return _extra.ReadArea(11, 3, 11, 6);
        }

        public string ObterEstacaoA()
        {
            return _extra.ReadArea(11, 7, 11, 11);
        }

        public string ObterGraA()
        {
            return _extra.ReadArea(11, 16, 11, 18);
        }

        public string ObterUfB()
        {
            return _extra.ReadArea(11, 37, 11, 38);
        }

        public string ObterLocalB()
        {
            return _extra.ReadArea(11, 27, 11, 30);
        }

        public string ObterEstacaoB()
        {
            return _extra.ReadArea(11, 32, 11, 35);
        }

        public string ObterGraB()
        {
            return _extra.ReadArea(11, 40, 11, 42);
        }

        public string ObterTecRota()
        {
            return _extra.ReadArea(11, 51, 11, 54);
        }

        public string ObterDesignacao()
        {
            return _extra.ReadArea(11, 51, 11, 54);
        }

        public string ObterVelocidade(short linIni, short colIni, short linFim, short colFim)
        {
            return _extra.ReadArea(linIni, colIni, linFim, colFim);
        }

        public string ObterQtdeFios(short linIni, short colIni, short linFim, short colFim)
        {
            return _extra.ReadArea(linIni, colIni, linFim, colFim);
        }

        public string LerArea(short linIni, short colIni, short linFim, short colFim)
        {
            return _extra.ReadArea(linIni, colIni, linFim, colFim).Trim();
        }

        public string LerTelaInteira()
        {
            return _extra.ReadScreen();
        }

        public List<string> ObterLinhasOutrasEmpresas()
        {
            return new List<string>
            {
                LerArea(14, 26, 14, 78),
                LerArea(15, 26, 15, 78),
                LerArea(16, 26, 16, 78),
                LerArea(17, 26, 17, 78),
                LerArea(18, 26, 18, 78),
                LerArea(19, 26, 19, 78)
            };
        }

        public string ObterTextoOutrasEmpresas()
        {
            return string.Join(" ", ObterLinhasOutrasEmpresas()
                .Where(l => !string.IsNullOrWhiteSpace(l)));
        }



    }
}
