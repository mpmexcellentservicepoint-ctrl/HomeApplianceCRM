import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
    <nav>
      <a routerLink="/jobs" routerLinkActive="active">Jobs</a>
      <a routerLink="/inventory" routerLinkActive="active">Inventory</a>
    </nav>
    <router-outlet></router-outlet>
  `,
  styles: [`
    nav { margin-bottom: 1em; }
    a { margin-right: 1em; }
    .active { font-weight: bold; }
  `]
})
export class AppComponent {}
