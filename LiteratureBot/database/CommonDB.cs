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
        private string connectionString = "Data Source=LAPTOP-K1DGUPRH\\SQLEXPRESS;Initial Catalog=LiteratureBot;Integrated Security=True";

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
                            result.AppendLine("Данные за " + r[4].ToString());
                            result.AppendLine("Количество запросов - " + r[1].ToString());
                            result.AppendLine("Количество новых пользователей - " + r[2].ToString());
                            result.AppendLine("Оценка по мнению пользователей - " + r[3].ToString());
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
                            result.Add(r[1].ToString() + " - " + r[2].ToString() + " - " + r[3].ToString());
                        }
                    }
                    else result.Add("На текущий момент ни один запрос не был обработан");
                    conn.Close();
                }
            }
            return result;
        }
    }
}
