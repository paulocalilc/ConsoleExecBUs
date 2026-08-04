using ConsoleExecBUs.Domain;
using ConsoleExecBUs.Interfaces;

namespace ConsoleExecBUs.Application
{
    public class SipProcessor
    {
        private readonly IExtraConnectionService _conn;
        private readonly ILogger _logger;
        private readonly TelaCAService _telaCA;
        private readonly TelaAAService _telaAA;

        public SipProcessor(
            IExtraConnectionService conn,
            ILogger logger,
            TelaCAService telaCA,
            TelaAAService telaAA)
        {
            _conn = conn;
            _logger = logger;
            _telaCA = telaCA;
            _telaAA = telaAA;
        }

        public void Processar(IEnumerable<SipContexto> registros)
        {
            var grupos = registros.GroupBy(x => x.Ambiente);

            foreach (var grupo in grupos)
            {
                //_logger.CaixaTexto($"Ambiente {grupo.Key}: {grupo.Count()} registros");
                ProcessarAmbiente(grupo.Key, grupo);
            }
        }

        private void ProcessarAmbiente(
            AmbienteStc ambiente,
            IEnumerable<SipContexto> registros)
        {
            try
            {
                _logger.CaixaTexto($"Processando ambiente {ambiente}");
                _logger.CaixaTexto($"Ambiente {ambiente}: {registros.Count()} registros");

                _conn.TrocarAmbiente(ambiente);

                ProcessarTelaCA(registros);

                ProcessarTelaAA(registros);
            }
            catch (Exception ex)
            {
                _logger.GerarLog("ERRO", $"Erro ao processar ambiente {ambiente}", nameof(SipProcessor), ex.Message);
                throw;
            }
        }

        private void ProcessarTelaCA(IEnumerable<SipContexto> registros)
        {
            foreach (var registro in registros)
            {
                _telaCA.Processar(registro);
            }
        }

        private void ProcessarTelaAA(IEnumerable<SipContexto> registros)
        {
            foreach (var registro in registros.Where(x => !string.IsNullOrWhiteSpace(x.NumeroChave)))
            {
                _telaAA.Processar(registro);
            }
        }
    }
}
