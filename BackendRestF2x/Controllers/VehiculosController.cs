using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Web.Models;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackendRestF2x.Controllers
{
    [Route("api/Vehiculos")]
    [ApiController]
    public class VehiculosController : ControllerBase
    {

        #region Conteo
        [HttpGet("LlenadoConteo")]
        [AllowAnonymous]
        public Response GetLlenadoVehiculosConteo()
        {
            Response objResponse = new Response()
            {
                Resp = true,
                StatusCode = 200,
                Msg = "Población con éxito a la base de datos",
                Detail = "Ejecución con éxito",
                Type = "Success",
            };

            if (Request.Headers.ContainsKey("Authorization") && Web.Util.Security.IsValidateAuth(AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]), Config.KeysApiControllerUsername, Config.KeysApiControllerPassword))
            {
                try
                {
                    //Conexion a la base de datos
                    Connection objConnection = new Connection(Config.DBPass, Config.DBServer, Config.DBName, Config.DBUser);
                    
                    // Bearer Token de autenticación
                    string strToken = Web.Util.Security.GetToken().Result;

                    bool blFecha = true;

                    // URL del servicio a consumir
                    DateTime fecha = DateTime.Parse("2021-07-02");

                    while (blFecha)
                    {
                        string strFechaFormated = fecha.ToString("yyyy-MM-dd");

                        // URL del servicio a consumir
                        string strUrl = Config.UrlBase + $"/ConteoVehiculos/{strFechaFormated}";

                        Response objResponseConteo = Web.Util.General.GetApi(strUrl, strToken);

                        if (!objResponseConteo.Resp)
                        {
                            objResponse.StatusCode = 400;
                            objResponse.Resp = false;
                            objResponse.Msg = objResponseConteo.Msg;
                            objResponse.Detail = objResponseConteo.Detail;
                            return objResponse;
                        }

                        if (objResponseConteo.StatusCode == 204)
                        {
                            blFecha = false;
                            break;
                        }

                        JArray arrayValue = new(JArray.Parse(objResponseConteo.Data));

                        foreach (JObject valueConteo in arrayValue)
                        {
                            string strEstacion = valueConteo["estacion"].ToString();
                            string strSentido = valueConteo["sentido"].ToString();
                            string strHora = valueConteo["hora"].ToString();
                            string strCategoria = valueConteo["categoria"].ToString();
                            string strCantidad = valueConteo["cantidad"].ToString();

                            string strBaseQuery = "INSERT INTO ConteoVehiculos(estacion, sentido, hora, categoria, cantidad, fecha) OUTPUT INSERTED.GuidRegistro ";
                            strBaseQuery += "VALUES(@P_estacion, @P_sentido, @P_hora, @P_categoria, @P_cantidad, @P_fecha )";

                            string[] arrayValues = new string[6] { strEstacion, strSentido, strHora, strCategoria, strCantidad, strFechaFormated };
                            string[] arrayParams = new string[6] { "@P_estacion", "@P_sentido", "@P_hora", "@P_categoria", "@P_cantidad", "@P_fecha" };

                            ResponseDB objResponseDBInsert = objConnection.InsData(strBaseQuery, arrayParams, arrayValues);

                            if (!objResponseDBInsert.Resp)
                            {
                                objResponse.Msg += objResponseDBInsert.Msg;
                                objResponse.Resp = false;
                                objResponse.Type = "Error";
                                objResponse.StatusCode = 400;
                            }
                        }

                        fecha = fecha.AddDays(1);
                    }
                }
                catch (Exception ex)
                {
                    objResponse.StatusCode = 400;
                    objResponse.Resp = false;
                    objResponse.Msg = ex.Message;
                    objResponse.Detail = ex.Message;
                    objResponse.Type = "Error";
                }
            }
            else
            {
                objResponse.StatusCode = 400;
                objResponse.Resp = false;
                objResponse.Msg = "No tiene las credenciales correctas";
                objResponse.Detail = "No tiene las credenciales correctas";
                objResponse.Type = "Error";
            }

            return objResponse;
        }

        [HttpGet("VehiculosConteoPorFecha")]
        [AllowAnonymous]
        public Response GetVehiculosConteo(string strFecha)
        {
            Response objResponse = new Response()
            {
                Resp = true,
                StatusCode = 200,
                Msg = "Población con éxito a la base de datos",
                Detail = "Ejecución con éxito",
                Type = "Success",
            };

            if (Request.Headers.ContainsKey("Authorization") && Web.Util.Security.IsValidateAuth(AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]), Config.KeysApiControllerUsername, Config.KeysApiControllerPassword)
)
            {
                try
                {
                    // Bearer Token de autenticación
                    string strToken = Web.Util.Security.GetToken().Result;

                    // URL del servicio a consumir
                    string strUrl = Config.UrlBase + $"/ConteoVehiculos/{strFecha}";

                    Response objResponseConteo = Web.Util.General.GetApi(strUrl, strToken);

                    if (!objResponseConteo.Resp)
                    {
                        objResponse.StatusCode = 400;
                        objResponse.Resp = false;
                        objResponse.Msg = objResponseConteo.Msg;
                        objResponse.Detail = objResponseConteo.Detail;
                        return objResponse;
                    }

                    if (objResponseConteo.StatusCode == 204)
                    {
                        objResponse.StatusCode = 200;
                        objResponse.Resp = true;
                        objResponse.Msg = "Información con éxito";
                        objResponse.Detail = "No se encuentran más registros con la fecha asociada";
                        objResponse.Type = "Success";
                        objResponse.Data = "[]";
                        return objResponse;
                    }

                    JArray arrayValue = new(JArray.Parse(objResponseConteo.Data));

                    objResponse.StatusCode = 200;
                    objResponse.Resp = true;
                    objResponse.Msg = "Información con éxito";
                    objResponse.Detail = "";
                    objResponse.Type = "Success";
                    objResponse.Data = arrayValue.ToString();
                }
                catch (Exception ex)
                {
                    objResponse.StatusCode = 400;
                    objResponse.Resp = false;
                    objResponse.Msg = ex.Message;
                    objResponse.Detail = ex.Message;
                    objResponse.Type = "Error";
                }

            }
            else
            {
                objResponse.StatusCode = 400;
                objResponse.Resp = false;
                objResponse.Msg = "No tiene las credenciales correctas";
                objResponse.Detail = "No tiene las credenciales correctas";
                objResponse.Type = "Error";
            }

            return objResponse;
        }

        [HttpGet("AllVehiculosConteoDB")]
        [AllowAnonymous]
        public Response GetAllVehiculosConteoDB()
        {
            Response objResponse = new Response()
            {
                Resp = true,
                StatusCode = 200,
                Msg = "Población con éxito a la base de datos",
                Detail = "Ejecución con éxito",
                Type = "Success",
            };

            if (Request.Headers.ContainsKey("Authorization") && Web.Util.Security.IsValidateAuth(AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]), Config.KeysApiControllerUsername, Config.KeysApiControllerPassword)
)
            {
                try
                {
                    Connection objConnection = new Connection(Config.DBPass, Config.DBServer, Config.DBName, Config.DBUser);

                    string strBaseQuery = "Select  [GuidRegistro],[id],[estacion] ,[sentido] ,[hora] " +
                        ",[categoria] ,[cantidad] ,[fecha] ,[createdOn] ,[modifiedOn] from ConteoVehiculos";

                    ResponseDB objRespConteo = objConnection.getRespFromQuery("", strBaseQuery, "NO_PAGINATE", "DataTable");

                    if (!objRespConteo.Resp)
                    {
                        objResponse.StatusCode = 400;
                        objResponse.Resp = false;
                        objResponse.Msg = objRespConteo.Msg;
                        return objResponse;
                    }

                    JArray arrayValue = new(JArray.Parse(JsonConvert.SerializeObject(objRespConteo.dtResult, Formatting.None)));

                    objResponse.StatusCode = 200;
                    objResponse.Resp = true;
                    objResponse.Msg = "Información con éxito";
                    objResponse.Detail = "";
                    objResponse.Type = "Success";
                    objResponse.Data = arrayValue.ToString();
                }
                catch (Exception ex)
                {
                    objResponse.StatusCode = 400;
                    objResponse.Resp = false;
                    objResponse.Msg = ex.Message;
                    objResponse.Detail = ex.Message;
                    objResponse.Type = "Error";
                }

            }
            else
            {
                objResponse.StatusCode = 400;
                objResponse.Resp = false;
                objResponse.Msg = "No tiene las credenciales correctas";
                objResponse.Detail = "No tiene las credenciales correctas";
                objResponse.Type = "Error";
            }

            return objResponse;
        }
        #endregion

        #region Recaudo 

        [HttpGet("LlenadoRecaudo")]
        [AllowAnonymous]
        public Response GetLlenadoVehiculosRecaudo()
        {
            Response objResponse = new Response()
            {
                Resp = true,
                StatusCode = 200,
                Msg = "Población con éxito a la base de datos",
                Detail = "Ejecución con éxito",
                Type = "Success",
            };

            if (Request.Headers.ContainsKey("Authorization") && Web.Util.Security.IsValidateAuth(AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]), Config.KeysApiControllerUsername, Config.KeysApiControllerPassword))
            {
                try
                {
                    //Conexion a la base de datos
                    Connection objConnection = new Connection(Config.DBPass, Config.DBServer, Config.DBName, Config.DBUser);

                    // Bearer Token de autenticación
                    string strToken = Web.Util.Security.GetToken().Result;

                    bool blFecha = true;
                    // URL del servicio a consumir
                    DateTime fecha = DateTime.Parse("2021-08-19");

                    while (blFecha)
                    {
                        string strFechaFormated = fecha.ToString("yyyy-MM-dd");

                        string strUrl = Config.UrlBase + $"/RecaudoVehiculos/{strFechaFormated}";

                        Response objResponseConteo = Web.Util.General.GetApi(strUrl, strToken);

                        if (!objResponseConteo.Resp)
                        {
                            objResponse.StatusCode = 400;
                            objResponse.Resp = false;
                            objResponse.Msg = objResponseConteo.Msg;
                            objResponse.Detail = objResponseConteo.Detail;
                            return objResponse;
                        }

                        if (objResponseConteo.StatusCode == 204)
                        {
                            blFecha = false;
                            break;
                        }

                        JArray arrayValue = new(JArray.Parse(objResponseConteo.Data));

                        foreach (JObject valueRecaudo in arrayValue)
                        {
                            #pragma warning disable CS8602 
                            string strEstacion = valueRecaudo["estacion"].ToString();
                            string strSentido = valueRecaudo["sentido"].ToString();
                            string strHora = valueRecaudo["hora"].ToString();
                            string strCategoria = valueRecaudo["categoria"].ToString();
                            string strvalorTabulado = valueRecaudo["valorTabulado"].ToString();
                            #pragma warning restore CS8602 

                            string strBaseQuery = "INSERT INTO RecaudoVehiculos(estacion, sentido, hora, categoria, valorTabulado, fecha) OUTPUT INSERTED.GuidRegistro ";
                            strBaseQuery += "VALUES(@P_estacion, @P_sentido, @P_hora, @P_categoria, @P_valorTabulado, @P_fecha )";

                            string[] arrayValues = new string[6] { strEstacion, strSentido, strHora, strCategoria, strvalorTabulado, strFechaFormated };
                            string[] arrayParams = new string[6] { "@P_estacion", "@P_sentido", "@P_hora", "@P_categoria", "@P_valorTabulado", "@P_fecha" };

                            ResponseDB objResponseDBInsert = objConnection.InsData(strBaseQuery, arrayParams, arrayValues);

                            if (!objResponseDBInsert.Resp)
                            {
                                objResponse.Msg += objResponseDBInsert.Msg;
                                objResponse.Resp = false;
                                objResponse.Type = "Error";
                                objResponse.StatusCode = 400;
                                return objResponse;
                            }
                        }

                        fecha = fecha.AddDays(1);
                    }
                }
                catch (Exception ex)
                {
                    objResponse.StatusCode = 400;
                    objResponse.Resp = false;
                    objResponse.Msg = ex.Message;
                    objResponse.Detail = ex.Message;
                    objResponse.Type = "Error";
                }
            }
            else
            {
                objResponse.StatusCode = 400;
                objResponse.Resp = false;
                objResponse.Msg = "No tiene las credenciales correctas";
                objResponse.Detail = "No tiene las credenciales correctas";
                objResponse.Type = "Error";
            }

            return objResponse;
        }

        [HttpGet("VehiculosRecaudoPorFecha")]
        [AllowAnonymous]
        public Response GetVehiculosRecaudoPorFecha(string strFecha)
        {
            Response objResponse = new Response()
            {
                Resp = true,
                StatusCode = 200,
                Msg = "Población con éxito a la base de datos",
                Detail = "Ejecución con éxito",
                Type = "Success",
            };

            if (Request.Headers.ContainsKey("Authorization") && Web.Util.Security.IsValidateAuth(AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]), Config.KeysApiControllerUsername, Config.KeysApiControllerPassword)
)
            {
                try
                {
                    // Bearer Token de autenticación
                    string strToken = Web.Util.Security.GetToken().Result;

                    // URL del servicio a consumir
                    string strUrl = Config.UrlBase + $"/RecaudoVehiculos/{strFecha}";

                    Response objResponseConteo = Web.Util.General.GetApi(strUrl, strToken);

                    if (!objResponseConteo.Resp)
                    {
                        objResponse.StatusCode = 400;
                        objResponse.Resp = false;
                        objResponse.Msg = objResponseConteo.Msg;
                        objResponse.Detail = objResponseConteo.Detail;
                        return objResponse;
                    }

                    if (objResponseConteo.StatusCode == 204)
                    {
                        objResponse.StatusCode = 200;
                        objResponse.Resp = true;
                        objResponse.Msg = "Información con éxito";
                        objResponse.Detail = "No se encuentran más registros con la fecha asociada";
                        objResponse.Type = "Success";
                        objResponse.Data = "[]";
                        return objResponse;
                    }

                    JArray arrayValue = new(JArray.Parse(objResponseConteo.Data));

                    objResponse.StatusCode = 200;
                    objResponse.Resp = true;
                    objResponse.Msg = "Información con éxito";
                    objResponse.Detail = "";
                    objResponse.Type = "Success";
                    objResponse.Data = arrayValue.ToString();
                }
                catch (Exception ex)
                {
                    objResponse.StatusCode = 400;
                    objResponse.Resp = false;
                    objResponse.Msg = ex.Message;
                    objResponse.Detail = ex.Message;
                    objResponse.Type = "Error";
                }

            }
            else
            {
                objResponse.StatusCode = 400;
                objResponse.Resp = false;
                objResponse.Msg = "No tiene las credenciales correctas";
                objResponse.Detail = "No tiene las credenciales correctas";
                objResponse.Type = "Error";
            }

            return objResponse;
        }

        [HttpGet("AllVehiculosRecaudoDB")]
        [AllowAnonymous]
        public Response GetAllVehiculosRecaudoDB()
        {
            Response objResponse = new Response()
            {
                Resp = true,
                StatusCode = 200,
                Msg = "Población con éxito a la base de datos",
                Detail = "Ejecución con éxito",
                Type = "Success",
            };

            if (Request.Headers.ContainsKey("Authorization") && Web.Util.Security.IsValidateAuth(AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]), Config.KeysApiControllerUsername, Config.KeysApiControllerPassword)
)
            {
                try
                {
                    Connection objConnection = new Connection(Config.DBPass, Config.DBServer, Config.DBName, Config.DBUser);

                    string strBaseQuery = "Select  [GuidRegistro],[id],[estacion] ,[sentido] ,[hora] " +
                        ",[categoria] ,[valorTabulado] ,[fecha] ,[createdOn] ,[modifiedOn] from RecaudoVehiculos";

                    ResponseDB objRespConteo = objConnection.getRespFromQuery("", strBaseQuery, "NO_PAGINATE", "DataTable");

                    if (!objRespConteo.Resp)
                    {
                        objResponse.StatusCode = 400;
                        objResponse.Resp = false;
                        objResponse.Msg = objRespConteo.Msg;
                        return objResponse;
                    }

                    JArray arrayValue = new(JArray.Parse(JsonConvert.SerializeObject(objRespConteo.dtResult, Formatting.None)));

                    objResponse.StatusCode = 200;
                    objResponse.Resp = true;
                    objResponse.Msg = "Información con éxito";
                    objResponse.Detail = "";
                    objResponse.Type = "Success";
                    objResponse.Data = arrayValue.ToString();
                }
                catch (Exception ex)
                {
                    objResponse.StatusCode = 400;
                    objResponse.Resp = false;
                    objResponse.Msg = ex.Message;
                    objResponse.Detail = ex.Message;
                    objResponse.Type = "Error";
                }

            }
            else
            {
                objResponse.StatusCode = 400;
                objResponse.Resp = false;
                objResponse.Msg = "No tiene las credenciales correctas";
                objResponse.Detail = "No tiene las credenciales correctas";
                objResponse.Type = "Error";
            }

            return objResponse;
        }

        #endregion

        #region Reporte

        [HttpGet("ReporteDB")]
        [AllowAnonymous]
        public Response GetReporteDB()
        {
            Response objResponse = new Response()
            {
                Resp = true,
                StatusCode = 200,
                Msg = "Población con éxito a la base de datos",
                Detail = "Ejecución con éxito",
                Type = "Success",
            };

            if (Request.Headers.ContainsKey("Authorization") && Web.Util.Security.IsValidateAuth(AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]), Config.KeysApiControllerUsername, Config.KeysApiControllerPassword)
)
            {
                try
                {
                    Connection objConnection = new Connection(Config.DBPass, Config.DBServer, Config.DBName, Config.DBUser);

                    string strBaseQueryReporteConteo = @"SELECT
                                          [fecha],
                                          [estacion],
                                          SUM([cantidad]) AS TotalCantidad
                                        FROM[ConteoVehiculos]
                                        GROUP BY[fecha], [estacion]
                                        ORDER BY[fecha]; ";

                    ResponseDB objRespConteo = objConnection.getRespFromQuery("", strBaseQueryReporteConteo, "NO_PAGINATE", "DataTable");

                    if (!objRespConteo.Resp)
                    {
                        objResponse.StatusCode = 400;
                        objResponse.Resp = false;
                        objResponse.Msg = objRespConteo.Msg;
                        return objResponse;
                    }

                    JArray arrayValueConteo = new(JArray.Parse(JsonConvert.SerializeObject(objRespConteo.dtResult, Formatting.None)));

                    string strBaseQueryReporteRecaudo = @"SELECT
                                          [fecha],
                                          [estacion],
                                          SUM([valorTabulado]) AS TotalValor
                                        FROM [RecaudoVehiculos]
                                        GROUP BY [fecha], [estacion]
                                        ORDER BY [fecha];";

                    ResponseDB objRespRecaudo = objConnection.getRespFromQuery("", strBaseQueryReporteRecaudo, "NO_PAGINATE", "DataTable");

                    if (!objRespRecaudo.Resp)
                    {
                        objResponse.StatusCode = 400;
                        objResponse.Resp = false;
                        objResponse.Msg = objRespRecaudo.Msg;
                        return objResponse;
                    }

                    JArray arrayValueRecaudo = new(JArray.Parse(JsonConvert.SerializeObject(objRespRecaudo.dtResult, Formatting.None)));

                    Dictionary<string, Dictionary<string, Reporte>> reporteUnificado = new Dictionary<string, Dictionary<string, Reporte>>();

                    foreach (var reporteCantidad in arrayValueConteo)
                    {
                        if (!reporteUnificado.ContainsKey(reporteCantidad["fecha"].ToString()))
                        {
                            reporteUnificado[reporteCantidad["fecha"].ToString()] = new Dictionary<string, Reporte>();
                        }

                        if (!reporteUnificado[reporteCantidad["fecha"].ToString()].ContainsKey(reporteCantidad["estacion"].ToString()))
                        {
                            reporteUnificado[reporteCantidad["fecha"].ToString()][reporteCantidad["estacion"].ToString()] = new Reporte();
                        }

                        reporteUnificado[reporteCantidad["fecha"].ToString()][reporteCantidad["estacion"].ToString()].TotalCantidad = reporteCantidad["TotalCantidad"].ToString();
                    }

                    foreach (JObject reporteValor in arrayValueRecaudo)
                    {
                        if (!reporteUnificado.ContainsKey(reporteValor["fecha"].ToString()))
                        {
                            reporteUnificado[reporteValor["fecha"].ToString()] = new Dictionary<string, Reporte>();
                        }

                        if (!reporteUnificado[reporteValor["fecha"].ToString()].ContainsKey(reporteValor["estacion"].ToString()))
                        {
                            reporteUnificado[reporteValor["fecha"].ToString()][reporteValor["estacion"].ToString()] = new Reporte();
                        }

                        reporteUnificado[reporteValor["fecha"].ToString()][reporteValor["estacion"].ToString()].TotalValor = reporteValor["TotalValor"].ToString();
                    }

                    JArray arrResult = new JArray();

                    foreach (var fecha in reporteUnificado.Keys)
                    {
                        JArray arrResultData = new JArray();

                        foreach (var estacion in reporteUnificado[fecha].Keys)
                        {
                            var reporte = reporteUnificado[fecha][estacion];
                           
                            JObject objParams = new JObject()
                            {
                                { "estacion", estacion },
                                {"ValorTotal",reporte.TotalValor },
                                {"ConteoTotal", reporte.TotalCantidad  }
                            };

                            arrResultData.Add(objParams);
                        }

                        JObject objData = new JObject()
                        {
                            { "fecha",fecha.ToString()},
                            { "estaciones", arrResultData }
                        };

                        arrResult.Add(objData);
                    }

                    objResponse.StatusCode = 200;
                    objResponse.Resp = true;
                    objResponse.Msg = "Información con éxito";
                    objResponse.Detail = "";
                    objResponse.Type = "Success";
                    objResponse.Data = arrResult.ToString();

                }
                catch (Exception ex)
                {
                    objResponse.StatusCode = 400;
                    objResponse.Resp = false;
                    objResponse.Msg = ex.Message;
                    objResponse.Detail = ex.Message;
                    objResponse.Type = "Error";
                }
            }
            else
            {
                objResponse.StatusCode = 400;
                objResponse.Resp = false;
                objResponse.Msg = "No tiene las credenciales correctas";
                objResponse.Detail = "No tiene las credenciales correctas";
                objResponse.Type = "Error";
            }

            return objResponse;
        }

        #endregion
    }

    public class Reporte
    {
        public string? TotalCantidad { get; set; }
        public string? TotalValor { get; set; }
    }
}
