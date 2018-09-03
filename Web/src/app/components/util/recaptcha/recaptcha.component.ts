import { Component, OnInit, Output, EventEmitter } from '@angular/core';

declare var grecaptcha: any;

@Component({
  selector: 'recaptcha',
  templateUrl: './recaptcha.component.html',
  styleUrls: ['./recaptcha.component.css']
})
export class RecaptchaComponent implements OnInit {
  @Output() onCaptchaResponse = new EventEmitter<string>();
  
  constructor() { }
  ngOnInit() { }

  private resolved(response: string) {
    this.onCaptchaResponse.emit(response);
  }

  public reset() {
    grecaptcha.reset();
  }
}
