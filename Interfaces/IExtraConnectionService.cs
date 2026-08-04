using ConsoleExecBUs.Domain;

namespace ConsoleExecBUs.Interfaces
{
    public interface IExtraConnectionService
    {
        bool ValidaExtra();
        bool FnObjetosExtra();
        bool FnReconectar();
        bool FnLoginUsuario();
        bool AbreExtra();
        bool TrocarAmbiente(AmbienteStc ambiente);
        void FechaExtra();

    }
}
