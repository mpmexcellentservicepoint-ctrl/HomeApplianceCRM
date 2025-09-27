import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { JobsModule } from './jobs/jobs.module';
import { JobListComponent } from './jobs/job-list.component';
import { JobDetailComponent } from './jobs/job-detail.component';

const routes: Routes = [
  { path: 'jobs', component: JobListComponent },
  { path: 'jobs/:id', component: JobDetailComponent },
  { path: '', redirectTo: 'jobs', pathMatch: 'full' }
];

@NgModule({
  declarations: [],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    JobsModule,
    RouterModule.forRoot(routes)
  ],
  bootstrap: []
})
export class AppModule {}
