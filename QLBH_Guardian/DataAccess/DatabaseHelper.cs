using Microsoft.Data.SqlClient;
using System.Data;

namespace QLBH_Guardian.DataAccess
{
    public static class DatabaseHelper
    {
        private static string _connectionString =
            @"Server=.\SQLServer16.0.1000-ADMIN-PC\Admin;Database=localhost;Integrated Security=True;TrustServerCertificate=True;";

        public static string ConnectionString
        {
            get => _connectionString;
            set => _connectionString = value;
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public static bool TestConnection()
        {
            try
            {
                using var conn = GetConnection();
                conn.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static DataTable ExecuteQuery(string sql, Dictionary<string, object>? parameters = null)
        {
            var dt = new DataTable();
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null)
                foreach (var p in parameters)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }

        public static int ExecuteNonQuery(string sql, Dictionary<string, object>? parameters = null)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null)
                foreach (var p in parameters)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
            return cmd.ExecuteNonQuery();
        }

        public static object? ExecuteScalar(string sql, Dictionary<string, object>? parameters = null)
        {
            using var conn = GetConnection();
            conn.Open();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null)
                foreach (var p in parameters)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
            var result = cmd.ExecuteScalar();
            return result == DBNull.Value ? null : result;
        }
        public static bool CheckLogin(string username, string password)
        {
            string sql = "SELECT COUNT(*) FROM Users WHERE Username=@u AND Password=@p";

            var parameters = new Dictionary<string, object>
 {
   { "@u", username },
   { "@p", password }
 };

            var result = ExecuteScalar(sql, parameters);

            if (result == null) return false;

            return Convert.ToInt32(result) > 0;
        }
        public static DataTable GetDonHang()
        {
            using var conn = GetConnection();
            string query = "SELECT * FROM DonHang";

            using var adapter = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }
    }
}