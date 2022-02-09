namespace LiteratureBot.classes
{
    // Класс для реализации многоступенчатого диалога
    public class Thread
    {
        public Command command; // Экземпляр класса команд
        public int step; // Шаг в диалоге
        public long? id; // id пользователя
        public object parametr; // Параметр
        //public Thread thread; 
        //public long? other_id;
        private int lives; // Количество доступных ошибок в команде

        // Конструктор
        public Thread(Command _command, long? _id)
        {
            command = _command;
            id = _id;
            step = 0;
            parametr = new object();
            //thread = null;
            //other_id = 0;
            lives = 3;
        }

        // Изменение количества ошибок
        public bool HasLives()
        {
            lives--;
            if (lives <= 0) return false;
            else return true;
        }

        // Восстановление количества доступных ошибок
        public void RestoreLives()
        {
            lives = 3;
        }
    }
}
