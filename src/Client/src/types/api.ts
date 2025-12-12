// TypeScript types matching .NET DTOs for type-safe API integration

export interface AddressDto {
  firstName: string;
  lastName: string;
  emailAddress: string;
  addressLine: string;
  country: string;
  state: string;
  zipCode: string;
}

export interface PaymentDto {
  cardName: string;
  cardNumber: string;
  expiration: string;
  cvv: string;
  paymentMethod: number;
}

export interface OrderItemDto {
  orderId: string;
  productId: string;
  quantity: number;
  price: number;
}

export enum OrderStatus {
  New = 0,
  Pending = 1,
  Processing = 2,
  Shipped = 3,
  Delivered = 4,
  Cancelled = 5,
}

export interface OrderDto {
  id: string;
  customerId: string;
  orderName: string;
  shippingAddress: AddressDto;
  billingAddress: AddressDto;
  payment: PaymentDto;
  status: OrderStatus;
  orderItems: OrderItemDto[];
}

export interface CreateOrderRequest {
  order: Omit<OrderDto, 'id' | 'status'>;
}

export interface CreateOrderResponse {
  id: string;
}

export interface OrderSummary {
  id: string;
  orderName: string;
  customerId: string;
  status: OrderStatus;
  totalAmount: number;
  itemCount: number;
  createdAt: string;
}

