export interface InheritanceInputOptions {
    inputType: InputType;
    textOptions?: InputTextTypeOptions;
    textAreaOptions?: InputTextAreaTypeOptions;
}

export enum InputType {
    Text = 0,
    Email = 1,
    Password = 2,
    TextArea = 3
}

interface InputTypeOptions {
    placeHolder: string;
    required?: boolean;
    disable?: boolean;
    showValidatorError?: boolean;
    setFocus?: boolean;
    specificHint?: string;
}

interface InputTextTypeOptions extends InputTypeOptions {
    minLength?: number;
    maxLength?: number;
    showHintSize?: boolean;
    browserAutocomplete?: string;
}

interface InputTextAreaTypeOptions {
    autosize?: boolean;
    minRows?: number;
    maxRows?: number;
}

