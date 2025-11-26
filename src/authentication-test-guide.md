# Authentication Test Script
# Test these endpoints in order using Postman or similar tool

## Step 0: Check if user exists in database (new debug endpoint)
GET http://localhost:44333/api/users/debug-check-user/turista1@gmail.com

Expected response:
{
    "username": "turista1@gmail.com",
    "exists": true,
    "isActive": true,
    "userData": {
        "id": -21,
        "role": "Tourist",
        "isActive": true
    }
}

## Step 1: Test Debug Login (should work without authentication)
POST http://localhost:44333/api/users/debug-login
Content-Type: application/json

{
    "username": "turista1@gmail.com",
    "password": "turista1"
}

Expected response: 
{
    "success": true,
    "token": "eyJ...", // Your JWT token
    "userId": -21,
    "message": "Login successful"
}

## Step 2: Copy the token from Step 1 and test the tourist problems endpoint
GET http://localhost:44333/api/tourist/problems?page=1&pageSize=10
Authorization: Bearer YOUR_TOKEN_HERE

Expected response: HTTP 200 with problems data

## Step 3: Test creating a problem
POST http://localhost:44333/api/tourist/problems
Authorization: Bearer YOUR_TOKEN_HERE
Content-Type: application/json

