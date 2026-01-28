-- Создание базы данных ProFormDB для проекта ProForm
-- Выполните этот скрипт от имени пользователя username или суперпользователя

-- Создаем базу данных, если она еще не существует
CREATE DATABASE "ProFormDB"
    WITH 
    OWNER = username
    ENCODING = 'UTF8'
    LC_COLLATE = 'Russian_Russia.1251'
    LC_CTYPE = 'Russian_Russia.1251'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

-- Подключаемся к созданной базе данных
\c ProFormDB

-- Даем все права пользователю username на базу данных
GRANT ALL PRIVILEGES ON DATABASE "ProFormDB" TO username;
