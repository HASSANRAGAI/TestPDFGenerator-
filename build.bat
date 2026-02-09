@echo off
REM Build script for Windows

echo Building backend...
dotnet build

if %errorlevel% neq 0 (
    echo Backend build failed
    exit /b 1
)
echo Backend build successful

echo.
echo Building frontend...
cd src\web
call npm install
call npm run build

if %errorlevel% neq 0 (
    echo Frontend build failed
    exit /b 1
)
echo Frontend build successful

echo.
echo All builds completed successfully!
