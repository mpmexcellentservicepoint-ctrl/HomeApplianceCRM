import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-job-detail',
  templateUrl: './job-detail.component.html'
})
export class JobDetailComponent implements OnInit {
  job: any;
  statusOptions = ['Assigned','Allocate','PartPending','CustomerChangeAppointmentDate','WorkCompleted','Closed','Cancelled'];
  technicianId = '';
  constructor(private route: ActivatedRoute, private http: HttpClient) {}
  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    this.http.get<any>(`/api/jobs/${id}`).subscribe(data => this.job = data);
  }
  updateStatus(newStatus: string) {
    this.http.post(`/api/jobs/${this.job.id}/status`, JSON.stringify(newStatus), {headers: {'Content-Type': 'application/json'}})
      .subscribe(() => this.job.status = newStatus);
  }
  assignTechnician() {
    this.http.post(`/api/jobs/${this.job.id}/assign-technician`, JSON.stringify(this.technicianId), {headers: {'Content-Type': 'application/json'}})
      .subscribe(() => this.job.technicianId = this.technicianId);
  }
}
