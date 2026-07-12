import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../order.service';
import { ReceiptResponse, SubmitOrderRequest } from '../order.models';

@Component({
  selector: 'app-order-form',
  imports: [FormsModule],
  templateUrl: './order-form.html',
  styleUrl: './order-form.css'
})
export class OrderForm {
  private readonly submittedOrders = new Set<string>();

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
  protected readonly wasAlreadyProcessed = signal(false);

  constructor(private readonly orders: OrderService) {}

  protected submit(): void {
    this.receipt.set(null);
    this.errorMessage.set(null);
    this.wasAlreadyProcessed.set(false);
    this.submitting.set(true);

    const orderNumber = this.request.orderNumber;
    const isResubmission = this.submittedOrders.has(orderNumber);

    this.orders.submitOrder(this.request).subscribe({
      next: (receipt) => {
        this.receipt.set(receipt);
        this.wasAlreadyProcessed.set(isResubmission);
        this.submittedOrders.add(orderNumber);
        this.submitting.set(false);
      },
      error: (message: string) => {
        this.errorMessage.set(message);
        this.submitting.set(false);
      }
    });
  }
}
