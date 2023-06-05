using BackendRestF2x.Controllers;
using System.Net;
using System.Net.Http.Headers;

namespace Web.Util
{
    public class General
    {
        
        /// <summary>
        ///  @Author: Juan Bernal
        ///  @Date: 31/05/2023
        ///  @Desc: Metodo que genera una consulta al api y trae la información asociada
        ///  @Retuns: Cadena.
        public static Response GetApi(string strUrl, string strToken)
        {

            Response objResponse = new Response()
            {
                Resp = true
            };

            try
            {

                using HttpClient client = new HttpClient();

                // Establecer el encabezado de autorización con el Bearer Token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", strToken);

                // Realizar la solicitud GET y obtener la respuesta
                HttpResponseMessage response = client.GetAsync(strUrl).Result;

                // Verificar si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Leer el contenido de la respuesta como una cadena
                    objResponse.Data = response.Content.ReadAsStringAsync().Result;
                    objResponse.Resp = true;
                    if (response.StatusCode == HttpStatusCode.NoContent)
                    {
                        objResponse.StatusCode = 204;
                    }
                    
                }
                else
                {
                    objResponse.StatusCode = 400;
                    objResponse.Resp = false;
                    objResponse.Msg = response.RequestMessage.ToString();
                    objResponse.Detail = "Error en la solicitud de conteo de vehiculos";
                }
            }
            catch (Exception ex)
            {
                objResponse.StatusCode = 400;
                objResponse.Resp = false;
                objResponse.Msg = ex.Message;
                objResponse.Detail = ex.Message;
            }

            return objResponse;
        }

    }
}
