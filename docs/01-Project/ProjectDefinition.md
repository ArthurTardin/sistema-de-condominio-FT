# Definição do Projeto

## Problema

A gestão de condomínios sofre com a desconexão entre a administração digital e a infraestrutura física. Atividades como reservas de espaços comuns, registros de chamados e acompanhamento de ocorrências frequentemente operam de forma isolada ou manual, gerando lentidão, retrabalho e falhas operacionais.

Paralelamente, o monitoramento de recursos físicos críticos, como reservatórios, bombas e sistemas de acesso, pode depender de inspeções humanas periódicas. Essa fragmentação reduz a eficiência operacional, aumenta os riscos de falhas e dificulta o acesso da administração a informações atualizadas para a tomada de decisão.

## Objetivo

Criar uma plataforma distribuída de gestão condominial, com capacidade de processamento em tempo real, capaz de centralizar operações de moradores, funcionários e administração em um único ambiente.

A plataforma deverá disponibilizar suas funcionalidades por meio de diferentes clientes, utilizando um backend centralizado para garantir consistência entre as aplicações.

As principais operações contempladas pelo sistema incluem:

- Reservas de espaços comuns;
- Chamados de manutenção e atendimento;
- Registro e acompanhamento de ocorrências;
- Gestão de unidades e usuários;
- Gestão financeira básica das unidades;
- Notificações e informações operacionais;
- Monitoramento da infraestrutura física.

## Integração da infraestrutura física

A plataforma atuará como camada central de integração com a infraestrutura física do condomínio, utilizando dispositivos IoT comunicando-se por meio do protocolo MQTT.

Essa integração permitirá:

- Automatizar o controle de acessos físicos vinculados às reservas;
- Gerar autorizações temporárias de acesso;
- Registrar eventos e tentativas de acesso;
- Monitorar continuamente recursos físicos e infraestrutura;
- Receber e processar telemetria dos dispositivos;
- Identificar condições anormais de operação;
- Gerar ocorrências e chamados de manutenção automaticamente a partir de eventos de telemetria.

## Principais atores

O sistema contempla diferentes perfis de interação com a plataforma:

- **Morador:** utiliza funcionalidades relacionadas à sua unidade, reservas, ocorrências, chamados e demais serviços disponibilizados pelo condomínio.
- **Funcionário:** executa atividades operacionais, acompanha chamados e atua sobre recursos e infraestrutura sob sua responsabilidade.
- **Administrador:** gerencia as operações, recursos, usuários e configurações do condomínio.
- **Master:** administra a plataforma e os condomínios cadastrados.
- **Sistema:** executa processos automáticos e operações assíncronas necessárias ao funcionamento da plataforma.
- **Dispositivo IoT:** interage com a plataforma para envio de telemetria, eventos e execução de operações relacionadas à infraestrutura física.

## Principais capacidades

A plataforma deverá fornecer, de forma integrada, capacidades para:

- Gerenciamento de usuários e unidades;
- Gerenciamento de espaços comuns;
- Reservas e cancelamentos;
- Gestão de chamados e ocorrências;
- Gestão de cobranças;
- Controle de acesso;
- Monitoramento de infraestrutura;
- Integração com dispositivos IoT;
- Processamento de telemetria;
- Notificações;
- Auditoria e rastreabilidade das operações;
- Processamento automático de eventos e tarefas operacionais.

## Características do projeto

O sistema será projetado considerando:

- Arquitetura distribuída;
- Processamento síncrono e assíncrono;
- Integração com infraestrutura física;
- Comunicação baseada em MQTT para dispositivos IoT;
- Processamento de eventos em tempo real quando aplicável;
- Isolamento de dados entre diferentes condomínios;
- Controle de acesso baseado em perfis e permissões;
- Rastreabilidade das operações relevantes.

## Status do projeto

O projeto encontra-se na fase de **especificação e modelagem**.

A definição do produto, o levantamento de requisitos, o modelo de dados, a arquitetura e o levantamento dos casos de uso estão sendo consolidados antes do início da implementação.

O escopo detalhado e os requisitos funcionais e não funcionais do sistema são especificados na documentação de requisitos.