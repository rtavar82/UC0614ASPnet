using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace UC0614_ExFinal
{
    public partial class AdminUsers : Page
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

            if (!IsPostBack)
            {
                CarregarUtilizadores();
            }
        }

        private void CarregarUtilizadores()
        {
            using (SqlConnection ligacao =
                new SqlConnection(DatabaseConfig.ConnectionString))

            using (SqlCommand comando =
                new SqlCommand("sp_ListarUtilizadores", ligacao))
            {
                comando.CommandType = CommandType.StoredProcedure;

                using (SqlDataAdapter adaptador =
                    new SqlDataAdapter(comando))
                {
                    DataTable tabela = new DataTable();

                    adaptador.Fill(tabela);

                    gvUtilizadores.DataSource = tabela;
                    gvUtilizadores.DataBind();
                }
            }
        }

        protected void gvUtilizadores_RowCommand(
    object sender,
    System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Eliminar")
                return;

            int utilizadorId = Convert.ToInt32(e.CommandArgument);

            // Impedir que o administrador elimine a própria conta
            if (utilizadorId == Convert.ToInt32(Session["UtilizadorId"]))
            {
                lblMensagem.CssClass = "mensagem-erro";
                lblMensagem.Text = "Não pode eliminar a conta com que tem sessão iniciada.";
                return;
            }

            using (SqlConnection ligacao =
                new SqlConnection(DatabaseConfig.ConnectionString))

            using (SqlCommand comando =
                new SqlCommand("sp_EliminarUtilizador", ligacao))
            {
                comando.CommandType = CommandType.StoredProcedure;

                comando.Parameters.AddWithValue(
                    "@id",
                    utilizadorId);

                ligacao.Open();
                comando.ExecuteNonQuery();
            }

            lblMensagem.CssClass = "mensagem-sucesso";
            lblMensagem.Text = "Utilizador eliminado com sucesso.";

            CarregarUtilizadores();
        }

    }
}