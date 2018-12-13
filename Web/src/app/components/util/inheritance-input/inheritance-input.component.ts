import { Component, OnInit, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { InheritanceInputOptions, InputType } from '../../../model/inheritanceInputOptions';
import { FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'inheritance-input',
  templateUrl: './inheritance-input.component.html',
  styleUrls: ['./inheritance-input.component.css']
})
export class InheritanceInputComponent implements OnInit {
  @Input() options: InheritanceInputOptions;
  @Input() value: any;
  @Output() onChange: EventEmitter<any> = new EventEmitter<any>();

  public inputType: InputType = InputType.Text; 
  public required: boolean = true;
  public darkLayout: boolean = false;
  public disabled: boolean = false;
  public placeholder: string = "";
  public minlength: number = 0;
  public maxlength: number = 4000;
  public autocomplete: string = "off";
  public showValidatorError: boolean = true;
  public specificHint?: string;
  public showHintSize: boolean = true;
  public showPasswordVisibility: boolean = true;
  public outlineField: boolean = true;
  public autosizeTextArea: boolean = true;
  public minRows: number = 2;
  public maxRows: number = 4;
  public min: number = 0;
  public max: number = 99999;
  public step: number = 1;
  public suffixText: string = null;
  public formClass: string = "inheritance-input-form";
  public labelClass: string = "";
  public inputClass: string = "mat-input-element";
  public suffixClass: string = "inheritance-input-suffix";

  public formControl: FormControl; 
  public passwordHide: boolean = false;
  public forcedError: string;

  public enableCopy: boolean = false;
  copied: boolean = false;
  @ViewChild("Input") Input: ElementRef;

  constructor() { }

  ngOnInit() {
    if (!!this.options) {
      this.inputType = this.setValue(this.inputType, this.options.inputType);
      this.darkLayout = this.setValue(this.darkLayout, this.options.darkLayout);

      if (this.inputType == InputType.Password) {
        this.autocomplete = "current-password";
        this.passwordHide = true;
      }
      else if (this.options.inputType == InputType.Email) this.autocomplete = "email";

      if (!!this.options.textOptions) {
        this.required = this.setValue(this.required, this.options.textOptions.required);
        this.placeholder = this.options.textOptions.placeHolder + (this.required ? ' *' : '');
        this.disabled = this.setValue(this.disabled, this.options.textOptions.disabled);
        this.outlineField = this.setValue(this.outlineField, this.options.textOptions.outlineField);
        this.showValidatorError = this.setValue(this.showValidatorError, this.options.textOptions.showValidatorError);
        this.showHintSize = this.setValue(this.showHintSize, this.options.textOptions.showHintSize);
        this.showPasswordVisibility = this.setValue(this.showPasswordVisibility, this.options.textOptions.showPasswordVisibility);
        this.specificHint = this.options.textOptions.specificHint;
        this.maxlength = this.setValue(this.maxlength, this.options.textOptions.maxLength);
        this.minlength = this.setValue(this.minlength, this.options.textOptions.minLength);
        this.autocomplete = this.setValue(this.autocomplete, this.options.textOptions.browserAutocomplete);
        this.enableCopy = this.setValue(this.enableCopy, this.options.textOptions.enableCopy);
      }
      if (!!this.options.textAreaOptions) {
        this.autosizeTextArea = this.setValue(this.autosizeTextArea, this.options.textAreaOptions.autosize);
        this.minRows = this.setValue(this.minRows, this.options.textAreaOptions.minRows);
        this.maxRows = this.setValue(this.maxRows, this.options.textAreaOptions.maxRows);
      }
      if (!!this.options.numberOptions) {
        this.min = this.setValue(this.min, this.options.numberOptions.min);
        this.max = this.setValue(this.max, this.options.numberOptions.max);
        this.step = this.setValue(this.step, this.options.numberOptions.step);
      }
      
      this.suffixText = this.setValue(this.suffixText, this.options.suffixText);
      this.formClass = this.setValueClass(this.formClass, this.options.formClass);
      this.labelClass = this.setValueClass(this.labelClass, this.options.labelClass);
      this.inputClass = this.setValueClass(this.inputClass, this.options.inputClass);
      this.suffixClass = this.setValueClass(this.suffixClass, this.options.suffixClass);
    }

    let validators = [];
    if (this.showValidatorError) {
      validators.push(Validators.maxLength(this.maxlength));
      if (this.inputType == InputType.Email) validators.push(Validators.email);
      if (this.required) validators.push(Validators.required);
      if (this.minlength > 0) validators.push(Validators.minLength(this.minlength));
      if (this.inputType == InputType.Number) {
        validators.push(Validators.min(this.min));
        validators.push(Validators.max(this.max));
      }
    }
    this.formControl = new FormControl('', validators);
  }

  public onInputClick(){
    if(this.enableCopy) {
      this.Input.nativeElement.select();
    }
  }

  public getInputType() : string {
    if (this.passwordHide) {
      return "password";
    } else if (this.inputType == InputType.Search) {
      return "search";
    } else if (this.inputType == InputType.Email) {
      return "email";
    } else if (this.inputType == InputType.Number) {
      return "number";
    } else {
      return "text";
    }
  }

  public isValid() : boolean {
    this.formControl.markAsTouched();
    return !this.forcedError && (!this.showValidatorError || this.formControl.valid);
  }

  public setForcedError(errorMessage: string) : void {
    this.forcedError = errorMessage;
    let errorType = this.getErrorType();
    if (errorType) {
      let error = {'incorrect': true};
      error[errorType] = true;
      this.formControl.setErrors(error);
    } else {
      this.formControl.setErrors(null);
    }
    this.formControl.markAsTouched();
  }

  public clear() {
    this.value = null;
    this.formControl.setErrors(null);
    this.formControl.reset();
  }

  public forceValue(value: any) {
    this.value = value;
    this.formControl.markAsTouched();
  }

  public forceSuffixText(value: string) {
    this.suffixText = value;
  }

  public forceHint(hint: string) {
    this.specificHint = hint;
    this.formControl.markAsTouched();
  }

  public showTextAreaField() : boolean {
    return this.inputType == InputType.TextArea;
  }

  public showTextField() : boolean {
    return this.inputType == InputType.Text || this.inputType == InputType.Email || this.inputType == InputType.Password || this.inputType == InputType.Search || this.inputType == InputType.Number;
  }

  private setValue(defaultValue: any, optionValue?: any) : any {
    if (optionValue === undefined || optionValue === null) return defaultValue;
    else return optionValue;
  }

  private setValueClass(defaultValue: any, optionValue?: any) : any {
    let className = defaultValue;
    if (this.disabled) className += " inheritance-input-disabled";
    else {
      if (this.darkLayout) className += " layout-theme-dark";
      else className += " layout-theme-light"; 
    }
    if (optionValue === undefined || optionValue === null) return className;
    else return optionValue + " " + className;
  }

  public onChangeInput() : void {
    this.onChange.emit(this.value);
  }

  public getErrorMessage() : string {
    let getErrorType = this.getErrorType();
    if (getErrorType == 'forced') return this.forcedError;
    else if (getErrorType == 'required') return 'Field must be filled';
    else if (getErrorType == 'email') return 'It is not a valid email';
    else if (getErrorType == 'maxlength') return ('Maximum field length is ' + this.maxlength);
    else if (getErrorType == 'minlength') return ('Minimum field length is ' + this.minlength);
    else if (getErrorType == 'min') return ('The value cannot be lesser than ' + this.min);
    else if (getErrorType == 'max') return ('The value cannot be greater than ' + this.max);
    else return '';
  }

  private getErrorType() : string {
    if (!!this.forcedError) return 'forced';
    else if (!this.showValidatorError) return '';
    else if (this.required && this.formControl.hasError('required')) return 'required';
    else if (this.inputType == InputType.Email && this.formControl.hasError('email')) return 'email';
    else if (this.formControl.hasError('maxlength')) return 'maxlength';
    else if (this.minlength > 0 && this.formControl.hasError('minlength')) return 'minlength';
    else if (this.inputType == InputType.Number && this.formControl.hasError('min')) return 'min';
    else if (this.inputType == InputType.Number && this.formControl.hasError('max')) return 'max';
    else return '';
  }

  public getLength() : number {
    return (!!this.value) ? this.value.toString().length : 0;
  }

  public getHintText() : string {
    let hint = '';
    if (!!this.specificHint) hint += this.specificHint;
    if (this.showHintSize) hint += (this.getLength() + ' / ' + this.maxlength);
    return hint;
  }

  copyClick() {
    this.copied = true;
    this.Input.nativeElement.select();
    document.execCommand('copy');
    this.Input.nativeElement.setSelectionRange(0, 0);
  }
}
