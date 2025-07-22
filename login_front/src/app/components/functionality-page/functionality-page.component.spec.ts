import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FunctionalityPageComponent } from './functionality-page.component';

describe('FunctionalityPageComponent', () => {
  let component: FunctionalityPageComponent;
  let fixture: ComponentFixture<FunctionalityPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FunctionalityPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FunctionalityPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
