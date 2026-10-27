# Instalação e execução local

## Status

As instruções abaixo definem o contrato esperado para a execução local do MVP. Os comandos concretos serão preenchidos junto com a implementação do backend, da interface web e das migrações.

## Pré-requisitos

- SDK e runtime definidos pelo projeto;
- PostgreSQL em versão compatível;
- ferramenta de execução de migrações;
- acesso às dependências restauradas pelo gerenciador de pacotes.

As versões exatas devem ser registradas nesta seção assim que os arquivos de solução e os manifests forem adicionados.

## Configuração

1. Restaurar as dependências do backend e da interface web.
2. Criar um banco PostgreSQL local para desenvolvimento.
3. Configurar a string de conexão e os segredos por variáveis de ambiente ou mecanismo local equivalente.
4. Aplicar as migrações do banco.
5. Carregar dados de demonstração, quando fornecidos.

Segredos não devem ser incluídos no repositório, em exemplos executáveis ou em logs.

## Execução

Os comandos de inicialização do backend, da interface web e das migrações serão adicionados quando os projetos forem implementados. A documentação deverá informar portas, dependências, ordem de inicialização e dados de acesso de demonstração sem incluir credenciais reais.

## Testes

A execução deverá incluir comandos separados para testes unitários, integração e aceitação. Os casos mínimos estão em [`../07-Testing/MvpTestCases.md`](../07-Testing/MvpTestCases.md).