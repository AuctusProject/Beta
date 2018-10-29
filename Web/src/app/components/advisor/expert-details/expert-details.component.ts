import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Title, Meta } from '@angular/platform-browser';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { ActivatedRoute } from '@angular/router';
import { AdvisorService } from '../../../services/advisor.service';
import { AssetResponse } from '../../../model/asset/assetResponse';
import { CONFIG } from "../../../services/config.service";
import { Util } from '../../../util/Util';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { AccountService } from '../../../services/account.service';
import { AssetService } from '../../../services/asset.service';
import { ViewChildren, QueryList } from '@angular/core';
import { AssetHistoryChartComponent } from '../../asset/asset-history-chart/asset-history-chart.component';
import { ModalService } from '../../../services/modal.service';
import { NavigationService } from '../../../services/navigation.service';
import { ValueDisplayPipe } from '../../../util/value-display.pipe';

@Component({
  selector: 'expert-details',
  templateUrl: './expert-details.component.html',
  styleUrls: ['./expert-details.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0', display: 'none' })),
      state('expanded', style({ height: '*', display: 'table-row' })),
      transition('expanded <=> collapsed', animate('5ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})
export class ExpertDetailsComponent implements OnInit {
  expert: AdvisorResponse;
  showOwnerButton: boolean = false;
  displayedColumns: string[] = ['assetName', 'position', 'value', 'currentReturn', 'close', 'date', 'ratings', 'chevron'];
  displayedMobileColumns: string[] = ['assetName', 'position', 'date', 'chevron'];
  assets = [];
  isExpansionDetailRow = (i: number, row: Object) => row.hasOwnProperty('detailRow');
  isMobile = (i: number, row: Object) => true;
  expandedElement: any;

  @ViewChildren('assetHistoryChart') assetHistoryCharts:QueryList<AssetHistoryChartComponent>;

  constructor(private route: ActivatedRoute,  
    public accountService: AccountService,
    private advisorService: AdvisorService,
    private navigationService: NavigationService,
    private assetService: AssetService,
    private modalService: ModalService,
    private changeDetectorRefs: ChangeDetectorRef,
    private titleService: Title,
    private metaTagService: Meta) { }

  ngOnInit() {
    this.route.params.subscribe(params => 
      {
        let loginData = this.accountService.getLoginData();
        this.showOwnerButton = !!loginData && loginData.isAdvisor && loginData.id == params['id'];
        this.advisorService.getExpertDetails(params['id']).subscribe(expert => 
          {
            this.expert = expert;
            this.fillDataSource();
            this.titleService.setTitle("Auctus Experts - " + expert.name);
            this.metaTagService.updateTag({name: 'description', content: expert.name + " - " + expert.description});
          });
      }
    );
  }

  onNewAdviceClick() {
    this.modalService.setNewAdvice();
  }

  onEditProfileClick() {
    this.modalService.setEditAdvisor(this.accountService.getLoginData().id);
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
    this.assets = [];
    this.expert.assets.forEach(element => this.assets.push(element, { detailRow: true, element }));
    this.changeDetectorRefs.detectChanges();
  }

  getAssetImgUrl(asset: AssetResponse) {
    return CONFIG.assetImgUrl.replace("{id}", asset.assetId.toString());
  }

  goToAssetDetails(assetId: number, event: Event) {
    if (event) {
      event.stopPropagation();
    }
    this.navigationService.goToAssetDetails(assetId);
  }

  getLastAdviceTypeDescription(asset: AssetResponse){
    return Util.GetRecommendationTypeDescription(asset.assetAdvisor[0].lastAdviceType);
  }

  getLastAdviceTypeColor(asset: AssetResponse){
    return Util.GetRecommendationTypeColor(asset.assetAdvisor[0].lastAdviceType);
  }

  getLastAdviceDate(asset: AssetResponse){
    return asset.assetAdvisor[0].lastAdviceDate;
  }

  getFirstAdviceParametersDescription(asset: AssetResponse) {
    if (!asset.assetAdvisor[0].lastAdviceTargetPrice && asset.assetAdvisor[0].lastAdviceOperationType == 0) {
      return '';
    } else {
      if (asset.assetAdvisor[0].lastAdviceTargetPrice) {
        return 'Take profit: ' +  new ValueDisplayPipe().transform(asset.assetAdvisor[0].lastAdviceTargetPrice);
      } else if (asset.assetAdvisor[0].lastAdviceOperationType != 0 && asset.assetAdvisor[0].lastAdviceType == 2) {
        return 'Triggered by ' + Util.GetCloseReasonDescription(asset.assetAdvisor[0].lastAdviceOperationType);
      } else {
        return '';
      }
    }
  }

  getSecondAdviceParametersDescription(asset: AssetResponse) {
    if (!asset.assetAdvisor[0].lastAdviceStopLoss) {
      return '';
    } else {
      return 'Stop loss: ' +  new ValueDisplayPipe().transform(asset.assetAdvisor[0].lastAdviceStopLoss);
    }
  }

  getTotalRatings(asset:AssetResponse){
    return asset.assetAdvisor[0].totalRatings;
  }

  getAdviceMode(asset:AssetResponse){
    return Util.GetAdviceModeDescription(asset.assetAdvisor[0].lastAdviceMode);
  }

  onFollowClick(){
    if(this.accountService.hasInvestmentToCallLoggedAction()){
      this.advisorService.followAdvisor(this.expert.userId).subscribe(result =>
      {
        this.expert.following = true;
        this.expert.numberOfFollowers = this.expert.numberOfFollowers + 1;
      });
    }
  }

  onUnfollowClick(){
    this.advisorService.unfollowAdvisor(this.expert.userId).subscribe(result =>
      {
        this.expert.following = false;
        this.expert.numberOfFollowers = this.expert.numberOfFollowers - 1;
      });
  }

  closePosition(asset: AssetResponse) {
    this.modalService.setNewAdvice(asset.assetId, 2);
  }

  onFollowAssetClick(event: Event, asset: AssetResponse){
    if(this.accountService.hasInvestmentToCallLoggedAction()){
      this.assetService.followAsset(asset.assetId).subscribe(result =>
      {
        asset.following = true;
        asset.numberOfFollowers = asset.numberOfFollowers + 1;
      });
    }
    event.stopPropagation();
  }

  onUnfollowAssetClick(event: Event, asset: AssetResponse){
    this.assetService.unfollowAsset(asset.assetId).subscribe(result =>
      {
        asset.following = false;
        asset.numberOfFollowers = asset.numberOfFollowers - 1;
      });
    event.stopPropagation();
  }

  getInvestorsWordPluralOrSingular(){
    var word = "investor";
    if(this.expert.numberOfFollowers != 1)
      word += "s";
    return word;
  }

  noRecommendationsYet() {
    return this.assets != null && this.assets.length == 0;
  }

  getNoRecommendationMessage() {
    if(this.showOwnerButton)
      return "You haven't emit any signal yet.<br>Start now!";
    else
      return "This expert hasn't emit any signal yet.";
  }
}
