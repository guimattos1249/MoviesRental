# MoviesRental

API para gerenciamento de uma locadora de DVDs construída com **.NET**, aplicando conceitos de **Clean Architecture**, **CQRS**, **Event-Driven Architecture** e separação entre modelos de leitura e escrita.

O objetivo principal do projeto é demonstrar, de forma prática, como estruturar uma aplicação distribuída em que operações de escrita e leitura utilizam tecnologias diferentes e são sincronizadas de forma assíncrona por eventos.

---

## Arquitetura

A solução utiliza uma arquitetura baseada em **CQRS (Command Query Responsibility Segregation)**.

Em vez de utilizar o mesmo banco de dados e o mesmo modelo para todas as operações, a aplicação separa:

- **Write Side**: responsável por comandos e persistência transacional.
- **Read Side**: responsável por consultas otimizadas para leitura.
- **Message Broker**: responsável por transportar eventos entre os dois lados.

Fluxo simplificado:

```text
                         ┌──────────────────┐
                         │      Client      │
                         └────────┬─────────┘
                                  │ HTTP
                                  ▼
                      ┌──────────────────────┐
                      │   MoviesRental API   │
                      │      Publisher       │
                      └──────────┬───────────┘
                                 │
                    Command / Application Layer
                                 │
                                 ▼
                      ┌──────────────────────┐
                      │     SQL Server       │
                      │     Write Model      │
                      └──────────┬───────────┘
                                 │
                              Event
                                 │
                                 ▼
                      ┌──────────────────────┐
                      │      RabbitMQ        │
                      │     + MassTransit    │
                      └──────────┬───────────┘
                                 │
                                 ▼
                      ┌──────────────────────┐
                      │ MoviesRental Consumer│
                      └──────────┬───────────┘
                                 │
                                 ▼
                      ┌──────────────────────┐
                      │       MongoDB        │
                      │      Read Model      │
                      └──────────┬───────────┘
                                 │
                                 ▼
                      ┌──────────────────────┐
                      │    Query Services    │
                      └──────────────────────┘
```

Esse modelo trabalha com **consistência eventual**: uma alteração confirmada no banco de escrita pode levar alguns milissegundos para aparecer no banco de leitura, pois a sincronização ocorre de forma assíncrona.

---

## Estrutura da solução

A solução é organizada aproximadamente da seguinte forma:

```text
src/
├── BuildingBlocks/
│   └── MoviesRental.Core/
│
├── Services/
│   ├── Publisher/
│   │   ├── API/
│   │   │   └── MoviesRental.API/
│   │   ├── Application/
│   │   │   └── MoviesRental.Application/
│   │   ├── Domain/
│   │   │   └── MoviesRental.Domain/
│   │   └── Infrastructure/
│   │       └── MoviesRental.Infrastructure/
│   │
│   └── Consumer/
│       └── MoviesRental.Consumer/
│
└── Shared/
    └── Queries/
        ├── Application/
        ├── Domain/
        └── Infrastructure/
```

> Os nomes das pastas podem variar levemente conforme a versão atual da solução, mas as responsabilidades arquiteturais permanecem as mesmas.

---

# Camadas

## API / Presentation

A camada de entrada da aplicação.

Responsabilidades principais:

- disponibilizar endpoints HTTP;
- receber requests;
- executar validações relacionadas ao contrato da API;
- encaminhar operações para a camada Application;
- configurar Dependency Injection;
- configurar OpenAPI;
- configurar MassTransit/RabbitMQ;
- configurar middleware e pipeline HTTP.

A API representa principalmente o **write side** da aplicação.

Ela não deve concentrar regras de negócio.

---

## Application

A camada Application coordena os casos de uso do sistema.

Exemplos:

```text
CreateDvd
UpdateDvd
DeleteDvd
CreateDirector
UpdateDirector
DeleteDirector
```

Ela é responsável por:

- executar casos de uso;
- coordenar entidades do domínio;
- acessar abstrações de persistência;
- publicar eventos;
- controlar o fluxo da operação.

A camada Application depende do Domain, mas não deve conhecer detalhes concretos de infraestrutura.

Exemplo conceitual:

```text
Controller
   ↓
Command / Handler
   ↓
Domain
   ↓
Repository Interface
```

A implementação concreta do repository fica na Infrastructure.

---

## Domain

É o núcleo da aplicação.

Contém os elementos que representam o negócio da locadora.

Exemplos:

```text
Dvd
Director
Genre
Domain Events
Repository Contracts
Domain Rules
```

As entidades protegem suas próprias invariantes.

Exemplo:

```csharp
dvd.UpdateTitle(title);
dvd.UpdateCopies(copies);
dvd.UpdatePublishedDate(published);
```

Em vez de permitir alterações indiscriminadas nas propriedades, regras importantes permanecem dentro da própria entidade.

Isso aproxima o projeto de um **Rich Domain Model**.

O Domain deve evitar dependências de:

- Entity Framework Core;
- MongoDB;
- RabbitMQ;
- ASP.NET Core;
- Redis.

Dessa maneira, a regra de negócio permanece independente das tecnologias externas.

---

## Infrastructure

A camada Infrastructure contém implementações técnicas.

Exemplos:

- `DbContext`;
- Entity Framework Core;
- repositories;
- mappings;
- SQL Server;
- configurações de banco;
- integrações externas.

Fluxo:

```text
Application
     │
     ▼
Repository Interface
     ▲
     │
Infrastructure Repository
     │
     ▼
SQL Server
```

Essa estrutura aplica o princípio da **Dependency Inversion**.

A camada interna define o contrato.

A camada externa fornece a implementação.

---

# Query Side

As operações de leitura são separadas do modelo de escrita.

O Query Side utiliza:

```text
MongoDB
```

Essa escolha permite que os documentos sejam armazenados já no formato mais adequado para consulta.

Enquanto o SQL Server mantém o modelo transacional:

```text
Director
Dvd
Relationships
Constraints
```

o MongoDB pode armazenar uma representação orientada à consulta.

Exemplo conceitual:

```json
{
  "id": "...",
  "title": "Interstellar",
  "genre": "SciFi",
  "director": {
    "id": "...",
    "name": "Christopher Nolan"
  },
  "copies": 5,
  "available": true
}
```

Assim, uma consulta pode retornar os dados necessários sem reconstruir constantemente agregados através de joins.

---

# Consumer

O projeto:

```text
MoviesRental.Consumer
```

é responsável por consumir eventos publicados pela aplicação.

Fluxo:

```text
MoviesRental.API
      │
      │ Event
      ▼
   RabbitMQ
      │
      ▼
MoviesRental.Consumer
      │
      ▼
   MongoDB
```

Exemplo conceitual:

```text
DvdCreated
DvdUpdated
DvdDeleted
DirectorCreated
DirectorUpdated
DirectorDeleted
```

Quando um evento é consumido, o Consumer atualiza o read model correspondente.

Isso mantém o banco de leitura sincronizado com o banco de escrita.

---

# RabbitMQ e MassTransit

O RabbitMQ funciona como **Message Broker**.

A aplicação utiliza **MassTransit** como abstração para comunicação com o broker.

Responsabilidades do RabbitMQ:

- transportar mensagens;
- desacoplar Publisher e Consumer;
- permitir processamento assíncrono;
- permitir retries;
- permitir filas independentes;
- permitir tratamento de falhas.

Fluxo:

```text
Publisher
    │
    ▼
RabbitMQ Exchange
    │
    ▼
Queue
    │
    ▼
Consumer
```

Uma vantagem importante desse modelo é que a API não precisa esperar o MongoDB ser atualizado para responder ao cliente.

Depois que a operação de escrita termina, a atualização do read model pode acontecer de forma assíncrona.

---

# SQL Server — Write Database

O SQL Server representa a fonte transacional do sistema.

Ele armazena o **write model**.

Responsabilidades:

- garantir integridade relacional;
- persistir entidades;
- aplicar constraints;
- executar transações;
- servir como fonte principal das alterações de negócio.

Tecnologia de acesso:

```text
Entity Framework Core
```

---

# MongoDB — Read Database

MongoDB representa o **read model**.

Ele é atualizado pelos Consumers através dos eventos recebidos pelo RabbitMQ.

Objetivos:

- otimizar consultas;
- permitir documentos desnormalizados;
- reduzir joins;
- separar carga de leitura da carga de escrita.

O MongoDB não deve ser tratado como a origem transacional da informação.

---

# Redis

A infraestrutura possui Redis disponível como mecanismo de cache.

O Redis pode ser utilizado principalmente no Query Side:

```text
Request
   │
   ▼
Redis
   │
   ├── HIT ──► Response
   │
   └── MISS
        │
        ▼
      MongoDB
        │
        ▼
      Cache
        │
        ▼
     Response
```

Ele permite diminuir a quantidade de consultas repetidas no banco de leitura.

---

# CQRS

O projeto aplica o princípio de **Command Query Responsibility Segregation**.

## Commands

Alteram estado.

Exemplos:

```text
CreateDvd
UpdateDvd
DeleteDvd
```

Fluxo:

```text
HTTP Request
    ↓
Controller
    ↓
Command Handler
    ↓
Domain
    ↓
SQL Server
    ↓
Domain / Integration Event
    ↓
RabbitMQ
```

## Queries

Apenas consultam dados.

Fluxo:

```text
HTTP Request
    ↓
Query Service
    ↓
Redis
    ↓
MongoDB
    ↓
Response
```

Commands e Queries possuem necessidades diferentes e podem evoluir de forma independente.

---

# Event-Driven Architecture

A comunicação entre os modelos de escrita e leitura é orientada a eventos.

Uma alteração no domínio gera uma mensagem que representa algo que aconteceu.

Exemplo:

```text
DvdCreated
```

Consumers interessados nesse fato podem reagir independentemente.

Essa distinção ajuda a manter baixo acoplamento entre os componentes.

---

# Consistência eventual

Por utilizar bancos diferentes e comunicação assíncrona, o sistema trabalha com **Eventual Consistency**.

Exemplo:

```text
T0  POST /dvds
T1  SQL Server atualizado
T2  API responde sucesso
T3  evento enviado ao RabbitMQ
T4  Consumer recebe evento
T5  MongoDB atualizado
```

Entre `T2` e `T5`, uma consulta extremamente rápida pode ainda encontrar o estado anterior.

Isso não é necessariamente um erro.

É uma característica esperada da arquitetura distribuída.

---

# Tecnologias

| Tecnologia | Responsabilidade |
|---|---|
| .NET | Runtime e plataforma |
| ASP.NET Core | API HTTP |
| Entity Framework Core | Persistência relacional |
| SQL Server | Write database |
| MongoDB | Read database |
| RabbitMQ | Message broker |
| MassTransit | Abstração de mensageria |
| Redis | Cache distribuído |
| Docker | Containerização |
| Docker Compose | Orquestração local |
| OpenAPI | Documentação da API |

---

# Ambiente Docker

A infraestrutura local é composta por containers independentes:

```text
moviesrental.api
moviesrental.consumer
writedb
querydb
cachedb
rabbitmq
```

Todos os serviços se comunicam por uma rede Docker compartilhada.

Portas normalmente utilizadas:

| Serviço | Porta |
|---|---|
| MoviesRental API | 8000 |
| Consumer | 8001 |
| SQL Server | 1433 |
| MongoDB | 27017 |
| Redis | 6379 |
| RabbitMQ | 5672 |
| RabbitMQ Management | 15672 |

Para subir o ambiente:

```bash
docker compose up -d
```

Para reconstruir as imagens:

```bash
docker compose up -d --build
```

Para acompanhar os containers:

```bash
docker compose ps
```

Logs da API:

```bash
docker compose logs -f moviesrental.api
```

Logs do Consumer:

```bash
docker compose logs -f moviesrental.consumer
```

---

# Fluxo completo de criação de um DVD

Um dos fluxos mais importantes para entender a arquitetura é a criação de um DVD.

```text
1. Cliente envia POST /dvds
          ↓
2. Controller recebe a request
          ↓
3. Application executa o caso de uso
          ↓
4. Domain valida as invariantes
          ↓
5. Repository persiste no SQL Server
          ↓
6. Operação é confirmada
          ↓
7. Evento DvdCreated é publicado
          ↓
8. RabbitMQ recebe a mensagem
          ↓
9. MoviesRental.Consumer processa o evento
          ↓
10. MongoDB recebe/atualiza o documento
          ↓
11. Queries passam a enxergar o novo DVD
```

Esse fluxo demonstra diversos conceitos comuns em sistemas distribuídos:

- Clean Architecture;
- CQRS;
- Dependency Inversion;
- Event-Driven Architecture;
- mensageria;
- processamento assíncrono;
- bancos poliglotas;
- consistência eventual.

---

# Decisões arquiteturais

## Por que SQL Server para escrita?

O modelo de escrita possui necessidades transacionais e relacionais.

SQL Server fornece:

- constraints;
- foreign keys;
- transações ACID;
- consistência forte;
- suporte adequado ao Entity Framework Core.

## Por que MongoDB para leitura?

O modelo de leitura não precisa necessariamente reproduzir o modelo relacional.

Documentos podem ser estruturados de acordo com a resposta esperada pela API.

Isso permite consultas simples e rápidas.

## Por que RabbitMQ?

Publisher e Consumer não precisam estar disponíveis simultaneamente.

A mensagem pode permanecer na fila até que o Consumer consiga processá-la.

Isso reduz o acoplamento entre os componentes.

## Por que Redis?

Consultas muito frequentes podem ser atendidas diretamente pelo cache, reduzindo carga no banco de leitura.

---

# Conceitos aplicados

Este projeto foi criado principalmente como exercício de arquitetura backend e permite praticar:

- Clean Architecture;
- SOLID;
- Dependency Injection;
- Repository Pattern;
- Rich Domain Model;
- CQRS;
- Event-Driven Architecture;
- RabbitMQ;
- MassTransit;
- processamento assíncrono;
- SQL Server;
- MongoDB;
- Redis;
- Docker;
- Docker Compose;
- consistência eventual;
- sistemas distribuídos.

---

# Próximos passos

Algumas evoluções naturais do projeto incluem:

- Transactional Outbox;
- Inbox / idempotência do Consumer;
- retry policies;
- Dead Letter Queue;
- correlation ID;
- logging estruturado;
- health checks;
- testes unitários;
- testes de integração;
- testes de arquitetura;
- observabilidade;
- cache com Redis;
- versionamento de eventos.

Uma trilha detalhada dessas melhorias está documentada em:

```text
docs\roadmap.md
```

---

## Objetivo do projeto

Mais do que implementar um CRUD de DVDs, este projeto busca demonstrar como componentes comuns de sistemas backend modernos podem trabalhar juntos.

O foco está em compreender os trade-offs de uma arquitetura distribuída e responder perguntas como:

- Como manter dois bancos sincronizados?
- O que acontece se o RabbitMQ estiver indisponível?
- Como evitar processamento duplicado?
- Como garantir que um evento não seja perdido?
- Como implementar retries sem duplicar dados?
- Qual banco é a fonte da verdade?
- O que significa consistência eventual?
- Quando CQRS vale a pena?
- Quando essa arquitetura seria complexidade desnecessária?

Essas decisões são tão importantes quanto o código que implementa as funcionalidades.
