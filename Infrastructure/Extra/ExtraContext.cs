using ConsoleExecBUs.Domain;

namespace ConsoleExecBUs.Infrastructure.Extra
{
    public class ExtraContext
    {
        public string Usuario { get; }
        public string Senha { get; }

        public AmbienteStc Ambiente { get; private set; }

        public string ArquivoEdp => AmbienteStcResolver.ObterEdp(Ambiente);

        public ExtraContext(
            string usuario,
            string senha,
            AmbienteStc ambiente = AmbienteStc.RJ)
        {
            Usuario = usuario;
            Senha = senha;
            Ambiente = ambiente;
        }

        public void AlterarAmbiente(AmbienteStc ambiente)
        {
            Ambiente = ambiente;
        }
    }
}
