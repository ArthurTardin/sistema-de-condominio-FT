# 5. Restrições de Integridade do Banco de Dados

As restrições abaixo devem ser implementadas no banco de dados, complementadas por validações no backend quando necessário. Seu objetivo é garantir integridade relacional, consistência dos dados, prevenção de duplicidades e isolamento entre tenants.

## 5.1. Tenant

### UNIQUE (CNPJ)

O CNPJ do condomínio deve ser globalmente único na plataforma.

A unicidade deve ser garantida pelo banco de dados, e o valor deve ser normalizado antes de sua persistência.

O encerramento lógico de um tenant não deve remover seu registro nem liberar sua identidade histórica para associação com outro tenant. A unicidade deve considerar também os registros inativos.

## 5.2. Usuário

### E-mail globalmente único

O e-mail normalizado de `Usuario` deve ser único globalmente no sistema, independentemente do `TenantID` e do perfil (`Morador`, `Funcionario`, `Administrador` ou `Master`).

A comparação deve ser insensível a maiúsculas e minúsculas. Diferenças de capitalização ou espaços externos não devem permitir cadastros duplicados.

A unicidade deve ser garantida pelo banco de dados e pela aplicação.

Usuários `Master` estão sujeitos à mesma regra, mesmo quando possuem `TenantID` nulo.

## 5.3. Unidade

### UNIQUE (TenantID, Bloco, Numero)

A combinação `(TenantID, Bloco, Numero)` deve ser única para unidades do mesmo tenant.

Os campos `TenantID`, `Bloco` e `Numero` são obrigatórios e não podem ser nulos, conforme o modelo de dados adotado.

Tenants diferentes podem utilizar a mesma convenção de endereçamento.

Os campos utilizados na identificação da unidade devem possuir normalização consistente antes da persistência.

## 5.4. Cobrança Condominial

### UNIQUE (TenantID, UnidadeID, Competencia)

Deve existir, no máximo, uma cobrança para determinada unidade e competência dentro do tenant.

A aplicação deve validar a associação entre a unidade e o tenant da cobrança, garantindo que ambos pertençam ao mesmo condomínio.

A competência deve utilizar um formato padronizado, evitando duplicidades decorrentes de diferentes representações da mesma competência.

## 5.5. Vínculo de Usuário e Unidade

### Unicidade dos vínculos ativos

Os vínculos encerrados entre usuários e unidades devem ser preservados para manter o histórico.

Deve existir, no máximo, um vínculo ativo para cada combinação `(TenantID, UsuarioID, UnidadeID)`.

A unicidade deve ser aplicada somente aos vínculos ativos, permitindo a criação de um novo vínculo para a mesma combinação quando o vínculo anterior estiver encerrado.

O tipo de vínculo (`Proprietario`, `Inquilino` ou `Dependente`) deve ser tratado conforme as regras de domínio.

No PostgreSQL, essa regra pode ser implementada por meio de um índice único parcial que considere somente os vínculos ativos.

## 5.6. Dispositivo IoT

### UNIQUE (MacAddress)

O endereço físico `MacAddress` do dispositivo IoT deve possuir unicidade global, independentemente do tenant.

A aplicação deve normalizar o formato do endereço antes de persistir o valor.

A inativação do dispositivo não deve liberar automaticamente o endereço físico para associação a outro dispositivo enquanto o registro original for preservado e a política de identidade determinar que o endereço continua reservado.

## 5.7. Reserva

### Restrição de intervalo de tempo por espaço

O banco de dados deve impedir que duas reservas válidas do mesmo espaço possuam intervalos de tempo sobrepostos.

A restrição deve considerar os estados que ocupam efetivamente a disponibilidade do espaço. Reservas canceladas ou em estados que não bloqueiem o espaço devem ser excluídas do conjunto considerado, conforme as regras de negócio.

A implementação deve considerar os limites de início e fim dos intervalos e definir se reservas consecutivas, nas quais uma termina exatamente quando a outra começa, são permitidas.

A proteção deve funcionar mesmo quando múltiplas transações tentarem criar reservas conflitantes simultaneamente.

No PostgreSQL, deve-se priorizar uma constraint de exclusão sobre intervalos de tempo quando apropriado, ou um mecanismo transacional equivalente que garanta a mesma integridade.

A aplicação deve tratar conflitos de concorrência e violações da restrição, retornando uma resposta controlada ao cliente.

## 5.8. Integridade Referencial e Isolamento entre Tenants

As entidades relacionadas devem possuir chaves estrangeiras para garantir a existência dos registros referenciados.

O sistema deve impedir associações entre entidades de tenants diferentes.

Quando necessário, a integridade do tenant deve ser garantida por constraints compostas, chaves estrangeiras compostas ou mecanismos equivalentes, além das validações do backend.

Os filtros globais do Entity Framework Core não devem ser considerados, isoladamente, uma garantia suficiente de isolamento entre tenants. O backend deve validar o contexto do tenant e as permissões de acesso, enquanto o banco de dados deve aplicar as restrições de integridade possíveis.

Exclusões em cascata não devem remover dados históricos, financeiros ou de auditoria que precisem ser preservados.

O comportamento de exclusão das relações deve ser definido explicitamente, priorizando restrição de exclusão ou desassociação controlada quando houver histórico relevante.

## 5.9. Validações de Domínio no Banco

Além das constraints de unicidade, o banco de dados e a aplicação devem garantir as seguintes invariantes:

- `Reserva.DataHoraFim` deve ser posterior a `Reserva.DataHoraInicio`.
- `AutorizacaoAcesso.ValidoAte` deve ser posterior a `AutorizacaoAcesso.ValidoDe`.
- `CobrancaCondominial.Valor` não pode ser negativo.
- `CobrancaCondominial.DataPagamento` deve ser nula enquanto a cobrança não estiver paga, conforme o modelo financeiro adotado.
- `Usuario.TenantID` deve ser nulo somente para usuários com papel `Master`.
- Usuários que não sejam `Master` devem possuir `TenantID`.
- Entidades relacionadas operacionalmente devem pertencer ao mesmo tenant.
- Uma reserva válida não pode sobrepor outra reserva válida para o mesmo espaço.
- Registros de `HistoricoChamado`, `RegistroAcesso` e `RegistroTelemetria` devem ser tratados como históricos imutáveis por operações comuns.
- Autorizações de acesso não podem estar associadas a credenciais, dispositivos, reservas ou usuários de tenants diferentes.
- Entidades inativadas não devem receber novas operações incompatíveis com seu estado.
- Tarefas assíncronas devem validar o estado atual do tenant antes de executar operações de negócio.
- Mensagens MQTT não podem provocar operações em um tenant diferente daquele ao qual o dispositivo está vinculado.
- Operações de suporte devem respeitar o escopo autorizado e não podem ampliar suas próprias permissões.
- Alterações administrativas devem preservar informações anteriores quando necessárias para auditoria.
- Preferências individuais de notificação não podem desativar notificações obrigatórias.
- Dados biométricos, quando a funcionalidade futura for implementada, devem obedecer às regras específicas de acesso, retenção e proteção de dados.

Sempre que uma invariante puder ser expressa por uma constraint ou restrição nativa do banco de dados, essa proteção deve ser priorizada em conjunto com as validações da aplicação. Regras que dependam de contexto de autorização, estado de negócio ou sistemas externos devem possuir validação explícita no backend.

## 5.10. Índices e Desempenho de Consultas

O banco de dados deve possuir índices adequados para consultas frequentes e operações críticas, considerando os padrões de acesso definidos pela aplicação.

As tabelas operacionais com escopo por tenant devem possuir índices compatíveis com as consultas que filtram por `TenantID` e outros campos utilizados frequentemente.

Os índices devem ser definidos conforme as consultas reais, evitando a criação indiscriminada de índices que prejudiquem operações de escrita.

Consultas críticas, como verificação de disponibilidade de reservas, recuperação de chamados, consulta de notificações e leitura de telemetria, devem ser avaliadas por meio de planos de execução e testes de desempenho.

A criação e a alteração de índices devem ser avaliadas durante a evolução do sistema, considerando o volume de dados, a frequência de leitura e escrita e o custo de manutenção.

## 5.11. Integridade dos Registros Históricos

Os registros de histórico, auditoria, acesso físico e telemetria devem ser protegidos contra alteração ou exclusão por operações comuns da aplicação.

Quando necessário, a proteção deve incluir permissões específicas no banco de dados, separação de responsabilidades e mecanismos de auditoria adicionais.

A política de retenção pode permitir a eliminação controlada de registros após o prazo aplicável, desde que a operação seja autorizada, documentada e compatível com as obrigações legais.

A imutabilidade operacional desses registros não deve impedir procedimentos autorizados de retenção, anonimização ou eliminação quando legalmente cabíveis.

Operações privilegiadas de manutenção não devem permitir que usuários comuns alterem ou removam registros históricos.

## 5.12. Integridade de Operações Assíncronas

Operações assíncronas devem possuir identificadores e mecanismos de idempotência adequados para impedir efeitos duplicados.

Falhas de comunicação não devem provocar automaticamente a repetição de operações financeiras, reservas, autorizações ou notificações sem verificação do estado anterior.

O sistema deve registrar falhas definitivas e permitir reprocessamento controlado conforme as regras de negócio.

Operações que dependam de sistemas externos devem permitir a identificação do resultado, distinguindo operações concluídas, pendentes, falhas e situações em que não seja possível determinar imediatamente se o efeito externo ocorreu.

Tarefas em segundo plano devem verificar permissões, estado do tenant e validade dos dados antes de executar alterações persistentes.

O reprocessamento deve preservar a rastreabilidade da operação original e impedir a duplicação de efeitos já confirmados.

