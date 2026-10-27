# Visão Geral da Arquitetura

## 1. Arquitetura do MVP

O MVP adotará uma arquitetura em camadas inspirada em **Clean Architecture**, com uma interface web consumindo um backend centralizado por meio de uma API HTTP.

O backend será organizado em:

- **Domain:** entidades, objetos de valor, regras e eventos de domínio;
- **Application:** casos de uso, DTOs, validadores, interfaces e orquestração;
- **Infrastructure:** Entity Framework Core, PostgreSQL, persistência, autenticação e serviços externos;
- **API:** endpoints HTTP, autenticação, autorização, middleware e injeção de dependência.

As regras de negócio e as autorizações devem permanecer no backend. A interface web não deve duplicar regras nem ser considerada uma barreira de segurança.

## 2. Multi-tenancy e segurança

Toda operação operacional deve ser executada dentro de um contexto de condomínio validado. O backend deve aplicar autorização por papel e escopo de tenant, impedindo acesso ou alteração de dados de outro condomínio.

O domínio deve permanecer independente de frameworks, banco de dados e protocolos externos. Dependências de infraestrutura devem apontar para abstrações definidas nas camadas internas.

## 3. Concorrência e processamento assíncrono

O MVP deve tratar concorrência nas reservas e registrar alterações relevantes. Processamento assíncrono pode ser utilizado para tarefas internas que não bloqueiem requisições HTTP, desde que preserve revalidação de autorização, idempotência e rastreabilidade.

## 4. Evoluções posteriores

Clientes mobile e desktop, MQTT, telemetria, dispositivos físicos, notificações multicanal e outros serviços externos não fazem parte do MVP. Quando forem incluídos, deverão integrar-se pela camada de Infrastructure sem introduzir regras de negócio independentes nos clientes.