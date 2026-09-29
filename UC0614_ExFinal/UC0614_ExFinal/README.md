# UC0614 – Projeto Final
## Gestão de Contas de Utilizador

Este projeto foi desenvolvido no âmbito da **UFCD UC0614**.

O objetivo foi criar uma aplicação web para registo, autenticação e gestão de utilizadores, com uma área pública e uma área administrativa.

---

## 1. Tecnologias utilizadas

No projeto utilizei:

- ASP.NET Web Forms
- .NET Framework 4.7.2
- C#
- SQL Server
- ADO.NET
- Stored Procedures
- OWIN
- Google OAuth
- Facebook OAuth
- SMTP
- HTML
- CSS

---

## 2. Contas e autenticação

A aplicação permite criar e gerir contas de utilizador, tanto através de registo normal como através de login externo.

### 2.1 Registo normal

O utilizador pode criar uma conta através da opção **Registar**.

O processo funciona da seguinte forma:

```text
Registo
  ↓
Conta criada com perfil de Utilizador
  ↓
Conta fica pendente
  ↓
É enviado um email de ativação
  ↓
O utilizador abre a ligação recebida
  ↓
Conta fica ativa
  ↓
Login disponível
```

O registo público cria sempre uma conta com o perfil **Utilizador**.

Não é possível escolher o perfil de Administrador durante o registo.

### 2.2 Ativação da conta

Depois do registo é enviado um email com uma ligação de ativação.

Enquanto a conta não for ativada, o utilizador não consegue fazer login normalmente.

Também não é possível utilizar Google ou Facebook para contornar este processo.

### 2.3 Login normal

O login pode ser feito com:

- email;
- palavra-passe.

As palavras-passe não são guardadas em texto simples.

A classe utilizada para gerar e validar as palavras-passe é:

```text
PasswordSecurity.cs
```

Na base de dados é guardada a informação necessária para validação:

```text
Salt + Hash
```

A palavra-passe original não fica guardada.

### 2.4 Alteração da palavra-passe

Um utilizador autenticado pode alterar a sua palavra-passe.

A nova palavra-passe volta a ser processada através da classe `PasswordSecurity.cs` antes de ser guardada.

### 2.5 Recuperação da palavra-passe

Na página de login existe a opção:

```text
Esqueci-me da palavra-passe
```

O utilizador indica o email associado à conta e recebe um link de recuperação.

Para isso é criado um token temporário.

Esse token:

- tem um tempo de validade;
- só pode ser utilizado uma vez;
- deixa de ser válido depois da alteração da palavra-passe.

### 2.6 Login com Google

Também implementei autenticação através do Google.

Para funcionar é necessário criar uma aplicação OAuth no **Google Cloud** e configurar:

```text
UC0614_GOOGLE_CLIENT_ID
UC0614_GOOGLE_CLIENT_SECRET
```

Exemplo em PowerShell:

```powershell
[Environment]::SetEnvironmentVariable(
    "UC0614_GOOGLE_CLIENT_ID",
    "CLIENT_ID",
    "User"
)

[Environment]::SetEnvironmentVariable(
    "UC0614_GOOGLE_CLIENT_SECRET",
    "CLIENT_SECRET",
    "User"
)
```

No Google Cloud deve ainda ser definido o endereço de redirecionamento:

```text
https://localhost:PORTA/signin-google
```

A `PORTA` deve corresponder à porta HTTPS utilizada pelo projeto.

### 2.7 Login com Facebook

Também foi preparado o login através do Facebook.

Para isso é necessário criar uma aplicação no **Meta for Developers** e configurar:

```text
UC0614_FACEBOOK_APP_ID
UC0614_FACEBOOK_APP_SECRET
```

Exemplo:

```powershell
[Environment]::SetEnvironmentVariable(
    "UC0614_FACEBOOK_APP_ID",
    "APP_ID",
    "User"
)

[Environment]::SetEnvironmentVariable(
    "UC0614_FACEBOOK_APP_SECRET",
    "APP_SECRET",
    "User"
)
```

O endereço de redirecionamento deve ser:

```text
https://localhost:PORTA/signin-facebook
```

Durante o desenvolvimento, este login fica limitado às contas autorizadas na aplicação Meta.

Para utilização pública podem ser necessárias configurações e aprovações adicionais.

### 2.8 Associação dos logins externos

Os logins Google e Facebook ficam associados aos utilizadores através da tabela:

```text
LoginsExternos
```

Nesta tabela são guardados:

- `UtilizadorId`
- `Fornecedor`
- `ChaveFornecedor`

Os fornecedores utilizados são:

- Google
- Facebook

Se já existir uma conta local ativa com o mesmo email, o login externo pode ser associado a essa conta.

---

## 3. Perfis e área administrativa

A aplicação utiliza dois perfis:

| ID | Perfil |
|---:|---|
| 1 | Administrador |
| 2 | Utilizador |

A principal diferença entre os dois está no acesso à área administrativa.

### 3.1 Utilizador

O perfil **Utilizador** é o perfil atribuído às contas criadas através do registo público.

Este perfil pode utilizar a área normal da aplicação, mas não tem acesso às opções administrativas.

### 3.2 Administrador

O perfil **Administrador** tem acesso ao Back-end da aplicação.

A partir dessa área é possível:

- consultar utilizadores;
- inserir utilizadores;
- alterar utilizadores;
- eliminar utilizadores;
- definir o perfil de cada utilizador;
- ativar ou desativar contas;
- criar novos administradores.

O registo público nunca cria administradores.

Um novo administrador só pode ser criado através da área administrativa ou através do administrador inicial criado na primeira execução.

### 3.3 Administrador inicial

Para ser possível entrar na área administrativa logo na primeira execução, configurei três variáveis de ambiente:

```text
UC0614_ADMIN_NAME
UC0614_ADMIN_EMAIL
UC0614_ADMIN_PASSWORD
```

```powershell
[Environment]::SetEnvironmentVariable(
    "UC0614_ADMIN_NAME",
    "Administrador",
    "User"
)

[Environment]::SetEnvironmentVariable(
    "UC0614_ADMIN_EMAIL",
    "admin@uc0614.pt",
    "User"
)

[Environment]::SetEnvironmentVariable(
    "UC0614_ADMIN_PASSWORD",
    "Admin123!",
    "User"
)
```

Na primeira execução, se ainda não existir nenhum administrador, a aplicação cria automaticamente essa conta com:

```text
PerfilId = 1
Ativo = 1
```

Se já existir um administrador, não é criada uma nova conta.

A palavra-passe do administrador também é guardada através de hash e nunca em texto simples.

Depois da autenticação fica disponível a opção **Administração**.

---

## 4. Base de dados

A aplicação utiliza **SQL Server**.

As principais tabelas são:

- `Perfis`
- `Utilizadores`
- `LoginsExternos`

A tabela `Utilizadores` guarda os dados necessários para:

- autenticação;
- ativação da conta;
- recuperação da palavra-passe;
- perfil do utilizador;
- estado da conta.

### 4.1 Stored Procedures

As operações sobre a base de dados são feitas através de **Stored Procedures**.

As principais são:

- `sp_AlterarPassword`
- `sp_AlterarUtilizador`
- `sp_AtivarConta`
- `sp_CriarTokenRecuperacao`
- `sp_EliminarUtilizador`
- `sp_InserirUtilizadorAdmin`
- `sp_ListarUtilizadores`
- `sp_ObterOuCriarLoginExterno`
- `sp_ObterUtilizadorLogin`
- `sp_ObterUtilizadorPorId`
- `sp_RegistarUtilizador`
- `sp_ReporPassword`

### 4.2 Criação automática da base de dados

Para evitar ter de criar toda a base de dados manualmente, utilizei a classe:

```text
DatabaseInitializer.cs
```

Na primeira execução, esta classe:

1. lê a connection string definida nas variáveis de ambiente;
2. verifica se a base de dados já existe;
3. cria a base de dados caso ainda não exista;
4. executa o script SQL;
5. cria as tabelas;
6. cria as Stored Procedures;
7. cria os perfis;
8. verifica se já existe um administrador;
9. cria o primeiro administrador, caso seja necessário.

O script SQL está em:

```text
Database/UC0614_ExFinal.sql
```

A conta SQL utilizada na connection string precisa de permissões para criar a base de dados.

Caso isso não seja possível, o mesmo script pode ser executado manualmente no **SQL Server Management Studio**.

---

## 5. Configuração do projeto

As credenciais não ficam escritas diretamente no código nem no `Web.config`.

Optei por utilizar **variáveis de ambiente do Windows**, para evitar guardar dados sensíveis dentro do projeto.

### 5.1 Ligação ao SQL Server

A ligação à base de dados é definida na variável:

```text
UC0614_DB_CONNECTION
```

Exemplo:

```powershell
[Environment]::SetEnvironmentVariable(
    "UC0614_DB_CONNECTION",
    "Data Source=localhost,1433;Initial Catalog=UC0614_ExFinal;User ID=sa;Password=SUA_PASSWORD;Encrypt=False;",
    "User"
)
```

`SUA_PASSWORD` deve ser substituído pela palavra-passe correspondente ao SQL Server utilizado.

### 5.2 Envio de emails

O envio de emails é utilizado em duas situações:

- ativação da conta;
- recuperação da palavra-passe.

Para isso são utilizadas:

```text
UC0614_SMTP_USER
UC0614_SMTP_PASSWORD
```

Exemplo:

```powershell
[Environment]::SetEnvironmentVariable(
    "UC0614_SMTP_USER",
    "email@gmail.com",
    "User"
)

[Environment]::SetEnvironmentVariable(
    "UC0614_SMTP_PASSWORD",
    "PASSWORD_DA_APLICACAO",
    "User"
)
```

No caso do Gmail é utilizada uma palavra-passe de aplicação.

A configuração usada é:

```text
Servidor: smtp.gmail.com
Porta: 587
SSL: ativo
```

Os links enviados por email são construídos automaticamente com base no endereço onde a aplicação está a ser executada.

### 5.3 Credenciais externas

Também ficam fora do código:

- dados de acesso ao SQL Server;
- credenciais do email;
- Google Client Secret;
- Facebook App Secret;
- palavra-passe do administrador inicial.

Cada ambiente onde o projeto seja executado deve ter as suas próprias variáveis configuradas.

---

## 6. Execução

Depois de configurar as variáveis de ambiente:

1. fechar o Visual Studio, caso esteja aberto;
2. abrir novamente o Visual Studio;
3. abrir a Solution;
4. executar:

```text
Build → Rebuild Solution
```

5. iniciar com:

```text
Ctrl + F5
```

Na primeira execução, a base de dados é criada automaticamente caso ainda não exista.

Para entrar como administrador devem ser utilizados os valores definidos em:

```text
UC0614_ADMIN_EMAIL
UC0614_ADMIN_PASSWORD
```

---

## 7. Estrutura simplificada da aplicação

### Fluxo normal

```text
Utilizador
    ↓
Página ASPX
    ↓
Code-Behind C#
    ↓
Stored Procedure
    ↓
SQL Server
```

### Login externo

```text
Utilizador
    ↓
Google / Facebook
    ↓
OWIN
    ↓
ExternalLoginCallback
    ↓
Stored Procedure
    ↓
Utilizadores / LoginsExternos
```

---

## 8. Estrutura principal do projeto

```text
UC0614_ExFinal
│
├── Database
│   └── UC0614_ExFinal.sql
│
├── Content
│   └── uc0614.css
│
├── Site.Master
├── Admin.Master
│
├── Default.aspx
├── Login.aspx
├── Register.aspx
├── ActivateAccount.aspx
├── ForgotPassword.aspx
├── ResetPassword.aspx
├── ChangePassword.aspx
│
├── AdminUsers.aspx
├── AdminCreateUser.aspx
├── AdminEditUser.aspx
│
├── ExternalLoginCallback.aspx
│
├── PasswordSecurity.cs
├── DatabaseConfig.cs
├── DatabaseInitializer.cs
├── EmailService.cs
├── Startup.cs
├── Global.asax
└── Web.config
```

---

## 9. Funcionalidades testadas

Durante o desenvolvimento fui testando as principais funcionalidades da aplicação.

Foram validadas:

- criação automática da base de dados;
- criação automática das tabelas;
- criação das Stored Procedures;
- criação automática do administrador inicial;
- registo de utilizadores;
- ativação da conta por email;
- login com email e palavra-passe;
- alteração da palavra-passe;
- recuperação da palavra-passe;
- utilização única do token de recuperação;
- login através do Google;
- login através do Facebook em ambiente de desenvolvimento;
- inserção de utilizadores;
- consulta de utilizadores;
- alteração de utilizadores;
- eliminação de utilizadores;
- criação de administradores;
- controlo de acesso à área administrativa.

---

## 10. Autenticação de dois fatores

A autenticação de dois fatores (**2FA**) não foi implementada nesta versão.

As restantes funcionalidades descritas neste README encontram-se implementadas no projeto.

---

## 11. Ficheiros principais

Os ficheiros principais para executar e verificar o projeto são:

```text
Solution do Visual Studio
Database/UC0614_ExFinal.sql
README.md
```

O ficheiro `UC0614_ExFinal.sql` pode ser utilizado para criar manualmente a estrutura da base de dados caso seja necessário.

---

## Autor

Projeto desenvolvido por Ricardo Tavares para a **UC00614**.
