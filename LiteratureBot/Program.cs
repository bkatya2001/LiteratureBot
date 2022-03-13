using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteratureBot.classes;
using System.Data.SqlClient;

namespace LiteratureBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Pullenti.Sdk.InitializeAll();
            Bot bot = new Bot();
            bot.StartBot(); // Запуск бота
            while (true) { }
        }
    }
}
