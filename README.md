# F2xBackend
Backend para f2x

Pasos para instalación y ejecución del proyecto. 

Descargar y abrir el proyecto con su editor de preferencia.

Nuggets a instalar o tener en cuenta:
1. entityframeworkcore\7.0.5
2. entityframeworkcore.tools\7.0.5
3. newtonsoft.json\13.0.3
4. swashbuckle.aspnetcore\6.5.0
5. swashbuckle.aspnetcore.swagger\6.5.0
6. system.data.sqlclient\4.8.5

**Para iniciar el swagger debe descomentar las lineas comentadas en el Program.cs (esto debido a que se debió hacer una configuración adicional para poder resolver el inconveniente común del CORS).

Url's para acceder a la api de conteo con sus explicaciones:

Conteo {

[HttpGet("LlenadoConteo")] -- Esté se encarga de poblar la base de datos con los datos encontrados día a día. (está desde 0, tarda aprox 8-10 min en llenarse por completo)

[HttpGet("VehiculosConteoPorFecha")] -- Esté se encarga de realizar una única petición GET con su parametro de fecha.

[HttpGet("AllVehiculosConteoDB")] -- Esté se encarga de devolver toda la información de la base de datos respecto a la tabla de conteo. (se recomienda primero ejecutar "Llenado de conteo").

}
Recaudo{

[HttpGet("LlenadoRecaudo")] -- Esté se encarga de poblar la base de datos con los datos encontrados día a día. (está desde 0, tarda aprox 8-10 min en llenarse por completo)

[HttpGet("VehiculosRecaudoPorFecha")] -- Esté se encarga de realizar una única petición GET con su parametro de fecha en la entidad de recaudo.

[HttpGet("AllVehiculosRecaudoDB")]  -- Esté se encarga de devolver toda la información de la base de datos respecto a la tabla de conteo. (se recomienda primero ejecutar "Llenado de recaudo").

}

Reporte{

[HttpGet("ReporteDB")] -- Está se encarga de la lógica para entregar el reporte al front. 

}



--------------------------------

En la carpeta Models encontrará ResponseDB que será el modeo de respuesta de la base de datos, además el config para las propiedades de ambiente.
En la carpeta Util encontrará la clase General, la cual será un método generico para peticiones get. Por otro lado, la clase Security para los llamados de token y seguridad de la aplicación.
