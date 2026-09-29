using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Web.UI;
using Microsoft.Owin.Security;
using Microsoft.Owin.Host.SystemWeb;

namespace UC0614_ExFinal
{
    public partial class ExternalLoginCallback : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            string fornecedor = Request.QueryString["fornecedor"];
            if (fornecedor != "Google" && fornecedor != "Facebook")
            {
                MostrarErro("Fornecedor de autenticação inválido.");
                return;
            }

            AuthenticateResult resultado = System.Web.HttpContextExtensions.GetOwinContext(Context).Authentication
                .AuthenticateAsync("ExternalCookie").Result;

            if (resultado == null || resultado.Identity == null || !resultado.Identity.IsAuthenticated)
            {
                MostrarErro("Não foi possível concluir a autenticação com " + fornecedor + ".");
                return;
            }

            string chaveFornecedor = resultado.Identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string email = resultado.Identity.FindFirst(ClaimTypes.Email)?.Value;
            string nome = resultado.Identity.FindFirst(ClaimTypes.Name)?.Value;
            if (String.IsNullOrWhiteSpace(chaveFornecedor) || String.IsNullOrWhiteSpace(email))
            {
                MostrarErro("O " + fornecedor + " não enviou um email. Confirma as permissões da aplicação externa.");
                return;
            }

            EntrarOuCriarUtilizador(fornecedor, chaveFornecedor, String.IsNullOrWhiteSpace(nome) ? email.Split('@')[0] : nome, email);
        }

        private void EntrarOuCriarUtilizador(string fornecedor, string chaveFornecedor, string nome, string email)
        {
            //using (SqlConnection ligacao = new SqlConnection(ConfigurationManager.ConnectionStrings["UC0614ConnectionString"].ConnectionString))
            using (SqlConnection ligacao = new SqlConnection(DatabaseConfig.ConnectionString))
            using (SqlCommand comando = new SqlCommand("sp_ObterOuCriarLoginExterno", ligacao))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@fornecedor", fornecedor);
                comando.Parameters.AddWithValue("@chaveFornecedor", chaveFornecedor);
                comando.Parameters.AddWithValue("@nome", nome);
                comando.Parameters.AddWithValue("@email", email);
                comando.Parameters.AddWithValue("@passwordHash", PasswordSecurity.CreateHash(Guid.NewGuid().ToString()));
                ligacao.Open();
                using (SqlDataReader leitor = comando.ExecuteReader())
                {
                    if (!leitor.Read())
                    {
                        MostrarErro("Já existe uma conta com este email. Inicia sessão normalmente para associar o login externo mais tarde.");
                        return;
                    }
                    Session["UtilizadorId"] = leitor["Id"];
                    Session["UtilizadorNome"] = leitor["Nome"];
                    Session["Perfil"] = leitor["Perfil"];
                }
            }

            System.Web.HttpContextExtensions.GetOwinContext(Context).Authentication.SignOut("ExternalCookie");
            Response.Redirect(Session["Perfil"].ToString() == "Administrador" ? "AdminUsers.aspx" : "Default.aspx");
        }

        private void MostrarErro(string mensagem)
        {
            System.Web.HttpContextExtensions.GetOwinContext(Context).Authentication.SignOut("ExternalCookie");
            lblMensagem.Text = mensagem;
        }
    }
}
