# Registro de Decisões do Projeto

## 1. Objetivo

Este documento registra as decisões relevantes tomadas durante a definição, modelagem, arquitetura e desenvolvimento do Sistema de Gestão Condominial.

O registro tem como finalidade preservar o contexto das decisões, documentar suas justificativas e permitir que seus impactos sejam compreendidos durante a evolução do projeto.

## 2. Convenções

Cada decisão deve possuir um identificador único no formato `DEC-NNN`.

Os status possíveis são:

- **Proposta:** decisão em avaliação, ainda não aprovada.
- **Aprovada:** decisão validada e adotada pelo projeto.
- **Substituída:** decisão que deixou de ser válida por ter sido substituída por outra.

Uma decisão aprovada não deve ser alterada silenciosamente. Caso seja necessário revisá-la, a mudança deve ser registrada, preservando o histórico e indicando a decisão substituta quando aplicável.

## 3. Decisões Registradas

### DEC-001 - Banco de Dados Relacional

- **Status:** Aprovada
- **Contexto:** O sistema precisa persistir informações relacionadas a condomínios, usuários, unidades, reservas, chamados, cobranças, dispositivos e registros históricos.
- **Decisão:** Utilizar PostgreSQL como sistema gerenciador de banco de dados.
- **Justificativa:** O projeto utiliza um banco relacional para representar entidades, relacionamentos, restrições e integridade dos dados.
- **Consequências:** O modelo de dados e as implementações de persistência devem considerar os recursos e as restrições do PostgreSQL.

### DEC-002 - Plataforma Multi-tenant

- **Status:** Aprovada
- **Contexto:** A plataforma deve permitir o gerenciamento de múltiplos condomínios sem comprometer o isolamento de seus dados.
- **Decisão:** Adotar uma arquitetura multi-tenant, com contexto de condomínio e mecanismos de isolamento entre tenants.
- **Justificativa:** A plataforma precisa atender diferentes condomínios mantendo a separação das informações e das operações.
- **Consequências:** A modelagem, a autorização, as consultas e as operações de escrita devem respeitar o contexto do tenant. Os mecanismos de isolamento devem ser aplicados de forma consistente nas camadas pertinentes.

### DEC-003 - Controle de Acesso Baseado em Papéis

- **Status:** Aprovada
- **Contexto:** Os diferentes perfis de usuário precisam acessar funcionalidades compatíveis com suas responsabilidades.
- **Decisão:** Adotar controle de acesso baseado em papéis (RBAC), contemplando os perfis Morador, Funcionário, Administrador e Master.
- **Justificativa:** A separação de responsabilidades permite controlar as operações disponíveis para cada perfil.
- **Consequências:** As permissões devem ser verificadas nas operações protegidas, respeitando também o contexto do tenant e as regras específicas de cada funcionalidade.

### DEC-004 - Integração com Dispositivos IoT por MQTT

- **Status:** Aprovada
- **Contexto:** O sistema precisa receber dados da infraestrutura física e se comunicar com dispositivos conectados.
- **Decisão:** Utilizar MQTT como protocolo de comunicação para a integração com dispositivos IoT.
- **Justificativa:** O protocolo atende ao modelo de comunicação por publicação e assinatura previsto para a integração.
- **Consequências:** Os tópicos, as mensagens, a identificação dos dispositivos, a segurança e o tratamento de falhas devem seguir especificações próprias da integração.

### DEC-005 - Registro Histórico de Eventos Operacionais

- **Status:** Aprovada
- **Contexto:** O sistema precisa permitir o acompanhamento das mudanças de estado dos chamados e a rastreabilidade dos eventos operacionais.
- **Decisão:** Manter registros históricos específicos para documentar as alterações e os eventos relevantes, preservando os registros cuja natureza exige histórico permanente.
- **Justificativa:** O histórico permite compreender a evolução dos eventos e identificar responsabilidades.
- **Consequências:** As operações de alteração e exclusão devem respeitar as regras de retenção e integridade definidas para cada tipo de registro.

### DEC-006 - Exclusão Lógica de Registros Operacionais

- **Status:** Aprovada
- **Contexto:** A remoção de registros pode comprometer relacionamentos, histórico, auditoria e rastreabilidade.
- **Decisão:** Utilizar exclusão lógica para as entidades previstas nas regras de ciclo de vida dos dados, sem aplicar indiscriminadamente esse mecanismo a todos os registros.
- **Justificativa:** A estratégia permite preservar informações relevantes sem manter registros excluídos disponíveis para uso operacional indevido.
- **Consequências:** Cada entidade deve seguir sua política de ciclo de vida. Registros históricos e de auditoria devem respeitar suas regras específicas de preservação.

### DEC-007 - Separação entre Clientes e Backend Central

- **Status:** Aprovada
- **Contexto:** O produto prevê diferentes aplicações para acesso às funcionalidades do sistema.
- **Decisão:** Disponibilizar as funcionalidades por meio de um backend central, utilizado pelas aplicações Web, Mobile e Desktop.
- **Justificativa:** A centralização das regras de negócio e do acesso aos dados favorece a consistência das operações entre os clientes.
- **Consequências:** As regras de negócio e as validações críticas devem ser aplicadas no backend, sem depender exclusivamente das interfaces dos clientes.

### DEC-008 - Reconhecimento Facial como Funcionalidade Futura

- **Status:** Aprovada
- **Contexto:** Foi solicitada a inclusão de reconhecimento facial em nuvem para apoiar o acesso de moradores, como funcionalidade de uma versão futura.
- **Decisão:** Registrar o reconhecimento facial no planejamento de versões futuras, separado do escopo inicial do MVP.
- **Justificativa:** A funcionalidade foi identificada como uma evolução do produto, não como parte do escopo inicial definido.
- **Consequências:** Sua implementação deverá considerar requisitos próprios de segurança, privacidade, proteção de dados biométricos, integração e controle de acesso.

## 4. Revisão das Decisões

As decisões devem ser revisadas quando houver mudanças relevantes nos requisitos, nas regras de negócio, na arquitetura, no modelo de dados ou nas restrições técnicas do projeto.

Quando uma decisão for substituída, seu registro deve ser preservado e relacionado à nova decisão.

## 5. Histórico de Alterações do Documento

| Data | Alteração |
|---|---|
| 2026-10-15 | Criação da estrutura inicial do registro de decisões. |