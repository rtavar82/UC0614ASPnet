using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;


namespace site
{
    public partial class alterar_pw : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_alterar_Click(object sender, EventArgs e)
        {
            SqlConnection myconn = new SqlConnection(ConfigurationManager.ConnectionStrings["atec0226ConnectionString"].ConnectionString);

            SqlCommand mycommand = new SqlCommand("");
            mycommand.CommandType = CommandType.StoredProcedure;
            mycommand.CommandText = "alterarPW";

            mycommand.Connection = myconn;

            mycommand.Parameters.AddWithValue("@utilizador", Session["utilizador"].ToString());
            mycommand.Parameters.AddWithValue("@pw_atual", EncryptString(tb_pw_atual.Text));
            mycommand.Parameters.AddWithValue("@pw_nova", EncryptString(tb_pw_nova.Text));


            SqlParameter valor = new SqlParameter();
            valor.ParameterName = "@retorno";
            valor.Direction = ParameterDirection.Output;
            valor.SqlDbType = SqlDbType.Int;
            mycommand.Parameters.Add(valor);


            myconn.Open();

            mycommand.ExecuteNonQuery();
            int respostaSP = Convert.ToInt32(mycommand.Parameters["@retorno"].Value);
            
            myconn.Close();

            if (respostaSP == 0)
            {
                lbl_mensagem.Text = "palavra-passe errada!";
            }
            else
            {
                lbl_mensagem.Text = "palavra-passe alterada com sucesso!";
            }

        }
        public static string EncryptString(string Message)
        {
            string Passphrase = "formacao";
            byte[] Results;
            System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

            // Step 1. We hash the passphrase using MD5
            // We use the MD5 hash generator as the result is a 128 bit byte array
            // which is a valid length for the TripleDES encoder we use below

            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

            // Step 2. Create a new TripleDESCryptoServiceProvider object
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

            // Step 3. Setup the encoder
            TDESAlgorithm.Key = TDESKey;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;

            // Step 4. Convert the input string to a byte[]
            byte[] DataToEncrypt = UTF8.GetBytes(Message);

            // Step 5. Attempt to encrypt the string
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                // Clear the TripleDes and Hashprovider services of any sensitive information
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }

            // Step 6. Return the encrypted string as a base64 encoded string

            string enc = Convert.ToBase64String(Results);
            enc = enc.Replace("+", "KKK");
            enc = enc.Replace("/", "JJJ");
            enc = enc.Replace("\\", "III");
            return enc;



        }
    }
}