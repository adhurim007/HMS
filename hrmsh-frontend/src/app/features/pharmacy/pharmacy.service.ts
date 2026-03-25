import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiService } from '../../core/services/api.service';
import {
  ProductDto,
  ProductListDto,
  PharmacyPurchaseInvoiceDto,
  StockBatchDto,
  StockMovementDto,
  StockMovementType,
  PagedApiResponse,
} from './pharmacy.api';
import { InvoiceDto } from '../billing/billing.api';

export interface ProductsQuery {
  page: number;
  pageSize: number;
  search?: string | null;
  isActive?: boolean | null;
  sortBy?: string | null;
  sortDescending?: boolean;
}

@Injectable({ providedIn: 'root' })
export class PharmacyService {
  constructor(private readonly api: ApiService) {}

  getProducts(query: ProductsQuery): Observable<PagedApiResponse<ProductListDto>> {
    const params: Record<string, string> = {
      page: String(query.page),
      pageSize: String(query.pageSize),
    };
    if (query.search) params['search'] = query.search;
    if (query.isActive != null) params['isActive'] = String(query.isActive);
    if (query.sortBy) params['sortBy'] = query.sortBy;
    if (query.sortDescending !== undefined) {
      params['sortDescending'] = String(query.sortDescending);
    }
    return this.api.get<PagedApiResponse<ProductListDto>>('Products', params);
  }

  getProduct(id: number): Observable<ProductDto> {
    return this.api
      .get<{ success?: boolean; data?: ProductDto; Data?: ProductDto }>(
        `Products/${id}`,
      )
      .pipe(map((x) => (x.data ?? x.Data)!));
  }

  createProduct(payload: {
    code: string;
    name: string;
    genericName?: string | null;
    strength?: string | null;
    unit?: string | null;
    defaultSalePrice?: number | null;
  }): Observable<ProductDto> {
    return this.api
      .post<{
        success?: boolean;
        data?: ProductDto;
        Data?: ProductDto;
      }>('Products', payload)
      .pipe(map((x) => (x.data ?? x.Data)!));
  }

  updateProduct(
    id: number,
    payload: {
      name: string;
      genericName?: string | null;
      strength?: string | null;
      unit?: string | null;
      defaultSalePrice?: number | null;
      isActive: boolean;
    },
  ): Observable<ProductDto> {
    return this.api
      .put<{
        success?: boolean;
        data?: ProductDto;
        Data?: ProductDto;
      }>(`Products/${id}`, {
        id,
        ...payload,
      })
      .pipe(map((x) => (x.data ?? x.Data)!));
  }

  deleteProduct(id: number): Observable<void> {
    return this.api
      .delete<{ success?: boolean }>(`Products/${id}`)
      .pipe(map(() => void 0));
  }

  getStockBatches(productId: number): Observable<StockBatchDto[]> {
    return this.api
      .get<{
        success?: boolean;
        data?: StockBatchDto[];
        Data?: StockBatchDto[];
      }>(`Stock/batches/product/${productId}`)
      .pipe(map((x) => (x.data ?? x.Data) ?? []));
  }

  createStockBatch(payload: {
    productId: number;
    batchNumber?: string | null;
    expiryDate?: string | null;
    quantity: number;
    unitCost?: number | null;
  }): Observable<StockBatchDto> {
    return this.api
      .post<{
        success?: boolean;
        data?: StockBatchDto;
        Data?: StockBatchDto;
      }>('Stock/batches', payload)
      .pipe(map((x) => (x.data ?? x.Data)!));
  }

  recordStockMovement(payload: {
    productId: number;
    stockBatchId?: number | null;
    type: StockMovementType;
    quantity: number;
    reason?: string | null;
    isIncreaseForAdjustment?: boolean;
  }): Observable<StockMovementDto> {
    return this.api
      .post<{
        success?: boolean;
        data?: StockMovementDto;
        Data?: StockMovementDto;
      }>('Stock/movements', payload)
      .pipe(map((x) => (x.data ?? x.Data)!));
  }

  createPurchaseInvoice(payload: {
    invoiceDate?: string | null;
    supplierName?: string | null;
    supplierReference?: string | null;
    paidAmount: number;
    items: {
      productId: number;
      batchNumber?: string | null;
      expiryDate: string;
      quantity: number;
      unitPurchasePrice: number;
    }[];
  }): Observable<PharmacyPurchaseInvoiceDto> {
    // Reuse the DTO type for client-side line structure, but backend expects input shape:
    // { productId, batchNumber, expiryDate, quantity, unitPurchasePrice }
    const items = payload.items.map((i) => ({
      productId: i.productId,
      batchNumber: i.batchNumber ?? null,
      expiryDate: i.expiryDate,
      quantity: i.quantity,
      unitPurchasePrice: i.unitPurchasePrice,
    }));

    return this.api
      .post<{
        success?: boolean;
        data?: PharmacyPurchaseInvoiceDto;
        Data?: PharmacyPurchaseInvoiceDto;
      }>('PharmacyPurchases', {
        invoiceDate: payload.invoiceDate ?? null,
        supplierName: payload.supplierName ?? null,
        supplierReference: payload.supplierReference ?? null,
        paidAmount: payload.paidAmount,
        items,
      })
      .pipe(map((x) => (x.data ?? x.Data)!));
  }

  createPharmacySale(payload: {
    patientId: number;
    items: { productId: number; quantity: number }[];
  }): Observable<InvoiceDto> {
    return this.api
      .post<{
        success?: boolean;
        data?: InvoiceDto;
        Data?: InvoiceDto;
      }>('PharmacySales/sell', payload)
      .pipe(map((x) => (x.data ?? x.Data)!));
  }
}

