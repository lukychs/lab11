using Microsoft.EntityFrameworkCore; // Подключение EF Core для работы с базой данных.
using ClassLibrary1; // Подключение пользовательской библиотеки, содержащей модели данных.
using System; // Подключение пространства имен для базовых системных классов.

var builder = WebApplication.CreateBuilder(args); // Создание и настройка приложения.

/// Добавление сервисов в контейнер зависимостей.

// Добавление контроллеров в приложение.
builder.Services.AddControllers();

// Настройка контекста базы данных с использованием SQLite и строки подключения из конфигурации.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавление API Explorer для создания конечных точек OpenAPI.
builder.Services.AddEndpointsApiExplorer();

// Добавление Swagger для документирования и тестирования API.
builder.Services.AddSwaggerGen();

var app = builder.Build(); // Построение приложения.

// Конфигурация конвейера обработки HTTP-запросов.

if (app.Environment.IsDevelopment()) // Проверка, выполняется ли приложение в режиме разработки.
{
    app.UseSwagger(); // Включение Swagger в режиме разработки.
    app.UseSwaggerUI(); // Включение пользовательского интерфейса Swagger.
}

app.UseHttpsRedirection(); // Перенаправление всех запросов на HTTPS.

app.UseAuthorization(); // Подключение авторизации.

app.MapControllers(); // Настройка маршрутов для контроллеров.

app.Run(); // Запуск приложения.