import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { InheritanceInputOptions, InputType } from '../../../model/inheritanceInputOptions';
import { FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'inheritance-input',
  templateUrl: './inheritance-input.component.html',
  styleUrls: ['./inheritance-input.component.css'],
  styles: ['width: 100%;']
})
export class InheritanceInputComponent implements OnInit {
  @Input() options: InheritanceInputOptions;
  @Input() forcedError: string;
  @Input() value: string;
  @Output() onChange: EventEmitter<string> = new EventEmitter<string>();

  private inputType: InputType = InputType.Text; 
  private required: boolean = true;
  private disabled: boolean = false;
  private placeholder: string = "";
  private minlength: number = 0;
  private maxlength: number = 4000;
  private autocomplete: string = "";
  private showValidatorError: boolean = true;
  private specificHint?: string;
  private showHintSize: boolean = true;
  private showPasswordVisibility: boolean = true;
  private autosizeTextArea: boolean = true;
  private minRows: number = 2;
  private maxRows: number = 4;

  private formControl: FormControl; 
  private passwordHide: boolean = false;

  constructor() { }

  ngOnInit() {
    if (!!this.options) {
      this.inputType = this.setValue(this.inputType, this.options.inputType);

      if (this.options.inputType == InputType.Password) {
        this.autocomplete = "current-password";
        this.passwordHide = true;
      }
      else if (this.options.inputType == InputType.Email) this.autocomplete = "email";

      if (!!this.options.textOptions) {
        this.required = this.setValue(this.required, this.options.textOptions.required);
        this.placeholder = this.options.textOptions.placeHolder + (this.required ? ' *' : '');
        this.disabled = this.setValue(this.disabled, this.options.textOptions.disabled);
        this.showValidatorError = this.setValue(this.showValidatorError, this.options.textOptions.showValidatorError);
        this.showHintSize = this.setValue(this.showHintSize, this.options.textOptions.showHintSize);
        this.showPasswordVisibility = this.setValue(this.showPasswordVisibility, this.options.textOptions.showPasswordVisibility);
        this.specificHint = this.options.textOptions.specificHint;
        this.maxlength = this.setValue(this.maxlength, this.options.textOptions.maxLength);
        this.minlength = this.setValue(this.minlength, this.options.textOptions.minLength);
        this.autocomplete = this.setValue(this.autocomplete, this.options.textOptions.browserAutocomplete);
      }
      if (!!this.options.textAreaOptions) {
        this.autosizeTextArea = this.setValue(this.autosizeTextArea, this.options.textAreaOptions.autosize);
        this.minRows = this.setValue(this.minRows, this.options.textAreaOptions.minRows);
        this.maxRows = this.setValue(this.maxRows, this.options.textAreaOptions.maxRows);
      }
    }

    let validators = [];
    if (this.showValidatorError) {
      validators.push(Validators.maxLength(this.maxlength));
      if (this.inputType == InputType.Email) validators.push(Validators.email);
      if (this.required) validators.push(Validators.required);
      if (this.minlength > 0) validators.push(Validators.minLength(this.minlength));
    }
    this.formControl = new FormControl('', validators);
  }

  public isValid() : boolean {
    this.formControl.markAsTouched();
    return !this.forcedError && (!this.showValidatorError || this.formControl.valid);
  }

  private showTextAreaField() : boolean {
    return this.inputType == InputType.TextArea;
  }

  private showTextField() : boolean {
    return this.inputType == InputType.Text || this.inputType == InputType.Email || this.inputType == InputType.Password;
  }

  private setValue(defaultValue: any, optionValue?: any) : any {
    if (optionValue === undefined || optionValue === null) return defaultValue;
    else return optionValue;
  }

  private onChangeInput() : void {
    this.onChange.emit(this.value);
  }

  private getErrorMessage() : string {
    if (!!this.forcedError) return this.forcedError;
    if (!this.showValidatorError) return '';
    if (this.required && this.formControl.hasError('required')) return 'Field must be filled';
    if (this.inputType == InputType.Email && this.formControl.hasError('email')) return 'It is not a valid email';
    if (this.formControl.hasError('maxlength')) return ('Maximum field length is ' + this.maxlength);
    if (this.minlength > 0 && this.formControl.hasError('minlength')) return ('Minimum field length is ' + this.minlength);
    return '';
  }

  private getLength() : number {
    return (!!this.value) ? this.value.length : 0;
  }

  private getHintText() : string {
    let hint = '';
    if (!!this.specificHint) hint += this.specificHint;
    if (this.showHintSize) hint += (this.getLength() + ' / ' + this.maxlength);
    return hint;
  }
}
