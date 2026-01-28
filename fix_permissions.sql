-- Исправление прав доступа для базы данных ProFormDB
-- Выполните этот скрипт от имени суперпользователя (postgres)

-- Подключаемся к базе данных ProFormDB
\c ProFormDB

-- Даем права пользователю username на схему public
GRANT ALL ON SCHEMA public TO username;
GRANT CREATE ON SCHEMA public TO username;

-- Даем права на все существующие таблицы в схеме public
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO username;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO username;

-- Даем права на будущие таблицы и последовательности
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO username;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO username;

-- Если нужно изменить владельца базы данных (опционально)
-- ALTER DATABASE "ProFormDB" OWNER TO username;
