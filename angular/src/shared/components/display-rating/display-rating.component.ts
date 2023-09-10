import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'talent-display-rating',
  templateUrl: './display-rating.component.html',
  styleUrls: ['./display-rating.component.scss']
})
export class DisplayRatingComponent implements OnInit {

  @Input() value: number = 0;

  constructor() { }

  ngOnInit(): void { }

  getStarColor(value) {
    switch (value) {
      case 1: case 2: return 'pi-star-fill pi-star-fill--grey';
      case 3: return 'pi-star-fill pi-star-fill--gold'
      case 4: return 'pi-star-fill pi-star-fill--orange'
      case 5: return 'pi-star-fill pi-star-fill--red'
      default: return ''
    }
  }

}
