using ConsoleExecBUs.EntryPoint;

namespace ConsoleExecBUs.Core
{
    public class ClasseInicializacao
    {
        //Injeção de classes
        private readonly Principal _princ;
        //private readonly ILogger _logger;

        //Constantes com os dados do banco de dados
        private const string CONSTANTE_HOST_DB = "HOST,PORTA";
        private const string CONSTANTE_NAME_DB1 = "NOME_BD_1";
        private const string CONSTANTE_NAME_DB2 = "NOME_BD2";
        private const string CONSTANTE_NAME_SCHEMA = "SCHEMA";

        //Parametros
        public string dirLog = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        //Construtor com array de parametros

        public ClasseInicializacao(string[] args)
        {
            IniciaArgumentos(args);
            ValidateInitialize();
            CarregarDataInicializacao(FechamentoMes);
        }


        private void IniciaArgumentos(string[] argument)
        {
            string argto;
            string argtoVal;

            //Verifica se possui ao menos 1 argumento valido
            if (argument.Length > 1)
            {
                //Prepara a atribuição de valores
                try
                {
                    //Estrutura de repetição para procurar os valores
                    for (var i = 0; i < argument.Length - 1; i++)
                    {
                        argto = argument[i];
                        argtoVal = argument[i + 1];

                        //Verifica se inicio do argumento é parametro
                        if (!string.IsNullOrEmpty(argto) && argto.StartsWith("-"))
                        {
                            //Verifica valor a valor
                            switch (argto)
                            {
                                //Flag para identificar se deverá ser executado o fechamento do mês
                                case "-fechames":
                                    if (!string.IsNullOrEmpty(argtoVal))
                                        FechamentoMes = int.Parse(argtoVal);
                                    else
                                        FechamentoMes = 0;
                                    break;

                                case "-usuarioExtra":
                                    UsuarioExtra = argtoVal;
                                    break;

                                case "-senhaExtra":
                                    SenhaExtra = argtoVal;
                                    break;

                                default:
                                    //Caso não encontre um argumento valido, retorna erro
                                    throw new ArgumentException(string.Concat("Argumento //", argto, "// inválido"));
                            }
                        }
                    }
                }
                //Caso ocorra alguma exceção, retorna erro
                catch (Exception ex)
                {
                    throw new ArgumentException(ex.Message);
                }
            }
            //Valida os valores
            ValidateInitialize();

            //Chama metodo de data
            CarregarDataInicializacao(FechamentoMes);
        }

        //propriedades das variáveis globais
        public int FechamentoMes { get; set; } = 0;
        public string ServidorBancoDados { get; set; } = CONSTANTE_HOST_DB;
        public string NomeTera {  get; set; } = CONSTANTE_NAME_DB1;
        public string NomeBdc { get; set; } = CONSTANTE_NAME_DB2;
        public string NomeSchema { get; set; } = CONSTANTE_NAME_SCHEMA;
        public string AnoMes { get; set; } = string.Empty;
        public string DirLog {  get; set; } = string.Empty;
        public string DataFormatada {  get; set; } = string.Empty;
        public string UsuarioRede { get; set; } = string.Empty;
        public string UsuarioExtra { get; set; } = string.Empty;
        public string SenhaExtra {  get; set; } = string.Empty;

        //Método validade para inicialização
        public bool ValidateInitialize()
        {
            string inconsistenciasPreenchimento = string.Empty;
            int contErros = 0;

            //Valida PropServidorBancoDados
            if (ServidorBancoDados.Length == 0)
            {
                ServidorBancoDados = CONSTANTE_HOST_DB;
            }   

            //Valida PropNomeBd
            if (string.IsNullOrEmpty(NomeTera))
            {
                NomeTera = CONSTANTE_NAME_DB1;
            }

            //Valida PropNomeBd
            if (string.IsNullOrEmpty(NomeBdc))
            {
                NomeBdc = CONSTANTE_NAME_DB2;
            }

            //Valida nomeSchema
            if (string.IsNullOrEmpty(NomeSchema))
            {
                NomeSchema = CONSTANTE_NAME_SCHEMA;
            }

            //Valida PropDirLog
            if (string.IsNullOrEmpty(DirLog))
            {
                DirLog = dirLog;
            }

            //Valida PropDirLog
            if (string.IsNullOrEmpty(UsuarioRede))
            {
                UsuarioRede = Environment.UserName;
            }

            //Verifica se encontrou algum erro
            if (contErros > 0)
            {
                //Gera Log
                //_logger.GerarLog("ERRO", string.Concat("Foram detectados ", contErros, " erros"), "ClassInitialize", inconsistenciasPreenchimento);
                throw new ArgumentException(string.Concat("Foram detectados ", contErros, " erros.", Environment.NewLine, Environment.NewLine, inconsistenciasPreenchimento));
            }
            else
            {
                //Retorna verdadeiro
                return true;
            }
        }

        //Metodo para definir data global
        private bool CarregarDataInicializacao(int fechamentoMes)
        {
            string dataRelatorios;
            string dia;
            string mes;
            string ano;

            //Verifica se data é fechamento
            if (fechamentoMes == 1)
            {
                //Obtem data do relatorio
                dataRelatorios = DateTime.Now.AddMonths(-FechamentoMes).ToString("dd/MM/yyyy");

                //Seta a variável anoMes para o fechamento
                string primeiroDia = string.Concat("01/", dataRelatorios.Substring(3, 7));
                string ultimoDia = DateTime.Parse(primeiroDia).AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy");

                //Realiza separação do dia, mes e ano
                dia = DateTime.Parse(ultimoDia).ToString("dd");
                mes = DateTime.Parse(ultimoDia).ToString("MM");
                ano = DateTime.Parse(ultimoDia).ToString("yyyy");

                //Incrementa as variaveis da classe
                DataFormatada = $"{ano}{mes}{dia}";
                AnoMes = $"{ano}{mes}";
            }
            else
            {
                //Obtem data do relatorio
                dataRelatorios = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");

                //Realiza separação do dia, mes e ano
                dia = DateTime.Parse(dataRelatorios).ToString("dd");
                mes = DateTime.Parse(dataRelatorios).ToString("MM");
                ano = DateTime.Parse(dataRelatorios).ToString("yyyy");

                //Incrementa as variaveis da classe
                DataFormatada = $"{ano}{mes}{dia}";
                AnoMes = $"{ano}{mes}";
            }
            return true;
        }
    }
}