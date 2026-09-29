using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace UC0614_ExFinal
{
    public partial class ForgotPassword : Page
    {
        protected void btnRecuperar_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblMensagem.CssClass = "mensagem-erro";
                lblMensagem.Text = "Introduza o email.";
                return;
            }

            string email = txtEmail.Text.Trim();

            Guid token = Guid.NewGuid();

            DateTime expira =
                DateTime.Now.AddMinutes(30);

            int resultado;

            using (SqlConnection ligacao =
                new SqlConnection(DatabaseConfig.ConnectionString))

            using (SqlCommand comando =
                new SqlCommand(
                    "sp_CriarTokenRecuperacao",
                    ligacao))
            {
                comando.CommandType =
                    CommandType.StoredProcedure;

                comando.Parameters.AddWithValue(
                    "@email",
                    email);

                comando.Parameters.AddWithValue(
                    "@token",
                    token);

                comando.Parameters.AddWithValue(
                    "@expira",
                    expira);

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

            if (resultado == 1)
            {
                try
                {
                    EmailService.EnviarRecuperacao(
                        email,
                        token.ToString());
                }
                catch
                {
                    lblMensagem.CssClass =
                        "mensagem-erro";

                    lblMensagem.Text =
                        "Não foi possível enviar o email.";

                    return;
                }
            }

           
            lblMensagem.CssClass =
                "mensagem-sucesso";

            lblMensagem.Text =
                "Se existir uma conta ativa com esse email, receberá uma ligação para recuperar a palavra-passe.";
        }
    }
}