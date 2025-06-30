import { Component, EventEmitter, Input, Output } from '@angular/core';
import { StageFilters } from '@app/modules/report/report-education/interfaces/report-education.interface';

@Component({
  selector: 'stage-filter',
  template: `
    <div class="stage-filters p-3 bg-light rounded">
      <div class="mb-3">
        <strong>{{ "Display recruitment statuses:" | localize }}</strong>
      </div>
      
      <div class="row">
        <div class="col-6 col-md-4 col-lg-2">
          <p-checkbox 
            inputId="filterPassCV"
            [(ngModel)]="filters.passCV"
            [binary]="true"
            (onChange)="onFilterChange()">
          </p-checkbox>
          <label for="filterPassCV" class="ml-2">
            {{ "Pass CV" | localize }}
          </label>
        </div>
        
        <div class="col-6 col-md-4 col-lg-2">
          <p-checkbox 
            inputId="filterPassTest"
            [(ngModel)]="filters.passTest"
            [binary]="true"
            (onChange)="onFilterChange()">
          </p-checkbox>
          <label for="filterPassTest" class="ml-2">
            {{ "Pass Test" | localize }}
          </label>
        </div>
        
        <div class="col-6 col-md-4 col-lg-2">
          <p-checkbox 
            inputId="filterPassInterview"
            [(ngModel)]="filters.passInterview"
            [binary]="true"
            (onChange)="onFilterChange()">
          </p-checkbox>
          <label for="filterPassInterview" class="ml-2">
            {{ "Pass Interview" | localize }}
          </label>
        </div>
        
        <div class="col-6 col-md-4 col-lg-2">
          <p-checkbox 
            inputId="filterOther"
            [(ngModel)]="filters.other"
            [binary]="true"
            (onChange)="onFilterChange()">
          </p-checkbox>
          <label for="filterOther" class="ml-2">
            {{ "Other" | localize }}
          </label>
        </div>
        
        <div class="col-6 col-md-4 col-lg-2">
          <p-checkbox 
            inputId="filterOnboard"
            [(ngModel)]="filters.onboard"
            [binary]="true"
            (onChange)="onFilterChange()">
          </p-checkbox>
          <label for="filterOnboard" class="ml-2">
            {{ "Onboard" | localize }}
          </label>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .stage-filters {
      border: 1px solid #dee2e6;
      
      label {
        font-size: 0.9rem;
        cursor: pointer;
        user-select: none;
      }
    }
    
    @media (max-width: 768px) {
      .row > div {
        margin-bottom: 0.5rem;
      }
    }
  `]
})
export class StageFilterComponent {
  @Input() filters: StageFilters = {
    passCV: true,
    passTest: true,
    passInterview: true,
    other: true,
    onboard: true
  };
  
  @Output() filtersChange = new EventEmitter<StageFilters>();
  
  onFilterChange(): void {
    this.filtersChange.emit(this.filters);
  }
}