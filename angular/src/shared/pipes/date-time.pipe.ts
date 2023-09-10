import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';

@Pipe({
  name: 'talentDateTime'
})
export class DateTimePipe implements PipeTransform {

  transform(value: Date | moment.Moment, dateFormat: string): string {
    return moment(value).format(dateFormat);
  }
}
