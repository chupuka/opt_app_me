-- Проверка пользователей в базе данных ProFormDB
-- Выполните этот скрипт после подключения к базе данных: \c ProFormDB

-- 1. Проверка всех пользователей
SELECT 
    "Id",
    "UserName",
    "Email",
    "EmailConfirmed",
    "LockoutEnabled",
    "LockoutEnd",
    "AccessFailedCount"
FROM "AspNetUsers"
ORDER BY "UserName";

-- 2. Проверка ролей пользователей
SELECT 
    u."UserName",
    u."Email",
    r."Name" AS "Role"
FROM "AspNetUsers" u
LEFT JOIN "AspNetUserRoles" ur ON u."Id" = ur."UserId"
LEFT JOIN "AspNetRoles" r ON ur."RoleId" = r."Id"
ORDER BY u."UserName", r."Name";

-- 3. Проверка конкретного пользователя admin
SELECT 
    "UserName",
    "Email",
    "EmailConfirmed",
    "LockoutEnabled",
    "LockoutEnd",
    "AccessFailedCount",
    "PasswordHash" IS NOT NULL AS "HasPassword"
FROM "AspNetUsers"
WHERE "NormalizedUserName" = UPPER('admin');
