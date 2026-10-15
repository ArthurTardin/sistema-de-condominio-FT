# Regras de Negócio

## 1. Objetivo

Este documento define as regras que determinam o comportamento operacional, administrativo e financeiro do Sistema de Condomínio.

As regras de negócio complementam os requisitos funcionais e orientam a implementação do backend, das interfaces Web, Mobile e Desktop, das integrações IoT e dos mecanismos de persistência.

As regras devem ser aplicadas de maneira consistente independentemente do cliente utilizado para acessar a plataforma.

## 2. Identificação e Aplicação das Regras

Cada regra possui um identificador único no formato `RN`, seguido de um número sequencial.

As regras devem ser implementadas na camada de domínio ou de aplicação, conforme sua natureza, e complementadas por restrições do banco de dados, validações de autorização e mecanismos de auditoria quando necessário.

Quando uma regra depender de uma configuração administrativa, o sistema deve validar essa configuração antes de executar a operação.

---

## 3. Multi-tenancy e Isolamento de Dados

### RN01 - Isolamento entre Tenants

Cada condomínio deve possuir um contexto de tenant próprio.

Usuários, unidades, reservas, chamados, cobranças, dispositivos IoT, credenciais de acesso e demais entidades operacionais devem pertencer ao tenant correspondente, salvo entidades explicitamente definidas como globais da plataforma.

Nenhum usuário ou processo operacional pode consultar, alterar ou excluir dados de outro tenant sem uma autorização específica prevista para funções administrativas da plataforma.

O contexto do tenant deve ser obtido de uma identidade autenticada e validada pelo backend. O sistema não deve confiar exclusivamente em um `TenantID` informado pelo cliente.

### RN02 - Encerramento de Tenant

O encerramento de um tenant deve ser realizado por exclusão lógica, preservando os registros históricos, financeiros, operacionais e de auditoria que devam ser mantidos.

Após o encerramento, o sistema deve impedir novas operações de negócio para o tenant, incluindo reservas, abertura de chamados, emissão de novas autorizações de acesso e processamento operacional de telemetria.

Sessões e credenciais de acesso devem ser revogadas ou invalidadas conforme a política de encerramento.

Mensagens recebidas de dispositivos vinculados a um tenant encerrado não devem provocar operações de negócio. Quando necessário, devem ser rejeitadas ou encaminhadas a um mecanismo controlado de quarentena.

A reativação de um tenant deve exigir autorização explícita e registro de auditoria. A reativação não deve restaurar automaticamente sessões, credenciais ou autorizações anteriormente revogadas.

### RN03 - Operações da Plataforma

Operações administrativas globais devem ser executadas somente por usuários com papel e permissões compatíveis com a função pretendida.

O papel `Master` representa uma função administrativa da plataforma e não concede, por si só, acesso irrestrito a todas as informações pessoais ou operacionais dos condomínios.

O acesso a dados de um tenant para suporte deve respeitar o escopo autorizado, a finalidade do atendimento e os controles definidos para operações de suporte.

---

## 4. Usuários, Unidades e Permissões

### RN04 - Autorização por Papel

O sistema deve aplicar controle de acesso baseado em papéis e permissões.

Os papéis previstos são `Morador`, `Funcionario`, `Administrador` e `Master`.

Cada operação deve verificar a identidade autenticada, o papel, as permissões específicas e o escopo do tenant antes de permitir a execução.

A existência de uma interface ou botão disponível ao usuário não substitui a validação de autorização no backend.

### RN05 - Vínculo entre Usuário e Unidade

Um usuário somente pode executar operações que dependam de vínculo com uma unidade quando possuir um vínculo ativo e compatível com a operação.

Os tipos de vínculo previstos são `Proprietario`, `Inquilino` e `Dependente`.

O encerramento de um vínculo deve impedir novas operações que dependam exclusivamente daquele vínculo, sem eliminar o histórico que precise ser preservado.

A existência de vínculo com uma unidade não concede automaticamente permissões administrativas sobre o condomínio.

### RN06 - Desativação de Usuários

Usuários desativados não podem iniciar novas operações autenticadas que exijam uma conta ativa.

Sessões e credenciais de autenticação devem ser invalidadas conforme a política de segurança.

A desativação não deve excluir automaticamente reservas passadas, cobranças, chamados, registros de acesso ou históricos associados ao usuário.

Operações futuras vinculadas ao usuário desativado devem ser tratadas conforme as regras específicas de cada módulo.

---

## 5. Reservas de Espaços Comuns

### RN07 - Elegibilidade para Reservar

Somente usuários autenticados, com vínculo ativo com uma unidade e permissão para reservar podem criar reservas.

A reserva deve pertencer ao tenant do usuário e estar associada a um espaço disponível para utilização.

A elegibilidade também deve considerar as restrições de inadimplência, os limites de antecedência, a duração máxima e as demais configurações aplicáveis ao condomínio.

### RN08 - Disponibilidade do Espaço

Uma reserva somente pode ser confirmada quando o espaço estiver disponível durante todo o intervalo solicitado.

O intervalo deve respeitar as configurações de funcionamento, a antecedência mínima, o limite máximo de duração e as regras de agendamento definidas pelo condomínio.

Duas reservas que ocupem efetivamente a disponibilidade do mesmo espaço não podem possuir intervalos sobrepostos.

A verificação de disponibilidade realizada pela interface é informativa. A confirmação definitiva depende da validação do backend e da garantia de integridade da operação persistida.

### RN09 - Estados que Ocupam a Disponibilidade

Os estados de reserva que bloqueiam o espaço devem ser definidos de maneira consistente no domínio e na persistência.

Reservas canceladas ou em estados que não ocupem a disponibilidade não devem impedir novos agendamentos.

A confirmação de uma reserva deve respeitar as regras de aprovação configuradas para o espaço e para o condomínio.

### RN10 - Cancelamento de Reservas

O morador pode cancelar uma reserva própria quando ela estiver em um estado que permita cancelamento e dentro das condições definidas pelo condomínio.

O cancelamento deve registrar o estado resultante e preservar o histórico da operação.

Quando o cancelamento for executado por um administrador, o sistema deve exigir justificativa quando determinado pelas regras ou pelos requisitos funcionais.

As partes afetadas devem ser notificadas conforme as políticas de notificação aplicáveis.

Uma reserva cancelada não deve continuar bloqueando a disponibilidade do espaço, salvo se existir uma regra operacional explícita e justificada para o caso.

### RN11 - Alteração da Disponibilidade após a Reserva

Quando um espaço se tornar interditado, indisponível ou entrar em manutenção, o sistema deve impedir novas reservas incompatíveis com seu estado.

Reservas já confirmadas e afetadas pela alteração devem ser identificadas e tratadas por um fluxo definido, que pode incluir cancelamento administrativo, realocação ou comunicação aos moradores.

O sistema não deve cancelar silenciosamente reservas confirmadas em decorrência de uma alteração operacional.

A ação adotada deve ser registrada, e os usuários afetados devem receber as notificações aplicáveis.

### RN12 - Concorrência em Reservas

Se duas ou mais solicitações concorrentes tentarem reservar o mesmo espaço em intervalos incompatíveis, o sistema deve confirmar somente as operações que não violem a regra de disponibilidade.

A operação que não puder ser confirmada deve receber uma resposta controlada informando o conflito.

Falhas de concorrência não devem produzir reservas duplicadas ou estados inconsistentes.

### RN13 - Histórico de Reservas

O histórico de reservas deve preservar as alterações relevantes, incluindo criação, confirmação, cancelamento e alterações administrativas.

A consulta ao histórico deve respeitar o tenant, o vínculo do usuário com a unidade e as permissões do papel autenticado.

---

## 6. Inadimplência e Cobranças

### RN14 - Definição de Inadimplência

A condição de inadimplência deve ser determinada com base no estado das cobranças, nos vencimentos e nas regras financeiras aplicáveis ao condomínio.

A definição de quais cobranças geram inadimplência deve ser explícita e consistente, incluindo o tratamento de cobranças vencidas, pagas, canceladas, contestadas ou em negociação.

### RN15 - Bloqueio de Reservas por Inadimplência

Quando a política do condomínio estabelecer bloqueio de reservas por inadimplência, o sistema deve verificar a condição financeira do usuário ou da unidade conforme a regra adotada antes de permitir a reserva.

A política deve definir quais cobranças são consideradas, se existe período de tolerância e quais condições permitem remover o bloqueio.

O sistema não deve aplicar o bloqueio a usuários ou unidades que não estejam abrangidos pela política configurada.

### RN16 - Regularização Financeira

A regularização da situação financeira deve atualizar a elegibilidade para novas operações sujeitas à política de inadimplência.

A remoção do bloqueio deve depender do estado financeiro efetivamente registrado, e não apenas de uma solicitação de pagamento ainda não confirmada.

O histórico financeiro e os registros de alterações devem ser preservados conforme as políticas de retenção aplicáveis.

### RN17 - Integridade das Cobranças

Cada cobrança deve estar associada à unidade e ao tenant correspondentes.

A criação de uma cobrança deve respeitar a regra de unicidade por unidade e competência definida no modelo de dados.

Alterações de estado financeiro devem ser autorizadas e registradas de acordo com os controles administrativos e de auditoria.

---

## 7. Chamados de Manutenção

### RN18 - Abertura de Chamados

Um morador pode abrir um chamado quando possuir vínculo ativo com uma unidade e permissão para utilizar o serviço.

O chamado deve conter as informações obrigatórias definidas para sua categoria, incluindo descrição e demais dados necessários à análise.

Chamados criados por usuários devem ser associados ao tenant e à identidade de origem corretos.

Chamados gerados automaticamente por eventos de IoT devem ser identificados como originados de automação, com referência ao dispositivo ou evento que motivou sua criação.

### RN19 - Transições de Estado dos Chamados

Um chamado somente pode transitar entre estados permitidos pelo fluxo de trabalho definido para o sistema.

O backend deve validar o estado atual e o estado pretendido antes de persistir uma transição.

Não devem ser permitidas transições arbitrárias que contornem as etapas de atendimento, resolução ou encerramento.

As regras específicas de transição devem ser documentadas em conjunto com a especificação dos estados do chamado.

### RN20 - Atribuição e Responsabilidade

A atribuição de um chamado deve ocorrer somente para funcionários ou responsáveis autorizados e ativos no tenant correspondente.

O sistema deve preservar o histórico de atribuições e permitir identificar o responsável atual e os responsáveis anteriores.

A alteração de responsável deve respeitar as permissões do usuário que executa a operação.

### RN21 - Histórico de Chamados

As transições de estado e demais alterações relevantes do chamado devem gerar registros históricos com informações suficientes para rastrear a operação.

O histórico deve permitir identificar o estado anterior, o novo estado, o responsável pela alteração, a data e hora e, quando aplicável, o comentário ou a justificativa.

Os registros históricos não devem ser alterados ou excluídos por operações comuns de gerenciamento de chamados.

### RN22 - Resolução e Encerramento

A resolução e o encerramento de um chamado devem respeitar as condições definidas para o fluxo de atendimento.

Quando houver exigência de descrição da solução, evidência ou justificativa, essas informações devem ser fornecidas antes da transição correspondente.

A reabertura de um chamado resolvido deve obedecer às transições permitidas e preservar o histórico anterior.

O sistema deve distinguir a conclusão técnica do atendimento do encerramento administrativo quando o fluxo de trabalho exigir essa separação.

### RN23 - Cancelamento de Chamados

O cancelamento de um chamado deve ser permitido somente nos estados e condições previstos no fluxo de trabalho.

Quando a operação exigir justificativa, ela deve ser registrada no histórico.

O cancelamento não deve apagar o chamado nem seus registros históricos.

### RN24 - Prazos e SLA

Os prazos de atendimento devem ser calculados conforme as regras de SLA definidas para o condomínio, para o tipo de chamado ou para a prioridade aplicável.

O sistema deve distinguir chamados dentro do prazo, próximos do vencimento e fora do prazo.

Pausas, suspensões ou reinícios do SLA somente devem ocorrer quando houver regra explícita que os autorize.

As alterações que afetem o cálculo do SLA devem ser rastreáveis.

---

## 8. Incidentes e Ocorrências

### RN25 - Registro de Ocorrências

Usuários autorizados podem registrar ocorrências relacionadas ao condomínio.

Cada ocorrência deve ser associada ao tenant e ao autor correspondente, respeitando as permissões aplicáveis.

O registro deve preservar as informações necessárias à análise e ao acompanhamento, incluindo data, descrição, categoria e anexos quando previstos.

### RN26 - Tratamento de Ocorrências

O tratamento administrativo de uma ocorrência deve respeitar o fluxo de estados definido para o módulo.

Alterações relevantes devem ser registradas e acessíveis somente aos perfis autorizados.

O encerramento de uma ocorrência não deve remover o registro original nem seu histórico necessário.

### RN27 - Relação entre Ocorrências e Chamados

Uma ocorrência pode originar um chamado quando houver necessidade de manutenção ou atendimento operacional.

A associação entre os registros deve preservar a rastreabilidade da ocorrência original e do chamado resultante.

A criação do chamado não deve provocar duplicidade de registros de atendimento para o mesmo evento sem justificativa operacional.

---

## 9. Ativos, Infraestrutura e IoT

### RN28 - Estados Operacionais dos Ativos

Os ativos de infraestrutura devem respeitar os estados operacionais definidos no sistema, incluindo `Disponivel`, `Interditado`, `Manutencao`, `Operacional` e `Inativo`, conforme aplicável à entidade.

A transição entre estados deve obedecer às permissões e às condições operacionais previstas.

Ativos interditados, inativos ou em manutenção não devem ser utilizados em operações incompatíveis com seu estado.

### RN29 - Associação de Dispositivos

Cada dispositivo IoT deve estar associado a um tenant e a um ativo ou local de instalação conforme o modelo de dados.

A associação deve ser validada pelo backend antes de aceitar operações ou mensagens operacionais.

Dispositivos desativados ou desvinculados não devem gerar novas operações de negócio como se estivessem ativos.

### RN30 - Recebimento de Telemetria

As mensagens de telemetria devem ser validadas quanto à origem, ao dispositivo, ao tenant, ao formato e aos campos obrigatórios.

Mensagens inválidas ou provenientes de dispositivos não autorizados não devem alterar o estado operacional do sistema.

O processamento deve evitar duplicação de registros quando a mesma mensagem for recebida novamente, utilizando identificadores ou mecanismos de idempotência adequados.

Os registros de telemetria devem preservar os dados necessários à rastreabilidade e à análise, conforme a política de retenção aplicável.

### RN31 - Alarmes Automáticos

Quando a telemetria ultrapassar limites configurados ou indicar uma condição de risco reconhecida, o sistema deve aplicar as regras de geração de alarmes.

Os alarmes devem ser associados ao dispositivo, ao tenant e ao evento que motivou sua criação.

A geração de chamados automáticos deve evitar duplicações indevidas para o mesmo evento ou condição persistente.

Os critérios de deduplicação, correlação e reabertura devem ser definidos conforme o tipo de alarme e a política operacional.

### RN32 - Falhas de Comunicação IoT

Falhas de comunicação, ausência de telemetria ou erros de sincronização devem ser tratados conforme a criticidade do dispositivo e as políticas de monitoramento.

O sistema deve distinguir indisponibilidade confirmada, atraso esperado e ausência de dados ainda não conclusiva, quando houver informações suficientes para isso.

As tentativas de reprocessamento devem respeitar limites e mecanismos de idempotência, sem produzir operações duplicadas.

### RN33 - Operação Offline e Sincronização

Dispositivos que suportem operação offline devem respeitar os limites de validade e as permissões previamente sincronizadas.

Após a reconexão, os eventos pendentes devem ser enviados e processados de maneira controlada, preservando sua origem e seus horários disponíveis.

A sincronização não deve permitir que dados antigos restabeleçam autorizações revogadas ou sobrescrevam estados centrais mais recentes sem validação.

### RN34 - Reconhecimento Facial Futuro

O reconhecimento facial para controle de acesso é uma funcionalidade futura e não deve ser considerado parte do MVP até que seja formalmente aprovado para implementação.

Sua implementação dependerá de avaliação específica de segurança, privacidade, base legal, finalidade, retenção, direitos dos titulares e mecanismos alternativos de acesso.

O uso de reconhecimento facial não deve ser ativado sem as configurações, autorizações e controles necessários à funcionalidade.

---

## 10. Credenciais e Controle de Acesso Físico

### RN35 - Emissão de Autorizações de Acesso

Uma autorização de acesso vinculada a uma reserva deve ser emitida somente quando a reserva atingir o estado que habilita a liberação, conforme as regras do sistema.

A autorização deve estar vinculada ao tenant, à reserva, ao usuário ou credencial correspondente e ao dispositivo autorizado, conforme o modelo de dados.

O período de validade deve respeitar o intervalo permitido para a reserva e as regras de acesso definidas pelo condomínio.

### RN36 - Validade e Revogação de Autorizações

Uma autorização expirada ou revogada não deve ser considerada válida pela plataforma.

A revogação deve ser propagada aos dispositivos de acesso conforme os mecanismos de sincronização disponíveis.

Quando houver operação offline, o sistema deve considerar explicitamente o intervalo durante o qual o dispositivo ainda pode não conhecer uma revogação central.

### RN37 - Registro de Acessos

Os eventos de acesso físico devem registrar as informações necessárias à rastreabilidade, incluindo dispositivo, data e hora, resultado da tentativa e referência à credencial ou autorização quando aplicável.

A consulta aos registros deve respeitar as permissões e o escopo do tenant.

Falhas de acesso e eventos de segurança devem ser tratados de acordo com as políticas de auditoria e monitoramento.

### RN38 - Revogação de Credenciais

Credenciais desativadas, revogadas ou expiradas não devem autorizar novos acessos pela plataforma.

A atualização do estado das credenciais deve ser refletida nas autorizações associadas, conforme as regras do sistema.

A propagação de revogações para dispositivos desconectados deve seguir a política de operação offline e os limites de validade definidos.

---

## 11. Notificações

### RN39 - Emissão de Notificações

As notificações devem ser geradas para os eventos e destinatários definidos nos requisitos funcionais e nas configurações aplicáveis.

O sistema deve evitar o envio duplicado de notificações para o mesmo evento lógico, quando houver identificador ou mecanismo de deduplicação disponível.

Falhas de entrega devem ser registradas e tratadas conforme a política de reprocessamento do canal utilizado.

### RN40 - Preferências de Notificação

As preferências individuais devem controlar somente as categorias e os canais que possam ser personalizados.

O usuário não deve conseguir desativar notificações classificadas como obrigatórias por regras operacionais, de segurança ou legais aplicáveis.

A preferência de um usuário não deve alterar as configurações de notificação de outros usuários.

### RN41 - Consistência das Notificações

A emissão de notificações relacionadas a operações persistentes deve considerar o resultado efetivamente confirmado da operação.

Falhas de comunicação ou repetição de tarefas não devem produzir duplicidades indevidas nem informar como concluída uma operação que não tenha sido confirmada.

Quando a entrega ocorrer de forma assíncrona, o sistema deve distinguir a criação da notificação de sua efetiva entrega ao canal de destino.

---

## 12. Auditoria e Suporte Administrativo

### RN42 - Registro de Auditoria

Operações administrativas e alterações críticas devem gerar registros de auditoria conforme os requisitos de segurança e rastreabilidade.

Os registros devem permitir identificar, quando aplicável, o ator, a ação, o recurso afetado, a data e hora, o tenant e o resultado da operação.

Informações sensíveis desnecessárias não devem ser incluídas nos registros.

### RN43 - Integridade da Auditoria

Registros de auditoria devem ser protegidos contra alteração ou exclusão por operações comuns da aplicação.

A retenção deve obedecer à política definida para a categoria de registro e às obrigações legais aplicáveis.

Operações privilegiadas de manutenção devem ser restritas e rastreáveis.

### RN44 - Suporte a Tenants

O acesso de suporte a dados de um condomínio deve depender de autorização específica, finalidade definida e escopo limitado.

O suporte não deve conceder a si próprio permissões adicionais nem utilizar o acesso para executar operações fora da finalidade autorizada.

A concessão, utilização, expiração e revogação de acessos de suporte devem ser rastreáveis.

Quando a operação exigir revisão ou aprovação adicional, o atendimento deve respeitar esse procedimento antes de executar ações privilegiadas.

### RN45 - Ações Administrativas Sensíveis

Ações administrativas sensíveis devem exigir as permissões e justificativas definidas para cada operação.

Quando aplicável, o sistema deve registrar o valor anterior, o valor posterior, o autor e a justificativa da alteração.

A interface não deve permitir que um administrador contorne as validações de domínio ou as restrições de autorização executadas pelo backend.

---

## 13. Tarefas Automáticas e Processamento em Segundo Plano

### RN46 - Expiração Automática

Reservas, autorizações, sessões e demais registros com validade temporal devem ser expirados conforme suas regras específicas.

A expiração automática deve ser idempotente, podendo ser executada novamente sem produzir efeitos duplicados ou estados inválidos.

A ausência temporária de execução da tarefa não deve permitir que registros expirados sejam tratados como válidos em operações críticas. Quando necessário, a validade deve ser verificada também no momento da operação.

### RN47 - Processamento de Vencimentos e SLA

Tarefas automáticas de vencimento financeiro, notificações e acompanhamento de SLA devem utilizar as datas, os estados e as configurações vigentes no momento da execução.

O processamento deve evitar duplicidades e registrar falhas que exijam intervenção ou reprocessamento.

A repetição de uma tarefa não deve duplicar cobranças, notificações, alterações de estado ou outros efeitos já confirmados.

### RN48 - Reprocessamento de Falhas

Operações que falharem de forma temporária podem ser reprocessadas conforme políticas definidas de tentativas, intervalo e limite de repetição.

Falhas definitivas devem ser identificadas e disponibilizadas para análise operacional.

O reprocessamento deve verificar o estado atual da operação e utilizar mecanismos de idempotência sempre que houver risco de duplicação.

### RN49 - Encerramento de Sessões

Sessões, tokens de atualização e credenciais de autenticação devem respeitar as regras de expiração e revogação definidas pelo sistema.

A revogação de uma sessão deve impedir sua renovação e invalidar o uso futuro das credenciais revogadas, respeitando as características do mecanismo de autenticação adotado.

A desativação de um usuário ou o encerramento de um tenant deve desencadear as ações de invalidação previstas nas regras de segurança.

### RN50 - Backup e Recuperação

Os processos de backup devem seguir a política de continuidade definida para o ambiente.

As rotinas de backup e restauração devem registrar resultados, falhas e ações corretivas relevantes.

A restauração deve preservar a integridade dos dados e respeitar os procedimentos de recuperação e validação definidos para o sistema.

### RN51 - Monitoramento Operacional

O sistema deve monitorar os componentes críticos da aplicação, incluindo API, banco de dados, tarefas em segundo plano, integrações e infraestrutura IoT.

Falhas que afetem a disponibilidade, a segurança ou a integridade operacional devem gerar registros e alertas conforme a criticidade definida.

O monitoramento não deve expor credenciais, dados pessoais desnecessários ou conteúdo sensível.

---

## 14. Disposições Gerais

### RN52 - Consistência entre Clientes

As interfaces Web, Mobile e Desktop devem respeitar as mesmas regras de negócio aplicáveis às operações executadas por meio do backend.

Nenhum cliente pode conceder permissões, confirmar operações ou alterar estados de forma independente das validações do servidor.

### RN53 - Consistência entre Regras e Persistência

As regras de negócio devem permanecer coerentes com os requisitos funcionais, as constraints de integridade do banco e as especificações de arquitetura.

Quando uma regra for alterada, os documentos e componentes afetados devem ser revisados para evitar contradições.

### RN54 - Tratamento de Falhas

Falhas de validação, autorização, concorrência, comunicação ou persistência devem produzir resultados controlados e rastreáveis.

O sistema não deve considerar uma operação concluída sem confirmação suficiente de seu resultado.

As mensagens apresentadas aos usuários devem explicar o resultado de maneira compreensível sem expor detalhes internos, segredos ou informações de outros tenants.

### RN55 - Configurações Administrativas

Configurações que alterem o comportamento operacional do sistema devem ser modificadas somente por perfis autorizados.

Alterações devem respeitar limites e validações de domínio e, quando relevantes, ser registradas para auditoria.

Configurações inválidas ou ausentes não devem resultar silenciosamente em permissões mais amplas ou em comportamentos inseguros.