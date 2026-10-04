# FCG.PaymentsAPI

Microsserviço independente responsável exclusivamente pelo domínio financeiro da plataforma FCG. A implementação neste estágio estabelece a fundação técnica para evolução do processamento de pagamentos.

## Responsabilidades

- Futuramente consumir `OrderPlacedEvent` e receber os dados necessários para o processamento.
- Registrar e processar pagamentos, definir aprovação ou rejeição e garantir idempotência.
- Futuramente publicar `PaymentProcessedEvent`.

O serviço não é responsável por catálogo, pedidos, biblioteca, usuários ou notificações e não acessa bancos pertencentes a outros serviços. Os contratos de integração serão adicionados no card correspondente; entidades de domínio não serão usadas como contratos de mensagem.

## Tecnologia

- .NET 8 / ASP.NET Core
- Swagger/OpenAPI
- Health endpoint: `GET /health`
- Testes: xUnit

## Estrutura

- `src/FCG.Payments.Api`: host HTTP, configuração, DI e endpoints operacionais.
- `src/FCG.Payments.Application`: futura camada de casos de uso, consumers e abstrações.
- `src/FCG.Payments.Domain`: futuro modelo do domínio financeiro, sem dependências de outras camadas.
- `src/FCG.Payments.Infrastructure`: futuras implementações de persistência e mensageria.
- `tests/FCG.Payments.UnitTests`: testes unitários.
- `tests/FCG.Payments.IntegrationTests`: testes de integração do host.

Direção das dependências: Api → Application/Infrastructure; Infrastructure → Application/Domain; Application → Domain; Domain não depende dos demais projetos.

## Executar

Na raiz do repositório:

```sh
dotnet restore FCG.Payments.sln
dotnet run --project src/FCG.Payments.Api
```

Em Development, Swagger fica disponível em `/swagger`. O health endpoint fica em `/health`.

## Testar

```sh
dotnet test FCG.Payments.sln
```

## Configuração e segredos

A configuração padrão está em `appsettings.json` e `appsettings.Development.json`. Valores sensíveis não devem ser versionados; usar variáveis de ambiente, User Secrets localmente ou Kubernetes Secrets nos ambientes correspondentes. RabbitMQ, banco, Inbox e Outbox ainda não estão configurados.

## Status da implementação

Fundação do serviço criada: solução e projetos independentes em .NET 8, estrutura para as camadas, host ASP.NET Core com DI, Problem Details, Swagger e health check, além de projetos de testes. Domínio de pagamento, banco/migrations, consumer RabbitMQ, contratos de eventos, processamento financeiro e idempotência permanecem para cards seguintes.