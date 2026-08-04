using System.Data.SqlClient;

public interface IDatabaseService
{
    SqlDataReader ExecuteQuery(string query, string db);
    string GerarInsert(string tabela, Dictionary<string, string> campos);
    string GerarSelect(string tabela, List<string> colunas, Dictionary<string, string?>? where = null);
    string GerarUpdate(string tabela, Dictionary<string, string> campos, Dictionary<string, string> where);
}