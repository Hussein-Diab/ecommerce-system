# Work Plan: Online Food Ordering + Restaurant Operations Panel

**Deadline:** Tuesday, Nov 24, 2026. **Time available:** 10 days.
**Rule for this project:** I write the code myself. Claude reviews it and explains, but does not hand me full solutions.

---

## 0. The system in one picture

```
┌──────────────────────────┐                                   ┌──────────────────────────┐
│  CLIENT APP (customers)  │                                   │  ADMIN APP (staff)       │
│  ASP.NET Core MVC        │      ┌─────────────────────┐      │  ASP.NET Core MVC        │
│  Identity: Customer      │─────▶│      RabbitMQ       │─────▶│  Identity: Admin/Staff/  │
│  SignalR: order status   │      │ exchange:           │      │            Manager       │
│  Stripe / dummy payment  │◀─────│  foodapp.events     │◀─────│  SignalR: new orders     │
│                          │      │ DLX: foodapp.dlx    │      │  Chart.js + PDF/Excel    │
│  DB: FoodClientDb        │      └─────────────────────┘      │  AI: review sentiment    │
└──────────────────────────┘                                   │  DB: FoodAdminDb         │
                                                               └──────────────────────────┘
          both reference ──▶  FoodOrdering.Contracts (message classes only)
```

**Who owns what.** This is the main design decision, so be ready to defend it:

| Data | Owner (source of truth) | The other app has... |
|---|---|---|
| Menu items and prices | **Admin** | a read-only copy, kept in sync by `MenuItemChanged` messages |
| Customer accounts | **Client** | only the customer name and id that come inside messages |
| Orders | **Client** creates them | its own copy, which it uses to accept, reject or update them |
| Order status | **Admin** decides it | the Client updates its copy when `OrderStatusChanged` arrives |
| Payments | **Client** (it talks to Stripe) | a copy from `PaymentCompleted`, used for revenue reports |
| Reviews | **Client** | a copy plus an AI sentiment score, used for analytics |

## 1. Solution structure

```
FoodOrdering.sln
├─ src/
│  ├─ FoodOrdering.Contracts/      class library: messages, enums, routing-key constants. NO EF, NO logic.
│  ├─ FoodOrdering.Client.Web/     MVC app for customers
│  └─ FoodOrdering.Admin.Web/      MVC app for staff
├─ tests/FoodOrdering.Tests/       (bonus) xUnit
├─ docs/                           architecture diagram, message catalogue, screenshots
└─ docker-compose.yml              RabbitMQ (+ SQL Server, + the apps for the bonus)
```

Folders inside **each** web app (this is the layered architecture they grade, feature 4):

```
Controllers/   thin: validate input, call a service, return a view
Services/      business rules (PlaceOrder, ChangeStatus...). Uses repositories and the publisher.
Repositories/  IOrderRepository + OrderRepository: the only place that touches DbContext
Data/          AppDbContext, Migrations/, seeders (roles and the admin user)
Models/        EF entities (Order, OrderItem, MenuItem, ProcessedMessage...)
ViewModels/    what views receive and post, with validation attributes
Messaging/     RabbitMqConnection, IMessagePublisher, consumers (BackgroundService)
Hubs/          SignalR hubs
Views/
```

## 2. Message catalogue (draft; it goes in the docs)

One **topic exchange** `foodapp.events` (durable). One **dead-letter exchange** `foodapp.dlx`.

| # | Message | Direction | Routing key | Queue (consumer) |
|---|---|---|---|---|
| 1 | `OrderPlaced` | Client → Admin | `order.placed` | `admin.orders` |
| 2 | `OrderCancelled` | Client → Admin | `order.cancelled` | `admin.orders` |
| 3 | `PaymentCompleted` | Client → Admin | `payment.completed` | `admin.payments` |
| 4 | `ReviewSubmitted` | Client → Admin | `review.submitted` | `admin.reviews` |
| 5 | `OrderStatusChanged` | Admin → Client | `order.status.changed` | `client.order-updates` |
| 6 | `MenuItemChanged` | Admin → Client | `menu.item.changed` | `client.menu` |

Dead-letter queues: `admin.dlq` and `client.dlq`, bound to `foodapp.dlx`.

Contracts design (OOP: inheritance and encapsulation):
- An abstract base class `IntegrationMessage` with `MessageId (Guid)` and `OccurredAtUtc`. Every message inherits it.
- `OrderStatus` enum (Pending, Accepted, Preparing, OutForDelivery, Delivered, Rejected, Cancelled) lives in Contracts so both apps agree on it.
- A static class of routing-key constants, so no magic strings are spread around the code.

> Research: *"topic exchange vs direct exchange"*. Why can one exchange serve both directions? Why use a topic exchange rather than fanout?

---

## 3. Day-by-day plan

Each phase has the same parts: **Learn** (what to research first), **Build**, **Done when** (how to check it), and **Exam questions** (be able to answer these out loud, because understanding carries the most weight in the grade).
**After each phase:** commit, push, and ask Claude for a review.

### Day 1 (Sat 14): Setup and RabbitMQ basics
**Learn**
- RabbitMQ official tutorials 1–5 for .NET (Hello World → Topics). Do them in a throwaway console app, **not** in the project.
- ⚠️ The current `RabbitMQ.Client` (v7+) is **fully async** (`IChannel`, `CreateChannelAsync`, `AsyncEventingBasicConsumer`). Many blog posts use v6 (`IModel`) and won't compile. Check the version before you copy ideas.
- Docker basics: running the `rabbitmq:management` image. The Management UI is at http://localhost:15672.

**Build**
- Create the solution with the 3 projects. Add a project reference from both web apps to Contracts.
- `docker-compose.yml` with RabbitMQ (ports 5672 and 15672). Write it yourself; the Docker Hub page for `rabbitmq` explains the image.
- Write all 6 message classes plus the base class and the enum in Contracts.
- First commit and push.

**Done when:** the solution builds, RabbitMQ runs and you can log in to the Management UI, and your console app sends and receives a message.

**Exam questions:** What are an exchange, a queue, a binding and a routing key? Why a broker instead of HTTP calls between the apps? What does "loosely coupled" mean here?

### Day 2 (Sun 15): Client app with database, auth and order placement (no messaging yet)
**Learn:** EF Core code-first and migrations; ASP.NET Core Identity with roles; the repository pattern and dependency-injection lifetimes (Scoped, Singleton, Transient).

**Build**
- Entities: `MenuItem` (the local copy), `Order`, `OrderItem` (**snapshot of the price** at order time), `Review`, `ProcessedMessage`.
- Identity with the `Customer` role. Register and login.
- Repositories and services. Pages: browse the menu, a cart (Session is fine), place an order (status = `Pending`), My Orders, order details, cancel a pending order.
- Temporarily seed a few menu items so you can test. Later they will arrive via messages.

**Done when:** a customer can register, order, and see the order as Pending. The migration is committed.

**Exam questions:** Why does OrderItem copy the price instead of pointing to the menu price? Why must controllers not use DbContext directly? Why is DbContext Scoped?

### Day 3 (Mon 16): Admin app with database, roles and menu CRUD
**Build**
- Identity with roles `Admin`, `Manager`, `Staff`. Seed the roles and one admin user at startup.
- `[Authorize(Roles = ...)]` for the rules, for example: Staff manage orders, Manager sees reports, Admin manages the menu and users.
- Entities: `MenuItem` (the master copy), `Order`/`OrderItem` (received copies), `Payment`, `Review`, `ProcessedMessage`.
- Full menu CRUD (create, edit, delete, list). Orders list page (empty for now). Accept/Reject form with a delivery time.

**Done when:** you can log in with each role and see different permissions, and menu CRUD works.

**Exam questions:** Authentication vs authorization? Where is the password stored and how (hashing)? Why does each app have its own Identity?

### Day 4 (Tue 17): ⭐ RabbitMQ round trip (the core of the grade)
**Learn:** a long-lived connection and channel per app (why?); `BackgroundService`; `IServiceScopeFactory` (**a BackgroundService is a singleton but DbContext is scoped**, which is the classic trap); `IOptions<T>` for configuration.

**Build**
- `RabbitMqOptions` in `appsettings.json` (host, user, password from **user-secrets**, never hard-coded).
- A `RabbitMqConnection` singleton and an `IMessagePublisher` with `PublishAsync<T>(message, routingKey)` that serializes to JSON with `persistent = true`.
- A topology setup that declares the exchange, queues and bindings (durable) at startup, in both apps.
- Client: `OrderService.PlaceOrder` saves to the DB, **then** publishes `OrderPlaced`.
- Admin: `OrderPlacedConsumer : BackgroundService` deserializes, saves to FoodAdminDb, then acks.
- Admin: Accept/Reject → publish `OrderStatusChanged`. Client consumer updates the order status.
- Admin menu CRUD → publish `MenuItemChanged`. Client consumer upserts its menu copy. Delete the temporary seed. Add a "Re-sync whole menu" admin button.

**Done when:** you can do the full round trip by hand. Then **stop the Admin app, place 3 orders, watch the queue count in the Management UI, start Admin, and see all 3 arrive.** (This is your demo.)

**Exam questions:** What happens if the Admin app is down when an order is placed? Why durable queues **and** persistent messages? What happens if the app crashes *between* SaveChanges and Publish? (This leads to the Outbox bonus.)

### Day 5 (Wed 18): Reliable messaging and logging
**Learn:** `autoAck: false`, `BasicAck` and `BasicNack(requeue: false)`; the `x-dead-letter-exchange` queue argument; idempotency; at-least-once delivery.

**Build**
- Manual ack only after the DB save succeeds.
- **Retry:** on an exception, retry up to 3 times. A simple approach: read an `x-retry-count` header, republish with count+1 and ack the original. When the count reaches 3, `BasicNack(requeue:false)` sends it to the DLQ. Compare this with a quorum queue's `x-delivery-limit` and pick one. Be able to explain why.
- **Duplicates:** before processing, check the `ProcessedMessages` table for the `MessageId`. Insert the id **in the same SaveChanges** as the business change, with a unique key on MessageId.
- Logging with `ILogger`: "Published {Type} {MessageId}", "Consumed...", "Failed... attempt {n}", "Dead-lettered...".
- An admin page (or just the Management UI) to look at the DLQ.

**Done when:** you can show a poison message ending in the DLQ, and the same message sent twice is processed once.

**Exam questions:** At-most-once vs at-least-once vs exactly-once? Why can't RabbitMQ guarantee no duplicates? What happens with `requeue: true` on a message that always fails?

### Day 6 (Thu 19): SignalR real-time
**Learn:** hubs, `IHubContext<T>` (to send from a consumer, not just from a controller), groups, `Clients.User(...)`, the JS client.

**Build**
- Admin: `OrdersHub`. The `OrderPlaced` consumer pushes the new order to the "staff" group, and a dashboard row appears without a refresh. Add a sound or toast if you like.
- Client: `CustomerHub`. The `OrderStatusChanged` consumer pushes to `Clients.User(customerId)`, so only that customer is notified.

**Done when:** two browser windows side by side show the round trip live.

**Exam questions:** SignalR vs RabbitMQ: why both? (One is browser↔server, the other is server↔server.) How does SignalR know which user is which?

### Day 7 (Fri 20): Payment and AI API
**Payment (feature 13):** Stripe **test mode** Checkout Session (card `4242 4242 4242 4242`). On the success URL, **verify the session on the server**; never trust the query string alone. Then mark the order paid and publish `PaymentCompleted`. If time is short, a dummy gateway is allowed by the spec.

**AI (feature 12):** when Admin consumes `ReviewSubmitted`, call a sentiment or LLM API (for example Hugging Face Inference, Google Gemini, OpenAI or Anthropic; check which has a free tier available to you) and store Positive/Neutral/Negative plus a score. Use a typed `HttpClient` service behind an interface, with the key in user-secrets. **If the API fails, still save the review** (sentiment = "Unknown"). Don't let an external API break the consumer.

**Exam questions:** Why must payment be confirmed server-side? Where are your API keys and why aren't they on GitHub? What happens if the AI API is down?

### Day 8 (Sat 21): Reports and dashboard
- Manager dashboard: revenue per day (line chart), orders by status (doughnut), top 5 items (bar), sentiment breakdown (pie), plus summary cards. Use LINQ `GroupBy` in a `ReportService` and pass JSON to Chart.js.
- Export: Excel with **ClosedXML**, PDF with **QuestPDF** (the Community license is free for this use; check its terms).
- Client- and server-side validation everywhere: DataAnnotations on ViewModels, `_ValidationScriptsPartial`, `ModelState.IsValid`. Add a global error page.

### Day 9 (Sun 22): Polish and bonuses (only if 1–13 work)
In this order: health checks (`/health` with a RabbitMQ and DB check) → a few xUnit tests for a consumer's duplicate logic → full Docker Compose → Outbox pattern.
Make the UI consistent: one Bootstrap layout per app, responsive, with clear status badges.

### Day 10 (Mon 23): Documentation and demo rehearsal
- `docs/`: features list, architecture diagram, message catalogue (section 2, final version), screenshots, how-to-run steps.
- Rehearse a 3–5 minute demo: login → order → admin popup → accept → customer live update → **stop Admin, order, restart, messages arrive** → dashboard and export.
- Answer every "Exam question" in this file out loud, without notes.

### Tue Nov 24: Submit. Do a final `git tag v1.0` and push.

---

## 4. If you fall behind, cut in this order
Keep features 1–8 at all costs, since they are the architecture grade. Then SignalR (9) → validation (11) → reports (10) → payment (13, use the dummy) → AI (12) → bonuses.

## 5. How to ask Claude for a review
1. Push your branch or commit.
2. Say: *"Review phase N"*, plus anything you're unsure about.
3. You'll get: bugs, design issues, and **why**. Hints first, not rewrites. Ask "show me" only after you've tried.
