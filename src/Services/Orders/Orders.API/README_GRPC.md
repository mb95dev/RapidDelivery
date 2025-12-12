# gRPC Service Implementation

This document describes the gRPC service implementation for the Orders API, demonstrating microservices communication patterns.

## Overview

The gRPC service provides high-performance, type-safe communication for order operations. It demonstrates:
- **Unary RPC**: Request-response pattern for single operations
- **Server Streaming**: Streaming data from server to client
- **Protocol Buffers**: Efficient binary serialization

## Service Definition

The service is defined in `Protos/orders.proto` and includes:

### Unary RPCs
- `GetOrder`: Retrieve a single order by ID
- `CreateOrder`: Create a new order
- `GetOrdersSummary`: Get aggregated statistics for reporting

### Server Streaming
- `StreamOrders`: Stream orders as they are processed

## When to Use gRPC vs REST

**Use gRPC for:**
- Service-to-service communication (microservices)
- High-performance requirements
- Strong typing and contract enforcement
- Streaming scenarios
- Real-time updates

**Use REST for:**
- Public APIs
- Browser clients (though gRPC-Web is available)
- Simple CRUD operations
- When HTTP/JSON is required

## Testing the gRPC Service

### Using gRPCurl (Command Line)

```bash
# Install grpcurl
# Windows: choco install grpcurl
# Mac: brew install grpcurl
# Linux: https://github.com/fullstorydev/grpcurl/releases

# List services
grpcurl -plaintext localhost:5003 list

# Get order by ID
grpcurl -plaintext -d '{"order_id": "your-order-id"}' \
  localhost:5003 orders.OrderService/GetOrder

# Get orders summary
grpcurl -plaintext localhost:5003 orders.OrderService/GetOrdersSummary
```

### Using .NET gRPC Client

```csharp
using var channel = GrpcChannel.ForAddress("http://localhost:5003");
var client = new OrderService.OrderServiceClient(channel);

var request = new GetOrderRequest { OrderId = "your-order-id" };
var response = await client.GetOrderAsync(request);
```

## Interview Talking Points

- **Performance**: gRPC uses HTTP/2 and Protocol Buffers for efficient binary serialization
- **Type Safety**: Protobuf provides compile-time type checking
- **Streaming**: Server streaming enables real-time data delivery
- **Contract-First**: Protobuf definitions serve as API contracts
- **Interoperability**: gRPC supports multiple languages (C#, Go, Java, Python, etc.)

