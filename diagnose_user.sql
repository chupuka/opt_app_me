-- Полная диагностика пользователя admin
-- Выполните: \c ProFormDB перед запуском этого скрипта

-- 1. Основная информация о пользователе
SELECT 
    "Id",
    "UserName",
    "NormalizedUserName",
    "Email",
    "NormalizedEmail",
    "EmailConfirmed",
    "PasswordHash" IS NOT NULL AS "HasPassword",
    "LockoutEnabled",
    "LockoutEnd",
    "AccessFailedCount",
    "TwoFactorEnabled",
    "PhoneNumberConfirmed"
FROM "AspNetUsers"
WHERE "NormalizedUserName" = UPPER('admin')
   OR "NormalizedEmail" = UPPER('admin@proform.ru');

-- 2. Роли пользователя
SELECT 
    u."UserName",
    r."Name" AS "Role",
    r."NormalizedName" AS "NormalizedRole"
FROM "AspNetUsers" u
JOIN "AspNetUserRoles" ur ON u."Id" = ur."UserId"
JOIN "AspNetRoles" r ON ur."RoleId" = r."Id"
WHERE u."NormalizedUserName" = UPPER('admin');

-- 3. Проверка блокировки
SELECT 
    "UserName",
    CASE 
        WHEN "LockoutEnd" IS NULL THEN 'НЕ ЗАБЛОКИРОВАН'
        WHEN "LockoutEnd" > NOW() THEN 'ЗАБЛОКИРОВАН до ' || "LockoutEnd"::text
        ELSE 'БЛОКИРОВКА ИСТЕКЛА'
    END AS "LockoutStatus",
    "AccessFailedCount" AS "FailedAttempts"
FROM "AspNetUsers"
WHERE "NormalizedUserName" = UPPER('admin');

-- 4. Исправление проблем (если нужно)
-- Раскомментируйте нужные строки:

-- Разблокировать аккаунт:
-- UPDATE "AspNetUsers"
-- SET "LockoutEnd" = NULL, "AccessFailedCount" = 0
-- WHERE "NormalizedUserName" = UPPER('admin');

-- Подтвердить email (если не подтвержден):
-- UPDATE "AspNetUsers"
-- SET "EmailConfirmed" = true
-- WHERE "NormalizedUserName" = UPPER('admin');
