import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MessengerImportInfoComponent } from './messenger-import-info.component';

describe('MessengerImportInfoComponent', () => {
  let component: MessengerImportInfoComponent;
  let fixture: ComponentFixture<MessengerImportInfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MessengerImportInfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MessengerImportInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
