using ConsoleExecBUs.Domain;

namespace ConsoleExecBUs.Interfaces
{
    public interface IStcValidationService
    {
        bool StatusCircuitoValido(string status);
        bool TelaTemErroCritico(string tela);
        bool ErroCampoComando(string tela);
        bool FnTrataCCTOInvalido(CircuitoContexto ctx);
        bool DeveReiniciar(string tela, ref bool reiniciar);
        ResultadoProcessamento VerificarErroTela(string tela);
    }
}
