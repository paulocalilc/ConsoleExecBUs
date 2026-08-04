using System.Data.SqlClient;
using ConsoleExecBUs.Core;
using ConsoleExecBUs.Interfaces;

namespace ConsoleExecBUs.Application
{
    public class StcExecutionService : IStcExecutionService
    {
        public SqlDataReader? drResultado = null;
        public SqlDataReader? drResult = null;
        public SqlDataReader? drCarga = null;
        private readonly IStcCommandService _comando;
        private readonly IStcValidationService _valida;
        private readonly IExtraConnectionService _conn;
        private readonly IStcScreenReaderService _leitor;
        private readonly IExtraNavigationService _naveg;
        private readonly ILogger _logger;
        private readonly IDatabaseService _db;
        private readonly CircuitoProcessor _processor;
        private readonly ClasseInicializacao _ini;

        public StcExecutionService(
            IStcCommandService comando,
            IStcValidationService valida,
            IExtraConnectionService conn,
            IStcScreenReaderService leitor,
            IExtraNavigationService naveg,
            ILogger logger,
            IDatabaseService db,
            CircuitoProcessor processor,
            ClasseInicializacao ini)
        {
            _comando = comando;
            _valida = valida;
            _conn = conn;
            _leitor = leitor;
            _naveg = naveg;
            _logger = logger;
            _db = db;
            _processor = processor;
            _ini = ini;
        }

        public bool ExecutarVarreduraTelaCA()
        {
            var ctx = new Domain.CircuitoContexto(); ;

            string sql = "";
            try
            {
                _logger.CaixaTexto("Iniciando a execução de macro no STC");
                _logger.CaixaTexto("Instaciando o Extra.");
                _conn.ValidaExtra();

                if (_conn.FnReconectar())
                {
                    _logger.CaixaTexto("Baixando os circuitos do BD.");

                    sql = "TRUNCATE TABLE DBO.TBL_PERM_MACRO_PLANTA ";
                    _db.ExecuteQuery(sql, "BDCMEMP");

                    sql = string.Concat("INSERT INTO BDCMEMP.DBO.TBL_PERM_MACRO_PLANTA (LOCALIDADE,CIRCUITO,LOCALIDADE_NUMERICA)", Environment.NewLine,
                            "SELECT P.LOCALIDADE, P.CIRCUITO, P.LOCALIDADE_NUMERICA FROM ( ", Environment.NewLine,
                            "   SELECT DISTINCT LOCALIDADE, CIRCUITO, LOCALIDADE_NUMERICA ", Environment.NewLine,
                            "   FROM DB_PLANEMP.DBO.TBL_PERM_BU_INSTALACAO P ", Environment.NewLine,
                            "   WHERE P.ANO_MES = ", _ini.AnoMes, Environment.NewLine,
                            "       AND P.PRODUTO IN ('VPN VIP','IP CONNECT','DATA','DIGITRONCO (CIRCUITO)','IP PROJETO ESCOLA','DIGITRONCO (CIRCUITO) < 2MBPS') ", Environment.NewLine,
                            "       AND LEN (CIRCUITO) IN (7,8)", Environment.NewLine,
                            "       AND LEFT (CIRCUITO,1) != '0')P ", Environment.NewLine,
                            "LEFT JOIN [BDCMEMP].DBO.TBL_121526_COD_LOGRAD_OFICIAL_NOVA T ", Environment.NewLine,
                            "ON P.LOCALIDADE + P.CIRCUITO = T.LOCALIDADE + T.CIRCUITO ", Environment.NewLine,
                            "WHERE T.CIRCUITO IS NULL AND P.LOCALIDADE_NUMERICA != '00000' ");

                    _db.ExecuteQuery(sql, "DB_PLANEMP");

                    sql = string.Empty;

                    sql = _db.GerarSelect(
                        "BDCMEMP.DBO.TBL_PERM_MACRO_PLANTA",
                        ObterColunasCircuito()
                    );

                    drResultado = _db.ExecuteQuery(sql, "BDCMEMP");

                    if (drResultado.HasRows)
                    {
                        while (drResultado.Read())
                        {
                            ctx.QtdRegistros += 1;
                        }
                    }

                    _logger.CaixaTexto(string.Concat("Foram baixados ", ctx.QtdRegistros, " registros."));

                    if (ctx.QtdRegistros == 0)
                        return true;

                    drResultado = _db.ExecuteQuery(sql, "BDCMEMP");

                    if (drResultado.HasRows)
                    {
                        _logger.CaixaTexto("Varrendo a tela CA e a tela 185.");
                        _processor.ProcessarRegistros(drResultado);

                        sql = _db.GerarSelect(
                            "BDCMEMP.DBO.TBL_PERM_MACRO_PLANTA",
                            ObterColunasCircuito(),
                            MontarWhereClienteNull()
                        );

                        drResultado = _db.ExecuteQuery(sql, "BDCMEMP");

                        _comando.EsperarRetorno(50, 50);

                    }
                    if (ctx.QtdRegistrosProcessados < ctx.QtdRegistros)
                    {
                        //goto InicioCA;
                    }
                }

                //EXECUTA AS PROCS NO BD
                _logger.CaixaTexto("Rodando as procs da macro no BD.");
                _db.ExecuteQuery("EXEC BDCMEmp.dbo.PR_CARGA_TBL_121526_COD_LOGRAD_OFICIAL_NOVA", "BDCMEMP");
                _db.ExecuteQuery("EXEC [DB_PLANEMP].[dbo].PR_CARGA_TBL_PERM_TIPO_ACESSO_CIRCUITO " + _ini.AnoMes, "DB_PLANEMP");
                _db.ExecuteQuery("EXEC [DB_PLANEMP].[dbo].[PR_ATLZ_TBL_PERM_DEPARA_DETALHE_OEMP]", "DB_PLANEMP");
                _logger.CaixaTexto("Carga das macros finalizada com sucesso!");
                _logger.CaixaTexto("inicio");
                _logger.GerarLog("SUCESSO", "Cargas Finalizadas com sucesso." + Environment.NewLine +
                                                "Registros baixados do BD: " + ctx.QtdRegistros + Environment.NewLine +
                                                "Registros processados pela macro:" + ctx.QtdRegistrosProcessados + Environment.NewLine +
                                                "", "ClasseExtra", "");

                _conn.FechaExtra();

                return true;
            }
            catch (Exception ex)
            {
                _logger.GerarLog("ERRO", "Erro ao executar a consulta no STC (tela CA).", "ClasseExtra", ex.Message);
                throw new ArgumentException("Erro ao executar a consulta no STC (tela CA). Erro: " + ex.Message);
            }

        }

        private void ExecutarComandoSql(string sql, string bd)
        {
            _db.ExecuteQuery(sql, bd);
        }

        private Dictionary<string, string?> MontarWhereClienteNull()
        {
            return new Dictionary<string, string?>
            {
                ["CLIENTE"] = null
            };
        }

        private List<string> ObterColunasCircuito()
        {
            return new List<string>
            {
                "LOCALIDADE",
                "CIRCUITO",
                "LOCALIDADE_NUMERICA"
            };
        }
    }
}
