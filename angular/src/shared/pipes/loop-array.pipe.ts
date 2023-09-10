import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'loopArray'
})
export class LoopArrayPipe implements PipeTransform {

  transform(arr: Array<number>, ...args: unknown[]): unknown {
    let res = [];
    for (let i = 0; i < arr.length; i++) {
      res.push({ value: arr[i] });
    }
    return res;
  }

}
