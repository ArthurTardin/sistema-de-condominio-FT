# Casos de Uso

## Objetivo

Os casos de uso descrevem as principais interações entre os atores e o sistema, representando os objetivos que cada ator pode realizar dentro da plataforma.

Eles serão utilizados como base para:

- Detalhamento dos fluxos funcionais;
- definição das regras envolvidas em cada operação;
- identificação das informações necessárias para cada fluxo;
- definição das resposabilidades de cada ator;
- elaboração dos diagramas de casos de uso;
- posteriormente, definição das interfaces dos clientes Web, Mobile e Desktop.

Os casos de uso devem permanecer alinhados aos requisitos funcionais, ao modelo de domínio e à arquitetura definidos para o sistema.

## Atores

Os principais atores identificados inicialmente são:

- Morador - utiliza as funcionalidades relacionadas à sua unidade e às operações disponíveis para moradores.
- Funcionário - executa atividades operacionais e atende chamados atribuídos.
- Administrador - Gerencia as operações e configurações do condomínio.
- Master - administra a plataforma e os tenants.
- Sistema - executa processos automáticos e regras que não dependem de uma ação direta ao usuário.
- Dispositivo IoT - participa da integração com a infraestrutura física, enviando telemetria e recebendo comandos ou autorizações quando aplicável.

## Organização

Os casos de uso serão levantados inicialmente por ator:

1. Morador;
2. Funcionário;
3. Administrador;
4. Master;
5. Sistema;
6. Dispositivo IoT.

Após o levantamento individual, os casos de uso serão revisados para identificar:

- Casos de uso compartilhados entre atores;
- relações de `include`;
- relações de `extend`;
- generalizações entre atores, quando aplicável;
- dependências entre casos de uso;
- possíveis casos de uso automáticos.

## Diagrama Geral

O diagrama geral de casos de uso será elaborado após o levantamento e a revisão dos casos de uso de todos os atores.

## Documentação Detalhada

Após a definição da lista de casoo de uso, os fluxos relevantes serã documentados individualmente, incluindo:

- ator principal;
- objetivo;
- pré-condições;
- pós-condições;
- fluxo principal;
- fluxos alternativos;
- exceções;
- regras de negócios relacionadas;
- requisitos relacionados.