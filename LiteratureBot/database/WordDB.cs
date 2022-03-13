using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteratureBot.classes;

namespace LiteratureBot.database
{
    partial class LiteratureBotDataSet
    {
        public List<int> GetWordsId(List<string> words)
        {
            List<int> result = new List<int>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    comm.CommandType = System.Data.CommandType.Text;
                    foreach (string word in words)
                    {
                        comm.CommandText = "SELECT id FROM Words WHERE CHARINDEX(@word, word) != 0;";
                        comm.Parameters.AddWithValue("@word", word);
                        conn.Open();
                        SqlDataReader r = comm.ExecuteReader();
                        if (r.HasRows)
                        {
                            while (r.Read())
                            {
                                result.Add(Convert.ToInt32(r[0]));
                                break;
                            }
                        }
                        conn.Close();
                        comm.Parameters.Clear();
                    }
                }
            }
            return result;
        }

        public Dictionary<int, int> GetWordsIdConnIs(List<int> wordsId)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            StringBuilder param_arr = new StringBuilder("(");
            foreach(int w in wordsId)
            {
                param_arr.Append(w.ToString() + ", ");
            }
            if (param_arr.Length > 2)
            {
                param_arr.Remove(param_arr.Length - 2, 2);
                param_arr.Append(")");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = conn.CreateCommand())
                    {
                        comm.CommandType = System.Data.CommandType.Text;
                        comm.CommandText = "SELECT id_book, COUNT(id_book) AS num FROM Ontology WHERE id_conn = 1 AND id_word_2 IN " + param_arr.ToString() + " GROUP BY id_book ORDER BY COUNT(id_book) DESC;";
                        conn.Open();
                        SqlDataReader r = comm.ExecuteReader();
                        if (r.HasRows)
                        {
                            while (r.Read())
                            {
                                result.Add(Convert.ToInt32(r[0]), Convert.ToInt32(r[1]));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public Dictionary<int, int> GetWordsIdConnDef(List<int> wordsId)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            StringBuilder param_arr = new StringBuilder("(");
            foreach (int w in wordsId)
            {
                param_arr.Append(w.ToString() + ", ");
            }
            if (param_arr.Length > 2)
            {
                param_arr.Remove(param_arr.Length - 2, 2);
                param_arr.Append(")");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = conn.CreateCommand())
                    {
                        comm.CommandType = System.Data.CommandType.Text;
                        comm.CommandText = "SELECT id_book, COUNT(id_book) AS num FROM Ontology WHERE id_conn = 3 AND id_word_2 IN " + param_arr.ToString() + " GROUP BY id_book ORDER BY COUNT(id_book) DESC;";
                        conn.Open();
                        SqlDataReader r = comm.ExecuteReader();
                        if (r.HasRows)
                        {
                            while (r.Read())
                            {
                                result.Add(Convert.ToInt32(r[0]), Convert.ToInt32(r[1]));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public Dictionary<int, int> GetWordsIdConnAdj(List<int> wordsId)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            StringBuilder param_arr = new StringBuilder("(");
            foreach (int w in wordsId)
            {
                param_arr.Append(w.ToString() + ", ");
            }
            if (param_arr.Length > 2)
            {
                param_arr.Remove(param_arr.Length - 2, 2);
                param_arr.Append(")");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = conn.CreateCommand())
                    {
                        comm.CommandType = System.Data.CommandType.Text;
                        comm.CommandText = "SELECT id_book, COUNT(id_book) AS num FROM Ontology WHERE id_conn = 2 AND id_word_1 IN " + param_arr.ToString() + " GROUP BY id_book ORDER BY COUNT(id_book) DESC;";
                        conn.Open();
                        SqlDataReader r = comm.ExecuteReader();
                        if (r.HasRows)
                        {
                            while (r.Read())
                            {
                                result.Add(Convert.ToInt32(r[0]), Convert.ToInt32(r[1]));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public Dictionary<int, int> GetWordsIdConnFor(List<int> wordsId)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            StringBuilder param_arr = new StringBuilder("(");
            foreach (int w in wordsId)
            {
                param_arr.Append(w.ToString() + ", ");
            }
            if (param_arr.Length > 2)
            {
                param_arr.Remove(param_arr.Length - 2, 2);
                param_arr.Append(")");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand comm = conn.CreateCommand())
                    {
                        comm.CommandType = System.Data.CommandType.Text;
                        comm.CommandText = "SELECT id_book, COUNT(id_book) AS num FROM Ontology WHERE id_conn = 6 AND id_word_2 IN " + param_arr.ToString() + " GROUP BY id_book ORDER BY COUNT(id_book) DESC;";
                        conn.Open();
                        SqlDataReader r = comm.ExecuteReader();
                        if (r.HasRows)
                        {
                            while (r.Read())
                            {
                                result.Add(Convert.ToInt32(r[0]), Convert.ToInt32(r[1]));
                            }
                        }
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public Book GetBook(int id)
        {
            string name = "";
            string author = "";
            byte[] photo = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand comm = conn.CreateCommand())
                {
                    comm.CommandType = System.Data.CommandType.Text;
                    comm.CommandText = "SELECT name, author, photo FROM Books WHERE id = @id;";
                    comm.Parameters.AddWithValue("@id", id);
                    conn.Open();
                    SqlDataReader r = comm.ExecuteReader();
                    if (r.HasRows)
                    {
                        while (r.Read())
                        {
                            name = Convert.ToString(r[0]);
                            author = Convert.ToString(r[1]);
                            photo = null;
                        }
                    }
                    conn.Close();
                }
            }
            return new Book(name, author, photo);
        }
    }
}
