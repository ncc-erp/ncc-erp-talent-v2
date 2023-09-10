import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FakeSkillComponent } from './fake-skill.component';

describe('FakeSkillComponent', () => {
  let component: FakeSkillComponent;
  let fixture: ComponentFixture<FakeSkillComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FakeSkillComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FakeSkillComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
