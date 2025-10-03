namespace Com.Gosol.INOUT.Security
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Configuration;

    public class DatabaseProxy
    {
        public static string _connectionString = "";
            //ConfigurationSettings.AppSettings["BackendDB"];
        private static string canNotCreateClass = "CanNotCreateClass";
        private static string databaseException = "DatabaseException";
        private static string noCommandText = "NoCommandText";
        private static string noConnectionString = "NoConnectionString";
        private static string noParamName = "NoParameterName";

        protected DatabaseProxy()
        {
            throw new DatabaseProxyException("cannot create class");
        }

        public static IDbDataParameter AddParameter(IDbCommand dbCommand, string paramName, int paramValue)
        {
            if ((paramName == null) || (paramName.Length <= 0))
            {
                throw new DatabaseProxyException("no parameter name");
            }
            IDbDataParameter parameter = dbCommand.CreateParameter();
            parameter.ParameterName = paramName;
            parameter.Value = paramValue;
            dbCommand.Parameters.Add(parameter);
            return parameter;
        }

        public static IDbDataParameter AddParameter(IDbCommand dbCommand, string paramName, string paramValue)
        {
            IDbDataParameter parameter = dbCommand.CreateParameter();
            parameter.ParameterName = paramName;
            parameter.Value = paramValue;
            dbCommand.Parameters.Add(parameter);
            return parameter;
        }

        public static IDbCommand CreateDBCommand(IDbConnection dbConnection, string commandText)
        {
            if ((commandText == null) || (commandText.Length <= 0))
            {
                throw new DatabaseProxyException("No Command Text");
            }
            IDbCommand command = dbConnection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = CommandType.Text;
            return command;
        }

        public static IDbConnection CreateDBConnection()
        {
            if ((_connectionString == null) || (_connectionString.Length <= 0))
            {
                throw new DatabaseProxyException("Cannot create connection");
            }
            return new SqlConnection(_connectionString);
        }
    }
}
