# RapidDelivery Architecture Documentation

## Overview

RapidDelivery is a microservices-based order management system built with .NET 10, demonstrating modern software architecture patterns including DDD, CQRS, Event Sourcing concepts, and Clean Architecture.

## Architecture Principles

### Domain-Driven Design (DDD)
- **Bounded Contexts**: Each service (Orders, Notifications) represents a distinct bounded context
- **Aggregate Roots**: Order is the aggregate root, managing its own consistency boundaries
- **Value Objects**: Address, Payment, OrderId, etc. are immutable value objects
- **Domain Events**: OrderCreatedEvent, OrderUpdatedEvent capture domain state changes

### CQRS (Command Query Responsibility Segregation)
- **Commands**: Write operations (CreateOrderCommand) modify state
- **Queries**: Read operations (GetOrdersQuery, GetOrderByIdQuery) retrieve data
- **Separation**: Commands and queries use different models optimized for their purpose

### Clean Architecture Layers

```
Orders.API (Presentation)
    ↓
Orders.Application (Use Cases)
    ↓
Orders.Domain (Business Logic)
    ↓
Orders.Infrastructure (Data Access, External Services)
```

## System Design

### Microservices Architecture

```
┌─────────────────┐         ┌──────────────────┐
│   Orders.API    │────────▶│  Notifications   │
│                 │  Events │      .API        │
│  - REST API     │         │                  │
│  - gRPC Service │         │  - Email Service │
└─────────────────┘         └──────────────────┘
         │
         │
    ┌────▼────┐
    │ RabbitMQ│
    │ (Events)│
    └─────────┘
```

### Communication Patterns

1. **Synchronous (REST/gRPC)**
   - Direct service-to-service calls
   - Used for: Real-time queries, immediate responses

2. **Asynchronous (Message Queue)**
   - Event-driven communication via RabbitMQ
   - Used for: Cross-service notifications, eventual consistency

### Outbox Pattern

Ensures reliable message delivery:
1. Domain event is raised
2. Event is stored in OutboxMessage table (same transaction)
3. Background service (OutboxProcessor) publishes to RabbitMQ
4. Message is marked as processed

This guarantees at-least-once delivery and maintains transactional consistency.

## Technology Stack

### Backend
- **.NET 10**: Latest framework features
- **Entity Framework Core**: ORM for data access
- **MediatR**: CQRS implementation
- **MassTransit**: Message bus abstraction
- **RabbitMQ**: Message broker
- **Carter**: Minimal API framework
- **gRPC**: High-performance service communication

### Frontend
- **Next.js 14**: React framework with App Router
- **TypeScript**: Type-safe frontend
- **React Query**: Server state management
- **Axios**: HTTP client

### Infrastructure
- **Docker**: Containerization
- **Kubernetes**: Orchestration
- **SQL Server**: Primary database
- **RabbitMQ**: Message broker

## Data Flow

### Creating an Order

```
1. Client → POST /orders (REST)
2. Orders.API → CreateOrderEndpoint
3. MediatR → CreateOrderHandler
4. Domain → Order.Create() (Aggregate Root)
5. Domain Event → OrderCreatedEvent
6. Outbox → OutboxMessage saved (same transaction)
7. Background Service → Publishes to RabbitMQ
8. Notifications Service → Consumes event, sends email
```

### Querying Orders

```
1. Client → GET /orders (REST)
2. Orders.API → GetOrdersEndpoint
3. MediatR → GetOrdersQueryHandler
4. EF Core → Database query
5. Response → OrderSummaryDto[]
```

## Scalability Considerations

### Horizontal Scaling
- Stateless API services can scale horizontally
- Database connection pooling handles concurrent requests
- Message queue decouples services for independent scaling

### Caching Strategy
- Application-level caching for frequently accessed data
- Redis (future): Distributed caching for shared state

### Database Optimization
- Indexed queries for fast lookups
- Pagination for large result sets
- Read replicas (future) for read-heavy workloads

## Security

### Authentication & Authorization
- JWT tokens for API authentication (to be implemented)
- Role-based access control (RBAC) for authorization

### Data Protection
- Sensitive data (payment info) encrypted at rest
- HTTPS for all external communication
- Secrets management via Kubernetes secrets

## Monitoring & Observability

### Health Checks
- `/health/live`: Liveness probe
- `/health/ready`: Readiness probe

### Logging
- Structured logging with Serilog (to be implemented)
- Correlation IDs for request tracking

### Metrics
- Application Insights integration (future)
- Custom metrics for business events

## Interview Talking Points

### Why DDD?
- **Business Alignment**: Domain model reflects business language
- **Maintainability**: Clear boundaries prevent coupling
- **Testability**: Domain logic isolated from infrastructure

### Why CQRS?
- **Performance**: Optimized read and write models
- **Scalability**: Read and write can scale independently
- **Flexibility**: Different storage strategies per side

### Why Microservices?
- **Independence**: Services can be developed/deployed independently
- **Technology Diversity**: Each service can use optimal tech stack
- **Fault Isolation**: Failure in one service doesn't cascade

### Trade-offs
- **Complexity**: More moving parts, harder to debug
- **Network Latency**: Service calls add overhead
- **Data Consistency**: Eventual consistency requires careful design

