using ConsoleExecBUs.Domain;
using ConsoleExecBUs.Interfaces;

namespace ConsoleExecBUs.Application
{
    public class CircuitoService
    {
        private readonly IStcCommandService _comando;
        private readonly IStcValidationService _valida;
        private readonly IExtraConnectionService _conn;
        private readonly IStcScreenReaderService _leitor;
        private readonly IExtraNavigationService _naveg;

        public CircuitoService(
            IStcCommandService comando,
            IStcValidationService valida,
            IExtraConnectionService conn,
            IStcScreenReaderService leitor,
            IExtraNavigationService naveg)
        {
            _comando = comando;
            _valida = valida;
            _conn = conn;
            _leitor = leitor;
            _naveg = naveg;
        }

        public ResultadoProcessamento ProcessarCaracteristicasProduto(string produto, CircuitoContexto ctx)
        {
            string tela = "";
            _comando.EsperarRetorno(10, 50);

            _naveg.FnFechaAvisosOtimz();

            ctx.Modalidade = _leitor.ObterModalidade();
            string teste = _leitor.LerTelaInteira();

            var resultado = EntraTelaCaracteristicas(produto, ctx);

            if (resultado == ResultadoProcessamento.Inicio)
                return ResultadoProcessamento.Inicio;

            if (resultado == ResultadoProcessamento.ProximoRegistro)
                return ResultadoProcessamento.ProximoRegistro;

            tela = _leitor.LerTelaInteira();

            var resultadoTela = _valida.VerificarErroTela(tela);

            if (resultadoTela == ResultadoProcessamento.Inicio)
                return ResultadoProcessamento.Inicio;



            _comando.EsperarRetorno(250, 25);

            while (_leitor.LerArea(20, 46, 20, 53) != "Situacao")
            {
                _comando.EsperarRetorno(15, 25);
                _comando.EnviarComando("<PF3>");

                _comando.ProcessamentoStc();

                _comando.EsperarRetorno(250, 25);

                tela = _leitor.LerTelaInteira();

                resultadoTela = _valida.VerificarErroTela(tela);

                if (resultadoTela == ResultadoProcessamento.Inicio)
                    return ResultadoProcessamento.Inicio;

            }

            _naveg.FnFechaAvisosOtimz();

            return ResultadoProcessamento.Normal;
        }

        private ResultadoProcessamento ProcessarDadosCirc(CircuitoContexto ctx)
        {
            string tela = "";

            while (_leitor.LerArea(9, 3, 9, 9) != "Estacao")
            {
                _comando.ProcessamentoStc();

                if (_leitor.LerArea(1, 2, 1, 6) == "Enter")
                {
                    ctx.AuxExec = "exec";
                    _conn.FnReconectar();
                    _comando.EsperarRetorno(15, 25);

                    if (ctx.Refazer == 1)
                    {
                        return ResultadoProcessamento.Inicio;
                    }
                }
            }

            tela = _leitor.LerTelaInteira();

            var resultadoTela = _valida.VerificarErroTela(tela);

            if (resultadoTela == ResultadoProcessamento.Inicio)
                return ResultadoProcessamento.Inicio;

            ctx.LocalA = _leitor.ObterLocalA();
            ctx.EstacaoA = _leitor.ObterEstacaoA();
            ctx.GraA = _leitor.ObterGraA();
            ctx.UfB = _leitor.ObterUfB();
            ctx.LocalB = _leitor.ObterLocalB();
            ctx.EstacaoB = _leitor.ObterEstacaoB();
            ctx.GraB = _leitor.ObterGraB();
            ctx.TecRota = _leitor.ObterTecRota();

            _comando.EsperarRetorno(15, 25);

            return ResultadoProcessamento.Normal;
        }

        private void ProcessarOutrasEmpresas(CircuitoContexto ctx)
        {
            string tela = "";

            _comando.EnviarComando("<TAB><TAB><TAB><TAB><TAB><TAB>x<ENTER>");

            if (_leitor.LerArea(19, 56, 19, 70) == "Outras Empresas")
            {
                _comando.EnviarComando("<TAB><TAB>x<ENTER>");

                _comando.ProcessamentoStc();

                _comando.EsperarRetorno(15, 25);

                tela = _leitor.LerTelaInteira();
                if (tela.Contains("NAO EXISTEM OUTRAS EMPRESAS"))
                {
                    ctx.Isp = "NAO EXISTEM OUTRAS EMPRESAS PRA ESTE CCTO";
                    _comando.EnviarComando("<ENTER>");

                    _comando.ProcessamentoStc();

                    _comando.EsperarRetorno(15, 25);
                }
                else
                {
                    ctx.Isp = "ND";
                    short i = 0;

                    for (i = 14; i <= 19; i++)
                    {
                        string tela2 = _leitor.LerArea(i, 26, i, 78);

                        if (tela2.Contains("DESIGNA"))
                        {
                            ctx.Designacao = _leitor.LerArea(i, 46, i, 78);
                            break;
                        }
                        else
                        {
                            ctx.Designacao = "ND";
                        }
                    }

                    ctx.TextoOutrasEmpresas = _leitor.ObterTextoOutrasEmpresas();
                }
            }
        }

        private ResultadoProcessamento EntraTelaCaracteristicas(string produto, CircuitoContexto ctx)
        {
            string tela = "";
            tela = _leitor.LerTelaInteira();

            if (tela.Contains("Numero Circuito...........:"))
            {
                _comando.EnviarComando("<PF4>", 250, 15);
            }

            tela = _leitor.LerTelaInteira();

            var resultadoTela = _valida.VerificarErroTela(tela);

            if (resultadoTela == ResultadoProcessamento.Inicio)
                return ResultadoProcessamento.Inicio;

            tela = _leitor.LerTelaInteira();

            if (tela.Contains("PF4  - Consulta Cadastral"))
            {
                _comando.EnviarComando("<PF4>", 250, 15);
            }

            _comando.EsperarRetorno(10, 25);

            tela = _leitor.LerTelaInteira();

            if (tela.Contains("_ Caracteristicas"))
            {
                var resultado = CalcularDadosProduto(produto, ctx);

                if (resultado == ResultadoProcessamento.Inicio)
                    return ResultadoProcessamento.Inicio;

                if (resultado == ResultadoProcessamento.ProximoRegistro)
                    return ResultadoProcessamento.ProximoRegistro;
            }

            tela = _leitor.LerTelaInteira();

            resultadoTela = _valida.VerificarErroTela(tela);

            if (resultadoTela == ResultadoProcessamento.Inicio)
                return ResultadoProcessamento.Inicio;

            if (!tela.Contains("Configuracao da Rota"))
                _comando.EnviarComando("<PF3>");

            ProcessarOutrasEmpresas(ctx);

            _comando.EsperarRetorno(15, 25);

            if (_leitor.LerArea(9, 18, 9, 27) == "Nao existe")
            {
                _comando.EnviarComando("<PF3>");
                _comando.EnviarComando("<PF3>");
                _comando.EnviarComando("<PF3>");
                _comando.EnviarComando("<PF3>");

                _comando.ProcessamentoStc();
                return ResultadoProcessamento.ProximoRegistro;
            }

            _comando.EnviarComando("<PF3>");
            _comando.EsperarRetorno(250, 150);

            var resultadoPdc = ProcessarDadosCirc(ctx);

            if (resultadoPdc == ResultadoProcessamento.Inicio)
                return ResultadoProcessamento.Inicio;

            if (resultadoPdc == ResultadoProcessamento.ProximoRegistro)
                return ResultadoProcessamento.ProximoRegistro;

            return ResultadoProcessamento.Normal;
        }

        private ResultadoProcessamento CalcularDadosProduto(string produto, CircuitoContexto ctx)
        {
            _comando.EnviarComando("x<ENTER>");
            _comando.ProcessamentoStc();
            _comando.EsperarRetorno(250, 15);

            if (produto.Contains("DT -") && produto.Contains("DP -") || produto.Contains("IS -"))
            {
                ctx.Velocidade = _leitor.ObterVelocidade(15, 41, 15, 80);
                ctx.QtdeFios = _leitor.ObterQtdeFios(13, 41, 13, 80);
            }
            else if (produto.Contains("DM -"))
            {
                ctx.Velocidade = _leitor.ObterVelocidade(10, 41, 10, 80);
                ctx.QtdeFios = _leitor.ObterQtdeFios(13, 41, 13, 80);
            }
            else if (produto.Contains("SL -"))
            {
                ctx.Velocidade = _leitor.ObterVelocidade(12, 41, 12, 80);
                ctx.QtdeFios = _leitor.ObterQtdeFios(14, 41, 14, 80);
            }
            else if (produto.Contains("EV -"))
            {
                ctx.Velocidade = "ND";
                ctx.QtdeFios = "ND";
            }
            else if (produto.Contains("CN -"))
            {
                ctx.Velocidade = _leitor.ObterVelocidade(13, 41, 13, 80);
                ctx.QtdeFios = _leitor.ObterQtdeFios(11, 41, 11, 80);
            }
            else
            {
                ctx.Velocidade = _leitor.ObterVelocidade(15, 41, 15, 80);
                ctx.QtdeFios = _leitor.ObterQtdeFios(11, 41, 11, 80);
            }

            _comando.EnviarComando("<PF3>");
            _comando.EsperarRetorno(10, 10);

            return ResultadoProcessamento.Normal;
        }
    }
}
