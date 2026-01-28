@echo off
cd /d "%~dp0"
dotnet ef migrations add InitialCreate --context ProForm.Data.ApplicationDbContext
pause
