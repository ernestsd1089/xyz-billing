export interface SubmitOrderRequest {
  orderNumber: string;
  userId: string;
  amount: number;
  paymentGatewayId: string;
  description?: string;
}

export interface ReceiptResponse {
  orderNumber: string;
  amount: number;
  timestamp: string;
  confirmationId: string;
  status: string;
}
