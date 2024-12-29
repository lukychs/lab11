using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace marketContext
{
    // Определяем класс контекста базы данных, наследующийся от DbContext.
    // Он используется для взаимодействия с базой данных через Entity Framework.
    public class MarketContext : DbContext
    {
        // Определяем таблицу Tickers, которая будет отображена на соответствующую сущность Ticker.
        public DbSet<Ticker> Tickers => Set<Ticker>();

        // Определяем таблицу Prices, которая будет отображена на соответствующую сущность Price.
        public DbSet<Price> Prices => Set<Price>();

        // Определяем таблицу todaysConditions, которая будет отображена на соответствующую сущность TodaysCondition.
        public DbSet<TodaysCondition> todaysConditions => Set<TodaysCondition>();

        // Конструктор класса. Обеспечивает создание базы данных, если она еще не существует.
        public MarketContext() => Database.EnsureCreated();

        // Переопределяем метод OnConfiguring для настройки параметров подключения к базе данных.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Устанавливаем строку подключения к SQL Server.
            // В данном случае используется сервер localhost и база данных MarketDB
            // с заданным пользователем и паролем.
            optionsBuilder.UseSqlServer("Server=localhost;Database=MarketDB;User Id=sa;Password=Password123!;TrustServerCertificate=True;");
        }
    }
}
