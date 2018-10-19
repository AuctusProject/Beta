import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { EventResponse } from '../../../model/asset/eventResponse';
import { CONFIG } from '../../../services/config.service';
import { AssetService } from '../../../services/asset.service';
import { FeedResponse } from '../../../model/advisor/feedResponse';

@Component({
  selector: 'events-list',
  templateUrl: './events-list.component.html',
  styleUrls: ['./events-list.component.css']
})
export class EventsListComponent implements OnInit, OnChanges {
  @Input() assetId: number;
  hasMore = false;
  pageSize = 20;
  events: FeedResponse[]=[];
  displayedColumns: string[] = ['title', 'date', 'proof'];
  
  constructor(private assetService: AssetService) { }

  ngOnInit() {
  }

  ngOnChanges() {
    this.events = null;
    this.loadEvents();
  }


  loadEvents(){
    this.assetService.getAssetsEvents(this.pageSize, this.getLastEventId(), this.assetId).subscribe(result => {
      if(this.events == null){
        this.events = [];
      }
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
