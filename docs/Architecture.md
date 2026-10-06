# System Architecture

## 1. Architecture Overview

O sistema adotará **Clean Architecture** com o objetivo de promover separação de responsabilidades, manutenibilidade, testabilidade e independência em relação a tecnologias externas.

A aplicação será composta por três clientes principais, **Web**, **Mobile** e **Desktop**, que consumirão um **Backend centralizado** por meio de uma API.

O Backend será organizado em quatro camadas principais:

* **Domain**: contém as entidades centrais do negócio, regras de domínio, objetos de valor, serviços de domínio e eventos de domínio.

* **Application**: contém os casos de uso da aplicação, lógica de orquestração, DTOs, interfaces, validadores e serviços de aplicação.

* **Infrastructure**: contém as implementações dependentes de tecnologias externas, como Entity Framework Core, PostgreSQL, MQTT, serviços de autenticação e outras integrações externas.

* **API**: expõe a interface HTTP da aplicação e é responsável por controllers, autenticação, autorização, middleware, injeção de dependência e configuração da API.

Os clientes **Web**, **Mobile** e **Desktop** não deverão possuir regras de negócio independentes. Todos utilizarão o Backend como ponto central de comunicação, garantindo que as regras de negócio sejam aplicadas de forma consistente independentemente do cliente utilizado.

A arquitetura seguirá o princípio da **inversão de dependência**. As dependências devem apontar para as camadas internas, garantindo que a camada **Domain** permaneça independente de frameworks, bancos de dados, protocolos de comunicação e outras preocupações de infraestrutura.

A integração com dispositivos IoT será realizada por meio da camada de **Infrastructure**, utilizando o protocolo **MQTT** para comunicação com a infraestrutura física. Os dispositivos IoT não serão considerados clientes da aplicação, mas componentes externos integrados ao Backend.

O sistema também deverá suportar processamento assíncrono em segundo plano para operações como processamento de telemetria IoT, comunicação MQTT, notificações e outras tarefas que não devem bloquear requisições HTTP.

A arquitetura deverá atender ao modelo **multi-tenant** do sistema, aos requisitos de comunicação em tempo real, à integração com dispositivos IoT, ao controle de concorrência e aos requisitos de segurança definidos na especificação de requisitos.
