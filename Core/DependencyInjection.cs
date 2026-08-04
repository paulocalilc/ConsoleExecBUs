using ConsoleExecBUs.Aplication.Services;
using ConsoleExecBUs.Application;
using ConsoleExecBUs.Domain;
using ConsoleExecBUs.EntryPoint;
using ConsoleExecBUs.Infrastructure.Database;
using ConsoleExecBUs.Infrastructure.Extra;
using ConsoleExecBUs.Infrastructure.Logging;
using ConsoleExecBUs.Interfaces;
using ConsoleExecBUs.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleExecBUs.Core
{
    public static class DependencyInjection
    {
        public static ServiceProvider Configure(string[] args)
        {
            var services = new ServiceCollection();

            // --------------------------------
            // Core / config
            // --------------------------------
            services.AddSingleton<ClasseInicializacao>();
            services.AddSingleton(args); // injeta args

            // --------------------------------
            // Infra
            // --------------------------------
            services.AddSingleton<ILogger, ClasseLog>();
            services.AddSingleton<IDatabaseService, ClasseSqlServer>();

            // Extra / COM
            services.AddSingleton<IExtraAdapter, ExtraAdapter>();
            services.AddSingleton<IExtraConnectionService, ExtraConnectionService>();
            services.AddSingleton<IExtraNavigationService, ExtraNavigationService>();

            services.AddSingleton<ExtraContext>(provider =>
            {
                var init = provider.GetRequiredService<ClasseInicializacao>();

                return new ExtraContext(
                    init.UsuarioExtra,
                    init.SenhaExtra,
                    AmbienteStc.RJ

                );
            });


            // --------------------------------
            // STC Services
            // --------------------------------
            services.AddSingleton<IStcCommandService, StcCommandService>();
            services.AddSingleton<IStcScreenReaderService, StcScreenReaderService>();
            services.AddSingleton<IStcValidationService, StcValidationService>();

            // --------------------------------
            // Application
            // --------------------------------
            services.AddSingleton<CircuitoService>();
            services.AddSingleton<Tela181Service>();
            services.AddSingleton<CircuitoProcessor>();
            services.AddSingleton<IStcExecutionService, StcExecutionService>();

            services.AddSingleton<TelaCAService>();
            services.AddSingleton<TelaAAService>();
            services.AddSingleton<SipProcessor>();
            services.AddSingleton<ISipExecutionService, SipExecutionService>();

            // --------------------------------
            // Entry
            // --------------------------------
            services.AddSingleton<Principal>();

            // --------------------------------
            // Comunicação
            // --------------------------------
            services.AddSingleton<IEmailService, EmailService>();

            return services.BuildServiceProvider();
        }
    }
}