# UC-RES-01 - Realizar Reserva

## 1. Identificação

- **Código:** UC-RES-01
- **Nome:** Realizar Reserva
- **Módulo:** Espaços Comuns e Reservas
- **Objetivo:** Permitir que um usuário autorizado solicite a reserva de um espaço comum para uma data e um período específicos.
- **Requisito associado:** RF05

## 2. Atores

- **Ator principal:** Morador
- **Ator secundário:** Administrador
- **Sistemas envolvidos:** Backend da plataforma, banco de dados e, quando aplicável, subsistema de controle de acesso IoT.

## 3. Pré-condições

- O usuário está autenticado.
- O usuário possui permissão para realizar a operação no condomínio correspondente.
- O espaço comum existe e pertence ao condomínio do contexto da operação.

## 4. Gatilho

O usuário seleciona um espaço comum, uma data e um horário e solicita a criação de uma reserva.

## 5. Fluxo principal

1. O usuário informa o espaço, a data e o período desejado.
2. O sistema valida a autenticação, as permissões e o contexto do condomínio.
3. O sistema verifica se o espaço está operacionalmente disponível.
4. O sistema verifica a disponibilidade do horário solicitado.
5. O sistema valida as regras de negócio aplicáveis à reserva.
6. O sistema verifica se existe algum bloqueio financeiro aplicável à unidade.
7. O sistema inicia a operação transacional e tenta persistir a reserva, respeitando as restrições contra sobreposição de horários.
8. O sistema registra o evento correspondente no histórico da reserva.
9. O sistema confirma a transação.
10. Se o espaço exigir controle de acesso IoT, o sistema solicita a geração da autorização correspondente.
11. O sistema agenda a notificação de confirmação.
12. O sistema informa ao usuário que a reserva foi criada.

## 6. Fluxos alternativos e exceções

- **A1 — Espaço indisponível:** o sistema informa que o espaço não pode ser utilizado.
- **A2 — Horário ocupado:** o sistema informa que o período solicitado não está disponível.
- **A3 — Regras de negócio violadas:** o sistema informa qual restrição impediu a reserva.
- **A4 — Bloqueio financeiro:** o sistema informa que a situação financeira impede a operação, conforme as regras do condomínio.
- **A5 — Conflito de concorrência:** se outra operação reservar o mesmo período, o sistema rejeita a operação conflitante e informa que o horário deixou de estar disponível.
- **A6 — Falha na persistência:** o sistema reverte a transação e informa que não foi possível concluir a reserva.
- **A7 — Falha na autorização IoT:** o sistema registra a falha e aplica a política definida para geração e retentativa da autorização.
- **A8 — Falha na notificação:** o sistema mantém o resultado da reserva e trata o envio conforme a política de retentativas definida na arquitetura.

## 7. Pós-condições

### Em caso de sucesso

- A reserva está persistida e confirmada.
- O histórico contém o evento de criação.
- A disponibilidade do horário reflete a reserva confirmada.
- Quando aplicável, a autorização de acesso é gerada ou fica registrada para processamento.
- A notificação de confirmação fica agendada para envio.

### Em caso de falha antes da confirmação

- A reserva não é confirmada parcialmente.
- As alterações obrigatórias da transação são revertidas.
- O sistema informa o motivo da falha, quando possível.

## 8. Regras de negócio

- A reserva deve respeitar as regras de antecedência, duração e capacidade configuradas.
- A reserva deve pertencer ao condomínio do contexto autenticado.
- Não podem existir reservas conflitantes para o mesmo espaço e período.
- A reserva pode ser bloqueada por inadimplência quando as regras aplicáveis assim determinarem.
- A criação da reserva e o registro obrigatório do histórico devem ser consistentes.
- A geração e a sincronização da autorização IoT devem respeitar as regras de controle de acesso do projeto.

## 9. Requisitos relacionados

Preencher com os identificadores dos requisitos funcionais e não funcionais efetivamente relacionados a este caso de uso.

## 10. Observações e decisões pendentes

- Definir o comportamento final quando a autorização IoT não puder ser gerada.
- Confirmar quais perfis podem reservar espaços em nome de uma unidade.
- Confirmar as regras específicas de bloqueio financeiro e antecedência.
