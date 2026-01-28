@echo off
echo ========================================
echo Исправление прав доступа к базе данных
echo ========================================
echo.
echo Выполните следующие команды в psql:
echo.
echo 1. Подключитесь к PostgreSQL как суперпользователь:
echo    psql -h localhost -U postgres -d ProFormDB
echo.
echo 2. Выполните команды из файла fix_permissions.sql
echo    или выполните вручную:
echo.
echo    GRANT ALL ON SCHEMA public TO username;
echo    GRANT CREATE ON SCHEMA public TO username;
echo    GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO username;
echo    GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO username;
echo    ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO username;
echo    ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO username;
echo.
echo 3. После этого выполните:
echo    dotnet ef database update --project ProForm --context ProForm.Data.ApplicationDbContext
echo.
pause
