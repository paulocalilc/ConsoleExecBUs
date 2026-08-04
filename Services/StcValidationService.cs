using ConsoleExecBUs.Domain;
using ConsoleExecBUs.Interfaces;

namespace ConsoleExecBUs.Services
{
    public class StcValidationService : IStcValidationService
    {
        private readonly IStcScreenReaderService _leitor;
        private readonly IStcCommandService _comando;
        private readonly ILogger _logger;

        public StcValidationService(
            IStcScreenReaderService leitor,
            IStcCommandService comando,
            ILogger logger)
        {
            _leitor = leitor;
            _comando = comando;
            _logger = logger;
        }

        public bool StatusCircuitoValido(string status)
        {
            return status != "INSTALADO" &&
                   status != "HISTORICO" &&
                   status != "PROJETADO" &&
                   status == "NAO EXISTE NENHUM CIRCUITO COM ESSA DESIGNACAO";
        }

        public bool TelaTemErroCritico(string tela)
        {
            var t = tela?.ToUpper();

            var erros = new[]
            {
                "\u008F …†‡", // caracteres corrompidos do STC (erro de tela)
                "UNSUPPORTED FUNCTION",
                "GCBL4NBV UNABLE TO ESTABLISH",
                "APPLICATION",
                "A COMMAND UNRECOGNIZED",
                "*UNSUPPORTED FUNCTION*",
                "GCBH7TEW UNABLE"
            };

            return erros.Any(e => t.Contains(e));
        }

        public bool ErroCampoComando(string tela)
        {
            return tela.Contains("x") ||
                   tela.Contains("X");
        }

        public bool FnTrataCCTOInvalido(CircuitoContexto ctx)
        {
            try
            {
                if (_leitor.LerArea(1, 2, 1, 11) == "NAO EXISTE")
                {
                    ctx.Cliente = "NAO EXISTE NENHUM CIRCUITO COM ESSA DESIGNACAO";
                    _comando.EsperarRetorno(15, 25);
                    _comando.EnviarComando("<Enter>", 250, 25);
                    ctx.ConfirmacaoCircuito = 1;
                }

                string teste = _leitor.LerTelaInteira();

                if (teste.Contains("ELIMINADO"))
                {
                    ctx.Cliente = "CIRCUITO ELIMINADO DAS BASES DO STC";
                    _comando.EnviarComando("<Enter>", 250, 25);
                    ctx.ConfirmacaoCircuito = 1;
                }

                if (teste.Contains("CIRCUITO SEM CARACTERISTICA"))
                {
                    ctx.Cliente = "CIRCUITO SEM CARACTERISTICA";
                    _comando.EnviarComando("<Enter>", 250, 25);
                    string statusCirc = _leitor.LerArea(20, 56, 20, 64);

                    if (statusCirc != "INSTALADO" &&
                           statusCirc != "HISTORICO" &&
                           statusCirc != "PROJETADO" &&
                           statusCirc != "NAO EXISTE NENHUM CIRCUITO COM ESSA DESIGNACAO")
                    {
                        _comando.EsperarRetorno(15, 25);
                        _comando.EnviarComando("<Enter>", 250, 25);
                        statusCirc = _leitor.LerArea(20, 56, 20, 64);
                    }

                    _comando.EsperarRetorno(250, 25);
                    ctx.ConfirmacaoCircuito = 1;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.GerarLog("ERRO", "Erro ao validar circuito do STC.", "ClasseExtra", ex.Message);
                throw new ArgumentException("Erro ao validar circuito do STC. Erro: " + ex.Message);
            }
        }

        public bool DeveReiniciar(string tela, ref bool reiniciar)
        {
            var resultado = VerificarErroTela(tela);

            if (resultado == ResultadoProcessamento.Inicio)
            {
                reiniciar = true;
                return true;
            }

            return false;
        }

        public ResultadoProcessamento VerificarErroTela(string tela)
        {
            if (TelaTemErroCritico(tela))
            {
                //_conn.FnReconectar();
                return ResultadoProcessamento.Inicio;
            }

            return ResultadoProcessamento.Normal;
        }
    }
}
