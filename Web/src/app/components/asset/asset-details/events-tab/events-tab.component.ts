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
  displayedColumns: string[] = ['eventDate', 'categories', 'createdDate', 'link'];
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
    if(!this.events){
      this.events = 
      [{
        eventId:1,
        description:"A gathering of global blockchain leaders to showcase technology’s real-world implementation and forthcoming developments | 9-10th Oct, Bali.",
        categories:[{name:'Conference',id:5},{name:'Teste2',id:2}],
        createdDate:new Date(), 
        eventDate: new Date(), 
        title:'XBlockchain Summit'}];
    }
    this.visibleEvents = this.events.slice(0, numberToShow);
  }

  onRowClick(row){
    
  }

  getEventUrl(report: EventResponse){
    return CONFIG.reportUrl.replace("{id}", report.eventId.toString());
  }

  getEventCategories(event: EventResponse){
    var categories = "";
    event.categories.forEach(category => {
      categories += category.name + ", "
    });
    return categories.substring(0, categories.length-2);
  }
}
