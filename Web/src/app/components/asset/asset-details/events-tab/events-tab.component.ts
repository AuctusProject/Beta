import { Component, OnInit, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { CONFIG } from '../../../../services/config.service';
import { EventResponse } from '../../../../model/asset/eventResponse';

@Component({
  selector: 'events-tab',
  templateUrl: './events-tab.component.html',
  styleUrls: ['./events-tab.component.css']
})
export class EventsTabComponent implements OnInit {
  displayedColumns: string[] = ['eventDate', 'categories', 'creationDate', 'link'];
  @Input() events: EventResponse[] = [];
  visibleEvents: EventResponse[] = [];
  promise: Subscription;
  currentPage = 1;
  pageSize = 10;

  constructor() { }

  ngOnInit() {
    this.setVisibleEvents();
  }

  loadMoreEvents(){
    this.currentPage++;
    this.setVisibleEvents();
  }

  hasMoreEvents(){
    return this.events != null && this.visibleEvents.length != this.events.length;
  }

  setVisibleEvents(){
    var numberToShow = this.pageSize * this.currentPage;
    this.visibleEvents = this.events.slice(0, numberToShow);
  }

  onRowClick(row){
    
  }

  getEventUrl(report: EventResponse){
    return CONFIG.eventUrl.replace("{id}", report.eventId.toString());
  }

  getEventCategories(event: EventResponse){
    var categories = "";
    event.categories.forEach(category => {
      categories += category.name + ", "
    });
    return categories.substring(0, categories.length-2);
  }
}
