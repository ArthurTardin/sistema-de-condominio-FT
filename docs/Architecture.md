# Arquitetura

Este arquivo permanece como ponto de entrada compatível para referências antigas.

A documentação arquitetural canônica está em [`05-Architecture/ArchitectureOverview.md`](05-Architecture/ArchitectureOverview.md), acompanhada por:

- [`ModuleDesign.md`](05-Architecture/ModuleDesign.md), para a organização dos módulos;
- [`Integrations.md`](05-Architecture/Integrations.md), para integrações externas;
- [`MqttSpecification.md`](05-Architecture/MqttSpecification.md), para a evolução da integração IoT.

O MVP utiliza uma interface web e um backend centralizado. Clientes mobile e desktop, MQTT, processamento de telemetria e outras integrações permanecem fora do MVP e devem ser tratados como evoluções arquiteturais posteriores.
