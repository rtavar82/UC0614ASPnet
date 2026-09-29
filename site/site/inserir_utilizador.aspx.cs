using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace site
{
    public partial class inserir_utilizador : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btn_inserir_Click(object sender, EventArgs e)
        {
            SqlConnection myconn = new SqlConnection(ConfigurationManager.ConnectionStrings["atec0226ConnectionString"].ConnectionString);

            SqlCommand mycommand = new SqlCommand("");

            mycommand.CommandType = CommandType.StoredProcedure;
            mycommand.CommandText = "inserir_utilizador";

            mycommand.Connection = myconn;

            mycommand.Parameters.AddWithValue("@utilizador", tb_utilizador.Text);
            mycommand.Parameters.AddWithValue("@pw", EncryptString(tb_passe.Text));
            mycommand.Parameters.AddWithValue("@cod_perfil", ddl_perfil.SelectedValue);
            mycommand.Parameters.AddWithValue("@email", tb_email.Text);

            SqlParameter valor = new SqlParameter();
            valor.ParameterName = "@retorno";
            valor.Direction = ParameterDirection.Output;
            valor.SqlDbType = SqlDbType.Int;
            mycommand.Parameters.Add(valor);

            myconn.Open();
            
            mycommand.ExecuteNonQuery();
            int respostaSP = Convert.ToInt32(mycommand.Parameters["@retorno"].Value);
            myconn.Close();

            if(respostaSP == 0)
            {
                lbl_mensagem.Text = "Este utilizador já existe!";
            }
            else
            {
                lbl_mensagem.Text = "Utilizador inserido com sucesso! Recebeu um email de confirmação para ativar a conta.";

                MailMessage email = new MailMessage();
                SmtpClient servidor = new SmtpClient();

                string gmailUser = ConfigurationManager.AppSettings["GmailSmtpUser"];
                string gmailPassword = ConfigurationManager.AppSettings["GmailSmtpAppPassword"];
                
                email.From = new MailAddress(gmailUser);
                email.To.Add(tb_email.Text);

                email.Subject = "Ativação de conta";
                email.IsBodyHtml = true;
                email.Body = $"Obrigado por registar-se. Por favor, ative a sua conta clicando no link abaixo.<br><br><a href='https://localhost:44362/ativar_conta.aspx?util={EncryptString(tb_utilizador.Text)}'>Ativar Conta</a>";

                servidor.Host = "smtp.gmail.com";
                servidor.Port = 587;
                servidor.UseDefaultCredentials = false;
                servidor.DeliveryMethod = SmtpDeliveryMethod.Network;
                servidor.EnableSsl = true;
                servidor.Credentials = new System.Net.NetworkCredential(gmailUser, gmailPassword);

                email.ReplyToList.Add(new MailAddress(tb_email.Text));

                servidor.Send(email);

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