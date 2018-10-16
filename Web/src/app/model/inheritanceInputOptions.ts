export interface InheritanceInputOptions {
    inputType: InputType;
    darkLayout: boolean;
    textOptions?: InputTextTypeOptions;
    textAreaOptions?: InputTextAreaTypeOptions;
    numberOptions?: InputNumberTypeOptions;
}

export enum InputType {
    Text = 0,
    Email = 1,
    Password = 2,
    TextArea = 3,
    Search = 4,
    Number = 5
}

interface InputTypeOptions {
    placeHolder: string;
    required?: boolean;
    disabled?: boolean;
    showValidatorError?: boolean;
    specificHint?: string;
    outlineField?: boolean;
}

interface InputTextTypeOptions extends InputTypeOptions {
    minLength?: number;
    maxLength?: number;
    showHintSize?: boolean;
    browserAutocomplete?: string;
    showPasswordVisibility?: boolean;
}

interface InputTextAreaTypeOptions {
    autosize?: boolean;
    minRows?: number;
    maxRows?: number;
}

interface InputNumberTypeOptions {
    min?: number;
    max?: number;
    step?: number;
}
