using ConsoleExecBUs.Domain;
using ConsoleExecBUs.Interfaces;
using Microsoft.Win32;

namespace ConsoleExecBUs.Application
{
    public class TelaCAService
    {
        private readonly IStcCommandService _comando;
        private readonly IStcScreenReaderService _leitor;
        private readonly IStcValidationService _valida;
        private readonly IExtraConnectionService _conn;
        private readonly IExtraNavigationService _naveg;

        public TelaCAService(
            IStcCommandService comando,
            IStcScreenReaderService leitor,
            IStcValidationService valida,
            IExtraConnectionService conn,
            IExtraNavigationService naveg)
        {
            _comando = comando;
            _leitor = leitor;
            _valida = valida;
            _conn = conn;
            _naveg = naveg;
        }

        public bool Processar(SipContexto ctx)
        {
            if (_valida.TelaTemErroCritico(_leitor.LerTelaInteira()))
                _conn.FnReconectar();

            string tela = _leitor.LerArea(18, 1, 18, 28);
            
            if (tela == "Comando Direto(Codigo)..:")
            {
                if (!_naveg.FnTelaInicial113CA())
                    return false;
            }

            tela = _leitor.LerArea(5, 1, 5, 11);

            if (tela != "Circuito:")
            {
                _conn.FnReconectar();
                if (!_naveg.FnTelaInicial113CA())
                    return false;
            }

            _comando.EsperarRetorno(250, 50);

            if (!ConsultarCircuito(ctx))
                return false;

            _comando.EsperarRetorno(250, 50);

            if (!EntrarDadosEspecificosCircuito())
                return false;
            
            _comando.EsperarRetorno(250, 50);

            ProcessarQuantidadeFios(ctx);
            _comando.EsperarRetorno(250, 50);

            ProcessarNumeroFisico(ctx);
            _comando.EsperarRetorno(250, 50);

            ProcessarPef(ctx);
            _comando.EsperarRetorno(250, 50);

            return true;
        }

        private bool ConsultarCircuito(SipContexto ctx)
        {
            _comando.MoverCursor(5, 12);
            _comando.EnviarComando("<eraseeof>", 250, 50);
            _comando.EnviarComando(ctx.Localidade, 250, 50);

            _comando.MoverCursor(5, 17);
            _comando.EnviarComando("<eraseeof>", 250, 50);
            _comando.EnviarComando(ctx.Circuito, 250, 50);
            _comando.EnviarComando("<Enter>", 250, 50);

            _naveg.FnFechaAvisosOtimz();

            return _naveg.EstaNaTelaCA();
        }

        private bool EntrarDadosEspecificosCircuito()
        {
            _comando.EnviarComando("<PF9>", 250, 50);
            _comando.EnviarComando("<PF4>", 250, 50);
            _comando.EnviarComando("<PF4>", 250, 50);

            var tela = _leitor.LerTelaInteira();

            return tela.Contains("Dados Especificos do Circuito");
        }

        private void ProcessarNumeroFisico(SipContexto ctx)
        {
            if (!EntrarNumeroFisico())
            {
                RetornaTelaAA();
                return;
            }

            _comando.EsperarRetorno(250, 125);
            var terminal = _leitor.LerArea(7, 55, 7, 80);

            _comando.EsperarRetorno(250, 125);
            var ddd = _leitor.LerArea(9, 63, 9, 64);

            _comando.EsperarRetorno(250, 125);

            if (!string.IsNullOrWhiteSpace(terminal))
            {   
                ctx.NumeroChave = $"{ddd}{terminal.Replace("-", "")}";
            }

            SairNumeroFisico();
        }

        private void RetornaTelaAA()
        {
            var tela = _leitor.LerArea(5, 2, 5, 11);

            do
            {
                if (tela != "Circuito:")
                    _comando.EnviarComando("<PF3>");

                tela = _leitor.LerArea(5, 2, 5, 11);
            }
            while (tela != "Circuito:");
        }

        private bool EntrarNumeroFisico()
        {
            _comando.MoverCursor(18, 16);

            _comando.EnviarComando("x<ENTER>", 250, 125);

            var tela = _leitor.LerTelaInteira();

            return tela.Contains("Numero Fisico");
        }

        private void SairNumeroFisico()
        {
            string tela = "";
            bool check = true;

            do
            {
                _comando.EnviarComando("<PF3>");

                tela = _leitor.LerArea(3, 30, 3, 60);
                if (tela == "Consulta Cadastral Circuito")
                    check = false;
                _comando.EsperarRetorno(250, 125);

            } while (check);
        }

        private void ProcessarPef(SipContexto ctx)
        {
            if (!EntrarListaPef())
                return;
            
            if (!EntrarDesignacaoRota())
                return;
            
            ctx.ReferenciaPef = LerReferenciaPef();
            
            ctx.DescricaoPef = LerDescricaoPef();
            
            if (string.IsNullOrWhiteSpace(ctx.NumeroChave) || 
                    !ctx.NumeroChave.All(char.IsDigit) || 
                    ctx.NumeroChave.Length < 10)
            {
                ctx.NumeroChave = ObterNumeroChavePelaPef(ctx.ReferenciaPef);
            }

            SairDesignacaoRota();
        }

        private string ObterNumeroChavePelaPef(string referenciaPef)
        {
            if (string.IsNullOrWhiteSpace(referenciaPef))
                return string.Empty;

            var d0 = referenciaPef;

            d0 = d0.Replace(" ", "");
            d0 = d0.Replace(":", "");
            d0 = d0.Replace("-", "");
            d0 = d0.Replace("CN", "");
            d0 = d0.Replace("CH", "");
            d0 = d0.Replace("(", "");
            d0 = d0.Replace(")", "");

            while (d0.Contains("//"))
            {
                d0 = d0.Replace("//", "/");
            }

            var partes = d0.Split('/');

            string numeroChave = string.Empty;

            if (!d0.Contains("/"))
            {
                if (d0.Contains("FR"))
                {
                    d0 = d0.Replace("CN", "");
                    d0 = d0.Replace("CH", "");
                    d0 = d0.Replace("FR", "/");

                    partes = d0.Split('/');

                    if (partes.Length > 0)
                    {
                        numeroChave = partes[0];
                    }
                }
            }
            else
            {
                if (partes.Length >= 2)
                {
                    bool chaveCompleta = partes[0].Length == 10 && partes[0].All(char.IsDigit);

                    if (chaveCompleta)
                    {
                        numeroChave = partes[0];
                    }
                    else
                    {
                        var numero = partes[1];
                        var posFr = numero.IndexOf("FR", StringComparison.OrdinalIgnoreCase);
                        // Quando a faixa de ramais vem concatenada ao telefone
                        // (ex.: 81/32510785FR0750A0799), ignora tudo após FR.
                        if (posFr > 0)
                        {
                            numero = numero[..posFr];
                            numeroChave = partes[0][^2..] + numero;
                            return numeroChave;
                        }

                        numeroChave = partes[0][^2..] + partes[1];
                    }
                }
            }

            return numeroChave;
        }

        private bool EntrarListaPef()
        {
            int tentativas = 0;

            while (_leitor.LerArea(16, 51, 16, 59) != "Lista PEF")
            {
                var tela = _leitor.LerTelaInteira();
                if (_valida.TelaTemErroCritico(tela))
                {
                    return false;
                }

                _comando.EnviarComando("<PF3>");

                tentativas++;

                if (tentativas > 10)
                    return false;
            }

            _comando.MoverCursor(16, 49);

            _comando.EnviarComando("x<ENTER>", 125, 125);

            return true;
        }

        private bool EntrarDesignacaoRota()
        {
            var tela = _leitor.LerTelaInteira();

            return tela.Contains("DESIGNACAO DA ROTA");
        }

        private string LerReferenciaPef()
        {
            return _leitor.LerArea(8, 10, 8, 49).Replace("_", "");
        }

        private string LerDescricaoPef()
        {
            var linhas = new[]
            {
                _leitor.LerArea(9, 15, 9, 74),
                _leitor.LerArea(10, 15, 10, 74),
                _leitor.LerArea(11, 15, 11, 74),
                _leitor.LerArea(12, 15, 12, 74)
            };

            return string.Join(" ",linhas.Where(x => !string.IsNullOrWhiteSpace(x))).Replace("_", "").Trim();
        }

        private void SairDesignacaoRota()
        {
            string tela = "";
            bool check = true;

            do
            {
                _comando.EnviarComando("<PF3>");

                tela = _leitor.LerArea(5, 1, 5, 11);
                if (tela == "Circuito:")
                    check = false;
                else
                {
                    tela = _leitor.LerArea(6, 25, 6, 50);

                    if (tela == "ATENDIMENTO A CLIENTES")
                    {
                        check = false;
                        _comando.MoverCursor(24, 6);
                        _comando.EnviarComando("CA");
                        _comando.EnviarComando("<ENTER>");
                    }
                }
                _comando.EsperarRetorno(250, 125);

            } while (check);

            _naveg.FnFechaAvisosOtimz();
        }



        private void ProcessarQuantidadeFios(
            SipContexto ctx)
        {
            if (!EntrarTelaFios())
                return;

            ctx.NumeroFios = _leitor.LerArea(13, 41, 13, 80);

            _comando.EnviarComando("<PF3>");
        }

        private bool EntrarTelaFios()
        {
            _comando.MoverCursor(13, 16);

            _comando.EnviarComando("x<ENTER>");

            var tela = _leitor.LerTelaInteira();

            return tela.Contains("FIOS");
        }

    }
}
