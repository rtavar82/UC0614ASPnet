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
    public partial class inserir_candidato : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_inserir_Click(object sender, EventArgs e)
        {

            SqlConnection myconn = new SqlConnection(ConfigurationManager.ConnectionStrings["atec0226ConnectionString"].ConnectionString);

            SqlCommand mycommand = new SqlCommand();

            mycommand.CommandType = CommandType.StoredProcedure;
            mycommand.CommandText = "inserir_candidato";

            mycommand.Connection = myconn;

            string ct = fu_foto.PostedFile.ContentType;

            Stream imgstream = fu_foto.PostedFile.InputStream;
            int imgTamanho = fu_foto.PostedFile.ContentLength;

            byte[] imgBinaryData = new byte[imgTamanho];
            imgstream.Read(imgBinaryData, 0, imgBinaryData.Length);

            mycommand.Parameters.AddWithValue("@nome", tb_nome.Text);
            mycommand.Parameters.AddWithValue("@curso", ddl_curso.SelectedItem.ToString());
            mycommand.Parameters.AddWithValue("@carta", tb_carta.Text);

            mycommand.Parameters.AddWithValue("@fotoDadosBinarios", imgBinaryData);
            mycommand.Parameters.AddWithValue("@fotoCt", ct);


            myconn.Open();
            mycommand.ExecuteNonQuery();

            myconn.Close();

        }
    }
}