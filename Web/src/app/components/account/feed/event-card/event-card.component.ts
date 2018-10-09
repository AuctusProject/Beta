import { Component, OnInit, Input } from '@angular/core';
import { FeedResponse } from '../../../../model/advisor/feedResponse';
import { CONFIG } from '../../../../services/config.service';
import { NavigationService } from '../../../../services/navigation.service';

@Component({
  selector: 'event-card',
  templateUrl: './event-card.component.html',
  styleUrls: ['./event-card.component.css']
})
export class EventCardComponent implements OnInit {
  @Input() eventFeed : FeedResponse;
  constructor(private navigationService : NavigationService) { }

  ngOnInit() {
  }

  
  getEventUrl(){
    return CONFIG.reportUrl.replace("{id}", this.eventFeed.event.id.toString());
  }

  getEventImgUrl(){
    return CONFIG.platformImgUrl.replace("{id}", "coinMarketCal");
  }

  getAssetImgUrl(){
    return CONFIG.assetImgUrl.replace("{id}", this.eventFeed.assetId.toString());
  }

  goToAssetDetails(){
    this.navigationService.goToAssetDetails(this.eventFeed.assetId);
  }
}
