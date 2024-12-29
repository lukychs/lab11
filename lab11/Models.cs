using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    // Представляет тикер акций
    public class Ticker
    {
        public int id { get; set; } // Первичный ключ таблицы Ticker

        [Column("ticker")]
        public string tickerSym { get; set; } // Символ тикера (например, "AAPL" для Apple)

        public List<Price> prices { get; set; } // Список цен, связанных с этим тикером
    }

    // Представляет цену акции в определенный момент времени
    public class Price
    {
        public int id { get; set; } // Первичный ключ таблицы Price

        public int tickerId { get; set; } // Внешний ключ, связывающий с таблицей Ticker

        public Ticker tickerSymPrices { get; set; } // Навигационное свойство для связанного тикера

        public double price { get; set; } // Значение цены акции

        public DateTime date { get; set; } // Дата и время, когда была записана цена
    }

    // Представляет состояние акции за день (например, "Вверх" или "Вниз")
    public class TodaysCondition
    {
        public int id { get; set; } // Первичный ключ таблицы TodaysCondition

        public int tickerId { get; set; } // Внешний ключ, связывающий с таблицей Ticker

        public string state { get; set; } // Состояние акции (например, "Вверх", "Вниз")

        public Ticker tickerSymConditions { get; set; } // Навигационное свойство для связанного тикера
    }
}