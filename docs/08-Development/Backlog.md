# Backlog Inicial do MVP
## Ordem de implementação
Os itens abaixo estão ordenados por dependência e por valor para validar o núcleo operacional. Cada item deve resultar em código, testes e documentação diretamente relacionada.
| ID | Entrega | Escopo | Dependências | Prioridade |
|---|---|---|---|---|
| MVP-01 | Estrutura da solução | Backend, configuração, PostgreSQL, migrações e ambientes locais | Nenhuma | P0 |
| MVP-02 | Modelo de tenant e identidade | Condomínio, usuários, papéis, unidades e vínculos | MVP-01 | P0 |
| MVP-03 | Autenticação e sessões | Login, JWT, refresh token, logout, recuperação e revogação | MVP-02 | P0 |
| MVP-04 | Autorização e isolamento | Políticas de acesso, contexto de tenant e testes de isolamento | MVP-03 | P0 |
| MVP-05 | Espaços comuns | Cadastro, regras básicas e estados operacionais | MVP-02 | P0 |
| MVP-06 | Consulta de disponibilidade | Pesquisa por período, espaço e conflitos | MVP-05 | P0 |
| MVP-07 | Criação e cancelamento de reservas | Regras de elegibilidade, concorrência, histórico e cancelamento | MVP-03, MVP-04, MVP-06 | P0 |
| MVP-08 | Ocorrências e chamados | Abertura, atribuição, máquina de estados e histórico | MVP-03, MVP-04 | P0 |
| MVP-09 | Interface inicial | Fluxos de login, reservas e chamados para morador e administração | MVP-07, MVP-08 | P1 |
| MVP-10 | Auditoria e observabilidade local | Auditoria, logs estruturados e tratamento de erros | MVP-03, MVP-07, MVP-08 | P1 |
| MVP-11 | Testes de aceitação | Testes dos fluxos completos e critérios do MVP | MVP-07, MVP-08, MVP-09 | P0 |
| MVP-12 | Documentação de execução | Instalação, configuração, comandos e dados de demonstração | MVP-01, MVP-11 | P1 |
## Fluxos verticais prioritários
### F1 - Morador realiza uma reserva
1. Usuário autentica-se.
2. Sistema identifica o condomínio e a unidade do usuário.
3. Usuário consulta a disponibilidade.
4. Usuário solicita uma reserva.
5. Backend valida autorização, regras e concorrência.
6. Sistema confirma a reserva e registra o histórico.
7. Usuário consulta ou cancela a reserva.
### F2 - Morador abre e acompanha um chamado
1. Usuário autentica-se.
2. Usuário registra uma ocorrência ou abre um chamado.
3. Administração atribui o chamado a um funcionário.
4. Funcionário altera o estado e registra comentários.
5. Morador acompanha o histórico.
6. Chamado é resolvido, fechado, cancelado ou reaberto conforme as regras.
### F3 - Administrador configura o condomínio
1. Administrador autentica-se.
2. Administrador cadastra usuários, unidades e vínculos.
3. Administrador cadastra espaços comuns.
4. Administrador define regras básicas de reserva.
5. Sistema registra as alterações na auditoria.
## Definition of Done do MVP
Um item do backlog só pode ser considerado concluído quando:
- o comportamento estiver implementado;
- as autorizações e o isolamento por tenant estiverem testados;
- os cenários de erro relevantes estiverem cobertos;
- a documentação diretamente afetada estiver atualizada;
- os testes automatizados passarem;
- o fluxo puder ser executado localmente.