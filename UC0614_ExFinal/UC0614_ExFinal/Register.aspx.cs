using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace UC0614_ExFinal
{
    public partial class Register : Page
    {
        protected void btnRegistar_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text.Length < 8) { lblMensagem.Text = "A palavra-passe deve ter pelo menos 8 caracteres."; return; }
            if (txtPassword.Text != txtConfirmarPassword.Text) { lblMensagem.Text = "As palavras-passe não coincidem."; return; }
            int resultado; Guid token;
            //using (SqlConnection ligacao = new SqlConnection(ConfigurationManager.ConnectionStrings["UC0614ConnectionString"].ConnectionString))
            using (SqlConnection ligacao = new SqlConnection(DatabaseConfig.ConnectionString))
            using (SqlCommand comando = new SqlCommand("sp_RegistarUtilizador", ligacao))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@nome", txtNome.Text.Trim());
                comando.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                comando.Parameters.AddWithValue("@passwordHash", PasswordSecurity.CreateHash(txtPassword.Text));
                SqlParameter retorno = comando.Parameters.Add("@resultado", SqlDbType.Int); retorno.Direction = ParameterDirection.Output;
                SqlParameter tokenParametro = comando.Parameters.Add("@token", SqlDbType.UniqueIdentifier); tokenParametro.Direction = ParameterDirection.Output;
                ligacao.Open(); comando.ExecuteNonQuery();
                resultado = Convert.ToInt32(retorno.Value); token = resultado == 1 ? (Guid)tokenParametro.Value : Guid.Empty;
            }
            if (resultado == 0) { lblMensagem.Text = "Já existe uma conta com este email."; return; }
            try { EmailService.EnviarAtivacao(txtEmail.Text.Trim(), txtNome.Text.Trim(), token.ToString()); lblMensagem.CssClass = "mensagem-sucesso"; lblMensagem.Text = "Conta criada. Consulte o email para a ativar."; }
            catch (Exception ex) { lblMensagem.Text = "Conta criada, mas o email não foi enviado: " + ex.Message; }
        }
    }
}
