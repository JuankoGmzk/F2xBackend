namespace BackendRestF2x.Controllers
{
    public class Response
    {

        #region Propiedades

        /// <summary>
        /// Identificador de la respuesta.
        /// </summary>
        public bool Resp
        {
            get;
            set;
        }

        /// <summary>
        /// Contenido del mensaje.
        /// </summary>
        public string Msg
        {
            get;
            set;
        }

        /// <summary>
        /// Tipo del mensaje.
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Json Data Resp
        /// </summary>
        public string Data
        {
            get;
            set;
        }

        /// <summary>
        /// Detalle del error.
        /// </summary>
        public string Detail
        {
            get;
            set;
        }

        /// <summary>
        /// Row Count
        /// </summary>
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// Estado respuesta del servicio
        /// </summary>
        public int StatusCode
        {
            get;
            set;
        }
        #endregion
    }

}
