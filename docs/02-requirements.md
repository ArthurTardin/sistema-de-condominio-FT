# Requisitos do Sistema

## Requisitos Funcionais

### Arquitetura e Autenticação

### RF01 - Arquitetura Multi-tenant

O sistema deve suportar múltiplos condomínios simultaneamente. Todo registro operacional que possua contexto de tenant deve ser associado a um `TenantID`, garantindo o isolamento lógico dos dados entre os condomínios.

O sistema deve utilizar Global Query Filters no Entity Framework Core como mecanismo padrão de filtragem por tenant, complementado por validações de autorização, regras de domínio e constraints de integridade no banco de dados quando aplicável.

Nenhuma operação de leitura ou escrita deve permitir acesso ou associação indevida entre dados pertencentes a tenants distintos.

### RF01.1 - Inativação e Encerramento de Tenant

O preenchimento de `DeletedAt` na entidade `Tenant` representa exclusivamente o encerramento comercial ou contratual do condomínio na plataforma.

O encerramento de um tenant deve:

- bloquear o acesso de todos os usuários vinculados;
- impedir novas operações no tenant;
- impedir novas reservas;
- impedir a abertura ou processamento de novos chamados;
- impedir novas operações de controle de acesso;
- suspender o processamento operacional de telemetria associada ao tenant.

É estritamente vedada a exclusão física ou em cascata dos registros operacionais, financeiros (`CobrancaCondominial`), históricos e de auditoria associados ao tenant.

Os dados históricos devem ser preservados de acordo com as políticas de retenção e os requisitos legais aplicáveis.

### RF02 - Autenticação e Controle de Acesso

O sistema deve autenticar usuários por meio de credenciais seguras e emitir tokens JWT para autenticação das requisições.

O sistema deve aplicar controle de acesso baseado em papéis (RBAC), contemplando:

- `Morador`;
- `Funcionario`;
- `Administrador`;
- `Master`.

O perfil `Master` representa um usuário de nível superior à administração de um tenant específico e pode possuir `TenantID` nulo.

Usuários dos demais papéis devem obrigatoriamente estar associados a um tenant.

### RF03 - Gestão de Contas (Soft Delete Seletivo)

A inativação de entidades cadastrais e operacionais que suportem desativação deve preservar seus registros físicos no banco de dados, utilizando Soft Delete por meio de `DeletedAt`.

Essa política aplica-se, entre outras entidades, a:

- usuários;
- unidades;
- espaços comuns;
- dispositivos IoT;
- credenciais;
- reservas e demais entidades operacionais que possuam histórico associado.

Registros históricos e de auditoria, como `HistoricoChamado`, `RegistroAcesso` e `RegistroTelemetria`, não devem utilizar Soft Delete como mecanismo de remoção.

Esses registros devem ser tratados conforme suas respectivas políticas de retenção e integridade histórica.

---

## Operações e Reservas

### RF04 - Máquina de Estados para Chamados e Histórico Append-Only

Os chamados devem seguir uma máquina de estados definida pelo domínio e devem possuir um tipo compatível com os fluxos de:

- `Manutencao`;
- `AlarmeIoT`.

As alterações de estado devem ser registradas em `HistoricoChamado`.

O histórico deve operar estritamente em modo Append-Only, não permitindo alteração ou exclusão lógica de registros históricos.

Cada alteração deve registrar, no mínimo:

- status anterior;
- novo status;
- usuário responsável pela alteração;
- data e hora da alteração;
- comentário opcional.

### RF05 - Motor de Reservas

O sistema deve permitir reservas de espaços comuns de acordo com as regras parametrizadas para cada espaço.

O motor de reservas deve validar, no mínimo:

- antecedência mínima;
- antecedência máxima;
- duração máxima da reserva;
- período solicitado;
- disponibilidade do espaço;
- status operacional do espaço;
- situação de inadimplência da unidade vinculada ao morador;
- regras de cancelamento aplicáveis.

Reservas não podem possuir intervalos de tempo sobrepostos para o mesmo espaço quando ambas estiverem em estado válido para ocupação.

A regra de não sobreposição deve ser garantida de forma transacional e, quando aplicável, por mecanismo de integridade do banco de dados.

O modelo deve separar o usuário que solicitou a reserva (`UsuarioID`) do usuário responsável pelo cancelamento (`UsuarioCancelamentoID`).

### RF06 - Cancelamento Automatizado

Reservas podem ser canceladas pelos moradores de acordo com o período mínimo de antecedência configurado para o espaço.

O sistema deve registrar:

- data e hora do cancelamento;
- usuário responsável pelo cancelamento;
- motivo do cancelamento, quando informado.

Cancelamentos realizados fora das regras permitidas devem ser rejeitados.

O sistema poderá executar cancelamentos automáticos quando previstos pelas regras de negócio ou políticas operacionais do sistema.

### RF07 - Estados Operacionais de Ativos

Espaços comuns e ativos de infraestrutura devem possuir controle de estado operacional independente de seu ciclo de vida cadastral.

Os estados devem permitir representar situações como:

- `Disponivel`;
- `Interditado`;
- `Manutencao`;
- `Operacional`;
- `Inativo`.

A alteração do estado operacional não deve resultar na exclusão ou alteração indevida do histórico cadastral ou operacional da entidade.

### RF08 - Dashboard Administrativo

O sistema deve disponibilizar ao administrador informações operacionais consolidadas, incluindo:

- taxa de ocupação dos espaços;
- reservas;
- chamados por estado;
- chamados dentro ou fora do SLA;
- situação de inadimplência;
- estado operacional da infraestrutura;
- estado dos dispositivos IoT;
- indicadores relevantes de telemetria.

Quando aplicável, essas informações devem ser atualizadas de forma próxima ao tempo real.

---

## Integração IoT

### RF-IoT01 - Emissão e Gestão de Credenciais de Acesso

Ao confirmar uma reserva que exija controle de acesso físico, o sistema deve gerar ou associar uma autorização de acesso vinculando:

- reserva;
- credencial;
- dispositivo físico;
- período de validade.

O modelo deve desacoplar:

- o estado lógico da autorização (`Ativa`, `Expirada`, `Revogada`);
- o estado de entrega/sincronização com a infraestrutura (`Pendente`, `Sincronizado`, `Falhou`).

Em caso de sincronização, o sistema deve registrar a data da última sincronização.

Em caso de falha, o sistema deve registrar o erro técnico associado à última tentativa de sincronização.

A revogação ou expiração de uma autorização deve impedir sua utilização conforme as políticas de segurança e o estado de sincronização da infraestrutura.

### RF-IoT02 - Telemetria da Infraestrutura

O sistema deve possuir processamento assíncrono para consumir dados de telemetria provenientes dos dispositivos IoT.

Os registros de telemetria devem ser armazenados em modo Append-Only e não devem utilizar Soft Delete como mecanismo de exclusão.

Cada dispositivo deve possuir informações suficientes para interpretar sua telemetria, incluindo:

- tipo do dispositivo;
- tipo de leitura;
- unidade de medida correspondente.

Os dados de telemetria devem possuir política própria de retenção, conforme definido no RNF10.

### RF-IoT03 - Abertura Autônoma de Chamados e Alertas

Quando dados de telemetria ultrapassarem limites críticos previamente parametrizados, o sistema deve ser capaz de:

1. identificar a condição anormal;
2. registrar o evento;
3. gerar automaticamente um chamado do tipo `AlarmeIoT`, quando aplicável;
4. disponibilizar um alerta operacional aos funcionários e administradores conectados.

Os alertas em tempo real devem utilizar WebSockets ou mecanismo equivalente de comunicação persistente.

---

## Ocorrências e Chamados

### RF10 - Registro de Ocorrências (Domínio Disciplinar)

O sistema deve possuir uma entidade dedicada (`Ocorrencia`) para registrar infrações, advertências e questões disciplinares ou administrativas relacionadas às unidades ou usuários.

As ocorrências devem possuir classificação de severidade e registrar seu responsável pela criação.

Uma ocorrência pode, opcionalmente, dar origem a um único chamado técnico correlato.

O relacionamento entre ocorrência e chamado deve preservar a distinção entre:

- ocorrência administrativa/disciplinar;
- chamado técnico de manutenção ou infraestrutura.

### RF11 - Abertura de Chamados

O sistema deve permitir a abertura de chamados exclusivamente para:

- manutenção física;
- alarmes provenientes da infraestrutura IoT.

Os chamados devem possuir título, descrição, tipo, estado atual e usuário responsável pela abertura.

O sistema deve permitir a atribuição do chamado a um funcionário e o acompanhamento de seu SLA.

Chamados originados automaticamente por telemetria devem possuir o tipo `AlarmeIoT`.

### RF12 - Atribuição de Chamado

O sistema deve permitir que usuários com permissão administrativa atribuam um chamado a um funcionário autorizado.

A atribuição deve registrar o usuário responsável pela ação e integrar-se ao histórico do chamado quando representar uma alteração relevante do fluxo operacional.

### RF13 - Gerenciamento do Chamado

O funcionário responsável deve poder gerenciar o chamado de acordo com as permissões atribuídas e com a máquina de estados definida pelo sistema.

O gerenciamento deve permitir, conforme o estado atual:

- atualização do andamento;
- registro de comentários;
- alteração de estado;
- registro da solução;
- encerramento do atendimento.

Todas as alterações relevantes devem ser registradas no histórico do chamado.

### RF14 - Histórico de Chamados

O sistema deve manter histórico completo e Append-Only das alterações relevantes dos chamados.

O morador deve poder visualizar o histórico dos chamados aos quais possui acesso.

Administradores e funcionários autorizados devem poder visualizar o histórico dos chamados pertencentes ao tenant.

O histórico não deve permitir alteração ou exclusão lógica dos registros já registrados.

---

# Requisitos Não Funcionais

## Desempenho e Processamento

### RNF01 - Tempo de Resposta

As APIs de backend devem apresentar tempo de processamento interno inferior a 200 milissegundos no percentil 95 (P95), exceto para operações explicitamente classificadas como rotinas pesadas, como determinados relatórios e processos analíticos.

A medição deve considerar o processamento interno da aplicação, não incluindo indisponibilidade ou latência causada por sistemas externos fora do controle da API.

### RNF02 - Processamento Assíncrono

O backend deve utilizar programação assíncrona baseada em `Task` (`async/await`) para operações de I/O de banco de dados, rede, mensageria e demais operações de entrada e saída que possam bloquear recursos.

O processamento assíncrono deve ser utilizado para evitar bloqueio desnecessário de threads durante picos de acesso ou indisponibilidade de serviços externos.

### RNF03 - Prevenção de Travamentos e Timeouts

Todas as chamadas externas, integrações e operações potencialmente demoradas devem suportar `CancellationToken`.

Operações sujeitas a espera por recursos externos devem possuir políticas de timeout específicas de acordo com sua natureza e criticidade.

O timeout deve impedir espera indefinida e liberar os recursos associados à operação.

Os limites de timeout devem ser definidos individualmente para cada tipo de operação, não sendo obrigatório um único limite global para todas as requisições.

---

## Concorrência e Integridade

### RNF04 - Controle de Concorrência e Prevenção de Double-Booking

O sistema deve implementar mecanismos de controle de concorrência para evitar inconsistências causadas por operações simultâneas.

A entidade `Reserva` deve utilizar controle de concorrência otimista (`RowVersion`) para detectar alterações concorrentes incompatíveis.

Além disso, o banco de dados deve garantir que duas reservas válidas do mesmo espaço não possuam intervalos de tempo sobrepostos.

A proteção contra double-booking deve funcionar mesmo quando duas requisições de reserva forem processadas simultaneamente.

Uma tentativa de criação de reserva conflitante deve falhar de forma controlada, sem permitir duplicidade de ocupação, e a API deve retornar uma resposta apropriada ao usuário.

---

## Segurança e IoT

### RNF05 - Segurança de Senhas

As senhas dos usuários não devem ser armazenadas em texto puro.

O sistema deve utilizar algoritmos modernos de password hashing com salt, como:

- Argon2id;
- bcrypt.

Os parâmetros de custo do algoritmo devem ser configuráveis de acordo com as recomendações de segurança vigentes.

### RNF06 - Restrições Físicas de Hardware e MQTT

O sistema deve garantir a unicidade global do endereço físico (`MacAddress`) dos dispositivos IoT cadastrados.

Os tópicos MQTT devem possuir estrutura hierárquica e escopo por tenant, impedindo colisões e associações indevidas entre dispositivos de condomínios distintos.

A convenção de tópicos deve seguir padrão consistente, como:

`tenant/{tenantId}/devices/{deviceId}/...`

### RNF07 - Edge Computing e Fallback IoT

Os dispositivos responsáveis pelo controle de acesso físico devem possuir capacidade de armazenamento local para operação temporariamente desconectada.

Em caso de indisponibilidade da conexão com a plataforma, o dispositivo deve conseguir validar localmente autorizações previamente sincronizadas para as próximas 24 horas.

A política de cache deve possuir regras explícitas para:

- expiração;
- sincronização;
- revogação;
- atualização;
- comportamento durante indisponibilidade da rede.

### RNF08 - Criptografia em Trânsito

Todo o tráfego HTTP da aplicação deve utilizar HTTPS com TLS 1.2 ou superior.

As comunicações MQTT devem utilizar transporte seguro e autenticação apropriada dos dispositivos, utilizando credenciais individuais ou mecanismo equivalente de autenticação por dispositivo.

---

## Conformidade e Operação

### RNF09 - Separação de Configurações e Segredos

Credenciais de banco de dados, chaves JWT, credenciais de serviços externos e URIs/configurações sensíveis do broker MQTT não devem ser armazenadas diretamente no código-fonte.

Essas configurações devem ser fornecidas por mecanismos externos de configuração, preferencialmente variáveis de ambiente ou serviço seguro de gerenciamento de segredos.

### RNF10 - LGPD e Retenção de Dados

Os dados operacionais, registros de acesso físico, credenciais e dados de telemetria devem possuir políticas de retenção compatíveis com sua finalidade, necessidade operacional, segurança e requisitos legais aplicáveis.

As políticas de retenção devem ser definidas por categoria de dado.

Quando aplicável, o sistema deve utilizar:

- retenção temporal;
- anonimização;
- agregação estatística;
- purga controlada.

A aplicação dessas políticas não deve comprometer registros que possuam obrigação de preservação legal, fiscal, contratual ou de auditoria.

---

# Constraints de Integridade

As seguintes restrições de unicidade devem ser aplicadas no banco de dados para garantir integridade relacional e evitar duplicidades inconsistentes.

## Tenant

### `CNPJ UNIQUE`

O CNPJ do condomínio deve ser globalmente único na plataforma.

## Usuário

### `UNIQUE (TenantID, Email)`

O e-mail do usuário deve ser único dentro do mesmo tenant.

O mesmo endereço de e-mail pode existir em tenants diferentes.

Usuários com papel `Master` constituem uma exceção ao modelo convencional de tenant, podendo possuir `TenantID` nulo.

## Unidade

### `UNIQUE (TenantID, Bloco, Numero)`

Garante que não existam duas unidades com o mesmo bloco e número dentro do mesmo tenant.

Tenants diferentes podem utilizar a mesma convenção de endereçamento.

## Cobrança Condominial

### `UNIQUE (TenantID, UnidadeID, Competencia)`

Garante que exista apenas uma cobrança para determinada unidade e competência dentro do tenant.

A cobrança não deve ser duplicada para a mesma competência.

## Vínculo de Usuário e Unidade

### `UNIQUE (TenantID, UsuarioID, UnidadeID)`

Impede que o mesmo usuário seja vinculado repetidamente à mesma unidade dentro do mesmo tenant.

O tipo de vínculo (`Proprietario`, `Inquilino`, `Dependente`) deve ser tratado conforme as regras de domínio.

## Dispositivo IoT

### `MacAddress UNIQUE`

O endereço físico (`MacAddress`) do dispositivo IoT deve possuir unicidade global em toda a base de dados, independentemente do tenant.

## Regras Adicionais de Integridade

Além das constraints de unicidade, o banco e a aplicação devem garantir as seguintes invariantes:

- `Reserva.DataHoraFim` deve ser posterior a `Reserva.DataHoraInicio`;
- `AutorizacaoAcesso.ValidoAte` deve ser posterior a `AutorizacaoAcesso.ValidoDe`;
- `CobrancaCondominial.Valor` não pode ser negativo;
- `CobrancaCondominial.DataPagamento` deve ser nula enquanto a cobrança não estiver paga;
- `Usuario.TenantID` deve ser nulo somente para usuários com papel `Master`;
- usuários que não sejam `Master` devem possuir `TenantID`;
- entidades relacionadas operacionalmente devem pertencer ao mesmo tenant;
- uma reserva válida não pode sobrepor outra reserva válida para o mesmo espaço;
- registros de `HistoricoChamado`, `RegistroAcesso` e `RegistroTelemetria` devem ser tratados como históricos imutáveis;
- exclusões em cascata não devem remover dados históricos, financeiros ou de auditoria.