import { Component, OnInit, Input, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'countdown',
  templateUrl: './countdown.component.html',
  styleUrls: ['./countdown.component.css']
})
export class CountdownComponent implements OnInit {
  @Input() theme:string;
  seconds:string="00";
  minutes:string="00";
  hours:string="00";
  days:string="00";
  constructor(@Inject(PLATFORM_ID) private platformId: Object) { }

  ngOnInit() {
    this.startCountdown();
  }
  
  startCountdown(){
    if(isPlatformBrowser(this.platformId)){
      var self = this;
      setTimeout(function() {
        var difference = self.getTimeToEndOfMonth();
        if (difference >= 0) {
          var seconds = Math.floor(difference / 1000);
          var minutes = Math.floor(seconds / 60);
          var hours = Math.floor(minutes / 60);
          self.days = self.twoDigits(Math.floor(hours / 24));
          self.hours = self.twoDigits(hours % 24);
          self.minutes = self.twoDigits(minutes % 60);
          self.seconds = self.twoDigits(seconds % 60);
          self.startCountdown();
        }
      }, 1000);
    }
  }

  twoDigits(n){
    return n > 9 ? "" + n: "0" + n;
  }

  getTimeToEndOfMonth(){
    var today = new Date();
    return Date.UTC(today.getUTCFullYear(), today.getUTCMonth()+1,0) - today.getTime();
  }
}