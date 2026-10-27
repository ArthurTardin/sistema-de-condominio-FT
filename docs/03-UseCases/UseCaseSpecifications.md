# Especificações dos Casos de Uso

## 1. Introdução

Este documento descreve o comportamento dos casos de uso identificados no `UseCaseInventory.md` do Sistema de Condomínio.

Cada especificação apresenta objetivo, ator principal, pré-condições, fluxo principal, fluxos alternativos e de exceção, pós-condições e regras de negócio relacionadas.

As especificações devem permanecer consistentes com os requisitos funcionais, o escopo, a arquitetura e os diagramas existentes. Regras ainda não definidas deverão ser confirmadas antes da implementação, sem presumir comportamentos não estabelecidos.

## 2. Padrão de especificação

Cada caso de uso utiliza a seguinte estrutura:

- **ID:** identificador único do caso de uso.
- **Nome:** nome definido no inventário.
- **Objetivo:** resultado esperado pelo ator.
- **Ator principal:** responsável por iniciar a interação.
- **Pré-condições:** condições necessárias para iniciar o fluxo.
- **Gatilho:** evento que inicia o caso de uso.
- **Fluxo principal:** sequência esperada de interações.
- **Fluxos alternativos e de exceção:** variações e falhas relevantes.
- **Pós-condições:** estado esperado após a execução.
- **Regras de negócio:** requisitos e restrições aplicáveis.

## 3. Casos de uso do Morador

### UC-MOR-001 — Autenticar-se

**Requisito relacionado:** RF02

**Objetivo:** permitir que o morador acesse sua conta de forma autenticada e segura.

**Ator principal:** Morador.

**Pré-condições:**
- O morador possui uma conta cadastrada.
- A conta está apta a autenticar-se conforme as regras de estado da conta e do condomínio.

**Gatilho:** o morador solicita acesso ao sistema.

**Fluxo principal:**
1. O morador informa suas credenciais.
2. O sistema valida os dados recebidos.
3. O sistema verifica a situação da conta e as condições de acesso ao condomínio.
4. O sistema autentica o morador.
5. O sistema emite um token de autenticação conforme as regras de segurança.
6. O sistema disponibiliza as funcionalidades autorizadas ao morador.

**Fluxos alternativos e de exceção:**
- **A1 — Credenciais inválidas:** o sistema rejeita a autenticação e informa que não foi possível entrar.
- **A2 — Conta inativa ou removida logicamente:** o sistema bloqueia a autenticação conforme as regras de ciclo de vida da conta.
- **A3 — Condomínio encerrado ou inativo:** o sistema impede o acesso às funcionalidades do tenant, conforme as regras de encerramento contratual.
- **A4 — Falha na autenticação:** o sistema informa a indisponibilidade sem emitir um token válido.
- **A5 — Sessão expirada ou revogada:** o sistema exige nova autenticação para operações protegidas.

**Pós-condições:**
- Em caso de sucesso, existe uma sessão autenticada e um token válido.
- Em caso de falha, nenhuma sessão autenticada válida é concedida.

**Regras de negócio:**
- As permissões são determinadas pelo papel do usuário e pelo contexto do condomínio.
- O acesso a dados de outro condomínio é proibido.
- Tokens, credenciais e sessões devem seguir as regras de segurança definidas na arquitetura.

### UC-MOR-002 — Realizar reservas

**Requisito relacionado:** RF05

**Objetivo:** permitir que o morador reserve um espaço comum disponível no condomínio.

**Ator principal:** Morador.

**Pré-condições:**
- O morador está autenticado.
- O morador possui permissão para realizar reservas.
- O espaço está cadastrado e apto para reserva.

**Gatilho:** o morador solicita uma nova reserva.

**Fluxo principal:**
1. O morador seleciona o espaço comum.
2. O morador informa a data, o horário e os demais dados exigidos para a reserva.
3. O sistema valida os dados informados.
4. O sistema verifica a disponibilidade do espaço no período solicitado.
5. O sistema verifica as regras de antecedência mínima, duração máxima e elegibilidade do morador.
6. O sistema verifica eventuais restrições financeiras que impeçam a reserva.
7. O sistema tenta persistir a reserva respeitando as restrições de concorrência e de não sobreposição.
8. O sistema confirma a reserva somente após a persistência bem-sucedida.
9. O sistema apresenta a confirmação ao morador.

**Fluxos alternativos e de exceção:**
- **A1 — Período indisponível:** o sistema rejeita a solicitação e informa que o horário não está disponível.
- **A2 — Antecedência ou duração inválida:** o sistema rejeita a reserva que não respeite os limites configurados.
- **A3 — Restrição de elegibilidade:** o sistema impede a reserva quando o morador não atende às condições aplicáveis.
- **A4 — Restrição financeira:** o sistema impede a reserva quando houver inadimplência impeditiva, conforme RF16.
- **A5 — Conflito de concorrência:** se outra operação ocupar o período antes da persistência, o sistema rejeita a reserva conflitante.
- **A6 — Falha de persistência:** o sistema não apresenta a reserva como confirmada.
- **A7 — Espaço inativo ou interditado:** o sistema impede a reserva conforme o estado operacional e as regras aplicáveis.

**Pós-condições:**
- Em caso de sucesso, a reserva fica registrada e confirmada.
- Em caso de falha, não deve existir uma nova reserva confirmada para a solicitação rejeitada.

**Regras de negócio:**
- A disponibilidade deve ser validada no momento da operação, não apenas na consulta anterior.
- A prevenção de reservas sobrepostas deve ser garantida também no nível do banco de dados.
- As regras de antecedência, duração e elegibilidade devem respeitar os requisitos e as configurações vigentes.
- A autorização de acesso físico decorrente da reserva segue seu próprio ciclo de vida e não deve ser presumida como sincronizada apenas porque a reserva foi confirmada.

### UC-MOR-003 — Cancelar reservas

**Requisito relacionado:** RF06

**Objetivo:** permitir que o morador solicite o cancelamento de uma reserva própria, respeitando as regras vigentes.

**Ator principal:** Morador.

**Pré-condições:**
- O morador está autenticado.
- A reserva existe e pertence ao contexto do condomínio do morador.
- A reserva está em um estado que permita cancelamento.

**Gatilho:** o morador solicita o cancelamento de uma reserva.

**Fluxo principal:**
1. O morador seleciona uma de suas reservas.
2. O sistema verifica a existência da reserva e sua associação ao morador e ao condomínio.
3. O sistema verifica se o estado atual da reserva permite cancelamento.
4. O sistema valida os limites e as regras de cancelamento aplicáveis.
5. O sistema registra o cancelamento.
6. O sistema processa as atualizações relacionadas, incluindo a autorização de acesso físico, conforme as regras de integração.
7. O sistema apresenta o resultado ao morador.

**Fluxos alternativos e de exceção:**
- **A1 — Reserva inexistente:** o sistema informa que a reserva não foi encontrada ou não pode ser acessada.
- **A2 — Reserva de outro morador:** o sistema impede a operação não autorizada.
- **A3 — Estado incompatível:** o sistema rejeita o cancelamento quando a reserva não puder mais ser cancelada.
- **A4 — Prazo de cancelamento excedido:** o sistema aplica a regra de cancelamento definida nos requisitos.
- **A5 — Falha ao registrar o cancelamento:** o sistema não apresenta a operação como concluída.
- **A6 — Falha na sincronização da autorização de acesso:** o sistema preserva o estado lógico registrado e acompanha a falha de sincronização conforme o mecanismo de processamento e recuperação definido na arquitetura.

**Pós-condições:**
- Em caso de sucesso, o cancelamento está registrado.
- A autorização de acesso associada deve ser tratada conforme seu estado lógico e seu estado de sincronização, sem presumir revogação física instantânea quando houver falha de comunicação.

**Regras de negócio:**
- O morador só pode cancelar reservas que esteja autorizado a gerenciar.
- As regras de prazo e os estados canceláveis devem seguir RF06.
- O cancelamento e a sincronização física são operações relacionadas, mas seus estados não devem ser confundidos.
- O histórico das operações relevantes deve ser preservado conforme as regras de auditoria.

### UC-MOR-004 — Consultar reservas

**Requisitos relacionados:** RF05, RF06

**Objetivo:** permitir que o morador consulte suas reservas e seus estados.

**Ator principal:** Morador.

**Pré-condições:**
- O morador está autenticado.

**Gatilho:** o morador acessa a área de reservas.

**Fluxo principal:**
1. O morador solicita a consulta de suas reservas.
2. O sistema identifica o contexto autenticado do morador e do condomínio.
3. O sistema recupera as reservas às quais o morador tem acesso.
4. O sistema apresenta os dados disponíveis, incluindo espaço, período e estado da reserva.
5. O morador seleciona uma reserva, caso deseje consultar seus detalhes.

**Fluxos alternativos e de exceção:**
- **A1 — Nenhuma reserva encontrada:** o sistema apresenta uma lista vazia e informa que não há reservas para os critérios utilizados.
- **A2 — Filtros inválidos:** o sistema solicita a correção dos critérios de consulta.
- **A3 — Falha de consulta:** o sistema informa que não foi possível recuperar as reservas.

**Pós-condições:**
- Nenhum dado é alterado pela consulta.

**Regras de negócio:**
- O morador só pode consultar reservas autorizadas para sua identidade e seu condomínio.
- Os filtros disponíveis e os estados exibidos devem corresponder ao modelo de reservas definido no projeto.

### UC-MOR-005 — Consultar disponibilidade de espaços

**Requisito relacionado:** RF05

**Objetivo:** permitir que o morador consulte os períodos disponíveis para reservar espaços comuns.

**Ator principal:** Morador.

**Pré-condições:**
- O morador está autenticado.
- Os espaços consultados pertencem ao condomínio ao qual ele tem acesso.

**Gatilho:** o morador solicita a disponibilidade de um espaço ou período.

**Fluxo principal:**
1. O morador seleciona um espaço comum ou os critérios de consulta disponíveis.
2. O morador informa o período desejado.
3. O sistema valida os critérios informados.
4. O sistema consulta as reservas existentes e as condições operacionais do espaço.
5. O sistema calcula ou recupera os períodos disponíveis conforme as regras de reserva.
6. O sistema apresenta os resultados ao morador.

**Fluxos alternativos e de exceção:**
- **A1 — Critérios inválidos:** o sistema solicita a correção dos dados.
- **A2 — Espaço indisponível para uso:** o sistema informa que não há disponibilidade para o espaço ou período consultado.
- **A3 — Nenhum período disponível:** o sistema apresenta o resultado sem horários disponíveis.
- **A4 — Falha de consulta:** o sistema informa que não foi possível obter a disponibilidade.

**Pós-condições:**
- Nenhuma reserva é criada ou alterada pela consulta.

**Regras de negócio:**
- A disponibilidade apresentada é informativa; uma reserva posterior deve validar novamente todas as condições.
- Espaços interditados ou inativos não devem ser apresentados como reserváveis.
- A consulta deve respeitar o isolamento entre condomínios.

### UC-MOR-006 — Registrar ocorrência

**Requisito relacionado:** RF10

**Objetivo:** permitir que o morador registre uma ocorrência relacionada ao condomínio.

**Ator principal:** Morador.

**Pré-condições:**
- O morador está autenticado.
- O morador possui permissão para registrar ocorrências.

**Gatilho:** o morador inicia o registro de uma ocorrência.

**Fluxo principal:**
1. O morador acessa a funcionalidade de registro de ocorrência.
2. O sistema solicita os dados necessários para o registro.
3. O morador informa a descrição e os demais dados exigidos.
4. O sistema valida os campos obrigatórios e os dados recebidos.
5. O sistema associa a ocorrência ao contexto autenticado e ao condomínio correspondente.
6. O sistema persiste a ocorrência.
7. O sistema apresenta a confirmação do registro.

**Fluxos alternativos e de exceção:**
- **A1 — Dados inválidos ou incompletos:** o sistema indica os campos que precisam ser corrigidos.
- **A2 — Sessão inválida:** o sistema exige nova autenticação.
- **A3 — Falha de persistência:** o sistema informa que a ocorrência não pôde ser registrada.
- **A4 — Anexo inválido, caso anexos sejam suportados:** o sistema rejeita o anexo conforme as regras de validação.

**Pós-condições:**
- Em caso de sucesso, a ocorrência fica registrada e associada ao condomínio e ao autor.
- Em caso de falha, o sistema não apresenta um registro não persistido como concluído.

**Regras de negócio:**
- A identidade do autor e o tenant devem ser obtidos de contexto confiável.
- As regras de consulta, alteração, tratamento e histórico de ocorrências devem seguir RF10.
- O registro de uma ocorrência não implica automaticamente a criação de um chamado, salvo quando houver uma regra explícita para isso.

### UC-MOR-007 — Abrir chamado

**Requisito relacionado:** RF11

**Objetivo:** permitir que o morador abra um chamado para solicitar atendimento ou comunicar uma necessidade de manutenção.

**Ator principal:** Morador.

**Pré-condições:**
- O morador está autenticado.
- O morador possui permissão para abrir chamados.

**Gatilho:** o morador solicita a abertura de um chamado.

**Fluxo principal:**
1. O morador acessa a funcionalidade de abertura de chamado.
2. O sistema solicita as informações necessárias.
3. O morador informa o tipo e a descrição da solicitação, além dos demais dados exigidos.
4. O sistema valida as informações.
5. O sistema associa o chamado ao morador e ao condomínio corretos.
6. O sistema cria o chamado no estado inicial permitido.
7. O sistema registra o evento inicial no histórico do chamado.
8. O sistema confirma a abertura ao morador.

**Fluxos alternativos e de exceção:**
- **A1 — Dados incompletos ou inválidos:** o sistema solicita correção.
- **A2 — Tipo de chamado não permitido:** o sistema rejeita o tipo informado.
- **A3 — Falha de persistência:** o sistema não confirma a abertura.
- **A4 — Sessão expirada ou revogada:** o sistema exige nova autenticação.

**Pós-condições:**
- Em caso de sucesso, o chamado está persistido, possui estado inicial válido e tem seu histórico inicial registrado.
- Em caso de falha, o sistema não apresenta um chamado inexistente como aberto.

**Regras de negócio:**
- Os tipos e estados permitidos devem seguir os requisitos do módulo de chamados.
- O ciclo de vida definido para os chamados deve ser respeitado.
- O histórico do chamado deve preservar as transições relevantes, sem permitir alterações arbitrárias de registros históricos.
- Chamados gerados automaticamente por eventos de IoT seguem as regras específicas de automação e não devem ser confundidos com este fluxo iniciado pelo morador.

### UC-MOR-008 — Consultar chamados

**Requisitos relacionados:** RF11, RF13

**Objetivo:** permitir que o morador consulte os chamados aos quais tem acesso.

**Ator principal:** Morador.

**Pré-condições:**
- O morador está autenticado.

**Gatilho:** o morador acessa a área de chamados.

**Fluxo principal:**
1. O morador solicita a listagem de chamados.
2. O sistema identifica o contexto autenticado e as permissões do morador.
3. O sistema recupera os chamados que o morador está autorizado a consultar.
4. O sistema apresenta os dados resumidos e os estados atuais.
5. O morador seleciona um chamado para consultar seus detalhes, caso essa funcionalidade esteja disponível na interface.

**Fluxos alternativos e de exceção:**
- **A1 — Nenhum chamado encontrado:** o sistema apresenta uma lista vazia.
- **A2 — Chamado sem autorização de acesso:** o sistema impede a consulta de dados restritos.
- **A3 — Falha na consulta:** o sistema informa que não foi possível recuperar os chamados.

**Pós-condições:**
- Nenhum dado é alterado pela consulta.

**Regras de negócio:**
- A consulta respeita o isolamento por condomínio e as permissões do morador.
- A visibilidade de chamados deve seguir as regras de acesso definidas nos requisitos; não se deve presumir que todo morador pode consultar chamados de outros moradores.
- O estado apresentado deve corresponder ao estado persistido do chamado.

### UC-MOR-009 — Consultar histórico de chamados

**Requisito relacionado:** RF14

**Objetivo:** permitir que o morador consulte o histórico de um chamado ao qual tem acesso.

**Ator principal:** Morador.

**Pré-condições:**
- O morador está autenticado.
- O chamado existe e pode ser consultado pelo morador.

**Gatilho:** o morador solicita o histórico de um chamado.

**Fluxo principal:**
1. O morador seleciona um chamado.
2. O sistema valida se o morador possui autorização para consultar o chamado.
3. O sistema recupera os registros históricos associados ao chamado.
4. O sistema organiza os eventos em uma sequência compreensível.
5. O sistema apresenta as informações históricas autorizadas.

**Fluxos alternativos e de exceção:**
- **A1 — Chamado inexistente ou inacessível:** o sistema rejeita a consulta.
- **A2 — Histórico vazio:** o sistema apresenta o resultado conforme o estado do chamado e as regras de registro.
- **A3 — Falha na recuperação:** o sistema informa que não foi possível carregar o histórico.

**Pós-condições:**
- Nenhum registro histórico é alterado pela consulta.

**Regras de negócio:**
- O histórico deve refletir os eventos efetivamente registrados.
- Os registros históricos são preservados conforme as regras de auditoria e retenção.
- A consulta não deve permitir a edição ou exclusão de eventos históricos.

### UC-MOR-010 — Consultar cobranças

**Requisito relacionado:** RF16

**Objetivo:** permitir que o morador consulte as cobranças associadas à sua unidade ou às unidades para as quais possui autorização.

**Ator principal:** Morador.

**Pré-condições:**
- O morador está autenticado.
- O morador possui vínculo válido com a unidade correspondente.

**Gatilho:** o morador acessa a área financeira.

**Fluxo principal:**
1. O morador solicita a consulta de cobranças.
2. O sistema valida o vínculo do morador com a unidade e o contexto do condomínio.
3. O sistema recupera as cobranças às quais o morador tem acesso.
4. O sistema apresenta as informações disponíveis, como descrição, valor, vencimento e situação.
5. O morador seleciona uma cobrança para consultar seus detalhes, caso essa opção esteja disponível.

**Fluxos alternativos e de exceção:**
- **A1 — Nenhuma cobrança encontrada:** o sistema apresenta uma lista vazia.
- **A2 — Unidade sem vínculo autorizado:** o sistema impede o acesso às cobranças.
- **A3 — Falha na consulta:** o sistema informa que não foi possível recuperar os dados financeiros.

**Pós-condições:**
- Nenhuma cobrança é criada, alterada ou quitada pela consulta.

**Regras de negócio:**
- O acesso aos dados financeiros deve respeitar as permissões do morador e o vínculo com a unidade.
- A consulta não equivale à confirmação de pagamento.
- As cobranças e seus históricos devem ser preservados conforme as regras de ciclo de vida financeiro do sistema.

### UC-MOR-011 — Gerenciar dados da conta

**Requisitos relacionados:** RF02, RF15

**Objetivo:** permitir que o morador consulte e atualize os dados de sua própria conta, dentro dos limites autorizados.

**Ator principal:** Morador.

**Pré-condições:**
- O morador está autenticado.
- A conta existe e pode ser acessada.

**Gatilho:** o morador acessa as configurações da própria conta.

**Fluxo principal:**
1. O morador solicita a visualização dos dados da conta.
2. O sistema recupera os dados que o morador está autorizado a consultar.
3. O sistema apresenta os dados disponíveis para gerenciamento.
4. O morador seleciona os campos que deseja alterar.
5. O morador informa os novos valores.
6. O sistema valida os dados e as permissões para cada alteração.
7. O sistema persiste as alterações autorizadas.
8. O sistema confirma o resultado.

**Fluxos alternativos e de exceção:**
- **A1 — Dados inválidos:** o sistema solicita correção.
- **A2 — Campo não editável pelo morador:** o sistema impede a alteração e preserva o valor atual.
- **A3 — Tentativa de alterar dados de outro usuário:** o sistema rejeita a operação.
- **A4 — Falha de persistência:** o sistema informa que a alteração não foi concluída.
- **A5 — Alteração que exige validação adicional:** o sistema segue o procedimento de confirmação definido para aquele campo, caso exista.

**Pós-condições:**
- Em caso de sucesso, somente os dados autorizados são atualizados.
- As informações protegidas e os dados de outros usuários permanecem inalterados.

**Regras de negócio:**
- O morador só pode gerenciar os próprios dados, respeitando os campos editáveis.
- Alterações de papel, permissões, vínculo com unidade ou outros dados administrativos não devem ser permitidas por esta funcionalidade sem autorização específica.
- As operações devem respeitar as regras de autenticação, autorização, auditoria e isolamento por condomínio.

### UC-MOR-012 — Consultar notificações

**Requisito relacionado:** RF22

**Objetivo:** permitir que o morador consulte as notificações destinadas a ele.

**Ator principal:** Morador.

**Pré-condições:**
- O morador está autenticado.

**Gatilho:** o morador acessa a área de notificações.

**Fluxo principal:**
1. O morador solicita a lista de notificações.
2. O sistema identifica o destinatário e o contexto autenticado.
3. O sistema recupera as notificações que o morador está autorizado a consultar.
4. O sistema apresenta o conteúdo e os metadados disponíveis, incluindo o estado de leitura, caso esse recurso esteja previsto.
5. O morador seleciona uma notificação para consultar seu conteúdo completo, quando aplicável.

**Fluxos alternativos e de exceção:**
- **A1 — Nenhuma notificação encontrada:** o sistema apresenta uma lista vazia.
- **A2 — Notificação de outro destinatário:** o sistema impede o acesso.
- **A3 — Falha de consulta:** o sistema informa que não foi possível recuperar as notificações.

**Pós-condições:**
- A consulta não altera o conteúdo da notificação.
- Se o sistema registrar leitura automaticamente ao abrir uma notificação, essa alteração deverá seguir a regra específica de leitura definida para o módulo.

**Regras de negócio:**
- O sistema deve respeitar o destinatário e o contexto do condomínio.
- A consulta de notificações não garante que uma mensagem tenha sido entregue por todos os canais externos.
- As regras de entrega, retentativa, leitura e retenção devem seguir RF22 e os requisitos específicos do módulo de notificações.

## 4. Pendências de validação

Os seguintes pontos precisam ser confirmados nos requisitos antes de considerar todas as especificações definitivas:

1. **RF06 — Cancelamento de reservas:** prazo-limite, estados canceláveis e efeitos de cancelamento tardio.
2. **RF05/RF16 — Reservas e cobranças:** quais situações financeiras efetivamente bloqueiam novas reservas.
3. **RF10 — Ocorrências:** campos obrigatórios, anexos, permissões de alteração e eventual vínculo automático com chamados.
4. **RF11/RF13 — Chamados:** quais chamados o morador pode consultar e quais dados ficam visíveis.
5. **RF14 — Histórico:** quais eventos são exibidos ao morador e quais são restritos a funcionários ou administradores.
6. **RF16 — Cobranças:** campos visíveis, vínculo com unidades e eventuais regras para múltiplos moradores por unidade.
7. **RF02/RF15 — Dados da conta:** quais campos o morador pode alterar e quais exigem confirmação adicional.
8. **RF22 — Notificações:** existência de estado de leitura e regra de atualização desse estado.