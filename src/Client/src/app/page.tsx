import { ReportingDashboard } from '@/components/ReportingDashboard';
import { OrdersList } from '@/components/OrdersList';

// Main page component using Next.js App Router
// Demonstrates Server Components with Client Components for interactivity
export default function Home() {
  return (
    <main className="min-h-screen p-8">
      <div className="max-w-7xl mx-auto">
        <h1 className="text-4xl font-bold mb-8">RapidDelivery - Order Management</h1>
        
        <div className="mb-8">
          <ReportingDashboard />
        </div>

        <div>
          <h2 className="text-2xl font-semibold mb-4">Recent Orders</h2>
          <OrdersList />
        </div>
      </div>
    </main>
  );
}

