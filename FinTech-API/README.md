# NeoBankApi

Простий ASP.NET Core Web API для домашнього завдання з FluentValidation, бізнес-помилками та глобальним `IExceptionHandler`.

## Запуск

```bash
dotnet restore
dotnet run
```

## Приклади запитів

Створити рахунок:

```http
POST /api/accounts
Content-Type: application/json

{
  "iban": "UA111111111111111111111111111",
  "accountHolder": "Ivan Petrenko",
  "email": "ivan@test.com",
  "initialDeposit": 10000,
  "currency": "UAH",
  "dailyLimit": 50000
}
```

Переказ:

```http
POST /api/transfers
Content-Type: application/json

{
  "fromIban": "UA111111111111111111111111111",
  "toIban": "UA222222222222222222222222222",
  "amount": 500,
  "idempotencyKey": "order-123456789",
  "description": "Test transfer"
}
```
