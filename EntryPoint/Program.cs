using ConsoleExecBUs.Core;
using ConsoleExecBUs.EntryPoint;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static void Main(string[] args)
    {
        var provider = DependencyInjection.Configure(args);

        var principal = provider.GetRequiredService<Principal>();

        principal.Inicio();
    }
}