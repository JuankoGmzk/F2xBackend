using System;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace BackendRestF2x
{
    public class Connection
    {
        private string strConnection;

        public Connection(string strSQLPass, string strServer, string strDBName, string strSQLUser)
        {
            try
            {
                SqlConnectionStringBuilder bldr = new SqlConnectionStringBuilder(strConnection)
                {
                    IntegratedSecurity = false,
                    DataSource = strServer,
                    InitialCatalog = strDBName,
                    UserID = strSQLUser,
                    Password = strSQLPass
                };

                strConnection = bldr.ConnectionString;
            }
            catch (Exception)
            {
            }
        }

        public ResponseDB InsData(string strQuery, string[] arrayParam, string[] arrayValue)
        {
            ResponseDB objResponseDB = new ResponseDB
            {
                Resp = true
            };

            SqlConnection objSQLConnection;

            try
            {
                objSQLConnection = new SqlConnection(strConnection);

                try
                {
                    objSQLConnection.Open();

                    if (objSQLConnection.State != System.Data.ConnectionState.Open)
                    {
                        objResponseDB.Resp = false;
                        objResponseDB.Msg = "The connection status is: " + objSQLConnection.State.ToString();

                        objSQLConnection.Close();
                    }
                    else
                    {

                        try
                        {
                            SqlCommand cmd = new SqlCommand(strQuery, objSQLConnection);

                            cmd.CommandText = strQuery;

                            for (int j = 0; j < arrayParam.Length; j++)
                            {
                                if (arrayValue[j] != null)
                                {
                                    cmd.Parameters.Add(new SqlParameter(arrayParam[j], arrayValue[j]));
                                }
                                else
                                {
                                    cmd.Parameters.Add(new SqlParameter(arrayParam[j], DBNull.Value));
                                }
                            }

                            Guid guidPK = (Guid)cmd.ExecuteScalar();

                            if (guidPK == null)
                            {
                                objResponseDB.Resp = false;
                            }
                            else
                            {
                                objResponseDB.guidResult = guidPK;
                            }

                            objSQLConnection.Close();

                        }
                        catch (Exception innerEx)
                        {
                            objResponseDB.Resp = false;
                            objResponseDB.Msg = innerEx.Message;
                            objResponseDB.Error++;
                        }
                    }

                }
                catch (SqlException ex)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Error opening the connection:: " + ex.Message;
                }
                catch (InvalidOperationException ix)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Invalid Operation error: " + ix.Message;
                }
            }
            catch (ArgumentException ax) 
            {
                objResponseDB.Resp = false;
                objResponseDB.Msg = "Error creating the connection: " + ax.Message;
            }

            return objResponseDB;
        }

        public ResponseDB getRespFromQuery(string strQueryCols, string strQuery, string strOrderByCol, string strRespType)
        {
            ResponseDB objResponseDB = new ResponseDB
            {
                Resp = true
            };

            SqlConnection objSQLConnection;

            try
            {
                objSQLConnection = new SqlConnection(strConnection);

                try
                {
                    objSQLConnection.Open();

                    if (objSQLConnection.State != System.Data.ConnectionState.Open)
                    {
                        objResponseDB.Resp = false;
                        objResponseDB.Msg = "The connection status is: " + objSQLConnection.State.ToString();

                        objSQLConnection.Close();
                    }
                    else
                    {
                        SqlDataReader rdr = null;

                        try
                        {
                            bool blCount = true;

                            if (!strOrderByCol.Equals("NO_PAGINATE"))
                            {
                                #region QueryCount

                                string strCountQuery = "SELECT COUNT(*) AS ROWS " + strQuery;
                                SqlCommand cmdCount = new SqlCommand(strCountQuery, objSQLConnection);

                                DataSet dsInfoCount = new DataSet();
                                SqlDataAdapter adapterCount = new SqlDataAdapter();

                                cmdCount.CommandText = strCountQuery;

                               

                                adapterCount.SelectCommand = cmdCount;
                                adapterCount.Fill(dsInfoCount);

                                if (dsInfoCount.Tables.Count > 0 && dsInfoCount.Tables[0].Rows.Count > 0)
                                {
                                    objResponseDB.Count = int.Parse(dsInfoCount.Tables[0].Rows[0][0].ToString());
                                }
                                else
                                {
                                    blCount = false;
                                }

                                #endregion

                                strQuery = strQueryCols + " " + strQuery;
                            }
                            else
                            {
                                strQuery = strQueryCols + " " + strQuery;
                            }

                            if (blCount)
                            {

                                #region QueryData
                                SqlCommand cmd = new SqlCommand(strQuery, objSQLConnection);

                                DataSet dsInfo = new DataSet();
                                SqlDataAdapter adapter = new SqlDataAdapter();

                                cmd.CommandText = strQuery;


                                adapter.SelectCommand = cmd;
                                adapter.Fill(dsInfo);

                                if (dsInfo.Tables.Count > 0 && dsInfo.Tables[0].Rows.Count > 0)
                                {
                                    switch (strRespType)
                                    {
                                        case "DataTable":
                                            objResponseDB.dtResult = dsInfo.Tables[0];
                                            break;
                                        case "Array":

                                            string[][] stringRepresentation = dsInfo.Tables[0].Rows
                                            .OfType<DataRow>()
                                            .Select(r => dsInfo.Tables[0].Columns
                                                .OfType<DataColumn>()
                                                .Select(c => r[c.ColumnName].ToString())
                                                .ToArray())
                                            .ToArray();

                                            objResponseDB.jsonData = JsonConvert.SerializeObject(stringRepresentation, Formatting.None);
                                            break;
                                    }

                                    if (strOrderByCol.Equals("NO_PAGINATE"))
                                    {
                                        objResponseDB.Count = dsInfo.Tables[0].Rows.Count;
                                    }
                                }
                                else
                                {
                                    objResponseDB.Count = 0;
                                }

                                #endregion QueryData
                            }

                            objSQLConnection.Close();
                        }
                        catch (SqlException ex)
                        {
                            // close the reader
                            if (rdr != null)
                            {
                                rdr.Close();
                            }

                            // 5. Close the connection
                            if (objSQLConnection != null)
                            {
                                objSQLConnection.Close();
                            }

                            objResponseDB.Resp = false;
                            objResponseDB.Msg = ex.Message;
                        }

                    }

                }
                // Catch errors specific to the Open method
                catch (SqlException ex)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Error al tratar de conectarse a la base de datos:: " + ex.Message;
                }
                catch (InvalidOperationException ix)
                {
                    objResponseDB.Resp = false;
                    objResponseDB.Msg = "Operación no válida en la base de datos:" + ix.Message;
                }
            }
            catch (ArgumentException ax)  // there was something wrong in the connection string.
            {
                objResponseDB.Resp = false;
                objResponseDB.Msg = "Error al crear la conexión a la base de datos: " + ax.Message;
            }

            return objResponseDB;
        }

    }
}
