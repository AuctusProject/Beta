import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { OrderRequest } from '../model/trade/orderRequest';
import { OrderResponse } from '../model/trade/orderResponse';
import { EditOrderRequest } from '../model/trade/editOrderRequest';

@Injectable()
export class TradeService {
  private tradesUrl = this.httpService.apiUrl("v1/trades");
  private tradesCloseUrl = this.httpService.apiUrl("v1/trades/{id}/close");
  private tradesCancelUrl = this.httpService.apiUrl("v1/trades/{id}/cancel");
  private tradesEditStopLossUrl = this.httpService.apiUrl("v1/trades/{id}/stop_loss");
  private tradesEditTakeProfitUrl = this.httpService.apiUrl("v1/trades/{id}/take_profit");
  private tradesCancelAllUrl = this.httpService.apiUrl("v1/trades/cancel_all");
  private tradesCloseAllUrl = this.httpService.apiUrl("v1/trades/close_all");

  constructor(private httpService : HttpService) { }

  listOrders(userId:number, assetsIds:number[], orderStatusTypes:number[], orderType:number): Observable<OrderResponse[]>{
    return this.httpService.get(this.buildListOrdersUrl(userId,assetsIds,orderStatusTypes,orderType));
  }

  private buildListOrdersUrl(userId:number, assetsIds:number[], orderStatusTypes:number[], orderType:number){
    let result = this.tradesUrl + "?";
    result = result + "userId=" + userId.toString() + "&";

    if (assetsIds)
      assetsIds.forEach(e => result = result + "assetsIds=" + e.toString() + "&" )
    if (orderStatusTypes)
    orderStatusTypes.forEach(e => result = result + "ordersStatusTypes=" + e.toString() + "&" )
    if (orderType)
      result = result + "orderType=" + orderType.toString() + "&";
    return result.substr(0, result.length-1);
  }
    
  createOrder(assetId: number, type: number, amount: number, price?: number, stopLoss?: number, takeProfit?: number): Observable<OrderResponse> {
    let orderRequest = new OrderRequest();
    orderRequest.assetId = assetId;
    orderRequest.type = type;
    orderRequest.quantity = amount;
    orderRequest.price = price;
    orderRequest.stopLoss = stopLoss;
    orderRequest.takeProfit = takeProfit;
    return this.httpService.post(this.tradesUrl, orderRequest);
  }

  editOrder(orderId: number, amount: number, price?: number, stopLoss?: number, takeProfit?: number): Observable<OrderResponse> {
    let orderRequest = new EditOrderRequest();
    orderRequest.quantity = amount;
    orderRequest.price = price;
    orderRequest.stopLoss = stopLoss;
    orderRequest.takeProfit = takeProfit;
    return this.httpService.put(this.tradesUrl + "/" + orderId, orderRequest);
  }

  closeOrder(orderId: number, quantity?: number): Observable<OrderResponse> {
    return this.httpService.post(this.tradesCloseUrl.replace("{id}", orderId.toString()), { value: quantity });
  }

  cancelOrder(orderId: number): Observable<OrderResponse> {
    return this.httpService.post(this.tradesCancelUrl.replace("{id}", orderId.toString()), null);
  }

  editOrderTakeProfit(orderId: number, price?: number): Observable<OrderResponse> {
    return this.httpService.put(this.tradesEditTakeProfitUrl.replace("{id}", orderId.toString()), { value: price });
  }

  editOrderStopLoss(orderId: number, price?: number): Observable<OrderResponse> {
    return this.httpService.put(this.tradesEditStopLossUrl.replace("{id}", orderId.toString()), { value: price });
  }

  cancelAllOpenOrders(assetId?: number): Observable<OrderResponse[]> {
    let model = (assetId ? { assetId: assetId } : null);
    return this.httpService.post(this.tradesCancelAllUrl, model);
  }

  closeAllOrders(assetId: number): Observable<OrderResponse[]> {
    return this.httpService.post(this.tradesCloseAllUrl, { assetId: assetId });
  }
}
