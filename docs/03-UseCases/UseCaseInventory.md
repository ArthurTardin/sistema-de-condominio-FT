# Inventário de Casos de Uso

## 1. Objetivo

Este documento centraliza os casos de uso do Sistema de Condomínio, identificando seus atores, códigos, descrições resumidas e requisitos funcionais relacionados.

O inventário serve como referência para a elaboração de diagramas de casos de uso, diagramas de atividade, diagramas de sequência e especificações detalhadas dos fluxos.

## 2. Convenções

### 2.1. Identificação dos casos de uso

| Prefixo | Ator principal |
|---|---|
| UC-MOR | Morador |
| UC-FUN | Funcionário |
| UC-ADM | Administrador do condomínio |
| UC-MAS | Master da plataforma |
| UC-SIS | Sistema |
| UC-IOT | Dispositivo IoT |

Cada caso de uso possui um identificador único. Os códigos devem permanecer estáveis mesmo quando o nome ou a descrição do caso for alterado.

### 2.2. Rastreabilidade

Os requisitos funcionais são identificados pelos códigos definidos em `Docs/02-Requirements/FunctionalRequirements.md`.

A indicação de um requisito significa que ele fundamenta ou está relacionado ao comportamento do caso de uso. Ela não substitui a leitura do requisito completo nem define, isoladamente, todos os critérios de aceitação.

Quando a correspondência entre um caso e um requisito ainda não estiver confirmada, a relação será identificada como pendente de validação.

### 2.3. Tipos de ator

- **Ator humano:** usuário que interage diretamente com a plataforma.
- **Ator automatizado:** processo lógico executado pelo sistema.
- **Ator externo:** dispositivo ou componente externo que troca informações com a plataforma.

O Sistema e os dispositivos IoT são apresentados separadamente dos usuários humanos porque participam de fluxos automatizados e integrações.

## 3. Morador

| ID | Caso de uso | Requisitos relacionados |
|---|---|---|
| UC-MOR-001 | Autenticar-se | RF02 |
| UC-MOR-002 | Realizar reservas | RF05 |
| UC-MOR-003 | Cancelar reservas | RF06 |
| UC-MOR-004 | Consultar reservas | RF05, RF06 |
| UC-MOR-005 | Consultar disponibilidade de espaços | RF05 |
| UC-MOR-006 | Registrar ocorrência | RF10 |
| UC-MOR-007 | Abrir chamado | RF11 |
| UC-MOR-008 | Consultar chamados | RF11, RF13 |
| UC-MOR-009 | Consultar histórico de chamados | RF14 |
| UC-MOR-010 | Consultar cobranças | RF16 |
| UC-MOR-011 | Gerenciar dados da conta | RF02, RF15 |
| UC-MOR-012 | Consultar notificações | RF22 |

## 4. Funcionário

| ID | Caso de uso | Requisitos relacionados |
|---|---|---|
| UC-FUN-001 | Autenticar-se | RF02 |
| UC-FUN-002 | Alterar estado operacional de ativo | RF07, RF19 |
| UC-FUN-003 | Acompanhar SLA de chamado | RF13 e requisito de monitoramento de SLA a confirmar |
| UC-FUN-004 | Gerenciar chamado | RF13 |
| UC-FUN-005 | Consultar histórico de chamado | RF14 |
| UC-FUN-006 | Consultar chamados | RF13 |
| UC-FUN-007 | Consultar ativos | RF19 |
| UC-FUN-008 | Consultar notificações | RF22 |
| UC-FUN-009 | Consultar telemetria | RF-IoT02 |

## 5. Administrador do condomínio

| ID | Caso de uso | Requisitos relacionados |
|---|---|---|
| UC-ADM-001 | Autenticar-se | RF02 |
| UC-ADM-002 | Realizar reservas | RF05 |
| UC-ADM-003 | Cancelar reservas | RF06 |
| UC-ADM-004 | Alterar estado operacional de ativo | RF07, RF19 |
| UC-ADM-005 | Visualizar dashboard administrativo | RF08 |
| UC-ADM-006 | Registrar ocorrência | RF10 |
| UC-ADM-007 | Abrir chamado | RF11 |
| UC-ADM-008 | Atribuir chamado | RF12 |
| UC-ADM-009 | Gerenciar chamado | RF13 |
| UC-ADM-010 | Consultar histórico de chamado | RF14 |
| UC-ADM-011 | Consultar reservas | RF05, RF06 |
| UC-ADM-012 | Consultar chamados | RF13 |
| UC-ADM-013 | Consultar ativos | RF19 |
| UC-ADM-014 | Consultar telemetria | RF-IoT02 |
| UC-ADM-015 | Consultar ocorrências | RF10 |
| UC-ADM-016 | Consultar cobranças | RF16 |
| UC-ADM-017 | Gerenciar usuários | RF15 |
| UC-ADM-018 | Consultar usuários | RF15 |
| UC-ADM-019 | Gerenciar unidades | RF17 |
| UC-ADM-020 | Consultar unidades | RF17 |
| UC-ADM-021 | Gerenciar espaços comuns | RF18 |
| UC-ADM-022 | Consultar espaços comuns | RF18 |
| UC-ADM-023 | Gerenciar cobranças | RF16 |
| UC-ADM-024 | Gerenciar dispositivos IoT | RF20 |
| UC-ADM-025 | Consultar infraestrutura | RF19, RF20, RF-IoT02 |
| UC-ADM-026 | Gerenciar infraestrutura | RF19 |
| UC-ADM-027 | Gerenciar configurações do condomínio | RF de configuração do tenant a confirmar |
| UC-ADM-028 | Consultar notificações | RF22 |
| UC-ADM-029 | Gerenciar notificações | RF22 |
| UC-ADM-030 | Consultar registros de acesso | RF24 |
| UC-ADM-031 | Gerenciar credenciais de acesso | RF23 |
| UC-ADM-032 | Gerenciar ocorrências | RF10 |
| UC-ADM-033 | Gerenciar autorizações de acesso | RF23, RF-IoT01 |

## 6. Master da plataforma

O ator Master atua no escopo global da plataforma. Suas permissões não devem ser confundidas com as do Administrador de um condomínio.

| ID | Caso de uso | Requisitos relacionados |
|---|---|---|
| UC-MAS-001 | Autenticar-se | RF02 |
| UC-MAS-002 | Gerenciar condomínios | RF26-RF33, conforme a operação |
| UC-MAS-003 | Gerenciar usuários Master | RF de gestão global de usuários a confirmar |
| UC-MAS-004 | Visualizar dashboard da plataforma | RF de dashboard global a confirmar |
| UC-MAS-005 | Consultar logs e auditoria da plataforma | RF25 e requisitos de administração global |
| UC-MAS-006 | Consultar saúde operacional da plataforma | RF de monitoramento global a confirmar |
| UC-MAS-007 | Gerenciar planos e limites dos condomínios | RF de planos e limites a confirmar |
| UC-MAS-008 | Consultar consumo de recursos | RF de monitoramento e consumo a confirmar |
| UC-MAS-009 | Gerenciar configurações globais da plataforma | RF de configuração global a confirmar |
| UC-MAS-010 | Gerenciar solicitações de suporte da plataforma | RF-SUP01-RF-SUP09 |
| UC-MAS-011 | Acessar dados de condomínio para suporte | RF-SUP01-RF-SUP09 |
| UC-MAS-012 | Gerenciar integrações globais da plataforma | RF de integrações globais a confirmar |
| UC-MAS-013 | Consultar histórico de alterações dos condomínios | RF25 e requisitos de administração global |
| UC-MAS-014 | Gerenciar versões e atualizações da plataforma | RF de manutenção e atualização a confirmar |
| UC-MAS-015 | Gerenciar manutenção da plataforma | RF de manutenção global a confirmar |
| UC-MAS-016 | Gerenciar permissões de usuários Master | RF de autorização global a confirmar |
| UC-MAS-017 | Gerenciar cobrança dos condomínios | RF de cobrança da plataforma a confirmar |
| UC-MAS-018 | Consultar notificações da plataforma | RF de notificações globais a confirmar |
| UC-MAS-019 | Gerenciar notificações da plataforma | RF de notificações globais a confirmar |

## 7. Sistema

Os casos de uso desta seção representam comportamentos automáticos do sistema. Alguns são acionados por eventos, outros por agendamentos e outros como parte de um fluxo iniciado por um usuário.

| ID | Caso de uso | Requisitos relacionados |
|---|---|---|
| UC-SIS-001 | Emitir token de autenticação | RF02 |
| UC-SIS-002 | Inativar entidade | RF03 |
| UC-SIS-003 | Controlar ciclo de vida do chamado | RF04 |
| UC-SIS-004 | Validar reserva | RF05 |
| UC-SIS-005 | Processar cancelamento de reserva | RF06 |
| UC-SIS-006 | Consolidar indicadores administrativos | RF08 |
| UC-SIS-007 | Gerar autorização de acesso | RF-IoT01 |
| UC-SIS-008 | Gerar chamado automaticamente | RF-IoT03 |
| UC-SIS-009 | Processar expiração de reservas e autorizações de acesso | Requisitos de automação de expiração a confirmar |
| UC-SIS-010 | Enviar notificações automaticamente | RF22 e requisitos de automação de notificações |
| UC-SIS-011 | Processar vencimento de cobranças | RF16 e requisitos de automação financeira |
| UC-SIS-012 | Detectar falhas de dispositivos IoT | RF20 e requisitos de monitoramento IoT a confirmar |
| UC-SIS-013 | Executar políticas de retenção de dados | RNF10 e requisitos de retenção a confirmar |
| UC-SIS-014 | Registrar eventos de auditoria | RF25 |
| UC-SIS-015 | Reprocessar operações com falha | Requisitos de retentativa e idempotência a confirmar |
| UC-SIS-016 | Monitorar SLA dos chamados | RF13 e requisitos de monitoramento de SLA a confirmar |
| UC-SIS-017 | Atualizar estado operacional automaticamente | RF19 e requisitos de automação de infraestrutura a confirmar |
| UC-SIS-018 | Executar backups automáticos | RNF11 e requisitos operacionais a confirmar |
| UC-SIS-019 | Detectar indisponibilidade de serviços | Requisitos de monitoramento operacional a confirmar |
| UC-SIS-020 | Gerenciar expiração e revogação de sessões | RF02 e requisitos de gestão de sessões a confirmar |
| UC-SIS-021 | Limpar dados temporários expirados | Requisitos de limpeza de dados temporários a confirmar |

## 8. Dispositivo IoT

O dispositivo IoT é um ator externo que troca mensagens com o backend ou com os componentes de controle de acesso.

| ID | Caso de uso | Requisitos relacionados |
|---|---|---|
| UC-IOT-001 | Sincronizar autorização de acesso | RF-IoT01 |
| UC-IOT-002 | Enviar telemetria | RF-IoT02 |
| UC-IOT-003 | Executar controle de acesso | RF-IoT01 e RF23 |
| UC-IOT-004 | Enviar eventos de acesso | RF24 |

## 9. Regras de modelagem dos casos de uso

### 9.1. Autenticação e autorização

A autenticação é comum aos atores humanos, mas as permissões variam conforme o papel e o tenant. Os casos de uso protegidos devem especificar as permissões exigidas e o tratamento de sessões expiradas ou revogadas.

### 9.2. Reservas

Os casos de reserva devem considerar disponibilidade, regras de antecedência e duração, elegibilidade do usuário, inadimplência quando aplicável, concorrência e cancelamento. As autorizações de acesso decorrentes de reservas devem seguir as regras específicas de acesso físico.

### 9.3. Chamados e histórico

A abertura, atribuição, gerenciamento e consulta de chamados devem respeitar as transições permitidas. Alterações de estado devem preservar o histórico de forma rastreável.

### 9.4. IoT e acesso físico

A sincronização entre backend e dispositivo deve considerar falhas de comunicação, validade da autorização, revogação e operação offline quando suportada. Eventos de acesso e telemetria devem preservar sua origem e associação com o tenant.

### 9.5. Administração global e suporte

As operações de Master devem possuir escopo global explicitamente autorizado. O acesso de suporte aos dados de um condomínio deve obedecer às regras de autorização, justificativa, auditoria e prazo aplicáveis.

### 9.6. Processos automáticos

Os casos de uso do Sistema devem identificar o gatilho de execução, a periodicidade quando houver, as condições de processamento, o tratamento de falhas e a estratégia de idempotência.