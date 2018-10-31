import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ModalService } from '../../../services/modal.service';
import { NavigationService } from '../../../services/navigation.service';
import { LoginResponse } from '../../../model/account/loginResponse';
import { AdvisorResponse } from '../../../model/advisor/advisorResponse';
import { AdvisorService } from '../../../services/advisor.service';
import { Util } from '../../../util/Util';

@Component({
  selector: 'top-image',
  templateUrl: './top-image.component.html',
  styleUrls: ['./top-image.component.css']
})
export class TopImageComponent implements OnInit {
  @Input() urlImage: string; 
  @Input() title: string;
  @Input() subtitle: string;
  @Input() subtitle2: string;
  @Input() showLatestUpdates: boolean = false;
  @Input() loginData: LoginResponse;
  @Output() onAdviceCreated:EventEmitter<void> = new EventEmitter<void>();


  expert: AdvisorResponse;
  latestUpdates: string[] = [];

  public constructor(private modalService: ModalService, 
    private navigationService: NavigationService,
    private advisorService: AdvisorService) { }

  ngOnInit() {
    if (this.loginData) {
      this.advisorService.getExpertDetails(this.loginData.id.toString()).subscribe(expert => 
        {
          this.expert = expert;
          this.fillLatestUpdates();
        });
    }
  }

  onNewAdviceClick() {
    this.modalService.setNewAdvice().afterClosed().subscribe(() => this.onAdviceCreated.emit());
  }

  viewProfile() {
    if (this.loginData)
      this.navigationService.goToExpertDetails(this.loginData.id);
  }

  fillLatestUpdates() {
    if (this.expert) {
      this.latestUpdates.push("Your avarage return is " + this.getRoundedPercentage(this.expert.averageReturn) + "%");
      this.latestUpdates.push("You are ranked #" + this.expert.ranking + " out of " + this.expert.totalAdvisors);
      this.latestUpdates.push("Your success rate is " + this.getRoundedPercentage(this.expert.successRate) + "%");
      
      let bestCall = this.getBestCall();
      if (bestCall && bestCall.replace(/\s/g, '').length >= 1) {
        this.latestUpdates.push("Your best signal was to " + this.getBestCall());
      }
    }
  }

  getRoundedPercentage(rawNumber: number) : number{
    return Math.round(rawNumber * 10000) / 100;
  }

  getBestCall() : string {
    let assetCode: string = "";
    let bestReturn: number = -1;
    let recommendationType: string = "";

    if (this.expert.assets) {
      this.expert.assets.forEach(asset => {
        if(asset.assetAdvisor) {
          asset.assetAdvisor.forEach(assetAdvisor => {
            if (assetAdvisor.lastAdviceType != 2 && assetAdvisor.currentReturn > bestReturn) {
              bestReturn = assetAdvisor.currentReturn;
              assetCode = asset.code;
              recommendationType = Util.GetRecommendationTypeDescription(assetAdvisor.lastAdviceType).toLowerCase();
            }
          });
        }
      });
    }
    
    return recommendationType + " " + assetCode;
  }
}
