# MoviesRental — Roadmap de Melhorias e Estudos

Este documento contém uma trilha de evolução técnica para o projeto **MoviesRental**.

O objetivo não é transformar uma aplicação de locadora em um sistema gigantesco.

A proposta é utilizar o escopo atual — DVDs, diretores, API, SQL Server, MongoDB, RabbitMQ e Redis — como laboratório para estudar problemas reais de backend, arquitetura distribuída e mensageria.

Cada etapa abaixo foi escolhida pensando principalmente em:

- aprendizado prático;
- domínio de conceitos cobrados em entrevistas;
- capacidade de explicar decisões arquiteturais;
- criação de um projeto de portfólio tecnicamente interessante;
- evolução incremental, sem reescrever a aplicação inteira.

---

# Estado atual

A arquitetura possui aproximadamente:

```text
                   WRITE SIDE

Client
  │
  ▼
ASP.NET Core API
  │
  ▼
Application
  │
  ▼
Domain
  │
  ▼
EF Core
  │
  ▼
SQL Server
  │
  ▼
RabbitMQ
  │

                   READ SIDE
  │
  ▼
Consumer
  │
  ▼
MongoDB
  │
  ▼
Queries
```

Infraestrutura adicional:

```text
Redis
Docker
Docker Compose
MassTransit
```

Essa base já permite estudar problemas importantes de sistemas distribuídos.

---

# Ordem recomendada

A evolução sugerida é:

```text
1. Testes
2. Transactional Outbox
3. Idempotência / Inbox
4. Retry + Dead Letter Queue
5. Observabilidade
6. Redis Cache
7. Health Checks
8. Concorrência
9. Versionamento de eventos
10. Testes de arquitetura
11. CI
12. Documentação arquitetural
```

As quatro primeiras melhorias são as mais importantes.

---

# 1. Testes unitários

## Objetivo

Criar testes para regras do Domain e handlers da Application.

Não comece testando Controller.

Priorize regras de negócio.

Exemplos:

```text
DvdTests
DirectorTests
CreateDvdHandlerTests
UpdateDvdHandlerTests
DeleteDvdHandlerTests
```

Casos interessantes:

```text
não permitir título vazio
não permitir título abaixo do tamanho mínimo
não permitir quantidade negativa
alterar quantidade de cópias
marcar disponibilidade corretamente
não criar DVD para diretor inexistente
```

Ferramentas possíveis:

```text
xUnit
FluentAssertions
NSubstitute ou Moq
```

## O que você aprende

- unit tests;
- AAA;
- mocks;
- test doubles;
- isolamento;
- comportamento de domínio.

## Pergunta de entrevista

> O que você costuma testar?

Resposta que o projeto permitirá demonstrar:

> Eu priorizo regras de domínio e casos de uso. Controllers normalmente possuem pouca lógica e são melhor cobertos por testes de integração.

---

# 2. Testes de integração

Depois dos testes unitários, crie testes envolvendo infraestrutura real.

Use:

```text
Testcontainers
```

Containers possíveis:

```text
SQL Server
MongoDB
RabbitMQ
Redis
```

Exemplo:

```text
CreateDvdIntegrationTests
```

Teste:

```text
POST /dvds
      ↓
SQL Server recebe registro
      ↓
evento é publicado
      ↓
Consumer processa
      ↓
MongoDB possui documento
```

Esse único teste exercita praticamente toda a arquitetura.

## O que você aprende

- integration testing;
- infraestrutura efêmera;
- Docker em testes;
- comportamento distribuído.

---

# 3. Transactional Outbox

Esta é provavelmente a melhoria mais importante para o projeto.

## Problema atual

Imagine:

```text
1. API salva DVD no SQL Server
2. SaveChanges termina com sucesso
3. aplicação tenta publicar DvdCreated
4. RabbitMQ está indisponível
```

Resultado:

```text
SQL Server = atualizado
MongoDB    = desatualizado
```

O sistema perdeu o evento.

Esse é o clássico problema de **dual write**.

---

## Solução

Criar uma tabela:

```text
OutboxMessages
```

Exemplo:

```text
Id
Type
Payload
OccurredOn
ProcessedOn
Error
```

Dentro da mesma transação:

```text
BEGIN TRANSACTION

INSERT Dvd
INSERT OutboxMessage

COMMIT
```

Depois, um worker publica os eventos pendentes.

```text
SQL Server
   │
   ├── Dvd
   │
   └── OutboxMessages
           │
           ▼
     BackgroundService
           │
           ▼
        RabbitMQ
```

Agora a alteração de negócio e a intenção de publicar o evento são atômicas.

## Implementação

Você pode criar:

```text
OutboxMessage
OutboxProcessor
OutboxBackgroundService
```

O worker executaria periodicamente:

```sql
SELECT TOP 50 *
FROM OutboxMessages
WHERE ProcessedOn IS NULL
ORDER BY OccurredOn
```

Publica cada mensagem.

Depois:

```text
ProcessedOn = DateTime.UtcNow
```

## O que você aprende

- Transactional Outbox Pattern;
- dual writes;
- atomicidade;
- sistemas distribuídos;
- background workers.

## Pergunta de entrevista

> Como garantir que a mensagem não seja perdida depois de salvar no banco?

Esse projeto passará a fornecer um exemplo concreto para responder.

---

# 4. Inbox / Consumer idempotente

Resolver o Outbox ainda não resolve todos os problemas.

RabbitMQ trabalha normalmente com semântica próxima de:

```text
at-least-once delivery
```

Uma mensagem pode chegar mais de uma vez.

Imagine:

```text
Consumer recebe DvdCreated
Consumer atualiza MongoDB
Consumer cai antes de confirmar ACK
RabbitMQ envia novamente
```

O evento será processado duas vezes.

---

## Solução

Cada evento recebe:

```text
MessageId
```

No Consumer:

```text
if (Inbox.Contains(MessageId))
    return;
```

Depois de processar:

```text
Inbox.Add(MessageId)
```

Estrutura possível:

```text
InboxMessages
---------------
MessageId
Consumer
ProcessedAt
```

Também é possível tornar as operações do Mongo naturalmente idempotentes.

Por exemplo:

```text
ReplaceOne(
    filter: x => x.Id == event.Id,
    replacement: document,
    options: new ReplaceOptions { IsUpsert = true }
)
```

## O que você aprende

- idempotência;
- at-least-once delivery;
- duplicate messages;
- acknowledgements;
- delivery semantics.

---

# 5. Retry Policy

Falhas temporárias são normais.

Exemplo:

```text
MongoDB indisponível durante 3 segundos
```

Não é necessário mandar imediatamente a mensagem para uma fila de erro.

Configure retry no MassTransit.

Exemplo conceitual:

```text
Retry 1 → 1 segundo
Retry 2 → 5 segundos
Retry 3 → 10 segundos
```

Estratégias:

```text
Immediate
Interval
Incremental
Exponential
```

Para esse projeto:

```text
Incremental ou Exponential
```

são bons exercícios.

## O que você aprende

- transient failures;
- retry;
- exponential backoff;
- resiliency.

---

# 6. Dead Letter Queue / Error Queue

Depois de várias tentativas, algumas mensagens realmente não podem ser processadas.

Exemplo:

```text
Payload inválido
Evento incompatível
Erro de regra
Documento inconsistente
```

Essas mensagens não devem ficar sendo processadas infinitamente.

Fluxo:

```text
RabbitMQ
   │
   ▼
Consumer
   │
   ├── sucesso ──► ACK
   │
   └── falha
        │
        ▼
      Retry
        │
        ▼
    Error Queue
```

Com MassTransit, filas `_error` já fazem parte do modelo de tratamento de mensagens com falha.

Estude também:

```text
Dead Letter Exchange
Poison Message
```

## Exercício

Force propositalmente um Consumer a falhar.

Observe:

```text
RabbitMQ Management
Queues
Messages
_error
```

Depois investigue e reenvie a mensagem.

---

# 7. Correlation ID

Implemente um identificador por request.

Exemplo:

```text
HTTP Request
CorrelationId = a7f...
```

Esse ID acompanha:

```text
API
↓
Handler
↓
Event
↓
RabbitMQ
↓
Consumer
↓
MongoDB
```

Nos logs:

```text
[CorrelationId=a7f...] Creating DVD
[CorrelationId=a7f...] Dvd persisted
[CorrelationId=a7f...] DvdCreated published
[CorrelationId=a7f...] DvdCreated consumed
```

## O que você aprende

- distributed tracing;
- rastreabilidade;
- troubleshooting.

---

# 8. Logging estruturado

Adicionar:

```text
Serilog
```

Evite:

```csharp
_logger.LogInformation($"DVD {dvd.Id} created");
```

Prefira:

```csharp
_logger.LogInformation(
    "DVD {DvdId} created for Director {DirectorId}",
    dvd.Id,
    dvd.DirectorId);
```

Isso gera propriedades pesquisáveis.

Estude:

```text
Structured Logging
Log Levels
Enrichment
CorrelationId
```

---

# 9. OpenTelemetry

Depois de correlation ID e logging, implemente tracing distribuído.

Componentes possíveis:

```text
OpenTelemetry
Jaeger
```

Trace esperado:

```text
POST /dvds
   │
   ├── SQL INSERT
   │
   ├── RabbitMQ Publish
   │
   └── Consumer
           │
           └── MongoDB Update
```

Você passa a visualizar quanto tempo cada etapa levou.

## O que você aprende

- traces;
- spans;
- context propagation;
- observabilidade distribuída.

---

# 10. Redis Cache

O Redis já está presente na infraestrutura.

Implemente **Cache Aside** nas queries.

Fluxo:

```text
GET /dvds/{id}
       │
       ▼
     Redis
       │
   ┌───┴────┐
 HIT       MISS
  │          │
  ▼          ▼
return     MongoDB
             │
             ▼
          Redis SET
             │
             ▼
           return
```

Exemplo de chave:

```text
dvd:{id}
```

TTL:

```text
5 minutos
```

---

## Invalidação

Ao receber:

```text
DvdUpdated
```

o Consumer pode executar:

```text
DEL dvd:{id}
```

Assim, a próxima consulta reconstruirá o cache.

## O que você aprende

- cache aside;
- cache invalidation;
- TTL;
- distributed cache;
- stale data.

---

# 11. Health Checks

Adicionar endpoints:

```text
/health
/health/live
/health/ready
```

Verificar:

```text
SQL Server
MongoDB
Redis
RabbitMQ
```

Diferença:

### Liveness

```text
A aplicação está viva?
```

### Readiness

```text
A aplicação está pronta para receber tráfego?
```

Um processo pode estar vivo, mas não pronto.

Exemplo:

```text
API iniciou
SQL Server ainda indisponível
```

---

# 12. Concorrência otimista

Imagine dois usuários atualizando o mesmo DVD simultaneamente.

```text
User A lê Copies = 5
User B lê Copies = 5

User A salva Copies = 4
User B salva Copies = 3
```

Uma atualização pode sobrescrever a outra.

Adicione uma coluna:

```text
RowVersion
```

No EF Core:

```csharp
builder
    .Property(x => x.RowVersion)
    .IsRowVersion();
```

Então trate:

```text
DbUpdateConcurrencyException
```

## O que você aprende

- optimistic concurrency;
- lost update;
- race conditions;
- concurrency tokens.

---

# 13. Versionamento de eventos

Eventos também possuem contratos.

Imagine que hoje exista:

```json
{
  "id": "...",
  "title": "Matrix"
}
```

Depois você acrescenta:

```json
{
  "id": "...",
  "title": "Matrix",
  "genre": "SciFi"
}
```

Existem consumers antigos que talvez ainda estejam processando mensagens antigas.

Estude estratégias:

```text
DvdCreatedV1
DvdCreatedV2
```

ou contratos compatíveis.

## O que você aprende

- schema evolution;
- backward compatibility;
- event contracts.

---

# 14. Separar Domain Event de Integration Event

Esse é um ótimo refinamento arquitetural.

## Domain Event

Representa algo ocorrido dentro do domínio.

```text
DvdCreatedDomainEvent
```

Pode ser tratado dentro da própria aplicação.

## Integration Event

Representa algo que será comunicado para fora do bounded context/processo.

```text
DvdCreatedIntegrationEvent
```

Fluxo:

```text
Domain
   │
   ▼
Domain Event
   │
   ▼
Application
   │
   ▼
Integration Event
   │
   ▼
Outbox
   │
   ▼
RabbitMQ
```

Isso evita que detalhes de transporte contaminem o Domain.

---

# 15. Resiliência usando Polly

Estude Polly para integrações externas ou operações adequadas.

Conceitos:

```text
Retry
Timeout
Circuit Breaker
Fallback
```

Mesmo que o projeto atualmente possua poucas integrações HTTP externas, você pode criar uma pequena integração opcional, por exemplo:

```text
Movie Metadata Provider
```

Não precisa aumentar o domínio.

Apenas consulte dados complementares.

Mas essa etapa é opcional.

---

# 16. Testes de arquitetura

Adicione:

```text
NetArchTest
```

ou:

```text
NetArchTest.Rules
```

Crie testes como:

```text
Domain não pode depender de Infrastructure
Domain não pode depender de API
Application não pode depender de API
Infrastructure pode depender de Application
```

Exemplo conceitual:

```csharp
Types
    .InAssembly(domainAssembly)
    .ShouldNot()
    .HaveDependencyOn("MoviesRental.Infrastructure")
```

## O que você aprende

- architectural fitness functions;
- dependências;
- enforcement de arquitetura.

---

# 17. Docker Healthcheck

Além do HealthChecks da aplicação, coloque healthchecks no Docker Compose.

Exemplo conceitual:

```yaml
depends_on:
  rabbitmq:
    condition: service_healthy
```

Isso é mais robusto do que assumir que:

```text
container iniciado == serviço pronto
```

Esse detalhe costuma gerar bugs em ambientes distribuídos.

---

# 18. Migrações automáticas com cuidado

Estude duas estratégias.

## Estratégia simples

API executa:

```csharp
Database.Migrate();
```

na inicialização.

Boa para:

```text
ambiente local
projeto de estudo
```

## Estratégia mais próxima de produção

Pipeline separado:

```text
Build
↓
Migration Job
↓
Deploy
```

Isso evita que múltiplas instâncias tentem migrar simultaneamente.

Não precisa implementar Kubernetes para estudar o conceito.

---

# 19. GitHub Actions

Crie um workflow:

```text
push / pull request
        │
        ▼
dotnet restore
        │
        ▼
dotnet build
        │
        ▼
dotnet test
```

Depois:

```text
docker build
```

Opcionalmente:

```text
integration tests
```

Arquivo:

```text
.github/workflows/ci.yml
```

## O que você aprende

- CI;
- build automatizado;
- quality gate;
- pipelines.

---

# 20. API Versioning

Adicionar:

```text
/api/v1/dvds
```

Mesmo que exista apenas `v1`.

Estude:

```text
URL versioning
Header versioning
Query string versioning
```

Para o projeto:

```text
URL versioning
```

é o mais simples de demonstrar.

---

# 21. Problem Details

Padronize respostas de erro usando:

```text
RFC 9457 Problem Details
```

Exemplo:

```json
{
  "type": "https://moviesrental/errors/dvd-not-found",
  "title": "DVD not found",
  "status": 404,
  "detail": "The requested DVD does not exist."
}
```

## O que você aprende

- contratos HTTP;
- padronização de erros;
- REST APIs.

---

# 22. Paginação no Query Side

Implemente:

```text
GET /dvds?page=1&pageSize=20
```

Resposta:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 20,
  "total": 100
}
```

Depois estude:

```text
Offset Pagination
Cursor Pagination
```

Não precisa implementar ambas.

---

# 23. Filtros de consulta

Exemplos:

```text
GET /dvds?genre=Action
GET /dvds?directorId=...
GET /dvds?available=true
GET /dvds?title=matrix
```

MongoDB é adequado para experimentar índices e queries.

Depois verifique:

```text
Explain Plan
Indexes
```

---

# 24. Índices no MongoDB

Crie índices baseados nas consultas reais.

Exemplos:

```text
Title
DirectorId
Genre
Available
```

Não crie índices para tudo.

Estude o trade-off:

```text
mais índices
    =
leitura mais rápida
+
escrita mais cara
+
mais armazenamento
```

---

# 25. ADR — Architecture Decision Records

Crie:

```text
docs/adr/
```

Exemplos:

```text
0001-use-cqrs.md
0002-use-rabbitmq.md
0003-use-mongodb-read-model.md
0004-use-transactional-outbox.md
```

Estrutura:

```markdown
# Context

# Decision

# Consequences
```

Isso treina uma habilidade valorizada em vagas mais maduras: explicar **por que** uma decisão foi tomada.

---

# Roadmap recomendado na prática

## Fase 1 — Qualidade

Implementar:

```text
[ ] Unit Tests
[ ] Integration Tests
[ ] Problem Details
[ ] API Versioning
```

Objetivo:

```text
Qualidade de API
Testabilidade
Boas práticas
```

---

## Fase 2 — Mensageria confiável

Implementar:

```text
[ ] Transactional Outbox
[ ] Inbox / Idempotency
[ ] Retry
[ ] Dead Letter / Error Queue
```

Essa é a fase com maior valor para entrevistas.

Ao terminar, você deve conseguir explicar:

```text
at-most-once
at-least-once
exactly-once
idempotency
ack
retry
dead letter
outbox
dual write
eventual consistency
```

---

## Fase 3 — Observabilidade

Implementar:

```text
[ ] Correlation ID
[ ] Serilog
[ ] OpenTelemetry
[ ] Jaeger
[ ] Health Checks
```

Objetivo:

```text
descobrir o que aconteceu quando algo falha
```

---

## Fase 4 — Performance

Implementar:

```text
[ ] Redis Cache
[ ] Cache invalidation
[ ] MongoDB indexes
[ ] Pagination
```

Objetivo:

```text
entender performance do read side
```

---

## Fase 5 — Arquitetura

Implementar:

```text
[ ] Architecture Tests
[ ] ADRs
[ ] Event Versioning
[ ] Domain Event vs Integration Event
```

Objetivo:

```text
ser capaz de defender a arquitetura tecnicamente
```

---

## Fase 6 — DevOps

Implementar:

```text
[ ] GitHub Actions
[ ] Docker healthchecks
[ ] migration strategy
```

Não há necessidade de adicionar Kubernetes apenas para aumentar a lista de tecnologias.

É mais valioso dominar profundamente o que já existe.

---

# Perguntas de entrevista que esse projeto pode te ajudar a responder

Depois dessas melhorias, tente conseguir responder sem consultar documentação:

### Clean Architecture

```text
Por que Domain não depende de Infrastructure?
Onde ficam interfaces de repository?
Qual a diferença entre Application e Domain?
```

### CQRS

```text
O que é CQRS?
CQRS exige bancos diferentes?
Quando você não usaria CQRS?
```

### RabbitMQ

```text
O que são Exchange, Queue e Binding?
O que é ACK?
O que acontece se um Consumer cair?
O que é Dead Letter Queue?
```

### Sistemas distribuídos

```text
O que é consistência eventual?
Como lidar com dual writes?
Como evitar perda de mensagens?
Como evitar processamento duplicado?
```

### Outbox

```text
Qual problema o Transactional Outbox resolve?
Como o Outbox worker funciona?
O que acontece se o worker publicar e cair antes de marcar ProcessedOn?
```

A última pergunta é importante.

A resposta leva novamente à:

```text
idempotência
```

---

# Cenários de falha para testar manualmente

Uma excelente forma de aprender sistemas distribuídos é quebrá-los propositalmente.

---

## Cenário 1

Desligue MongoDB.

```bash
docker stop querydb
```

Crie um DVD.

Observe:

```text
SQL Server
RabbitMQ
Consumer
Retry
Error Queue
```

Depois ligue Mongo novamente.

---

## Cenário 2

Desligue RabbitMQ.

Crie um DVD.

Sem Outbox:

```text
evento pode ser perdido
```

Com Outbox:

```text
evento permanece pendente no SQL Server
```

Depois ligue RabbitMQ.

O worker deve publicar a mensagem.

---

## Cenário 3

Mate o Consumer enquanto ele processa.

Verifique se a mensagem é entregue novamente.

Isso demonstra:

```text
at-least-once delivery
```

---

## Cenário 4

Envie a mesma mensagem duas vezes.

MongoDB deve terminar no mesmo estado.

Isso demonstra:

```text
idempotência
```

---

## Cenário 5

Atualize simultaneamente o mesmo DVD.

Observe o comportamento antes e depois do:

```text
RowVersion
```

Isso demonstra concorrência otimista.

---

# Melhorias que eu evitaria inicialmente

Para esse projeto, eu não colocaria agora:

```text
Kubernetes
Service Mesh
Event Sourcing
GraphQL
gRPC
ElasticSearch
Saga distribuída complexa
Autenticação completa
Frontend
múltiplos microsserviços artificiais
```

Não porque sejam tecnologias ruins.

Mas porque aumentariam muito o escopo e desviariam do principal aprendizado.

O projeto já possui material suficiente para explorar conceitos avançados apenas tornando a arquitetura atual mais confiável e observável.

---

# Projeto final desejado

Depois dessa evolução, a arquitetura poderia ser:

```text
                         Client
                           │
                           ▼
                    ASP.NET Core API
                           │
                           ▼
                     Application
                           │
                           ▼
                       Domain
                           │
              ┌────────────┴────────────┐
              │                         │
              ▼                         ▼
         SQL Server                 Outbox
              │                         │
              └────────────┬────────────┘
                           │
                           ▼
                   Outbox Processor
                           │
                           ▼
                       RabbitMQ
                           │
                    Retry / DLQ
                           │
                           ▼
                       Consumer
                           │
                    Inbox Check
                           │
                           ▼
                       MongoDB
                           │
                           ▼
                         Redis
                           │
                           ▼
                        Queries
```

Observabilidade:

```text
Correlation ID
      +
Structured Logs
      +
OpenTelemetry
      +
Health Checks
```

Qualidade:

```text
Unit Tests
Integration Tests
Architecture Tests
CI Pipeline
```

Esse continua sendo o mesmo projeto de locadora.

A diferença é que ele passa a demonstrar problemas e soluções encontrados em sistemas backend reais.

---

# Checklist principal

## Qualidade

- [ ] Testes unitários
- [ ] Testes de integração com Testcontainers
- [ ] Problem Details
- [ ] API Versioning

## Mensageria

- [ ] Transactional Outbox
- [ ] Outbox Processor
- [ ] Message ID
- [ ] Consumer idempotente
- [ ] Inbox
- [ ] Retry Policy
- [ ] Error Queue / DLQ

## Observabilidade

- [ ] Correlation ID
- [ ] Serilog
- [ ] OpenTelemetry
- [ ] tracing do RabbitMQ
- [ ] Health Checks

## Performance

- [ ] Cache Aside com Redis
- [ ] Cache invalidation
- [ ] MongoDB indexes
- [ ] Pagination

## Concorrência

- [ ] RowVersion
- [ ] tratamento de DbUpdateConcurrencyException

## Arquitetura

- [ ] Domain Event
- [ ] Integration Event
- [ ] Event Versioning
- [ ] Architecture Tests
- [ ] ADRs

## DevOps

- [ ] GitHub Actions
- [ ] Docker healthchecks
- [ ] estratégia de migrations

---

# Meta de aprendizado

Ao finalizar esse roadmap, o objetivo não deve ser dizer:

> "Usei RabbitMQ, MongoDB, Redis e CQRS."

O objetivo deve ser conseguir explicar:

> "Eu usei CQRS para separar os modelos de leitura e escrita. O SQL Server funciona como fonte transacional, enquanto o MongoDB mantém uma projeção otimizada para leitura. A sincronização ocorre através de eventos enviados via RabbitMQ. Como uma gravação no banco e uma publicação no broker não fazem parte da mesma transação distribuída, implementei Transactional Outbox para impedir perda de eventos. Como o broker pode entregar a mesma mensagem mais de uma vez, tornei o Consumer idempotente e utilizei Inbox. Também configurei retries, fila de erro, correlation ID e tracing para tornar o fluxo resiliente e observável."

Quando você conseguir explicar naturalmente **o problema que cada técnica resolve**, o projeto já estará cumprindo muito bem seu objetivo de aprendizado e preparação para entrevistas.
