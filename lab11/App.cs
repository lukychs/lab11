using Functions;
using marketContext;
using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app
{
    internal class App
    {
        static async Task Main(string[] args)
        {
            // Устанавливаем кодировку для вывода в консоль на UTF-8,
            // чтобы поддерживать корректное отображение символов.
            Console.OutputEncoding = Encoding.UTF8;

            // Создаем новый контекст базы данных для взаимодействия с ней.
            using (var db = new MarketContext())
            {
                // Инициализируем объект MarketFunctions, предоставляющий методы работы с данными.
                var marketFuncs = new MarketFunctions(db);

                // Задаем порт, на котором будет работать TCP-сервер.
                int port = 1112;

                // Создаем сервер с использованием асинхронного TCP-сервера и передаем порт
                // вместе с объектом функций работы с рынком.
                var server = new AsyncEchoServer(port, marketFuncs);

                // Запускаем сервер, чтобы начать прием и обработку входящих соединений.
                server.Start();

                // Уведомляем пользователя, что сервер запущен, и ждем нажатия Enter для завершения работы.
                Console.WriteLine("TCP server is running. Press Enter to shut down.");
                Console.ReadLine();
            }
        }
    }
}