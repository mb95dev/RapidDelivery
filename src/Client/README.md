# RapidDelivery Client - Next.js Frontend

Modern React/TypeScript frontend application built with Next.js 14 App Router, demonstrating integration with .NET backend.

## Features

- **Next.js 14 App Router** - Latest Next.js with Server and Client Components
- **TypeScript** - Fully typed for type-safe API integration
- **React Query (TanStack Query)** - Efficient server state management
- **Type-safe API Client** - Generated types matching .NET DTOs
- **Reporting Dashboard** - Real-time order statistics and summaries
- **Optimistic Updates** - Better UX with instant feedback

## Getting Started

### Prerequisites

- Node.js 18+ 
- npm or yarn
- .NET backend running on `http://localhost:5000`

### Installation

```bash
npm install
```

### Development

```bash
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) in your browser.

### Build for Production

```bash
npm run build
npm start
```

## Project Structure

```
Client/
├── src/
│   ├── app/              # Next.js App Router pages
│   │   ├── layout.tsx    # Root layout with providers
│   │   ├── page.tsx      # Home page with dashboard
│   │   └── providers.tsx # React Query provider
│   ├── components/       # React components
│   │   ├── ReportingDashboard.tsx
│   │   └── OrdersList.tsx
│   ├── hooks/            # Custom React hooks
│   │   └── useOrders.ts  # Orders data fetching hooks
│   ├── lib/              # Utilities
│   │   └── api-client.ts # Type-safe API client
│   └── types/            # TypeScript type definitions
│       └── api.ts        # API types matching .NET DTOs
├── package.json
├── tsconfig.json
└── next.config.js
```

## Key Concepts Demonstrated

### Server vs Client Components

- **Server Components** (`app/page.tsx`) - Rendered on server, no JavaScript bundle
- **Client Components** (`components/*.tsx`) - Interactive components with 'use client' directive

### React Query Patterns

- **useQuery** - For fetching data (orders, summaries)
- **useMutation** - For creating/updating data with optimistic updates
- **Query Invalidation** - Automatic cache refresh after mutations

### Type Safety

All API types are manually defined to match .NET DTOs, ensuring compile-time type safety across the full stack.

## Integration with .NET Backend

The API client is configured to communicate with the Orders.API running on `http://localhost:5000`. 

Update the base URL in `src/lib/api-client.ts` if your backend runs on a different port.

## Interview Talking Points

- **Performance**: Server Components reduce JavaScript bundle size
- **Type Safety**: Shared types between frontend and backend
- **State Management**: React Query handles server state, Context for client state
- **Error Handling**: Centralized error handling in API client interceptors
- **Optimistic Updates**: Better UX with instant feedback

