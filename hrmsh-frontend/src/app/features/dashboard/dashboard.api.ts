export interface PagedApiResponse<T> {
  success?: boolean;
  items?: T[];
  Items?: T[];
  totalCount?: number;
  TotalCount?: number;
}

export interface ApiResponse<T> {
  success?: boolean;
  message?: string | null;
  data?: T | null;
  Data?: T | null;
}

export interface DailyPaymentRow {
  date: string;
  totalAmount: number;
  paymentCount: number;
}

export interface StockExpiryAlertRow {
  batchId: number;
  productId: number;
  productCode: string;
  productName: string;
  batchNumber?: string | null;
  expiryDate?: string | null;
  quantityOnHand: number;
  daysUntilExpiry?: number | null;
}

