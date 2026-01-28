@echo off
cd /d "%~dp0"
echo Creating PostgreSQL migration...
dotnet ef migrations add InitialPostgreSQLSetup --context ProForm.Data.ApplicationDbContext
if %ERRORLEVEL% EQU 0 (
    echo Migration created successfully!
    echo.
    echo Now apply it to database:
    echo dotnet ef database update --project ProForm --context ProForm.Data.ApplicationDbContext
) else (
    echo Failed to create migration. Check errors above.
)
pause
