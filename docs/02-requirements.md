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

* bloquear o acesso de todos os usuários vinculados;

* impedir novas operações no tenant;

* impedir novas reservas;

* impedir a abertura ou processamento de novos chamados;

* impedir novas operações de controle de acesso;

* suspender o processamento operacional de telemetria associada ao tenant.

É estritamente vedada a exclusão física ou em cascata dos registros operacionais, financeiros (`CobrancaCondominial`), históricos e de auditoria associados ao tenant.

Os dados históricos devem ser preservados de acordo com as políticas de retenção e os requisitos legais aplicáveis.

### RF02 - Autenticação e Controle de Acesso

O sistema deve autenticar usuários por meio de credenciais seguras e emitir tokens JWT para autenticação das requisições.

O sistema deve aplicar controle de acesso baseado em papéis (RBAC), contemplando:

* `Morador`;

* `Funcionario`;

* `Administrador`;

* `Master`.

O perfil `Master` representa um usuário de nível superior à administração de um tenant específico e pode possuir `TenantID` nulo.

Usuários dos demais papéis devem obrigatoriamente estar associados a um tenant.

### RF03 - Gestão de Contas (Soft Delete Seletivo)

A inativação de entidades cadastrais e operacionais que suportem desativação deve preservar seus registros físicos no banco de dados, utilizando Soft Delete por meio de `DeletedAt`.

Essa política aplica-se, entre outras entidades, a:

* usuários;

* unidades;

* espaços comuns;

* dispositivos IoT;

* credenciais;

* demais entidades operacionais que possuam histórico associado.

Registros históricos e de auditoria, como `HistoricoChamado`, `RegistroAcesso` e `RegistroTelemetria`, não devem utilizar Soft Delete como mecanismo de remoção.

Esses registros devem ser tratados conforme suas respectivas políticas de retenção e integridade histórica.

---

## Operações e Reservas

### RF04 - Máquina de Estados para Chamados e Histórico Append-Only

Os chamados devem seguir uma máquina de estados definida pelo domínio e devem possuir um tipo compatível com os fluxos de:

* `Manutencao`;

* `AlarmeIoT`.

As alterações de estado devem ser registradas em `HistoricoChamado`.

O histórico deve operar estritamente em modo Append-Only, não permitindo alteração ou exclusão lógica de registros históricos.

Cada alteração deve registrar, no mínimo:

* status anterior;

* novo status;

* usuário responsável pela alteração;

* data e hora da alteração;

* comentário opcional.

### RF05 - Motor de Reservas

O sistema deve permitir reservas de espaços comuns com base em uma grade de horários parametrizável por espaço. O sistema deve validar regras de antecedência mínima, limite de horas e inadimplência do morador (bloqueando a reserva caso RF16 conste como pendente).

### RF06 - Cancelamento de Reservas

Reservas podem ser canceladas pelos moradores de acordo com o período mínimo de antecedência configurado para o espaço.

O sistema deve registrar:

* data e hora do cancelamento;

* usuário responsável pelo cancelamento;

* motivo do cancelamento, quando informado.

Cancelamentos realizados fora das regras permitidas devem ser rejeitados.

O sistema poderá executar cancelamentos automáticos quando previstos pelas regras de negócio ou políticas operacionais do sistema.

### RF07 - Estados Operacionais de Ativos

Espaços comuns e ativos de infraestrutura devem possuir controle de estado operacional independente de seu ciclo de vida cadastral.

Os estados devem permitir representar situações como:

* `Disponivel`;

* `Interditado`;

* `Manutencao`;

* `Operacional`;

* `Inativo`.

A alteração do estado operacional não deve resultar na exclusão ou alteração indevida do histórico cadastral ou operacional da entidade.

### RF08 - Dashboard Administrativo

O sistema deve disponibilizar ao administrador informações operacionais consolidadas, incluindo:

* taxa de ocupação dos espaços;

* reservas;

* chamados por estado;

* chamados dentro ou fora do SLA;

* situação de inadimplência;

* estado operacional da infraestrutura;

* estado dos dispositivos IoT;

* indicadores relevantes de telemetria.

Quando aplicável, essas informações devem ser atualizadas de forma próxima ao tempo real.

---

## Integração IoT

### RF-IoT01 - Emissão e Gestão de Credenciais de Acesso

Ao confirmar uma reserva que exija controle de acesso físico, o sistema deve gerar ou associar uma autorização de acesso vinculando:

* reserva;

* credencial;

* dispositivo físico;

* período de validade.

O modelo deve desacoplar:

* o estado lógico da autorização (`Ativa`, `Expirada`, `Revogada`);

* o estado de entrega/sincronização com a infraestrutura (`Pendente`, `Sincronizado`, `Falhou`).

Em caso de sincronização, o sistema deve registrar a data da última sincronização.

Em caso de falha, o sistema deve registrar o erro técnico associado à última tentativa de sincronização.

A revogação ou expiração de uma autorização deve impedir sua utilização conforme as políticas de segurança e o estado de sincronização da infraestrutura.

### RF-IoT02 - Telemetria da Infraestrutura

O sistema deve possuir processamento assíncrono para consumir dados de telemetria provenientes dos dispositivos IoT.

Os registros de telemetria devem ser armazenados em modo Append-Only e não devem utilizar Soft Delete como mecanismo de exclusão.

Cada dispositivo deve possuir informações suficientes para interpretar sua telemetria, incluindo:

* tipo do dispositivo;

* tipo de leitura;

* unidade de medida correspondente.

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

### RF10 - Registro de ocorrências

O sistema deve permitir que moradores registrem ocorrências relacionadas ao condomínio. O sistema deve permitir também que o administrador registre ocorrências recebidas por outros canais (telefone, presencial).

### RF11 - Abertura de chamados

O sistema deve permitir que moradores abram chamados para solicitar atendimento ou manutenção.

### RF12 - Atribuição de chamado

O sistema deve permitir que o administrador atribua o chamado a um funcionário.

### RF13 - Gerenciamento do chamado

O sistema deve permitir que o funcionário gerencie o chamado e sua solução.

### RF14 - Histórico de chamados

O sistema deve manter o histórico de alterações do chamado. O morador deve poder visualizar o histórico dos próprios chamados. O administrador e funcionários devem poder visualizar o histórico de todos os chamados.

## Gestão de Usuários, Unidades e Operações Administrativas

### RF15 - Gestão de Usuários e Dados da Conta

O sistema deve permitir que usuários autorizados realizem o cadastro, consulta, edição e inativação de usuários de acordo com suas permissões.

O administrador deve poder gerenciar usuários pertencentes ao próprio tenant, incluindo moradores e funcionários.

O sistema deve permitir que o usuário gerencie os dados de sua própria conta, respeitando os campos e operações permitidos pelo seu perfil.

A alteração de dados sensíveis de autenticação deve exigir validações de segurança apropriadas.

A inativação de um usuário deve preservar seu histórico operacional e impedir novas operações incompatíveis com seu estado.

### RF16 - Cobranças Condominiais e Inadimplência

O sistema deve permitir o cadastro, consulta, alteração, registro de pagamento e acompanhamento das cobranças condominiais das unidades, conforme as permissões do administrador.

Cada cobrança deve possuir, no mínimo, unidade, competência, valor, vencimento e situação.

O sistema deve permitir identificar cobranças pendentes, pagas e vencidas.

O sistema deve atualizar a situação de inadimplência das unidades de acordo com as cobranças pendentes e suas regras de vencimento.

O sistema deve impedir novas reservas quando as regras de negócio determinarem bloqueio por inadimplência.

O registro de pagamento deve preservar a data e demais informações necessárias para auditoria financeira.

### RF17 - Gestão de Unidades e Vínculos

O sistema deve permitir ao administrador cadastrar, consultar, editar e inativar unidades pertencentes ao próprio tenant.

O sistema deve permitir associar usuários às unidades e administrar os tipos de vínculo:

Proprietario;

Inquilino;

Dependente.

O sistema deve impedir vínculos entre entidades pertencentes a tenants distintos.

A alteração ou encerramento de uma unidade não deve remover seu histórico de vínculos ou operações anteriores.

### RF18 - Gestão de Espaços Comuns

O sistema deve permitir ao administrador cadastrar, consultar, editar, configurar e inativar espaços comuns.

Cada espaço deve possuir informações necessárias para sua utilização e reserva, incluindo regras de disponibilidade e, quando aplicável:

capacidade;

duração máxima;

antecedência mínima;

regras de funcionamento;

estado operacional.

As configurações do espaço devem ser utilizadas pelo motor de reservas.

Um espaço indisponível ou interditado não deve aceitar novas reservas incompatíveis com seu estado.

### RF19 - Gestão de Ativos e Infraestrutura

O sistema deve permitir ao administrador cadastrar, consultar, editar e inativar ativos e elementos de infraestrutura do condomínio.

Os ativos devem possuir estado operacional independente de seu ciclo de vida cadastral, conforme o RF07.

O sistema deve permitir consultar informações operacionais da infraestrutura e associar dispositivos IoT aos elementos físicos correspondentes quando aplicável.

A alteração do estado de um ativo deve preservar seu histórico operacional.

### RF20 - Gestão de Dispositivos IoT

O sistema deve permitir ao administrador cadastrar, consultar e gerenciar dispositivos IoT pertencentes ao próprio tenant.

Cada dispositivo deve possuir identificação única e informações necessárias para sua operação, incluindo tipo, identificação física e associação à infraestrutura quando aplicável.

O sistema deve permitir inativar dispositivos sem excluir seus registros históricos de telemetria, acesso e auditoria.

O sistema deve registrar informações necessárias para identificar falhas de comunicação e o estado operacional dos dispositivos.

### RF21 - Gestão de Ocorrências

O sistema deve permitir que usuários autorizados registrem, consultem e gerenciem ocorrências conforme suas permissões.

O administrador deve poder registrar ocorrências recebidas por canais externos e gerenciar seu ciclo de tratamento.

As ocorrências devem possuir informações suficientes para identificar sua natureza, situação, severidade, responsável e histórico de alterações quando aplicável.

O sistema deve preservar o histórico das ocorrências e impedir alterações incompatíveis com suas regras de integridade.

### RF22 - Gestão de Notificações

O sistema deve permitir a geração, consulta e gerenciamento de notificações direcionadas aos usuários.

As notificações devem poder estar associadas a eventos operacionais, administrativos ou de segurança, como:

confirmação ou cancelamento de reserva;

alteração de chamado;

atribuição de chamado;

alertas de infraestrutura;

alertas de telemetria;

falhas de sincronização;

comunicados administrativos.

O sistema deve permitir que administradores enviem notificações aos usuários do próprio tenant.

O sistema deve controlar o estado de entrega ou processamento das notificações quando necessário.

## Controle de Acesso e Auditoria

### RF23 - Gestão de Credenciais e Autorizações de Acesso

O sistema deve permitir ao administrador gerenciar credenciais de acesso associadas aos usuários, conforme os tipos de credencial suportados.

O sistema deve permitir consultar, criar, alterar, revogar e inativar credenciais conforme as permissões definidas.

As autorizações de acesso devem ser distintas das credenciais físicas ou lógicas.

O sistema deve permitir administrar autorizações de acesso, incluindo validade, associação ao usuário, reserva, dispositivo e situação lógica.

A revogação ou expiração de uma autorização deve impedir sua utilização conforme as políticas de segurança.

### RF24 - Registro e Consulta de Acessos

O sistema deve registrar eventos de acesso físico, incluindo, quando aplicável:

usuário ou credencial utilizada;

dispositivo responsável;

data e hora;

resultado da tentativa;

motivo de negação;

informações necessárias para auditoria.

O administrador deve poder consultar os registros de acesso do próprio tenant.

Os registros de acesso devem ser históricos e não devem ser alterados ou excluídos por operações administrativas comuns.

### RF25 - Auditoria de Operações

O sistema deve registrar automaticamente eventos relevantes de segurança, administração e operação para fins de auditoria.

Os registros devem permitir identificar, quando aplicável:

usuário responsável;

tenant;

operação executada;

entidade afetada;

data e hora;

resultado da operação.

Operações administrativas sensíveis, incluindo alterações de permissões, encerramento de tenants e acesso de suporte a dados de condomínios, devem ser auditáveis.

Os registros de auditoria devem possuir integridade histórica e não devem ser alterados ou excluídos por usuários comuns.

## Administração da Plataforma

### RF26 - Administração de Condomínios pela Plataforma

O sistema deve permitir ao usuário Master cadastrar, consultar, editar, encerrar e reativar condomínios na plataforma.

O encerramento de um condomínio deve obedecer ao RF01.1 e não deve provocar exclusão física de dados históricos, financeiros ou de auditoria.

O sistema deve registrar o histórico das alterações administrativas realizadas sobre cada tenant.

### RF27 - Gestão de Usuários Master e Permissões

O sistema deve permitir que usuários Master autorizados gerenciem outros usuários Master.

O sistema deve permitir definir permissões aplicáveis aos usuários Master, respeitando uma política de autoridade e evitando que uma configuração deixe a plataforma sem um usuário Master ativo.

Um usuário Master não deve poder realizar operações incompatíveis com suas próprias permissões.

Operações de criação, alteração, inativação e mudança de permissões de usuários Master devem ser auditadas.

### RF28 - Dashboard e Monitoramento da Plataforma

O sistema deve disponibilizar ao Master um dashboard com informações consolidadas da plataforma, incluindo, quando aplicável:

quantidade de tenants ativos e encerrados;

quantidade de usuários;

consumo de recursos;

estado dos serviços;

indicadores de integração;

alertas relevantes;

informações de utilização da plataforma.

O sistema deve permitir consultar a saúde operacional dos principais componentes da plataforma.

### RF29 - Planos, Limites e Consumo de Recursos

O sistema deve permitir ao Master cadastrar e gerenciar planos comerciais e limites de utilização da plataforma.

Os limites podem contemplar, conforme a definição comercial:

quantidade de usuários;

quantidade de dispositivos IoT;

volume de telemetria;

armazenamento;

outros recursos controlados pela plataforma.

O sistema deve permitir consultar o consumo de recursos de cada tenant e identificar situações próximas ou superiores aos limites definidos.

### RF30 - Suporte da Plataforma

O sistema deve permitir ao Master consultar, criar, acompanhar e atualizar solicitações de suporte relacionadas ao funcionamento da plataforma.

As solicitações de suporte devem ser separadas dos chamados operacionais de manutenção dos condomínios.

O sistema deve permitir que usuários autorizados acessem informações necessárias de um tenant para investigação de problemas de suporte, respeitando escopo mínimo de acesso e auditoria.

### RF31 - Configurações e Integrações Globais

O sistema deve permitir ao Master gerenciar configurações aplicáveis à plataforma como um todo.

O sistema deve permitir administrar integrações globais utilizadas pela plataforma, incluindo serviços externos necessários ao seu funcionamento.

Credenciais, chaves e segredos utilizados pelas integrações não devem ser expostos diretamente por interfaces administrativas comuns.

Alterações em configurações e integrações globais devem ser auditadas.

### RF32 - Manutenção e Atualizações da Plataforma

O sistema deve permitir ao Master administrar o estado de manutenção da plataforma e registrar manutenções programadas.

O sistema deve permitir informar usuários afetados por períodos de manutenção.

O sistema deve permitir controlar ou acompanhar versões e atualizações da plataforma conforme o processo de implantação adotado.

Operações de manutenção e atualização devem possuir registro de auditoria.

### RF33 - Cobrança da Plataforma

O sistema deve permitir ao Master gerenciar a relação financeira entre a empresa responsável pela plataforma e os condomínios contratantes.

Esse módulo deve ser separado das cobranças condominiais dos moradores.

O sistema deve permitir, conforme o modelo comercial adotado:

consultar cobranças;

registrar pagamentos;

acompanhar pendências;

relacionar cobranças ao plano contratado.

## Automação e Processamento do Sistema

### RF34 - Expiração Automática de Reservas e Autorizações

O sistema deve processar automaticamente o término de reservas e autorizações de acesso.

Ao atingir a validade, a autorização deve ser marcada como expirada e não deve permanecer utilizável.

O processamento deve respeitar as condições de sincronização e revogação da infraestrutura.

### RF35 - Notificações Automáticas

O sistema deve gerar e enviar notificações automaticamente quando eventos configurados ocorrerem.

O mecanismo deve suportar, conforme a funcionalidade:

confirmação de reserva;

cancelamento;

atribuição ou alteração de chamado;

alertas de telemetria;

falhas de sincronização;

eventos de acesso;

avisos de manutenção;

outros eventos operacionais relevantes.

Falhas de envio devem ser registradas e, quando aplicável, reprocessadas.

### RF36 - Processamento de Vencimentos e Inadimplência

O sistema deve processar automaticamente os vencimentos das cobranças condominiais.

O sistema deve identificar cobranças vencidas e atualizar a situação de inadimplência das unidades conforme as regras financeiras do tenant.

O processamento automático não deve alterar registros históricos de pagamento de forma incompatível com a auditoria financeira.

### RF37 - Monitoramento de SLA

O sistema deve monitorar automaticamente os prazos de atendimento dos chamados.

O sistema deve identificar chamados próximos do vencimento do SLA e chamados que tenham ultrapassado o prazo.

Quando aplicável, o sistema deve gerar alertas para os responsáveis e registrar a ocorrência do descumprimento do SLA.

### RF38 - Detecção de Falhas de Dispositivos IoT

O sistema deve monitorar a comunicação dos dispositivos IoT.

O sistema deve identificar, conforme limites configurados:

ausência prolongada de telemetria;

falhas recorrentes de comunicação;

falhas de sincronização;

indisponibilidade do dispositivo.

Quando aplicável, o sistema deve gerar alerta operacional e permitir o tratamento por usuários responsáveis.

### RF39 - Reprocessamento de Operações com Falha

O sistema deve permitir o reprocessamento controlado de operações assíncronas que falharem.

As tentativas devem possuir limites e registro de falhas.

Operações que não puderem ser concluídas após as tentativas previstas devem ser identificadas como falhas definitivas e disponibilizadas para tratamento administrativo ou operacional.

### RF40 - Gestão de Sessões e Revogação

O sistema deve controlar a validade dos tokens e sessões de autenticação.

Sessões expiradas devem deixar de permitir acesso às operações protegidas.

O sistema deve permitir revogar sessões ou tokens quando necessário por motivos de segurança ou administração.

### RF41 - Limpeza de Dados Temporários

O sistema deve executar tarefas automáticas de limpeza de dados temporários expirados, respeitando as políticas de retenção e evitando a remoção de registros históricos ou dados sujeitos a obrigação de preservação.

### RF42 - Continuidade e Backup

O sistema deve executar backups automáticos dos dados necessários à recuperação da plataforma.

Os backups devem possuir política de retenção, proteção contra acesso indevido e mecanismos de verificação de execução.

A restauração de backups deve ser uma operação administrativa controlada e não deve ocorrer automaticamente sem uma política explícita de recuperação.

### RF43 - Monitoramento de Serviços da Plataforma

O sistema deve monitorar a disponibilidade dos principais serviços necessários ao funcionamento da plataforma.

Quando detectar indisponibilidade ou falha relevante, deve registrar o evento e disponibilizar alerta aos responsáveis.

O monitoramento pode abranger, conforme a arquitetura:

API;

banco de dados;

broker MQTT;

processamento assíncrono;

serviços de integração;

serviços de armazenamento.

## Recurso Futuro - Reconhecimento Facial

### RF-IoT04 - Reconhecimento Facial para Controle de Acesso

Em uma versão futura, fora do escopo do MVP, o sistema deverá permitir a integração com um serviço de inteligência artificial em nuvem para reconhecimento facial aplicado ao controle de acesso dos moradores.

A funcionalidade deverá permitir:

cadastrar ou vincular uma identidade facial a um usuário autorizado;

enviar a imagem ou representação necessária ao serviço de reconhecimento facial, conforme a arquitetura definida;

receber o resultado da identificação ou verificação;

relacionar o resultado à tentativa de acesso;

autorizar ou negar o acesso conforme as regras de segurança;

registrar o evento de acesso para auditoria.

A solução deverá utilizar mecanismos seguros de transmissão e armazenamento.

O tratamento de dados biométricos deverá observar a LGPD e as bases legais, políticas de retenção, consentimento quando aplicável e requisitos de segurança definidos para a funcionalidade.

A indisponibilidade do serviço de reconhecimento facial não deve, por si só, eliminar mecanismos de contingência de acesso previamente definidos.

O reconhecimento facial deverá permanecer desacoplado do modelo atual de credenciais, permitindo coexistência com RFID, senha numérica e demais mecanismos de acesso suportados.

## Requisitos Não-Funcionais

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

* Argon2id;

* bcrypt.

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

* expiração;

* sincronização;

* revogação;

* atualização;

* comportamento durante indisponibilidade da rede.

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

* retenção temporal;

* anonimização;

* agregação estatística;

* purga controlada.

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

* `Reserva.DataHoraFim` deve ser posterior a `Reserva.DataHoraInicio`;

* `AutorizacaoAcesso.ValidoAte` deve ser posterior a `AutorizacaoAcesso.ValidoDe`;

* `CobrancaCondominial.Valor` não pode ser negativo;

* `CobrancaCondominial.DataPagamento` deve ser nula enquanto a cobrança não estiver paga;

* `Usuario.TenantID` deve ser nulo somente para usuários com papel `Master`;

* usuários que não sejam `Master` devem possuir `TenantID`;

* entidades relacionadas operacionalmente devem pertencer ao mesmo tenant;

* uma reserva válida não pode sobrepor outra reserva válida para o mesmo espaço;

* registros de `HistoricoChamado`, `RegistroAcesso` e `RegistroTelemetria` devem ser tratados como históricos imutáveis;

* exclusões em cascata não devem remover dados históricos, financeiros ou de auditoria.