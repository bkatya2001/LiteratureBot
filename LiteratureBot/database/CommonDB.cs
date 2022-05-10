using LiteratureBot.classes;
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
        public static string connectionString;

        // Получение статистических данных
        public string GetStatistics(string date)
        {
            StringBuilder result = new StringBuilder();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    comm.CommandType = System.Data.CommandType.Text;
                    comm.CommandText = "SELECT * FROM Statistics_data WHERE date = @date;";
                    comm.Parameters.AddWithValue("@date", date);
                    conn.Open();
                    SqlDataReader r = comm.ExecuteReader();
                    if (r.HasRows)
                    {
                        while (r.Read())
                        {
                            result.AppendLine("Данные за " + r[4].ToString().Split(' ')[0]);
                            result.AppendLine("Количество запросов на подбор - " + r[1].ToString());
                            result.AppendLine("Количество новых пользователей - " + r[2].ToString());
                            result.AppendLine("Оценка по мнению пользователей - " + (Convert.ToInt32(r[3]) / Convert.ToDouble(r[5])).ToString());
                        }
                    }
                    else result.Append("Данные на указанную дату не найдены");
                    conn.Close();
                }
            }
            return result.ToString();
        }

        // Получение истории запросов
        public List<string> GetRequestsHistory()
        {
            List<string> result = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    comm.CommandType = System.Data.CommandType.Text;
                    comm.CommandText = "SELECT * FROM Requests;";
                    conn.Open();
                    SqlDataReader r = comm.ExecuteReader();
                    if (r.HasRows)
                    {
                        while (r.Read())
                        {
                            result.Add(r[1].ToString() + ":\n" + r[2].ToString() + " Похожие:\n" + r[3].ToString());
                        }
                    }
                    else result.Add("На текущий момент ни один запрос не был обработан");
                    conn.Close();
                }
            }
            return result;
        }

        public void UpdateRequestsCount()
        {
            string date = DateTime.Now.ToShortDateString();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    comm.CommandType = System.Data.CommandType.Text;
                    comm.CommandText = "IF (SELECT COUNT(*) FROM Statistics_data WHERE date = @date) = 0 " +
                        "INSERT INTO Statistics_data VALUES(1, 0, 0, @date, 0) " +
                        "ELSE UPDATE Statistics_data SET request_count = request_count + 1 WHERE date = @date; ";
                    comm.Parameters.AddWithValue("@date", date);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public void UpdateRating(int rating)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    comm.CommandType = System.Data.CommandType.Text;
                    comm.CommandText = "IF (SELECT COUNT(*) FROM Statistics_data WHERE date = @date) = 0 " +
                        "INSERT INTO Statistics_data VALUES(0, 0, @rating, @date, 1) " +
                        "ELSE UPDATE Statistics_data SET rating = rating + @rating, rating_count = rating_count + 1 WHERE date = @date;";
                    comm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
                    comm.Parameters.AddWithValue("@rating", rating);
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public void AddRequest(List<List<Book>> books, string request, long? id_vk)
        {
            StringBuilder main_books = new StringBuilder();
            StringBuilder same_books = new StringBuilder();
            if (books[0] == null || books[0].Count == 0)
            {
                main_books.AppendLine("Ничего не найдено");
                if (books[1] == null || books[1].Count == 0)
                {
                    same_books.AppendLine("Ничего не найдено");
                }
                else
                {
                    for (int i = 0; i < books[1].Count; i++)
                    {
                        same_books.AppendLine((i + 1).ToString() + ") " + books[1][i].name + " - " + books[1][i].author);
                    }
                }
            }
            else
            {
                for (int i = 0; i < books[0].Count; i++)
                {
                    main_books.AppendLine((i + 1).ToString() + ") " + books[0][i].name + " - " + books[0][i].author);
                }
                if (books[1] == null || books[1].Count == 0)
                {
                    same_books.AppendLine("Ничего не найдено");
                }
                else
                {
                    for (int i = 0; i < books[1].Count; i++)
                    {
                        same_books.AppendLine((i + 1).ToString() + ") " + books[1][i].name + " - " + books[1][i].author);
                    }
                }
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    comm.CommandType = System.Data.CommandType.Text;
                    comm.CommandText = "INSERT INTO Requests VALUES (@request, @result, @similar);";
                    comm.Parameters.AddWithValue("@request", request);
                    comm.Parameters.AddWithValue("@result", main_books.ToString());
                    comm.Parameters.AddWithValue("@similar", same_books.ToString());
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                    comm.Parameters.Clear();
                    comm.CommandText = "INSERT INTO History VALUES " +
                        "((SELECT TOP 1 id FROM Users WHERE id_vk = @id_vk), " +
                        "(SELECT TOP 1 id FROM Requests WHERE request = @request), @date_time);";
                    comm.Parameters.AddWithValue("@request", request);
                    comm.Parameters.AddWithValue("@id_vk", id_vk);
                    comm.Parameters.AddWithValue("date_time", DateTime.Now.ToString());
                    conn.Open();
                    comm.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }
    }
}
