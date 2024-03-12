using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Npgsql.Internal;

namespace stroymodern
{
    internal class Database
    {
        NpgsqlConnection conn = new NpgsqlConnection("Server = localhost; Port = 5432; Database = stroyModern; User Id = postgres; Password = assaq123;");
        public void OpenConnection()
        {
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        public void CloseConnection()
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }

        public NpgsqlConnection GetConnection()
        {
            return conn;
        }
    }
}
