# backend-purchase-order

Backend em .NET para ordens de compra empresariais relacionado a um teste técnico em um processo de seleção.

## Pré-requisitos

- [.NET SDK](https://dotnet.microsoft.com/download) instalado em sua máquina.
- Acesso ao banco de dados Azure SQL especificado.

## Como Executar Localmente

Siga estas etapas para executar a aplicação em sua máquina local e acessar os endpoints via localhost:

1. **Clone o repositório e navegue até o diretório do projeto:**

   ```bash
   git clone <url_do_repositorio>
   cd backend-purchase-order
   ```

2. **Configure a String de Conexão do Banco de Dados:**
   A aplicação requer a `AZURE_SQL_CONNECTIONSTRING` para conectar-se ao banco de dados. Em um ambiente de desenvolvimento, você pode definir isso por meio de variáveis de ambiente ou logando via azure cli.

   **Linux / macOS:**

   ```bash
   export ConnectionStrings__AZURE_SQL_CONNECTIONSTRING="Your_Database_Connection_String_Here"
   # Ou
   ```

   **Windows (Prompt de Comando):**

   ```cmd
   set ConnectionStrings__AZURE_SQL_CONNECTIONSTRING="Your_Database_Connection_String_Here"
   ```

   **Windows (PowerShell):**

   ```powershell
   $env:ConnectionStrings__AZURE_SQL_CONNECTIONSTRING="Your_Database_Connection_String_Here"
   ```

   _(Alternativamente, você pode criar/editar o arquivo `appsettings.Development.json` e adicionar a string de conexão sob `ConnectionStrings`)_

3. **Restaurar dependências:**

   ```bash
   dotnet restore
   ```

4. **Aplicar migrações do banco de dados (se o esquema do banco de dados não estiver atualizado):**
   _Nota: Requer as ferramentas CLI `dotnet-ef` (`dotnet tool install --global dotnet-ef`)._

   ```bash
   dotnet ef database update
   ```

5. **Executar a aplicação:**
   ```bash
   dotnet run
   ```

Uma vez em execução, os endpoints da aplicação estarão acessíveis via localhost (ex: `http://localhost:5000` ou `https://localhost:5001`, dependendo do seu `launchSettings.json`).

Você pode então fazer requisições para a API, por exemplo:

```bash
curl -X GET https://localhost:<porta>/api/Order
```

Ou usando ferramentas como Thunder Client, Postman, etc.
