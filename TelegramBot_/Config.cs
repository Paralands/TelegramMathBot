using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBot_
{
    public static class Config
    {
        //Bot token
        public const string TOKEN = "6313132236:AAEejFxAhDnL1ft5PLBjyJuNtEckgpO9ezc";

        //Path where all the info will be saved
        public const string PATH = "D:\\TelegramBot";

        public const string START_TEXT = "Привет! \n" +
                                  "Этот бот - работа @paralands для его резюме \n" +
                                  "<b>Основные функции:</b> \n" +
                                  "<b>Math</b> - подробнее читайте в /mathInfo \n" +
                                  "<b>Парсинг</b> - все данные, отправленные сюда, сохраняются в папку юзера \n\n" + 
                                  "Чтоб открыть это меню - введите /help ❤️";

        public const string MATH_TEXT = "<b>Основные функции:</b> \n" +
                                  "🔸 Сложение (a+b), вычитание (a-b), умножение (a*b), деление (a/b), факториал (a!), степень (a^b) \n" +
                                  "🔸 Порядок арифметических дейстив, включая скобки  \n" +
                                  "🔸 Чтоб писать нецелое число, достаточно писать '.1', не обязательно '0,1'\n" +
                                  "🔸 Чтоб умножать на скобку не обязательно писать знак умножить: 2*(2+2) = 2(2+2) \n" +
                                  "🔸 Если перед знаком факториала будет написано нецелое или отрицательное число, факториал проигнорируется \n\n" +
                                  "<b>Чтоб что-либо посчитать, нужно написать ключевое слово 'math' и ваш пример</b>)";

        public const string MATH_NO_EXPRESSION_TEXT = "Сообщение не содержит математический пример, который бы можно было решить! Пожалуйста, проверьте условие!";
    }
}
