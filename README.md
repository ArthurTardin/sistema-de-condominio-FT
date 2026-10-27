# Sistema de Gestão Condominial

<p align="center">
  <strong>Gestão condominial centralizada, rastreável e integrada.</strong>
</p>

<p align="center">
  Plataforma para centralização das operações administrativas, serviços aos moradores e futura integração com infraestrutura inteligente.
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Status-Em%20Desenvolvimento-yellow" alt="Status do projeto">
  <img src="https://img.shields.io/badge/Database-PostgreSQL-blue?logo=postgresql" alt="PostgreSQL">
  <img src="https://img.shields.io/badge/ORM-Entity%20Framework%20Core-purple" alt="Entity Framework Core">
  <img src="https://img.shields.io/badge/Auth-JWT-orange" alt="JWT">
  <img src="https://img.shields.io/badge/IoT-MQTT-green" alt="MQTT">
</p>

<p align="center">
  <a href="#sobre-o-projeto">Sobre</a> •
  <a href="#funcionalidades">Funcionalidades</a> •
  <a href="#perfis-de-acesso">Perfis</a> •
  <a href="#tecnologias">Tecnologias</a> •
  <a href="#arquitetura">Arquitetura</a> •
  <a href="#como-executar">Instalação</a> •
  <a href="#documentação">Documentação</a>
</p>

---

## Sobre o projeto

O **Sistema de Gestão Condominial** é uma plataforma destinada a centralizar operações administrativas, serviços aos moradores e processos relacionados à infraestrutura de condomínios em um único sistema.

A solução busca reduzir a fragmentação causada pelo uso de ferramentas desconectadas e processos manuais, proporcionando maior rastreabilidade, organização e integração entre moradores, funcionários, administradores e, futuramente, dispositivos inteligentes.

A primeira versão do sistema será desenvolvida como um **MVP focado no núcleo operacional**, contemplando autenticação, cadastros, reservas, chamados e histórico das operações.

Funcionalidades mais avançadas, como gestão financeira, integração com dispositivos IoT e controle físico de acesso, serão incorporadas posteriormente, após a validação do núcleo do produto.

> **Status atual:** Definição e implementação planejada do MVP.
>
> As funcionalidades descritas neste README representam o escopo planejado e não significam que já estejam implementadas.

### Objetivos

- Centralizar informações e operações condominiais.
- Simplificar reservas de espaços comuns.
- Facilitar o registro e acompanhamento de chamados.
- Melhorar a rastreabilidade das operações.
- Isolar os dados de diferentes condomínios.
- Preparar a plataforma para integrações com dispositivos IoT.
- Estabelecer uma base arquitetural capaz de evoluir para novos módulos.

## Funcionalidades

### Núcleo do MVP

- [ ] Autenticação e autorização de usuários.
- [ ] Gerenciamento de usuários.
- [ ] Gerenciamento de unidades.
- [ ] Gerenciamento de espaços comuns.
- [ ] Consulta de disponibilidade.
- [ ] Criação de reservas.
- [ ] Cancelamento de reservas.
- [ ] Registro de ocorrências.
- [ ] Abertura de chamados.
- [ ] Acompanhamento de chamados.
- [ ] Histórico das operações relevantes.
- [ ] Auditoria.
- [ ] Isolamento de dados entre condomínios.

### Evoluções planejadas

- [ ] Gestão financeira.
- [ ] Acompanhamento de inadimplência.
- [ ] Recebimento de telemetria.
- [ ] Detecção automática de eventos.
- [ ] Geração automática de chamados.
- [ ] Gerenciamento de credenciais de acesso.
- [ ] Controle de autorizações de acesso.
- [ ] Sincronização com dispositivos físicos.
- [ ] Notificações multicanal.
- [ ] Dashboards administrativos.
- [ ] Recursos avançados de gestão.

### Segurança

- [ ] Autenticação baseada em JWT.
- [ ] Autorização baseada em papéis.
- [ ] Isolamento de dados entre condomínios.
- [ ] Registro de operações relevantes.
- [ ] Controle de integridade dos dados.
- [ ] Controle de concorrência.
- [ ] Proteção de informações sensíveis.
- [ ] Adequação aos requisitos aplicáveis da LGPD.

> Funcionalidades futuras serão adicionadas progressivamente após a validação e implementação do núcleo do sistema.

## Perfis de acesso

O sistema prevê diferentes perfis de acesso, cada um com responsabilidades específicas.

| Perfil | Responsabilidade |
|---|---|
| **Morador** | Realizar reservas, consultar informações e acompanhar solicitações. |
| **Funcionário** | Atender chamados e acompanhar operações e infraestrutura. |
| **Administrador** | Gerenciar usuários, recursos e operações do condomínio. |
| **Master** | Administrar a plataforma e os condomínios cadastrados. |
| **Sistema** | Executar rotinas e processamentos automáticos. |
| **Dispositivo IoT** | Comunicar eventos, telemetria e operações relacionadas ao controle de acesso. |

## Tecnologias

As tecnologias atualmente definidas ou previstas para o projeto são:

| Tecnologia | Finalidade |
|---|---|
| PostgreSQL | Banco de dados relacional |
| Entity Framework Core | Persistência e acesso aos dados |
| JWT | Autenticação e controle de identidade |
| RBAC | Controle de acesso baseado em papéis |
| MQTT | Comunicação com dispositivos IoT |
| Interface Web | Aplicação inicial para interação com o sistema |

As tecnologias específicas do backend, frontend, comunicação entre componentes e implantação serão consolidadas na documentação de arquitetura antes da implementação.

## Arquitetura

A solução inicial será composta por uma aplicação web integrada a um backend e a um banco de dados relacional.

A arquitetura deverá permitir a evolução progressiva do sistema sem acoplar o núcleo do MVP às integrações futuras.

### Visão geral

```text
                         ┌──────────────────┐
                         │    Interface     │
                         │       Web        │
                         └────────┬─────────┘
                                  │
                                  │ HTTP / API
                                  ▼
                         ┌──────────────────┐
                         │     Backend      │
                         │                  │
                         │ Regras de negócio│
                         │ Autenticação     │
                         │ Autorização      │
                         └────────┬─────────┘
                                  │
                                  │ EF Core
                                  ▼
                         ┌──────────────────┐
                         │    PostgreSQL    │
                         │                  │
                         │ Dados isolados   │
                         │ por condomínio   │
                         └──────────────────┘


              Integrações futuras
                         │
             ┌───────────┴───────────┐
             ▼                       ▼
        ┌─────────┐             ┌─────────────┐
        │  MQTT   │             │ Notificações│
        │   IoT   │             │   externas  │
        └─────────┘             └─────────────┘
```

### Princípios arquiteturais

A arquitetura deverá considerar:

- Separação de responsabilidades entre os módulos.
- APIs para comunicação entre aplicações e serviços.
- Isolamento de dados por condomínio.
- Processamento assíncrono para tarefas apropriadas.
- Tratamento de falhas em integrações externas.
- Estratégias de retentativa quando necessário.
- Monitoramento e registro de eventos relevantes.
- Segurança desde a concepção da solução.

Para as decisões técnicas detalhadas, consulte a documentação de arquitetura.

## Estrutura do repositório

A organização atual do repositório concentra a documentação do projeto:

```text
.
├── docs/
│   ├── 01-Project/
│   ├── 02-Requirements/
│   ├── 03-UseCases/
│   ├── 04-DataModel/
│   ├── 05-Architecture/
│   ├── 06-SecurityPrivacy/
│   ├── 07-Testing/
│   ├── 08-Development/
│   ├── 09-Operations/
│   ├── 10-UserGuides/
│   └── 11-UIUX/
│
└── README.md
```

> A estrutura representa a organização documental atual. A estrutura de código será adicionada conforme a implementação do sistema for iniciada.

## Documentação

A documentação técnica e funcional do projeto está concentrada em [`docs/`](docs/).

| Diretório | Finalidade |
|---|---|
| `01-Project/` | Definição, escopo, glossário e decisões do projeto. |
| `02-Requirements/` | Requisitos funcionais, não funcionais e regras de negócio. |
| `03-UseCases/` | Inventário, especificações e diagramas de casos de uso. |
| `04-DataModel/` | DER, modelo de dados, dicionário e restrições. |
| `05-Architecture/` | Arquitetura, APIs e integrações. |
| `06-SecurityPrivacy/` | Segurança, privacidade e riscos. |
| `07-Testing/` | Estratégia, plano e casos de teste. |
| `08-Development/` | MVP, backlog e padrões de desenvolvimento. |
| `09-Operations/` | Implantação, monitoramento e recuperação. |
| `10-UserGuides/` | Guias de instalação e utilização. |
| `11-UIUX/` | Design system e inventário de telas. |

### Documentos principais

A arquitetura geral do sistema está documentada em:

[`docs/05-Architecture/ArchitectureOverview.md`](docs/05-Architecture/ArchitectureOverview.md)

Este documento é a referência principal para as decisões arquiteturais do projeto.

## Como executar

**A execução ainda não está disponível nesta fase do projeto.**

O contrato de instalação e execução está documentado em [`docs/10-UserGuides/Installation.md`](docs/10-UserGuides/Installation.md) e será atualizado com comandos reais conforme os componentes forem implementados.

A execução deverá contemplar:

- Pré-requisitos e versões das ferramentas.
- Configuração das variáveis de ambiente.
- Configuração de secrets.
- Inicialização do PostgreSQL.
- Aplicação das migrações do banco de dados.
- Configuração dos serviços necessários.
- Execução do backend.
- Execução da aplicação web.
- Execução dos testes automatizados.

> Os comandos somente serão adicionados a este README após serem efetivamente implementados e validados.

## Configuração

A aplicação utilizará configurações específicas para cada ambiente de execução.

As informações sensíveis, como credenciais de banco de dados, chaves de assinatura e secrets de autenticação, não deverão ser armazenadas diretamente no repositório.

A configuração definitiva será documentada juntamente com o processo de instalação.

## Testes

A estratégia de testes está sendo definida em [`docs/07-Testing/`](docs/07-Testing/).

O projeto deverá contemplar, conforme aplicável:

- Testes unitários.
- Testes de integração.
- Testes de API.
- Testes de interface.
- Testes de segurança.
- Testes de concorrência.
- Testes de isolamento entre condomínios.
- Testes dos fluxos de IoT.

Os comandos para execução dos testes serão adicionados quando a infraestrutura de testes estiver implementada.

## Roadmap

### Fase 1 — Definição

- [x] Definir escopo inicial do projeto.
- [x] Definir escopo do MVP.
- [x] Levantar requisitos.
- [x] Definir atores e perfis de acesso.
- [x] Estruturar documentação inicial.
- [ ] Consolidar decisões arquiteturais.

### Fase 2 — MVP

- [ ] Implementar autenticação.
- [ ] Implementar autorização.
- [ ] Implementar gerenciamento de usuários.
- [ ] Implementar gerenciamento de unidades.
- [ ] Implementar espaços comuns.
- [ ] Implementar reservas.
- [ ] Implementar chamados.
- [ ] Implementar histórico e auditoria.
- [ ] Implementar isolamento entre condomínios.
- [ ] Implementar testes automatizados.

### Fase 3 — Expansão

- [ ] Gestão financeira.
- [ ] Gestão de inadimplência.
- [ ] Notificações.
- [ ] Dashboards administrativos.
- [ ] Recursos avançados da plataforma.

### Fase 4 — IoT

- [ ] Comunicação MQTT.
- [ ] Recebimento de telemetria.
- [ ] Processamento de eventos.
- [ ] Geração automática de chamados.
- [ ] Gerenciamento de credenciais.
- [ ] Sincronização com dispositivos físicos.
- [ ] Integração com controle de acesso.

> O roadmap representa a direção planejada do projeto e poderá ser ajustado conforme os requisitos e decisões técnicas evoluírem.

## Desenvolvimento e contribuição

O projeto é desenvolvido de forma colaborativa.

As contribuições devem:

1. Respeitar os requisitos e as decisões arquiteturais documentadas.
2. Preservar o isolamento de dados entre condomínios.
3. Incluir testes compatíveis com as alterações realizadas.
4. Atualizar a documentação quando houver mudanças relevantes de comportamento ou arquitetura.
5. Ser revisadas antes da integração à branch principal.

Os padrões de código, estratégia de branches, convenções de commits e processo de revisão serão definidos em [`docs/08-Development/`](docs/08-Development/).

## Versionamento

O projeto utiliza **Git** para controle de versão e **GitHub** para hospedagem do código, documentação, acompanhamento das alterações e colaboração entre os integrantes.

A estratégia de branches e as convenções de commits serão definidas na documentação de desenvolvimento.

## Licença

A licença do projeto ainda será definida pelos responsáveis pelo repositório.

A distribuição, modificação e reutilização do código deverão respeitar os termos da licença escolhida.

---

<p align="center">
  Desenvolvido como projeto de engenharia de software para gestão condominial.
</p>