# MVP - Núcleo Operacional
## 1. Objetivo
Entregar uma primeira versão utilizável para organizar as operações essenciais de um condomínio: acesso seguro, cadastro básico, reservas de espaços comuns e acompanhamento de chamados.
O MVP deve validar o fluxo principal do produto antes da implementação de integrações físicas, gestão financeira avançada e recursos de plataforma.
## 2. Escopo incluído
### 2.1. Fundação e segurança
- Autenticação de usuários por JWT e refresh token.
- Controle de acesso por papéis: `Morador`, `Funcionario` e `Administrador`.
- Isolamento de dados entre condomínios.
- Gestão de sessões, revogação e recuperação de senha.
- Auditoria das operações administrativas e alterações relevantes.
### 2.2. Cadastros essenciais
- Cadastro e manutenção de condomínios.
- Cadastro de usuários.
- Cadastro de unidades.
- Vínculo entre usuários e unidades.
- Cadastro de espaços comuns.
- Configuração das regras básicas de reserva por espaço.
### 2.3. Reservas
- Consulta da disponibilidade de espaços.
- Criação de reservas.
- Consulta de reservas próprias e administrativas.
- Cancelamento de reservas.
- Validação de antecedência, duração, capacidade e elegibilidade.
- Prevenção de conflitos concorrentes.
- Histórico das alterações da reserva.
### 2.4. Chamados
- Registro de ocorrências simples.
- Abertura de chamados por moradores e administradores.
- Atribuição de chamados a funcionários.
- Transições de estado conforme a máquina documentada.
- Comentários e histórico append-only.
- Consulta por solicitante, responsável, estado e período.
- Cancelamento, resolução, fechamento e reabertura conforme autorização.
### 2.5. Entrega mínima da aplicação
- API backend documentada.
- Uma interface inicial para os fluxos de morador e administrador.
- Persistência em PostgreSQL.
- Migrações do banco de dados.
- Testes unitários e de integração dos fluxos de autenticação, reservas e chamados.
- Logs estruturados para falhas e operações relevantes.
## 3. Critérios de aceite do MVP
- Um usuário de um condomínio não consegue consultar ou alterar dados de outro condomínio.
- Duas reservas conflitantes para o mesmo espaço não podem ser confirmadas.
- Uma reserva cancelada deixa de ocupar a disponibilidade.
- As regras de autorização são aplicadas no backend, independentemente do cliente.
- Toda mudança relevante em reserva ou chamado gera histórico consultável.
- Um chamado não pode executar transições de estado não previstas.
- Os fluxos principais possuem testes automatizados reproduzíveis.
- O sistema pode ser executado localmente seguindo instruções documentadas.
## 4. Fora do MVP
Ficam explicitamente fora desta primeira versão:
- Gestão financeira e bloqueios por inadimplência.
- Integração MQTT e telemetria.
- Controle físico de acesso e sincronização com dispositivos.
- Notificações multicanal.
- Reconhecimento facial.
- Dashboard avançado e indicadores operacionais.
- Administração comercial da plataforma, planos e cobrança SaaS.
- Aplicação mobile e cliente desktop nativos.
- Backups automatizados e observabilidade de produção.
Esses itens permanecem como evolução planejada e só devem entrar no desenvolvimento após a validação do núcleo operacional.
## 5. Rastreabilidade principal
| Capacidade | Referências |
|---|---|
| Multi-tenancy e autenticação | RF01, RF01.1, RF02, RF03 |
| Reservas | RF05, RF06, RF07, RF18, RF45 |
| Chamados e ocorrências | RF04, RF10, RF11, RF12, RF13, RF14, RF21 |
| Cadastros | RF15, RF17, RF18 |
| Auditoria e histórico | RF25, RF42, RN13, RN21, RN42, RN43 |
## 6. Status
**Status:** aprovado como recorte inicial para validação e implementação.
O escopo deve ser revisado após a entrega dos fluxos principais e antes da inclusão de novos módulos.