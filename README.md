# Sistema de Gestão Condominial

Plataforma de gestão condominial que busca centralizar operações administrativas, serviços aos moradores e monitoramento da infraestrutura em um único sistema, com aplicações web e desktop e integração com dispositivos IoT.

<!-- Substituir os marcadores abaixo por badges reais quando os dados estiverem definidos. -->

## Status do projeto

**Fase atual:** Documentação e planejamento técnico.

O projeto está em desenvolvimento documental. Os requisitos, o modelo de dados, a arquitetura e os casos de uso estão sendo definidos e consolidados antes da implementação.

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

### Gestão condominial
- Gerenciamento de usuários, unidades e espaços comuns.
- Consulta de disponibilidade, reservas e cancelamentos.
- Registro e acompanhamento de ocorrências e chamados.
- Gestão de cobranças e acompanhamento financeiro.
- Notificações e histórico de operações.

### Infraestrutura e IoT
- Cadastro e gerenciamento de dispositivos.
- Recebimento e consulta de telemetria.
- Detecção de eventos e geração automática de chamados.
- Gerenciamento de credenciais e autorizações de acesso.
- Sincronização de autorizações com dispositivos físicos.

### Administração da plataforma
- Gerenciamento dos condomínios cadastrados.
- Gerenciamento de planos e limites de utilização.
- Administração de usuários e permissões da plataforma.
- Auditoria, monitoramento e gerenciamento de integrações.

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
| Aplicações | Web e desktop |

As tecnologias específicas de frontend, backend, comunicação entre componentes e implantação devem ser confirmadas na documentação de arquitetura antes de serem tratadas como definitivas.

## Arquitetura

A solução prevê aplicações web e desktop integradas a um backend, banco de dados relacional e serviços de comunicação com dispositivos IoT.

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
├── Docs/
│   ├── 01-Project/
│   ├── 02-Requirements/
│   ├── 03-UseCases/
│   ├── 04-DataModel/
│   ├── 05-Architecture/
│   ├── 06-SecurityPrivacy/
│   ├── 07-Testing/
│   ├── 08-Development/
│   ├── 09-Operations/
│   └── 10-UserGuides/
└── README.md
```

A estrutura representa a organização documental planejada. Os diretórios serão preenchidos à medida que os respectivos documentos forem concluídos.

## Documentação

A documentação do projeto está concentrada em [`Docs/`](Docs/).

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

A estratégia de testes será documentada em `Docs/07-Testing/`.

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

Os padrões de código, a estratégia de branches e o processo de revisão serão detalhados em `Docs/08-Development/`.

## Licença

A licença do projeto ainda será definida pelos responsáveis pelo repositório.