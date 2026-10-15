# Glossário do Projeto

## 1. Objetivo

Este documento define os principais termos, conceitos e siglas utilizados no Sistema de Gestão Condominial, estabelecendo uma terminologia padronizada para a documentação, o desenvolvimento e a manutenção do sistema.

As definições deste glossário devem ser utilizadas como referência na elaboração e atualização dos requisitos funcionais e não funcionais, regras de negócio, casos de uso, modelo de dados e documentação arquitetural.

## 2. Termos de Negócio

| Termo | Definição |
|---|---|
| Condomínio | Unidade organizacional responsável pela gestão de moradores, unidades, áreas comuns, reservas, chamados e infraestrutura. |
| Morador | Pessoa vinculada a uma ou mais unidades condominiais, autorizada a utilizar as funcionalidades disponíveis conforme suas permissões. |
| Funcionário | Pessoa responsável por atividades operacionais do condomínio, incluindo atendimento de chamados e acompanhamento da infraestrutura. |
| Administrador | Usuário responsável pela configuração e gestão operacional de um condomínio, respeitando suas permissões. |
| Administrador da Plataforma (Master) | Usuário com permissões de administração da plataforma como um todo, incluindo o gerenciamento dos condomínios cadastrados. |
| Unidade condominial | Apartamento, casa ou outra unidade individual pertencente à estrutura de um condomínio. |
| Área comum | Espaço compartilhado do condomínio que pode possuir regras de utilização e disponibilidade para reservas. |
| Reserva | Registro da solicitação ou confirmação de uso de uma área comum durante determinado período. |
| Ocorrência | Registro de um evento relevante que exige documentação, acompanhamento ou eventual ação corretiva. |
| Chamado | Registro formal de uma solicitação de atendimento, manutenção ou tratamento de um alerta operacional. |
| SLA | Acordo ou parâmetro de nível de serviço utilizado para estabelecer e acompanhar prazos de atendimento e resolução de chamados. |
| Cobrança condominial | Registro financeiro associado a uma unidade ou responsável, com valor, vencimento e situação de pagamento. |
| Inadimplência | Situação em que uma obrigação financeira permanece pendente após as condições de vencimento estabelecidas. |
| Ativo | Recurso físico ou operacional monitorado ou gerenciado pelo sistema, como equipamentos e componentes da infraestrutura condominial. |

## 3. Termos de Arquitetura e Tecnologia

| Termo | Definição |
|---|---|
| Plataforma multi-tenant | Arquitetura que permite atender múltiplos condomínios na mesma plataforma, mantendo os dados e as operações de cada um isolados. |
| Tenant | Contexto organizacional que representa um condomínio ou cliente isolado dentro da plataforma. |
| Isolamento entre tenants | Conjunto de mecanismos que impede que usuários ou operações de um condomínio acessem ou alterem dados de outro condomínio sem autorização. |
| Backend | Camada responsável pelo processamento das regras de negócio, autenticação, autorização, persistência e disponibilização dos serviços do sistema. |
| API | Interface de comunicação que permite que aplicações e outros sistemas utilizem funcionalidades disponibilizadas pelo backend. |
| Autenticação | Processo de verificação da identidade de um usuário ou sistema. |
| Autorização | Processo de determinar quais operações e recursos uma identidade autenticada pode acessar. |
| RBAC | Controle de acesso baseado em papéis, no qual as permissões são associadas a funções ou perfis de usuário. |
| JWT | Formato de token utilizado para transmitir informações de autenticação e autorização entre componentes, conforme as políticas de segurança adotadas. |
| EF Core | Entity Framework Core, ferramenta de mapeamento objeto-relacional utilizada para integrar a aplicação .NET ao banco de dados. |
| PostgreSQL | Sistema gerenciador de banco de dados relacional utilizado pelo projeto. |
| MQTT | Protocolo de comunicação baseado em publicação e assinatura, utilizado na comunicação com dispositivos IoT. |
| IoT | Internet das Coisas (*Internet of Things*): conjunto de dispositivos físicos capazes de coletar, transmitir ou receber informações. |
| Telemetria | Dados enviados por dispositivos ou equipamentos para registrar medições, estados ou eventos operacionais. |
| WebSocket | Tecnologia de comunicação que permite a troca de mensagens em tempo real entre cliente e servidor por uma conexão persistente. |

## 4. Termos de Acesso Físico e IoT

| Termo | Definição |
|---|---|
| Dispositivo IoT | Equipamento conectado que envia telemetria, recebe comandos ou participa de processos automatizados. |
| Sensor | Dispositivo responsável por medir uma grandeza física ou identificar uma condição do ambiente ou equipamento. |
| Atuador | Dispositivo capaz de executar uma ação física a partir de um comando, como acionar um mecanismo de acesso. |
| Credencial de acesso | Identificador ou meio utilizado para reconhecer uma pessoa ou autorizar uma tentativa de acesso, como tag RFID ou senha numérica. |
| Autorização de acesso | Registro que define se determinado acesso está permitido, considerando identidade, dispositivo, contexto e período de validade. |
| Registro de acesso | Evento persistido que documenta uma tentativa ou ocorrência de acesso físico, incluindo seu resultado quando disponível. |
| Estado de sincronização | Situação que indica se uma autorização ou informação já foi sincronizada com o dispositivo correspondente ou se ainda há falha ou pendência. |
| Alerta de telemetria | Evento gerado quando uma medição ou condição monitorada atende a um critério de alerta configurado. |
| Alarme IoT | Chamado ou evento operacional originado de uma condição detectada por um dispositivo ou processo de monitoramento IoT. |
| Reconhecimento facial | Tecnologia biométrica utilizada para comparar características faciais e apoiar a identificação ou autenticação de uma pessoa, conforme as regras de segurança e privacidade aplicáveis. |

## 5. Termos de Dados e Auditoria

| Termo | Definição |
|---|---|
| Exclusão lógica (*soft delete*) | Marcação de um registro como excluído ou inativo sem remover fisicamente seus dados do banco. |
| Exclusão física | Remoção efetiva dos dados armazenados. Seu uso deve respeitar as regras de integridade, retenção e privacidade do sistema. |
| Histórico | Conjunto de registros que documentam alterações ou eventos ocorridos ao longo do tempo. |
| Auditoria | Registro de ações relevantes para permitir rastreabilidade, investigação e verificação de responsabilidades. |
| Rastreabilidade | Capacidade de relacionar requisitos, regras de negócio, casos de uso, componentes, dados e testes. |
| Integridade referencial | Garantia de que os relacionamentos entre registros respeitam as restrições definidas no banco de dados. |
| Concorrência | Situação em que duas ou mais operações tentam acessar ou modificar recursos relacionados ao mesmo tempo. |
| Idempotência | Propriedade pela qual a repetição de uma operação produz o mesmo efeito final que sua execução única, dentro das condições definidas para a operação. |

## 6. Convenções e Siglas

| Termo ou sigla | Significado |
|---|---|
| DER | Diagrama Entidade-Relacionamento. |
| RF | Requisito Funcional. |
| RNF | Requisito Não Funcional. |
| LGPD | Lei Geral de Proteção de Dados Pessoais. |
| P95 | Percentil 95: valor abaixo do qual se encontram 95% das observações de uma métrica, como o tempo de resposta. |
| RFID | Identificação por radiofrequência (*Radio-Frequency Identification*). |
| CRUD | Operações de criação, consulta, atualização e exclusão de dados. |
| TLS | Protocolo de segurança utilizado para proteger comunicações em trânsito. |
| .NET | Plataforma de desenvolvimento utilizada para aplicações e serviços do ecossistema Microsoft. |
| C# | Linguagem de programação utilizada no desenvolvimento da aplicação. |

## 7. Manutenção do Glossário

Novos termos devem ser adicionados quando forem introduzidos conceitos relevantes para o negócio, a arquitetura, os requisitos ou a implementação.

As definições devem permanecer consistentes com as decisões registradas no projeto. Caso um termo passe a ter um significado diferente, os documentos relacionados devem ser revisados para evitar ambiguidades.

Este glossário define a terminologia do projeto, mas não substitui os requisitos, as regras de negócio, as especificações técnicas ou as restrições do modelo de dados.
