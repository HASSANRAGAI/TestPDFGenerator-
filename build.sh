#!/bin/bash

# Build Backend
echo "Building backend..."
cd "$(dirname "$0")"
dotnet build

if [ $? -eq 0 ]; then
    echo "✓ Backend build successful"
else
    echo "✗ Backend build failed"
    exit 1
fi

# Build Frontend
echo ""
echo "Building frontend..."
cd src/web
npm install
npm run build

if [ $? -eq 0 ]; then
    echo "✓ Frontend build successful"
else
    echo "✗ Frontend build failed"
    exit 1
fi

echo ""
echo "✓ All builds completed successfully!"
