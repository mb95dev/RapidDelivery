import axios, { AxiosInstance, AxiosError } from 'axios';
import type {
  CreateOrderRequest,
  CreateOrderResponse,
  OrderDto,
  OrderSummary,
} from '@/types/api';

// API client for type-safe communication with .NET backend
// Uses axios for HTTP requests with proper error handling

class ApiClient {
  private client: AxiosInstance;

  constructor(baseURL: string = 'http://localhost:5000') {
    this.client = axios.create({
      baseURL,
      headers: {
        'Content-Type': 'application/json',
      },
      timeout: 10000,
    });

    // Request interceptor for adding auth tokens if needed
    this.client.interceptors.request.use(
      (config) => {
        // Add JWT token if available
        const token = typeof window !== 'undefined' 
          ? localStorage.getItem('authToken') 
          : null;
        
        if (token) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        
        return config;
      },
      (error) => Promise.reject(error)
    );

    // Response interceptor for error handling
    this.client.interceptors.response.use(
      (response) => response,
      (error: AxiosError) => {
        // Handle common HTTP errors
        if (error.response) {
          switch (error.response.status) {
            case 401:
              // Handle unauthorized - redirect to login
              if (typeof window !== 'undefined') {
                window.location.href = '/login';
              }
              break;
            case 404:
              throw new Error('Resource not found');
            case 500:
              throw new Error('Server error. Please try again later.');
            default:
              throw new Error(
                error.response.data?.message || 'An unexpected error occurred'
              );
          }
        } else if (error.request) {
          throw new Error('Network error. Please check your connection.');
        }
        throw error;
      }
    );
  }

  // Create a new order
  async createOrder(request: CreateOrderRequest): Promise<CreateOrderResponse> {
    const response = await this.client.post<CreateOrderResponse>(
      '/orders',
      request
    );
    return response.data;
  }

  // Get order by ID
  async getOrderById(id: string): Promise<OrderDto> {
    const response = await this.client.get<OrderDto>(`/orders/${id}`);
    return response.data;
  }

  // Get all orders (with pagination support)
  async getOrders(
    page: number = 1,
    pageSize: number = 10
  ): Promise<OrderSummary[]> {
    const response = await this.client.get<OrderSummary[]>('/orders', {
      params: { page, pageSize },
    });
    return response.data;
  }

  // Get orders summary for reporting
  async getOrdersSummary(): Promise<{
    totalOrders: number;
    totalRevenue: number;
    ordersByStatus: Record<string, number>;
  }> {
    const response = await this.client.get('/orders/summary');
    return response.data;
  }
}

// Export singleton instance
export const apiClient = new ApiClient();

