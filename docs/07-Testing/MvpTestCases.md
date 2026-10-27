# Casos de Teste do MVP

Este documento define os casos de teste mínimos para validar o núcleo operacional antes da inclusão de módulos financeiros, IoT ou clientes adicionais.

| ID | Fluxo | Cenário | Resultado esperado |
|---|---|---|---|
| CT-AUTH-01 | Autenticação | Usuário fornece credenciais válidas | Sessão e tokens são emitidos para o tenant autorizado |
| CT-AUTH-02 | Autenticação | Usuário fornece credenciais inválidas | Acesso é rejeitado sem revelar informação sensível |
| CT-TENANT-01 | Isolamento | Usuário consulta recurso de outro condomínio | Operação é rejeitada e nenhum dado é exposto |
| CT-TENANT-02 | Autorização | Morador tenta executar operação administrativa | Operação é rejeitada pelo backend |
| CT-RES-01 | Reserva | Morador cria reserva válida | Reserva é confirmada e aparece no histórico |
| CT-RES-02 | Reserva | Duas solicitações concorrentes ocupam o mesmo horário | No máximo uma reserva é confirmada |
| CT-RES-03 | Reserva | Morador cancela uma reserva válida | Reserva deixa de ocupar a disponibilidade e o cancelamento é registrado |
| CT-RES-04 | Reserva | Solicitação viola antecedência, duração ou capacidade | Reserva é rejeitada com erro de validação |
| CT-TICKET-01 | Chamado | Morador abre chamado válido | Chamado é criado no estado inicial correto |
| CT-TICKET-02 | Chamado | Administrador atribui chamado a funcionário | Atribuição é autorizada e registrada no histórico |
| CT-TICKET-03 | Chamado | Usuário tenta transição de estado inválida | Transição é rejeitada e o estado permanece inalterado |
| CT-TICKET-04 | Chamado | Usuário consulta histórico de chamado autorizado | Histórico é retornado sem dados de outro tenant |
| CT-AUDIT-01 | Auditoria | Usuário altera reserva, chamado ou cadastro relevante | A ação é registrada com ator, data, operação e entidade afetada |

## Critérios de execução

- Os testes devem ser reproduzíveis em ambiente local.
- Os testes de isolamento devem usar pelo menos dois condomínios distintos.
- O teste de concorrência deve executar solicitações simultâneas reais contra a persistência utilizada pelo MVP.
- Cada caso deve indicar se é unitário, integração ou aceitação quando for implementado.

Os IDs desta tabela são referências estáveis para a matriz de rastreabilidade e não significam que os testes já estejam implementados.
