import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CandidateOnboardListComponent } from './candidate-onboard-list.component';

describe('CandidateOnboardListComponent', () => {
  let component: CandidateOnboardListComponent;
  let fixture: ComponentFixture<CandidateOnboardListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CandidateOnboardListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CandidateOnboardListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
