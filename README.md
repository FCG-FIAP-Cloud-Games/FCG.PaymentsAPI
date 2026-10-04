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
- `src/FCG.Payments.Application`: casos de uso de criação idempotente e atualização de status, além das abstrações.
- `src/FCG.Payments.Domain`: pagamentos, tentativas e regras de transição do domínio financeiro, sem dependências de outras camadas.
- `src/FCG.Payments.Infrastructure`: persistência EF Core exclusiva do serviço e futuras integrações de mensageria.
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

A configuração padrão está em `appsettings.json` e `appsettings.Development.json`. Configure o banco próprio do serviço por `ConnectionStrings__PaymentsDatabase` (ou `ConnectionStrings:PaymentsDatabase` em User Secrets); o valor versionado é vazio e não contém credenciais. Em produção, injete a configuração por Kubernetes Secrets. Persistência usa SQL Server e as migrations pertencem ao projeto Infrastructure. O serviço não cria chaves estrangeiras para Order, User ou Game, que são identificadores externos. Uma tentativa financeira é registrada em `PaymentAttempt` e permanece ligada ao pagamento lógico; `OrderId` é único para impedir duplicidade.

## Status da implementação

Fundação financeira implementada: pagamentos `Pending` por padrão, transições para `Approved`/`Rejected`, tentativas, persistência própria com unicidade por `OrderId` e caso de uso idempotente para registrar pedidos. Consumer RabbitMQ, contratos de eventos e publicação de `PaymentProcessedEvent` permanecem para cards de mensageria seguintes.