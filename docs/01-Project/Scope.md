# Escopo do Projeto

## 1. Visão Geral

Este documento define o escopo do Sistema de Gestão Condominial, delimitando as funcionalidades previstas para o produto, as funcionalidades planejadas para versões futuras e os itens que não fazem parte do escopo atual.

O escopo serve como referência para a definição dos requisitos, arquitetura, desenvolvimento, testes e evolução do sistema, evitando a inclusão descontrolada de funcionalidades durante o projeto.

## 2. Escopo do MVP

O Produto Mínimo Viável (MVP) contempla as funcionalidades essenciais para a gestão condominial, incluindo operações administrativas, processos operacionais, monitoramento da infraestrutura e integração com dispositivos IoT.

### 2.1. Autenticação e controle de acesso

- Autenticação de usuários.
- Controle de acesso baseado em papéis e permissões.
- Gerenciamento de sessões.
- Isolamento de dados entre condomínios.
- Gerenciamento de credenciais de acesso físico.
- Geração, consulta e revogação de autorizações de acesso.
- Registro de eventos de acesso.

### 2.2. Gestão condominial

- Gerenciamento de condomínios.
- Gerenciamento de usuários.
- Gerenciamento de unidades e vínculos entre usuários e unidades.
- Gerenciamento de espaços comuns.
- Gerenciamento de configurações do condomínio.
- Gerenciamento operacional de ativos e recursos da infraestrutura.

### 2.3. Reservas

- Consulta de disponibilidade de espaços comuns.
- Criação de reservas.
- Consulta de reservas.
- Cancelamento de reservas.
- Validação das regras de reserva.
- Aplicação de restrições de capacidade, duração e antecedência.
- Prevenção de conflitos de horário.
- Aplicação de restrições financeiras às reservas, conforme as regras definidas.
- Geração e gerenciamento de autorizações de acesso vinculadas às reservas, quando aplicável.

### 2.4. Ocorrências e chamados

- Registro de ocorrências.
- Abertura de chamados.
- Atribuição de chamados aos responsáveis.
- Gerenciamento dos estados dos chamados.
- Consulta de chamados.
- Consulta do histórico de alterações.
- Acompanhamento de prazos e SLA.
- Geração automática de chamados a partir de eventos relevantes da infraestrutura.

### 2.5. Gestão financeira básica

- Gerenciamento de cobranças condominiais.
- Consulta de cobranças.
- Identificação de cobranças pendentes e situações de inadimplência.
- Aplicação de restrições financeiras às operações, quando previsto nas regras de negócio.
- Processamento automático de eventos relacionados ao vencimento das cobranças.

O MVP não tem como objetivo substituir sistemas completos de contabilidade, gestão empresarial ou serviços bancários.

### 2.6. Notificações

- Notificações relacionadas a reservas.
- Notificações relacionadas a chamados e ocorrências.
- Notificações de eventos operacionais.
- Alertas relacionados à infraestrutura e aos dispositivos IoT.
- Processamento automático de notificações.
- Consulta e gerenciamento das notificações disponibilizadas aos usuários.

### 2.7. Infraestrutura e integração com IoT

- Cadastro e gerenciamento de dispositivos IoT.
- Comunicação com dispositivos por meio do protocolo MQTT.
- Recebimento e processamento de telemetria.
- Monitoramento da infraestrutura física.
- Identificação de condições anormais de operação.
- Geração de alertas a partir de eventos de telemetria.
- Geração automática de chamados para eventos relevantes.
- Detecção de falhas de dispositivos.
- Integração com dispositivos de controle de acesso.
- Sincronização de autorizações com dispositivos físicos.
- Recebimento e registro de eventos de acesso.

### 2.8. Processos automatizados

- Processamento da expiração de reservas.
- Processamento da expiração e revogação de autorizações de acesso.
- Envio automático de notificações.
- Monitoramento de SLA.
- Processamento de vencimentos de cobranças.
- Detecção de falhas de dispositivos IoT.
- Atualização automática de estados operacionais, quando aplicável.
- Retentativas de operações que falharam.
- Registro de eventos de auditoria.
- Expiração e revogação de sessões.
- Limpeza de dados temporários expirados.
- Execução de backups automatizados.
- Monitoramento da disponibilidade dos serviços.
- Execução das políticas de retenção de dados.

### 2.9. Monitoramento administrativo

- Visualização de indicadores de reservas.
- Acompanhamento de chamados e respectivos SLA.
- Consulta de indicadores de inadimplência.
- Monitoramento do estado da infraestrutura.
- Acompanhamento do estado dos dispositivos IoT.
- Consulta de indicadores relevantes de telemetria.
- Visualização de eventos operacionais pertinentes à administração.

### 2.10. Administração da plataforma multi-tenant

- Gerenciamento dos condomínios cadastrados na plataforma.
- Isolamento dos dados de cada condomínio.
- Gerenciamento de usuários e permissões de nível de plataforma.
- Gerenciamento de planos e limites de utilização.
- Consulta de indicadores operacionais da plataforma.
- Gerenciamento de configurações globais.
- Consulta de registros de auditoria e informações necessárias à operação da plataforma.

## 3. Funcionalidades previstas para versões futuras

As funcionalidades desta seção representam possibilidades de evolução do produto. Sua inclusão no MVP dependerá de uma decisão explícita e da correspondente atualização dos requisitos e do planejamento.

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

**Status:** definido inicialmente, sujeito à validação final dos limites do MVP.

Este documento estabelece uma referência para o planejamento do produto. A inclusão de uma funcionalidade neste escopo não significa que ela já esteja implementada ou validada.

O escopo deverá permanecer consistente com os requisitos consolidados e com as decisões registradas durante o desenvolvimento do projeto.