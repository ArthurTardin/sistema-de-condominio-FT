# Requisitos Não Funcionais

## 1. Desempenho e Processamento

### RNF01 - Tempo de Resposta

As APIs do backend devem apresentar tempo de resposta inferior a 200 milissegundos no percentil 95 (P95) para operações comuns, exceto para operações classificadas como pesadas, incluindo relatórios extensos, exportações e processamento analítico.

A medição deve considerar o tempo de processamento da API, incluindo as operações internas necessárias para atender à requisição. Quando tecnicamente possível, a latência de sistemas externos deve ser medida separadamente para evitar atribuir à aplicação atrasos que estejam fora de seu controle.

O P95 deve ser calculado sobre uma janela de medição representativa e acompanhado por ferramentas de monitoramento e testes de desempenho.

As operações classificadas como pesadas devem possuir metas próprias, definidas de acordo com sua finalidade, volume de dados e criticidade.

**Critérios de validação:**

- Validar a meta de P95 inferior a 200 ms por meio de testes de carga.
- Definir previamente a quantidade esperada de usuários simultâneos e a taxa de requisições.
- Registrar latência, taxa de erros e utilização dos recursos envolvidos.
- Avaliar separadamente operações pesadas, como relatórios extensos e processamento de grandes volumes de telemetria.
- Documentar as condições do ambiente e os resultados dos testes para permitir comparações futuras.

### RNF02 - Processamento Assíncrono

O backend deve utilizar programação assíncrona baseada em `Task` e `async/await` para operações de entrada e saída, incluindo consultas ao banco de dados, chamadas de rede, comunicação com serviços externos e mensageria, quando suportado pelas bibliotecas utilizadas.

O processamento assíncrono deve evitar o bloqueio desnecessário de threads durante períodos de espera por recursos externos ou operações de entrada e saída.

Operações intensivas em CPU devem ser tratadas de acordo com sua natureza, evitando o uso indiscriminado de processamento assíncrono quando não houver benefício técnico.

O sistema deve evitar chamadas síncronas bloqueantes desnecessárias em fluxos de execução assíncronos.

### RNF03 - Cancelamento de Operações e Timeouts

As chamadas externas, integrações e operações potencialmente demoradas devem aceitar e propagar `CancellationToken` sempre que houver suporte técnico.

Operações sujeitas à espera por banco de dados, serviços externos, rede ou mensageria devem possuir políticas de timeout compatíveis com sua criticidade.

Os timeouts devem impedir esperas indefinidas e permitir a liberação adequada dos recursos utilizados.

O cancelamento de uma requisição não deve ser interpretado automaticamente como garantia de que uma operação já confirmada no banco de dados ou em um sistema externo foi revertida.

Operações que possam produzir efeitos persistentes devem utilizar transações, mecanismos de idempotência ou consultas de estado para determinar o resultado após cancelamentos, interrupções ou falhas de comunicação.

Os limites de timeout devem ser definidos por categoria de operação, considerando o tempo esperado de execução e os limites dos sistemas dependentes.

---

## 2. Concorrência e Integridade

### RNF04 - Controle de Concorrência e Prevenção de Reservas Duplicadas

O sistema deve implementar mecanismos de controle de concorrência para evitar inconsistências causadas por operações simultâneas.

A entidade `Reserva` deve utilizar controle de concorrência otimista, por meio de um campo de versão ou mecanismo equivalente compatível com PostgreSQL, para detectar alterações concorrentes incompatíveis.

A implementação não deve presumir que o tipo `rowversion` do SQL Server existe nativamente no PostgreSQL. O controle de versão pode ser implementado por uma coluna atualizada pela aplicação ou por mecanismo equivalente devidamente definido.

Além do controle de concorrência otimista, o banco de dados deve impedir que duas reservas que ocupem efetivamente a disponibilidade do mesmo espaço possuam intervalos de tempo sobrepostos.

Essa proteção deve funcionar mesmo quando duas ou mais requisições tentarem criar ou alterar reservas conflitantes simultaneamente.

No PostgreSQL, deve-se priorizar uma constraint de exclusão sobre intervalos de tempo, quando aplicável, ou outro mecanismo transacional que ofereça garantia equivalente de integridade.

A restrição deve considerar os estados da reserva que efetivamente bloqueiam o espaço, excluindo estados que, conforme as regras de negócio, não ocupem sua disponibilidade.

Os intervalos devem possuir limites definidos de maneira consistente, permitindo reservas consecutivas quando uma terminar exatamente no momento em que a outra começar, desde que as regras de negócio permitam.

Uma tentativa de criação ou alteração de reserva conflitante deve falhar de forma controlada, sem produzir duplicidade de ocupação.

A API deve retornar uma resposta apropriada, informando que o horário deixou de estar disponível ou que ocorreu um conflito de concorrência.

**Critérios de validação:**

- Executar testes com requisições simultâneas para o mesmo espaço e intervalo.
- Garantir que, entre reservas conflitantes, no máximo uma operação seja confirmada.
- Verificar que reservas canceladas ou em estados que não bloqueiam disponibilidade não impeçam novas reservas.
- Confirmar que conflitos sejam tratados sem expor detalhes internos do banco de dados.
- Verificar a integridade após falhas e tentativas simultâneas de alteração.

---

## 3. Segurança e IoT

### RNF05 - Segurança de Senhas e Credenciais

As senhas dos usuários não devem ser armazenadas em texto puro, por criptografia reversível ou em formato que permita sua recuperação direta.

O sistema deve utilizar algoritmos modernos de derivação de senha com salt individual, como:

- Argon2id;
- bcrypt.

Os parâmetros de custo devem ser configuráveis e ajustados conforme as recomendações de segurança vigentes e os recursos disponíveis.

A implementação deve permitir a evolução dos parâmetros de hashing e a migração gradual para algoritmos mais fortes quando necessário.

Tokens de recuperação, tokens de atualização de sessão e outros segredos de autenticação devem ser protegidos contra exposição. Quando aplicável, devem ser armazenados somente em formato de hash ou por meio de mecanismo seguro equivalente.

Senhas, tokens completos, hashes de autenticação e segredos não devem ser expostos em logs, mensagens de erro ou respostas da API.

As credenciais utilizadas por dispositivos IoT e integrações devem possuir mecanismos próprios de proteção, rotação e revogação, independentes das senhas dos usuários.

### RNF06 - Integridade Física e Segurança MQTT

O sistema deve garantir a unicidade global do endereço físico `MacAddress` dos dispositivos IoT cadastrados, conforme a política de identidade definida para os dispositivos.

Os tópicos MQTT devem possuir estrutura hierárquica e escopo por tenant, impedindo colisões e associações indevidas entre dispositivos de condomínios diferentes.

A convenção de tópicos deve seguir um padrão consistente, como:

`tenant/{tenantId}/devices/{deviceId}/...`

O broker MQTT deve aplicar autenticação e autorização para publicação e assinatura de mensagens.

Cada dispositivo deve utilizar uma identidade própria ou mecanismo equivalente de autenticação individualizada, com permissões limitadas aos tópicos necessários para seu funcionamento.

Um dispositivo não deve conseguir publicar ou consumir mensagens de outro dispositivo ou tenant sem autorização explícita.

O backend deve validar a identidade autenticada do dispositivo, o tenant ao qual ele está vinculado e a consistência das mensagens recebidas. O sistema não deve confiar exclusivamente no `tenantId` ou `deviceId` informado no tópico ou no conteúdo da mensagem.

As credenciais de comunicação devem ser armazenadas de forma segura e possuir procedimentos de revogação e substituição quando necessário.

Mensagens inválidas, não autenticadas ou incompatíveis com o dispositivo de origem devem ser rejeitadas e registradas de acordo com a política de segurança e auditoria.

### RNF07 - Edge Computing e Operação Offline de Dispositivos de Acesso

Os dispositivos responsáveis pelo controle de acesso físico devem possuir capacidade de armazenamento local seguro para operação temporariamente desconectada, quando suportado pelo hardware.

Em caso de indisponibilidade da conexão com a plataforma, o dispositivo deve conseguir validar localmente autorizações previamente sincronizadas para as próximas 24 horas, respeitando as regras de segurança definidas para a infraestrutura.

A validade máxima dos dados armazenados localmente deve ser configurável conforme o tipo de dispositivo e a criticidade do acesso.

A política de armazenamento e sincronização local deve definir:

- Expiração das autorizações.
- Sincronização com a plataforma.
- Atualização das permissões.
- Tratamento de revogações.
- Integridade e proteção dos dados armazenados.
- Comportamento durante indisponibilidade da rede.
- Recuperação após reconexão.
- Tratamento de conflitos entre o estado local e o estado central.
- Proteção contra repetição indevida de mensagens ou comandos.

O sistema deve considerar que uma autorização revogada na plataforma pode permanecer disponível em um dispositivo desconectado até que a revogação seja sincronizada ou que a autorização expire.

Esse risco deve ser tratado por meio de limites de validade, políticas locais de segurança e procedimentos de sincronização.

Para acessos classificados como críticos, a política deve definir explicitamente se o acesso será permitido ou negado durante a indisponibilidade da plataforma.

A sincronização posterior deve preservar a rastreabilidade dos eventos ocorridos offline, incluindo o horário do evento no dispositivo e o horário de recebimento pelo backend, quando disponíveis.

### RNF08 - Criptografia em Trânsito

Todo o tráfego HTTP da aplicação deve utilizar HTTPS com TLS 1.2 ou superior.

As comunicações MQTT devem utilizar transporte seguro, preferencialmente MQTT sobre TLS, com autenticação apropriada dos dispositivos.

As integrações externas devem utilizar canais criptografados e validar certificados conforme as políticas de segurança adotadas.

Credenciais e segredos não devem ser transmitidos em parâmetros de URL ou incluídos em mensagens de erro e logs.

Os protocolos e algoritmos criptográficos utilizados devem ser mantidos de acordo com as recomendações de segurança vigentes.

---

## 4. Configuração, Operação e Conformidade

### RNF09 - Separação de Configurações e Segredos

Credenciais de banco de dados, chaves JWT, credenciais de serviços externos, certificados privados e configurações sensíveis do broker MQTT não devem ser armazenados diretamente no código-fonte ou em repositórios de versionamento.

Essas configurações devem ser fornecidas por mecanismos externos, como variáveis de ambiente ou serviços de gerenciamento de segredos.

Os segredos devem possuir acesso restrito, proteção contra exposição acidental e procedimentos de rotação e revogação quando aplicável.

Os ambientes de desenvolvimento, testes e produção devem utilizar configurações e credenciais separadas.

A aplicação não deve disponibilizar segredos por meio de endpoints de diagnóstico, logs, arquivos públicos ou interfaces administrativas comuns.

Os logs e arquivos de configuração devem ser revisados para impedir a exposição acidental de informações sensíveis.

### RNF10 - LGPD e Retenção de Dados

Os dados operacionais, registros de acesso físico, credenciais, dados de telemetria, informações cadastrais e demais dados pessoais devem possuir políticas de tratamento e retenção compatíveis com sua finalidade, necessidade operacional, segurança e obrigações legais aplicáveis.

As políticas devem ser definidas por categoria de dado, identificando:

- Finalidade do tratamento.
- Responsáveis e perfis autorizados.
- Prazo de retenção ou critério para sua definição.
- Condições de acesso.
- Condições de anonimização ou eliminação.
- Obrigações de preservação.
- Procedimentos para atendimento dos direitos dos titulares, quando aplicáveis.
- Medidas de segurança adequadas ao risco.

Quando apropriado, o sistema deve utilizar:

- Retenção temporal.
- Anonimização efetiva.
- Agregação estatística.
- Eliminação controlada.
- Restrição de acesso.
- Minimização dos dados coletados.

A anonimização deve impedir a identificação do titular por meios razoavelmente disponíveis, considerando os riscos de reidentificação. A simples remoção do nome ou de outro identificador direto não deve ser considerada suficiente automaticamente.

A aplicação das políticas de retenção não deve comprometer dados cuja preservação seja necessária para cumprir obrigações legais, fiscais, contratuais ou de auditoria devidamente identificadas.

Os registros de histórico, acesso, auditoria e telemetria não devem ser considerados de retenção ilimitada apenas por serem tratados como registros históricos ou imutáveis durante as operações comuns. Sua preservação deve obedecer a políticas documentadas de retenção e descarte autorizado.

O sistema deve permitir a identificação e o controle de dados sujeitos a diferentes prazos de retenção.

Dados biométricos destinados ao reconhecimento facial devem possuir avaliação específica de necessidade, finalidade, base legal, segurança, retenção e direitos dos titulares antes da implementação da funcionalidade.

A implementação futura do reconhecimento facial deve considerar os riscos associados ao tratamento de dados biométricos, incluindo acesso indevido, vazamento, uso incompatível com a finalidade e impossibilidade prática de substituição de características biométricas comprometidas.

### RNF11 - Objetivos de Recuperação

O sistema deve definir objetivos de recuperação de serviço (RTO) e de recuperação de dados (RPO), considerando a criticidade das operações, os requisitos de continuidade do negócio e a infraestrutura contratada.

A estratégia de backup e restauração deve ser compatível com os objetivos definidos.

Os backups devem possuir controles de acesso, proteção contra alteração ou exclusão não autorizada e medidas de segurança adequadas aos dados armazenados.

A capacidade de recuperação deve ser verificada por testes periódicos de restauração, com registro dos resultados e das falhas encontradas.

Os valores finais de RTO e RPO devem ser estabelecidos antes da entrada em produção.