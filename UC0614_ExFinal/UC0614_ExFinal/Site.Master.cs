using System;
using System.Web;
using System.Web.UI;

namespace UC0614_ExFinal
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool autenticado = Session["UtilizadorId"] != null;

            phAnonimo.Visible = !autenticado;
            phAutenticado.Visible = autenticado;

            if (autenticado)
            {
                lblNomeUtilizador.Text =
                    Server.HtmlEncode(
                        Convert.ToString(Session["UtilizadorNome"])
                    );

                phAdmin.Visible =
                    Convert.ToString(Session["Perfil"]) == "Administrador";
            }
            else
            {
                phAdmin.Visible = false;
            }
        }

        protected void btnTerminarSessao_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Redirect("~/Login.aspx");
        }
    }
}