#!/bin/bash

# Start Backend API
echo "Starting backend API on http://localhost:5000..."
cd "$(dirname "$0")"
dotnet run --project src/PdfTemplateSystem.Api/PdfTemplateSystem.csproj --urls "http://localhost:5000" &
BACKEND_PID=$!

# Wait for backend to start
echo "Waiting for backend to start..."
sleep 5

# Start Frontend
echo ""
echo "Starting frontend on http://localhost:3000..."
cd src/web
npm run dev &
FRONTEND_PID=$!

echo ""
echo "=========================================="
echo "Services started successfully!"
echo "Backend API: http://localhost:5000"
echo "Frontend: http://localhost:3000"
echo "Swagger: http://localhost:5000/swagger"
echo "=========================================="
echo ""
echo "Press Ctrl+C to stop all services"

# Trap Ctrl+C and cleanup
trap 'echo "Stopping services..."; kill $BACKEND_PID $FRONTEND_PID; exit 0' INT

# Wait for both processes
wait
