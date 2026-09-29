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
    public partial class mostra_candidato : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_ver_Click(object sender, EventArgs e)
        {

            SqlConnection myconn = new SqlConnection(ConfigurationManager.ConnectionStrings["atec0226ConnectionString"].ConnectionString);

            SqlCommand mycommand = new SqlCommand();
            mycommand.CommandType = CommandType.StoredProcedure;
            mycommand.CommandText = "mostra_candidato";

            mycommand.Connection = myconn;

            myconn.Open();
            mycommand.Parameters.AddWithValue("@numCand", ddl_candidato.SelectedValue);
            SqlDataReader myDataReader = mycommand.ExecuteReader();

            if (myDataReader.Read())
            {
                lbl_nome.Text = myDataReader["nome"].ToString();
                lbl_curso.Text = myDataReader["curso"].ToString();
                lt_carta.Text = myDataReader["carta"].ToString();

                img_foto.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String((byte[])myDataReader["foto"]);

            }         

            myconn.Close();
        }

        protected void SqlDataSource_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {

        }
    }
}