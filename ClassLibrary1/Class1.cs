using Microsoft.EntityFrameworkCore; // Подключение EF Core для работы с базой данных.
using System.Collections.Generic; // Пространство имен для работы с коллекциями.
using System.ComponentModel.DataAnnotations; // Пространство имен для аннотаций данных.

namespace ClassLibrary1
{
    // Контекст базы данных, определяющий подключение и наборы данных.
    public class AppDbContext : DbContext
    {
        // Конструктор, принимающий параметры конфигурации базы данных.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Свойство для работы с таблицей Regions в базе данных.
        public DbSet<Regions> Regions { get; set; }
    }
}

// Класс, представляющий модель данных для таблицы Regions.
public class Regions
{
    [Key] // Атрибут, указывающий, что это свойство является первичным ключом.
    public int RegionID { get; set; } // Идентификатор региона.

    public string RegionDescription { get; set; } // Описание региона.
}