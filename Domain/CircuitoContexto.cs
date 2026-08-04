namespace ConsoleExecBUs.Domain
{
    public class CircuitoContexto
    {
        // 🔹 Identificação do circuito
        public string Localidade { get; set; }
        public string Circuito { get; set; }
        public string LocNum { get; set; }

        // 🔹 Dados principais
        public string? Cliente { get; set; }
        public string? Cnpj { get; set; }
        public string? Produto { get; set; }
        public string? StatusCircuito { get; set; }

        // 🔹 Endereço A (principal)
        public string? UfA { get; set; }
        public string? TipoPessoa { get; set; }
        public string? TipoLogradouroA { get; set; }
        public string? LogradouroA { get; set; }
        public string? NumeroA { get; set; }
        public string? BairroA { get; set; }
        public string? Bairro { get; set; }
        public string? LogradouroCompleto { get; set; }

        // 🔹 Endereço STC (consultado tela 181)
        public string? CodLocal { get; set; }
        public string? CodLogradouro { get; set; }
        public string? Endereco { get; set; }
        public string? LogradouroTrechado { get; set; }
        public string? Cep { get; set; }
        public string? BairroSTC { get; set; }

        // 🔹 Ponta A e B
        public string? LocalA { get; set; }
        public string? EstacaoA { get; set; }
        public string? GraA { get; set; }

        public string? UfB { get; set; }
        public string? LocalB { get; set; }
        public string? EstacaoB { get; set; }
        public string? GraB { get; set; }

        // 🔹 Técnica
        public string? TecRota { get; set; }

        // 🔹 Características
        public string? Velocidade { get; set; }
        public string? QtdeFios { get; set; }
        public string? Modalidade { get; set; }

        // 🔹 Outras empresas
        public string? Isp { get; set; }
        public string? Designacao { get; set; }
        public string? TextoOutrasEmpresas { get; set; }

        // 🔹 Auxiliares
        public string? TipoLogradBD { get; set; }
        public string? TipoLogrCorrigido { get; set; }

        // 🔹 Controle de execução
        public int Refazer { get; set; }
        public int ConfirmacaoCircuito { get; set; }
        public string? AuxExec { get; set; }

        // 🔹 Controle batch
        public int QtdRegistros { get; set; }
        public int QtdRegistrosProcessados { get; set; }
        public int ContCarga { get; set; }
    }
}
