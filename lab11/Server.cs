using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Web;
using Functions;

namespace Server
{
    // Асинхронный сервер, который принимает запросы от клиентов и отвечает с использованием данных о рынке
    public class AsyncEchoServer
    {
        private readonly int _listeningPort; // Порт, на котором сервер слушает подключения
        private readonly MarketFunctions _marketFunctions; // Экземпляр класса MarketFunctions для работы с данными о рынке

        // Конструктор, принимает порт и экземпляр MarketFunctions
        public AsyncEchoServer(int port, MarketFunctions marketFunctions)
        {
            _listeningPort = port;
            _marketFunctions = marketFunctions;
        }

        // Метод запуска сервера
        public async void Start()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); // IP-адрес для локального сервера PORT: 1112 ticker для примера: AACG
            TcpListener listener = new TcpListener(ipAddress, _listeningPort); // Создаем TCP-сервер
            listener.Start(); // Запускаем сервер
            Console.WriteLine("Server is running on port " + _listeningPort); // Логируем запуск сервера

            // Бесконечный цикл для обработки подключений клиентов
            while (true)
            {
                try
                {
                    var tcpClient = await listener.AcceptTcpClientAsync(); // Ожидаем подключения клиента
                    HandleConnectionAsync(tcpClient); // Обрабатываем подключение клиента
                }
                catch (Exception exp)
                {
                    Console.WriteLine(exp.ToString()); // Логируем ошибки
                }
            }
        }

        // Метод для обработки подключения клиента
        private async void HandleConnectionAsync(TcpClient tcpClient)
        {
            string clientInfo = tcpClient.Client.RemoteEndPoint.ToString(); // Информация о подключении клиента
            Console.WriteLine("Client connected: " + clientInfo); // Логируем подключение клиента

            try
            {
                // Работа с сетевым потоком
                using (var networkStream = tcpClient.GetStream())
                using (var reader = new StreamReader(networkStream, Encoding.UTF8))
                using (var writer = new StreamWriter(networkStream, Encoding.UTF8) { AutoFlush = true }) // Автоматическая отправка данных
                {
                    // Цикл чтения запросов от клиента
                    while (true)
                    {
                        string ticker = await reader.ReadLineAsync(); // Читаем тикер, отправленный клиентом
                        if (string.IsNullOrEmpty(ticker)) // Если тикер пустой, клиент отключился
                        {
                            Console.WriteLine("Client disconnected: " + clientInfo); // Логируем отключение клиента
                            break;
                        }

                        Console.WriteLine($"Received ticker from {clientInfo}: {ticker}"); // Логируем полученный тикер

                        // Определяем даты для запроса данных
                        string startDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                        string endDate = DateTime.Now.ToString("yyyy-MM-dd");

                        // Загружаем и сохраняем данные о тикере
                        await _marketFunctions.GetDataAndSaveAsync(ticker, startDate, endDate);

                        // Получаем последнюю цену тикера
                        string response = await _marketFunctions.GetStockPriceAsync(ticker);

                        // Отправляем ответ клиенту
                        await writer.WriteLineAsync(response);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error handling client " + clientInfo + ": " + ex.Message); // Логируем ошибки при работе с клиентом
            }
            finally
            {
                tcpClient.Close(); // Закрываем подключение
            }
        }
    }
}