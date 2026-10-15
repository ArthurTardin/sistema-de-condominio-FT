# UC-RES-02 - Cancelar Reserva

## 1. Identificação

- **ID:** UC-RES-02
- **Nome:** Cancelar Reserva
- **Atores principais:** Morador e Administrador
- **Requisito funcional principal:** RF06
- **Categoria:** Caso de uso de negócio
- **Objetivo:** Permitir o cancelamento de uma reserva existente, garantindo o cumprimento das regras de autorização e prazo, a consistência dos dados, o registro do histórico e o tratamento das autorizações de acesso vinculadas.

## 2. Descrição

Este caso de uso permite que um morador cancele uma reserva própria ou que um administrador cancele uma reserva conforme suas permissões. O sistema verifica se a reserva pode ser cancelada, aplica as regras específicas do ator, registra a alteração de estado e preserva o histórico da operação.

Quando houver autorização de acesso associada à reserva, o sistema também deverá iniciar seu processo de revogação e sincronização com os dispositivos de controle de acesso.

## 3. Pré-condições

- O ator deve estar autenticado.
- O ator deve possuir permissão para cancelar a reserva.
- A reserva deve existir e pertencer ao condomínio ao qual o ator está vinculado.
- A reserva não pode estar excluída logicamente.
- A reserva deve estar em um estado que permita o cancelamento.
- O condomínio deve estar ativo para a realização de novas operações, conforme as regras de ciclo de vida do tenant.

## 4. Pós-condições

### 4.1. Sucesso

- A reserva passa para o estado `Cancelado`.
- A alteração é registrada no histórico da reserva, com data e hora, responsável e informações pertinentes à operação.
- A transação persiste a alteração de estado e o registro de histórico de forma atômica.
- Caso exista autorização de acesso vinculada, sua revogação lógica é registrada e a sincronização com o dispositivo é encaminhada conforme o mecanismo definido no projeto.
- As notificações aplicáveis são agendadas para processamento.
- O horário reservado volta a estar disponível, desde que nenhuma outra regra de negócio impeça uma nova reserva.

### 4.2. Falha

- A reserva permanece inalterada quando a operação não puder ser concluída.
- Nenhuma alteração parcial da reserva ou do histórico deve ser persistida.
- Quando a operação falhar por conflito de concorrência, o sistema informa que o estado pode ter mudado e solicita nova consulta.

## 5. Fluxo principal

1. O ator solicita o cancelamento de uma reserva.
2. O sistema valida a autenticação, o contexto do condomínio e as permissões do ator.
3. O ator informa ou seleciona a reserva que deseja cancelar.
4. O sistema localiza a reserva e valida sua existência e seu estado atual.
5. O sistema verifica se o ator possui autorização para cancelar aquela reserva.
6. Se o ator for um morador, o sistema valida o prazo e as condições de cancelamento aplicáveis.
7. Se o ator for um administrador, o sistema valida suas permissões administrativas e exige o motivo do cancelamento quando determinado pelas regras do projeto.
8. O sistema inicia uma transação e verifica se a reserva continua elegível para cancelamento.
9. O sistema altera o estado da reserva para `Cancelado`, aplicando o mecanismo de controle de concorrência.
10. O sistema registra o evento correspondente no histórico da reserva.
11. Se houver autorização de acesso vinculada, o sistema registra sua revogação lógica e o estado de sincronização necessário.
12. O sistema confirma a transação, persistindo as alterações obrigatórias de forma atômica.
13. O sistema encaminha as notificações e as tarefas de sincronização pendentes para processamento.
14. O sistema informa que o cancelamento foi realizado com sucesso.

## 6. Fluxos alternativos e exceções

### FA01 - Reserva inexistente

1. No passo 4, o sistema não encontra a reserva solicitada.
2. O sistema informa que a reserva não foi encontrada ou não está disponível para consulta.
3. O caso de uso é encerrado sem alterações.

### FA02 - Ator sem permissão

1. No passo 5, o sistema identifica que o ator não possui permissão para cancelar a reserva.
2. O sistema nega a operação sem revelar dados de reservas de outros usuários ou condomínios.
3. Nenhuma alteração é persistida.

### FA03 - Prazo de cancelamento ultrapassado

1. No passo 6, o sistema identifica que o prazo permitido para cancelamento foi ultrapassado.
2. O sistema informa que o cancelamento não é permitido pelas regras vigentes.
3. A reserva permanece inalterada.

### FA04 - Estado incompatível com cancelamento

1. No passo 4 ou 8, o sistema identifica que a reserva já foi cancelada ou está em um estado que não permite cancelamento.
2. O sistema informa que a operação não pode ser realizada.
3. Nenhuma alteração é persistida.

### FA05 - Administrador não informa o motivo obrigatório

1. No passo 7, o administrador não fornece o motivo exigido para a operação.
2. O sistema solicita o preenchimento do motivo.
3. O cancelamento não prossegue até que a informação obrigatória seja fornecida.

### FA06 - Conflito de concorrência

1. No passo 9, o sistema identifica que a reserva foi alterada por outra operação concorrente.
2. A transação é revertida.
3. O sistema informa que a reserva foi modificada e orienta o ator a consultar seu estado atualizado.
4. Nenhuma alteração parcial é persistida.

### FA07 - Falha de persistência

1. Durante a execução da transação, ocorre uma falha no banco de dados ou na persistência obrigatória.
2. O sistema reverte a transação.
3. O sistema registra a falha técnica sem expor informações sensíveis.
4. O sistema informa que não foi possível concluir o cancelamento.

### FA08 - Falha na sincronização da autorização de acesso

1. Após a confirmação da transação, a sincronização da revogação com o dispositivo de acesso falha.
2. A reserva permanece cancelada e a revogação lógica permanece registrada.
3. A sincronização fica pendente para nova tentativa, conforme a política de retentativas do sistema.
4. A falha é registrada para monitoramento e auditoria, conforme aplicável.

### FA09 - Falha no envio da notificação

1. O cancelamento é persistido com sucesso, mas o envio da notificação falha.
2. O sistema mantém o cancelamento confirmado.
3. A notificação fica sujeita ao mecanismo de retentativa definido pelo projeto.

## 7. Regras de negócio

- **RN01:** Somente atores autenticados e autorizados podem cancelar reservas.
- **RN02:** O morador só pode cancelar reservas próprias, respeitadas as permissões e regras vigentes.
- **RN03:** O administrador pode cancelar reservas dentro do escopo de suas permissões.
- **RN04:** O prazo e as condições de cancelamento devem seguir os requisitos funcionais e as configurações aplicáveis ao condomínio.
- **RN05:** O motivo deve ser informado quando exigido pelas regras do projeto, incluindo os casos administrativos definidos nos requisitos.
- **RN06:** Uma reserva não pode ser cancelada mais de uma vez nem transitar de um estado incompatível com o cancelamento.
- **RN07:** A alteração de estado e o registro obrigatório do histórico devem ser persistidos atomicamente.
- **RN08:** O mecanismo de concorrência deve impedir que operações simultâneas produzam estados inconsistentes.
- **RN09:** Uma autorização de acesso vinculada à reserva cancelada deve ter sua revogação lógica registrada, quando aplicável.
- **RN10:** A sincronização física da revogação é independente da revogação lógica e deve permitir retentativas em caso de falha.
- **RN11:** Falhas de notificação ou sincronização posteriores à confirmação não devem desfazer o cancelamento já persistido.
- **RN12:** A operação deve respeitar o isolamento entre condomínios e não permitir acesso ou alteração de reservas de outro tenant.
- **RN13:** Os registros de histórico e auditoria devem respeitar as políticas de retenção e integridade definidas no projeto.

## 8. Requisitos relacionados

- **RF01:** Isolamento e contexto multi-tenant.
- **RF01.1:** Regras de encerramento e bloqueio operacional do condomínio.
- **RF02:** Autenticação e controle de acesso.
- **RF06:** Cancelamento de reservas.
- **RF-IoT01:** Geração, revogação e sincronização de autorizações de acesso, quando aplicável.
- **RNF de integridade e concorrência:** Consistência das operações concorrentes e atomicidade das alterações obrigatórias.
- **RNF de auditoria e segurança:** Proteção de dados, rastreabilidade e controle de acesso.

## 9. Relacionamento com outros casos de uso

- **Autenticar-se:** fornece a identidade e o contexto de autorização do ator.
- **Realizar Reserva:** estabelece a reserva que poderá ser cancelada.
- **Consultar Reservas:** permite localizar e visualizar reservas elegíveis.
- **Gerar Autorização de Acesso:** pode ter criado a autorização vinculada à reserva.
- **Sincronizar Autorização de Acesso:** propaga a revogação lógica para os dispositivos.
- **Enviar Notificações Automaticamente:** comunica o cancelamento aos destinatários aplicáveis.

## 10. Observações técnicas

- A alteração de estado e o registro obrigatório do histórico devem ocorrer na mesma transação.
- Deve ser utilizado o mecanismo de concorrência definido na arquitetura e no DER, com tratamento explícito de conflitos.
- A revogação lógica da autorização de acesso deve ser persistida antes de depender da comunicação com dispositivos externos.
- Operações assíncronas, como sincronização e notificação, devem admitir retentativas sem duplicar efeitos indevidamente.
- Falhas técnicas não devem expor credenciais, tokens, dados sensíveis ou informações de outros condomínios.
- As regras de prazo, motivo obrigatório e estados canceláveis devem ser implementadas conforme os requisitos consolidados, sem duplicar ou contradizer suas definições.