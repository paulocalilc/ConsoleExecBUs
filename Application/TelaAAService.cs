using ConsoleExecBUs.Domain;
using ConsoleExecBUs.Interfaces;

namespace ConsoleExecBUs.Application
{
    public class TelaAAService
    {
        private readonly IStcCommandService _comando;
        private readonly IStcScreenReaderService _leitor;
        private readonly IStcValidationService _valida;
        private readonly IExtraConnectionService _conn;
        private readonly IExtraNavigationService _naveg;
        private readonly ILogger _logger;

        public TelaAAService(
            IStcCommandService comando,
            IStcScreenReaderService leitor,
            IStcValidationService valida,
            IExtraConnectionService conn,
            IExtraNavigationService naveg,
            ILogger logger)
        {
            _comando = comando;
            _leitor = leitor;
            _valida = valida;
            _conn = conn;
            _naveg = naveg;
            _logger = logger;
        }

        public bool Processar(SipContexto ctx)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ctx.NumeroChave))
                    return true;

                if (!_naveg.FnTelaInicialAA())
                    return false;

                _comando.EsperarRetorno(250, 50);
                ConsultarTerminal(ctx);

                if (ctx.Produto == "PORTADO" || 
                        ctx.Produto == "NÃO ENCONTRADO" || 
                        ctx.Produto == "VAGO")
                    return true;

                _comando.EsperarRetorno(250, 50);
                ObterDadosOs(ctx);
                return true;
            }
            catch (Exception ex)
            {
                _logger.GerarLog("ERRO", "Erro ao processar tela AA.", nameof(TelaAAService), ex.Message);
                return false;
            }
        }

        private void ConsultarTerminal(SipContexto ctx)
        {
            var terminal = SomenteNumeros(ctx.NumeroChave);

            if (terminal.Length > 8)
            {
                terminal = terminal.Substring(terminal.Length - 8);
            }

            _comando.MoverCursor(5, 12);
            _comando.EsperarRetorno(250, 50);
            _comando.EnviarComando("<eraseeof>", 250, 50);
            _comando.EsperarRetorno(250, 50);
            _comando.EnviarComando(terminal, 250, 50);
            _comando.MoverCursor(5, 30);
            _comando.EsperarRetorno(250, 50);
            _comando.EnviarComando("<eraseeof>", 250, 50);
            _comando.EnviarComando(ctx.Localidade, 250, 50);
            _comando.EnviarComando("<ENTER>", 250, 50);

            _comando.EsperarRetorno(250, 250);
            string tela = _leitor.LerArea(7, 15, 7, 50);

            if (tela == "Selecione a localidade desejada:")
            {
                short idx = 11;
                for (idx = 11; idx <= 20; idx++)
                {
                    _comando.EsperarRetorno(250, 250);
                    string loc = _leitor.LerArea(idx, 68, idx, 72);

                    if (loc == ctx.Localidade)
                    {
                        _comando.MoverCursor(idx, 17);
                        _comando.EnviarComando("X", 125, 125);
                        _comando.EnviarComando("<ENTER>", 250, 125);
                        _comando.EsperarRetorno(250, 250);
                        break;
                    }

                    ctx.Produto = "NÃO ENCONTRADO";
                    ctx.MicroServico = "NÃO ENCONTRADO";
                    ctx.NumeroOs = "NÃO ENCONTRADO";
                    ctx.StatusOs = "ND";

                    _comando.EnviarComando("<PF3>", 250, 50);
                } 
            }

            tela = _leitor.LerTelaInteira();

            if (tela.Contains("A T E N C A O - CGS000"))
            {
                ctx.Produto = "PORTADO";
                ctx.MicroServico = "PORTADO";
                ctx.NumeroOs = "PORTADO";
                ctx.StatusOs = "PORTADO";

                _comando.EnviarComando("<ENTER>", 250, 50);
            }

            tela = _leitor.LerArea(16, 54, 16, 80);

            if (tela == "DISPONIVEL")
            {
                ctx.Produto = "VAGO";
                ctx.MicroServico = "ND";
                ctx.NumeroOs = "ND";
                ctx.StatusOs = "ND";
            }

            _naveg.FnFechaAvisosOtimz();

            //tela = _leitor.LerArea(16, 54, 16, 80);

            //if (tela == "DISPONIVEL" || tela == "PENDENCIA EMPRESA")
            //{
            //    ctx.Produto = "VAGO";
            //    ctx.MicroServico = "ND";
            //    ctx.NumeroOs = "ND";
            //    ctx.StatusOs = "ND";
            //}
        }


        private string SomenteNumeros(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;

            return new string(valor.Where(char.IsDigit) .ToArray());
        }


        private void ObterDadosOs(SipContexto ctx)
        {
            _comando.EsperarRetorno(250, 250);
            _comando.EnviarComando("<PF6>", 250, 50);
            _comando.EsperarRetorno(250, 250);

            _naveg.FnFechaAvisosOtimz();

            if (ProcessarOsAbertas(ctx))
                return;

            if (ProcessarGeracaoProtocolo(ctx))
                return;

            ProcessarAlteracaoOs(ctx);
        }

        private bool ProcessarOsAbertas(SipContexto ctx)
        {
            bool flagExec = true;

            _comando.EsperarRetorno(250, 250);
            var tela = _leitor.LerArea(12, 28, 12, 50);
            if (tela != "Relacao de 'OS' abertas")
                return false;

            SelecionarPrimeiraOs();

            _comando.EsperarRetorno(250, 250);
            tela = _leitor.LerArea(3, 18, 3, 64);
            if (tela != "ALTERACAO/CANCELAMENTO DE OS")
                PercorrerHistoricoOs(ctx);

            if (tela == "ALTERACAO/CANCELAMENTO DE OS")
                return false;

            _naveg.FnTelaInicialAA();

            _comando.EsperarRetorno(250, 100);
            tela = _leitor.LerArea(5, 1, 5, 11);
            if (tela != "Telefone:")
            {
                _conn.FnReconectar();
                _naveg.FnTelaInicialAA();
            }

            return true;
        }

        private bool ProcessarGeracaoProtocolo(SipContexto ctx)
        {
            _comando.EsperarRetorno(250, 250);
            var tela = _leitor.LerArea(16, 28, 16, 58);
            if (tela != "GERAR PROTOCOLO DE ATENDIMENTO?")
                return false;

            _comando.EnviarComando("<PF3>", 250, 100);
            _comando.EnviarComando("<PF3>", 250, 100);

            _naveg.FnFechaAvisosOtimz();

            if(_leitor.LerArea(5, 1, 5, 11) != "Telefone:")
            {
                _conn.FnReconectar();
                _naveg.FnTelaInicialAA();
                ConsultarTerminal(ctx);
                ObterDadosOs(ctx);

                return true;
            }

            _comando.MoverCursor(24, 6);
            _comando.EsperarRetorno(250, 50);

            _comando.EnviarComando("ACH", 250, 50);

            _comando.EnviarComando("<ENTER>", 250, 50);

            _comando.EnviarComando("<ENTER>", 250, 50);

            if (_leitor.LerArea(3, 24, 3, 58) != "HISTORICO / OCORRENCIAS DE SERVICOS")
                return false;

            PercorrerHistoricoOs(ctx);

            _naveg.FnTelaInicialAA();

            return true;
        }

        private void SelecionarPrimeiraOs()
        {
            _comando.MoverCursor(16, 6);
            _comando.EsperarRetorno(250, 50);
            _comando.EnviarComando("X<ENTER>", 250, 120);
        }

        private void PercorrerHistoricoOs(SipContexto ctx)
        {
            short linha = 8;

            var msgm = _leitor.LerArea(linha, 5, linha, 18);

            while (!string.IsNullOrWhiteSpace(msgm))
            {
                var numeroOs = _leitor.LerArea(linha, 5, linha, 18);

                var statusOs = _leitor.LerArea(linha, 23, linha, 37);

                if (AbrirDetalheOs(linha))
                {
                    ctx.MicroServico = "MIGPRO";
                    ctx.NumeroOs = numeroOs;
                    ctx.StatusOs = statusOs;

                    return;
                }

                linha++;
                _comando.EnviarComando("<PF3>", 250, 120);
                msgm = _leitor.LerArea(linha, 5, linha, 18);
            }
        }

        private bool AbrirDetalheOs(int linha)
        {
            _comando.MoverCursor((short)linha, 3);
            _comando.EsperarRetorno(250, 50);

            _comando.EnviarComando("X<ENTER>", 250, 120);

            TratarAlerta();
            TratarMotivoRetirada();

            return _leitor.LerTelaInteira().Contains("MIGPRO");
        }

        private void TratarAlerta()
        {
            if (_leitor.LerArea(17, 34, 17, 44) == "A L E R T A")
            {
                _comando.EnviarComando("<ENTER>", 250, 120);
            }
        }

        private void TratarMotivoRetirada()
        {
            if (_leitor.LerArea(9, 23, 9, 56) == "DETALHAMENTO DO MOTIVO DE RETIRADA")
                _comando.EnviarComando("<ENTER>", 250, 120);
        }

        private bool ProcessarAlteracaoOs(SipContexto ctx)
        {
            _comando.EsperarRetorno(250, 250);
            var tela = _leitor.LerArea(3, 19, 3, 46);
            if (_leitor.LerArea(3, 19, 3, 46) != "ALTERACAO/CANCELAMENTO DE OS")
                return false;

            _comando.EnviarComando("<PF8>", 250, 50);

            ctx.NumeroOs = _leitor.LerArea(4, 13, 4, 26);
            _comando.EsperarRetorno(250, 50);

            ctx.MicroServico = LerMicroServico();
            _comando.EsperarRetorno(250, 50);

            ctx.StatusOs = "ABERTA";

            _comando.EnviarComando("<PF3>", 250, 50);

            _naveg.FnTelaInicialAA();

            return true;
        }

        private string LerMicroServico()
        {
            var linhas = new List<string>();

            short linha = 8;

            while (!string.IsNullOrWhiteSpace(_leitor.LerArea(linha,41,linha,80)))
            {
                linhas.Add(_leitor.LerArea(linha,41,linha,80));
                linha++;
            }

            return string.Join(" / ", linhas);
        }
    }
}