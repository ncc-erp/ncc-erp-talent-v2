import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FakeSkillDialogComponent } from './fake-skill-dialog.component';

describe('FakeSkillDialogComponent', () => {
  let component: FakeSkillDialogComponent;
  let fixture: ComponentFixture<FakeSkillDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FakeSkillDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FakeSkillDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
