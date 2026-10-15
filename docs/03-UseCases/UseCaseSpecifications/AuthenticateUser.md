# UC-AUTH-01 - Autenticar-se

## 1. Identificação

- **ID:** UC-AUTH-01
- **Nome:** Autenticar-se
- **Requisito funcional:** RF02 - Autenticação e Controle de Acesso
- **Categoria:** Caso de uso principal
- **Prioridade:** Alta
- **Atores principais:** Morador, Funcionário, Administrador e Master
- **Sistema envolvido:** Plataforma de Gestão Condominial

## 2. Objetivo

Permitir que usuários cadastrados autentiquem sua identidade por meio de credenciais válidas, obtendo um token JWT para acessar os recursos da plataforma de acordo com suas permissões e seu papel no sistema.

## 3. Descrição

O usuário fornece suas credenciais à plataforma. O sistema valida as informações, verifica se a conta pode acessar a plataforma e determina o contexto de acesso correspondente ao seu perfil.

Quando a autenticação é bem-sucedida, o sistema emite um token JWT para autenticar as requisições subsequentes. A autorização de cada operação deve respeitar o controle de acesso baseado em papéis (RBAC) e as regras de isolamento entre condomínios.

## 4. Pré-condições

1. O usuário deve possuir uma conta previamente cadastrada.
2. O usuário deve informar as credenciais exigidas pelo mecanismo de autenticação.
3. A plataforma e o serviço responsável pela autenticação devem estar operacionais.
4. A conta do usuário não pode estar inativada ou excluída logicamente.
5. Para usuários dos papéis `Morador`, `Funcionario` e `Administrador`, deve existir um vínculo válido com um condomínio.
6. O usuário `Master` pode possuir `TenantID` nulo, pois seu acesso é de nível de plataforma.

## 5. Pós-condições

### 5.1. Sucesso

1. A identidade do usuário é validada.
2. O sistema emite um token JWT válido conforme as configurações de segurança da plataforma.
3. O usuário pode realizar requisições autenticadas enquanto o token for válido e o acesso permanecer autorizado.
4. O acesso aos recursos continua sujeito às permissões do perfil e às regras de isolamento multi-tenant.

### 5.2. Falha

1. Nenhum token de autenticação deve ser emitido.
2. O usuário não deve obter acesso autenticado aos recursos protegidos.
3. A falha deve ser comunicada por uma resposta apropriada, sem expor informações sensíveis.

## 6. Fluxo principal

1. O usuário acessa a tela ou o ponto de entrada de autenticação da plataforma.
2. O sistema solicita as credenciais necessárias.
3. O usuário informa suas credenciais e solicita a autenticação.
4. O sistema valida o formato e a presença dos dados recebidos.
5. O sistema localiza a conta correspondente ao identificador informado.
6. O sistema verifica a validade das credenciais fornecidas.
7. O sistema verifica se a conta está ativa e pode utilizar a plataforma.
8. O sistema identifica o papel do usuário: `Morador`, `Funcionario`, `Administrador` ou `Master`.
9. O sistema valida as regras de associação ao tenant:
   - Para `Morador`, `Funcionario` e `Administrador`, deve existir um tenant válido e ativo.
   - Para `Master`, a ausência de `TenantID` é permitida.
10. O sistema gera um token JWT de acordo com as configurações de segurança.
11. O sistema retorna o resultado positivo da autenticação e o token ao cliente.
12. O caso de uso é encerrado com sucesso.

## 7. Fluxos alternativos e de exceção

### FA01 - Credenciais inválidas

1. No passo 6 do fluxo principal, as credenciais não são validadas.
2. O sistema rejeita a autenticação.
3. O sistema retorna uma mensagem genérica de falha, sem revelar se o identificador existe ou se apenas a senha está incorreta.
4. Nenhum token é emitido.
5. O caso de uso é encerrado sem sucesso.

### FA02 - Conta inativada

1. No passo 7, o sistema identifica que a conta está inativada ou excluída logicamente.
2. O sistema rejeita a autenticação.
3. Nenhum token é emitido.
4. O caso de uso é encerrado sem sucesso.

### FA03 - Condomínio encerrado ou indisponível para acesso

1. No passo 9, o sistema identifica que o tenant vinculado está encerrado ou não pode permitir acesso operacional.
2. O sistema bloqueia a autenticação do usuário vinculado.
3. Nenhum token é emitido.
4. O caso de uso é encerrado sem sucesso.

### FA04 - Associação inválida ao tenant

1. No passo 9, o sistema identifica que um usuário `Morador`, `Funcionario` ou `Administrador` não possui um vínculo válido com um tenant.
2. O sistema rejeita a autenticação.
3. Nenhum token é emitido.
4. O caso de uso é encerrado sem sucesso.

### FA05 - Dados de entrada inválidos

1. No passo 4, o sistema identifica campos obrigatórios ausentes ou dados em formato inválido.
2. O sistema informa que os dados enviados precisam ser corrigidos.
3. A autenticação não é processada até que uma nova solicitação válida seja enviada.

### FA06 - Falha técnica durante a autenticação

1. Ocorre uma falha no banco de dados, no serviço de autenticação ou no processo de emissão do token.
2. O sistema registra as informações técnicas necessárias para diagnóstico, sem registrar senhas ou outros segredos.
3. O sistema retorna uma resposta de erro apropriada.
4. Nenhum token deve ser considerado emitido com sucesso se o processo não tiver sido concluído.

### FA07 - Token expirado ou sessão revogada

1. O usuário tenta acessar um recurso protegido utilizando um token expirado ou uma sessão revogada.
2. O mecanismo de autenticação das requisições rejeita o acesso.
3. O usuário deve autenticar-se novamente quando necessário.

**Observação:** FA07 representa um comportamento relacionado ao ciclo de vida da sessão. A validação de tokens em requisições subsequentes e a revogação de sessões também devem ser detalhadas nos requisitos e projetos técnicos correspondentes.

## 8. Regras de negócio

- **RN01:** Somente usuários com credenciais válidas podem concluir a autenticação.
- **RN02:** O sistema deve emitir tokens JWT para autenticação das requisições.
- **RN03:** O sistema deve reconhecer os papéis `Morador`, `Funcionario`, `Administrador` e `Master`.
- **RN04:** Usuários que não sejam `Master` devem possuir associação válida a um tenant.
- **RN05:** O papel `Master` pode operar sem associação a um tenant específico.
- **RN06:** Usuários vinculados a tenants encerrados devem ter o acesso bloqueado, conforme o RF01.1.
- **RN07:** A autenticação não concede autorização irrestrita. Cada operação deve respeitar o RBAC e as regras de isolamento multi-tenant.
- **RN08:** Senhas não devem ser armazenadas em texto puro. O armazenamento e a validação devem respeitar o RNF05 — Segurança de Senhas.
- **RN09:** Segredos e chaves de assinatura JWT devem ser obtidos de configurações seguras, conforme o RNF09.
- **RN10:** A expiração e a revogação das sessões devem respeitar as políticas de segurança definidas para a plataforma.

## 9. Requisitos especiais

- A comunicação HTTP deve utilizar HTTPS com TLS 1.2 ou superior, conforme o RNF08.
- O sistema não deve expor senhas, segredos, chaves de assinatura ou dados sensíveis em mensagens de erro e logs.
- A emissão e a validação dos tokens devem utilizar configurações de segurança consistentes.
- As permissões devem ser verificadas também no backend; ocultar funcionalidades na interface não é suficiente.
- A autenticação e a autorização devem preservar o isolamento entre tenants.
- A duração do token e a estratégia de revogação devem ser definidas na arquitetura de segurança.

## 10. Rastreabilidade

- **RF01:** Arquitetura Multi-tenant.
- **RF01.1:** Inativação e Encerramento de Tenant.
- **RF02:** Autenticação e Controle de Acesso.
- **RF03:** Gestão de Contas (Soft Delete Seletivo).
- **RNF05:** Segurança de Senhas.
- **RNF08:** Criptografia em Trânsito.
- **RNF09:** Separação de Configurações e Segredos.

## 11. Fora do escopo

Não fazem parte deste caso de uso:

- Cadastro de novos usuários.
- Recuperação ou redefinição de senha.
- Gerenciamento administrativo de contas.
- Execução das operações específicas de cada perfil.
- Detalhamento técnico do algoritmo de geração e validação do JWT.
- Implementação do mecanismo de revogação de sessões.

Essas funcionalidades devem ser documentadas separadamente quando aplicáveis.
