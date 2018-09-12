import { Component, OnInit } from '@angular/core';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { AdvisorService } from '../../../services/advisor.service';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { CONFIG } from "../../../services/config.service";
import { Util } from '../../../util/Util';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { DataSource } from '@angular/cdk/table';
import { Observable, of } from 'rxjs';
import { MatDialog } from '@angular/material';
import { AccountService } from '../../../services/account.service';
import { FullscreenModalComponentInput } from '../../../model/modal/fullscreenModalComponentInput';
import { NewAdviceComponent } from '../new-advice/new-advice.component';
import { FullscreenModalComponent } from '../../util/fullscreen-modal/fullscreen-modal.component';
import { AdvisorEditComponent } from '../advisor-edit/advisor-edit.component';

@Component({
  selector: 'expert-details',
  templateUrl: './expert-details.component.html',
  styleUrls: ['./expert-details.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0', visibility: 'hidden' })),
      state('expanded', style({ height: '*', visibility: 'visible' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class ExpertDetailsComponent implements OnInit {
  expert: AdvisorResponse;
  showOwnerButton: boolean = false;
  displayedColumns: string[] = ['assetName', 'position', 'action', 'date', 'ratings', 'followButton'];
  assets = [];
  isExpansionDetailRow = (i: number, row: Object) => row.hasOwnProperty('detailRow');
  expandedElement: any;

  constructor(private route: ActivatedRoute,  
    public dialog: MatDialog, 
    public accountService: AccountService,
    private advisorService: AdvisorService) { }

  ngOnInit() {
    this.route.params.subscribe(params => 
      {
        let loginData = this.accountService.getLoginData();
        this.showOwnerButton = !!loginData && loginData.isAdvisor && loginData.id == params['id'];
        this.advisorService.getExpertDetails(params['id']).subscribe(expert => 
          {
            this.expert = expert;
            this.fillDataSource();
          });
      }
    );
  }

  onNewAdviceClick() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = NewAdviceComponent;
    this.dialog.open(FullscreenModalComponent, { data: modalData }); 
  }

  onEditProfileClick() {
    let modalData = new FullscreenModalComponentInput();
    modalData.component = AdvisorEditComponent;
    this.dialog.open(FullscreenModalComponent, { data: modalData }); 
  }

  onRowClick(row){
    if(this.expandedElement == row){
      this.expandedElement = null;
    }
    else{
      this.expandedElement = row;
    }
  }

  fillDataSource(){
    this.expert.assets.forEach(element => this.assets.push(element, { detailRow: true, element }));
  }

  getAssetImgUrl(asset: AssetResponse){
    return CONFIG.assetImgUrl.replace("{id}", asset.assetId.toString());
  }

  getLastAdviceTypeDescription(asset: AssetResponse){
    return Util.GetRecommendationTypeDescription(asset.assetAdvisor[0].lastAdviceType);
  }

  getLastAdviceDate(asset: AssetResponse){
    return asset.assetAdvisor[0].lastAdviceDate;
  }

  getTotalRatings(asset:AssetResponse){
    return asset.assetAdvisor[0].totalRatings;
  }

  getAdviceMode(asset:AssetResponse){
    return Util.GetAdviceModeDescription(asset.assetAdvisor[0].lastAdviceMode);
  }
}