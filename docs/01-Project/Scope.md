# Escopo do Projeto

## 1. Visão Geral

Este documento define o escopo do Sistema de Gestão Condominial, delimitando as funcionalidades previstas para o produto, as funcionalidades planejadas para versões futuras e os itens que não fazem parte do escopo atual.

O escopo serve como referência para a definição dos requisitos, arquitetura, desenvolvimento, testes e evolução do sistema, evitando a inclusão descontrolada de funcionalidades durante o projeto.

## 2. Escopo do MVP

O Produto Mínimo Viável (MVP) foi reduzido ao núcleo operacional do sistema. A primeira versão deve validar autenticação, isolamento entre condomínios, cadastros essenciais, reservas e chamados antes da inclusão de integrações físicas e módulos financeiros.

### 2.1. Autenticação e controle de acesso

- Autenticação de usuários.
- Controle de acesso baseado em papéis e permissões.
- Gerenciamento de sessões.
- Isolamento de dados entre condomínios.
- Gestão de sessões e recuperação de senha.
- Auditoria das operações relevantes.

### 2.2. Gestão condominial

- Gerenciamento de condomínios.
- Gerenciamento de usuários.
- Gerenciamento de unidades e vínculos entre usuários e unidades.
- Gerenciamento de espaços comuns.
- Configuração das regras básicas de reserva dos espaços.

### 2.3. Reservas

- Consulta de disponibilidade de espaços comuns.
- Criação de reservas.
- Consulta de reservas.
- Cancelamento de reservas.
- Validação das regras de reserva.
- Aplicação de restrições de capacidade, duração e antecedência.
- Prevenção de conflitos de horário.
- Histórico das alterações da reserva.

### 2.4. Ocorrências e chamados

- Registro de ocorrências.
- Abertura de chamados.
- Atribuição de chamados aos responsáveis.
- Gerenciamento dos estados dos chamados.
- Consulta de chamados.
- Consulta do histórico de alterações.
- Transições de estado conforme as regras do chamado.
- Acompanhamento básico de prazos, quando aplicável ao fluxo do MVP.

### 2.5. Entrega técnica mínima
- API backend documentada.
- Persistência em PostgreSQL e migrações.
- Interface inicial para morador e administração.
- Testes automatizados dos fluxos principais.
- Instruções para execução local.

## 3. Funcionalidades previstas para versões futuras

As funcionalidades desta seção representam possibilidades de evolução do produto. Elas não fazem parte do MVP atual e somente devem entrar no desenvolvimento após revisão do escopo, dos requisitos, da arquitetura e dos testes.

### 3.1. Reconhecimento facial

- Reconhecimento facial em nuvem para identificação de moradores.
- Integração do reconhecimento facial com o controle de acesso físico.
- Gerenciamento dos processos de identificação e autorização associados à biometria facial.

Por envolver dados biométricos, essa funcionalidade deverá ter seus requisitos de segurança, privacidade, retenção e conformidade com a LGPD avaliados antes da implementação.

### 3.2. Gestão financeira avançada

- Funcionalidades contábeis completas.
- Integração com sistemas financeiros externos.
- Integração com gateways de pagamento.
- Conciliação automática de pagamentos.
- Relatórios financeiros avançados.
- Processos ampliados de cobrança e recuperação de inadimplência.

### 3.3. Evolução da integração com IoT

- Integração com novos tipos de sensores e atuadores.
- Manutenção preditiva da infraestrutura.
- Análise avançada de anomalias.
- Análises históricas e avançadas de telemetria.
- Aplicação de modelos de aprendizado de máquina à análise da infraestrutura.
- Integração com novas plataformas e protocolos de IoT.

### 3.4. Evolução da administração da plataforma

- Análises avançadas de utilização de recursos.
- Provisionamento automatizado de condomínios.
- Gestão ampliada de assinaturas e planos.
- Novas integrações externas.
- Indicadores avançados para administração e suporte.

### 3.5. Módulos adiados após o núcleo operacional
- Gestão operacional de ativos e recursos da infraestrutura.
- Gestão financeira básica e bloqueios por inadimplência.
- Notificações multicanal.
- Integração MQTT, telemetria e detecção automática de falhas.
- Controle físico de acesso e sincronização de autorizações.
- Geração automática de chamados a partir de eventos de infraestrutura.
- Dashboards operacionais avançados.
- Administração comercial da plataforma, planos e cobrança SaaS.
- Aplicações mobile e desktop nativas.
- Backups automatizados e observabilidade de produção.

## 4. Itens fora do escopo

Não fazem parte do escopo atual, salvo decisão formal de expansão do produto:

- Sistema completo de contabilidade empresarial.
- Serviços bancários.
- Prestação de serviços jurídicos condominiais.
- Gestão de recursos humanos e folha de pagamento.
- Fabricação de dispositivos IoT.
- Instalação física e manutenção material da infraestrutura condominial.
- Substituição integral dos sistemas e equipamentos físicos existentes.
- Automação predial genérica sem relação com as funcionalidades previstas para o condomínio.
- Funcionalidades empresariais sem relação direta com os objetivos do produto.

## 5. Limites de responsabilidade do sistema

O sistema será responsável pelo gerenciamento digital das operações condominiais, pela integração lógica com dispositivos físicos e pelo processamento dos eventos recebidos.

A instalação de sensores, a manutenção física de equipamentos, os reparos de infraestrutura e a fabricação de dispositivos não são responsabilidades diretas da plataforma.

Os dispositivos IoT serão tratados como componentes externos integrados ao sistema. A plataforma deverá gerenciar a comunicação, as autorizações, o monitoramento, o processamento de eventos e a representação digital de seus estados operacionais.

## 6. Controle de alterações do escopo

Alterações no escopo deverão ser avaliadas considerando:

- Os objetivos do produto.
- Os requisitos funcionais e não funcionais.
- As regras de negócio.
- A arquitetura do sistema.
- O modelo de dados.
- Os requisitos de segurança e privacidade.
- O impacto no desenvolvimento e nos testes.
- O impacto operacional e de manutenção.
- As dependências com funcionalidades já existentes.

Funcionalidades novas ou significativamente ampliadas deverão ser registradas e avaliadas antes de sua inclusão no MVP.

Alterações aprovadas deverão refletir-se nos documentos afetados, preservando a rastreabilidade entre escopo, requisitos, arquitetura e testes.

## 7. Status do escopo

**Status:** recorte do MVP validado: núcleo operacional. O detalhamento continua sujeito à revisão durante a implementação.

Este documento estabelece uma referência para o planejamento do produto. A inclusão de uma funcionalidade neste escopo não significa que ela já esteja implementada ou validada.

O escopo deverá permanecer consistente com os requisitos consolidados e com as decisões registradas durante o desenvolvimento do projeto.