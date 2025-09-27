import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { JobListComponent } from './job-list.component';
import { JobDetailComponent } from './job-detail.component';

const routes: Routes = [
  { path: 'jobs', component: JobListComponent },
  { path: 'jobs/:id', component: JobDetailComponent }
];

@NgModule({
  declarations: [JobListComponent, JobDetailComponent],
  imports: [
    CommonModule,
    FormsModule,
    HttpClientModule,
    RouterModule.forChild(routes)
  ],
  exports: [JobListComponent, JobDetailComponent]
})
export class JobsModule {}
