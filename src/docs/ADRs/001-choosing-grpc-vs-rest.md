# ADR-001: Choosing gRPC vs REST for Service Communication

## Status
Accepted

## Context
We need to decide on the communication protocol for service-to-service communication in our microservices architecture. The options are:
- REST (HTTP/JSON)
- gRPC (HTTP/2 with Protocol Buffers)

## Decision
We will use **both** REST and gRPC, each for their appropriate use cases:
- **REST** for public APIs and browser clients
- **gRPC** for internal service-to-service communication

## Rationale

### REST Advantages
- Universal support (browsers, mobile apps, any HTTP client)
- Human-readable (JSON)
- Easy to debug with standard tools (curl, Postman)
- Caching support (HTTP cache headers)
- Well-understood by all developers

### gRPC Advantages
- Higher performance (binary Protocol Buffers vs text JSON)
- Strong typing with code generation
- HTTP/2 multiplexing (multiple requests over single connection)
- Built-in streaming support
- Smaller payload size

### Use Cases

**REST for:**
- Public API endpoints
- Frontend applications (browser clients)
- Third-party integrations
- Simple CRUD operations

**gRPC for:**
- Internal microservices communication
- High-throughput scenarios
- Real-time streaming (server/client/bidirectional)
- When strong typing is critical

## Consequences

### Positive
- Best of both worlds: developer-friendly REST + performant gRPC
- Flexibility to choose the right tool for each scenario
- Future-proof: can migrate endpoints as needed

### Negative
- More code to maintain (two API surfaces)
- Need to keep both in sync
- Additional complexity in service registration

## Implementation Notes
- REST endpoints use Carter framework
- gRPC services use Grpc.AspNetCore
- Both share the same application layer (MediatR handlers)
- Protocol-specific mapping happens at the API layer

## References
- [gRPC vs REST Performance](https://grpc.io/blog/grpc-stacks/)
- [Microsoft gRPC Documentation](https://learn.microsoft.com/en-us/aspnet/core/grpc/)

