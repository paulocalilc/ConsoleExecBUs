using System.Data.SqlClient;
using System.Globalization;
using ConsoleExecBUs.Core;
using ConsoleExecBUs.Interfaces;
using ConsoleExecBUs.Domain;

namespace ConsoleExecBUs.EntryPoint
{
    public class Principal
    {
        private readonly IExtraConnectionService _conn;
        private readonly IStcExecutionService _exec;
        private readonly ILogger _logger;
        private readonly IDatabaseService _db;
        private readonly ClasseInicializacao _ini;
        private readonly ISipExecutionService _sipExec;
        private readonly IEmailService _email;

        public Principal(
            IExtraConnectionService conn,
            IStcExecutionService exec,
            ILogger logger,
            IDatabaseService db,
            ClasseInicializacao ini,
            ISipExecutionService sipExec,
            IEmailService email)
        {
            _conn = conn;
            _exec = exec;
            _logger = logger;
            _db = db;
            _ini = ini;
            _sipExec = sipExec;
            _email = email;
        }

        public void Inicio()
        {
            _logger.CaixaTexto("inicio");
            _logger.CaixaTexto("Script desenvolvido por Paulo Calil");
            _logger.CaixaTexto("Área: Inteligência do Negócio B2B");
            _logger.CaixaTexto("Objetivo: Execução automatizada das BUs");
            _logger.CaixaTexto("Data de criação: Junho/2026");
            _logger.CaixaTexto("inicio");
            _logger.CaixaTexto("");

            if (!CarregarAplicacao())
            {   
            }
            else
            {
                try
                {
                    // 🔹 ETAPA 1
                    var listaBov = BuscarBovs();

                    // 🔹 ETAPA 2
                    ProcessarBUs(listaBov);

                    // 🔹 ETAPA 3
                    ExecutarFluxoMarcacoes();
                }
                catch (Exception ex)
                {
                    _logger.CaixaTexto("Erro ao executar fluxo principal.");
                    _logger.GerarLog("ERRO", "Erro geral", "Principal", ex.Message);
                }
            }

            _logger.GerarLog("INFORMAÇÃO", "Fim de execução", "Principal", "");
            Thread.Sleep(10000);
            Environment.Exit(0);
        }

        private void ExecutarMarcacoes()
        {
            _logger.CaixaTexto("Marcando que as procedures estão sendo executadas.");
            ExecutarProcedure(
                "UPDATE CONTROLE.TABELAS_LOCK SET INICIO_PROCEDURE = GETDATE() WHERE TABELA = 'MARCACOE_BUS'",
                "Procedures marcadas iniciadas.",
                "Erro ao fazer o update de início das 'MARCACOES_BUS'.");

            var procedures = new List<(
                string descricao,
                string query,
                string sucesso,
                string erro)>
            {
                ("SP_PERM_MARCA_LOC_SAC_BU_INS",
                    $"EXEC SP_PERM_MARCA_LOC_SAC_BU_INS '{_ini.AnoMes}'",
                    "Marcação Loc SAC na Instalação executada",
                    "Erro na execução da Loc SAC na Instalação"),

                ("SP_PERM_MARCA_LOC_SAC_BU_RET",
                    $"EXEC SP_PERM_MARCA_LOC_SAC_BU_RET '{_ini.AnoMes}'",
                    "Marcação Loc SAC na Retirada executada.",
                    "Erro na execução da Loc SAC na Retirada."),

                ("PR_ATUALIZA_WLL_BUS",
                    $"EXEC PR_ATUALIZA_WLL_BUS '{_ini.AnoMes}'",
                    "Marcação de WLL + PBX executada.",
                    "Erro na execução da procedure de WLL + PBX."),

                ("PR_ATUALIZA_TIPO_ROTA_BUS",
                    $"EXEC PR_ATUALIZA_TIPO_ROTA_BUS  '{_ini.AnoMes}', 'BASES'",
                    "Marcação de Tipo Circuito executada.",
                    "Erro ao executar a procedure de Tipo Circuito."),

                ("PR_ATUALIZA_LOB_BUS",
                    $"EXEC PR_ATUALIZA_LOB_BUS  '{_ini.AnoMes}', 'BASES'",
                    "Marcação de LOB executada.",
                    "Erro ao executar a procedure de LOB."),

                ("SP_PERM_MARCA_ISP_INS_RET",
                    $"EXEC SP_PERM_MARCA_ISP_INS_RET '{_ini.AnoMes}'",
                    "Marcação de ISP executada.",
                    "Erro ao executar a procedure de ISP."),

                ("SP_PERM_MARCA_DATA_INS_RET",
                    $"EXEC SP_PERM_MARCA_DATA_INS_RET '{_ini.AnoMes}'",
                    "Marcação do DATA executada.",
                    "Erro ao executar a procedure do DATA."),

                ("SP_PERM_MARCA_CIRC_DUPLICADO_INS_RET",
                    $"EXEC SP_PERM_MARCA_CIRC_DUPLICADO_INS_RET '{_ini.AnoMes}'",
                    "Marcação de Circuito Duplicado executada.",
                    "Erro ao executar a procedure de Circuito Duplicado."),

                ("SP_PERM_MARCA_CHAVE_UNICA_BU_INS_RET",
                    $"EXEC SP_PERM_MARCA_CHAVE_UNICA_BU_INS_RET '{_ini.AnoMes}'",
                    "Marcação de Chave Única executada.",
                    "Erro ao executar a procedure de Chave Única."),

                ("SP_PERM_MARCA_RET_INS",
                    $"EXEC SP_PERM_MARCA_RET_INS '{_ini.AnoMes}'",
                    "Marcação de Ret-Ins executada.",
                    "Erro ao executar a procedure de Ret-Ins."),

                ("SP_PERM_MARCA_FAT_LIQ_CONSIDERADO",
                    $"EXEC SP_PERM_MARCA_FAT_LIQ_CONSIDERADO '{_ini.AnoMes}'",
                    "Marcação de Faturamento executada.",
                    "Erro ao executar a procedure de Faturamento."),

                ("PR_ATLZ_TBL_PERM_BU_PLANTA_PREVIA",
                    $"EXEC PR_ATLZ_TBL_PERM_BU_PLANTA_PREVIA '{_ini.AnoMes}'",
                    "Marcação de Planta Prévia executada.",
                    "Erro ao executar a procedure de Planta Prévia."),

                ("SP_PERM_MARCA_GESTAO360",
                    "EXEC SP_PERM_MARCA_GESTAO360",
                    "Marcação de Gestão 360 executada.",
                    "Erro ao executar a procedure de Gestão 360."),

                ("SP_PERM_MARCA_LOC_EST_VELOX",
                    $"EXEC SP_PERM_MARCA_LOC_EST_VELOX '{_ini.AnoMes}'",
                    "Marcação de Loc Est de Velox executada.",
                    "Erro ao executar a procedure de Loc Est de Velox."),

                ("SP_PERM_MARCA_LOC_EST_FIXA",
                    $"EXEC SP_PERM_MARCA_LOC_EST_FIXA '{_ini.AnoMes}'",
                    "Marcação de Loc Est de Fixa executada.",
                    "Erro ao executar a procedure de Loc Est de Fixa."),

                ("SP_PERM_MARCA_LOC_EST_DADOS",
                    $"EXEC SP_PERM_MARCA_LOC_EST_DADOS '{_ini.AnoMes}'",
                    "Marcação de Loc Est de Dados executada.",
                    "Erro ao executar a procedure de Loc Est de Dados."),

                ("PR_ATLZ_IBGE_PERIMETRO_BUS",
                    $"EXEC PR_ATLZ_IBGE_PERIMETRO_BUS '{_ini.AnoMes}'",
                    "Marcação de Perímetro das BUs executada.",
                    "Erro ao executar a procedure de Perímetro das BUs."),

                ("PR_ATLZ_IBGE_PERIMETRO_BU_PLANTA",
                    $"EXEC PR_ATLZ_IBGE_PERIMETRO_BU_PLANTA '{_ini.AnoMes}'",
                    "Marcação de Perímetro da Planta executada.",
                    "Erro ao executar a procedure de Perímetro da Planta."),
            };

            foreach (var proc in procedures)
            {
                _logger.CaixaTexto($"Executando a procedure '{proc.descricao}'.");

                ExecutarProcedure(
                    proc.query,
                    proc.sucesso,
                    proc.erro
                );
            }

            _logger.CaixaTexto("Marcando que as procedures já foram executadas.");
            ExecutarProcedure(
                "UPDATE CONTROLE.TABELAS_LOCK SET FIM_PROCEDURE = GETDATE(), DISPONIVEL = 1 WHERE TABELA = 'MARCACOE_BUS'",
                "Procedures marcadas como executadas.",
                "Erro ao fazer o update de fim das 'MARCACOES_BUS'.");
        }

        private bool CarregarAplicacao()
        {
            try
            {
                ExibirValores();

                //Grava LOG
                _logger.GerarLog("INFORMAÇÃO", 
                    $"Quantidade de parametros utilizados:{Environment.GetCommandLineArgs().Length}",
                    "",
                    "");
                return true;
            }
            catch (Exception ex)
            {
                _logger.CaixaTexto(ex.Message);
                return false;
            }
        }

        public bool ExibirValores()
        {
            _logger.CaixaTexto("inicio");
            _logger.CaixaTexto($"Servidor de Banco Dados: {_ini.ServidorBancoDados}");
            _logger.CaixaTexto($"Nome do Banco de Dados 1: {_ini.NomeTera}");
            _logger.CaixaTexto($"Nome do Banco de Dados 2: {_ini.NomeBdc}");
            _logger.CaixaTexto($"Schema do Banco de Dados: {_ini.NomeSchema}");
            _logger.CaixaTexto($"Diretório do Log: {_ini.DirLog}");
            _logger.CaixaTexto($"Data Formatada: {_ini.DataFormatada}");
            _logger.CaixaTexto($"Ano Mês: {_ini.AnoMes}");
            _logger.CaixaTexto($"Fechamento do Mês: {_ini.FechamentoMes}");
            _logger.CaixaTexto($"Usuário de Rede: {_ini.UsuarioRede}");
            _logger.CaixaTexto("inicio");

            return true;
        }

        private bool VerficaCargas(string tabela, string procExec, string indic)
        {
            try
            {
                SqlDataReader drControle;
                SqlDataReader drResultado;

                string verificacao = string.Concat(
                    "SELECT CASE WHEN DATEDIFF(HH,FIM_PROCEDURE,GETDATE()) < 15 THEN '1' ELSE '0' END AS EXECUTADO", Environment.NewLine,
                    "FROM CONTROLE.TABELAS_LOCK", Environment.NewLine,
                    "WHERE TABELA IN ('", tabela, "')");

                drControle = _db.ExecuteQuery(verificacao, "DB_PLANEMP");

                while (drControle.Read())
                {
                    string controle = drControle.GetString(0);
                    bool modoFechamento = _ini.FechamentoMes == 1;

                    if (controle == "0" || modoFechamento)
                    {
                        _logger.CaixaTexto(string.Concat("A BU ", indic, " não foi executada hoje."));
                        _logger.GerarLog("LOG", string.Concat("A BU ", indic, " não foi executada hoje"), "Principal", "");

                        try
                        {
                            _logger.CaixaTexto(string.Concat("Executando a BU ", indic, "."));
                            drResultado = _db.ExecuteQuery(string.Concat("EXEC ", procExec, " '", _ini.AnoMes, "', 0"), "DB_PLANEMP");
                            _logger.CaixaTexto(string.Concat("BU ", indic, " executada."));
                            _logger.CaixaTexto("inicio");
                        }
                        catch (Exception ex)
                        {
                            //Grava LOG
                            _logger.CaixaTexto(string.Concat("Erro ao executar a BU ", indic, "."));
                            _logger.CaixaTexto("inicio");
                            _logger.GerarLog("ERRO", string.Concat("Ocorreu erro ao executar a BU ", indic, "."), "Principal", ex.Message);
                        }

                        return true;
                    }
                    else
                    {
                        _logger.CaixaTexto(string.Concat("A BU ", indic, " já foi executada hoje."));
                        _logger.CaixaTexto("inicio");
                        _logger.GerarLog("LOG", string.Concat("A BU ", indic, " já foi executada hoje"), "Principal", "");
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                //Grava LOG
                _logger.CaixaTexto(string.Concat("Erro ao verificar a BU ", indic, "."));
                _logger.GerarLog("ERRO", string.Concat("Ocorreu erro ao verificar a BU ", indic, "."), "Principal", ex.Message);
                return false;
            }
        }

        private void ProcessarRegistroBU(string indicador, string statusBOV)
        {
            Thread.Sleep(100);

            if (indicador == "INSTALAÇÃO" && statusBOV == "1")
            {
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("As BOVs de Instalação foram carregadas.");
                _logger.CaixaTexto("Verificando se a BU Instalação foi executada.");

                string tabela = "[dbo].[sp_perm_bu_instalacao]";
                string proc = "[dbo].[sp_perm_bu_instalacao]";
                if (VerficaCargas(tabela, proc, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(indicador.ToLower())))
                {
                    _exec.ExecutarVarreduraTelaCA();
                }

                _logger.CaixaTexto("");
            }
            else if (indicador == "INSTALAÇÃO" && statusBOV != "1")
            {
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("As BOVs de Instalação não foram carregadas.");
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("");
            }

            if (indicador == "RETIRADA" && statusBOV == "1")
            {
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("As BOVs de Retirada foram carregadas.");
                _logger.CaixaTexto("Verificando se a BU Retirada foi executada.");

                string tabela = "[dbo].[sp_perm_bu_retirada]";
                string proc = "[dbo].[sp_perm_bu_retirada]";

                if (VerficaCargas(tabela, proc, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(indicador.ToLower()))) { }

                _logger.CaixaTexto("");
            }
            else if (indicador == "RETIRADA" && statusBOV != "1")
            {
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("As BOVs de Retirada não foram carregadas.");
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("");
            }

            if (indicador == "MIGRAÇÃO" && statusBOV == "1")
            {
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("As BOVs de Migração foram carregadas.");
                _logger.CaixaTexto("Verificando se a BU Migração foi executada.");

                string tabela = "[dbo].[sp_perm_bu_migracao]";
                string proc = "[dbo].[sp_perm_bu_migracao]";

                if (VerficaCargas(tabela, proc, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(indicador.ToLower()))) { }
                _logger.CaixaTexto("");
            }
            else if (indicador == "MIGRAÇÃO" && statusBOV != "1")
            {
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("As BOVs de Migração não foram carregadas.");
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("");
            }

            if (indicador == "CANCELAMENTO" && statusBOV == "1")
            {
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("As BOVs de Cancelamento foram carregadas.");
                _logger.CaixaTexto("Verificando se a BU Cancelamento foi executada.");

                string tabela = "[dbo].[sp_perm_bu_cancelamento]";
                string proc = "[dbo].[sp_perm_bu_cancelamento]";

                if (VerficaCargas(tabela, proc, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(indicador.ToLower()))) { }
                _logger.CaixaTexto("");
            }
            else if (indicador == "CANCELAMENTO" && statusBOV != "1")
            {
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("As BOVs de Migração não foram carregadas.");
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("");
            }

            if (indicador == "VENDA LÍQUIDA" && statusBOV == "1")
            {
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("As BOVs de Venda Líquida foram carregadas.");
                _logger.CaixaTexto("Verificando se a BU Venda Líquida foi executada.");

                string tabela = "[dbo].[sp_perm_bu_venda_liquida]";
                string proc = "[dbo].[sp_perm_bu_venda_liquida]";

                if (VerficaCargas(tabela, proc, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(indicador.ToLower()))) { }
                _logger.CaixaTexto("");
            }
            else if (indicador == "VENDA LÍQUIDA" && statusBOV != "1")
            {
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("As BOVs de Venda Líquida não foram carregadas.");
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("");
            }

            if (indicador == "BACKLOG" && statusBOV == "1")
            {
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("As BOVs de Backlog foram carregadas.");
                _logger.CaixaTexto("Verificando se a BU Backlog foi executada.");

                string tabela = "[dbo].[sp_perm_bu_backlog]";
                string proc = "[dbo].[sp_perm_bu_backlog]";

                if (VerficaCargas(tabela, proc, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(indicador.ToLower()))) { }

                _logger.CaixaTexto("Iniciando processamento SIP.");

                _sipExec.ExecutarSip();

                _logger.CaixaTexto("");
            }
            else if (indicador == "BACKLOG" && statusBOV != "1")
            {
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("As BOVs de Backlog não foram carregadas.");
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("");
            }

            if (indicador == "PROD DADOS" && statusBOV == "1")
            {
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("O produção Dados foi carregadas.");
                _logger.CaixaTexto("Verificando se a BU produção Dados foi executada.");

                string tabela = "[dbo].[sp_perm_bu_producao_dados]";
                string proc = "[dbo].[sp_perm_bu_producao_dados]";

                if (VerficaCargas(tabela, proc, CultureInfo.CurrentCulture.TextInfo.ToTitleCase(indicador.ToLower()))) { }
                _logger.CaixaTexto("");
            }
            else if (indicador == "PROD DADOS" && statusBOV != "1")
            {
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("O Produção Dados não foi carregado.");
                _logger.CaixaTexto("inicio");
                _logger.CaixaTexto("");
            }
        }

        private bool ValidarMarcacoes()
        {
            string qry = string.Concat(
                "SELECT TABELA,CASE WHEN DATEDIFF(HH,FIM_PROCEDURE,GETDATE()) <= 15 THEN '1' ELSE '0' END AS BUS ",
                "FROM CONTROLE.TABELAS_LOCK ",
                "WHERE TABELA IN (",
                "'[DBO].[SP_PERM_BU_RETIRADA]', ",
                "'[DBO].[SP_PERM_BU_INSTALACAO]', ",
                "'[DBO].[SP_PERM_BU_VENDA_LIQUIDA]', ",
                "'[DBO].[SP_PERM_BU_MIGRACAO]', ",
                "'[DBO].[SP_PERM_BU_CANCELAMENTO]', ",
                "'[DBO].[SP_PERM_BU_PRODUCAO_DADOS]', ",
                "'[DBO].[SP_PERM_BU_BACKLOG]')"
            );

            _logger.CaixaTexto("inicio");
            _logger.CaixaTexto("Verificando se as Marcações das BUs já rodaram.");

            using var drMarcacoes = _db.ExecuteQuery(qry, "DB_PLANEMP");

            if (drMarcacoes.HasRows)
            {
                string stTabela = string.Empty;
                string stBUs = string.Empty;

                while (drMarcacoes.Read())
                {
                    stTabela = drMarcacoes.GetString(0);
                    stBUs = drMarcacoes.GetString(1);

                    if (stBUs == "0")
                    {
                        _logger.CaixaTexto($"A tabela {stTabela} ainda não foi executada.");
                        _logger.CaixaTexto("inicio");
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        private string ObterControleMarcacoes()
        {
            string qryDash = string.Concat(
                "SELECT MIN(CASE WHEN DATEDIFF(HH, FIM_PROCEDURE, GETDATE()) <= 15 THEN '1' ELSE '0' END) AS EXECUTADO ",
                "FROM CONTROLE.TABELAS_LOCK ",
                "WHERE TABELA IN('MARCACOE_BUS','[DBO].[SP_PERM_BU_DASHBOARD]')"
            );

            using var dr = _db.ExecuteQuery(qryDash, "DB_PLANEMP");

            string controle = "0";

            if (dr != null)
            {
                while (dr.Read())
                {
                    controle = dr.GetString(0);
                }
            }

            return controle;
        }

        private void ExecutarProcedure(
            string query,
            string msgSucesso,
            string msgErro)
        {
            try
            {
                _db.ExecuteQuery(query, "DB_PLANEMP");
                _logger.CaixaTexto(msgSucesso);
                _logger.CaixaTexto("");
            }
            catch (Exception ex)
            {
                _logger.CaixaTexto(msgErro);
                _logger.CaixaTexto("");
                _logger.GerarLog("ERRO", msgErro, "Principal", ex.Message);
            }
        }

        private List<BovStatus> BuscarBovs()
        {
            var data = _ini.DataFormatada;

            string qry = $@"
                SELECT 
                    CASE 
                        WHEN NUM_BOV IN ('6769','6802','6803') THEN 'INSTALAÇÃO'
                        WHEN NUM_BOV IN ('6771','6804','6823') THEN 'RETIRADA'
                        WHEN NUM_BOV IN ('6893','6918','6919') THEN 'CANCELAMENTO'
                        WHEN NUM_BOV IN ('6897','6901') THEN 'MIGRAÇÃO'
                        WHEN NUM_BOV IN ('6876','6909') THEN 'BACKLOG'
                        WHEN NUM_BOV IN ('6905','6908') THEN 'VENDA LÍQUIDA'
                        ELSE 'VERIFICAR NUM BOV'
                    END AS INDICADOR_BOV,
                    CASE 
                        WHEN STATUS_UPLOAD_BOV IS NULL THEN '0' 
                        ELSE STATUS_UPLOAD_BOV 
                    END AS STATUS_UPLOAD_BOV
                FROM TBL_PERM_AUTOMACAO_BOV_ARQUIVOS_BOV
                WHERE PERIODO_ATE_BOV = '{data}'
                AND NUM_BOV IN (
                    '6769','6802','6803',
                    '6771','6804','6823',
                    '6893','6918','6919',
                    '6897','6901',
                    '6876','6909',
                    '6905','6908'
                )
                GROUP BY 
                    CASE 
                        WHEN NUM_BOV IN ('6769','6802','6803') THEN 'INSTALAÇÃO'
                        WHEN NUM_BOV IN ('6771','6804','6823') THEN 'RETIRADA'
                        WHEN NUM_BOV IN ('6893','6918','6919') THEN 'CANCELAMENTO'
                        WHEN NUM_BOV IN ('6897','6901') THEN 'MIGRAÇÃO'
                        WHEN NUM_BOV IN ('6876','6909') THEN 'BACKLOG'
                        WHEN NUM_BOV IN ('6905','6908') THEN 'VENDA LÍQUIDA'
                        ELSE 'VERIFICAR NUM BOV'
                    END,
                    CASE 
                        WHEN STATUS_UPLOAD_BOV IS NULL THEN '0' 
                        ELSE STATUS_UPLOAD_BOV 
                    END
                ";

            _logger.CaixaTexto("inicio");
            _logger.CaixaTexto("Pesquisando status das BOVs no BD.");
            _logger.CaixaTexto("inicio");
            _logger.CaixaTexto("");

            var lista = new List<BovStatus>();

            using (var dr = _db.ExecuteQuery(qry, "DB_PLANEMP"))
            {
                while (dr.Read())
                {
                    lista.Add(new BovStatus
                    {
                        Indicador = dr.GetString(0),
                        Status = dr.GetString(1)
                    });
                }
            }

            return lista;
        }

        private void ProcessarBUs(List<BovStatus> lista)
        {
            if (!lista.Any())
                return;

            foreach (var bov in lista)
            {
                ProcessarRegistroBU(bov.Indicador, bov.Status);
            }
        }

        private void ExecutarFluxoMarcacoes()
        {
            try
            {
                bool modoFechamento = _ini.FechamentoMes == 1;

                if (!modoFechamento && !ValidarMarcacoes())
                    return;

                var controle = ObterControleMarcacoes();

                Thread.Sleep(100);

                if (controle == "0" || modoFechamento)
                {
                    _logger.CaixaTexto("As Marcações ainda não rodaram.");
                    _logger.CaixaTexto("");

                    ExecutarMarcacoes();

                    _logger.CaixaTexto("As marcações das BUs foram executadas.");
                    _logger.CaixaTexto("inicio");
                    _logger.CaixaTexto("");

                    _logger.CaixaTexto("inicio");
                    _logger.CaixaTexto("Iniciando a execução da BU Dashboard");

                    ExecutarDashboard();

                    var buStatus = GeraTotalizador();
                    var destinatarios = _email.ObterDestinatarios();
                    var assunto = $"Relatório BOV {_ini.DataFormatada}";
                    var corpo = _email.GerarCorpoRelatorio(buStatus);                    

                    _email.Enviar(assunto,corpo,destinatarios);
                }
                else
                {
                    _logger.CaixaTexto("As Marcações das BUs já rodaram.");
                    _logger.CaixaTexto("inicio");
                }
            }
            catch (Exception ex)
            {
                _logger.CaixaTexto("Erro ao verificar as marcações das BUs.");
                _logger.CaixaTexto("inicio");
                _logger.GerarLog("ERRO", "Ocorreu erro ao verificar as BUs", "Principal", ex.Message);
            }
        }

        private void ExecutarDashboard()
        {
            string qry = $"EXEC SP_PERM_BU_DASHBOARD '{_ini.AnoMes}', 0";

            try
            {
                _db.ExecuteQuery(qry, "DB_PLANEMP");
                _logger.CaixaTexto("A BU Dash foi executada.");
                _logger.CaixaTexto("inicio");
            }
            catch (Exception ex)
            {
                _logger.CaixaTexto("Erro ao executar a BU Dash.");
                _logger.CaixaTexto("inicio");
                _logger.GerarLog("ERRO", "Erro ao executar a BU Dash.", "Principal", ex.Message);
            }
        }

        private List<BuStatus> GeraTotalizador()
        {
            string qry = @$"SELECT TABELA, INDICADOR, SUM(QUANTIDADE) AS QTD_REGS
    FROM TBL_PERM_CHECKS_FISICOS
    WHERE UN = 'GRANDES EMPRESAS'
    GROUP BY TABELA, INDICADOR
    ORDER BY TABELA, INDICADOR";

            var lista = new List<BuStatus>();

            using (var dr = _db.ExecuteQuery(qry, "DB_PLANEMP"))
            {
                while (dr.Read())
                {
                    lista.Add(new BuStatus
                    {
                        NomeBU = dr.GetString(0),
                        Indicador = dr.GetString(1),
                        QtdeRegs = Convert.ToInt64(dr["QTD_REGS"])
                    });
                }
            }
            return lista;
        }
    }
}