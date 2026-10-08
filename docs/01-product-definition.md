# Definição do Produto

## Problema

A gestão de condomínios sofre com a desconexão entre a administração digital e a infraestrutura física. Atividades como reservas de espaços e registros de chamados frequentemente operam de forma isolada ou manual, gerando lentidão, retrabalho e falhas operacionais.

Paralelamente, o monitoramento de recursos físicos críticos, como reservatórios, bombas e acessos, pode depender de inspeções humanas periódicas. Essa fragmentação reduz a eficiência operacional, aumenta os riscos de falhas de segurança e dificulta o acesso da administração a dados em tempo real para a tomada de decisão.

## Objetivo

Criar uma plataforma distribuída de gestão condominial, com processamento em tempo real, capaz de centralizar as operações de moradores, administração e funcionários em um único ambiente.

A plataforma será disponibilizada por meio de uma aplicação Web, um aplicativo Mobile e um aplicativo Desktop, utilizando um Backend centralizado para disponibilizar as funcionalidades do sistema de forma consistente entre os diferentes clientes.

A plataforma deverá gerenciar operações como:

* Reservas de espaços comuns;
* Chamados de manutenção e atendimento;
* Registro e acompanhamento de ocorrências;
* Gestão de unidades e usuários;
* Gestão financeira básica das unidades;
* Notificações e informações operacionais;
* Monitoramento da infraestrutura física.

### Integração Física — IoT

A plataforma atuará como camada central de integração com a infraestrutura física do condomínio, utilizando dispositivos IoT comunicando-se por meio do protocolo MQTT.

Essa integração permitirá:

* Automatizar o controle de acessos físicos vinculados às reservas;
* Gerar autorizações temporárias de acesso;
* Registrar tentativas de acesso;
* Monitorar continuamente recursos físicos e infraestrutura;
* Receber telemetria dos dispositivos;
* Identificar condições anormais;
* Gerar ocorrências e chamados de manutenção automaticamente a partir de eventos de telemetria.

## Status

Escopo, usuários (morador, administrador, funcionário) e funcionalidades definidos. Detalhamento completo em Requisitos Funcionais (RF01–RF19) e Requisitos Não-Funcionais (RNF01–RNF09).