using JewellPro;
using Npgsql;
using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace CommonLayer
{
    public static class ConnectionCreator
    {
        static string connStr = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;
        #region Helper Methods

        /// <summary>
        /// creates and open a sqlconnection
        /// </summary>
        /// <param name="connectionString">
        /// A <see cref="System.String"/> that contains the sql connectin parameters
        /// </param>
        /// <returns>
        /// A <see cref="SqlConnection"/> 
        /// </returns>
        public static OleDbConnection GetConnection()
        {
            OleDbConnection connection = null;
            try
            {
                connection = new OleDbConnection(connStr);
                connection.Open();
            }
            catch (Exception)
            {
                if (connection != null)
                {
                    connection.Dispose();
                }
            }
            return connection;
        }

        /// <summary>
        /// Creates a sqlcommand
        /// </summary>
        /// <param name="connection">
        /// A <see cref="SqlConnection"/>
        /// </param>
        /// <param name="commandText">
        /// A <see cref="System.String"/> of the sql query.
        /// </param>
        /// <param name="commandType">
        /// A <see cref="CommandType"/> of the query type.
        /// </param>
        /// <returns>
        /// A <see cref="SqlCommand"/>
        /// </returns>
        public static SqlCommand GetCommand(this SqlConnection connection, string commandText, CommandType commandType)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandTimeout = connection.ConnectionTimeout;
            command.CommandType = commandType;
            command.CommandText = commandText;
            return command;
        }

        #endregion
    }

    public static class DBWrapper
    {
        #region Helper Methods
        static string connStrNpgSql = ConfigurationManager.ConnectionStrings["dbConnection"].ConnectionString;

        public static SqlConnection GetConnection(string connectionString)
        {
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                if (connection != null)
                {
                    connection.Dispose();
                }
                Logger.LogError(ex);
            }
            return connection;
        }


        public static SqlCommand GetCommand(this SqlConnection connection, string commandText, CommandType commandType)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandTimeout = connection.ConnectionTimeout;
            command.CommandType = commandType;
            command.CommandText = commandText;
            return command;
        }


        public static NpgsqlConnection GetNpgsqlConnection()
        {
            NpgsqlConnection connection = null;
            try
            {
                connection = new NpgsqlConnection(connStrNpgSql);
                connection.Open();
            }
            catch (Exception ex)
            {
                if (connection != null)
                {
                    connection.Dispose();
                }
                Logger.LogError(ex);
            }
            return connection;
        }

        public static NpgsqlCommand GetNpgsqlCommand(this NpgsqlConnection connection, string commandText, CommandType commandType)
        {
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = commandText;
            return command;
        }


        #endregion
    }
}
