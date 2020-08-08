# Instruções para execução

## Requisitos

- .NET Core 3.1 SDK [windows x64 download](https://dotnet.microsoft.com/download/dotnet-core/thank-you/sdk-3.1.200-windows-x64-installer) | [outras opções](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- Node JS versão LTS [download](https://nodejs.org/en/)
- **[opcional]** SQL Server LocalDb `(localdb)\mssqllocaldb` [instruções](https://docs.microsoft.com/pt-br/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver15#try-it-out)
    - Se você já tiver outra versão de SQL Server instalado, então será necessário trocar a connection string que fica no arquivo `appsettings.json`

## Execução

Assumindo que você já tem tudo instalado no seu computador, e já clonou o repositorio, vou assumir que o diretório onde foi clonado é
```
C:\projetos\HandlingErrors
```

1. Abra um powershell, e navega até o diretório da aplicação
1. Execute um `dotnet restore`
1. Navegue até o diretório `cd .\src\HandlingErrors.Web\ClientApp\`
1. Execute `npm install`
1. Volte 1 diretório `cd ..`
1. Se você instalou o SQL Server LocalDB
    1. Execute `dotnet run`
1. Se você **não** instalou o SQL Server LocalDB
    1. Execute `dotnet run useInMemory`
    1. **Atenção:** o banco de dados vai iniciar sempre zerado com essa opção
    1. **Atenção²:** com essa opção não vai ser possível testar a regra de 6 meses, uma vez que os dados estão em memória, não tem como simular criar um recado a mais de 6 meses atrás, já com SQL Server poderia ser trocado a data diretamente no banco.
1. Agora é só abrir o broswer(navegador) e ir até a url http://localhost:5000

## Testes e Cobertura

Para a execução dos testes e geração do relatório de cobertura você deve executar os seguintes passos:

1. Usando um console powershell, navegue até a raiz do projeto
1. Execute o seguinte comando `.\build.ps1 -Target Coverage`
1. Quando o comando terminar, você pode abrir o relatório gerado, para isso execute `.\coverageOutput\index.htm`

## Tecnologias usadas

Para a construção eu escolhi a seguintes tecnologias:

### Backend

1. C# 8.0
1. .NET Core 3.1
1. ASP.NET Core 3.1
1. AutoMapper
1. Entity Framework Core 3.*
1. OData 7.4
1. xUnit
1. SimpleInjector
1. FluentValidation
1. Mediatr

### Frontend

1. [Aurelia (Framework frontend para construção de SPAs)](https://aurelia.io)
1. TypeScript
1. Stylus
1. Bootstrap 4
1. Momentjs

### DevOps

1. CakeBuild
    1. Facilmente plugavel em qualquer servidor de CI(TeamCity/Jenkins/Travis/Azure DevOps/AppVeyor/GitLab Runner/etc)
    1. Script versionável
    1. Possibilidade de rodar na máquina do Dev
    1. Agnóstico a CI server
    1. Muito fácil de usar
