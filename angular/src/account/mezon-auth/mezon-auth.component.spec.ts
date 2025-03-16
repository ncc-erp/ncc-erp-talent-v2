import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MezonAuthComponent } from './mezon-auth.component';

describe('MezonAuthComponent', () => {
  let component: MezonAuthComponent;
  let fixture: ComponentFixture<MezonAuthComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MezonAuthComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MezonAuthComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
