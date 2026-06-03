# Customer Account API — Test Scripts

Base URL: `http://localhost:5041`

> **Tip:** Replace `{{token}}` with the JWT returned from login.  
> Replace `{{adminToken}}` with the token from admin login.  
> Replace `{{userId}}` with the target customer's ID.

---

## 1. Authentication

### Register a new customer

```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "Test@1234",
  "firstName": "John",
  "lastName": "Doe",
  "dateOfBirth": "1995-06-15",
  "phoneNumber": "08012345678"
}
```

**Expected:** `200 OK`
```json
{ "message": "User registered successfully." }
```

---

### Login as customer

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "password": "Test@1234"
}
```

**Expected:** `200 OK`
```json
{
  "token": "eyJhbGciOi...",
  "email": "john.doe@example.com",
  "userId": "guid-here",
  "roles": ["Customer"]
}
```

---

### Login as admin (seeded account)

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@customeraccount.com",
  "password": "Admin@123456"
}
```

**Expected:** `200 OK` with `"roles": ["Admin"]`

---

## 2. Account Operations (requires Customer token)

### Create a savings account

```http
POST /api/accounts
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "accountType": "Savings",
  "initialDeposit": 5000.00
}
```

**Expected:** `201 Created`
```json
{
  "id": 1,
  "accountNumber": "ACC202406031234",
  "balance": 5000.00,
  "accountType": "Savings",
  "openedDate": "2026-06-03T...",
  "isActive": true
}
```

---

### Create a checking account

```http
POST /api/accounts
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "accountType": "Checking",
  "initialDeposit": 2000.00
}
```

---

### Get my accounts

```http
GET /api/accounts
Authorization: Bearer {{token}}
```

**Expected:** `200 OK` — Array of accounts belonging to the logged-in user.

---

### Get account by ID

```http
GET /api/accounts/1
Authorization: Bearer {{token}}
```

**Expected:** `200 OK` — Single account object, or `404` if not found/not owned.

---

### Transfer money between accounts

```http
POST /api/accounts/transfer
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "fromAccountId": 1,
  "toAccountId": 2,
  "amount": 500.00
}
```

**Expected:** `200 OK`
```json
{ "message": "Transfer completed successfully." }
```

---

## 3. Admin Operations (requires Admin token)

### Get all customers

```http
GET /api/admin/customers
Authorization: Bearer {{adminToken}}
```

**Expected:** `200 OK` — Array of all customers with their accounts.

---

### Get customer by ID

```http
GET /api/admin/customers/{{userId}}
Authorization: Bearer {{adminToken}}
```

**Expected:** `200 OK` — Single customer with accounts, or `404`.

---

### Get all accounts (across all customers)

```http
GET /api/admin/accounts
Authorization: Bearer {{adminToken}}
```

**Expected:** `200 OK` — Array of all accounts in the system.

---

### Assign a role to a customer

```http
POST /api/admin/customers/{{userId}}/assign-role
Authorization: Bearer {{adminToken}}
Content-Type: application/json

{
  "role": "Admin"
}
```

**Expected:** `200 OK`
```json
{ "message": "Role 'Admin' assigned successfully." }
```

---

### Delete a customer

```http
DELETE /api/admin/customers/{{userId}}
Authorization: Bearer {{adminToken}}
```

**Expected:** `200 OK`
```json
{ "message": "Customer deleted successfully." }
```

---

## 4. Validation Error Examples

### Invalid account type

```http
POST /api/accounts
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "accountType": "InvalidType",
  "initialDeposit": 1000.00
}
```

**Expected:** `400 Bad Request`
```json
{
  "errors": [
    { "field": "AccountType", "message": "Account type must be one of: Savings, Checking, Business." }
  ]
}
```

---

### Transfer with insufficient funds

```http
POST /api/accounts/transfer
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "fromAccountId": 1,
  "toAccountId": 2,
  "amount": 999999.00
}
```

**Expected:** `400 Bad Request`
```json
{ "message": "Insufficient funds." }
```

---

### Access admin endpoint without admin role

```http
GET /api/admin/customers
Authorization: Bearer {{token}}
```

**Expected:** `403 Forbidden`

---

## 5. Full Test Flow (Step-by-step)

1. **Login as admin** → save `adminToken`
2. **Register customer** (john.doe@example.com)
3. **Login as customer** → save `token` and `userId`
4. **Create Savings account** (deposit 5000)
5. **Create Checking account** (deposit 2000)
6. **Get my accounts** → verify both appear
7. **Transfer 500** from Savings to Checking
8. **Get account 1** → verify balance is 4500
9. **Admin: Get all customers** → verify john appears
10. **Admin: Get customer by ID** → verify accounts listed
11. **Admin: Assign Manager role** to john
12. **Admin: Delete customer** → verify deletion
13. **Login as deleted user** → expect `401 Unauthorized`

---

## Available Roles

| Role     | Description                          |
|----------|--------------------------------------|
| Admin    | Full system access, manage users     |
| Customer | Default role, manage own accounts    |
| Manager  | Reserved for future use              |

## Valid Account Types

| Type      |
|-----------|
| Savings   |
| Checking  |
| Business  |
