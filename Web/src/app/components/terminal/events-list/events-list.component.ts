import { Component, OnInit, Input } from '@angular/core';
import { EventResponse } from 'src/app/model/asset/eventResponse';
import { CONFIG } from 'src/app/services/config.service';
import { AssetService } from 'src/app/services/asset.service';
import { FeedResponse } from 'src/app/model/advisor/feedResponse';

@Component({
  selector: 'events-list',
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.css']
})
export class EventsListComponent implements OnInit {
  @Input() assetId: number;
  hasMore = false;
  pageSize = 20;
  events: FeedResponse[]=[];
  displayedColumns: string[] = ['title', 'date', 'proof'];
  
  constructor(private assetService: AssetService) { }

  ngOnInit() {
    this.loadEvents();
  }

  loadEvents(){
    this.assetService.getAssetsEvents(this.pageSize, this.getLastEventId(), this.assetId).subscribe(result => {
      this.events = this.events.concat(result);
      this.hasMore = true;
      if(!result || result.length == 0 || result.length < this.pageSize){
        this.hasMore = false;
      }
    });
  }

  getLastEventId() {
    if(this.events != null && this.events.length > 0){
      return this.events[this.events.length-1].event.eventId;
    }
    return null;
  }

  onScroll() {
    this.loadEvents();
  }
  
  getProofLink(report: EventResponse) : string{
    return CONFIG.eventUrl.replace("{id}", report.eventId.toString());
  }
}
