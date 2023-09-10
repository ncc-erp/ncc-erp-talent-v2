import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupSkillDialogComponent } from './group-skill-dialog.component';

describe('GroupSkillDialogComponent', () => {
  let component: GroupSkillDialogComponent;
  let fixture: ComponentFixture<GroupSkillDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GroupSkillDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupSkillDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
