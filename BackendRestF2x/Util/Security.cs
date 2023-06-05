using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using Web.Models;


namespace Web.Util
{
    public class Security
    {


        /*
        Author: Juan Pablo Bernal
        Date: 05/31/2023
        Desc: Valida la credenciales obtenidas en el Header de una petición Http
        Params: El header de autenticación, UserNameConfig, PasswordConfig
        Return: True si coincide con las credenciales alojadas en el código fuente, de lo contrario false
        */
        public static bool IsValidateAuth(AuthenticationHeaderValue authHeader, string strConfigUserName, string strConfigPassword)
        {
            bool blValid = false;
            try
            {
                var btCredentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var arrayCredentials = Encoding.UTF8.GetString(btCredentialBytes).Split(new[] { ':' }, 2);
                var strUsername = arrayCredentials[0];
                var strPassword = arrayCredentials[1];
                blValid = strUsername.Equals(strConfigUserName) && strPassword.Equals(strConfigPassword);
            }
            catch (Exception)
            {
                blValid = false;
            }
            return blValid;
        }

        /// <summary>
        ///  @Author: Juan Bernal
        ///  @Date: 31/05/2023
        ///  @Desc: Metodo que genera un token
        ///  @Retuns: Cadena.
        public static async Task<string> GetToken()
        {

            string strAccessToken = "";


            try
            {
                HttpClient client = new HttpClient();

                string url = Config.UrlBase + "/Login";

                string requestData = "{\"userName\": \"user\", \"password\": \"1234\"}";

                HttpContent content = new StringContent(requestData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    JObject objValue = JObject.Parse(responseBody);

                    strAccessToken = objValue["token"].ToString();
                }
                else
                {
                    strAccessToken = "Error";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return strAccessToken;
        }

    }
}
