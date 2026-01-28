-- ВНИМАНИЕ: Этот скрипт НЕ может сбросить пароль напрямую через SQL,
-- так как пароли хранятся в виде хэша. Используйте код приложения для сброса пароля.

-- Однако, вы можете разблокировать аккаунт и сбросить счетчик неудачных попыток:

-- 1. Разблокировать аккаунт admin
UPDATE "AspNetUsers"
SET 
    "LockoutEnd" = NULL,
    "AccessFailedCount" = 0,
    "EmailConfirmed" = true
WHERE "NormalizedUserName" = UPPER('admin');

-- 2. Проверить результат
SELECT 
    "UserName",
    "Email",
    "EmailConfirmed",
    "LockoutEnd",
    "AccessFailedCount"
FROM "AspNetUsers"
WHERE "NormalizedUserName" = UPPER('admin');

-- Для сброса пароля используйте код в Program.cs (см. TROUBLESHOOTING_LOGIN.md)
