import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResumeGenerateComponent } from './resume-generate.component';

describe('ResumeGenerateComponent', () => {
  let component: ResumeGenerateComponent;
  let fixture: ComponentFixture<ResumeGenerateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ResumeGenerateComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ResumeGenerateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
