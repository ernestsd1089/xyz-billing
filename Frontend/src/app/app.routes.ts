import { Routes } from '@angular/router';
import { OrderForm } from './features/orders/order-form/order-form';

export const routes: Routes = [
  { path: '', component: OrderForm },
  { path: '**', redirectTo: '' }
];
