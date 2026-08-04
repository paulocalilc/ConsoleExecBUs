using System.Data;
using System.Data.SqlClient;
using ConsoleExecBUs.Core;
using ConsoleExecBUs.Interfaces;

namespace ConsoleExecBUs.Infrastructure.Database
{
    public class ClasseSqlServer : IDatabaseService
    {

        //Definição das variaveis de conexão
        private string? connectionString;
        SqlConnection? connection;
        SqlCommand? command;

        private readonly ClasseInicializacao _ini;
        private readonly ILogger _logger;

        public ClasseSqlServer(
            ClasseInicializacao ini, 
            ILogger logger)
        {
            _ini = ini;
            _logger = logger;
        }



        //Abrir banco de dados"
        //Responsável pela abertura da conexão
        private bool OpenConn(string bd)
        {
            //String de conexão (Pode ser mutavel de acordo com o banco de dados)
            if (bd == "DB_PLANEMP")
            {
                connectionString = $"Server={_ini.ServidorBancoDados}; " +
                    $"Database={_ini.NomeTera}; " +
                    $"Trusted_Connection=True; " +
                    $"Min Pool Size=5; " +
                    $"Max Pool Size=300; " +
                    $"Connect Timeout=5000";
            }
            else if (bd == "BDCMEMP")
            {
                connectionString = $"Server={_ini.ServidorBancoDados};" +
                    $"Database={_ini.NomeBdc};" +
                    $"Trusted_Connection=True; " +
                    $"Min Pool Size=5; " +
                    $"Max Pool Size=300; " +
                    $"Connect Timeout=5000";
            }
            //Atribui string de conexão a variavel de conexão
            connection = new SqlConnection(connectionString);

            try
            {
                //Tenta realizar conexão
                connection.Open();

                //Grava LOG
                _logger.GerarLog("INFORMAÇÃO", "Abertura de conexão", "", "");

                return true;
            }
            catch (SqlException ex)
            {
                //Gera Log
                _logger.GerarLog("ERRO", "Houve uma falha na conexão ao banco de dados", "ClassSqlServer", ex.Message);

                //Retorna mensagem de erro
                throw new ArgumentException(string.Concat("Houve uma falha na conexão ao banco de dados", Environment.NewLine, "Erro: ", ex.Message));
            }
        }

        //"Fechar banco de dados"
        //Responsável por encerrar a conexão com o banco de dados
        private bool CloseConn()
        {
            //Verifica se existe alguma conexão
            if (connection is null is false)
            {
                //Caso exista uma instancia de conexão e esteja aberta, realiza o encerramento
                if (connection.State == ConnectionState.Open)
                {
                    //Fecha conexão
                    connection.Close();
                    return true;
                }
                else
                {
                    //Não existe conexão aberta
                    return false;
                }
            }
            else
            {
                //Não existe instancia de conexão
                return false;
            }
        }

        //"Executar Query"
        //Responsável por executar querys diretamente do código
        public SqlDataReader ExecuteQuery(string query, string bd)
        {
            //Executa querys passadas por parametro ao método
            try
            {
                //Grava LOG
                _logger.GerarLog("INFORMAÇÃO", string.Concat("Executado query ", query), "", "");

                //Abre conexão
                OpenConn(bd);

                //Instancia execução de novo código SQL
                command = new SqlCommand();

                //Aumento de tempo do timeout das consultas. Modificado o valor default de 30 segundos devido a tempo de execução do bulkinsert
                command.CommandTimeout = 30000;
                command.CommandType = CommandType.Text;
                command.CommandText = query;
                command.Connection = connection;

                //Atribui valor da consulta
                return command.ExecuteReader();

                //Verifica se ocorreu excessão por parte do código SQL
            }
            catch (SqlException ex)
            {
                //Grava LOG
                _logger.GerarLog("ERRO", "Falha ao executar consulta SQL", "ClassSqlServer", ex.Message);
                throw new ArgumentException(string.Concat("Falha ao executar consulta SQL", Environment.NewLine, "SqlException: ", ex.Message));

            }   //Verifica se ocorreu algum outro erro   
            catch (Exception ex)
            {

                //Gera Log
                _logger.GerarLog("ERRO", "Falha ao realizar operação", "ClassSqlServer", ex.Message);
                throw new ArgumentException(string.Concat("Falha ao realizar operação", Environment.NewLine, "Exception: ", ex.Message));
            }
        }

        public string GerarUpdate(
            string tabela,
            Dictionary<string, string> campos,
            Dictionary<string, string> where)
        {
            // monta SET
            var set = string.Join(",\n", campos.Select(c => $"{c.Key} = '{c.Value}'"));

            // monta WHERE
            var cond = string.Join(" AND ", where.Select(w => $"{w.Key} = '{w.Value}'"));

            return $@"
            UPDATE {tabela}
            SET {set}
            WHERE {cond}";
        }

        public string GerarInsert(
            string tabela,
            Dictionary<string, string> campos)
        {
            var colunas = string.Join(", ", campos.Keys);

            var valores = string.Join(", ",
                campos.Values.Select(v => $"'{v}'"));

            return $@"
                INSERT INTO {tabela}
                ({colunas})
                VALUES
                ({valores})";
        }

        public string GerarSelect(
            string tabela,
            List<string> colunas,
            Dictionary<string, string?>? where = null)
        {
            var select = string.Join(", ", colunas);

            var sql = $"SELECT {select} FROM {tabela}";

            if (where != null && where.Count > 0)
            {
                var cond = string.Join(" AND ",
                    where.Select(w =>
                        w.Value == null
                            ? $"{w.Key} IS NULL"
                            : $"{w.Key} = '{w.Value}'"));

                sql += $" WHERE {cond}";
            }

            return sql;
        }

    }
}