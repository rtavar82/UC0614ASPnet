using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace UC0614_ExFinal
{
    public partial class ResetPassword : Page
    {
        protected void btnRepor_Click(object sender, EventArgs e)
        {
            Guid token;

            if (!Guid.TryParse(Request.QueryString["token"], out token))
            {
                lblMensagem.CssClass = "mensagem-erro";
                lblMensagem.Text = "Ligação de recuperação inválida.";
                return;
            }

            if (String.IsNullOrWhiteSpace(txtPasswordNova.Text) ||
                String.IsNullOrWhiteSpace(txtConfirmarPassword.Text))
            {
                lblMensagem.CssClass = "mensagem-erro";
                lblMensagem.Text = "Preencha os dois campos.";
                return;
            }

            if (txtPasswordNova.Text.Length < 8)
            {
                lblMensagem.CssClass = "mensagem-erro";
                lblMensagem.Text = "A palavra-passe deve ter pelo menos 8 caracteres.";
                return;
            }

            if (txtPasswordNova.Text != txtConfirmarPassword.Text)
            {
                lblMensagem.CssClass = "mensagem-erro";
                lblMensagem.Text = "As palavras-passe não coincidem.";
                return;
            }

            string passwordHash =
                PasswordSecurity.CreateHash(txtPasswordNova.Text);

            int resultado;

            using (SqlConnection ligacao =
                new SqlConnection(DatabaseConfig.ConnectionString))

            using (SqlCommand comando =
                new SqlCommand(
                    "sp_ReporPassword",
                    ligacao))
            {
                comando.CommandType =
                    CommandType.StoredProcedure;

                comando.Parameters.AddWithValue(
                    "@token",
                    token);

                comando.Parameters.AddWithValue(
                    "@passwordHash",
                    passwordHash);

                SqlParameter retorno =
                    comando.Parameters.Add(
                        "@resultado",
                        SqlDbType.Int);

                retorno.Direction =
                    ParameterDirection.Output;

                ligacao.Open();

                comando.ExecuteNonQuery();

                resultado =
                    Convert.ToInt32(retorno.Value);
            }

            if (resultado == 0)
            {
                lblMensagem.CssClass = "mensagem-erro";
                lblMensagem.Text =
                    "A ligação expirou, já foi utilizada ou não é válida.";

                return;
            }

            txtPasswordNova.Text = "";
            txtConfirmarPassword.Text = "";

            lblMensagem.CssClass = "mensagem-sucesso";
            lblMensagem.Text =
                "Palavra-passe alterada com sucesso. Já pode iniciar sessão.";
        }
    }
}