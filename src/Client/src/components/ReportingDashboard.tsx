'use client';

import { useOrdersSummary } from '@/hooks/useOrders';
import { OrderStatus } from '@/types/api';

// Reporting Dashboard Component
// Demonstrates React Query for server state management
// Relevant to their fuel economy reporting system requirements
export function ReportingDashboard() {
  const { data, isLoading, error } = useOrdersSummary();

  if (isLoading) {
    return (
      <div className="bg-white dark:bg-gray-800 rounded-lg shadow p-6">
        <p className="text-gray-600">Loading dashboard data...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-red-50 dark:bg-red-900/20 rounded-lg shadow p-6">
        <p className="text-red-600 dark:text-red-400">
          Error loading dashboard: {error.message}
        </p>
      </div>
    );
  }

  if (!data) {
    return null;
  }

  const statusLabels: Record<number, string> = {
    [OrderStatus.New]: 'New',
    [OrderStatus.Pending]: 'Pending',
    [OrderStatus.Processing]: 'Processing',
    [OrderStatus.Shipped]: 'Shipped',
    [OrderStatus.Delivered]: 'Delivered',
    [OrderStatus.Cancelled]: 'Cancelled',
  };

  return (
    <div className="bg-white dark:bg-gray-800 rounded-lg shadow-lg p-6">
      <h2 className="text-2xl font-bold mb-6">Orders Summary</h2>
      
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
        <div className="bg-blue-50 dark:bg-blue-900/20 p-4 rounded-lg">
          <h3 className="text-sm font-medium text-gray-600 dark:text-gray-400 mb-1">
            Total Orders
          </h3>
          <p className="text-3xl font-bold text-blue-600 dark:text-blue-400">
            {data.totalOrders.toLocaleString()}
          </p>
        </div>

        <div className="bg-green-50 dark:bg-green-900/20 p-4 rounded-lg">
          <h3 className="text-sm font-medium text-gray-600 dark:text-gray-400 mb-1">
            Total Revenue
          </h3>
          <p className="text-3xl font-bold text-green-600 dark:text-green-400">
            ${data.totalRevenue.toLocaleString(undefined, {
              minimumFractionDigits: 2,
              maximumFractionDigits: 2,
            })}
          </p>
        </div>

        <div className="bg-purple-50 dark:bg-purple-900/20 p-4 rounded-lg">
          <h3 className="text-sm font-medium text-gray-600 dark:text-gray-400 mb-1">
            Active Orders
          </h3>
          <p className="text-3xl font-bold text-purple-600 dark:text-purple-400">
            {Object.entries(data.ordersByStatus)
              .filter(([status]) => 
                parseInt(status) !== OrderStatus.Delivered && 
                parseInt(status) !== OrderStatus.Cancelled
              )
              .reduce((sum, [, count]) => sum + count, 0)}
          </p>
        </div>
      </div>

      <div className="mt-6">
        <h3 className="text-lg font-semibold mb-4">Orders by Status</h3>
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4">
          {Object.entries(data.ordersByStatus).map(([status, count]) => (
            <div
              key={status}
              className="bg-gray-50 dark:bg-gray-700 p-3 rounded-lg text-center"
            >
              <p className="text-sm text-gray-600 dark:text-gray-400">
                {statusLabels[parseInt(status)] || `Status ${status}`}
              </p>
              <p className="text-2xl font-bold mt-1">{count}</p>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

