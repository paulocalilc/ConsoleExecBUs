using ConsoleExecBUs.Core;
using ConsoleExecBUs.Interfaces;

namespace ConsoleExecBUs.Infrastructure.Logging
{
    internal class ClasseLog : ILogger
    {
        private readonly ClasseInicializacao _ini;

        public ClasseLog(ClasseInicializacao ini)
        {
            _ini = ini;
        }

        public bool GerarLog(string tipo, string mensagem, string classe, string detalhe)
        {
            StreamWriter sw = StreamWriter.Null;

            try
            {
                //Associa arquivo texto
                sw = File.AppendText(Path.Combine(_ini.DirLog, "LogExecucaoBUs.txt"));
                sw.WriteLine(string.Concat(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ": Tipo: ", tipo, " | Class: ", classe, " | Mensagem: ", mensagem, " | Detalhe: ", detalhe));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Concat("Ocorreu um erro ao tentar gerar LOG ", ex.Message));
                return false;
            }
            finally
            {
                sw?.Close();
            }
        }

        public void CaixaTexto(string menssagem)
        {
            // Calculate starting position for the message based on the width and height
            int largura = 87;

            if (menssagem == "inicio")
            {
                Console.WriteLine("X-------------------------------------------------------------------------------------X");
            }
            else
            {
                Console.Write('|');
                Console.Write(menssagem);
                Console.Write(new string(' ', largura - 2 - menssagem.Length));
                Console.WriteLine('|');
            }
        }
    }
}