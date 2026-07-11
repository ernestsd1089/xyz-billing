import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { OrderService, ReceiptResponse, SubmitOrderRequest } from './order.service';

@Component({
  selector: 'app-root',
  imports: [FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly request: SubmitOrderRequest = {
    orderNumber: '',
    userId: '',
    amount: 0,
    paymentGatewayId: 'stripe',
    description: ''
  };

  protected readonly receipt = signal<ReceiptResponse | null>(null);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly submitting = signal(false);

  constructor(private readonly orders: OrderService) {}

  protected submit(): void {
    this.receipt.set(null);
    this.errorMessage.set(null);
    this.submitting.set(true);

    this.orders.submitOrder(this.request).subscribe({
      next: (receipt) => {
        this.receipt.set(receipt);
        this.submitting.set(false);
      },
      error: (message: string) => {
        this.errorMessage.set(message);
        this.submitting.set(false);
      }
    });
  }
}
