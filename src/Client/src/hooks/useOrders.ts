import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '@/lib/api-client';
import type { CreateOrderRequest, OrderDto, OrderSummary } from '@/types/api';

// Custom hook for fetching orders list
export function useOrders(page: number = 1, pageSize: number = 10) {
  return useQuery({
    queryKey: ['orders', page, pageSize],
    queryFn: () => apiClient.getOrders(page, pageSize),
    staleTime: 30000, // Consider data fresh for 30 seconds
    gcTime: 5 * 60 * 1000, // Keep in cache for 5 minutes
  });
}

// Custom hook for fetching single order
export function useOrder(orderId: string | null) {
  return useQuery({
    queryKey: ['order', orderId],
    queryFn: () => {
      if (!orderId) throw new Error('Order ID is required');
      return apiClient.getOrderById(orderId);
    },
    enabled: !!orderId, // Only fetch if orderId is provided
    staleTime: 60000, // Consider data fresh for 1 minute
  });
}

// Custom hook for orders summary (for reporting dashboard)
export function useOrdersSummary() {
  return useQuery({
    queryKey: ['orders', 'summary'],
    queryFn: () => apiClient.getOrdersSummary(),
    staleTime: 60000, // Refresh every minute for reporting data
    refetchInterval: 60000, // Auto-refetch every minute
  });
}

// Custom hook for creating orders with optimistic updates
export function useCreateOrder() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (request: CreateOrderRequest) => apiClient.createOrder(request),
    onMutate: async (newOrder) => {
      // Cancel outgoing refetches
      await queryClient.cancelQueries({ queryKey: ['orders'] });

      // Snapshot previous value for rollback
      const previousOrders = queryClient.getQueryData<OrderSummary[]>(['orders']);

      // Optimistically update cache
      if (previousOrders) {
        const optimisticOrder: OrderSummary = {
          id: 'temp-' + Date.now(),
          orderName: newOrder.order.orderName,
          customerId: newOrder.order.customerId,
          status: 0, // New
          totalAmount: newOrder.order.orderItems.reduce(
            (sum, item) => sum + item.price * item.quantity,
            0
          ),
          itemCount: newOrder.order.orderItems.length,
          createdAt: new Date().toISOString(),
        };

        queryClient.setQueryData<OrderSummary[]>(['orders'], (old) => [
          optimisticOrder,
          ...(old || []),
        ]);
      }

      return { previousOrders };
    },
    onError: (_err, _newOrder, context) => {
      // Rollback on error
      if (context?.previousOrders) {
        queryClient.setQueryData(['orders'], context.previousOrders);
      }
    },
    onSettled: () => {
      // Refetch to ensure consistency
      queryClient.invalidateQueries({ queryKey: ['orders'] });
    },
  });
}

