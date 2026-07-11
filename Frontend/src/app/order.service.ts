import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';

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

@Injectable({ providedIn: 'root' })
export class OrderService {
  constructor(private readonly http: HttpClient) {}

  submitOrder(request: SubmitOrderRequest): Observable<ReceiptResponse> {
    return this.http.post<ReceiptResponse>('/api/orders', request).pipe(
      catchError((response: HttpErrorResponse) => throwError(() => this.toMessage(response)))
    );
  }

  private toMessage(response: HttpErrorResponse): string {
    const error = response.error;
    if (error?.message) {
      return error.message;
    }
    if (error?.errors?.length) {
      return error.errors.map((item: { errorMessage: string }) => item.errorMessage).join(' ');
    }
    return 'Something went wrong while submitting the order.';
  }
}
