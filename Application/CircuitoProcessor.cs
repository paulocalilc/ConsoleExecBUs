using System.Data.SqlClient;
using ConsoleExecBUs.Domain;
using ConsoleExecBUs.Interfaces;

namespace ConsoleExecBUs.Application
{
    public class CircuitoProcessor
    {
        public SqlDataReader? drResultado = null;
        public SqlDataReader? drResult = null;
        public SqlDataReader? drCarga = null;
        private readonly IStcCommandService _comando;
        private readonly IStcValidationService _valida;
        private readonly IExtraConnectionService _conn;
        private readonly IStcScreenReaderService _leitor;
        private readonly IExtraNavigationService _naveg;
        private readonly IDatabaseService _db;
        private readonly CircuitoService _circuitoService;
        private readonly Tela181Service _tela181Service;
        private readonly CircuitoContexto ctx;

        public CircuitoProcessor(
            IStcCommandService comando,
            IStcValidationService valida,
            IExtraConnectionService conn,
            IStcScreenReaderService leitor,
            IExtraNavigationService naveg,
            IDatabaseService db,
            CircuitoService circuitoService,
            Tela181Service tela181Service)
        {
            drResultado = drResultado;
            drResult = drResult;
            drCarga = drCarga;
            _comando = comando;
            _valida = valida;
            _conn = conn;
            _leitor = leitor;
            _naveg = naveg;
            _db = db;
            _circuitoService = circuitoService;
            _tela181Service = tela181Service;
        }

        public void ProcessarRegistros(SqlDataReader drResultado)
        {
            while (drResultado.Read())
            {
                ProcessarRegistro(drResultado);
            }
        }

        private void ProcessarRegistro(SqlDataReader drResultado)
        {
            string sql = "";
            var ctx = new CircuitoContexto
            {
                Localidade = drResultado["LOCALIDADE"].ToString(),
                Circuito = drResultado["CIRCUITO"].ToString(),
                LocNum = drResultado["LOCALIDADE_NUMERICA"].ToString(),
            };

            ctx.AuxExec = "";
            ctx.QtdRegistrosProcessados += 1;

            _comando.EsperarRetorno(10, 75);
            string tela = "";

            bool reiniciar;

            do
            {
                reiniciar = false;

                if (_valida.DeveReiniciar(_leitor.LerTelaInteira(), ref reiniciar))
                {
                    _conn.FnReconectar();
                    break;
                }
                ctx.Refazer = 0;

                int contador = 1;

                tela = _leitor.LerArea(5, 2, 5, 9);

                while (tela != "Circuito")
                {
                    _comando.EsperarRetorno(10, 25);

                    tela = _leitor.LerTelaInteira();

                    if (_valida.DeveReiniciar(tela, ref reiniciar))
                    {
                        _conn.FnReconectar();
                        break;
                    }

                    if (contador > 30)
                    {
                        _comando.EnviarComando("<PF11>", 250, 100);
                        _naveg.FnTelaInicial113CA();
                    }
                    contador++;
                }

                PreencherCircuito(ctx.Localidade, ctx.Circuito);

                _naveg.FnFechaAvisosOtimz();
                ctx.ConfirmacaoCircuito = 0;

                tela = _leitor.LerTelaInteira();

                if (_valida.DeveReiniciar(tela, ref reiniciar))
                {
                    _conn.FnReconectar();
                    break;
                }

                if (_valida.StatusCircuitoValido(ctx.StatusCircuito))
                {
                    _comando.EnviarComando("<PF3>");
                    _valida.FnTrataCCTOInvalido(ctx);
                    return;
                }
                else
                {

                    tela = _leitor.LerTelaInteira();
                    if (_valida.DeveReiniciar(tela, ref reiniciar))
                    {
                        _conn.FnReconectar();
                        break;
                    }

                    tela = _leitor.LerArea(23, 8, 23, 8);
                    if (_valida.ErroCampoComando(tela))
                    {
                        _comando.MoverCursor(23, 8);
                        _comando.EnviarComando("<eraseeof>", 50, 20);

                        reiniciar = true;
                        break;
                    }

                    tela = _leitor.LerArea(23, 2, 23, 6);
                    if (tela == "Opcao")
                    {
                        _conn.FnReconectar();
                        reiniciar = true;
                        break;
                    }

                    _comando.EsperarRetorno(50, 50);

                    LerDadosPrincipais(ctx);

                    _comando.EnviarComando("<PF9>");

                    var resultado = _circuitoService.ProcessarCaracteristicasProduto(ctx.Produto, ctx);
                    if (resultado == ResultadoProcessamento.ProximoRegistro)
                    {
                        return;
                    }

                    tela = _leitor.LerTelaInteira();
                    if (_valida.DeveReiniciar(tela, ref reiniciar))
                    {
                        _conn.FnReconectar();
                        break;
                    }

                    _comando.MoverCursor(24, 6);
                    _comando.EnviarComando("CA", 250, 50);
                    _comando.EnviarComando("<Enter>", 250, 25);

                    tela = _leitor.LerTelaInteira();
                    if (_valida.DeveReiniciar(tela, ref reiniciar))
                        break;

                    ctx.UfA = ctx.UfA.Replace(" ", "");

                    sql = _db.GerarSelect(
                        "TBL_AUX_DE_PARA_ENDERECO",
                        ObterColunasLogradouro(),
                        MontarWhereLogradouro(ctx)
                    );

                    drResult = _db.ExecuteQuery(sql, "DB_PLANEMP");

                    if (drResult.HasRows)
                    {
                        while (drResult.Read())
                        {
                            ctx.TipoLogradBD = drResult["NOME_ITEM"].ToString();
                            ctx.TipoLogrCorrigido = drResult["NOME_POR_EXTENSO"].ToString();
                            sql = string.Empty;

                            if (ctx.TipoLogradouroA == ctx.TipoLogradBD)
                            {
                                ctx.LogradouroCompleto = string.Concat(ctx.TipoLogrCorrigido, " ", ctx.LogradouroA);
                                break;
                            }

                            ctx.LogradouroCompleto = string.Concat(ctx.TipoLogradouroA, " ", ctx.LogradouroA);
                        }
                    }
                    else
                    {
                        ctx.LogradouroCompleto = ctx.LogradouroA;
                    }

                    if (_tela181Service.ProcessarTela181(ctx))
                    {
                        _comando.EsperarRetorno(50, 50);


                        var campos = MontarCamposUpdateCircuito(ctx);
                        var where = MontarWhereCircuito(ctx);

                        sql = _db.GerarUpdate(
                            "BDCMEMP.DBO.TBL_PERM_MACRO_PLANTA",
                            campos,
                            where
                        );

                        _db.ExecuteQuery(sql, "BDCMEMP");

                        ctx.ContCarga++;
                        sql = string.Empty;

                    }
                }

            } while (reiniciar);

        }

        private void PreencherCircuito(string localidade, string circuito)
        {
            _comando.MoverCursor(5, 12);
            _comando.EnviarComando("<eraseeof>", 250, 50);
            _comando.EnviarComando(localidade, 250, 50);

            _comando.MoverCursor(5, 17);
            _comando.EnviarComando("<eraseeof>", 250, 50);
            _comando.EnviarComando(circuito, 250, 50);
            _comando.EnviarComando("<Enter>", 250, 50);
        }

        private void LerDadosPrincipais(CircuitoContexto ctx)
        {
            ctx.Cliente = _leitor.ObterCliente();
            ctx.Cnpj = _leitor.ObterCnpj();
            ctx.Produto = _leitor.ObterProduto();
            ctx.StatusCircuito = _leitor.ObterStatus();
            ctx.UfA = _leitor.ObterUfA();
            ctx.TipoPessoa = _leitor.ObterTipoPessoa();
            ctx.TipoLogradouroA = _leitor.ObterTipoLogradA();
            ctx.LogradouroA = _leitor.ObterLogradA();
            ctx.NumeroA = _leitor.ObterNumA();
            ctx.BairroA = _leitor.ObterBairroA();
            ctx.LogradouroCompleto = string.Concat(ctx.TipoLogradouroA, " ", ctx.LogradouroA);
        }

        private List<string> ObterColunasLogradouro()
        {
            return new List<string>
            {
                "NOME_ITEM",
                "NOME_POR_EXTENSO",
                "PADRAO"
            };
        }

        private Dictionary<string, string> MontarWhereLogradouro(CircuitoContexto ctx)
        {
            return new Dictionary<string, string>
            {
                ["NOME_ITEM"] = ctx.TipoLogradouroA
            };
        }

        private Dictionary<string, string> MontarCamposUpdateCircuito(CircuitoContexto ctx)
        {
            return new Dictionary<string, string>
            {
                ["CLIENTE"] = ctx.Cliente,
                ["CNPJ"] = ctx.Cnpj,
                ["PRODUTO"] = ctx.Produto,
                ["STATUS"] = ctx.StatusCircuito,
                ["VELOCIDADE"] = ctx.Velocidade ?? null,
                ["UFA"] = ctx.UfA ?? null,
                ["LOCAL_A"] = ctx.LocalA ?? null,
                ["EST_A"] = ctx.EstacaoA ?? null,
                ["GRA_A"] = ctx.GraA ?? null,
                ["UF_B"] = ctx.UfB ?? null,
                ["LOCAL_B"] = ctx.LocalB ?? null,
                ["EST_B"] = ctx.EstacaoB ?? null,
                ["GRA_B"] = ctx.GraB ?? null,
                ["TEC_ROTA"] = ctx.TecRota ?? null,
                ["TIPO_PESSOA"] = ctx.TipoPessoa ?? null,
                ["TIPO_LOGRAD_A"] = ctx.TipoLogrCorrigido ?? null,
                ["LOGRAD_A"] = ctx.LogradouroA ?? null,
                ["NUM_A"] = ctx.NumeroA ?? null,
                ["BAIRRO_A"] = ctx.BairroA ?? null,
                ["COD_LOCAL"] = ctx.CodLocal ?? null,
                ["COD_LOGRAD"] = ctx.CodLogradouro ?? null,
                ["LOGRAD_TRECHADO"] = ctx.LogradouroTrechado ?? null,
                ["ENDERECO_STC"] = ctx.Endereco ?? null,
                ["BAIRRO_STC"] = ctx.BairroSTC ?? null,
                ["CEP"] = ctx.Cep ?? null,
                ["ISP"] = ctx.Isp ?? null,
                ["DESIGNACAO"] = ctx.Designacao ?? null,
                ["TXT_OUTRAS_EMP"] = ctx.TextoOutrasEmpresas ?? null,
                ["MODALIDADE"] = ctx.Modalidade ?? null,
                ["QTDE_FIOS"] = ctx.QtdeFios ?? null
            };
        }

        private Dictionary<string, string> MontarWhereCircuito(CircuitoContexto ctx)
        {
            return new Dictionary<string, string>
            {
                ["LOCALIDADE"] = ctx.Localidade,
                ["CIRCUITO"] = ctx.Circuito,
                ["LOCALIDADE_NUMERICA"] = ctx.LocNum
            };
        }

    }
}
