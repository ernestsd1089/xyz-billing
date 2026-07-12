# XYZ Billing

## Quick start (Docker — recommended)

Requires Docker Desktop running.

```bash
docker compose up --build
```

Then open **http://localhost:8080**.

---

## API

### `POST /api/orders`

Request:

```json
{
  "orderNumber": "ORD-1001",
  "userId": "u-42",
  "amount": 49.99,
  "paymentGatewayId": "stripe",
  "description": "2x widget"
}
```

Success — `200 OK`:

```json
{
  "orderNumber": "ORD-1001",
  "amount": 49.99,
  "timestamp": "2026-07-11T08:21:31.01+00:00",
  "confirmationId": "stripe_49ced910eda34309906ec48f72d2a3b9",
  "status": "Succeeded"
}
```

Errors:

| Status | When | Body |
|--------|------|------|
| `400 Bad Request` | Validation failed (missing field, amount ≤ 0) | `{ "errors": [ { "propertyName": "...", "errorMessage": "..." } ] }` |
| `400 Bad Request` | Unknown `paymentGatewayId` | `{ "error": "UnknownGateway", "message": "..." }` |
| `402 Payment Required` | Gateway declined the charge | `{ "error": "PaymentFailed", "message": "..." }` |

---

## Configuration

No secrets are required — the payment gateways are mocked, so the app runs with zero external
configuration.

| Setting | Where | Purpose | Default |
|---------|-------|---------|---------|
| `ASPNETCORE_ENVIRONMENT` | environment variable | `Development` / `Production` | `Development` local, `Production` in Compose |
| `ASPNETCORE_HTTP_PORTS` | environment variable | API listening port | `8080` (Compose) |
| `Cors:AllowedOrigins` | `Api/appsettings.json` | Origins allowed to call the API cross-origin | `http://localhost:4200` |
| dev proxy target | `Frontend/proxy.conf.json` | Where the Angular dev server forwards `/api` | `http://localhost:5207` |
