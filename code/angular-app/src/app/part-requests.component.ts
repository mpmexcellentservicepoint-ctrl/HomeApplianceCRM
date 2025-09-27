import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface PartRequest {
  id: number;
  partId: number;
  jobId?: number;
  quantity: number;
  status: string;
  part?: any;
  job?: any;
}

@Component({
  selector: 'app-part-requests',
  templateUrl: './part-requests.component.html',
  styleUrls: ['./part-requests.component.css']
})
export class PartRequestsComponent implements OnInit {
  requests: PartRequest[] = [];
  loading = false;
  error = '';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.fetchRequests();
  }

  fetchRequests() {
    this.loading = true;
    this.http.get<PartRequest[]>('/api/partrequests').subscribe({
      next: data => {
        this.requests = data;
        this.loading = false;
      },
      error: err => {
        this.error = 'Failed to load part requests';
        this.loading = false;
      }
    });
  }

  markArrived(id: number) {
    this.http.post(`/api/partrequests/arrived/${id}`, {}).subscribe(() => this.fetchRequests());
  }
}
