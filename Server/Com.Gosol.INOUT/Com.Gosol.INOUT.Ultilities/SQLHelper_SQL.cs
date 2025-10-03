using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Com.Gosol.KKTS.Utility
{

    /// <summary>
    /// The SqlHelper class is intended to encapsulate high performance, 
    /// scalable best practices for common uses of SqlClient.
    /// </summary>
    public class SQLHelper
    {
        public static readonly string CONN_BACKEND = ConfigurationSettings.AppSettings["appConnectionStrings"];
        public static readonly string CONN_MERGE_BACKEND = ConfigurationSettings.AppSettings["MergeBackendDB"];

        // Hashtable to store cached parameters
        public static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {

            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">an existing database connection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection conn, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {

            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset) using an existing SQL Transaction 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="trans">an existing sql transaction</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>an int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }
        public static int ExecuteNonQueryNonClearParams(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            int val = cmd.ExecuteNonQuery();
            //cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Execute a SqlCommand return object, Create by Namts@vega.com.vn
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static object ExecuteScalar(SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// Execute a SqlCommand that returns a resultset against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  SqlDataReader r = ExecuteReader(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>A SqlDataReader containing the results</returns>
        /// 
        public static SqlDataReader ExecuteReader(string connString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against the database specified in the connection string 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connectionString">a valid connection string for a SqlConnection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(string connString, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// Execute a SqlCommand that returns the first column of the first record against an existing database connection 
        /// using the provided parameters.
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  Object obj = ExecuteScalar(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="conn">an existing database connection</param>
        /// <param name="commandType">the CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">the stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">an array of SqlParamters used to execute the command</param>
        /// <returns>An object that should be converted to the expected type using Convert.To{Type}</returns>
        public static object ExecuteScalar(SqlConnection conn, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {

            SqlCommand cmd = new SqlCommand();

            PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// add parameter array to the cache
        /// </summary>
        /// <param name="cacheKey">Key to the parameter cache</param>
        /// <param name="cmdParms">an array of SqlParamters to be cached</param>
        public static void CacheParameters(string cacheKey, params SqlParameter[] cmdParms)
        {
            parmCache[cacheKey] = cmdParms;
        }

        /// <summary>
        /// Retrieve cached parameters
        /// </summary>
        /// <param name="cacheKey">key used to lookup parameters</param>
        /// <returns>Cached SqlParamters array</returns>
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] cachedParms = (SqlParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            SqlParameter[] clonedParms = new SqlParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (SqlParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">SqlCommand object</param>
        /// <param name="conn">SqlConnection object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
            {
                cmd.Transaction = trans;
            }

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }

        //------------------------------------------------------------------------------
        //Modify
        //------------------------------------------------------------------------------

        /// <summary>
        ///  
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(string value)
        {
            if (value != null)
                return (value.Trim().Length == 0);

            return true;
        }
        /// <summary>
        ///  
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(string[] values)
        {
            if (values != null)
                return (values.Length == 0);

            return true;
        }
        /// <summary>
        ///  
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string IfNotNullOrEmpty(string oldValue, string newValue)
        {
            return IsNullOrEmpty(oldValue) ? string.Empty : newValue;
        }
        /// <summary>
        ///  
        /// </summary>
        /// <param name="oldValues"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string IfNotNullOrEmpty(string[] oldValues, string newValue)
        {
            return IsNullOrEmpty(oldValues) ? string.Empty : newValue;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="criterias"></param>
        /// <param name="priorities"></param>
        /// <returns></returns>
        public static string CreateQueryString(string criterias, string priorities)
        {

            string queryString = string.Empty;
            queryString += IfNotNullOrEmpty(criterias, "WHERE " + criterias);
            queryString += IfNotNullOrEmpty(priorities, "ORDER BY " + priorities);
            if (queryString.IndexOf(";") > -1) return string.Empty;
            return queryString;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="priorities"></param>
        /// <returns></returns>
        public static string CreateSearchQueryString(string keyword, string priorities)
        {
            string queryString = string.Empty;
            queryString += IfNotNullOrEmpty(keyword, String.Format("WHERE CONTAINS(*,'\"{0}\"')", keyword));
            queryString += IfNotNullOrEmpty(priorities, "ORDER BY " + priorities);
            if (queryString.IndexOf(";") > -1) return string.Empty;
            return queryString;
        }
        /// <summary>
        ///  
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="criterias"></param>
        /// <param name="priorities"></param>
        /// <returns></returns>
        public static string CreateSearchQueryString(string keyword, string criterias, string priorities)
        {
            string queryString = string.Empty;
            queryString += IfNotNullOrEmpty(keyword, String.Format("WHERE CONTAINS(*,'\"{0}\"')", keyword));
            if (IsNullOrEmpty(queryString))
            {
                queryString += IfNotNullOrEmpty(criterias, " WHERE " + criterias);
            }
            else
            {
                queryString += IfNotNullOrEmpty(criterias, " AND " + criterias);
            }
            queryString += IfNotNullOrEmpty(priorities, "ORDER BY " + priorities);
            if (queryString.IndexOf(";") > -1) return string.Empty;
            return queryString;
        }

        //public static SqlParameter[] GetParameters_QueryBuilder(IParameterizedQuery queryParameters, Hashtable dateColumns)
        //{

        //    ArrayList parms = new ArrayList();

        //    int parmsCount = 0;

        //    int secondValue = 0;

        //    IEnumerator parmsEnumerator = queryParameters.GetParameterizedTables();

        //    if (parmsEnumerator != null)
        //    {

        //        while (parmsEnumerator.MoveNext())
        //        {

        //            secondValue = 0;

        //            if (((IParameterizedTable)parmsEnumerator.Current).ParameterSecondValue != null)
        //            {

        //                secondValue = 1;

        //            }

        //            for (int count = 0; count <= secondValue; count++)
        //            {

        //                if (dateColumns.ContainsKey((object)((IParameterizedTable)parmsEnumerator.Current).ParameterName))
        //                {

        //                    parms.Add(new SqlParameter((secondValue == 1) ? ((IParameterizedTable)parmsEnumerator.Current).ParameterName + count.ToString() : ((IParameterizedTable)parmsEnumerator.Current).ParameterName, (secondValue == 1 && count == 1) ? DateTime.Parse((string)(((IParameterizedTable)parmsEnumerator.Current).ParameterSecondValue)) : DateTime.Parse((string)(((IParameterizedTable)parmsEnumerator.Current).ParameterValue))));

        //                }

        //                else
        //                {

        //                    parms.Add(new SqlParameter((secondValue == 1) ? ((IParameterizedTable)parmsEnumerator.Current).ParameterName + count.ToString() : ((IParameterizedTable)parmsEnumerator.Current).ParameterName, (secondValue == 1 && count == 1) ? ((IParameterizedTable)parmsEnumerator.Current).ParameterSecondValue : ((IParameterizedTable)parmsEnumerator.Current).ParameterValue));

        //                }

        //            }

        //            parmsCount++;

        //        }

        //    }

        //    return (SqlParameter[])parms.ToArray(typeof(SqlParameter));

        //}

    }
}