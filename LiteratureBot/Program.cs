using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteratureBot.classes;
using System.Data.SqlClient;
using LiteratureBot.database;

namespace LiteratureBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Pullenti.Sdk.InitializeAll();
            Bot bot = new Bot();
            LiteratureBotDataSet.connectionString = args[0].Replace("\\\\", "\\");
            Bot.token = args[1];
            bot.StartBot(); // Запуск бота
            while (true) { }
        }
    }
}
