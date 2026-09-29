using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace UC0614_ExFinal
{
    public partial class ActivateAccount : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Guid token;
            if (!Guid.TryParse(Request.QueryString["token"], out token)) { lblMensagem.Text = "Link de ativação inválido."; return; }
            //using (SqlConnection ligacao = new SqlConnection(ConfigurationManager.ConnectionStrings["UC0614ConnectionString"].ConnectionString))
            using (SqlConnection ligacao = new SqlConnection(DatabaseConfig.ConnectionString))
            using (SqlCommand comando = new SqlCommand("sp_AtivarConta", ligacao))
            {
                comando.CommandType = CommandType.StoredProcedure;
                comando.Parameters.AddWithValue("@token", token);
                SqlParameter retorno = comando.Parameters.Add("@resultado", SqlDbType.Int); retorno.Direction = ParameterDirection.Output;
                ligacao.Open(); comando.ExecuteNonQuery();
                lblMensagem.CssClass = Convert.ToInt32(retorno.Value) == 1 ? "mensagem-sucesso" : "mensagem-erro";
                lblMensagem.Text = Convert.ToInt32(retorno.Value) == 1 ? "Conta ativada com sucesso. Já pode iniciar sessão." : "Este link já foi utilizado ou não é válido.";
            }
        }
    }
}
