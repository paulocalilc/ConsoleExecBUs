namespace ConsoleExecBUs.Interfaces
{
    public interface IStcScreenReaderService
    {
        public string ObterCliente();
        public string ObterCnpj();
        public string ObterProduto();
        public string ObterStatus();
        public string ObterUfA();
        public string ObterTipoPessoa();
        public string ObterTipoLogradA();
        public string ObterLogradA();
        public string ObterNumA();
        public string ObterBairroA();
        public string ObterModalidade();
        public string ObterLocalA();
        public string ObterEstacaoA();
        public string ObterGraA();
        public string ObterUfB();
        public string ObterLocalB();
        public string ObterEstacaoB();
        public string ObterGraB();
        public string ObterTecRota();
        public string ObterDesignacao();
        public string ObterVelocidade(short LinIni, short ColIni, short LinFim, short ColFim);
        public string ObterQtdeFios(short LinIni, short ColIni, short LinFim, short ColFim);
        public string LerArea(short linIni, short colIni, short linFim, short colFim);
        public string LerTelaInteira();
        public List<string> ObterLinhasOutrasEmpresas();
        public string ObterTextoOutrasEmpresas();
    }
}
