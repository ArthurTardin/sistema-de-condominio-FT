# Sistema de Gestão Condominial

Plataforma de gestão condominial que busca centralizar operações administrativas e serviços aos moradores em um único sistema. A primeira versão será focada no núcleo operacional de autenticação, cadastros, reservas e chamados.

<!-- Substituir os marcadores abaixo por badges reais quando os dados estiverem definidos. -->

## Status do projeto

**Fase atual:** Definição e implementação planejada do MVP.

O MVP foi recortado para validar o núcleo operacional do produto antes da inclusão de módulos financeiros, integrações IoT, controle físico de acesso e recursos avançados de plataforma.

> **Atenção:** as funcionalidades descritas neste README representam o escopo planejado e não significam que já estejam implementadas.

## Sumário

- [Sobre o projeto](#sobre-o-projeto)
- [Funcionalidades](#funcionalidades)
- [Perfis de acesso](#perfis-de-acesso)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Estrutura do repositório](#estrutura-do-repositório)
- [Documentação](#documentação)
- [Como executar](#como-executar)
- [Testes](#testes)
- [Desenvolvimento e contribuição](#desenvolvimento-e-contribuição)
- [Licença](#licença)

## Sobre o projeto

A gestão de condomínios envolve reservas de espaços comuns, manutenção, ocorrências, controle de acesso, cobranças e monitoramento da infraestrutura. Quando essas atividades são realizadas por ferramentas desconectadas e processos manuais, podem ocorrer atrasos, retrabalho, falhas de comunicação e dificuldade de rastrear operações.

O Sistema de Gestão Condominial pretende integrar esses processos em uma plataforma que conecte moradores, funcionários, administradores e dispositivos inteligentes.

### Objetivos

- Centralizar informações e operações condominiais.
- Simplificar reservas e solicitações de manutenção.
- Melhorar a rastreabilidade de ocorrências e operações.
- Integrar o controle de acesso à gestão das reservas.
- Monitorar a infraestrutura por meio de dispositivos IoT.
- Disponibilizar indicadores para a administração.
- Garantir isolamento de dados entre condomínios.

## Funcionalidades

### Núcleo do MVP
- Gerenciamento de usuários, unidades e espaços comuns.
- Consulta de disponibilidade, reservas e cancelamentos.
- Registro e acompanhamento de ocorrências e chamados.
- Histórico e auditoria das operações relevantes.
- Isolamento de dados entre condomínios.

### Evoluções planejadas
- Gestão financeira e acompanhamento de inadimplência.
- Recebimento e consulta de telemetria.
- Detecção de eventos e geração automática de chamados.
- Gerenciamento de credenciais e autorizações de acesso.
- Sincronização de autorizações com dispositivos físicos.
- Notificações multicanal, dashboards e recursos avançados de plataforma.

### Segurança
- Autenticação e autorização por perfil.
- Isolamento de dados entre condomínios.
- Histórico de operações relevantes.
- Controles de integridade e concorrência.
- Proteção de dados e consideração dos requisitos da LGPD.

## Perfis de acesso

| Perfil | Responsabilidade |
|---|---|
| Morador | Realizar reservas, consultar informações e acompanhar solicitações. |
| Funcionário | Atender chamados e acompanhar operações e infraestrutura. |
| Administrador | Gerenciar usuários, recursos e operações do condomínio. |
| Master | Administrar a plataforma e os condomínios cadastrados. |
| Sistema | Executar rotinas e processamentos automáticos. |
| Dispositivo IoT | Comunicar eventos, telemetria e operações de acesso. |

## Tecnologias

Tecnologias previstas ou definidas na documentação atual:

| Componente | Tecnologia |
|---|---|
| Banco de dados relacional | PostgreSQL |
| Persistência no backend | Entity Framework Core |
| Autenticação | JWT e controle de acesso por papéis |
| Comunicação com dispositivos IoT | MQTT |
| Aplicação inicial | Interface web |

As tecnologias específicas de frontend, backend, comunicação entre componentes e implantação serão consolidadas na documentação de arquitetura antes da implementação.

## Arquitetura

A solução inicial prevê uma interface web integrada a um backend e a um banco de dados relacional. Integrações IoT e clientes adicionais serão avaliados após a validação do MVP.

A arquitetura deverá contemplar:

- Separação de responsabilidades entre os módulos.
- APIs para comunicação entre aplicações e serviços.
- Isolamento de dados por condomínio.
- Processamento assíncrono para tarefas como notificações e sincronização de dispositivos.
- Tratamento de falhas e retentativas em integrações.
- Monitoramento e registro de eventos relevantes.

Consulte a documentação de arquitetura para conhecer as decisões técnicas detalhadas.

## Estrutura do repositório

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
└── README.md
```

A estrutura representa a organização documental do projeto. Alguns documentos ainda estão em fase de especificação e serão complementados durante a implementação.

## Documentação

A documentação do projeto está concentrada em [`docs/`](docs/).

| Documento | Finalidade |
|---|---|
| `01-Project/` | Definição, escopo, glossário e decisões. |
| `02-Requirements/` | Requisitos funcionais, não funcionais e regras de negócio. |
| `03-UseCases/` | Inventário, especificações e diagramas de casos de uso. |
| `04-DataModel/` | DER, dicionário de dados e restrições. |
| `05-Architecture/` | Arquitetura, APIs e integrações. |
| `06-SecurityPrivacy/` | Segurança, privacidade e riscos. |
| `07-Testing/` | Estratégia e casos de teste. |
| `08-Development/` | MVP, backlog e padrões de desenvolvimento. |
| `09-Operations/` | Implantação, monitoramento e recuperação. |
| `10-UserGuides/` | Guias de instalação e utilização. |
| `11-UIUX/` | Design system e inventário de telas. |

## Como executar

**A execução ainda não está disponível nesta fase do projeto.**

Antes de publicar instruções de execução, serão definidos e implementados os componentes necessários, incluindo:

- Pré-requisitos de ambiente e versões das ferramentas.
- Configuração das variáveis de ambiente e segredos.
- Inicialização do PostgreSQL.
- Aplicação das migrações do banco de dados.
- Configuração dos serviços necessários para integrações.
- Comandos para executar o backend e as aplicações.
- Procedimentos para executar os testes.

Quando esses componentes estiverem disponíveis, esta seção conterá comandos reais, exemplos de configuração e instruções para executar o sistema localmente.

## Testes

A estratégia de testes será documentada em `docs/07-Testing/`.

Ela deverá contemplar testes unitários, de integração, de API, de interface, de segurança, de concorrência e dos fluxos de IoT, conforme aplicável.

Os comandos de execução dos testes serão adicionados quando a estrutura de testes estiver implementada.

## Desenvolvimento e contribuição

O projeto é desenvolvido de forma colaborativa.

As contribuições deverão:

1. Respeitar os requisitos e as decisões arquiteturais documentadas.
2. Manter o isolamento de dados entre condomínios.
3. Incluir testes compatíveis com as alterações realizadas.
4. Atualizar a documentação quando houver mudanças de comportamento ou arquitetura.
5. Ser revisadas antes de serem integradas à branch principal.

Os padrões de código, a estratégia de branches e o processo de revisão serão detalhados em `docs/08-Development/`.

## Licença

A licença do projeto ainda será definida pelos responsáveis pelo repositório.