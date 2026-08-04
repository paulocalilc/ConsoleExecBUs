using ConsoleExecBUs.Domain;
using ConsoleExecBUs.Interfaces;
using System.Data.SqlClient;

namespace ConsoleExecBUs.Application
{
    public class SipExecutionService : ISipExecutionService
    {

        private readonly IDatabaseService _db;
        private readonly IExtraConnectionService _conn;
        private readonly SipProcessor _processor;
        private readonly ILogger _logger;

        public SipExecutionService(
            IDatabaseService db,
            SipProcessor processor,
            ILogger logger,
            IExtraConnectionService conn)
        {
            _db = db;
            _processor = processor;
            _logger = logger;
            _conn = conn;
        }

        private SipContexto MontarContexto(
            SqlDataReader dr)
        {
            return new SipContexto
            {
                Localidade = dr["LOCALIDADE"]?.ToString(),
                Circuito = dr["CIRCUITO"]?.ToString(),
                Ambiente = ConverterAmbiente(dr["STC"]?.ToString())
            };
        }

        private AmbienteStc ConverterAmbiente(string ambiente)
        {
            return ambiente.ToUpper() switch
            {
                "BA" => AmbienteStc.BA,
                "MG" => AmbienteStc.MG,
                "NE" => AmbienteStc.NE,
                "RJ" => AmbienteStc.RJ,
                "R2" => AmbienteStc.R2,
                _ => throw new ArgumentException(
                    $"Ambiente inválido: {ambiente}")
            };
        }

        private List<SipContexto> CarregarPendentes()
        {
            var registros = new List<SipContexto>();

            var sql =
                @"SELECT DISTINCT SIP.LOCALIDADE,
                    CIRCUITO,
                    CASE
		                WHEN LC.UF = 'BA' THEN 'BA'
		                WHEN LC.UF = 'SP' THEN 'BA'
		                WHEN LC.UF = 'SE' THEN 'BA'
		                WHEN LC.UF = 'ES' THEN 'MG'
		                WHEN LC.UF = 'MG' THEN 'MG'
		                WHEN LC.UF = 'AL' THEN 'BA'
		                WHEN LC.UF = 'AM' THEN 'NE'
		                WHEN LC.UF = 'AP' THEN 'NE'
		                WHEN LC.UF = 'CE' THEN 'NE'
		                WHEN LC.UF = 'MA' THEN 'NE'
		                WHEN LC.UF = 'PA' THEN 'NE'
		                WHEN LC.UF = 'PB' THEN 'NE'
		                WHEN LC.UF = 'PE' THEN 'NE'
		                WHEN LC.UF = 'PI' THEN 'NE'
		                WHEN LC.UF = 'RN' THEN 'NE'
		                WHEN LC.UF = 'RJ' THEN 'RJ'
		                WHEN LC.UF = 'RR' THEN 'NE'
		                ELSE 'R2'
                    END STC
                FROM (
	                SELECT DISTINCT LOCALIDADE,UF,CIRCUITO,
		                CASE WHEN ISNULL(LOCALIDADE_SAC,LOCALIDADE) = 'TSA' AND ISNULL(UF,'') = '' THEN 'NOK' ELSE 'OK' END AS FLG_RODAR
	                FROM TBL_PERM_INSTALACAO_SIP
	                WHERE ISNULL(NUM_CHAVE,'')='') SIP
                LEFT JOIN (
	                SELECT DISTINCT LITERAL, UF
	                FROM TBL_AUX_LOCALIDADES_X_CODIBGE) LC
                ON LC.LITERAL = SIP.LOCALIDADE
                WHERE FLG_RODAR = 'OK'
                ORDER BY 3";

            using var dr = _db.ExecuteQuery(sql, "DB_PLANEMP");

            while (dr.Read())
            {
                registros.Add(MontarContexto(dr));
            }

            return registros;
        }

        public bool ExecutarSip()
        {
            try
            {
                _logger.CaixaTexto("Carregando registros SIP.");

                var registros = CarregarPendentes();

                _logger.CaixaTexto($"Encontrados {registros.Count} registros.");

                if (registros.Count > 0)
                {
                    _conn.ValidaExtra();
                }
                else
                {
                    _logger.CaixaTexto($"Encerrando execução por falta de registros.");
                    return false;
                }

                _conn.FechaExtra();

                _processor.Processar(registros);

                PersistirResultado(registros);

                return true;
            }
            catch (Exception ex)
            {
                _logger.GerarLog("ERRO", "Erro ao executar SIP.", nameof(SipExecutionService), ex.Message);

                return false;
            }
        }

        private void PersistirResultado(
            List<SipContexto> registros)
        {
            foreach (var ctx in registros)
            {

                if (string.IsNullOrWhiteSpace(ctx.NumeroChave)
                        && string.IsNullOrWhiteSpace(ctx.NumeroOs)
                        && string.IsNullOrWhiteSpace(ctx.ReferenciaPef))
                {
                    continue;
                }

                var campos = MontarCamposUpdate(ctx);

                var where = MontarWhere(ctx);

                var sql = _db.GerarUpdate(
                    "DB_PLANEMP.dbo.TBL_PERM_INSTALACAO_SIP",
                    campos,
                    where);

                _db.ExecuteQuery(sql, "DB_PLANEMP");
            }
        }

        private Dictionary<string, string> MontarCamposUpdate(SipContexto ctx)
        {
            return new Dictionary<string, string>
            {
                ["NUM_CHAVE"] = Limpar(ctx.NumeroChave),
                ["REFERENCIA_PEF"] = Limpar(ctx.ReferenciaPef),
                ["DESCRICAO_PEF"] = Limpar(ctx.DescricaoPef),
                ["NRO_FIOS"] = Limpar(ctx.NumeroFios),
                ["MICRO_SERVICO"] = Limpar(ctx.MicroServico),
                ["NUM_OS_STC"] = Limpar(ctx.NumeroOs),
                ["STATUS_OS"] = Limpar(ctx.StatusOs),
                ["PRODUTO"] = Limpar(ctx.Produto)
            };
        }

        private static string Limpar(string? valor)
        {
            return (valor ?? "").Replace("'", "").Replace(";", "");
        }

    private Dictionary<string, string> MontarWhere(SipContexto ctx)
        {
            return new Dictionary<string, string>
            {
                ["LOCALIDADE"] = ctx.Localidade,
                ["CIRCUITO"] = ctx.Circuito
            };
        }
    }
}
