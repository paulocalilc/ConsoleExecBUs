using ConsoleExecBUs.Domain;
using ConsoleExecBUs.Interfaces;

namespace ConsoleExecBUs.Application
{
    public class Tela181Service
    {
        private readonly IStcCommandService _comando;
        private readonly IStcValidationService _valida;
        private readonly IExtraConnectionService _conn;
        private readonly IStcScreenReaderService _leitor;
        private readonly IExtraNavigationService _naveg;
        private readonly ILogger _logger;

        public Tela181Service(
            IStcCommandService comando,
            IStcValidationService valida,
            IExtraConnectionService conn,
            IStcScreenReaderService leitor,
            IExtraNavigationService naveg,
            ILogger logger)
        {
            _comando = comando;
            _valida = valida;
            _conn = conn;
            _leitor = leitor;
            _naveg = naveg;
            _logger = logger;
        }

        public bool ProcessarTela181(CircuitoContexto ctx)
        {
            try
            {
                _comando.EnviarComando("<PF10>");

                if (_naveg.FnTelaInicial181())
                {
                    bool reiniciar = true;

                    do
                    {
                        var tela = _leitor.LerTelaInteira();

                        if (_valida.TelaTemErroCritico(tela))
                        {
                            _conn.FnReconectar();
                            reiniciar = false;
                            break;
                        }

                        tela = _leitor.LerTelaInteira();

                        if (string.IsNullOrWhiteSpace(ctx.LocNum))
                        {
                            EnderecoVazio(ctx);

                            _comando.EnviarComando("<PF3>", 250, 50);
                            _comando.EnviarComando("<PF11>", 250, 50);
                            _naveg.FnTelaInicial113CA();
                            _comando.EsperarRetorno(250, 150);

                            return true;
                        }

                        if (string.IsNullOrWhiteSpace(ctx.LogradouroA))
                        {
                            EnderecoVazio(ctx);

                            _comando.EnviarComando("<PF3>", 250, 50);
                            _comando.EnviarComando("<PF11>", 250, 50);
                            _naveg.FnTelaInicial113CA();
                            _comando.EsperarRetorno(250, 150);

                            return true;
                        }

                        _comando.MoverCursor(9, 26);
                        _comando.EsperarRetorno(15, 50);
                        _comando.EnviarComando(ctx.LocNum, 250);
                        _comando.MoverCursor(13, 26);
                        _comando.EsperarRetorno(15, 50);
                        _comando.EnviarComando(ctx.LogradouroA, 250);
                        _comando.EnviarComando("<Enter>", 250, 15);

                        tela = _leitor.LerTelaInteira();

                        if (_valida.DeveReiniciar(tela, ref reiniciar))
                        {
                            _conn.FnReconectar();
                            break;
                        }

                        if (_leitor.LerArea(1, 2, 1, 34) == "Nome de Logradouro Nao Encontrado")
                        {
                            _comando.EnviarComando("<PF3>");
                            _comando.EnviarComando("5");
                            _comando.EnviarComando("<ENTER>");
                            EnderecoVazio(ctx);

                            _comando.EnviarComando("<PF3>", 250, 50);
                            _comando.EnviarComando("<PF11>", 250, 50);
                            _naveg.FnTelaInicial113CA();
                            _comando.EsperarRetorno(250, 150);

                            return true;
                        }

                        if (_leitor.LerArea(10, 22, 10, 57) == "Logradouro Inexistente na Localidade")
                        {
                            _comando.EnviarComando("<PF3>");
                            _comando.EnviarComando("<PF3>");
                            EnderecoVazio(ctx);
                            return true;
                        }

                        if (_leitor.LerArea(2, 2, 2, 22) == "VAB00607 1960 NAT1313")
                        {
                            _comando.EnviarComando("<PF3>");
                            _comando.EnviarComando("<PF3>");
                            EnderecoVazio(ctx);

                            _comando.EnviarComando("<PF3>", 250, 50);
                            _comando.EnviarComando("<PF11>", 250, 50);
                            _naveg.FnTelaInicial113CA();
                            _comando.EsperarRetorno(250, 150);

                            return true;
                        }

                        short contLin = 12;
                        int rodada185 = 1;
                        string comando = "<PF8>";
                        ctx.Bairro = ctx.BairroA.Trim();

                        _comando.EsperarRetorno(15, 250);

                        //int valida = 0;
                        string tela181 = _leitor.LerArea(contLin, 23, contLin, 47);

                        while (tela181.Trim() != "")
                        {
                            int cont185 = 2;
                            string logradStc = _leitor.LerArea(contLin, 23, contLin, 56);

                            if (contLin > 21)
                            {
                                EnderecoVazio(ctx);

                                _comando.EnviarComando("<PF3>", 250, 50);
                                _comando.EnviarComando("<PF11>", 250, 50);
                                _naveg.FnTelaInicial113CA();
                                _comando.EsperarRetorno(250, 150);

                                return true;
                            }

                            _comando.EsperarRetorno(15, 25);

                            if (ctx.LogradouroCompleto == logradStc)
                            {
                                _comando.MoverCursor(contLin, 7);
                                _comando.EnviarComando("x");
                                _comando.EnviarComando("<Enter>");

                                ctx.BairroSTC = _leitor.LerArea(15, 32, 15, 70);

                                if (contLin > 20)
                                {
                                    rodada185 += 1;
                                    contLin = 12;
                                }

                                if (rodada185 > 1)
                                {
                                    if (cont185 != rodada185)
                                    {
                                        comando = comando + "<PF8>";
                                        cont185 = rodada185;
                                    }

                                    _comando.EnviarComando(comando);

                                    _comando.ProcessamentoStc();

                                    if (cont185 > 8)
                                    {
                                        EnderecoVazio(ctx);

                                        _comando.EnviarComando("<PF3>", 250, 50);
                                        _comando.EnviarComando("<PF11>", 250, 50);
                                        _naveg.FnTelaInicial113CA();
                                        _comando.EsperarRetorno(250, 150);

                                        return true;
                                    }
                                }

                                if (ctx.Bairro?.Trim() == ctx.BairroSTC?.Trim())
                                {
                                    _comando.EsperarRetorno(250, 25);
                                    ctx.CodLocal = _leitor.LerArea(7, 32, 7, 36);
                                    ctx.CodLogradouro = _leitor.LerArea(9, 32, 9, 36);
                                    ctx.Endereco = _leitor.LerArea(13, 32, 13, 80);
                                    ctx.LogradouroTrechado = _leitor.LerArea(11, 32, 11, 80);
                                    ctx.Cep = _leitor.LerArea(17, 32, 17, 40);

                                    _comando.EnviarComando("<PF3>", 250, 50);
                                    _comando.EnviarComando("<PF11>", 250, 50);
                                    _naveg.FnTelaInicial113CA();
                                    _comando.EsperarRetorno(250, 150);

                                    return true;
                                }
                                else
                                {
                                    _comando.EnviarComando("<PF3>", 250);

                                    _comando.MoverCursor(21, 27);
                                    _comando.EnviarComando("5", 250);

                                    _comando.EnviarComando("<Enter>", 250);

                                    _comando.MoverCursor(9, 26);
                                    _comando.EnviarComando(ctx.LocNum, 250);

                                    _comando.MoverCursor(13, 26);
                                    _comando.EnviarComando(ctx.LogradouroA, 250);

                                    _comando.EnviarComando("<Enter>", 250);
                                }
                            }

                            contLin += 1;

                            _comando.ProcessamentoStc();

                            if (_leitor.LerArea(25, 9, 25, 13) == "\u008F …†‡")
                            {
                                _comando.EnviarComando("<ESCAPE>");
                                _comando.ProcessamentoStc();
                            }

                            if (_valida.DeveReiniciar(_leitor.LerTelaInteira(), ref reiniciar))
                            {
                                _conn.FnReconectar();
                                break;
                            }
                        }

                    } while (reiniciar);
                }

                _comando.EnviarComando("<PF3>", 250, 50);
                _comando.EnviarComando("<PF11>", 250, 50);
                _naveg.FnTelaInicial113CA();
                _comando.EsperarRetorno(250, 150);

                return true;
            }
            catch (Exception ex)
            {
                _logger.GerarLog("ERRO", "Erro ao executar a consulta no STC (tela 118).", "ClasseExtra", ex.Message);
                _logger.CaixaTexto(string.Concat("Erro ao executar a consulta no STC (tela 118). Erro: ", ex.Message));
                return false;
            }
        }

        private void EnderecoVazio(CircuitoContexto ctx)
        {
            ctx.CodLocal = "N/E";
            ctx.CodLogradouro = "N/E";
            ctx.Endereco = "N/E";
            ctx.LogradouroTrechado = "N/E";
            ctx.Cep = "N/E";
            ctx.BairroSTC = "N/E";
        }
    }
}
