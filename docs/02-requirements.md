# Requisitos do sistema

## Requisitos Funcionais

### Arquitetura e Autenticação

### RF01 - Arquitetura Multi-tenant
O sistema deve suportar múltiplos condomínios simultaneamente. Todo registro no banco de dados (usuários, reservas, dispositivos IoT) deve ser obrigatoriamente associado a um TenantID. O isolamento lógico dos dados entre os condomínios deve ser absoluto.

### RF02 - Autenticação e Controle de Acesso
O sistema deve autenticar usuários (gerando tokens JWT) e aplicar controle de acesso baseado em papéis (Morador, Funcionário, Administrador e Master/Superadmin).

### RF03 - Gestão de Contas (Soft Delete)
A inativação de usuários, unidades ou espaços não deve excluir registros físicos no banco de dados, apenas marcá-los como inativos (Soft Delete) para manter a integridade do histórico financeiro e de ocorrências.

### Operações e Reservas

### RF04 - Máquina de Estados para Chamados
Os chamados não são apenas "gerenciados". O sistema deve aplicar um fluxo estrito de estados: Aberto, Atribuído, Em Andamento, Aguardando Peça/Terceiro, Resolvido e Fechado. A transição de estados deve registrar a data, hora e o usuário responsável (Trilha de Auditoria).

### RF05 - Motor de Reservas
O sistema deve permitir reservas de espaços comuns com base em uma grade de horários parametrizável por espaço. O sistema deve validar regras de antecedência mínima, limite de horas e inadimplência do morador (bloqueando a reserva caso RF10 conste como pendente).

### RF06 - Cancelamento Automatizado
Reservas podem ser canceladas pelos moradores de acordo com o SLA definido no cadastro do espaço.

### RF07 - Dashboard Administrativo
O sistema deve compilar dados em tempo real sobre taxa de ocupação dos espaços, chamados por status (SLA estourado vs no prazo), inadimplência e status da rede IoT.

### Integração IoT

### RF-IoT01 - Emissão de Credencias de Acesso
Ao confirmar uma reserva, o sistema deve gerar um token/hash temporário e publicá-lo via broker MQTT para o microcontrolador da porta correspondente. A credencial deve expirar e ser revogada automaticamente pelo sistema no fim do horário da reserva.

### RF-IoT02 - Telemetria da Infraestrutura
O sistema deve possuir um serviço assíncrono para consumir dados contínuos de sensores.

### RF-IoT03 - Abertura Autônoma de Chamados
Se os dados de telemetria ultrapassarem limites críticos parametrizados pelo administrador, o sistema deve registrar automaticamente uma Ocorrência/Chamado e disparar um alerta visual e sonoro (via WebSockets) para os funcionários online.

### Ocorrências e Chamados

### RF08 - Registro de ocorrências
O sistema deve permitir que moradores registrem ocorrências relacionadas ao condomínio. O sistema deve permitir também que o administrador registre ocorrências recebidas por outros canais (telefone, presencial).

### Financeiro

### RF09 - Gerenciamento de taxa condominial
O sistema deve permitir que o administrador cadastre o valor da taxa condominial por unidade/apartamento

### RF10 - Consulta de status de pagamento
O sistema deve permitir que o morador visualize o status de pagamento (pago/pendente) da taxa condominial de sua unidade.

### Estrutura Física

### RF11 - Gerenciamento de unidades
O sistema deve permitir o cadastro e gerenciamento das unidades do condomínio.

### RF12 - Gerenciamento de apartamentos
O sistema deve permitir o cadastro e gerenciamento de apartamentos nas unidades dos condomínio.

### RF13 - Gerenciamento de espaços
O sistema deve permitir o cadastro e gerenciamento dos espaços comuns disponíveis no condomínio.

### Calendário e Notificações

### RF14 - Calendário
O sistema deve disponibilizar um calendário do condomínio com reservas, eventos e avisos, por dia, semana e mês.

### RF15 - Notificações
O sistema deve notificar o morador e funcionário sobre reservas, alteração nos chamados e avisos.

## Requisitos Não-Funcionais

### RNF01 - Tempo de Resposta
As APIs de backend devem ter tempo de processamento interno inferior a 200 milissegundos no pilar 95 (P95), exceto para rotinas pesadas de relatórios.

### RNF02 - Processamento Assíncrono
O backend deve ser construído para evitar o esgotamento de threads durante picos de acesso ou falhas na rede IoT. Toda comunicação de rede e I/O de banco de dados deve utilizar programação assíncrona baseada em Task (async/await).

### RNF03 - Prevenção de Travamentos (Timeouts)
Todas as chamadas externas, integrações e consultas demoradas devem implementar obrigatoriamente um CancellationToken. Caso a requisição ultrapasse 5 segundos, ela deve ser abortada imediatamente para liberar o servidor.

### RNF04 - Controle de Concorrência (Race Conditions)
O banco de dados e a API devem implementar travas otimistas (Optimistic Concurrency Control) para garantir que, caso dois moradores tentem reservar o mesmo horário no mesmo milissegundo, a segunda transação falhe de forma tratada e avise o usuário, impedindo duplicação (double-booking).

### Segurança e IoT

### RNF05 - Segurança de Senhas
Senhas devem ser armazenadas utilizando algoritmos de hashing seguros e com salt (ex: Bcrypt, Argon2id).

### RNF06 - Latência IoT(MQTT)
A comunicação entre os nós IoT e a central de gestão deve usar o protocolo MQTT sobre TCP/IP para garantir latência inferior a 1 segundo no acionamento de portas e envio de telemetria.

### RNF07 - Edge Computing (Fallback IoT)
Os microcontroladores das travas eletrônicas devem possuir capacidade de armazenamento local (cache). Em caso de queda de internet, o hardware deve conseguir validar acessos pré-agendados para as próximas 24 horas consultando sua própria memória.

### RNF08 - Criptografia em Trânsito
Todo o tráfego HTTP deve ser forçado via TLS 1.2+ (HTTPS) e os tópicos MQTT devem exigir autenticação de cliente (usuário/senha do device).

### Conformidade e Operação

### RNF09 - Separação de Configurações
Credenciais de banco, chaves JWT e URIs do broker MQTT devem ser lidas estritamente a partir de variáveis de ambiente do SO.

### RNF10 - LGPD e Retenção
Os logs de acesso físico aos espaços devem ser anonimizados ou descartados automaticamente após 90 dias, mantendo apenas dados agregados para estatísticas de uso.