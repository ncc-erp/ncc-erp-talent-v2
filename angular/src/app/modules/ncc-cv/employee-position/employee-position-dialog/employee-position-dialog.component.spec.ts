import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeePositionDialogComponent } from './employee-position-dialog.component';

describe('EmployeePositionDialogComponent', () => {
  let component: EmployeePositionDialogComponent;
  let fixture: ComponentFixture<EmployeePositionDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EmployeePositionDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeePositionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
