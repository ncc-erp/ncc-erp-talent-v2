import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'loopNumber'
})
export class LoopNumberPipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): unknown {
    let res = [];
    for (let i = 0; i < value; i++) {
      res.push(i);
    }
    return res;
  }

}
