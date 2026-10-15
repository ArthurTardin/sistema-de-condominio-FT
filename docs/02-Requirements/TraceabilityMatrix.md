# Matriz de Rastreabilidade

## 1. Objetivo

Este documento estabelece a relação entre os requisitos funcionais, os requisitos não funcionais, as regras de negócio, os casos de uso, o modelo de dados e os testes do Sistema de Condomínio.

A matriz de rastreabilidade tem como objetivos:

- Verificar se os requisitos estão relacionados às regras de negócio correspondentes.
- Identificar requisitos que ainda não possuem casos de uso detalhados.
- Orientar a elaboração de testes funcionais, de integração, de segurança e de desempenho.
- Identificar impactos decorrentes de alterações nos requisitos.
- Acompanhar a cobertura da especificação e da implementação.
- Evitar funcionalidades implementadas sem requisitos ou regras documentadas.

## 2. Convenções

Os seguintes identificadores são utilizados na documentação:

| Prefixo | Documento ou elemento |
|---|---|
| RF | Requisito funcional |
| RNF | Requisito não funcional |
| RN | Regra de negócio |
| UC | Caso de uso |
| CT | Caso de teste |
| ENT | Entidade ou elemento do modelo de dados |
| DEC | Decisão arquitetural registrada |

Os identificadores `UC`, `CT` e `ENT` devem corresponder aos identificadores efetivamente utilizados nos respectivos documentos. Quando ainda não existir um identificador formal, a relação será indicada como pendente de vinculação.

## 3. Critérios de rastreabilidade

Cada requisito deve possuir, conforme sua natureza, relações identificáveis com:

1. Uma ou mais regras de negócio, quando houver comportamento de domínio associado.
2. Um ou mais casos de uso, quando representar uma funcionalidade acionada por um ator ou processo.
3. Uma ou mais entidades, quando depender diretamente da persistência ou manipulação de dados.
4. Um ou mais testes que permitam verificar seu cumprimento.

Requisitos de arquitetura, segurança, desempenho e operação podem exigir testes transversais, sem necessariamente corresponder a um único caso de uso.

A ausência de uma relação aplicável deve ser justificada. A ausência de uma relação necessária deve ser tratada como uma pendência de documentação.

## 4. Matriz de rastreabilidade funcional

A tabela a seguir relaciona os grupos de requisitos funcionais às regras de negócio e aos elementos que precisam ser especificados ou verificados.

As referências às regras de negócio são indicativas de rastreabilidade inicial. A associação definitiva deve ser confirmada durante a revisão conjunta dos requisitos e das regras.

| Requisitos funcionais | Domínio | Regras de negócio relacionadas | Dados e entidades relevantes | Validação necessária |
|---|---|---|---|---|
| RF01, RF01.1 | Multi-tenancy e encerramento de tenant | RN01, RN02, RN03 | `Tenant`, usuários e entidades operacionais vinculadas | Isolamento entre tenants, encerramento, bloqueio de operações e reativação controlada |
| RF02 | Autenticação e autorização | RN04, RN06, RN49 | `Usuario`, sessões e credenciais de autenticação | Autenticação, permissões, expiração e revogação |
| RF03 | Exclusão lógica e preservação histórica | RN02, RN06, RN21, RN43 | Entidades com exclusão lógica e registros históricos | Preservação dos dados e bloqueio de operações incompatíveis |
| RF04 | Estados e transições de chamados | RN19, RN21, RN22, RN23 | `Chamado`, `HistoricoChamado` | Transições permitidas, rejeição de transições inválidas e histórico |
| RF05, RF06 | Criação, disponibilidade e cancelamento de reservas | RN07, RN08, RN09, RN10, RN12, RN13 | `Reserva`, `Espaco`, `Unidade`, `Usuario` | Disponibilidade, regras de elegibilidade, cancelamento e concorrência |
| RF07 | Estado operacional de ativos ou espaços | RN11, RN28 | `Ativo`, `Espaco`, `Reserva` | Bloqueio de novas operações e tratamento de reservas existentes |
| RF08 | Dashboard administrativo | RN03, RN14, RN21, RN24, RN32, RN42 | Reservas, chamados, cobranças, ativos, telemetria e auditoria | Consistência dos indicadores, autorização e desempenho |
| RF09 | Clientes e interfaces da plataforma | RN52, RN54 | Dados e serviços consumidos pelos clientes | Consistência das regras entre Web, Mobile e Desktop |
| RF10 | Registro de ocorrências | RN25, RN27 | `Ocorrencia`, `Usuario`, `Unidade` | Autorização, associação correta e preservação do registro |
| RF11 | Abertura de chamados | RN18, RN19, RN21 | `Chamado`, `Usuario`, `Unidade`, `Ativo` | Validação dos campos, vínculo com tenant e estado inicial |
| RF12 | Atribuição de chamados | RN20, RN21 | `Chamado`, `Usuario`, `HistoricoChamado` | Atribuição autorizada e rastreabilidade dos responsáveis |
| RF13 e sub-requisitos | Gerenciamento de chamados | RN19, RN21, RN22, RN23, RN24 | `Chamado`, `HistoricoChamado` | Transições, resolução, reabertura, cancelamento e SLA |
| RF14 | Consulta de histórico | RN13, RN21, RN26, RN37, RN43 | Reservas, chamados, ocorrências e registros históricos | Integridade histórica, autorização e isolamento entre tenants |
| RF15 | Gestão de usuários e contas | RN04, RN05, RN06 | `Usuario`, vínculos e sessões | Permissões, vínculos ativos, desativação e histórico |
| RF16 | Cobranças e inadimplência | RN14, RN15, RN16, RN17 | `CobrancaCondominial`, `Unidade`, `Usuario` | Cálculo de inadimplência, regularização e bloqueios aplicáveis |
| RF17 | Gestão de unidades e vínculos | RN05, RN17 | `Unidade`, `Usuario`, `UsuarioUnidade`, `Tenant` | Unicidade, vínculos ativos e isolamento entre tenants |
| RF18 | Gestão de espaços comuns | RN07, RN08, RN09, RN11, RN28 | `Espaco`, `Reserva`, `Tenant` | Disponibilidade, estado operacional e regras de reserva |
| RF19 | Gestão de ativos de infraestrutura | RN28, RN32 | `Ativo`, registros operacionais e histórico | Transições de estado e restrições operacionais |
| RF20 | Gestão de dispositivos IoT | RN29, RN32, RN33 | `DispositivoIoT`, `Ativo`, `Tenant` | Associação, identidade, estado e permissões do dispositivo |
| RF21 | Gestão de ocorrências | RN25, RN26, RN27 | `Ocorrencia`, `Chamado`, `Usuario` | Fluxo de tratamento, histórico e associação com chamados |
| RF22 | Notificações | RN39, RN40, RN41 | `Notificacao`, preferências e eventos de origem | Deduplicação, preferências, entrega e notificações obrigatórias |
| RF23 | Credenciais e autorizações de acesso | RN35, RN36, RN38 | `CredencialAcesso`, `AutorizacaoAcesso`, `Reserva`, `DispositivoIoT` | Emissão, validade, revogação e isolamento entre tenants |
| RF24 | Registros de acesso físico | RN37, RN43 | `RegistroAcesso`, dispositivos e autorizações | Rastreabilidade, proteção dos registros e permissões de consulta |
| RF25 | Auditoria | RN42, RN43, RN45 | Registros de auditoria e entidades afetadas | Registro das ações críticas, integridade e controle de acesso |
| RF26 a RF33 | Administração global da plataforma | RN03, RN42, RN44, RN45, RN51, RN55 | `Tenant`, usuários administrativos, configurações e registros de suporte | Permissões globais, escopo de suporte, auditoria e configurações |
| RF-IoT01 | Autorização de acesso por dispositivos | RN35, RN36, RN38 | `CredencialAcesso`, `AutorizacaoAcesso`, `DispositivoIoT` | Validade, revogação e sincronização |
| RF-IoT02 | Telemetria | RN29, RN30, RN32, RN33 | `RegistroTelemetria`, `DispositivoIoT`, `Ativo` | Validação da origem, idempotência, persistência e retenção |
| RF-IoT03 | Alarmes e chamados automáticos | RN18, RN30, RN31, RN32 | `Chamado`, `HistoricoChamado`, `RegistroTelemetria` | Geração automática, correlação, deduplicação e reprocessamento |
| RF-IoT04 | Reconhecimento facial futuro | RN34, RN37, RN38, RN43 | Credenciais biométricas e registros de acesso, se aprovados | Avaliação de privacidade, segurança, autorização e retenção |
| RF-SUP01 a RF-SUP09 | Suporte administrativo controlado | RN03, RN42, RN43, RN44, RN45 | Registros de suporte, autorizações e auditoria | Escopo, prazo, justificativa, revisão e revogação do acesso |
| RF34 a RF47 | Automações e funcionalidades complementares | RN39, RN41, RN46, RN47, RN48, RN49, RN50, RN51, RN55 | Sessões, tarefas, notificações, registros operacionais e configurações | Expiração, reprocessamento, monitoramento, backup e consistência |

**Observação:** a associação acima é uma primeira organização por domínio. Os identificadores dos requisitos funcionais devem ser conferidos com `FunctionalRequirements.md`, principalmente os sub-requisitos e os requisitos de automação, para que nenhuma relação seja considerada definitiva apenas pela proximidade numérica.

## 5. Matriz de rastreabilidade dos requisitos não funcionais

| Requisito | Aspecto verificável | Elementos relacionados | Estratégia de validação |
|---|---|---|---|
| RNF01 | P95 inferior a 200 ms para operações comuns da API | Endpoints, banco de dados e integrações | Testes de carga, medição de latência e análise de erros |
| RNF02 | Operações de entrada e saída não bloqueantes quando suportadas | Serviços, repositórios e integrações | Revisão técnica e testes sob carga |
| RNF03 | Cancelamento propagado e timeouts definidos | API, banco, mensageria e serviços externos | Testes de timeout, cancelamento e recuperação |
| RNF04 | Concorrência segura e prevenção de reservas sobrepostas | `Reserva` e PostgreSQL | Testes concorrentes e validação das restrições |
| RNF05 | Senhas e credenciais protegidas | Autenticação e armazenamento de credenciais | Revisão de configuração, testes de segurança e inspeção de logs |
| RNF06 | Autenticação MQTT e isolamento de dispositivos | Broker, dispositivos e backend | Testes de autorização, isolamento e mensagens inválidas |
| RNF07 | Operação offline com validade e sincronização controladas | Dispositivos de acesso e autorizações | Testes de desconexão, expiração, revogação e reconexão |
| RNF08 | Criptografia em trânsito | API, MQTT e integrações | Inspeção de configuração TLS e testes de conexão |
| RNF09 | Segredos externos ao código e separados por ambiente | Configuração e implantação | Revisão de repositórios, configuração e permissões |
| RNF10 | Retenção e tratamento de dados conforme a LGPD | Dados pessoais, telemetria, auditoria e biometria | Revisão de políticas, permissões e procedimentos de retenção |
| RNF11 | RTO e RPO definidos e recuperação testada | Banco de dados, backups e infraestrutura | Testes de restauração e registro dos tempos e perdas observados |

## 6. Rastreabilidade das regras de negócio

As regras de negócio estão agrupadas nos seguintes domínios:

| Intervalo de regras | Domínio | Requisitos funcionais relacionados |
|---|---|---|
| RN01 a RN03 | Multi-tenancy e administração global | RF01, RF01.1, RF26 a RF33 e RF-SUP01 a RF-SUP09 |
| RN04 a RN06 | Usuários, permissões e vínculos | RF02, RF03, RF15 e RF17 |
| RN07 a RN13 | Reservas e disponibilidade | RF05, RF06, RF07 e RF18 |
| RN14 a RN17 | Cobranças e inadimplência | RF16 e requisitos de reserva aplicáveis |
| RN18 a RN24 | Chamados e SLA | RF04, RF11, RF12, RF13 e RF14 |
| RN25 a RN27 | Ocorrências | RF10 e RF21 |
| RN28 a RN34 | Ativos, IoT e reconhecimento facial futuro | RF19, RF20, RF-IoT02, RF-IoT03 e RF-IoT04 |
| RN35 a RN38 | Credenciais e acesso físico | RF23, RF24 e RF-IoT01 |
| RN39 a RN41 | Notificações | RF22 e requisitos de automação relacionados |
| RN42 a RN45 | Auditoria e suporte | RF25, RF26 a RF33 e RF-SUP01 a RF-SUP09 |
| RN46 a RN51 | Automações e operação | RF34 a RF47 e RNF11 |
| RN52 a RN55 | Consistência entre clientes, falhas e configurações | RF09 e requisitos transversais |

## 7. Rastreabilidade do Modelo de Dados

O modelo de dados deve ser relacionado aos requisitos e às regras que justificam a existência e as restrições de cada entidade.

| Entidade ou grupo | Requisitos relacionados | Regras relacionadas | Restrições relevantes |
|---|---|---|---|
| `Tenant` | RF01, RF01.1, RF26 a RF33 | RN01, RN02, RN03 | CNPJ único, encerramento lógico e isolamento |
| `Usuario` | RF02, RF15 | RN04, RN05, RN06 | E-mail único, vínculo com tenant e permissões |
| `Unidade` e `UsuarioUnidade` | RF15, RF17 | RN05, RN17 | Unicidade da unidade e dos vínculos ativos |
| `Espaco` e `Reserva` | RF05, RF06, RF07, RF18 | RN07 a RN13 | Intervalos válidos e prevenção de sobreposição |
| `Chamado` e `HistoricoChamado` | RF04, RF11 a RF14 | RN18 a RN24 | Integridade das transições e preservação histórica |
| `Ocorrencia` | RF10, RF21 | RN25 a RN27 | Associação com tenant e histórico de tratamento |
| `CobrancaCondominial` | RF16 | RN14 a RN17 | Unicidade por unidade e competência, valores válidos |
| Ativos e dispositivos IoT | RF19, RF20, RF-IoT02, RF-IoT03 | RN28 a RN33 | Identidade do dispositivo, associação e estados |
| Credenciais e autorizações | RF23, RF-IoT01, RF-IoT04 | RN34 a RN38 | Validade, revogação e integridade entre entidades |
| `RegistroAcesso` | RF24 | RN37, RN43 | Preservação histórica e controle de acesso |
| Notificações e preferências | RF22 e requisitos de automação | RN39 a RN41 | Deduplicação e proteção de notificações obrigatórias |
| Registros de auditoria | RF25 e requisitos de suporte | RN42 a RN45 | Integridade e rastreabilidade |
| Sessões e tarefas automáticas | RF34 a RF47 | RN46 a RN51 | Expiração, idempotência e consistência de execução |

Os nomes das entidades devem ser conferidos com o DER consolidado e com `DataDictionary.md`. A tabela não substitui a especificação física das chaves, relacionamentos, índices e constraints.

## 8. Matriz de cobertura de testes

Os testes devem ser definidos a partir dos requisitos e das regras de negócio. Os identificadores definitivos dos casos de teste serão registrados em `Docs/07-Testing/TestCases/`.

| Identificador do grupo | Escopo mínimo | Tipo de teste |
|---|---|---|
| CT-AUTH | Autenticação, autorização, sessões e revogação | Funcional e segurança |
| CT-TENANT | Isolamento de dados, encerramento e reativação de tenant | Integração e segurança |
| CT-RESERVA | Disponibilidade, cancelamento, concorrência e conflitos de horário | Funcional, integração e concorrência |
| CT-CHAMADO | Estados, transições, atribuição, resolução e histórico | Funcional e integração |
| CT-FINANCEIRO | Cobranças, inadimplência e regularização | Funcional e integridade |
| CT-OCORRENCIA | Registro, tratamento e associação com chamados | Funcional |
| CT-IOT | Autenticação MQTT, telemetria, alarmes e idempotência | Integração e segurança |
| CT-ACESSO | Emissão, validade, revogação e sincronização de autorizações | Funcional, integração e segurança |
| CT-NOTIFICACAO | Destinatários, preferências, deduplicação e falhas de entrega | Funcional e integração |
| CT-AUDITORIA | Registro e proteção de ações administrativas | Segurança e integridade |
| CT-SUPORTE | Escopo, autorização, prazo e revogação de acessos de suporte | Segurança e auditoria |
| CT-PERFORMANCE | Latência, carga e concorrência | Desempenho |
| CT-RECUPERACAO | Backup, restauração e continuidade | Recuperação e operação |
| CT-LGPD | Acesso, retenção, anonimização e eliminação controlada | Segurança e conformidade |
| CT-CLIENTES | Consistência entre Web, Mobile e Desktop | Funcional e integração |

Os identificadores acima representam grupos de teste propostos. Eles não devem ser tratados como casos de teste já especificados ou executados.