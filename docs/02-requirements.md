# Requisitos do Sistema

## Requisitos Funcionais

### Arquitetura e Autenticação

### RF01 - Arquitetura Multi-tenant

O sistema deve suportar múltiplos condomínios simultaneamente. Todo registro operacional que possua contexto de tenant deve estar associado a um `TenantID`, garantindo o isolamento lógico dos dados entre os condomínios.

O sistema deve utilizar Global Query Filters no Entity Framework Core como mecanismo padrão de filtragem por tenant, complementado por validações de autorização, regras de domínio e constraints de integridade no banco de dados quando aplicável.

Nenhuma operação de leitura ou escrita deve permitir acesso ou associação indevida entre dados pertencentes a tenants distintos.

O `TenantID` utilizado nas operações deve ser obtido de um contexto autenticado e validado no servidor. O sistema não deve confiar exclusivamente no `TenantID` enviado pelo cliente.

As consultas administrativas globais devem ser explicitamente autorizadas e separadas das operações comuns de um tenant.

### RF01.1 - Inativação e Encerramento de Tenant

O preenchimento de `DeletedAt` na entidade `Tenant` representa exclusivamente o encerramento comercial ou contratual do condomínio na plataforma.

O encerramento de um tenant deve:

* bloquear o acesso de todos os usuários vinculados;
* impedir novas operações no tenant;
* impedir novas reservas;
* impedir a abertura e o processamento operacional de novos chamados;
* impedir novas operações de controle de acesso;
* suspender o processamento operacional de telemetria associada ao tenant;
* impedir novas autorizações de acesso e novas sincronizações operacionais;
* preservar os dados necessários à auditoria, à recuperação e ao cumprimento de obrigações legais.

É estritamente vedada a exclusão física ou em cascata dos registros operacionais, financeiros, históricos e de auditoria associados ao tenant, incluindo `CobrancaCondominial`, `HistoricoChamado`, `RegistroAcesso` e `RegistroTelemetria`.

Os dados históricos devem ser preservados de acordo com as políticas de retenção e os requisitos legais aplicáveis.

Mensagens MQTT recebidas após o encerramento do tenant não devem provocar operações de negócio. Quando necessário para diagnóstico, a mensagem deve ser rejeitada ou encaminhada para uma área de quarentena técnica, sem processamento operacional, respeitando limites de retenção e minimização de dados.

A reativação de um tenant deve ser uma operação administrativa explícita, autorizada e auditada. A reativação não deve restaurar automaticamente credenciais, autorizações ou sessões que tenham sido revogadas durante o encerramento.

### RF02 - Autenticação e Controle de Acesso

O sistema deve autenticar usuários por meio de credenciais seguras e emitir tokens JWT para autenticação das requisições protegidas.

O sistema deve aplicar controle de acesso baseado em papéis, RBAC, contemplando:

* `Morador`;
* `Funcionario`;
* `Administrador`;
* `Master`.

O perfil `Master` representa um usuário de nível superior à administração de um tenant específico e pode possuir `TenantID` nulo.

Usuários dos demais papéis devem obrigatoriamente estar associados a um tenant válido.

O sistema deve aplicar autorização no servidor para cada operação protegida, verificando papel, tenant, propriedade do recurso e permissões específicas. Ocultar botões ou páginas no cliente não substitui a autorização no backend.

O sistema deve implementar as seguintes regras de autenticação e sessão:

* emitir tokens de acesso com validade limitada;
* utilizar tokens de atualização, denominados refresh tokens, com armazenamento seguro;
* rotacionar refresh tokens durante sua utilização;
* detectar reutilização indevida de refresh tokens quando tecnicamente aplicável;
* permitir encerramento da sessão atual;
* permitir encerramento de todas as sessões do próprio usuário;
* permitir revogação administrativa de sessões de usuários autorizados;
* invalidar ou impedir a renovação de sessões após inativação da conta, alteração de senha ou incidente de segurança;
* aplicar limitação de tentativas de autenticação e mecanismos de proteção contra abuso;
* registrar eventos relevantes de segurança sem armazenar senhas ou tokens completos nos logs.

Quando for exigida revogação imediata, a aplicação deve utilizar um mecanismo de verificação de sessão, versão de segurança, lista de revogação ou solução equivalente. A simples expiração de um JWT não garante revogação imediata.

O login dos usuários vinculados a condomínios deve identificar o tenant por meio de um identificador público apropriado, como um slug na URL ou outro mecanismo validado pelo servidor. O login de usuários `Master` deve utilizar um fluxo separado, como `/master/login`.

A recuperação de senha deve utilizar um token temporário, de uso único e com expiração. A resposta não deve revelar se um e-mail está cadastrado. Após a redefinição de senha, as sessões existentes devem ser revogadas conforme a política de segurança.

### RF03 - Gestão de Contas e Soft Delete Seletivo

A inativação de entidades cadastrais e operacionais que suportem desativação deve preservar seus registros físicos no banco de dados, utilizando Soft Delete por meio de `DeletedAt`.

Essa política aplica-se, entre outras entidades, a:

* usuários;
* unidades;
* espaços comuns;
* ativos de infraestrutura;
* dispositivos IoT;
* credenciais;
* reservas;
* demais entidades operacionais que possuam histórico associado.

Registros históricos e de auditoria, como `HistoricoChamado`, `RegistroAcesso`, `RegistroTelemetria` e registros de auditoria, não devem utilizar Soft Delete como mecanismo comum de remoção.

Esses registros devem ser tratados conforme suas respectivas políticas de retenção, integridade histórica e obrigações legais.

A inativação de uma entidade deve impedir novas operações incompatíveis com seu estado, sem apagar ou invalidar indevidamente operações anteriores.

---

## Operações e Reservas

### RF04 - Máquina de Estados para Chamados e Histórico Append-Only

Os chamados devem seguir uma máquina de estados definida pelo domínio e devem possuir um tipo compatível com os fluxos de:

* `Manutencao`;
* `AlarmeIoT`.

Os estados operacionais dos chamados devem ser:

* `Aberto`;
* `Atribuido`;
* `EmAndamento`;
* `AguardandoPecaTerceiro`;
* `Resolvido`;
* `Fechado`;
* `Cancelado`.

As transições permitidas devem seguir as seguintes regras:

* `Aberto` para `Atribuido`, quando um administrador atribuir o chamado a um funcionário;
* `Atribuido` para `EmAndamento`, quando o funcionário responsável iniciar o atendimento;
* `EmAndamento` para `AguardandoPecaTerceiro`, quando a execução depender de peça, fornecedor ou terceiro;
* `AguardandoPecaTerceiro` para `EmAndamento`, quando a pendência for resolvida e o atendimento puder continuar;
* `EmAndamento` para `Resolvido`, quando o responsável registrar a solução;
* `Resolvido` para `Fechado`, quando o morador solicitante confirmar o encerramento;
* `Resolvido` para `EmAndamento`, quando o chamado for reaberto por solicitação autorizada, com justificativa;
* `Aberto`, `Atribuido` ou `EmAndamento` para `Cancelado`, conforme as permissões e regras de cancelamento.

As transições não listadas devem ser rejeitadas, exceto quando uma regra de domínio específica for formalmente definida e implementada.

Um chamado nos estados `Fechado` ou `Cancelado` é terminal e não pode ser reaberto ou alterado diretamente. Caso surja um novo problema, deve ser criado outro chamado, com referência ao chamado anterior quando houver relação entre os eventos.

O morador solicitante pode confirmar o fechamento de um chamado resolvido ou solicitar sua reabertura. A reabertura deve registrar a justificativa e retornar o chamado para `EmAndamento`.

O funcionário responsável pode atualizar o chamado dentro dos limites de seu papel e das transições permitidas. O administrador pode executar as operações administrativas autorizadas no tenant, respeitando as regras de auditoria.

O cancelamento de um chamado pode ser realizado pelo solicitante, pelo funcionário responsável quando o chamado estiver atribuído a ele, ou por um administrador do mesmo tenant. O cancelamento deve respeitar os estados permitidos e exigir justificativa não vazia.

Chamados do tipo `AlarmeIoT` devem ser gerados pelo sistema a partir de eventos de infraestrutura quando aplicável. Esses chamados devem utilizar uma identidade técnica de sistema previamente definida para o tenant, com `UsuarioID` válido e não nulo, sem simular a atuação de um usuário humano.

As alterações de estado devem ser registradas em `HistoricoChamado`.

O histórico deve operar estritamente em modo Append-Only, não permitindo alteração ou exclusão lógica de registros históricos por operações comuns.

Cada alteração deve registrar, no mínimo:

* status anterior;
* novo status;
* usuário responsável pela alteração ou identidade técnica do sistema;
* data e hora da alteração;
* comentário ou justificativa, quando exigido;
* origem da alteração, quando aplicável, como usuário, automação ou integração.

### RF05 - Motor de Reservas

O sistema deve permitir reservas de espaços comuns com base em uma grade de horários parametrizável por espaço.

O sistema deve validar:

* disponibilidade do espaço;
* duração permitida;
* antecedência mínima;
* limite máximo de antecedência, quando configurado;
* horários de funcionamento;
* estado operacional do espaço;
* existência de outra reserva válida no mesmo intervalo;
* situação de inadimplência do morador, conforme o RF16;
* permissões e vínculo do morador com o tenant.

O sistema deve bloquear novas reservas quando as regras de inadimplência do tenant determinarem essa restrição.

A validação de disponibilidade deve ocorrer no servidor e ser protegida contra requisições simultâneas. A validação feita apenas na interface não é suficiente.

O banco de dados deve impedir intervalos sobrepostos para o mesmo espaço entre reservas válidas, mesmo quando duas requisições forem processadas simultaneamente.

### RF06 - Cancelamento de Reservas

Reservas podem ser canceladas pelos moradores de acordo com o período mínimo de antecedência configurado para o espaço.

O administrador pode cancelar reservas de moradores do próprio tenant antes do início da reserva, desde que informe uma justificativa obrigatória.

Funcionários não podem cancelar reservas de moradores por padrão. Essa permissão somente poderá ser concedida por uma regra explícita de autorização.

O sistema deve registrar:

* data e hora do cancelamento;
* usuário responsável pelo cancelamento;
* motivo do cancelamento, quando informado ou exigido;
* estado anterior e novo estado da reserva;
* origem do cancelamento, quando aplicável.

Cancelamentos realizados fora das regras permitidas devem ser rejeitados.

O sistema poderá executar cancelamentos automáticos quando previstos pelas regras de negócio ou políticas operacionais do sistema. Cancelamentos automáticos devem identificar a rotina responsável e registrar o motivo.

Quando o administrador cancelar uma reserva de um morador, o sistema deve notificar o titular da reserva e preservar o histórico da operação.

O cancelamento deve liberar a disponibilidade futura do espaço sem apagar o registro da reserva ou de suas alterações anteriores.

### RF07 - Estados Operacionais de Ativos

Espaços comuns e ativos de infraestrutura devem possuir controle de estado operacional independente de seu ciclo de vida cadastral.

Os estados operacionais devem permitir representar:

* `Disponivel`;
* `Interditado`;
* `Manutencao`;
* `Operacional`;
* `Inativo`.

A alteração do estado operacional não deve resultar na exclusão ou alteração indevida do histórico cadastral ou operacional da entidade.

Espaços interditados ou indisponíveis não devem aceitar novas reservas incompatíveis com seu estado.

O sistema deve definir como tratar reservas futuras existentes quando um espaço passar para um estado incompatível com sua utilização. A aplicação dessa regra deve preservar o histórico, registrar a justificativa e notificar os usuários afetados quando aplicável.

### RF08 - Dashboard Administrativo

O sistema deve disponibilizar ao administrador informações operacionais consolidadas, incluindo:

* taxa de ocupação dos espaços;
* reservas;
* chamados por estado;
* chamados dentro ou fora do SLA;
* situação de inadimplência;
* estado operacional da infraestrutura;
* estado dos dispositivos IoT;
* indicadores relevantes de telemetria;
* alertas críticos pendentes.

Quando aplicável, essas informações devem ser atualizadas de forma próxima ao tempo real.

O dashboard deve apresentar somente dados do tenant ao qual o administrador está autorizado.

Os indicadores devem utilizar critérios consistentes e documentados, evitando divergências entre os números exibidos e os registros operacionais.

### RF09 - Aplicações Web e Desktop

O sistema deve disponibilizar interfaces web e desktop conforme os perfis de usuário e as necessidades operacionais.

As aplicações devem consumir uma API compartilhada para as operações de negócio, salvo quando uma necessidade técnica específica justificar outra arquitetura documentada.

A autorização, as regras de domínio, a validação de tenant e as restrições de integridade devem ser aplicadas no backend, independentemente do cliente utilizado.

A aplicação web deve permitir acesso às funcionalidades destinadas aos usuários por meio de navegador compatível.

A aplicação desktop deve disponibilizar as funcionalidades operacionais previstas para seus perfis, especialmente quando forem necessárias para a rotina administrativa ou de atendimento.

As funcionalidades disponíveis em cada cliente devem ser documentadas por perfil e caso de uso. A existência de uma aplicação desktop não deve conceder permissões adicionais por si só.

Ambas as aplicações devem tratar expiração de sessão, erros de comunicação, falhas de autenticação e indisponibilidade da API de maneira controlada.

---

## Integração IoT

### RF-IoT01 - Emissão e Gestão de Credenciais de Acesso

Ao confirmar uma reserva que exija controle de acesso físico, o sistema deve gerar ou associar uma autorização de acesso vinculando:

* reserva;
* credencial;
* dispositivo físico;
* usuário autorizado;
* tenant;
* período de validade.

O modelo deve desacoplar:

* o estado lógico da autorização, composto por `Ativa`, `Expirada` e `Revogada`;
* o estado de entrega ou sincronização com a infraestrutura, composto por `Pendente`, `Sincronizado` e `Falhou`.

Em caso de sincronização, o sistema deve registrar a data da última sincronização.

Em caso de falha, o sistema deve registrar o erro técnico associado à última tentativa de sincronização.

A revogação ou expiração de uma autorização deve impedir sua utilização conforme as políticas de segurança e o estado de sincronização da infraestrutura.

O sistema deve impedir que autorizações sejam criadas ou sincronizadas para usuários, credenciais, dispositivos, reservas ou tenants inativos.

### RF-IoT02 - Telemetria da Infraestrutura

O sistema deve possuir processamento assíncrono para consumir dados de telemetria provenientes dos dispositivos IoT.

Os registros de telemetria devem ser armazenados em modo Append-Only e não devem utilizar Soft Delete como mecanismo comum de exclusão.

Cada dispositivo deve possuir informações suficientes para interpretar sua telemetria, incluindo:

* tipo do dispositivo;
* tipo de leitura;
* unidade de medida correspondente;
* data e hora da medição;
* identificação do dispositivo de origem;
* tenant associado;
* valor medido;
* informações de qualidade ou validade da leitura, quando aplicável.

O sistema deve validar a origem das mensagens, o escopo do tenant, o dispositivo remetente e o formato dos dados recebidos.

Mensagens duplicadas ou repetidas não devem provocar duplicidade indevida de operações de negócio. Quando o protocolo permitir, o sistema deve utilizar identificadores de mensagem, sequências ou mecanismos equivalentes de idempotência.

Os dados de telemetria devem possuir política própria de retenção, conforme definido no RNF10.

### RF-IoT03 - Abertura Autônoma de Chamados e Alertas

Quando dados de telemetria ultrapassarem limites críticos previamente parametrizados, o sistema deve ser capaz de:

1. identificar a condição anormal;
2. registrar o evento;
3. gerar automaticamente um chamado do tipo `AlarmeIoT`, quando aplicável;
4. disponibilizar um alerta operacional aos funcionários e administradores autorizados;
5. correlacionar o evento com o dispositivo, o tipo de alarme, a condição monitorada e o chamado relacionado.

Os alertas em tempo real devem utilizar WebSockets ou mecanismo equivalente de comunicação persistente.

Eventos repetidos referentes à mesma condição anormal não devem gerar indefinidamente novos chamados para o mesmo incidente ativo. O sistema deve correlacionar os eventos e associá-los ao chamado existente quando a regra de negócio determinar que pertencem ao mesmo incidente.

Se o chamado anterior estiver fechado e uma nova condição anormal representar um novo incidente, o sistema deve criar um novo chamado e preservar a relação histórica entre os eventos quando aplicável.

O encerramento de um chamado de alarme deve considerar a normalização da condição monitorada ou uma justificativa operacional registrada.

### RF-IoT04 - Reconhecimento Facial para Controle de Acesso

Em uma versão futura, fora do escopo do MVP, o sistema deverá permitir a integração com um serviço de inteligência artificial em nuvem para reconhecimento facial aplicado ao controle de acesso de moradores autorizados.

A funcionalidade deverá permitir:

* cadastrar ou vincular uma identidade facial a um usuário autorizado;
* enviar a imagem ou representação necessária ao serviço de reconhecimento facial, conforme a arquitetura definida;
* receber o resultado da identificação ou verificação;
* relacionar o resultado à tentativa de acesso;
* autorizar ou negar o acesso conforme as regras de segurança;
* registrar o evento de acesso para auditoria;
* desativar ou remover a associação facial conforme as regras de retenção e os direitos aplicáveis.

O tratamento de dados biométricos deve observar a LGPD, considerando a natureza sensível desses dados, a finalidade específica, a base legal aplicável, a transparência, a necessidade, a segurança e os direitos dos titulares.

O consentimento não deve ser presumido como a única base legal possível nem considerado automaticamente suficiente. A base legal apropriada e a necessidade de avaliação de impacto devem ser analisadas antes da implementação.

A solução deve limitar a coleta e o armazenamento ao estritamente necessário. Sempre que tecnicamente viável, deve-se evitar o armazenamento desnecessário de imagens brutas e preferir mecanismos seguros de representação biométrica.

A integração deve utilizar transmissão criptografada, autenticação entre serviços, controle de acesso restrito e gestão segura de segredos.

A funcionalidade deve definir regras para:

* cadastramento e atualização de dados faciais;
* exclusão ou revogação da associação;
* prazo de retenção;
* acesso administrativo aos dados;
* registro de consultas e operações;
* tratamento de falsos positivos e falsos negativos;
* indisponibilidade do provedor em nuvem;
* contestação de uma decisão de acesso;
* alternativa de autenticação quando o reconhecimento facial não estiver disponível.

Uma decisão automatizada de reconhecimento facial não deve eliminar mecanismos de contingência previamente definidos nem impedir a investigação de uma tentativa de acesso contestada.

O reconhecimento facial deve permanecer desacoplado do modelo atual de credenciais, permitindo coexistência com RFID, senha numérica e outros mecanismos de acesso suportados.

---

## Ocorrências e Chamados

### RF10 - Registro de Ocorrências

O sistema deve permitir que moradores registrem ocorrências relacionadas ao condomínio.

O sistema deve permitir também que o administrador registre ocorrências recebidas por outros canais, como telefone ou atendimento presencial.

Cada ocorrência deve possuir informações suficientes para identificar sua natureza, descrição, data, autor ou origem, situação e severidade, quando aplicável.

O sistema deve preservar o histórico das ocorrências e controlar quem pode consultá-las ou alterá-las.

### RF11 - Abertura de Chamados

O sistema deve permitir que moradores abram chamados para solicitar atendimento ou manutenção.

Cada chamado deve estar associado ao tenant correto e possuir informações suficientes para identificar o solicitante, o tipo de atendimento, a descrição, a data de abertura e o estado inicial.

Quando necessário, o chamado poderá estar associado a uma ocorrência, unidade, ativo, dispositivo ou reserva.

O sistema deve impedir a criação de chamados em tenants encerrados ou por usuários sem permissão.

### RF12 - Atribuição de Chamados

O sistema deve permitir que o administrador atribua um chamado a um funcionário pertencente ao mesmo tenant.

O sistema deve validar se o funcionário está ativo e autorizado a receber o chamado.

A atribuição deve registrar o responsável anterior, quando existir, o novo responsável, o usuário que realizou a alteração e a data e hora da operação.

A alteração deve ser registrada no histórico do chamado e deve gerar notificação aos usuários pertinentes conforme as preferências e as regras de notificação obrigatória.

### RF13 - Gerenciamento do Chamado

O sistema deve permitir que funcionários autorizados gerenciem os chamados atribuídos a eles, respeitando a máquina de estados definida no RF04.

O gerenciamento deve incluir, conforme as permissões:

* iniciar o atendimento;
* registrar comentários;
* atualizar informações operacionais;
* indicar dependência de peça ou terceiro;
* registrar a solução;
* solicitar ou efetuar o cancelamento autorizado;
* acompanhar o histórico do atendimento.

O sistema deve impedir transições de estado não permitidas e registrar as alterações relevantes.

O funcionário não deve poder acessar chamados de outros tenants.

### RF13.1 - Início do Atendimento

Um funcionário autorizado pode mover um chamado de `Atribuido` para `EmAndamento`.

O sistema deve registrar a data e hora de início do atendimento.

### RF13.2 - Aguardando Peça ou Terceiro

Um funcionário autorizado pode mover um chamado de `EmAndamento` para `AguardandoPecaTerceiro` quando houver uma dependência externa que impeça a continuidade.

A mudança deve possuir justificativa ou comentário explicativo.

O retorno para `EmAndamento` deve ser registrado quando a dependência for resolvida.

### RF13.3 - Resolução do Chamado

Um funcionário autorizado pode mover um chamado de `EmAndamento` para `Resolvido`, registrando a solução aplicada e as informações necessárias para compreensão do atendimento.

O sistema deve preservar os registros anteriores.

### RF13.4 - Fechamento do Chamado

O morador solicitante pode confirmar o fechamento de um chamado no estado `Resolvido`, alterando-o para `Fechado`.

O sistema deve registrar a data e hora do fechamento.

Quando o morador não puder realizar a confirmação, qualquer regra alternativa de fechamento deverá ser definida de maneira explícita e auditável, sem permitir fechamento arbitrário por usuários sem autorização.

### RF13.5 - Cancelamento do Chamado

O cancelamento deve respeitar as transições definidas no RF04.

O solicitante, o funcionário responsável quando autorizado e o administrador do tenant podem cancelar um chamado nos estados permitidos.

O cancelamento deve exigir justificativa não vazia, registrar o responsável e preservar o histórico.

Chamados em `Resolvido`, `Fechado` ou `Cancelado` não devem ser cancelados novamente.

### RF13.6 - Reabertura do Chamado

Um chamado em `Resolvido` pode retornar para `EmAndamento` mediante solicitação autorizada do morador ou ação administrativa permitida.

A reabertura deve exigir justificativa, registrar o usuário responsável, a data e hora e o motivo.

Chamados em `Fechado` ou `Cancelado` não podem ser reabertos diretamente.

### RF13.7 - Permissões do Funcionário

O funcionário deve poder gerenciar somente os chamados autorizados pelo seu perfil e escopo de atendimento.

Por padrão, o funcionário pode alterar os chamados atribuídos a ele. A visualização de outros chamados deve seguir as permissões definidas para a função.

### RF13.8 - Histórico e Comentários

Comentários e alterações relevantes devem ser registrados de forma que seja possível reconstruir a sequência de atendimento.

O sistema deve diferenciar comentários operacionais de alterações formais de estado.

Registros históricos não devem ser apagados ou alterados por operações comuns.

### RF13.9 - Regras de Cancelamento e Justificativa

O cancelamento de chamados deve respeitar os estados e os papéis autorizados definidos no RF04.

A justificativa deve ser obrigatória, não podendo conter apenas espaços em branco.

O sistema deve registrar o estado anterior, o novo estado, o autor do cancelamento, a data e hora e a justificativa.

O cancelamento deve gerar as notificações previstas nas regras de negócio e não deve remover o histórico associado.

### RF14 - Histórico de Chamados

O sistema deve manter o histórico de alterações do chamado.

O morador deve poder visualizar o histórico dos próprios chamados.

O funcionário deve poder visualizar o histórico dos chamados que estejam dentro de seu escopo de autorização.

O administrador deve poder visualizar o histórico dos chamados do próprio tenant.

Os registros devem ser apresentados em ordem cronológica e preservar informações necessárias para auditoria e acompanhamento.

---

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

Alterações em cobranças e pagamentos devem ser auditadas. O histórico financeiro não deve ser apagado por inativação de unidades ou encerramento do tenant.

### RF17 - Gestão de Unidades e Vínculos

O sistema deve permitir ao administrador cadastrar, consultar, editar e inativar unidades pertencentes ao próprio tenant.

O sistema deve permitir associar usuários às unidades e administrar os tipos de vínculo:

* `Proprietario`;
* `Inquilino`;
* `Dependente`.

O sistema deve impedir vínculos entre entidades pertencentes a tenants distintos.

A alteração ou o encerramento de uma unidade não deve remover seu histórico de vínculos ou operações anteriores.

O sistema deve validar a consistência dos vínculos e impedir associações duplicadas incompatíveis com as regras de integridade.

### RF18 - Gestão de Espaços Comuns

O sistema deve permitir ao administrador cadastrar, consultar, editar, configurar e inativar espaços comuns.

Cada espaço deve possuir informações necessárias para sua utilização e reserva, incluindo:

* capacidade;
* duração máxima;
* antecedência mínima;
* horários de funcionamento;
* regras de funcionamento;
* estado operacional;
* regras de cancelamento;
* exigências de controle de acesso, quando aplicáveis.

As configurações do espaço devem ser utilizadas pelo motor de reservas.

Um espaço indisponível ou interditado não deve aceitar novas reservas incompatíveis com seu estado.

Alterações nas regras de reserva devem afetar novas reservas, sem modificar retroativamente as condições das reservas existentes, salvo quando uma regra explícita de negócio exigir outra conduta.

### RF19 - Gestão de Ativos e Infraestrutura

O sistema deve permitir ao administrador cadastrar, consultar, editar e inativar ativos e elementos de infraestrutura do condomínio.

Os ativos devem possuir estado operacional independente de seu ciclo de vida cadastral, conforme o RF07.

O sistema deve permitir consultar informações operacionais da infraestrutura e associar dispositivos IoT aos elementos físicos correspondentes quando aplicável.

A alteração do estado de um ativo deve preservar seu histórico operacional.

### RF20 - Gestão de Dispositivos IoT

O sistema deve permitir ao administrador cadastrar, consultar e gerenciar dispositivos IoT pertencentes ao próprio tenant.

Cada dispositivo deve possuir identificação única e informações necessárias para sua operação, incluindo tipo, identificação física, estado de comunicação e associação à infraestrutura quando aplicável.

O sistema deve permitir inativar dispositivos sem excluir seus registros históricos de telemetria, acesso e auditoria.

O sistema deve registrar informações necessárias para identificar falhas de comunicação e o estado operacional dos dispositivos.

O sistema deve impedir a associação de um dispositivo a entidades de outro tenant.

### RF21 - Gestão de Ocorrências

O sistema deve permitir que usuários autorizados registrem, consultem e gerenciem ocorrências conforme suas permissões.

O administrador deve poder registrar ocorrências recebidas por canais externos e gerenciar seu ciclo de tratamento.

As ocorrências devem possuir informações suficientes para identificar sua natureza, situação, severidade, responsável e histórico de alterações quando aplicável.

O sistema deve preservar o histórico das ocorrências e impedir alterações incompatíveis com suas regras de integridade.

O acesso às ocorrências deve respeitar o princípio do menor privilégio. Moradores devem visualizar somente as ocorrências que registraram ou aquelas às quais possuam autorização explícita.

### RF22 - Gestão de Notificações

O sistema deve permitir a geração, consulta e gerenciamento de notificações direcionadas aos usuários.

As notificações devem poder estar associadas a eventos operacionais, administrativos ou de segurança, como:

* confirmação ou cancelamento de reserva;
* alteração de chamado;
* atribuição de chamado;
* alertas de infraestrutura;
* alertas críticos de telemetria;
* falhas de sincronização;
* eventos de acesso relevantes;
* comunicados administrativos;
* alterações relevantes em cobranças, quando aplicável.

O sistema deve permitir que administradores enviem notificações aos usuários do próprio tenant.

O sistema deve controlar o estado de processamento e entrega das notificações quando necessário.

As categorias de notificação obrigatória devem incluir:

* `security`, para eventos relevantes de segurança;
* `legal`, para comunicações exigidas por obrigação legal ou política aplicável;
* `ticket`, para comunicações necessárias ao fluxo de atendimento de chamados;
* eventos críticos de controle de acesso e infraestrutura classificados como segurança.

Notificações obrigatórias não podem ser desativadas por preferências individuais.

O cancelamento administrativo de uma reserva deve gerar notificação ao titular da reserva.

A entrega de uma notificação deve ser registrada conforme o canal utilizado e as informações técnicas necessárias.

---

## Controle de Acesso e Auditoria

### RF23 - Gestão de Credenciais e Autorizações de Acesso

O sistema deve permitir ao administrador gerenciar credenciais de acesso associadas aos usuários, conforme os tipos de credencial suportados.

Os tipos de credencial podem incluir:

* `TagRFID`;
* `SenhaNumerica`;
* `Biometria`, quando implementada e autorizada.

O sistema deve permitir consultar, criar, alterar, revogar e inativar credenciais conforme as permissões definidas.

As autorizações de acesso devem ser distintas das credenciais físicas ou lógicas.

O sistema deve permitir administrar autorizações de acesso, incluindo validade, associação ao usuário, reserva, dispositivo e situação lógica.

A revogação ou expiração de uma autorização deve impedir sua utilização conforme as políticas de segurança.

O sistema deve preservar o histórico de emissão, sincronização, alteração, expiração e revogação de credenciais e autorizações.

### RF24 - Registro e Consulta de Acessos

O sistema deve registrar eventos de acesso físico, incluindo, quando aplicável:

* usuário ou credencial utilizada;
* dispositivo responsável;
* data e hora;
* tenant;
* resultado da tentativa;
* motivo de negação;
* autorização ou reserva relacionada;
* informações necessárias para auditoria.

O administrador deve poder consultar os registros de acesso do próprio tenant.

O acesso aos registros deve respeitar as permissões e a necessidade operacional.

Os registros de acesso devem ser históricos e não devem ser alterados ou excluídos por operações administrativas comuns.

### RF25 - Auditoria de Operações

O sistema deve registrar automaticamente eventos relevantes de segurança, administração e operação para fins de auditoria.

Os registros devem permitir identificar, quando aplicável:

* usuário responsável ou identidade técnica;
* tenant;
* operação executada;
* entidade afetada;
* data e hora;
* resultado da operação;
* justificativa;
* valores anteriores e posteriores, quando necessários;
* origem da operação, como interface, API, automação ou integração.

Operações administrativas sensíveis, incluindo alterações de permissões, encerramento de tenants, cancelamentos administrativos de reservas e acesso de suporte a dados de condomínios, devem ser auditáveis.

Os registros de auditoria devem possuir integridade histórica e não devem ser alterados ou excluídos por usuários comuns.

Os registros devem evitar o armazenamento de senhas, tokens completos, segredos ou dados pessoais desnecessários.

---

## Administração da Plataforma

### RF26 - Administração de Condomínios pela Plataforma

O sistema deve permitir ao usuário `Master` autorizado cadastrar, consultar, editar, encerrar e reativar condomínios na plataforma.

O encerramento de um condomínio deve obedecer ao RF01.1 e não deve provocar exclusão física de dados históricos, financeiros ou de auditoria.

O sistema deve registrar o histórico das alterações administrativas realizadas sobre cada tenant.

A reativação deve exigir autorização e registro de auditoria.

### RF27 - Gestão de Usuários Master e Permissões

O sistema deve permitir que usuários `Master` autorizados gerenciem outros usuários `Master`.

O sistema deve permitir definir permissões aplicáveis aos usuários `Master`, respeitando uma política de autoridade e evitando que uma configuração deixe a plataforma sem um usuário Master ativo.

Um usuário Master não deve poder realizar operações incompatíveis com suas próprias permissões.

Operações de criação, alteração, inativação e mudança de permissões de usuários Master devem ser auditadas.

Permissões críticas devem ser concedidas somente a usuários autorizados e não devem ser ampliadas automaticamente por meio de acesso de suporte.

### RF28 - Dashboard e Monitoramento da Plataforma

O sistema deve disponibilizar ao Master um dashboard com informações consolidadas da plataforma, incluindo, quando aplicável:

* quantidade de tenants ativos e encerrados;
* quantidade de usuários;
* consumo de recursos;
* estado dos serviços;
* indicadores de integração;
* alertas relevantes;
* informações de utilização da plataforma;
* falhas operacionais relevantes.

O sistema deve permitir consultar a saúde operacional dos principais componentes da plataforma.

Informações de tenants diferentes devem ser agregadas de forma controlada. A consulta de dados individualizados de um tenant deve exigir permissão específica.

### RF29 - Planos, Limites e Consumo de Recursos

O sistema deve permitir ao Master cadastrar e gerenciar planos comerciais e limites de utilização da plataforma.

Os limites podem contemplar, conforme a definição comercial:

* quantidade de usuários;
* quantidade de dispositivos IoT;
* volume de telemetria;
* armazenamento;
* outros recursos controlados pela plataforma.

O sistema deve permitir consultar o consumo de recursos de cada tenant e identificar situações próximas ou superiores aos limites definidos.

A aplicação de limites deve possuir comportamento definido para cada tipo de recurso, evitando bloqueios inesperados de operações críticas de segurança ou preservação de dados.

### RF30 - Suporte da Plataforma

O sistema deve permitir ao Master consultar, criar, acompanhar e atualizar solicitações de suporte relacionadas ao funcionamento da plataforma.

As solicitações de suporte devem ser separadas dos chamados operacionais de manutenção dos condomínios.

O sistema deve permitir que usuários Master autorizados acessem informações necessárias de um tenant para investigar problemas de suporte, respeitando o escopo mínimo de acesso, a finalidade declarada, o prazo da sessão e a auditoria.

O acesso de suporte não deve conceder acesso irrestrito a todos os tenants ou permissões administrativas permanentes.

#### RF-SUP01 - Autorização de Acesso de Suporte

O sistema deve permitir que um usuário Master com permissão específica de suporte inicie uma sessão de suporte para um tenant autorizado.

A sessão normal de suporte não exige aprovação prévia de outro Master, desde que o executor possua a permissão de suporte necessária e cumpra todas as condições estabelecidas neste requisito.

A autorização deve ser validada no servidor antes do início da sessão.

O sistema deve registrar o solicitante, o tenant, a finalidade, o escopo autorizado, o resultado da autorização e o horário de início.

#### RF-SUP02 - Escopo Mínimo e Lista de Permissões

Cada sessão de suporte deve possuir um escopo explícito e limitado.

As operações permitidas devem ser selecionadas a partir de uma lista de permissões controlada. O sistema deve bloquear operações que não estejam explicitamente autorizadas.

A lista de operações de suporte pode contemplar:

* `read`, para leitura dos dados necessários ao diagnóstico;
* `correct`, para correções de dados autorizadas e delimitadas;
* `config`, para alterações de configurações específicas do tenant;
* `sessions`, para consulta ou revogação de sessões conforme a autorização concedida;
* `reprocess`, para reprocessamento controlado de operações assíncronas;
* `access`, para diagnóstico de dispositivos e autorizações de acesso;
* `financial`, para consulta ou correção financeira dentro do escopo autorizado;
* `permissions`, para consulta ou alteração de permissões somente quando expressamente autorizada.

A permissão `permissions` deve ser considerada crítica, exigir autorização específica e produzir auditoria detalhada.

O sistema deve adotar o princípio de negação por padrão. Qualquer operação não autorizada de maneira explícita deve ser bloqueada.

O acesso de suporte não deve permitir escalada de privilégios, acesso a outro tenant, exclusão irrestrita de dados, remoção de registros históricos ou alterações globais da plataforma.

#### RF-SUP03 - Duração e Expiração da Sessão de Suporte

Toda sessão de suporte deve possuir limite de duração, tenant de destino e escopo de operações.

A sessão normal deve ter duração máxima de 60 minutos por autorização, salvo limite menor definido pela política da plataforma.

A continuidade após o vencimento deve exigir nova autorização e novo registro de auditoria.

O encerramento da sessão deve invalidar imediatamente o contexto de suporte para novas operações.

O sistema deve registrar o início, o término, a expiração e o motivo do encerramento da sessão.

#### RF-SUP04 - Justificativa e Finalidade

Antes de iniciar uma sessão de suporte, o usuário Master deve informar uma justificativa não vazia e uma finalidade compatível com a solicitação de suporte.

A justificativa deve ser específica o suficiente para permitir auditoria posterior.

O sistema deve impedir o início da sessão quando a justificativa estiver ausente ou contiver apenas espaços em branco.

A justificativa, o tenant de destino, o executor e o escopo solicitado devem ser registrados.

#### RF-SUP05 - Auditoria das Operações de Suporte

O sistema deve registrar todas as operações executadas durante uma sessão de suporte.

A auditoria deve permitir identificar:

* usuário Master executor;
* tenant acessado;
* identificador da sessão;
* justificativa;
* autorização concedida;
* escopo autorizado;
* data e hora de cada operação;
* recurso consultado ou alterado;
* resultado da operação;
* valores anteriores e posteriores, quando aplicável;
* motivo de falha ou bloqueio, quando aplicável.

A auditoria não deve armazenar senhas, tokens completos ou segredos. Dados pessoais devem ser registrados somente quando necessários à finalidade de auditoria.

Os registros de auditoria devem ser protegidos contra alteração ou exclusão pelo executor da sessão.

#### RF-SUP06 - Acesso Emergencial de Suporte

O sistema deve oferecer um fluxo separado de acesso emergencial para situações críticas em que a operação normal de suporte não seja suficiente.

O acesso emergencial deve exigir justificativa específica, identificação do incidente, tenant de destino e escopo solicitado.

O sistema deve registrar o caráter emergencial da sessão e as permissões concedidas.

O acesso emergencial deve possuir duração limitada e não deve permitir acesso irrestrito ou permanente.

A sessão deve gerar um evento de auditoria prioritário para revisão posterior.

#### RF-SUP07 - Revisão Posterior do Acesso Emergencial

Toda sessão emergencial deve ser revisada por um responsável formalmente designado para auditoria.

O revisor deve ser diferente do executor da sessão emergencial.

A revisão deve ocorrer em até dois dias úteis após o encerramento da sessão.

O responsável pela revisão deve avaliar a justificativa, o escopo, as operações realizadas, os recursos acessados e a conformidade com as regras de segurança.

O sistema deve registrar o revisor, a data da revisão, o resultado, eventuais inconformidades e as medidas corretivas determinadas.

#### RF-SUP08 - Encerramento e Revogação da Sessão

O sistema deve permitir o encerramento manual de uma sessão de suporte por seu executor autorizado ou por um administrador da plataforma com permissão adequada.

O sistema deve encerrar automaticamente sessões expiradas.

A revogação da permissão do executor, o encerramento do tenant ou a identificação de incidente de segurança deve impedir a continuidade do acesso conforme a política da plataforma.

O encerramento deve impedir novas operações no contexto da sessão, sem eliminar os registros de auditoria já produzidos.

#### RF-SUP09 - Proteção de Dados e Restrições Operacionais

O acesso de suporte deve limitar a visualização e a alteração aos dados estritamente necessários para resolver o problema.

O sistema deve impedir:

* acesso a tenants fora do escopo autorizado;
* elevação indevida de privilégios;
* exclusão irrestrita de dados;
* alteração ou exclusão de registros históricos e de auditoria;
* exposição desnecessária de segredos;
* alterações globais da plataforma por meio de uma sessão de suporte de tenant;
* manutenção de acesso após a expiração ou revogação da sessão.

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

* consultar cobranças;
* registrar pagamentos;
* acompanhar pendências;
* relacionar cobranças ao plano contratado.

Os dados financeiros da plataforma devem respeitar as permissões de acesso e possuir histórico de alterações.

---

## Automação e Processamento do Sistema

### RF34 - Expiração Automática de Reservas e Autorizações

O sistema deve processar automaticamente o término de reservas e autorizações de acesso.

Ao atingir a validade, a autorização deve ser marcada como expirada e não deve permanecer utilizável.

O processamento deve respeitar as condições de sincronização e revogação da infraestrutura.

As operações automáticas devem ser idempotentes, evitando alterações duplicadas quando uma tarefa for executada mais de uma vez.

Falhas de processamento devem ser registradas e disponibilizadas para reprocessamento controlado.

### RF35 - Notificações Automáticas

O sistema deve gerar e enviar notificações automaticamente quando eventos configurados ocorrerem.

O mecanismo deve suportar, conforme a funcionalidade:

* confirmação de reserva;
* cancelamento;
* atribuição ou alteração de chamado;
* alertas de telemetria;
* falhas de sincronização;
* eventos de acesso;
* avisos de manutenção;
* notificações financeiras pertinentes;
* outros eventos operacionais relevantes.

Falhas de envio devem ser registradas e, quando aplicável, reprocessadas.

O sistema deve evitar duplicidade indevida de notificações em decorrência de reprocessamentos.

As preferências individuais definidas no RF47 devem ser respeitadas para notificações opcionais, sem desativar comunicações obrigatórias.

### RF36 - Processamento de Vencimentos e Inadimplência

O sistema deve processar automaticamente os vencimentos das cobranças condominiais.

O sistema deve identificar cobranças vencidas e atualizar a situação de inadimplência das unidades conforme as regras financeiras do tenant.

O processamento automático não deve alterar registros históricos de pagamento de forma incompatível com a auditoria financeira.

Falhas no processamento devem ser registradas e tratadas por meio de reprocessamento controlado.

### RF37 - Monitoramento de SLA

O sistema deve monitorar automaticamente os prazos de atendimento dos chamados.

O sistema deve identificar chamados próximos do vencimento do SLA e chamados que tenham ultrapassado o prazo.

Quando aplicável, o sistema deve gerar alertas para os responsáveis e registrar a ocorrência do descumprimento do SLA.

As regras de cálculo do SLA devem ser configuráveis e documentadas, incluindo calendários, horários de atendimento e pausas permitidas, quando aplicáveis.

### RF38 - Detecção de Falhas de Dispositivos IoT

O sistema deve monitorar a comunicação dos dispositivos IoT.

O sistema deve identificar, conforme limites configurados:

* ausência prolongada de telemetria;
* falhas recorrentes de comunicação;
* falhas de sincronização;
* indisponibilidade do dispositivo.

Quando aplicável, o sistema deve gerar alerta operacional e permitir o tratamento por usuários responsáveis.

A detecção deve considerar o intervalo esperado de comunicação de cada tipo de dispositivo, evitando classificar como falha um dispositivo cujo comportamento normal não exige comunicação contínua.

### RF39 - Reprocessamento de Operações com Falha

O sistema deve permitir o reprocessamento controlado de operações assíncronas que falharem.

As tentativas devem possuir limites e registro de falhas.

Operações que não puderem ser concluídas após as tentativas previstas devem ser identificadas como falhas definitivas e disponibilizadas para tratamento administrativo ou operacional.

O reprocessamento deve evitar duplicidade de efeitos e respeitar o estado atual do tenant, do dispositivo e da entidade relacionada.

Operações de reprocessamento devem registrar o executor, a data, o motivo e o resultado quando iniciadas manualmente.

### RF40 - Gestão de Sessões e Revogação

O sistema deve controlar a validade dos tokens e sessões de autenticação.

Sessões expiradas devem deixar de permitir acesso às operações protegidas.

O sistema deve permitir revogar sessões ou tokens quando necessário por motivos de segurança ou administração.

A revogação deve impedir novas operações conforme a política de validade e verificação de sessão definida no RF02.

O sistema deve registrar eventos relevantes de criação, renovação, encerramento e revogação de sessões.

### RF41 - Limpeza de Dados Temporários

O sistema deve executar tarefas automáticas de limpeza de dados temporários expirados, respeitando as políticas de retenção e evitando a remoção de registros históricos ou dados sujeitos a obrigação de preservação.

A limpeza deve utilizar critérios explícitos de expiração e possuir registro operacional suficiente para diagnóstico.

A execução deve ser idempotente e não deve remover dados necessários à recuperação, à auditoria ou ao cumprimento de obrigações legais.

### RF42 - Continuidade e Backup

O sistema deve executar backups automáticos dos dados necessários à recuperação da plataforma.

Os backups devem possuir política de retenção, proteção contra acesso indevido e mecanismos de verificação de execução.

A restauração de backups deve ser uma operação administrativa controlada e não deve ocorrer automaticamente sem uma política explícita de recuperação.

O plano de continuidade deve definir objetivos de recuperação, periodicidade de testes, responsabilidades e procedimentos para restauração.

Os objetivos de tempo de recuperação e de perda máxima aceitável de dados devem ser definidos antes da implantação em produção, considerando os recursos e a criticidade da plataforma.

### RF43 - Monitoramento de Serviços da Plataforma

O sistema deve monitorar a disponibilidade dos principais serviços necessários ao funcionamento da plataforma.

Quando detectar indisponibilidade ou falha relevante, deve registrar o evento e disponibilizar alerta aos responsáveis.

O monitoramento pode abranger, conforme a arquitetura:

* API;
* banco de dados;
* broker MQTT;
* processamento assíncrono;
* serviços de integração;
* serviços de armazenamento;
* mecanismos de autenticação;
* canais de notificação.

Os registros de monitoramento devem permitir identificar o componente afetado, o período da falha, a severidade e a recuperação.

Os logs de monitoramento não substituem os registros funcionais de histórico ou auditoria.

### RF44 - Configurações do Condomínio

O sistema deve permitir ao administrador configurar parâmetros operacionais do próprio tenant, respeitando as permissões e os limites da plataforma.

As configurações podem incluir:

* horários de funcionamento;
* regras de reserva;
* antecedência mínima;
* duração máxima;
* regras de cancelamento;
* parâmetros de notificação;
* limites operacionais de dispositivos;
* regras de SLA.

Alterações nas configurações devem registrar o usuário responsável, a data e hora, os valores anteriores e os novos valores.

Alterações em regras de reserva devem ser aplicadas a novas reservas. Reservas já confirmadas devem preservar as condições registradas no momento de sua criação, salvo regra explícita e auditada que determine outra conduta.

### RF45 - Histórico de Reservas

O sistema deve manter histórico das reservas e de suas alterações, incluindo criação, confirmação, cancelamento, alteração autorizada e encerramento.

O morador deve poder consultar o histórico das próprias reservas.

O administrador deve poder consultar o histórico das reservas do próprio tenant.

O histórico deve preservar a identificação do responsável, a data e hora, os estados anteriores e posteriores e as justificativas pertinentes.

O cancelamento ou a inativação de entidades relacionadas não deve apagar o histórico da reserva.

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

* notificações dentro da aplicação;
* e-mail;
* notificações push, quando suportadas.

O sistema deve permitir configurar preferências por categoria de notificação, quando aplicável.

As preferências individuais devem afetar somente as notificações opcionais.

Notificações obrigatórias de segurança, obrigações legais, eventos críticos de acesso e comunicações necessárias ao fluxo de atendimento de chamados não podem ser desativadas pelo usuário.

O sistema deve preservar as preferências por usuário e tenant, quando aplicável, e impedir que um usuário altere as preferências de outro sem autorização.

A alteração das preferências deve ser validada no servidor.

A desativação de um canal opcional não deve impedir a geração do registro interno da notificação nem eliminar a possibilidade de consulta de eventos obrigatórios dentro da aplicação.

# Requisitos Não-Funcionais

## Desempenho e Processamento

### RNF01 - Tempo de Resposta

As APIs de backend devem apresentar tempo de processamento inferior a 200 milissegundos no percentil 95 (P95) para operações comuns, exceto para operações explicitamente classificadas como pesadas, como relatórios extensos, exportações e processos analíticos.

A medição deve considerar o tempo de processamento da API, incluindo as operações internas necessárias para atender à requisição, mas excluindo indisponibilidade e latência de sistemas externos fora do controle da aplicação, quando tecnicamente possível isolar esses fatores.

O indicador P95 deve ser calculado sobre uma janela de medição representativa e deve ser acompanhado por monitoramento e testes de desempenho.

As operações classificadas como pesadas devem possuir metas de desempenho próprias, definidas conforme sua finalidade e volume de dados.

**Critérios de validação de desempenho**

A meta de latência P95 inferior a 200 ms para operações comuns da API deve ser validada por meio de testes de carga, considerando uma carga representativa do uso esperado da plataforma.

Os testes devem registrar a latência das requisições, a taxa de erros e a utilização dos recursos envolvidos. Operações pesadas, como relatórios extensos e processamento de grandes volumes de telemetria, devem ser avaliadas separadamente.

A quantidade de usuários simultâneos e a taxa de requisições utilizadas nos testes devem ser definidas antes da validação de desempenho, conforme a capacidade e o uso esperado do sistema.


### RNF02 - Processamento Assíncrono

O backend deve utilizar programação assíncrona baseada em `Task` e `async/await` para operações de entrada e saída, incluindo consultas ao banco de dados, chamadas de rede, comunicação com serviços externos e mensageria, quando suportado pelas bibliotecas utilizadas.

O processamento assíncrono deve evitar o bloqueio desnecessário de threads durante períodos de alta demanda ou espera por recursos externos.

Operações intensivas em CPU devem ser tratadas de acordo com sua natureza, evitando o uso indiscriminado de processamento assíncrono quando não houver benefício técnico.

O sistema deve evitar chamadas síncronas bloqueantes desnecessárias em fluxos de execução assíncronos.

### RNF03 - Cancelamento de Operações e Timeouts

As chamadas externas, integrações e operações potencialmente demoradas devem aceitar e propagar `CancellationToken` sempre que houver suporte técnico.

Operações sujeitas à espera por banco de dados, serviços externos, rede ou mensageria devem possuir políticas de timeout compatíveis com sua criticidade.

Os timeouts devem impedir esperas indefinidas e permitir a liberação adequada dos recursos utilizados.

O cancelamento de uma requisição não deve ser interpretado automaticamente como garantia de que uma operação já confirmada no banco de dados ou em um sistema externo foi revertida.

Operações que possam produzir efeitos persistentes devem utilizar transações, idempotência ou mecanismos de consulta de estado para determinar seu resultado após cancelamentos, interrupções ou falhas de comunicação.

Os limites de timeout devem ser definidos individualmente para cada categoria de operação.

---

## Concorrência e Integridade

### RNF04 - Controle de Concorrência e Prevenção de Double-Booking

O sistema deve implementar mecanismos de controle de concorrência para evitar inconsistências causadas por operações simultâneas.

A entidade `Reserva` deve utilizar controle de concorrência otimista, por meio de `RowVersion` ou mecanismo equivalente compatível com o banco de dados utilizado, para detectar alterações concorrentes incompatíveis.

No PostgreSQL, a implementação deve utilizar um mecanismo compatível com o banco, como uma coluna de versão atualizada pela aplicação ou por mecanismo controlado, quando necessário. O tipo `rowversion` do SQL Server não deve ser presumido como recurso nativo do PostgreSQL.

Além do controle de concorrência otimista, o banco de dados deve impedir que duas reservas válidas do mesmo espaço possuam intervalos de tempo sobrepostos.

Essa proteção deve funcionar mesmo quando duas ou mais requisições tentarem criar reservas conflitantes simultaneamente.

A integridade deve ser garantida no banco de dados por uma constraint de exclusão sobre intervalos de tempo, quando aplicável, ou por outro mecanismo transacional equivalente que ofereça a mesma garantia.

A restrição deve considerar os estados da reserva que efetivamente ocupam o espaço, desconsiderando estados que, conforme as regras de negócio, não bloqueiem a disponibilidade.

Uma tentativa de criação ou alteração de reserva conflitante deve falhar de forma controlada, sem produzir duplicidade de ocupação.

A API deve retornar uma resposta apropriada ao usuário, informando que o horário deixou de estar disponível ou que ocorreu um conflito de concorrência.

---

## Segurança e IoT

### RNF05 - Segurança de Senhas

As senhas dos usuários não devem ser armazenadas em texto puro, criptografia reversível ou formato que permita sua recuperação direta.

O sistema deve utilizar algoritmos modernos de derivação de senha com salt individual, como:

* Argon2id;
* bcrypt.

Os parâmetros de custo devem ser configuráveis e ajustados conforme as recomendações de segurança vigentes e os recursos disponíveis.

O sistema deve permitir a evolução dos parâmetros de hashing e a migração gradual para algoritmos mais fortes quando necessário.

Senhas, hashes de tokens de recuperação, tokens completos e segredos de autenticação não devem ser expostos em logs ou respostas de erro.

### RNF06 - Integridade Física e Segurança MQTT

O sistema deve garantir a unicidade global do endereço físico `MacAddress` dos dispositivos IoT cadastrados.

Os tópicos MQTT devem possuir estrutura hierárquica e escopo por tenant, impedindo colisões e associações indevidas entre dispositivos de condomínios diferentes.

A convenção de tópicos deve seguir um padrão consistente, como:

`tenant/{tenantId}/devices/{deviceId}/...`

O broker MQTT deve aplicar autenticação e autorização para publicação e assinatura de mensagens.

Cada dispositivo deve utilizar uma identidade própria ou mecanismo equivalente de autenticação individualizada, com permissões limitadas aos tópicos necessários para seu funcionamento.

Um dispositivo não deve conseguir publicar ou consumir mensagens de outro dispositivo ou tenant sem autorização explícita.

O backend deve validar a identidade do dispositivo, o tenant associado e a consistência das mensagens recebidas. O sistema não deve confiar exclusivamente no `tenantId` ou `deviceId` informado no tópico ou no conteúdo da mensagem.

As credenciais de comunicação devem ser armazenadas de forma segura e possuir procedimento de revogação e substituição quando necessário.

### RNF07 - Edge Computing e Fallback IoT

Os dispositivos responsáveis pelo controle de acesso físico devem possuir capacidade de armazenamento local seguro para operação temporariamente desconectada, quando suportado pelo hardware.

Em caso de indisponibilidade da conexão com a plataforma, o dispositivo deve conseguir validar localmente autorizações previamente sincronizadas para as próximas 24 horas, respeitando as regras de segurança definidas para a infraestrutura.

A validade máxima dos dados armazenados localmente deve ser configurável conforme o tipo de dispositivo e a criticidade do acesso.

A política de armazenamento e sincronização local deve definir:

* expiração das autorizações;
* sincronização com a plataforma;
* atualização das permissões;
* tratamento de revogações;
* integridade e proteção dos dados armazenados;
* comportamento durante indisponibilidade da rede;
* recuperação após reconexão;
* tratamento de conflitos entre o estado local e o estado central.

O sistema deve considerar que uma autorização revogada na plataforma pode permanecer disponível em um dispositivo desconectado até que a revogação seja sincronizada ou que a autorização expire.

Esse risco deve ser tratado por meio de limites de validade, políticas locais de segurança e procedimentos de sincronização.

Para acessos classificados como críticos, a política deve definir explicitamente se o acesso será permitido ou negado durante a indisponibilidade da plataforma.

### RNF08 - Criptografia em Trânsito

Todo o tráfego HTTP da aplicação deve utilizar HTTPS com TLS 1.2 ou superior.

As comunicações MQTT devem utilizar transporte seguro, preferencialmente MQTT sobre TLS, com autenticação apropriada dos dispositivos.

As integrações externas devem utilizar canais criptografados e validar certificados conforme as políticas de segurança adotadas.

Credenciais e segredos não devem ser transmitidos em parâmetros de URL ou incluídos em mensagens de erro e logs.

---

## Conformidade e Operação

### RNF09 - Separação de Configurações e Segredos

Credenciais de banco de dados, chaves JWT, credenciais de serviços externos, certificados privados e configurações sensíveis do broker MQTT não devem ser armazenados diretamente no código-fonte ou em repositórios de versionamento.

Essas configurações devem ser fornecidas por mecanismos externos, como variáveis de ambiente ou serviços de gerenciamento de segredos.

Os segredos devem possuir acesso restrito, proteção contra exposição acidental e procedimentos de rotação e revogação quando aplicável.

Os ambientes de desenvolvimento, testes e produção devem utilizar configurações e credenciais separadas.

A aplicação não deve disponibilizar segredos por meio de endpoints de diagnóstico, logs, arquivos públicos ou interfaces administrativas comuns.

### RNF10 - LGPD e Retenção de Dados

Os dados operacionais, registros de acesso físico, credenciais, dados de telemetria, informações cadastrais e demais dados pessoais devem possuir políticas de tratamento e retenção compatíveis com sua finalidade, necessidade operacional, segurança e obrigações legais aplicáveis.

As políticas devem ser definidas por categoria de dado, identificando:

* finalidade do tratamento;
* responsáveis e perfis autorizados;
* prazo de retenção ou critério para sua definição;
* condições de acesso;
* condições de anonimização ou eliminação;
* obrigações de preservação;
* procedimentos para atendimento dos direitos dos titulares, quando aplicáveis.

Quando apropriado, o sistema deve utilizar:

* retenção temporal;
* anonimização efetiva;
* agregação estatística;
* eliminação controlada;
* restrição de acesso;
* minimização dos dados coletados.

A anonimização deve impedir a identificação do titular por meios razoavelmente disponíveis, considerando os riscos de reidentificação. A simples remoção do nome ou do identificador direto não deve ser considerada suficiente automaticamente.

A aplicação das políticas de retenção não deve comprometer dados que possuam obrigação legítima de preservação legal, fiscal, contratual ou de auditoria.

O sistema deve permitir a identificação e o controle de dados sujeitos a diferentes prazos de retenção.

Dados biométricos destinados ao reconhecimento facial devem possuir avaliação específica de necessidade, finalidade, base legal, segurança, retenção e direitos dos titulares antes da implementação da funcionalidade.

### RNF11 - Objetivos de recuperação

O sistema deve definir objetivos de recuperação de serviço (RTO) e de recuperação de dados (RPO), considerando a criticidade das operações, os requisitos de continuidade do negócio e a infraestrutura contratada.

A estratégia de backup e restauração deve ser compatível com os objetivos definidos. A capacidade de recuperação deve ser verificada por testes periódicos de restauração.

Os valores finais de RTO e RPO devem ser estabelecidos antes da entrada em produção.


---

# Constraints de Integridade

As seguintes restrições devem ser aplicadas no banco de dados para garantir integridade relacional, consistência dos dados e prevenção de duplicidades.

## Tenant

### `UNIQUE (CNPJ)`

O CNPJ do condomínio deve ser globalmente único na plataforma.

A unicidade deve ser garantida pelo banco de dados, e o valor deve ser normalizado antes de sua persistência.

O encerramento lógico de um tenant não deve remover seu registro nem liberar a identidade histórica para associação com outro tenant. A unicidade deve considerar também os registros inativos.

## Usuário

### E-mail globalmente único

O e-mail normalizado de `Usuario` deve ser único globalmente no sistema, independentemente do `TenantID` e do perfil (`Morador`, `Funcionario`, `Administrador` ou `Master`).

A comparação deve ser insensível a maiúsculas e minúsculas. Diferenças de capitalização ou espaços externos não devem permitir cadastros duplicados.

A unicidade deve ser garantida pelo banco de dados e pela aplicação. Usuários `Master` estão sujeitos à mesma regra, mesmo quando possuem `TenantID` nulo.

## Unidade

### `UNIQUE (TenantID, Bloco, Numero)`

A combinação `(TenantID, Bloco, Numero)` deve ser única para unidades do mesmo tenant.

Os campos `TenantID`, `Bloco` e `Numero` são obrigatórios e não podem ser nulos.

Tenants diferentes podem utilizar a mesma convenção de endereçamento. Os campos utilizados na identificação da unidade devem possuir normalização consistente.

## Cobrança Condominial

### `UNIQUE (TenantID, UnidadeID, Competencia)`

Deve existir, no máximo, uma cobrança para determinada unidade e competência dentro do tenant.

A aplicação deve validar a associação entre a unidade e o tenant da cobrança, garantindo que ambos pertençam ao mesmo condomínio.

A competência deve utilizar um formato padronizado, evitando duplicidades decorrentes de diferentes representações da mesma competência.

## Vínculo de Usuário e Unidade

### Unicidade dos vínculos ativos

Os vínculos encerrados entre usuários e unidades devem ser preservados para manter o histórico.

Deve existir, no máximo, um vínculo ativo para cada combinação `(TenantID, UsuarioID, UnidadeID)`.

A unicidade deve ser aplicada somente aos vínculos ativos, permitindo a criação de um novo vínculo para a mesma combinação quando o vínculo anterior estiver encerrado.

O tipo de vínculo (`Proprietario`, `Inquilino` ou `Dependente`) deve ser tratado conforme as regras de domínio.

No PostgreSQL, essa regra pode ser implementada por meio de um índice único parcial que considere somente os vínculos ativos.

## Dispositivo IoT

### `UNIQUE (MacAddress)`

O endereço físico `MacAddress` do dispositivo IoT deve possuir unicidade global, independentemente do tenant.

A aplicação deve normalizar o formato do endereço antes de persistir o valor.

A inativação do dispositivo não deve liberar automaticamente o endereço físico para associação a outro dispositivo enquanto o registro original for preservado e a política de identidade determinar que o endereço continua reservado.

## Reserva

### Restrição de intervalo de tempo por espaço

O banco de dados deve impedir que duas reservas válidas do mesmo espaço possuam intervalos de tempo sobrepostos.

A restrição deve considerar os estados que ocupam efetivamente a disponibilidade do espaço. Reservas canceladas ou em estados que não bloqueiem o espaço devem ser excluídas do conjunto considerado, conforme as regras de negócio.

A implementação deve considerar os limites de início e fim dos intervalos e definir se reservas consecutivas, nas quais uma termina exatamente quando a outra começa, são permitidas.

A proteção deve funcionar mesmo quando múltiplas transações tentarem criar reservas conflitantes simultaneamente.

No PostgreSQL, deve-se priorizar uma constraint de exclusão sobre intervalos de tempo quando apropriado, ou um mecanismo transacional equivalente que garanta a mesma integridade.

A aplicação deve tratar conflitos de concorrência e violações da restrição, retornando uma resposta controlada ao cliente.

## Integridade Referencial

As entidades relacionadas devem possuir chaves estrangeiras para garantir a existência dos registros referenciados.

O sistema deve impedir associações entre entidades de tenants diferentes.

Quando necessário, a integridade do tenant deve ser garantida por constraints compostas, chaves estrangeiras compostas ou mecanismos equivalentes, além das validações do backend.

Exclusões em cascata não devem remover dados históricos, financeiros ou de auditoria.

O comportamento de exclusão das relações deve ser definido explicitamente, priorizando restrição de exclusão ou desassociação controlada quando houver histórico relevante.

## Validações de Domínio no Banco

Além das constraints de unicidade, o banco de dados e a aplicação devem garantir as seguintes invariantes:

* `Reserva.DataHoraFim` deve ser posterior a `Reserva.DataHoraInicio`.
* `AutorizacaoAcesso.ValidoAte` deve ser posterior a `AutorizacaoAcesso.ValidoDe`.
* `CobrancaCondominial.Valor` não pode ser negativo.
* `CobrancaCondominial.DataPagamento` deve ser nula enquanto a cobrança não estiver paga, conforme o modelo financeiro adotado.
* `Usuario.TenantID` deve ser nulo somente para usuários com papel `Master`.
* Usuários que não sejam `Master` devem possuir `TenantID`.
* Entidades relacionadas operacionalmente devem pertencer ao mesmo tenant.
* Uma reserva válida não pode sobrepor outra reserva válida para o mesmo espaço.
* Registros de `HistoricoChamado`, `RegistroAcesso` e `RegistroTelemetria` devem ser tratados como históricos imutáveis por operações comuns.
* Autorizações de acesso não podem estar associadas a credenciais, dispositivos, reservas ou usuários de tenants diferentes.
* Entidades inativadas não devem receber novas operações incompatíveis com seu estado.
* Tarefas assíncronas devem validar o estado atual do tenant antes de executar operações de negócio.
* Mensagens MQTT não podem provocar operações em um tenant diferente daquele ao qual o dispositivo está vinculado.
* Operações de suporte devem respeitar o escopo autorizado e não podem ampliar suas próprias permissões.
* Alterações administrativas devem preservar informações anteriores quando necessárias para auditoria.
* Preferências individuais de notificação não podem desativar notificações obrigatórias.
* Dados biométricos, quando a funcionalidade futura for implementada, devem obedecer às regras específicas de acesso, retenção e proteção de dados.

## Índices e Desempenho de Consultas

O banco de dados deve possuir índices adequados para consultas frequentes e operações críticas, considerando os padrões de acesso definidos pela aplicação.

As tabelas operacionais com escopo por tenant devem possuir índices compatíveis com as consultas que filtram por `TenantID` e outros campos utilizados frequentemente.

Os índices devem ser definidos conforme as consultas reais, evitando a criação indiscriminada de índices que prejudiquem operações de escrita.

Consultas críticas, como verificação de disponibilidade de reservas, recuperação de chamados, consulta de notificações e leitura de telemetria, devem ser avaliadas por meio de planos de execução e testes de desempenho.

## Integridade dos Registros Históricos

Os registros de histórico, auditoria, acesso físico e telemetria devem ser protegidos contra alteração ou exclusão por operações comuns da aplicação.

Quando necessário, a proteção deve incluir permissões específicas no banco de dados, separação de responsabilidades e mecanismos de auditoria adicionais.

A política de retenção pode permitir eliminação controlada de registros após o prazo aplicável, desde que a operação seja autorizada, documentada e compatível com as obrigações legais.

## Integridade de Operações Assíncronas

Operações assíncronas devem possuir identificadores e mecanismos de idempotência adequados para impedir efeitos duplicados.

Falhas de comunicação não devem provocar automaticamente a repetição de operações financeiras, reservas, autorizações ou notificações sem verificação do estado anterior.

O sistema deve registrar falhas definitivas e permitir reprocessamento controlado conforme as regras de negócio.
