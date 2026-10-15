## Requisitos Funcionais

## Arquitetura e Autenticação

### RF01 - Arquitetura Multi-tenant

O sistema deve suportar múltiplos condomínios simultaneamente, mantendo o isolamento lógico entre os dados de cada tenant.

Todo registro operacional que possua contexto de tenant deve estar associado a um `TenantID` válido. Exceções, como registros globais da plataforma e usuários `Master`, devem ser explicitamente definidas no modelo de dados e nas regras de autorização.

O sistema deve utilizar Global Query Filters no Entity Framework Core como mecanismo padrão de filtragem por tenant, complementado por validações de autorização, regras de domínio e restrições de integridade no banco de dados, quando aplicáveis.

Nenhuma operação de leitura, escrita, atualização ou exclusão deve permitir acesso ou associação indevida entre dados pertencentes a tenants distintos.

O `TenantID` utilizado em operações protegidas deve ser obtido de um contexto autenticado e validado no servidor. O sistema não deve confiar exclusivamente no identificador de tenant enviado pelo cliente.

As consultas administrativas globais devem possuir autorização explícita e ser separadas das operações comuns de um tenant.

Os filtros globais do Entity Framework Core não devem ser considerados, isoladamente, uma barreira de segurança. Operações que os ignorem, consultas SQL diretas, tarefas assíncronas e integrações devem aplicar as mesmas regras de isolamento e autorização.

### RF01.1 - Inativação e Encerramento de Tenant

O preenchimento de `DeletedAt` na entidade `Tenant` representa exclusivamente o encerramento comercial ou contratual do condomínio na plataforma.

O encerramento de um tenant deve:

- bloquear a autenticação e o acesso operacional de todos os usuários vinculados ao tenant;
- impedir novas operações de negócio no tenant;
- impedir a criação e a confirmação de novas reservas;
- impedir a abertura de novos chamados e a continuidade de operações de atendimento incompatíveis com o encerramento;
- impedir novas operações de controle de acesso;
- impedir a criação de novas autorizações de acesso;
- impedir novas sincronizações operacionais de credenciais e autorizações;
- suspender o processamento operacional de telemetria associada ao tenant;
- impedir que mensagens ou tarefas pendentes provoquem novas operações de negócio;
- preservar os dados necessários à auditoria, à recuperação e ao cumprimento de obrigações legais.

O encerramento de um tenant não deve provocar a exclusão física ou em cascata de registros operacionais, financeiros, históricos ou de auditoria associados, incluindo `CobrancaCondominial`, `HistoricoChamado`, `RegistroAcesso` e `RegistroTelemetria`.

Os dados históricos devem ser preservados de acordo com suas políticas de retenção, finalidade de tratamento e obrigações legais aplicáveis.

Mensagens MQTT recebidas após o encerramento do tenant não devem provocar operações de negócio. Quando necessário para diagnóstico, as mensagens devem ser rejeitadas ou encaminhadas para uma área de quarentena técnica, respeitando os limites de retenção e minimização de dados.

Tarefas assíncronas pendentes devem validar o estado atual do tenant antes de executar operações de negócio. O encerramento deve impedir a execução de tarefas incompatíveis com esse estado.

A reativação de um tenant deve ser uma operação administrativa explícita, autorizada e auditada.

A reativação não deve restaurar automaticamente credenciais, autorizações ou sessões revogadas durante o encerramento, nem reativar entidades que tenham sido inativadas individualmente.

Após a reativação, somente as operações compatíveis com o estado atual do tenant, dos usuários, das credenciais e dos demais recursos associados poderão ser executadas.

## RF02 - Autenticação e Controle de Acesso

O sistema deve autenticar usuários por meio de credenciais seguras e utilizar tokens JWT para autenticação das requisições protegidas.

O sistema deve aplicar controle de acesso baseado em papéis (RBAC), contemplando:

- `Morador`;
- `Funcionario`;
- `Administrador`;
- `Master`.

O perfil `Master` representa um usuário de nível superior à administração de um tenant específico e pode possuir `TenantID` nulo.

Usuários dos demais papéis devem estar associados a um tenant válido e ativo.

O sistema deve aplicar autorização no servidor para cada operação protegida, verificando, conforme aplicável:

- papel do usuário;
- permissões específicas;
- tenant associado à operação;
- vínculo do usuário com o recurso;
- propriedade ou responsabilidade sobre o recurso;
- estado atual do tenant, do usuário e da entidade;
- condições e regras de negócio aplicáveis.

A autenticação bem-sucedida não concede, por si só, autorização para acessar todos os recursos de um tenant. Ocultar botões ou páginas no cliente não substitui a autorização no backend.

O sistema deve implementar as seguintes regras de autenticação e sessão:

- emitir tokens de acesso com validade limitada;
- utilizar refresh tokens com armazenamento seguro;
- rotacionar refresh tokens durante sua utilização;
- detectar reutilização indevida de refresh tokens quando tecnicamente aplicável;
- permitir o encerramento da sessão atual;
- permitir que o usuário encerre todas as próprias sessões;
- permitir a revogação administrativa de sessões por usuários autorizados;
- invalidar ou impedir a renovação de sessões após inativação da conta, alteração de senha ou incidente de segurança, conforme a política definida;
- aplicar limitação de tentativas de autenticação e mecanismos de proteção contra abuso;
- registrar eventos relevantes de segurança sem armazenar senhas, tokens completos ou segredos nos logs.

Quando for exigida revogação imediata, a aplicação deve utilizar um mecanismo de verificação de sessão, versão de segurança, lista de revogação ou solução equivalente. A simples expiração de um JWT não garante revogação imediata.

O login dos usuários vinculados a condomínios deve identificar o tenant por meio de um identificador público apropriado, como um slug ou outro mecanismo validado pelo servidor.

O login de usuários `Master` deve utilizar um fluxo separado, como `/master/login`, com as respectivas regras de autorização.

A recuperação de senha deve utilizar um token temporário, de uso único e com expiração. A resposta ao pedido de recuperação não deve revelar se um e-mail está cadastrado.

Após a redefinição de senha, as sessões existentes devem ser revogadas conforme a política de segurança.

O sistema deve impedir que usuários de tenants encerrados realizem novas operações protegidas, ainda que possuam tokens anteriormente emitidos.

## RF03 - Gestão de Contas e Soft Delete Seletivo

A inativação de entidades cadastrais e operacionais que suportem desativação deve preservar seus registros físicos no banco de dados, utilizando Soft Delete por meio de `DeletedAt`.

Essa política deve ser aplicada, conforme o modelo de dados, a:

- usuários;
- unidades;
- espaços comuns;
- ativos de infraestrutura;
- dispositivos IoT;
- credenciais;
- reservas;
- demais entidades operacionais que possuam histórico associado.

Registros históricos e de auditoria, como `HistoricoChamado`, `RegistroAcesso`, `RegistroTelemetria` e registros de auditoria, não devem utilizar Soft Delete como mecanismo comum de remoção.

Esses registros devem seguir suas respectivas políticas de retenção, integridade histórica e obrigações legais.

A inativação de uma entidade deve impedir novas operações incompatíveis com seu estado, sem apagar ou invalidar indevidamente operações anteriores.

A aplicação deve diferenciar a inativação cadastral, a alteração do estado operacional e o encerramento do tenant. Esses eventos possuem finalidades distintas e não devem ser tratados como equivalentes.

O sistema deve aplicar as políticas de retenção e minimização de dados pessoais. O uso de Soft Delete ou de registros Append-Only não deve ser interpretado como obrigação de conservar indefinidamente todos os dados.

---

## Operações e Reservas

### RF04 - Máquina de Estados para Chamados e Histórico Append-Only

Os chamados devem seguir uma máquina de estados definida pelo domínio e possuir um tipo compatível com os fluxos de:

- `Manutencao`;
- `AlarmeIoT`.

Os estados operacionais dos chamados devem ser:

- `Aberto`;
- `Atribuido`;
- `EmAndamento`;
- `AguardandoPecaTerceiro`;
- `Resolvido`;
- `Fechado`;
- `Cancelado`.

As transições permitidas devem seguir as seguintes regras:

| Estado de origem | Estado de destino | Condição |
|---|---|---|
| `Aberto` | `Atribuido` | Atribuição a funcionário ativo e autorizado do mesmo tenant. |
| `Aberto` | `Cancelado` | Cancelamento por ator autorizado, conforme as regras de cancelamento. |
| `Atribuido` | `EmAndamento` | Início do atendimento pelo funcionário autorizado. |
| `Atribuido` | `Cancelado` | Cancelamento por ator autorizado, conforme as regras de cancelamento. |
| `EmAndamento` | `AguardandoPecaTerceiro` | Existência de dependência que impeça temporariamente a continuidade do atendimento. |
| `EmAndamento` | `Resolvido` | Registro da solução pelo funcionário autorizado. |
| `EmAndamento` | `Cancelado` | Cancelamento por ator autorizado, com justificativa obrigatória. |
| `AguardandoPecaTerceiro` | `EmAndamento` | Resolução da dependência e retomada do atendimento. |
| `Resolvido` | `EmAndamento` | Reabertura autorizada, com justificativa registrada. |
| `Resolvido` | `Fechado` | Confirmação de encerramento pelo morador solicitante, conforme as regras do sistema. |

As transições não listadas devem ser rejeitadas, exceto quando uma alteração formal das regras de negócio definir uma nova transição.

Os estados `Fechado` e `Cancelado` são terminais. Chamados nesses estados não podem ser reabertos nem ter seu estado alterado diretamente.

Caso surja um novo problema após o fechamento ou cancelamento, deve ser criado outro chamado, com referência ao anterior quando houver relação entre os eventos.

O morador solicitante pode confirmar o fechamento de um chamado resolvido ou solicitar sua reabertura. A reabertura deve ser autorizada, exigir justificativa e retornar o chamado para `EmAndamento`.

O funcionário responsável pode atualizar o chamado dentro dos limites de seu papel, de suas permissões e das transições permitidas. O administrador pode executar as operações administrativas autorizadas no próprio tenant, respeitando as regras de auditoria.

O cancelamento de um chamado pode ser realizado pelo solicitante, pelo funcionário responsável quando autorizado, ou por um administrador do mesmo tenant. A operação deve respeitar os estados permitidos e exigir justificativa não vazia, composta por conteúdo diferente de espaços em branco.

Chamados do tipo `AlarmeIoT` podem ser gerados automaticamente a partir de eventos de infraestrutura quando aplicável. Esses chamados devem utilizar uma identidade técnica de sistema previamente definida, válida no contexto do tenant e distinguível de usuários humanos nos registros de auditoria.

As alterações de estado devem ser registradas em `HistoricoChamado`.

O histórico deve operar em modo Append-Only para as operações comuns da aplicação, não permitindo alteração ou exclusão de registros históricos por operações ordinárias.

Cada alteração de estado deve registrar, no mínimo:

- estado anterior;
- novo estado;
- usuário responsável ou identidade técnica do sistema;
- data e hora da alteração;
- comentário ou justificativa, quando exigido;
- origem da alteração, quando aplicável, como usuário, automação ou integração.

Correções excepcionais em dados históricos, quando admissíveis, devem seguir procedimento controlado e auditado, sem alteração silenciosa dos registros originais.

### RF05 - Motor de Reservas

O sistema deve permitir a criação de reservas de espaços comuns com base em uma grade de horários parametrizável por espaço.

O sistema deve validar:

- disponibilidade do espaço;
- duração permitida;
- antecedência mínima;
- limite máximo de antecedência, quando configurado;
- horários de funcionamento;
- estado operacional do espaço;
- existência de outra reserva válida no mesmo intervalo;
- situação de inadimplência do morador, conforme o RF16;
- permissões do solicitante;
- vínculo do solicitante com o tenant;
- estado atual do tenant e do espaço.

O sistema deve bloquear novas reservas quando as regras de inadimplência do tenant determinarem essa restrição.

A validação de disponibilidade deve ocorrer no servidor e ser protegida contra requisições simultâneas. A validação realizada somente na interface não é suficiente.

O banco de dados deve impedir intervalos sobrepostos para o mesmo espaço entre reservas que ocupem períodos, inclusive quando duas requisições forem processadas simultaneamente.

Os estados de reserva que ocupam um período devem ser definidos explicitamente nas regras de domínio. Reservas canceladas ou expiradas não devem bloquear períodos futuros após a atualização consistente de seu estado.

A criação e a confirmação de uma reserva devem ocorrer somente após a validação das regras aplicáveis e a persistência bem-sucedida do registro.

Em caso de concorrência, somente as reservas compatíveis com as restrições de integridade poderão ser confirmadas.

As falhas na criação ou confirmação não devem resultar em reservas confirmadas indevidamente.

As configurações de reserva devem respeitar o RF18 e o RF44. Alterações de configuração não devem modificar retroativamente as condições de reservas já confirmadas, salvo regra explícita e auditada.

### RF06 - Cancelamento de Reservas

Reservas podem ser canceladas pelos moradores de acordo com o período mínimo de antecedência configurado para o espaço.

O administrador pode cancelar reservas de moradores do próprio tenant antes do início da reserva, desde que informe uma justificativa obrigatória.

Funcionários não podem cancelar reservas de moradores por padrão. Essa permissão somente poderá ser concedida por uma regra explícita de autorização.

O sistema deve registrar:

- data e hora do cancelamento;
- usuário responsável pelo cancelamento;
- motivo do cancelamento, quando informado ou exigido;
- estado anterior e novo estado da reserva;
- origem do cancelamento, quando aplicável.

Cancelamentos realizados fora das regras permitidas devem ser rejeitados.

O sistema poderá executar cancelamentos automáticos quando previstos pelas regras de negócio ou políticas operacionais. Esses cancelamentos devem identificar a rotina responsável e registrar o motivo.

Quando o administrador cancelar uma reserva, o sistema deve notificar o titular da reserva e preservar o histórico da operação.

O cancelamento deve liberar a disponibilidade futura do espaço sem apagar o registro da reserva ou de suas alterações anteriores.

Cancelamentos decorrentes de indisponibilidade operacional do espaço devem respeitar as regras definidas no RF07.

### RF07 - Estados Operacionais de Ativos

Espaços comuns e ativos de infraestrutura devem possuir controle de estado operacional independente de seu ciclo de vida cadastral.

Os estados operacionais previstos são:

- `Disponivel`;
- `Interditado`;
- `Manutencao`;
- `Operacional`;
- `Inativo`.

A aplicação de cada estado deve respeitar o tipo de entidade. O sistema deve distinguir a disponibilidade para reserva de um espaço do estado operacional de um ativo de infraestrutura, evitando tratar esses conceitos como equivalentes.

A alteração do estado operacional não deve resultar na exclusão ou alteração indevida do histórico cadastral ou operacional da entidade.

Espaços interditados ou indisponíveis não devem aceitar novas reservas incompatíveis com seu estado.

Quando um espaço passar para um estado incompatível com sua utilização, o sistema deve avaliar as reservas futuras já confirmadas conforme uma política explícita de indisponibilidade.

Essa política deve definir se a reserva será mantida, cancelada ou encaminhada para tratamento administrativo. Deve distinguir situações planejadas, como manutenção programada, de situações emergenciais, como falhas críticas de infraestrutura.

O sistema não deve cancelar reservas silenciosamente. Quando houver cancelamento administrativo ou automático autorizado, deve registrar o responsável ou a rotina, a justificativa, o estado anterior e o novo estado, além de notificar os usuários afetados quando aplicável.

A alteração do estado operacional não deve apagar o histórico das reservas relacionadas.

### RF08 - Dashboard Administrativo

O sistema deve disponibilizar ao administrador informações operacionais consolidadas, incluindo:

- taxa de ocupação dos espaços;
- reservas;
- chamados por estado;
- chamados dentro ou fora do SLA;
- situação de inadimplência;
- estado operacional da infraestrutura;
- estado dos dispositivos IoT;
- indicadores relevantes de telemetria;
- alertas críticos pendentes.

Quando aplicável, essas informações devem ser atualizadas de forma próxima ao tempo real.

O dashboard deve apresentar somente dados do tenant ao qual o administrador está autorizado.

Os indicadores devem utilizar critérios consistentes e documentados, evitando divergências entre os números exibidos e os registros operacionais.

### RF09 - Aplicações Web, Mobile e Desktop

O sistema deve disponibilizar aplicações Web, Mobile e Desktop conforme o escopo aprovado, os perfis de usuário e os casos de uso previstos.

As aplicações devem utilizar o backend central e a API compartilhada para executar operações de negócio, salvo quando uma necessidade técnica específica justificar outra arquitetura documentada.

A autorização, as regras de domínio, a validação de tenant e as restrições de integridade devem ser aplicadas no backend, independentemente do cliente utilizado.

As funcionalidades disponíveis em cada cliente devem ser documentadas por perfil e caso de uso.

A aplicação Web deve permitir acesso às funcionalidades previstas por meio de navegador compatível.

A aplicação Mobile deve disponibilizar as funcionalidades previstas para dispositivos móveis, respeitando as permissões do usuário e as regras de negócio.

A aplicação Desktop deve disponibilizar as funcionalidades operacionais previstas para seus perfis, especialmente quando necessárias à rotina administrativa ou de atendimento.

A existência de um cliente específico não deve conceder permissões adicionais por si só.

As aplicações devem tratar expiração de sessão, erros de comunicação, falhas de autenticação e indisponibilidade da API de maneira controlada.

---

## Integração IoT

### RF-IoT01 - Emissão e Gestão de Credenciais de Acesso

Ao confirmar uma reserva que exija controle de acesso físico, o sistema deve criar ou associar uma autorização de acesso vinculando:

- reserva;
- credencial;
- dispositivo físico;
- usuário autorizado;
- tenant;
- período de validade.

O modelo deve distinguir o estado lógico da autorização do estado de entrega ou sincronização com a infraestrutura.

Os estados lógicos da autorização devem ser:

- `Ativa`;
- `Expirada`;
- `Revogada`.

Os estados de sincronização devem ser:

- `Pendente`;
- `Sincronizado`;
- `Falhou`.

O sistema deve registrar a data e hora da última tentativa de sincronização e, quando houver falha, as informações técnicas necessárias para diagnóstico.

A revogação ou expiração de uma autorização deve impedir sua utilização conforme as políticas de segurança e as capacidades da infraestrutura.

O sistema deve impedir a criação ou sincronização de novas autorizações para usuários, credenciais, dispositivos, reservas ou tenants inativos.

A sincronização deve ser idempotente e não deve reativar autorizações revogadas ou expiradas.

### RF-IoT02 - Telemetria da Infraestrutura

O sistema deve possuir processamento assíncrono para consumir dados de telemetria provenientes dos dispositivos IoT.

Os registros de telemetria devem ser armazenados em modo Append-Only para as operações comuns da aplicação e não devem utilizar Soft Delete como mecanismo ordinário de exclusão.

Cada registro deve possuir informações suficientes para interpretar sua origem e conteúdo, incluindo:

- tipo do dispositivo;
- tipo de leitura;
- unidade de medida correspondente;
- data e hora da medição;
- identificação do dispositivo de origem;
- tenant associado;
- valor medido;
- informações de qualidade ou validade da leitura, quando aplicável.

O sistema deve validar a origem das mensagens, o escopo do tenant, a identificação do dispositivo remetente e o formato dos dados recebidos.

Mensagens duplicadas ou repetidas não devem provocar duplicidade indevida de operações de negócio. Quando o protocolo permitir, o sistema deve utilizar identificadores de mensagem, sequências ou mecanismos equivalentes de idempotência.

Os dados de telemetria devem possuir política própria de retenção, conforme definido no RNF10 e nas regras de ciclo de vida dos dados.

O caráter Append-Only não deve ser interpretado como obrigação de conservação ilimitada de dados pessoais. A retenção deve considerar finalidade, segurança, necessidade operacional e obrigações legais.

### RF-IoT03 - Abertura Autônoma de Chamados e Alertas

Quando dados de telemetria ultrapassarem limites críticos previamente parametrizados, o sistema deve ser capaz de:

1. identificar a condição anormal;
2. registrar o evento;
3. gerar automaticamente um chamado do tipo `AlarmeIoT`, quando aplicável;
4. disponibilizar um alerta operacional aos funcionários e administradores autorizados;
5. correlacionar o evento com o dispositivo, o tipo de alarme, a condição monitorada e o chamado relacionado.

Os alertas em tempo real devem utilizar WebSockets ou mecanismo equivalente.

Eventos repetidos referentes à mesma condição anormal não devem gerar indefinidamente novos chamados para o mesmo incidente ativo. O sistema deve correlacionar os eventos e associá-los ao chamado existente quando as regras de negócio determinarem que pertencem ao mesmo incidente.

A correlação deve utilizar critérios documentados, como dispositivo, condição monitorada, tipo de alarme e situação do incidente.

O processamento deve ser idempotente. A repetição de uma mensagem ou tarefa não deve produzir duplicidade indevida de chamados ou notificações.

O sistema deve definir como uma condição é considerada normalizada e quando um novo evento representa um novo incidente.

A normalização da telemetria não deve fechar automaticamente um chamado que exija validação humana, salvo quando uma regra explícita permitir esse comportamento.

Se um chamado anterior estiver fechado e uma nova condição anormal representar um novo incidente, o sistema deve criar outro chamado e preservar a relação histórica entre os eventos quando aplicável.

Falhas no processamento, na criação do chamado ou na entrega do alerta devem ser registradas e disponibilizadas para reprocessamento controlado.

### RF-IoT04 - Reconhecimento Facial para Controle de Acesso

Em uma versão futura, fora do escopo do MVP, o sistema deverá permitir a integração com um serviço de inteligência artificial em nuvem para reconhecimento facial aplicado ao controle de acesso de moradores autorizados.

A funcionalidade deverá permitir:

- cadastrar ou vincular uma identidade facial a um usuário autorizado;
- enviar ao serviço de reconhecimento facial a imagem ou representação necessária, conforme a arquitetura aprovada;
- receber o resultado da identificação ou verificação;
- relacionar o resultado à tentativa de acesso;
- autorizar ou negar o acesso conforme as regras de segurança;
- registrar o evento de acesso para auditoria;
- desativar ou remover a associação facial conforme as regras de retenção e os direitos aplicáveis.

O tratamento de dados biométricos deve observar a LGPD, considerando a natureza sensível desses dados, a finalidade específica, a base legal aplicável, a transparência, a necessidade, a segurança e os direitos dos titulares.

A base legal adequada e a necessidade de avaliação de impacto devem ser analisadas antes da implementação. O consentimento não deve ser presumido como a única base legal possível nem considerado automaticamente suficiente.

A solução deve limitar a coleta e o armazenamento ao estritamente necessário. Sempre que tecnicamente viável, deve evitar o armazenamento desnecessário de imagens brutas e utilizar mecanismos seguros de representação biométrica.

A integração deve utilizar transmissão criptografada, autenticação entre serviços, controle de acesso restrito e gestão segura de segredos.

A funcionalidade deve definir regras para:

- cadastramento e atualização dos dados faciais;
- exclusão ou revogação da associação;
- prazo de retenção;
- acesso administrativo aos dados;
- registro de consultas e operações;
- tratamento de falsos positivos e falsos negativos;
- indisponibilidade do provedor em nuvem;
- contestação de uma decisão de acesso;
- alternativa de autenticação quando o reconhecimento facial não estiver disponível.

Uma decisão automatizada de reconhecimento facial não deve eliminar mecanismos de contingência previamente definidos nem impedir a investigação de uma tentativa de acesso contestada.

O reconhecimento facial deve permanecer desacoplado do modelo de credenciais, permitindo coexistência com RFID, senha numérica e outros mecanismos suportados.

A inclusão dessa funcionalidade no modelo de dados não significa que ela esteja disponível no MVP ou autorizada para uso em produção.

---

## Ocorrências e Chamados

### RF10 - Registro de Ocorrências

O sistema deve permitir que moradores registrem ocorrências relacionadas ao condomínio.

O administrador também deve poder registrar ocorrências recebidas por outros canais, como telefone ou atendimento presencial.

Cada ocorrência deve possuir informações suficientes para identificar sua natureza, descrição, data, autor ou origem, situação e severidade, quando aplicável.

O sistema deve preservar o histórico das ocorrências e controlar quem pode consultá-las ou alterá-las.

### RF11 - Abertura de Chamados

O sistema deve permitir que moradores abram chamados para solicitar atendimento ou manutenção.

Cada chamado deve estar associado ao tenant correto e possuir informações suficientes para identificar o solicitante, o tipo de atendimento, a descrição, a data de abertura e o estado inicial.

Quando necessário, o chamado poderá estar associado a uma ocorrência, unidade, ativo, dispositivo ou reserva.

O sistema deve impedir a criação de chamados em tenants encerrados ou por usuários sem permissão.

O chamado deve ser criado inicialmente no estado `Aberto`, salvo fluxo automatizado explicitamente definido para um tipo específico de chamado.

### RF12 - Atribuição de Chamados

O sistema deve permitir que o administrador atribua um chamado a um funcionário ativo e autorizado pertencente ao mesmo tenant.

O sistema deve validar se o funcionário pode receber o chamado de acordo com seu papel e suas permissões.

A atribuição deve registrar o responsável anterior, quando existir, o novo responsável, o usuário que realizou a alteração e a data e hora da operação.

A alteração deve ser registrada no histórico do chamado e deve gerar notificação aos usuários pertinentes conforme as regras de notificação aplicáveis.

### RF13 - Gerenciamento do Chamado

O sistema deve permitir que funcionários autorizados gerenciem os chamados atribuídos a eles, respeitando a máquina de estados definida no RF04.

O gerenciamento deve incluir, conforme as permissões:

- iniciar o atendimento;
- registrar comentários;
- atualizar informações operacionais;
- indicar dependência de peça ou terceiro;
- registrar a solução;
- executar cancelamento autorizado;
- acompanhar o histórico do atendimento.

O sistema deve impedir transições de estado não permitidas e registrar as alterações relevantes.

O funcionário não deve poder acessar ou alterar chamados de outros tenants.

### RF13.1 - Início do Atendimento

Um funcionário autorizado pode mover um chamado de `Atribuido` para `EmAndamento`.

O sistema deve validar o estado atual do chamado e registrar a data e hora de início do atendimento.

### RF13.2 - Aguardando Peça ou Terceiro

Um funcionário autorizado pode mover um chamado de `EmAndamento` para `AguardandoPecaTerceiro` quando houver uma dependência externa que impeça a continuidade.

A mudança deve possuir justificativa ou comentário explicativo.

O retorno para `EmAndamento` deve ser registrado quando a dependência for resolvida.

### RF13.3 - Resolução do Chamado

Um funcionário autorizado pode mover um chamado de `EmAndamento` para `Resolvido`, registrando a solução aplicada e as informações necessárias para compreender o atendimento.

O sistema deve preservar os registros anteriores.

### RF13.4 - Fechamento do Chamado

O morador solicitante pode confirmar o fechamento de um chamado no estado `Resolvido`, alterando-o para `Fechado`.

O sistema deve registrar a data e hora do fechamento.

Qualquer regra alternativa de fechamento deve ser definida de maneira explícita e auditável, sem permitir fechamento arbitrário por usuários sem autorização.

### RF13.5 - Cancelamento do Chamado

O cancelamento deve respeitar as transições e os estados permitidos definidos no RF04.

O solicitante, o funcionário responsável quando autorizado e o administrador do tenant podem cancelar um chamado dentro das condições aplicáveis.

O cancelamento deve exigir justificativa não vazia, registrar o responsável e preservar o histórico.

Chamados em `Resolvido`, `Fechado` ou `Cancelado` não devem ser cancelados novamente.

### RF13.6 - Reabertura do Chamado

Um chamado em `Resolvido` pode retornar para `EmAndamento` mediante solicitação autorizada do morador ou ação administrativa permitida.

A reabertura deve exigir justificativa e registrar o usuário responsável, a data e hora e o motivo.

Chamados em `Fechado` ou `Cancelado` não podem ser reabertos diretamente.

### RF13.7 - Permissões do Funcionário

O funcionário deve poder gerenciar somente os chamados autorizados pelo seu perfil e escopo de atendimento.

Por padrão, o funcionário pode alterar os chamados atribuídos a ele. A visualização de outros chamados deve seguir as permissões definidas para a função.

### RF13.8 - Histórico e Comentários

Comentários e alterações relevantes devem ser registrados de forma que seja possível reconstruir a sequência do atendimento.

O sistema deve diferenciar comentários operacionais de alterações formais de estado.

Registros históricos não devem ser apagados ou alterados por operações comuns.

### RF13.9 - Regras de Cancelamento e Justificativa

O cancelamento de chamados deve respeitar os estados e os papéis autorizados definidos no RF04.

A justificativa deve ser obrigatória e não pode conter apenas espaços em branco.

O sistema deve registrar o estado anterior, o novo estado, o autor do cancelamento, a data e hora e a justificativa.

O cancelamento deve gerar as notificações previstas nas regras de negócio e não deve remover o histórico associado.

### RF14 - Histórico de Chamados

O sistema deve manter o histórico das alterações dos chamados.

O morador deve poder visualizar o histórico dos próprios chamados.

O funcionário deve poder visualizar o histórico dos chamados que estejam dentro de seu escopo de autorização.

O administrador deve poder visualizar o histórico dos chamados do próprio tenant.

Os registros devem ser apresentados em ordem cronológica e preservar informações necessárias para auditoria e acompanhamento.

---

## Gestão de Usuários, Unidades e Operações Administrativas

### RF15 - Gestão de Usuários e Dados da Conta

O sistema deve permitir que usuários autorizados realizem o cadastro, a consulta, a edição e a inativação de usuários de acordo com suas permissões.

O administrador deve poder gerenciar usuários pertencentes ao próprio tenant, incluindo moradores e funcionários.

O sistema deve permitir que o usuário gerencie os dados da própria conta, respeitando os campos e as operações permitidos pelo seu perfil.

A alteração de dados sensíveis de autenticação deve exigir validações de segurança apropriadas.

A inativação de um usuário deve preservar seu histórico operacional e impedir novas operações incompatíveis com seu estado.

A inativação de um usuário não deve revogar ou alterar registros históricos de acesso, chamados, reservas ou operações financeiras anteriormente realizados.

### RF16 - Cobranças Condominiais e Inadimplência

O sistema deve permitir o cadastro, a consulta, a alteração, o registro de pagamento e o acompanhamento das cobranças condominiais das unidades, conforme as permissões do administrador.

Cada cobrança deve possuir, no mínimo:

- unidade associada;
- competência;
- valor;
- vencimento;
- situação.

O sistema deve permitir identificar cobranças pendentes, pagas e vencidas.

O sistema deve atualizar a situação de inadimplência das unidades de acordo com as cobranças e as regras de vencimento aplicáveis.

O sistema deve impedir novas reservas quando as regras de negócio determinarem bloqueio por inadimplência.

As regras devem definir como são tratadas cobranças contestadas, pagamentos registrados e eventuais ajustes financeiros, quando essas operações forem suportadas.

O registro de pagamento deve preservar a data e as demais informações necessárias para auditoria financeira.

Alterações em cobranças e pagamentos devem ser auditadas. O histórico financeiro não deve ser apagado por inativação de unidades ou encerramento do tenant.

### RF17 - Gestão de Unidades e Vínculos

O sistema deve permitir ao administrador cadastrar, consultar, editar e inativar unidades pertencentes ao próprio tenant.

O sistema deve permitir associar usuários às unidades e administrar os tipos de vínculo:

- `Proprietario`;
- `Inquilino`;
- `Dependente`.

O sistema deve impedir vínculos entre entidades pertencentes a tenants distintos.

A alteração ou o encerramento de uma unidade não deve remover seu histórico de vínculos ou operações anteriores.

O sistema deve validar a consistência dos vínculos e impedir associações duplicadas incompatíveis com as regras de integridade.

### RF18 - Gestão de Espaços Comuns

O sistema deve permitir ao administrador cadastrar, consultar, editar, configurar e inativar espaços comuns.

Cada espaço deve possuir informações necessárias para sua utilização e reserva, incluindo:

- capacidade;
- duração máxima;
- antecedência mínima;
- limite máximo de antecedência, quando aplicável;
- horários de funcionamento;
- regras de funcionamento;
- estado operacional;
- regras de cancelamento;
- exigências de controle de acesso, quando aplicáveis.

As configurações devem ser utilizadas pelo motor de reservas definido no RF05.

Um espaço indisponível ou interditado não deve aceitar novas reservas incompatíveis com seu estado.

Alterações nas regras de reserva devem afetar novas reservas, sem modificar retroativamente as condições das reservas existentes, salvo quando uma regra explícita e auditada determinar outra conduta.

### RF19 - Gestão de Ativos e Infraestrutura

O sistema deve permitir ao administrador cadastrar, consultar, editar e inativar ativos e elementos de infraestrutura do condomínio.

Os ativos devem possuir estado operacional independente de seu ciclo de vida cadastral, conforme o RF07.

O sistema deve permitir consultar informações operacionais da infraestrutura e associar dispositivos IoT aos elementos físicos correspondentes quando aplicável.

A alteração do estado de um ativo deve preservar seu histórico operacional.

### RF20 - Gestão de Dispositivos IoT

O sistema deve permitir ao administrador cadastrar, consultar e gerenciar dispositivos IoT pertencentes ao próprio tenant.

Cada dispositivo deve possuir identificação única e informações necessárias para sua operação, incluindo tipo, identificação física, estado de comunicação e associação à infraestrutura quando aplicável.

O sistema deve impedir identificações físicas duplicadas quando a regra de unicidade estiver definida para o tipo de identificador.

O sistema deve permitir inativar dispositivos sem excluir seus registros históricos de telemetria, acesso e auditoria.

O sistema deve registrar informações necessárias para identificar falhas de comunicação e o estado operacional dos dispositivos.

O sistema deve impedir a associação de um dispositivo a entidades de outro tenant.

### RF21 - Gestão de Ocorrências

O sistema deve permitir que usuários autorizados registrem, consultem e gerenciem ocorrências conforme suas permissões.

O administrador deve poder registrar ocorrências recebidas por canais externos e gerenciar seu ciclo de tratamento.

As ocorrências devem possuir informações suficientes para identificar sua natureza, situação, severidade, responsável e histórico de alterações, quando aplicável.

O sistema deve preservar o histórico das ocorrências e impedir alterações incompatíveis com suas regras de integridade.

O acesso às ocorrências deve respeitar o princípio do menor privilégio. Moradores devem visualizar somente as ocorrências que registraram ou aquelas às quais possuam autorização explícita.

### RF22 - Gestão de Notificações

O sistema deve permitir a geração, consulta e gerenciamento de notificações direcionadas aos usuários.

As notificações devem poder estar associadas a eventos operacionais, administrativos ou de segurança, incluindo:

- confirmação ou cancelamento de reserva;
- alteração ou atribuição de chamado;
- alertas de infraestrutura;
- alertas críticos de telemetria;
- falhas de sincronização;
- eventos de acesso relevantes;
- comunicados administrativos;
- alterações relevantes em cobranças, quando aplicável.

O sistema deve permitir que administradores enviem notificações aos usuários do próprio tenant.

O sistema deve controlar o estado de processamento e entrega das notificações quando necessário.

As categorias de notificação obrigatória devem incluir:

- `security`, para eventos relevantes de segurança;
- `legal`, para comunicações exigidas por obrigação legal ou política aplicável;
- `ticket`, para comunicações necessárias ao fluxo de atendimento de chamados;
- eventos críticos de controle de acesso e infraestrutura classificados como relevantes para a segurança.

Notificações obrigatórias não podem ser desativadas por preferências individuais.

O cancelamento administrativo de uma reserva deve gerar notificação ao titular da reserva.

A entrega de uma notificação deve ser registrada conforme o canal utilizado e as informações técnicas necessárias.

Falhas de entrega devem ser tratadas conforme as regras de processamento e reprocessamento definidas no RF35 e no RF39.

---

## Controle de Acesso e Auditoria

### RF23 - Gestão de Credenciais e Autorizações de Acesso

O sistema deve permitir ao administrador gerenciar credenciais de acesso associadas aos usuários, conforme os tipos de credencial efetivamente suportados pela versão do sistema.

Os tipos previstos para o controle de acesso podem incluir:

- `TagRFID`;
- `SenhaNumerica`;
- `Biometria`, quando implementada e autorizada.

A inclusão de um tipo no modelo de dados não significa que seu mecanismo esteja disponível no MVP ou autorizado para uso em produção.

Credenciais biométricas, incluindo reconhecimento facial, devem permanecer como funcionalidades futuras até que seus requisitos funcionais, controles de segurança, requisitos de privacidade e integrações tenham sido aprovados.

O sistema deve permitir consultar, criar, alterar, revogar e inativar credenciais conforme as permissões definidas.

As autorizações de acesso devem ser distintas das credenciais físicas ou lógicas.

O sistema deve permitir administrar autorizações de acesso, incluindo validade, associação ao usuário, reserva, dispositivo e situação lógica.

A revogação ou expiração de uma autorização deve impedir sua utilização conforme as políticas de segurança e as capacidades da infraestrutura.

O sistema deve preservar o histórico de emissão, sincronização, alteração, expiração e revogação de credenciais e autorizações.

### RF24 - Registro e Consulta de Acessos

O sistema deve registrar eventos de acesso físico, incluindo, quando aplicável:

- usuário ou credencial utilizada;
- dispositivo responsável;
- data e hora;
- tenant;
- resultado da tentativa;
- motivo de negação;
- autorização ou reserva relacionada;
- informações necessárias para auditoria.

O administrador deve poder consultar os registros de acesso do próprio tenant, respeitando as permissões definidas.

O acesso aos registros deve observar o princípio do menor privilégio e a necessidade operacional.

Os registros de acesso devem ser históricos e não devem ser alterados ou excluídos por operações administrativas comuns.

A retenção dos registros deve seguir uma política definida, considerando segurança, auditoria, minimização de dados e obrigações legais.

### RF25 - Auditoria de Operações

O sistema deve registrar automaticamente eventos relevantes de segurança, administração e operação para fins de auditoria.

Os registros devem permitir identificar, quando aplicável:

- usuário responsável ou identidade técnica;
- tenant;
- operação executada;
- entidade afetada;
- data e hora;
- resultado da operação;
- justificativa;
- valores anteriores e posteriores, quando necessários;
- origem da operação, como interface, API, automação ou integração.

Operações administrativas sensíveis, incluindo alterações de permissões, encerramento de tenants, cancelamentos administrativos de reservas e acesso de suporte a dados de condomínios, devem ser auditáveis.

Os registros de auditoria devem possuir integridade histórica e não devem ser alterados ou excluídos por usuários comuns.

O sistema deve diferenciar registros de auditoria, históricos funcionais e logs técnicos de monitoramento. Cada categoria deve possuir finalidade, controle de acesso e política de retenção adequados.

Os registros devem evitar o armazenamento de senhas, tokens completos, segredos ou dados pessoais desnecessários.

A retenção deve considerar a finalidade de auditoria, as obrigações legais e o princípio de minimização de dados.

---

## Administração da Plataforma

### RF26 - Administração de Condomínios pela Plataforma

O sistema deve permitir ao usuário `Master` autorizado cadastrar, consultar, editar, encerrar e reativar condomínios na plataforma.

O encerramento de um condomínio deve obedecer ao RF01.1 e não deve provocar exclusão física de dados históricos, financeiros ou de auditoria.

O sistema deve registrar o histórico das alterações administrativas realizadas sobre cada tenant.

A reativação deve exigir autorização e registro de auditoria.

O sistema deve validar o estado atual do tenant antes de permitir operações administrativas incompatíveis com seu ciclo de vida.

### RF27 - Gestão de Usuários Master e Permissões

O sistema deve permitir que usuários `Master` autorizados gerenciem outros usuários `Master`.

O sistema deve permitir definir permissões aplicáveis aos usuários `Master`, respeitando uma política de autoridade e evitando que uma configuração deixe a plataforma sem um usuário Master ativo.

Um usuário Master não deve poder realizar operações incompatíveis com suas próprias permissões.

Operações de criação, alteração, inativação e mudança de permissões de usuários Master devem ser auditadas.

Permissões críticas devem ser concedidas somente a usuários autorizados e não devem ser ampliadas automaticamente por meio de acesso de suporte.

### RF28 - Dashboard e Monitoramento da Plataforma

O sistema deve disponibilizar ao Master um dashboard com informações consolidadas da plataforma, incluindo, quando aplicável:

- quantidade de tenants ativos e encerrados;
- quantidade de usuários;
- consumo de recursos;
- estado dos serviços;
- indicadores de integração;
- alertas relevantes;
- informações de utilização da plataforma;
- falhas operacionais relevantes.

O sistema deve permitir consultar a saúde operacional dos principais componentes da plataforma.

Informações de tenants diferentes devem ser agregadas de forma controlada. A consulta de dados individualizados de um tenant deve exigir permissão específica.

Os indicadores devem possuir critérios documentados e não devem expor dados pessoais desnecessários.

### RF29 - Planos, Limites e Consumo de Recursos

O sistema deve permitir ao Master cadastrar e gerenciar planos comerciais e limites de utilização da plataforma.

Os limites podem contemplar, conforme a definição comercial:

- quantidade de usuários;
- quantidade de dispositivos IoT;
- volume de telemetria;
- armazenamento;
- outros recursos controlados pela plataforma.

O sistema deve permitir consultar o consumo de recursos de cada tenant e identificar situações próximas ou superiores aos limites definidos.

A aplicação de limites deve possuir comportamento definido para cada tipo de recurso, evitando bloqueios inesperados de operações críticas de segurança ou preservação de dados.

O sistema não deve eliminar dados históricos automaticamente apenas porque um limite comercial foi atingido.

### RF30 - Suporte da Plataforma

O sistema deve permitir ao Master consultar, criar, acompanhar e atualizar solicitações de suporte relacionadas ao funcionamento da plataforma.

As solicitações de suporte devem ser separadas dos chamados operacionais de manutenção dos condomínios.

O sistema deve permitir que usuários Master autorizados acessem informações necessárias de um tenant para investigar problemas de suporte, respeitando o escopo mínimo de acesso, a finalidade declarada, o prazo da sessão e a auditoria.

O acesso de suporte não deve conceder acesso irrestrito a todos os tenants ou permissões administrativas permanentes.

### RF-SUP01 - Autorização de Acesso de Suporte

O sistema deve permitir que um usuário Master com permissão específica de suporte inicie uma sessão de suporte para um tenant autorizado.

A sessão normal de suporte não exige aprovação prévia de outro Master, desde que o executor possua a permissão de suporte necessária e cumpra todas as condições estabelecidas neste requisito.

A autorização deve ser validada no servidor antes do início da sessão.

O sistema deve registrar o solicitante, o tenant, a finalidade, o escopo autorizado, o resultado da autorização e o horário de início.

### RF-SUP02 - Escopo Mínimo e Lista de Permissões

Cada sessão de suporte deve possuir um escopo explícito e limitado.

As operações permitidas devem ser selecionadas a partir de uma lista de permissões controlada. O sistema deve bloquear operações que não estejam explicitamente autorizadas.

A lista de operações de suporte pode contemplar:

- `read`, para leitura dos dados necessários ao diagnóstico;
- `correct`, para correções de dados autorizadas e delimitadas;
- `config`, para alterações de configurações específicas do tenant;
- `sessions`, para consulta ou revogação de sessões conforme a autorização concedida;
- `reprocess`, para reprocessamento controlado de operações assíncronas;
- `access`, para diagnóstico de dispositivos e autorizações de acesso;
- `financial`, para consulta ou correção financeira dentro do escopo autorizado;
- `permissions`, para consulta ou alteração de permissões somente quando expressamente autorizada.

A permissão `permissions` deve ser considerada crítica, exigir autorização específica e produzir auditoria detalhada.

O sistema deve adotar o princípio de negação por padrão. Qualquer operação não autorizada de maneira explícita deve ser bloqueada.

As permissões efetivas durante uma sessão de suporte devem corresponder à interseção entre as permissões permanentes do executor, as permissões autorizadas para suporte e o escopo específico concedido à sessão.

Uma sessão de suporte não pode ampliar as permissões permanentes do executor nem conceder operações que ele não esteja autorizado a executar na plataforma.

O sistema deve validar o escopo da sessão no servidor em cada operação protegida, incluindo operações assíncronas iniciadas durante a sessão.

O acesso de suporte não deve permitir escalada de privilégios, acesso a outro tenant, exclusão irrestrita de dados, remoção de registros históricos ou alterações globais da plataforma.

### RF-SUP03 - Duração e Expiração da Sessão de Suporte

Toda sessão de suporte deve possuir limite de duração, tenant de destino e escopo de operações.

A sessão normal deve ter duração máxima de 60 minutos por autorização, salvo limite menor definido pela política da plataforma.

A continuidade após o vencimento deve exigir nova autorização e novo registro de auditoria.

O encerramento ou a expiração da sessão deve invalidar o contexto de suporte para novas operações.

Operações assíncronas ainda não executadas devem revalidar a autorização e o estado da sessão antes de iniciar. O sistema deve possuir política explícita para o tratamento seguro de operações já iniciadas.

O sistema deve registrar o início, o término, a expiração e o motivo do encerramento da sessão.

### RF-SUP04 - Justificativa e Finalidade

Antes de iniciar uma sessão de suporte, o usuário Master deve informar uma justificativa não vazia e uma finalidade compatível com a solicitação.

A justificativa deve ser específica o suficiente para permitir auditoria posterior.

O sistema deve impedir o início da sessão quando a justificativa estiver ausente ou contiver apenas espaços em branco.

A justificativa, o tenant de destino, o executor e o escopo solicitado devem ser registrados.

### RF-SUP05 - Auditoria das Operações de Suporte

O sistema deve registrar todas as operações executadas durante uma sessão de suporte.

A auditoria deve permitir identificar:

- usuário Master executor;
- tenant acessado;
- identificador da sessão;
- justificativa;
- autorização concedida;
- escopo autorizado;
- data e hora de cada operação;
- recurso consultado ou alterado;
- resultado da operação;
- valores anteriores e posteriores, quando aplicável;
- motivo de falha ou bloqueio, quando aplicável.

A auditoria não deve armazenar senhas, tokens completos ou segredos. Dados pessoais devem ser registrados somente quando necessários à finalidade de auditoria.

Os registros de auditoria devem ser protegidos contra alteração ou exclusão pelo executor da sessão.

### RF-SUP06 - Acesso Emergencial de Suporte

O sistema deve oferecer um fluxo separado de acesso emergencial para situações críticas em que a operação normal de suporte não seja suficiente.

O acesso emergencial deve exigir justificativa específica, identificação do incidente, tenant de destino e escopo solicitado.

O sistema deve registrar o caráter emergencial da sessão e as permissões concedidas.

O acesso emergencial deve possuir duração limitada e não deve permitir acesso irrestrito ou permanente.

A sessão deve gerar um evento de auditoria prioritário para revisão posterior.

### RF-SUP07 - Revisão Posterior do Acesso Emergencial

Toda sessão emergencial deve ser revisada por um responsável formalmente designado para auditoria.

O revisor deve ser diferente do executor da sessão emergencial.

A revisão deve ocorrer em até dois dias úteis após o encerramento da sessão.

O responsável pela revisão deve avaliar a justificativa, o escopo, as operações realizadas, os recursos acessados e a conformidade com as regras de segurança.

O sistema deve registrar o revisor, a data da revisão, o resultado, eventuais inconformidades e as medidas corretivas determinadas.

### RF-SUP08 - Encerramento e Revogação da Sessão

O sistema deve permitir o encerramento manual de uma sessão de suporte por seu executor autorizado ou por um administrador da plataforma com permissão adequada.

O sistema deve encerrar automaticamente sessões expiradas.

A revogação da permissão do executor, o encerramento do tenant ou a identificação de incidente de segurança deve impedir a continuidade do acesso conforme a política da plataforma.

O encerramento deve impedir novas operações no contexto da sessão, sem eliminar os registros de auditoria já produzidos.

### RF-SUP09 - Proteção de Dados e Restrições Operacionais

O acesso de suporte deve limitar a visualização e a alteração aos dados estritamente necessários para resolver o problema.

O sistema deve impedir:

- acesso a tenants fora do escopo autorizado;
- elevação indevida de privilégios;
- exclusão irrestrita de dados;
- alteração ou exclusão de registros históricos e de auditoria;
- exposição desnecessária de segredos;
- alterações globais da plataforma por meio de uma sessão de suporte restrita a um tenant;
- manutenção de acesso após a expiração ou revogação da sessão.

O suporte deve respeitar as políticas de privacidade, segurança e retenção de dados da plataforma.

### RF31 - Configurações e Integrações Globais

O sistema deve permitir ao Master gerenciar configurações aplicáveis à plataforma como um todo.

O sistema deve permitir administrar integrações globais utilizadas pela plataforma, incluindo serviços externos necessários ao seu funcionamento.

Credenciais, chaves e segredos utilizados pelas integrações não devem ser expostos diretamente por interfaces administrativas comuns.

Alterações em configurações e integrações globais devem ser auditadas.

A alteração de uma configuração global deve respeitar permissões específicas e não deve ser permitida por meio de uma sessão de suporte restrita a um tenant.

### RF32 - Manutenção e Atualizações da Plataforma

O sistema deve permitir ao Master administrar o estado de manutenção da plataforma e registrar manutenções programadas.

O sistema deve permitir informar usuários afetados por períodos de manutenção.

O sistema deve permitir controlar ou acompanhar versões e atualizações da plataforma conforme o processo de implantação adotado.

Operações de manutenção e atualização devem possuir registro de auditoria.

### RF33 - Cobrança da Plataforma

O sistema deve permitir ao Master gerenciar a relação financeira entre a empresa responsável pela plataforma e os condomínios contratantes.

Esse módulo deve ser separado das cobranças condominiais dos moradores.

O sistema deve permitir, conforme o modelo comercial adotado:

- consultar cobranças;
- registrar pagamentos;
- acompanhar pendências;
- relacionar cobranças ao plano contratado.

Os dados financeiros da plataforma devem respeitar as permissões de acesso e possuir histórico de alterações.

---

## Automação e Processamento do Sistema

### RF34 - Expiração Automática de Reservas e Autorizações

O sistema deve processar automaticamente o término de reservas e autorizações de acesso, conforme as regras de validade definidas.

Ao atingir a validade, a autorização deve ser marcada como expirada e não deve permanecer utilizável.

O processamento deve respeitar as condições de sincronização e revogação da infraestrutura.

As operações automáticas devem ser idempotentes, evitando alterações duplicadas quando uma tarefa for executada mais de uma vez.

As tarefas devem validar o estado atual do tenant, da reserva, da credencial, do dispositivo e da autorização antes de executar operações de negócio.

Falhas de processamento devem ser registradas e disponibilizadas para reprocessamento controlado.

### RF35 - Notificações Automáticas

O sistema deve gerar e enviar notificações automaticamente quando eventos configurados ocorrerem.

O mecanismo deve suportar, conforme a funcionalidade:

- confirmação de reserva;
- cancelamento;
- atribuição ou alteração de chamado;
- alertas de telemetria;
- falhas de sincronização;
- eventos de acesso;
- avisos de manutenção;
- notificações financeiras pertinentes;
- outros eventos operacionais relevantes.

Falhas de envio devem ser registradas e, quando aplicável, reprocessadas.

O sistema deve evitar duplicidade indevida de notificações em decorrência de reprocessamentos.

As preferências individuais definidas no RF47 devem ser respeitadas para notificações opcionais, sem desativar comunicações obrigatórias definidas no RF22.

A geração e a entrega devem ser tratadas como etapas distintas, permitindo registrar a existência da notificação mesmo quando sua entrega falhar.

### RF36 - Processamento de Vencimentos e Inadimplência

O sistema deve processar automaticamente os vencimentos das cobranças condominiais.

O sistema deve identificar cobranças vencidas e atualizar a situação de inadimplência das unidades conforme as regras financeiras do tenant.

O processamento automático não deve alterar registros históricos de pagamento de forma incompatível com a auditoria financeira.

O sistema deve evitar processamento duplicado de uma mesma cobrança ou vencimento.

Falhas no processamento devem ser registradas e tratadas por meio de reprocessamento controlado.

### RF37 - Monitoramento de SLA

O sistema deve monitorar automaticamente os prazos de atendimento dos chamados.

O sistema deve identificar chamados próximos do vencimento do SLA e chamados que tenham ultrapassado o prazo.

Quando aplicável, o sistema deve gerar alertas para os responsáveis e registrar a ocorrência do descumprimento do SLA.

As regras de cálculo do SLA devem ser configuráveis e documentadas, incluindo calendários, horários de atendimento e pausas permitidas, quando aplicáveis.

O cálculo deve considerar os estados e as regras de pausa definidos para os chamados, sem presumir que todo estado suspende automaticamente o SLA.

### RF38 - Detecção de Falhas de Dispositivos IoT

O sistema deve monitorar a comunicação dos dispositivos IoT.

O sistema deve identificar, conforme limites configurados:

- ausência prolongada de telemetria;
- falhas recorrentes de comunicação;
- falhas de sincronização;
- indisponibilidade do dispositivo.

Quando aplicável, o sistema deve gerar alerta operacional e permitir o tratamento por usuários responsáveis.

A detecção deve considerar o intervalo esperado de comunicação de cada tipo de dispositivo, evitando classificar como falha um dispositivo cujo comportamento normal não exige comunicação contínua.

O processamento deve evitar a geração repetida de alertas equivalentes para uma mesma falha ativa, conforme as regras de correlação e notificação.

### RF39 - Reprocessamento de Operações com Falha

O sistema deve permitir o reprocessamento controlado de operações assíncronas que falharem.

As tentativas devem possuir limites configuráveis e registro das falhas.

Operações que não puderem ser concluídas após as tentativas previstas devem ser identificadas como falhas definitivas e disponibilizadas para tratamento administrativo ou operacional.

O reprocessamento deve evitar duplicidade de efeitos e respeitar o estado atual do tenant, do dispositivo e da entidade relacionada.

Antes de executar uma operação reprocessada, o sistema deve validar se ela ainda é válida e autorizada. Uma operação pendente não deve ser executada automaticamente quando o tenant ou a entidade relacionada tiver sido inativado ou encerrado.

Operações de reprocessamento iniciadas manualmente devem registrar o executor, a data, o motivo e o resultado.

### RF40 - Gestão de Sessões e Revogação

O sistema deve controlar a validade dos tokens e das sessões de autenticação conforme as políticas definidas no RF02.

Sessões expiradas devem deixar de permitir acesso às operações protegidas.

O sistema deve permitir revogar sessões ou tokens quando necessário por motivos de segurança ou administração.

A revogação deve impedir novas operações conforme a política de validade e verificação de sessão definida no RF02.

O sistema deve registrar eventos relevantes de criação, renovação, encerramento e revogação de sessões.

O encerramento de uma sessão de usuário não deve revogar automaticamente registros históricos de operações já executadas.

### RF41 - Limpeza de Dados Temporários

O sistema deve executar tarefas automáticas de limpeza de dados temporários expirados, respeitando as políticas de retenção e evitando a remoção de registros históricos ou dados sujeitos a obrigação de preservação.

A limpeza deve utilizar critérios explícitos de expiração e possuir registro operacional suficiente para diagnóstico.

A execução deve ser idempotente e não deve remover dados necessários à recuperação, à auditoria ou ao cumprimento de obrigações legais.

Os critérios de limpeza devem distinguir dados temporários, dados operacionais e registros históricos sujeitos a retenção específica.

### RF42 - Continuidade e Backup

O sistema deve executar backups automáticos dos dados necessários à recuperação da plataforma.

Os backups devem possuir política de retenção, proteção contra acesso indevido e mecanismos de verificação de execução.

A restauração de backups deve ser uma operação administrativa controlada e não deve ocorrer automaticamente sem uma política explícita de recuperação.

O plano de continuidade deve definir objetivos de recuperação, periodicidade de testes, responsabilidades e procedimentos para restauração.

Os objetivos de tempo de recuperação (RTO) e de perda máxima aceitável de dados (RPO) devem ser definidos antes da implantação em produção, considerando os recursos e a criticidade da plataforma.

Os procedimentos de restauração devem considerar o isolamento entre tenants, a integridade dos dados e a consistência dos registros financeiros, históricos e de auditoria.

### RF43 - Monitoramento de Serviços da Plataforma

O sistema deve monitorar a disponibilidade dos principais serviços necessários ao funcionamento da plataforma.

Quando detectar indisponibilidade ou falha relevante, deve registrar o evento e disponibilizar alerta aos responsáveis.

O monitoramento pode abranger, conforme a arquitetura:

- API;
- banco de dados;
- broker MQTT;
- processamento assíncrono;
- serviços de integração;
- serviços de armazenamento;
- mecanismos de autenticação;
- canais de notificação.

Os registros de monitoramento devem permitir identificar o componente afetado, o período da falha, a severidade e a recuperação.

Os logs de monitoramento não substituem os registros funcionais de histórico ou auditoria.

### RF44 - Configurações do Condomínio

O sistema deve permitir ao administrador configurar parâmetros operacionais do próprio tenant, respeitando as permissões e os limites da plataforma.

As configurações podem incluir:

- horários de funcionamento;
- regras de reserva;
- antecedência mínima;
- duração máxima;
- regras de cancelamento;
- parâmetros de notificação;
- limites operacionais de dispositivos;
- regras de SLA.

Alterações nas configurações devem registrar o usuário responsável, a data e hora, os valores anteriores e os novos valores.

Alterações em regras de reserva devem ser aplicadas a novas reservas. Reservas já confirmadas devem preservar as condições registradas no momento da confirmação, salvo regra explícita e auditada que determine outra conduta.

As configurações devem ser validadas no servidor e não podem permitir a violação das regras de integridade ou segurança do sistema.

### RF45 - Histórico de Reservas

O sistema deve manter histórico das reservas e de suas alterações, incluindo criação, confirmação, cancelamento, alteração autorizada e encerramento.

O morador deve poder consultar o histórico das próprias reservas.

O administrador deve poder consultar o histórico das reservas do próprio tenant.

O histórico deve preservar a identificação do responsável, a data e hora, os estados anteriores e posteriores e as justificativas pertinentes.

O cancelamento ou a inativação de entidades relacionadas não deve apagar o histórico da reserva.

O histórico deve permitir distinguir alterações realizadas por usuários de alterações realizadas por rotinas automáticas.

### RF46 - Histórico de Ocorrências

O sistema deve manter histórico das alterações relevantes das ocorrências.

O morador deve poder consultar as ocorrências que registrou, respeitando as regras de privacidade e autorização.

Funcionários devem acessar ocorrências conforme sua necessidade operacional e suas permissões.

O administrador deve poder consultar as ocorrências do próprio tenant.

O histórico deve preservar alterações de situação, responsáveis, severidade, comentários e relações com chamados quando aplicável.

O histórico de ocorrência deve permanecer separado do histórico de chamado, ainda que ambos estejam relacionados ao mesmo incidente.

### RF47 - Preferências Individuais de Notificações

O sistema deve permitir que cada usuário configure suas preferências individuais de recebimento de notificações, respeitando as políticas do tenant e as categorias obrigatórias definidas no RF22.

As preferências podem contemplar os canais disponíveis na plataforma, como:

- notificações dentro da aplicação;
- e-mail;
- notificações push, quando suportadas.

O sistema deve permitir configurar preferências por categoria de notificação, quando aplicável.

As preferências individuais devem afetar somente as notificações opcionais.

Notificações obrigatórias de segurança, obrigações legais, eventos críticos de acesso e comunicações necessárias ao fluxo de atendimento de chamados não podem ser desativadas pelo usuário.

O sistema deve preservar as preferências por usuário e tenant, quando aplicável, e impedir que um usuário altere as preferências de outro sem autorização.

A alteração das preferências deve ser validada no servidor.

A desativação de um canal opcional não deve impedir a geração do registro interno da notificação nem eliminar a possibilidade de consulta de eventos obrigatórios dentro da aplicação.