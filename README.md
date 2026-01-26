# ProForm - Система управления фитнес-клубом

Автоматизированная система управления фитнес-клубом «ProForm» разработана в соответствии с техническим заданием.

## Технологии

- **Backend**: ASP.NET Core 9.0 (C#)
- **Frontend**: HTML5, CSS3, JavaScript, Bootstrap 5
- **База данных**: MySQL 8.0
- **ORM**: Entity Framework Core 9.0
- **Аутентификация**: ASP.NET Core Identity

## Требования

- .NET 9.0 SDK
- MySQL 8.0 Server
- Visual Studio 2022 или VS Code

## Установка и настройка

1. Клонируйте репозиторий или скопируйте проект
2. Настройте строку подключения к БД в `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=ProFormDB;User=root;Password=your_password;Port=3306;"
   }
   ```

3. Создайте базу данных MySQL:
   ```sql
   CREATE DATABASE ProFormDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   ```

4. Примените миграции:
   ```bash
   dotnet ef database update
   ```

5. Запустите приложение:
   ```bash
   dotnet run
   ```

## Инициализация ролей

После первого запуска необходимо создать роли пользователей. Для этого можно использовать следующий код в `Program.cs` или создать отдельный скрипт инициализации:

- **Admin** - полный доступ ко всем функциям
- **Trainer** - просмотр расписания, фиксация посещений
- **Manager** - просмотр отчетов и аналитики

## Структура проекта

- `Models/` - модели данных
- `Controllers/` - контроллеры MVC
- `Views/` - представления (Razor)
- `Data/` - контекст базы данных
- `Migrations/` - миграции Entity Framework

## Основные функции

- Управление клиентами
- Управление абонементами
- Управление тренерами
- Расписание занятий (с FullCalendar)
- Учет посещаемости
- Финансовые отчеты
- Экспорт отчетов в PDF

## Тестирование

Тесты находятся в папке `Tests/` и соответствуют сценарию тестов из технического задания.

## Лицензия

Учебный проект

