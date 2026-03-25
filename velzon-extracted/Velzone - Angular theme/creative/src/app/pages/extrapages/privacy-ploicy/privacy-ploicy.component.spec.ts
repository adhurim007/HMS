import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrivacyPloicyComponent } from './privacy-ploicy.component';

describe('PrivacyPloicyComponent', () => {
  let component: PrivacyPloicyComponent;
  let fixture: ComponentFixture<PrivacyPloicyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PrivacyPloicyComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrivacyPloicyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
