using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiteratureBot.database
{
    partial class LiteratureBotDataSet
    {
        // Проверка на наличие прав администратора
        public bool IsAdmin(long? id_vk)
        {
            bool result = false;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    comm.CommandType = System.Data.CommandType.Text;
                    comm.CommandText = "SELECT id FROM Users WHERE id_vk = @id_vk AND status = 1;";
                    comm.Parameters.AddWithValue("@id_vk", id_vk);
                    conn.Open();
                    SqlDataReader reader = comm.ExecuteReader();
                    if (reader.HasRows) result = true;
                    conn.Close();
                }
            }
            return result;
        }

        // Изменение прав администратора на пользовательские
        public void ChangeStatus(long? id_vk, int status)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    comm.CommandType = System.Data.CommandType.Text;
                    comm.CommandText = "UPDATE Users SET status = @status WHERE id_vk = @id_vk;";
                    comm.Parameters.AddWithValue("@id_vk", id_vk);
                    comm.Parameters.AddWithValue("@status", status);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        // Создание нового пользователя при первом обращении к системе
        public void CheckAndAddUser(long? id_vk)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    comm.CommandType = System.Data.CommandType.Text;
                    comm.CommandText = "IF (SELECT COUNT(*) FROM Users WHERE id_vk = @id_vk) = 0 INSERT INTO Users VALUES (@id_vk, 0);";
                    comm.Parameters.AddWithValue("@id_vk", id_vk);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public void DeleteRequestsHistoryByVkId(long? id_vk)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    comm.CommandType = System.Data.CommandType.Text;
                    comm.CommandText = "DELETE FROM History WHERE id_user IN (SELECT id FROM Users WHERE id_vk = @id_vk);";
                    comm.Parameters.AddWithValue("@id_vk", id_vk);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public List<string> GetUserRequestsHistory(long? id_vk)
        {
            List<string> result = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    comm.CommandType = System.Data.CommandType.Text;
                    comm.CommandText = "SELECT H.data_time, R.request, R.result, R.similar FROM History H, Requests R WHERE id_user IN " +
                        "(SELECT id FROM Users WHERE id_vk = @id_vk); ";
                    comm.Parameters.AddWithValue("@id_vk", id_vk);
                    conn.Open();
                    SqlDataReader r = comm.ExecuteReader();
                    if (r.HasRows)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        while (r.Read())
                        {
                            stringBuilder.AppendLine(r[0].ToString());
                            stringBuilder.AppendLine("Запрос: " + "'" + r[1].ToString() + "'");
                            stringBuilder.AppendLine("Результат: " + r[2].ToString());
                            stringBuilder.AppendLine("Похожие: " + r[3].ToString());
                            result.Add(stringBuilder.ToString());
                            stringBuilder.Clear();
                        }
                    }
                    conn.Close();
                }
            }
            return result;
        }

        public void AddRating(long? id_vk, int rating, string comment)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    comm.CommandType = System.Data.CommandType.Text;
                    comm.CommandText = "INSERT INTO Rating VALUES ((SELECT id FROM Users WHERE id_vk = @id_vk), @rating, @comment);";
                    comm.Parameters.AddWithValue("@id_vk", id_vk);
                    comm.Parameters.AddWithValue("@rating", rating);
                    comm.Parameters.AddWithValue("@comment", comment);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
    }
}
