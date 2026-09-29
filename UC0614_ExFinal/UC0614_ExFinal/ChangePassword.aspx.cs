using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace UC0614_ExFinal
{
    public partial class ChangePassword : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Só permite alterar a password se existir sessão iniciada
            if (Session["UtilizadorId"] == null)
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void btnAlterar_Click(object sender, EventArgs e)
        {
            lblMensagem.CssClass = "mensagem-erro";

            // 1. Validar campos
            if (String.IsNullOrWhiteSpace(txtPasswordAtual.Text) ||
                String.IsNullOrWhiteSpace(txtPasswordNova.Text) ||
                String.IsNullOrWhiteSpace(txtConfirmarPassword.Text))
            {
                lblMensagem.Text = "Preencha todos os campos.";
                return;
            }

            // 2. Validar tamanho mínimo
            if (txtPasswordNova.Text.Length < 8)
            {
                lblMensagem.Text = "A nova palavra-passe deve ter pelo menos 8 caracteres.";
                return;
            }

            // 3. Confirmar se as novas passwords coincidem
            if (txtPasswordNova.Text != txtConfirmarPassword.Text)
            {
                lblMensagem.Text = "As novas palavras-passe não coincidem.";
                return;
            }

            int utilizadorId = Convert.ToInt32(Session["UtilizadorId"]);

            string connectionString = DatabaseConfig.ConnectionString;

            string passwordHashAtual;

            // 4. Obter o hash atual da base de dados
            using (SqlConnection ligacao = new SqlConnection(connectionString))
            using (SqlCommand comando = new SqlCommand(
                "SELECT PasswordHash FROM Utilizadores WHERE Id = @id", ligacao))
            {
                comando.Parameters.AddWithValue("@id", utilizadorId);

                ligacao.Open();

                object resultado = comando.ExecuteScalar();

                if (resultado == null)
                {
                    lblMensagem.Text = "Utilizador não encontrado.";
                    return;
                }

                passwordHashAtual = resultado.ToString();
            }

            // 5. Verificar a palavra-passe atual
            if (!PasswordSecurity.Verify(txtPasswordAtual.Text, passwordHashAtual))
            {
                lblMensagem.Text = "A palavra-passe atual está incorreta.";
                return;
            }

            // 6. Criar hash da nova palavra-passe
            string novoPasswordHash =
                PasswordSecurity.CreateHash(txtPasswordNova.Text);

            // 7. Atualizar a palavra-passe
            using (SqlConnection ligacao = new SqlConnection(connectionString))
            using (SqlCommand comando = new SqlCommand("sp_AlterarPassword", ligacao))
            {
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue("@id", utilizadorId);
                comando.Parameters.AddWithValue("@passwordHash", novoPasswordHash);

                ligacao.Open();
                comando.ExecuteNonQuery();
            }

            // 8. Limpar campos e apresentar sucesso
            txtPasswordAtual.Text = "";
            txtPasswordNova.Text = "";
            txtConfirmarPassword.Text = "";

            lblMensagem.CssClass = "mensagem-sucesso";
            lblMensagem.Text = "Palavra-passe alterada com sucesso.";
        }
    }
}