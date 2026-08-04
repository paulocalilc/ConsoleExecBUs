ConsoleRodaBus

Automação corporativa para processamento de informações em sistemas legados (STC/EXTRA!), execução de BUs, atualização de bases e integração com banco de dados.

Objetivo

O ConsoleRodaBus foi desenvolvido para consolidar processos originalmente executados por macros VBA e rotinas manuais, centralizando a automação em uma única aplicação estruturada em C#.

Entre as funcionalidades atuais estão:

Execução automática de BUs.
Navegação e consulta em ambientes STC via EXTRA!.
Processamento de circuitos.
Atualização de informações em banco de dados.
Execução de procedures de marcação.
Atualização de dashboards.
Registro de logs operacionais e técnicos.
Arquitetura

A aplicação foi construída seguindo uma arquitetura em camadas, com separação clara de responsabilidades.

EntryPoint
    ↓
Core
    ↓
Application
    ↓
Business Rules
    ↓
STC Services
    ↓
Infrastructure
    ↓
Database

Estrutura do Projeto

ConsoleExecBUs
│
├── EntryPoint
│   └── Principal
│
├── Core
│   ├── ClasseInicializacao
│   └── DependencyInjection
│
├── Application
│   ├── StcExecutionService
│   └── CircuitoProcessor
│
├── Services
│   ├── CircuitoService
│   ├── Tela181Service
│   ├── StcCommandService
│   ├── StcScreenReaderService
│   └── StcValidationService
│
├── Infrastructure
│   ├── Database
│   │   └── ClasseSqlServer
│   │
│   ├── Extra
│   │   ├── ExtraAdapter
│   │   ├── ExtraConnectionService
│   │   ├── ExtraNavigationService
│   │   └── ExtraContext
│   │
│   └── Logging
│       └── ClasseLog
│
├── Domain
│   ├── CircuitoContexto
│   ├── SipContexto
│   └── ResultadoProcessamento
│
└── Interfaces

Fluxo de Execução

Inicialização
    ↓
Carregamento de parâmetros
    ↓
Busca de BOVs
    ↓
Processamento de BUs
    ↓
Execução STC
    ↓
Atualização de Banco
    ↓
Execução de marcações
    ↓
Atualização Dashboard
    ↓
Finalização

Tecnologias
.NET
C#
Dependency Injection
SQL Server
EXTRA! (IBM 3270)
Mainframe STC

Dependency Injection

O projeto utiliza injeção de dependências para desacoplamento das camadas.

Principais serviços registrados:

ILogger
IDatabaseService

IExtraConnectionService
IExtraNavigationService
IExtraAdapter

IStcCommandService
IStcScreenReaderService
IStcValidationService
IStcExecutionService

Integração com STC

Toda navegação no STC é abstraída através da camada de serviços.

ExtraAdapter
    ↓
StcCommandService
    ↓
StcScreenReaderService
    ↓
StcValidationService
    ↓
Serviços de negócio

Isso permite:
  reutilização de código;
  redução de duplicidade;
  manutenção simplificada;
  criação de novos processos utilizando a mesma infraestrutura.

Logs

A aplicação possui mecanismo centralizado de logs.

Funções suportadas:

Log técnico.
Log operacional.
Saída visual em console.
Rastreamento de erros.
Futuras Evoluções
Pesquisador Mainframe

Projeto planejado para consolidação de consultas atualmente distribuídas em diversas macros VBA.

Objetivos:

Importação de arquivos TXT.
Consulta em STC.
Consulta em SAC.
Persistência em banco de dados.
Exportação automática para Excel.
Processamento em lote.
Histórico de execuções.

Estrutura prevista:

Pesquisador Mainframe
│
├── Módulo STC
│
├── Módulo SAC
│
└── Infraestrutura Compartilhada

Autor

Paulo Daniel De Campos Calil Cadi

Área de Inteligência do Negócio B2B

Histórico

Projeto originado a partir da evolução e migração gradual de automações VBA para uma plataforma centralizada em C#, visando:

melhoria de manutenção;
redução de dependência de planilhas;
reutilização de componentes;
aumento da estabilidade operacional;
criação de uma base para futuras automações em ambientes legados.
