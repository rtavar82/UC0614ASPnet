using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace UC0614_ExFinal
{
    public partial class AdminCreateUser : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UtilizadorId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            if (Convert.ToString(Session["Perfil"]) != "Administrador")
            {
                Response.Redirect("~/Default.aspx");
                return;
            }
        }

        protected void btnCriar_Click(object sender, EventArgs e)
        {
            lblMensagem.CssClass = "mensagem-erro";

            if (String.IsNullOrWhiteSpace(txtNome.Text) ||
                String.IsNullOrWhiteSpace(txtEmail.Text) ||
                String.IsNullOrWhiteSpace(txtPassword.Text) ||
                String.IsNullOrWhiteSpace(txtConfirmarPassword.Text))
            {
                lblMensagem.Text = "Preencha todos os campos.";
                return;
            }

            if (txtPassword.Text.Length < 8)
            {
                lblMensagem.Text =
                    "A palavra-passe deve ter pelo menos 8 caracteres.";
                return;
            }

            if (txtPassword.Text != txtConfirmarPassword.Text)
            {
                lblMensagem.Text =
                    "As palavras-passe não coincidem.";
                return;
            }

            string nome = txtNome.Text.Trim();
            string email = txtEmail.Text.Trim();

            int perfilId =
                Convert.ToInt32(ddlPerfil.SelectedValue);

            bool ativo =
                ddlEstado.SelectedValue == "1";

            string passwordHash =
                PasswordSecurity.CreateHash(txtPassword.Text);

            Guid? tokenAtivacao =
                ativo ? (Guid?)null : Guid.NewGuid();

            int resultado;

            using (SqlConnection ligacao =
                new SqlConnection(DatabaseConfig.ConnectionString))

            using (SqlCommand comando =
                new SqlCommand(
                    "sp_InserirUtilizadorAdmin",
                    ligacao))
            {
                comando.CommandType =
                    CommandType.StoredProcedure;

                comando.Parameters.AddWithValue(
                    "@nome",
                    nome);

                comando.Parameters.AddWithValue(
                    "@email",
                    email);

                comando.Parameters.AddWithValue(
                    "@passwordHash",
                    passwordHash);

                comando.Parameters.AddWithValue(
                    "@perfilId",
                    perfilId);

                comando.Parameters.AddWithValue(
                    "@ativo",
                    ativo);

                comando.Parameters.AddWithValue(
                    "@tokenAtivacao",
                    tokenAtivacao.HasValue
                        ? (object)tokenAtivacao.Value
                        : DBNull.Value);

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
                lblMensagem.Text =
                    "Já existe um utilizador com este email.";
                return;
            }

            // Se a conta foi criada como pendente,
            // envia o mesmo email de ativação usado no registo público.
            if (!ativo && tokenAtivacao.HasValue)
            {
                try
                {
                    EmailService.EnviarAtivacao(
                        email,
                        nome,
                        tokenAtivacao.Value.ToString());
                }
                catch
                {
                    lblMensagem.CssClass =
                        "mensagem-erro";

                    lblMensagem.Text =
                        "O utilizador foi criado, mas não foi possível enviar o email de ativação.";

                    return;
                }
            }

            Response.Redirect("~/AdminUsers.aspx");
        }
    }
}