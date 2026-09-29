using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using Microsoft.Owin.Security;
using Microsoft.Owin.Host.SystemWeb;

namespace UC0614_ExFinal
{
    public partial class Login : Page
    {
        protected void btnGoogle_Click(object sender, EventArgs e)
        {
            IniciarLoginExterno("Google");
        }

        protected void btnFacebook_Click(object sender, EventArgs e)
        {
            IniciarLoginExterno("Facebook");
        }

        private void IniciarLoginExterno(string fornecedor)
        {
            AuthenticationProperties propriedades = new AuthenticationProperties
            {
                RedirectUri = ResolveUrl("~/ExternalLoginCallback.aspx?fornecedor=" + fornecedor)
            };

            System.Web.HttpContextExtensions.GetOwinContext(Context).Authentication.Challenge(propriedades, fornecedor);
            Response.StatusCode = 401;
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnEntrar_Click(object sender, EventArgs e)
        {
            //using (SqlConnection ligacao = new SqlConnection(ConfigurationManager.ConnectionStrings["UC0614ConnectionString"].ConnectionString))
            using (SqlConnection ligacao = new SqlConnection(DatabaseConfig.ConnectionString))
            using (SqlCommand comando = new SqlCommand("sp_ObterUtilizadorLogin", ligacao))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@email", txtUtilizador.Text.Trim());
                ligacao.Open();
                using (SqlDataReader leitor = comando.ExecuteReader())
                {
                    if (!leitor.Read() || !PasswordSecurity.Verify(txtPassword.Text, leitor["PasswordHash"].ToString()))
                    { lblMensagem.Text = "Email ou palavra-passe incorretos."; return; }
                    if (!Convert.ToBoolean(leitor["Ativo"]))
                    { lblMensagem.Text = "A conta ainda não foi ativada por email."; return; }
                    Session["UtilizadorId"] = leitor["Id"];
                    Session["UtilizadorNome"] = leitor["Nome"];
                    Session["Perfil"] = leitor["Perfil"];
                    Response.Redirect(leitor["Perfil"].ToString() == "Administrador" ? "AdminUsers.aspx" : "Default.aspx");
                }
            }
        }
    }
}
