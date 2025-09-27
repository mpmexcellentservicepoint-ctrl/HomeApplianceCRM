import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { JobsModule } from './jobs/jobs.module';
import { JobListComponent } from './jobs/job-list.component';
import { JobDetailComponent } from './jobs/job-detail.component';
import { InventoryListComponent } from './inventory-list.component';
import { InventoryFormComponent } from './inventory-form.component';
import { AppComponent } from './app.component';
import { ReactiveFormsModule } from '@angular/forms';
import { PartRequestsComponent } from './part-requests.component';
import { InvoiceComponent } from './invoice.component';

const routes: Routes = [
  { path: 'jobs', component: JobListComponent },
  { path: 'inventory', component: InventoryListComponent },
  { path: 'inventory/add', component: InventoryFormComponent },
  { path: 'inventory/edit/:id', component: InventoryFormComponent },
  { path: 'part-requests', component: PartRequestsComponent },
  { path: 'invoice/:jobId', component: InvoiceComponent },
  { path: '', redirectTo: 'jobs', pathMatch: 'full' }
];

@NgModule({
  declarations: [AppComponent, InventoryListComponent, InventoryFormComponent, PartRequestsComponent, InvoiceComponent],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    JobsModule,
    RouterModule.forRoot(routes),
    ReactiveFormsModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
