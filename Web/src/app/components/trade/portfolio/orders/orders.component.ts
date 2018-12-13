import { Component, OnInit, Input, OnChanges, SimpleChanges, ViewChild } from "@angular/core";
import { NotificationsService } from 'angular2-notifications';
import { OrderResponse } from "../../../../model/trade/orderResponse";
import { CONFIG } from "../../../../services/config.service";
import { Util } from "../../../../util/util";
import { Constants } from "../../../../util/constants";
import { TradeService } from "../../../../services/trade.service";
import { AdvisorService } from "../../../../services/advisor.service";
import { PositionResponse } from "../../../../model/advisor/advisorResponse";
import { ModalService } from "../../../../services/modal.service";
import { EventsService } from "angular-event-service/dist";
import { ValueDisplayPipe } from "../../../../util/value-display.pipe";
import { MatSort, MatTableDataSource } from "@angular/material";
import { NavigationService } from "../../../../services/navigation.service";

@Component({
  selector: "orders",
  templateUrl: "./orders.component.html",
  styleUrls: ["./orders.component.css"]
})
export class OrdersComponent implements OnInit, OnChanges {
  displayedColumns: string[] = [
    "image",
    "asset",
    "units",
    "executeAt",
    "value",
    "SL",
    "TP",
    "SLTP",
    "edit",
    "cancel"
  ];
  
  @ViewChild(MatSort) sort: MatSort;
  dataSource = new MatTableDataSource<OrderResponse>();
  
  utilProxy = Util;
  @Input() userId: number;
  @Input() isOwner: boolean;
  @Input() assetId: number = null;
  @Input() assetCode: string = null;
  initialized: boolean = false;

  constructor(private advisorService: AdvisorService, 
    private tradeService: TradeService, 
    private notificationsService: NotificationsService,
    private modalService: ModalService,
    private eventsService: EventsService, 
    private navigationService: NavigationService) {
  }

  ngOnInit() {
    this.refresh();
    this.eventsService.on("onUpdateAdvisor", () => this.refresh());
    this.initialized = true;
  }

  ngOnChanges(changes: SimpleChanges) {
    if(this.initialized && ((!!changes.assetId && changes.assetId.previousValue != changes.assetId.currentValue) || 
      (!!changes.userId && changes.userId.previousValue != changes.userId.currentValue))) {
      if (!!changes.userId) {
        this.userId = changes.userId.currentValue;
      }
      if (!!changes.assetId) {
        this.assetId = changes.assetId.currentValue;
      }
      if (!!changes.assetCode) {
        this.assetCode = changes.assetCode.currentValue;
      }
      this.refresh();
    }
  }

  refresh() {
    this.advisorService.getAdvisorOrders(this.userId, [Constants.OrderStatus.Open], this.assetId).subscribe(result => {
      this.dataSource.data = result;
      if (!this.dataSource.sort) {
        this.dataSource.sort = this.sort;
      }
    });
  }

  getAssetImgUrl(id: number) {
    return CONFIG.assetImgUrl.replace("{id}", id.toString());
  }

  getUsdInOpenOrders(){
    let total:number = 0;
    if (this.dataSource)
    this.dataSource.data.forEach(order => {
      total += order.invested;
    });
    return total;
  }

  cancelAllOpenOrders() {
    let title = (this.assetId && this.assetCode ? "Cancel all " + this.assetCode + " open orders" : "Cancel all open orders");
    this.modalService.setConfirmDialog(title, "Are you sure that you want to cancel all open orders?", "DON'T CANCEL", "OK").afterClosed().subscribe((ret) => 
    {
      if (ret)
      {
        this.tradeService.cancelAllOpenOrders(this.assetId).subscribe(result=>{
          let success = true;
          result.forEach(element => {
            success = success && (element.status == Constants.OrderStatus.Canceled);
          });
          if (success){
            this.eventsService.broadcast("onUpdateAdvisor", result);
            this.notificationsService.success(null, "All open orders were canceled");
          }
          else{
            this.notificationsService.error(null, "Orders could not be canceled");
          }
        });
      }
    });
  }

  cancelOrder(id: number){
    this.modalService.setConfirmDialog("Cancel open order", "Are you sure that you want to cancel this open order?", "DON'T CANCEL", "OK").afterClosed().subscribe((ret) => 
    {
      if (ret)
      {
        this.tradeService.cancelOrder(id).subscribe(result => {
          if (result.status == Constants.OrderStatus.Canceled){
            this.eventsService.broadcast("onUpdateAdvisor", [result]);
            this.notificationsService.success(null, "Order was canceled.");
          }
          else{
            this.notificationsService.error(null, "Order could not be canceled");
          }
        });
      }
    });
  }

  editOrder(order: OrderResponse){
    this.modalService.setEditOrderDialog(order);
  }

  onAssetClick(assetId: number) {
    this.navigationService.goToAssetDetails(assetId);
  }

  hasOpenOrders() {
    return this.dataSource && this.dataSource.data && this.dataSource.data.length > 0;
  }
}
