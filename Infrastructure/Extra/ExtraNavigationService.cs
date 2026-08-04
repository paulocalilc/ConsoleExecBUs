using ConsoleExecBUs.Domain;
using ConsoleExecBUs.Interfaces;
using System.Runtime.InteropServices;

namespace ConsoleExecBUs.Infrastructure.Extra
{
    public class ExtraNavigationService : IExtraNavigationService
    {
        private readonly IStcCommandService _comando;
        private readonly IStcScreenReaderService _leitor;
        private readonly IStcValidationService _valida;
        private readonly ILogger _logger;


        public ExtraNavigationService(
            IStcCommandService comando,
            IStcScreenReaderService leitor,
            IStcValidationService valida,
            ILogger logger)
        {
            _comando = comando;
            _leitor = leitor;
            _valida = valida;
            _logger = logger;
        }
        public bool FnTelaInicial113CA()
        {
            try
            {
                _comando.MoverCursor(18, 29);
                _comando.EnviarComando("113");
                _comando.EnviarComando("<Enter>", 250, 150);

                if (FnFechaAvisosOtimz())
                {
                    _comando.EnviarComando("C");
                    _comando.EnviarComando("<Enter>", 250);
                    _comando.EnviarComando("A", 250);
                    _comando.EnviarComando("<Enter>", 250);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.GerarLog("ERRO", "Erro ao acessar a tela CA do STC.", "ClasseExtra", ex.Message);
                throw new ArgumentException("Erro ao acessar a tela CA do STC. Erro: " + ex.Message);
            }
        }

        public bool FnTelaInicial181()
        {
            try
            {
                _comando.MoverCursor(18, 29);
                _comando.EnviarComando("1", 250);
                _comando.EnviarComando("<Enter>", 250);
                _comando.EnviarComando("8", 250);
                _comando.EnviarComando("<Enter>", 250);
                _comando.EnviarComando("5", 250);
                _comando.EnviarComando("<Enter>", 250);

                return true;
            }
            catch (Exception ex)
            {
                _logger.GerarLog("ERRO", "Erro ao acessar a tela 185 do STC.", "ClasseExtra", ex.Message);
                throw new ArgumentException("Erro ao acessar a tela 185 do STC. Erro: " + ex.Message);
            }
        }

        public bool FnFechaAvisosOtimz()
        {
            try
            {
                _comando.EsperarRetorno(250, 25);

                for (int i = 1; i <= 4; i++)
                {
                    string tela = _leitor.LerTelaInteira();
                    while (tela.Contains("A T E N C A O") ||
                           tela.Contains("ATENCAO") ||
                           tela.Contains("A L E R T A") ||
                           tela.Contains("OUTSOURCING") ||
                           tela.Contains("PROJETO ESCOLA"))
                    {
                        _comando.EnviarComando("<Enter>", 250, 50);

                        bool reiniciar = false;
                        tela = _leitor.LerTelaInteira();

                        if (_valida.DeveReiniciar(tela, ref reiniciar))
                            break;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool FnTelaInicialAA()
        {
            bool check = true;
            string tela = "";
            int cont = 1;

            _comando.EsperarRetorno(250, 50);
            if (EstaNaTelaAA())
                return true;

            _comando.EsperarRetorno(250, 50);
            if (EstaNaTelaCA())
                return EntrarTelaAA();

            _comando.EsperarRetorno(250, 50);
            if (EstaNaTelaHistorico())
            {
                do
                {
                    _comando.EnviarComando("<PF3>", 250, 50);
                    _comando.EsperarRetorno(250, 25);

                    tela = _leitor.LerArea(6, 20, 6, 50);

                    if (tela == "A = ATENDIMENTO A CLIENTES")
                        check = false;
                }
                while (check);

                if (tela == "A = ATENDIMENTO A CLIENTES")
                    return EntrarTelaAA();
            }

            do
            {
                if (cont > 20)
                    return false;

                _comando.EnviarComando("<PF3>", 250, 50);

                tela = _leitor.LerArea(6, 20, 6, 50);
                if (tela == "A = ATENDIMENTO A CLIENTES") 
                    check = false;

                cont++;
            } 
            while (check);

            if (tela == "A = ATENDIMENTO A CLIENTES")
                return EntrarTelaAA();

            _comando.EsperarRetorno(250, 50);
            return false;
        }

        public bool EstaNaTelaCA()
        {
            return _leitor.LerArea(5, 2, 5, 10) == "Circuito:";
        }

        private bool EstaNaTelaAA()
        {
            return _leitor.LerArea(5, 2, 5, 10) == "Telefone:";
        }

        private bool EstaNaTelaHistorico()
        {
            return _leitor.LerArea(3, 24, 3, 58) == "HISTORICO / OCORRENCIAS DE SERVICOS";
        }

        private bool EntrarTelaAA()
        {
            _comando.EnviarComando("<PF10>", 250, 50);
            _comando.MoverCursor(18, 29);
            _comando.EsperarRetorno(5, 50);
            _comando.EnviarComando("3");
            _comando.EnviarComando("<ENTER>",250, 50);

            FnFechaAvisosOtimz();

            _comando.MoverCursor(24, 6);
            _comando.EsperarRetorno(5, 50);
            _comando.EnviarComando("AA");
            _comando.EnviarComando("<ENTER>",250, 50);

            return EstaNaTelaAA();
        }
    }
}
