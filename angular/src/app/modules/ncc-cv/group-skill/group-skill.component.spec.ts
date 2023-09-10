import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupSkillComponent } from './group-skill.component';

describe('GroupSkillComponent', () => {
  let component: GroupSkillComponent;
  let fixture: ComponentFixture<GroupSkillComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GroupSkillComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupSkillComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
