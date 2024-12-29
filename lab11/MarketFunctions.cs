using marketContext;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Functions
{
    // Класс, представляющий структуру данных для получения биржевой информации из API.
    public class StockData
    {
        public string s { get; set; } // Статус ответа API.
        public List<double> c { get; set; } // Цены закрытия.
        public List<double> h { get; set; } // Максимальные цены.
        public List<double> l { get; set; } // Минимальные цены.
        public List<double> o { get; set; } // Цены открытия.
        public List<int> t { get; set; } // Временные метки в формате UNIX.
        public List<int> v { get; set; } // Объемы торгов.
    };

    // Класс, содержащий функции для работы с биржевыми данными.
    public class MarketFunctions
    {
        // Приватное поле для работы с базой данных.
        private readonly MarketContext _db;

        // Конструктор, инициализирующий контекст базы данных.
        public MarketFunctions(MarketContext db)
        {
            _db = db;
        }

        // Метод для получения данных с API и их сохранения в базу данных.
        public async Task GetDataAndSaveAsync(string ticker, string startDate, string endDate)
        {
            // Ключ API для аутентификации.
            string apiKey = "YlJIQkZUMFM3V0c4VFphYmh1RzlJbDVjY2lKbFJJdEdMa2t6U0hYUHBTdz0";

            // Формируем URL для запроса данных по указанному тикеру и диапазону дат.
            string URL = $"https://api.marketdata.app/v1/stocks/candles/D/{ticker}/?from={startDate}&to={endDate}&token={apiKey}";

            // Создаем HTTP-клиент для выполнения запроса.
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(URL);

            // Проверяем статус ответа.
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Что-то пошло не так: {response.StatusCode}");
                return;
            }

            // Читаем JSON-ответ и десериализуем его в объект StockData.
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<StockData>(json);

            // Проверяем, что данные корректны.
            if (data == null || data.s == null || data.c == null || data.h == null || data.l == null || data.o == null || data.t == null || data.v == null)
            {
                Console.WriteLine($"Ошибка получения данных: недостаточно информации");
                return;
            }

            // Проверяем, существует ли тикер в базе данных.
            var dbTicker = _db.Tickers.FirstOrDefault(t => t.tickerSym == ticker);
            await _db.SaveChangesAsync();
            if (dbTicker == null)
            {
                // Если тикера нет, создаем новый.
                dbTicker = new Models.Ticker { tickerSym = ticker };
                _db.Tickers.Add(dbTicker);
                await _db.SaveChangesAsync();
            }

            // Сохраняем полученные данные о ценах в базу данных.
            for (int i = 0; i < data.c.Count; i++)
            {
                var price = data.c[i];
                var time = DateTimeOffset.FromUnixTimeSeconds(data.t[i]).DateTime;
                _db.Prices.Add(new Models.Price
                {
                    price = price,
                    date = time,
                    tickerId = dbTicker.id
                });
            }

            // Сохраняем изменения в базе данных.
            await _db.SaveChangesAsync();
        }

        // Метод для анализа данных и определения состояния рынка (например, "рост" или "падение").
        public async Task AnalyzeData()
        {
            // Получаем список всех тикеров из базы данных.
            var Tickers = _db.Tickers.ToList();
            foreach (var ticker in Tickers)
            {
                // Получаем список цен для текущего тикера, отсортированный по убыванию даты.
                var prices = _db.Prices
                    .Where(p => p.tickerSymPrices == ticker)
                    .OrderByDescending(p => p.date)
                    .ToList();

                // Если есть минимум две записи о ценах, проводим анализ.
                if (prices.Count >= 2)
                {
                    var latestPrice = prices[0]; // Последняя цена.
                    var previousPrice = prices.FirstOrDefault(p => p.price != latestPrice.price); // Предыдущая отличная цена.

                    if (previousPrice != null && latestPrice != null)
                    {
                        // Определяем состояние (рост или падение цены).
                        var condition = latestPrice.price > previousPrice.price
                            ? $"UP    Latest Price: {latestPrice.price}    Previous Price: {previousPrice.price}" :
                            $"Down    Latest Price: {latestPrice.price}    Previous Price: {previousPrice.price}";

                        // Сохраняем состояние в базу данных.
                        _db.todaysConditions.Add(new TodaysCondition
                        {
                            tickerId = ticker.id,
                            state = condition
                        });
                    }
                }
            }

            // Сохраняем изменения в базе данных.
            await _db.SaveChangesAsync();
        }

        // Метод для получения последней цены акций по тикеру.
        public async Task<string> GetStockPriceAsync(string tick)
        {
            try
            {
                // Ищем тикер в базе данных.
                var dbTicker = await _db.Tickers
                    .FirstOrDefaultAsync(t => t.tickerSym == tick);

                if (dbTicker == null)
                {
                    return $"Тикер '{tick}' не найден в базе данных.";
                }

                // Получаем последнюю цену по тикеру.
                var latestPrice = await _db.Prices
                    .Where(p => p.tickerId == dbTicker.id)
                    .OrderByDescending(p => p.date)
                    .FirstOrDefaultAsync();

                if (latestPrice == null)
                {
                    return $"Нет данных о ценах для тикера '{tick}'.";
                }

                // Возвращаем последнюю цену.
                return $"Последняя цена для {tick}: {latestPrice.price}.";
            }
            catch (Exception ex)
            {
                // Логируем ошибку и возвращаем сообщение об ошибке.
                Console.WriteLine("Ошибка базы данных: " + ex.Message);
                return "Ошибка при получении данных. Попробуйте позже.";
            }
        }
    }
}