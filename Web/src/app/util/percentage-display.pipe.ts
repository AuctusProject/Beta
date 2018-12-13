import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'percentageDisplay'})
export class PercentageDisplayPipe implements PipeTransform {
  transform(value: number, decimals: number = 2): string {
    let zeros = '';
    for (let i = 0; i < decimals; i++) {
      zeros += '0';
    }
    let multiplier = parseInt('1' + zeros);
    return (Math.round(value * 100 * multiplier) / multiplier).toLocaleString(undefined, { minimumFractionDigits: zeros.length, maximumFractionDigits: zeros.length }) + '%';
  }
}