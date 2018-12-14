import { Component, OnInit, Input, OnChanges, EventEmitter, ViewChild, Output, SimpleChanges } from '@angular/core';
import { NotificationsService } from 'angular2-notifications';
import { CONFIG } from "../../../../services/config.service";
import { Util } from "../../../../util/util";
import { Constants } from "../../../../util/constants";
import { TradeService } from "../../../../services/trade.service";
import { AssetPositionResponse } from '../../../../model/advisor/advisorResponse';
import { OrdersTableComponent } from './orders-table/orders-table.component';
import { AdvisorService } from '../../../../services/advisor.service';
import { OrderResponse } from '../../../../model/trade/orderResponse';
import { ModalService } from '../../../../services/modal.service';
import { EventsService } from 'angular-event-service/dist';
import { Subscription } from 'rxjs';
import { AccountService } from '../../../../services/account.service';
import { MatTableDataSource, MatSort } from '@angular/material';


@Component({
  selector: 'open-positions',
  templateUrl: './open-positions.component.html',
  styleUrls: ['./open-positions.component.css']
})
export class OpenPositionsComponent implements OnInit, OnChanges {
  displayedColumns: string[] = [
    "asset",
    "units",    
    "value",
    "avgOpen",
    "PL",
    "close"
  ];

  @ViewChild(MatSort) sort: MatSort;
  dataSource = new MatTableDataSource<AssetPositionResponse>();

  utilProxy = Util;
  constantsProxy = Constants;
  expandedAssetId: number;
  orders: OrderResponse[] = [];
  promise: Subscription;
  @ViewChild("Orders") Orders: OrdersTableComponent;
  @Input() userId: number;
  @Input() assetId?: number = null;
  @Output() updated = new EventEmitter<void>();
  initialized: boolean = false;
  isOwner: boolean = false;

  constructor(private advisorService: AdvisorService,
    private accountService: AccountService,
    private modalService: ModalService,
    private tradeService: TradeService,
    private eventsService: EventsService,
    private notificationsService: NotificationsService) { }

  ngOnInit() {
    this.setIsOwner();
    if (this.assetId) {
      this.expandedAssetId = this.assetId;
      this.refresh();
    }
    this.initialized = true;
  }

  private setIsOwner() {
    let loginData = this.accountService.getLoginData();
    this.isOwner = !!loginData && loginData.id == this.userId;
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.initialized && ((!!changes.userId && changes.userId.previousValue != changes.userId.currentValue) ||
      (!!changes.assetId && changes.assetId.previousValue != changes.assetId.currentValue))) {
        if (!!changes.userId) {
          this.userId = changes.userId.currentValue;
          this.setIsOwner();
        }
        if (!!changes.assetId) {
          this.assetId = changes.assetId.currentValue;
        }
        this.setDataSourceElement([]);
        this.orders = [];
        if (this.assetId) {
          this.expandedAssetId = this.assetId;
        } else {
          this.expandedAssetId = null;
        }
        this.refresh();
    }
  }

  sortEvent() {
    this.orders = [];
    if (this.assetId) {
      this.expandedAssetId = this.assetId;
    } else {
      this.expandedAssetId = null;
    }
    this.refresh();
  }

  setDataSource(positionResponse: AssetPositionResponse[]) {
    let forceRefresh = !this.dataSource || !this.dataSource.data || this.dataSource.data.length != positionResponse.length;
    if (forceRefresh || (this.dataSource.data.length == 1 && this.dataSource.data[0].assetId != positionResponse[0].assetId)) {
      this.setDataSourceElement(positionResponse);
      if (this.assetId) {
        if (forceRefresh || this.expandedAssetId != this.assetId) {
          this.expandedAssetId = this.assetId;
          this.refresh();
        }
      } else {
        this.orders = [];
        this.expandedAssetId = null;
      }
    } else {
      for (let i = 0; i < positionResponse.length; ++i) {
        for (let j = 0; j < this.dataSource.data.length; ++j) {
          if (positionResponse[i].assetId == this.dataSource.data[j].assetId) {
            this.dataSource.data[j].positionResponse.averageReturn = positionResponse[i].positionResponse.averageReturn;
            this.dataSource.data[j].positionResponse.totalProfit = positionResponse[i].positionResponse.totalProfit;
            this.dataSource.data[j].positionResponse.totalVirtual = positionResponse[i].positionResponse.totalVirtual;
            if (this.dataSource.data[j].positionResponse.averagePrice != positionResponse[i].positionResponse.averagePrice ||
              this.dataSource.data[j].positionResponse.totalQuantity != positionResponse[i].positionResponse.totalQuantity ||
              this.dataSource.data[j].positionResponse.type != positionResponse[i].positionResponse.type) {
              this.dataSource.data[j].positionResponse.totalQuantity = positionResponse[i].positionResponse.totalQuantity;
              this.dataSource.data[j].positionResponse.averagePrice = positionResponse[i].positionResponse.averagePrice;
              this.dataSource.data[j].positionResponse.type = positionResponse[i].positionResponse.type;
              this.refresh();
            }
            break;
          }
        }
      }
    }
  }

  setDataSourceElement(positionResponse: AssetPositionResponse[]) {
    this.dataSource.data = positionResponse;
    if(!this.dataSource.sort){
      this.dataSource.sortingDataAccessor = Util.CustomSortingData;
      this.dataSource.sort = this.sort;
    }
  }
  
  getAssetImgUrl(id: number) {
    return CONFIG.assetImgUrl.replace("{id}", id.toString());
  }

  onAssetClick(assetId: number) {
    if (!this.assetId) {
      this.orders = [];
      if (this.expandedAssetId === assetId) {
        this.expandedAssetId = null;
      } else {
        this.expandedAssetId = assetId;
        this.refresh();
      }
    }
  }

  refresh(updateAll?: boolean) {
    if (this.expandedAssetId) {
      this.advisorService.getAdvisorOrders(this.userId, [Constants.OrderStatus.Executed], this.expandedAssetId).subscribe(result => {
        this.orders = result;
      });
    }
    if (updateAll) {
      this.updated.emit();
    }
  }

  closeAllOpenPositions(assetId: number, assetCode: string) {
    this.modalService.setConfirmDialog("Close all " + assetCode + " open positions", "Are you sure that you want to close all open positions?", "DON'T CLOSE", "OK").afterClosed().subscribe((ret) => 
    {
      if (ret)
      {
        this.promise = this.tradeService.closeAllOrders(assetId).subscribe(result => {
          this.eventsService.broadcast("onUpdateAdvisor", result);
          this.notificationsService.success(null, "Positions were closed.");
        });
      }
    });
  }

  hasOpenPositions() {
    return this.dataSource && this.dataSource.data && this.dataSource.data.length > 0;
  }

  onAssetSummaryClick($event: Event){
    $event.stopPropagation();
  }
}
