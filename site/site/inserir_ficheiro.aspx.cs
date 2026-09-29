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
    public partial class inserir_ficheiro : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_adicionar_Click(object sender, EventArgs e)
        {
            SqlConnection myconn = new SqlConnection(ConfigurationManager.ConnectionStrings["atec0226ConnectionString"].ConnectionString);

            SqlCommand mycommand = new SqlCommand();

            mycommand.CommandType = CommandType.StoredProcedure;
            mycommand.CommandText = "inserir_ficheiro";

            mycommand.Connection = myconn;

            string ct = FileUpload1.PostedFile.ContentType;

            Stream imgstream = FileUpload1.PostedFile.InputStream;
            int imgTamanho = FileUpload1.PostedFile.ContentLength;

            byte[] imgBinaryData = new byte[imgTamanho];
            imgstream.Read(imgBinaryData, 0, imgBinaryData.Length);

            mycommand.Parameters.AddWithValue("@nomeFicheiro", tb_ficheiro.Text);
            mycommand.Parameters.AddWithValue("@dadosBinarios", imgBinaryData);
            mycommand.Parameters.AddWithValue("@ct",ct);


            myconn.Open();
            mycommand.ExecuteNonQuery();

            myconn.Close();

        }
    }
}