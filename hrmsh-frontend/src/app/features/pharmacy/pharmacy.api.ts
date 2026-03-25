export interface ProductDto {
  id: number;
  code: string;
  name: string;
  genericName?: string | null;
  strength?: string | null;
  unit?: string | null;
  defaultSalePrice?: number | null;
  isActive: boolean;
}

export interface ProductListDto {
  id: number;
  code: string;
  name: string;
  genericName?: string | null;
  unit?: string | null;
  defaultSalePrice?: number | null;
  isActive: boolean;
}

export interface StockBatchDto {
  id: number;
  productId: number;
  batchNumber?: string | null;
  expiryDate?: string | null;
  unitCost?: number | null;
  quantityOnHand: number;
}

export type StockMovementType = 1 | 2 | 3 | 4; // Purchase, Adjustment, Sale, Return

export interface StockMovementDto {
  id: number;
  productId: number;
  stockBatchId?: number | null;
  type: StockMovementType;
  quantity: number;
  reason?: string | null;
  movementDate: string;
}

export interface PharmacyPurchaseInvoiceItemDto {
  id: number;
  productId: number;
  batchNumber?: string | null;
  expiryDate: string;
  quantity: number;
  unitPurchasePrice: number;
  lineTotal: number;
}

export interface PharmacyPurchaseInvoiceDto {
  id: number;
  invoiceNumber: string;
  invoiceDate: string;
  supplierName?: string | null;
  supplierReference?: string | null;
  totalAmount: number;
  paidAmount: number;
  status: InvoiceStatus;
  items: PharmacyPurchaseInvoiceItemDto[];
}

// Keep consistent naming with existing billing invoices.
export type InvoiceStatus = 'Draft' | 'Unpaid' | 'PartiallyPaid' | 'Paid' | 'Cancelled' | number;

export interface PagedApiResponse<T> {
  success: boolean;
  items: T[];
  totalCount: number;
}

