import { Component } from '@angular/core';
import { FooterComponent } from '@coreui/angular';

@Component({
  selector: 'app-default-footer',
  standalone: true,
  imports: [],
  templateUrl: './defauld-footer.html',
  styleUrl: './defauld-footer.css',
})
export class DefaultFooterComponent extends FooterComponent {

  constructor() {
    super();
  }
}
