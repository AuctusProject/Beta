import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'valueDisplay'})
export class ValueDisplayPipe implements PipeTransform {
  transform(value: number, currencyPrefix: string = '$'): string {
    if (!currencyPrefix) {
      currencyPrefix = '';
    }
    if (value == 0) {
      return currencyPrefix + value.toFixed(2);
    } else if(!value){
      return null;
    }
    var decimals = 1;
    var currentValue = 0;
    while(Math.abs(currentValue) <= 1 &&  decimals < 8){
        decimals++; 
        currentValue = value * Math.pow(10, decimals - 3);
    }

    var result = currencyPrefix + value.toLocaleString(undefined, { minimumFractionDigits: decimals, maximumFractionDigits: decimals });
    
    if (currentValue < 0){
        result = "-" + result.replace("-","");
    }
    
    return result;
  }
}