using ConsoleExecBUs.Domain;

public static class AmbienteStcResolver
{
    public static string ObterEdp(AmbienteStc ambiente)
    {
        return ambiente switch
        {
            AmbienteStc.RJ =>
                @"C:\Sessões do Extra\RJ(10.31.15.28).edp",

            AmbienteStc.NE =>
                @"C:\Sessões do Extra\AP-AM-CE-MA-PB-PA-PE-PI-RN-RR(10.31.15.32).edp",

            AmbienteStc.BA =>
                @"C:\Sessões do Extra\BA-AL-SE(10.31.9.25).edp",

            AmbienteStc.MG =>
                @"C:\Sessões do Extra\MG-ES(10.31.9.100).edp",

            AmbienteStc.R2 =>
                @"C:\Sessões do Extra\AC-DF-GO-MS-MT-PR-RO-RS-SC-TO.edp",

            _ => throw new ArgumentOutOfRangeException()
        };
    }
}