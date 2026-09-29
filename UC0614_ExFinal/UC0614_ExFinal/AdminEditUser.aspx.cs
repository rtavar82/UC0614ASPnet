using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace UC0614_ExFinal
{
    public partial class AdminEditUser : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Apenas administradores
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

            int utilizadorId;

            if (!Int32.TryParse(
                Request.QueryString["id"],
                out utilizadorId))
            {
                Response.Redirect("~/AdminUsers.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarUtilizador(utilizadorId);
            }
        }

        private void CarregarUtilizador(int utilizadorId)
        {
            using (SqlConnection ligacao =
                new SqlConnection(DatabaseConfig.ConnectionString))

            using (SqlCommand comando =
                new SqlCommand(
                    "sp_ObterUtilizadorPorId",
                    ligacao))
            {
                comando.CommandType =
                    CommandType.StoredProcedure;

                comando.Parameters.AddWithValue(
                    "@id",
                    utilizadorId);

                ligacao.Open();

                using (SqlDataReader leitor =
                    comando.ExecuteReader())
                {
                    if (!leitor.Read())
                    {
                        Response.Redirect(
                            "~/AdminUsers.aspx");

                        return;
                    }

                    txtNome.Text =
                        leitor["Nome"].ToString();

                    txtEmail.Text =
                        leitor["Email"].ToString();

                    ddlPerfil.SelectedValue =
                        leitor["PerfilId"].ToString();

                    ddlEstado.SelectedValue =
                        Convert.ToBoolean(
                            leitor["Ativo"])
                            ? "1"
                            : "0";
                }
            }
        }

        protected void btnGuardar_Click(
            object sender,
            EventArgs e)
        {
            int utilizadorId;

            if (!Int32.TryParse(
                Request.QueryString["id"],
                out utilizadorId))
            {
                return;
            }

            if (String.IsNullOrWhiteSpace(txtNome.Text) ||
                String.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblMensagem.CssClass =
                    "mensagem-erro";

                lblMensagem.Text =
                    "Preencha o nome e o email.";

                return;
            }

            int resultado;

            using (SqlConnection ligacao =
                new SqlConnection(DatabaseConfig.ConnectionString))

            using (SqlCommand comando =
                new SqlCommand(
                    "sp_AlterarUtilizador",
                    ligacao))
            {
                comando.CommandType =
                    CommandType.StoredProcedure;

                comando.Parameters.AddWithValue(
                    "@id",
                    utilizadorId);

                comando.Parameters.AddWithValue(
                    "@nome",
                    txtNome.Text.Trim());

                comando.Parameters.AddWithValue(
                    "@email",
                    txtEmail.Text.Trim());

                comando.Parameters.AddWithValue(
                    "@perfilId",
                    Convert.ToInt32(
                        ddlPerfil.SelectedValue));

                comando.Parameters.AddWithValue(
                    "@ativo",
                    ddlEstado.SelectedValue == "1");

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
                lblMensagem.CssClass =
                    "mensagem-erro";

                lblMensagem.Text =
                    "Utilizador não encontrado.";

                return;
            }

            if (resultado == 2)
            {
                lblMensagem.CssClass =
                    "mensagem-erro";

                lblMensagem.Text =
                    "Já existe outro utilizador com este email.";

                return;
            }

            Response.Redirect("~/AdminUsers.aspx");
        }
    }
}