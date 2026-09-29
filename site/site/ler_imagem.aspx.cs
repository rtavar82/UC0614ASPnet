using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace site
{
    public partial class ler_imagem : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_mostrar_Click(object sender, EventArgs e)
        {
            SqlConnection myconn = new SqlConnection(ConfigurationManager.ConnectionStrings["atec0226ConnectionString"].ConnectionString);

            SqlCommand mycommand = new SqlCommand();

            mycommand.CommandType = CommandType.StoredProcedure;
            mycommand.CommandText = "ler_imagem";

            mycommand.Connection = myconn;

            mycommand.Parameters.AddWithValue("@numero",ddl_imagem.SelectedValue);

            myconn.Open();
            SqlDataReader myDataReader = mycommand.ExecuteReader();
            if(myDataReader.Read())
            {
                Response.ContentType = myDataReader["contentType"].ToString();
                Response.BinaryWrite((byte[])myDataReader["dadosBinarios"]);
            }

            myconn.Close();

        }
    }
}