import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-job-list',
  templateUrl: './job-list.component.html'
})
export class JobListComponent implements OnInit {
  jobs: any[] = [];
  constructor(private http: HttpClient) {}
  ngOnInit() {
    this.http.get<any[]>('/api/jobs').subscribe(data => this.jobs = data);
  }
}
