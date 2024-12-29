using Microsoft.EntityFrameworkCore; // ����������� EF Core ��� ������ � ����� ������.
using ClassLibrary1; // ����������� ���������������� ����������, ���������� ������ ������.
using System; // ����������� ������������ ���� ��� ������� ��������� �������.

var builder = WebApplication.CreateBuilder(args); // �������� � ��������� ����������.

/// ���������� �������� � ��������� ������������.

// ���������� ������������ � ����������.
builder.Services.AddControllers();

// ��������� ��������� ���� ������ � �������������� SQLite � ������ ����������� �� ������������.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ���������� API Explorer ��� �������� �������� ����� OpenAPI.
builder.Services.AddEndpointsApiExplorer();

// ���������� Swagger ��� ���������������� � ������������ API.
builder.Services.AddSwaggerGen();

var app = builder.Build(); // ���������� ����������.

// ������������ ��������� ��������� HTTP-��������.

if (app.Environment.IsDevelopment()) // ��������, ����������� �� ���������� � ������ ����������.
{
    app.UseSwagger(); // ��������� Swagger � ������ ����������.
    app.UseSwaggerUI(); // ��������� ����������������� ���������� Swagger.
}

app.UseHttpsRedirection(); // ��������������� ���� �������� �� HTTPS.

app.UseAuthorization(); // ����������� �����������.

app.MapControllers(); // ��������� ��������� ��� ������������.

app.Run(); // ������ ����������.