using ConsoleExecBUs.Domain;
using ConsoleExecBUs.Interfaces;
using System.Diagnostics;
using static ConsoleExecBUs.Core.ClasseExtra;

namespace ConsoleExecBUs.Infrastructure.Extra
{
    public class ExtraConnectionService : IExtraConnectionService
    {
        private readonly ILogger _logger;
        private readonly IStcCommandService _comando;
        private readonly IStcScreenReaderService _leitor;
        private readonly IExtraNavigationService _naveg;
        private readonly IStcValidationService _valida;
        private readonly ExtraContext _context;
        private readonly IExtraAdapter _extra;

        public ExtraConnectionService(
            IStcCommandService comando,
            IStcScreenReaderService leitor,
            IExtraNavigationService naveg,
            ExtraContext context,
            ILogger logger,
            IStcValidationService valida,
            IExtraAdapter extra)
        {
            _comando = comando;
            _leitor = leitor;
            _naveg = naveg;
            _context = context;
            _logger = logger;
            _valida = valida;
            _extra = extra;
        }

        public bool ValidaExtra()
        {
            string inconsistenciasPreenchimento = string.Empty;

            try
            {
                _extra.AbrirSessao(_context.ArquivoEdp);

                _extra.SelecionarSessao(_context.ArquivoEdp);

                _extra.TornarVisivel();

                if (_extra.EstaConectado())
                {
                    _extra.Desconectar();
                }

                _extra.Conectar();

                _extra.DefinirTelaAtual();

                //"C:\\Temp\\RJ.edp"
                return true;
            }
            catch (Exception ex)
            {
                _logger.GerarLog("ERRO", string.Concat("Foram detectados erros na inicialização dos métodos do Extra."), "ClasseExtra", ex.Message);
                throw new ArgumentException(string.Concat("Foram detectados erros na inicialização dos métodos do Extra.", ex.Message));
            }
        }

        public bool FnObjetosExtra()
        {
            try
            {
                _extra.AbrirSessao(_context.ArquivoEdp);

                _extra.SelecionarSessao(_context.ArquivoEdp);

                _extra.TornarVisivel();

                if (_extra.EstaConectado())
                {
                    _extra.Desconectar();

                    _comando.EsperarRetorno(250, 1000);
                }

                _extra.Conectar();

                _extra.DefinirTelaAtual();

                var waitobject = PropMyScreen.MyScreen.WaitForString("Application:");

                return true;
            }
            catch (Exception ex)
            {
                _logger.GerarLog("ERRO", "Erro ao criar os objetos do EXTRA.", "ClasseExtra", ex.Message);
                throw new ArgumentException("Erro ao criar os objetos do EXTRA. Erro: " + ex.Message);
            }
        }

        public bool FnLoginUsuario()
        {
            try
            {
                if (_context.Usuario == null)
                {
                    _logger.GerarLog("ERRO", "Usuário do Extra vazio, rode o parâmetro '-usuarioExtra' preenchido para rodar esse robô", "Classe RodaExtra", "");
                    throw new ArgumentException("Usuário do Extra vazio, rode o parâmetro '-usuarioExtra' preenchido para rodar esse robô");
                }

                if (_context.Senha == null)
                {
                    _logger.GerarLog("ERRO", "Senha do Extra vazia, rode o parâmetro \"-senhaExtra\" preenchida para rodar esse robô", "Classe RodaExtra", "");
                    throw new ArgumentException("Senha do Extra vazia, rode o parâmetro \"-senhaExtra\" preenchida para rodar esse robô");
                }

                if (PropSessao.Sessao.Visible == 0)
                {
                    PropSessao.Sessao.Visible = 1;
                }

                string tela = _leitor.LerArea(3, 1, 3, 13);
                int contador = 1;

                do
                {
                    tela = _leitor.LerArea(3, 1, 3, 13);
                    _comando.EsperarRetorno(1000, 1000);

                    if (contador > 30)
                    {
                        FnObjetosExtra();
                        contador = 1;
                    }
                    contador++;
                }
                while (tela != "Application:");

                _comando.MoverCursor(3, 15);
                _comando.EsperarRetorno(1000, 1000);
                _comando.EnviarComando("CHR", 250, 25);
                _comando.EnviarComando("<ENTER>", 250, 50);

                string verif = _leitor.LerArea(16, 2, 16, 8);
                int contLogin = 1;

                while (verif != "USUARIO")
                {
                    _comando.ProcessamentoStc();

                    verif = _leitor.LerArea(16, 2, 16, 8);
                    bool reiniciar = false;

                    if (_valida.DeveReiniciar(verif, ref reiniciar))
                    {
                        break;
                    }

                    if (contLogin >= 30)
                    {
                        FnReconectar();
                        contLogin = 1;
                    }

                    contLogin += contLogin;
                }

                _comando.MoverCursor(16, 23);
                _comando.EnviarComando(_context.Usuario);
                _comando.MoverCursor(17, 23);
                _comando.EnviarComando(_context.Senha);
                _comando.EnviarComando("<ENTER>");

                tela = _leitor.LerArea(1, 15, 1, 28);
                if (tela == "rio Inexistent")
                {
                    _logger.CaixaTexto("Usuário inexistente!");
                }
                else if (tela == "Wrong password")
                {
                    _logger.CaixaTexto("Senha incorreta!");
                }
                else if (string.IsNullOrWhiteSpace(tela))
                {
                    _logger.CaixaTexto("Erro de login!");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.GerarLog("ERRO", "Erro ao abrir o EXTRA.", "ClasseExtra", ex.Message);
                throw new ArgumentException("Erro ao abrir o EXTRA. Erro: " + ex.Message);
            }
        }

        public bool FnReconectar()
        {
            try
            {
                FnObjetosExtra();
                FnLoginUsuario();
                _naveg.FnTelaInicial113CA();
                return true;
            }
            catch (Exception ex)
            {
                _logger.GerarLog("ERRO", "Erro ao reconectar no STC.", "ClasseExtra", ex.Message);
                throw new ArgumentException("Erro ao reconectar no STC. Erro: " + ex.Message);
            }
        }

        public bool FnReconectarAA()
        {
            try
            {
                FnObjetosExtra();
                FnLoginUsuario();
                _naveg.FnTelaInicialAA();
                return true;
            }
            catch (Exception ex)
            {
                _logger.GerarLog("ERRO", "Erro ao reconectar no STC.", "ClasseExtra", ex.Message);
                throw new ArgumentException("Erro ao reconectar no STC. Erro: " + ex.Message);
            }
        }

        public bool AbreExtra()
        {
            try
            {
                Process ProcessoExterno = new Process();

                foreach (var processo in Process.GetProcessesByName("EXTRA"))
                {
                    processo.Kill();
                }

                Thread.Sleep(500);

                ProcessoExterno.StartInfo = new ProcessStartInfo(_context.ArquivoEdp)
                {
                    UseShellExecute = true
                };
                ProcessoExterno.Start();
                Thread.Sleep(5000);

                return true;
            }
            catch (Exception ex)
            {
                _logger.GerarLog("ERRO", "Erro ao abrir o EXTRA.", "ClasseExtra", ex.Message);
                throw new ArgumentException("Erro ao abrir o EXTRA. Erro: " + ex.Message);
            }
        }

        public void FechaExtra()
        {
            Process ProcessoExterno = new Process();

            foreach (var processo in Process.GetProcessesByName("EXTRA"))
            {
                processo.Kill();
            }

            Thread.Sleep(500);
        }

        public bool TrocarAmbiente(AmbienteStc ambiente)
        {
            _logger.CaixaTexto($"Trocando ambiente para {ambiente}");

            _context.AlterarAmbiente(ambiente);

            foreach (var processo in Process.GetProcessesByName("EXTRA"))
            {
                processo.Kill();
            }

            Thread.Sleep(500);

            Process.Start(new ProcessStartInfo(AmbienteStcResolver.ObterEdp(ambiente))
            {
                UseShellExecute = true
            });

            Thread.Sleep(5000);

            ValidaExtra();

            FnObjetosExtra();

            FnLoginUsuario();

            return true;
        }
    }
}
