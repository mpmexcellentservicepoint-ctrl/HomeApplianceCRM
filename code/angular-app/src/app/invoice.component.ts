import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-invoice',
  templateUrl: './invoice.component.html',
  styleUrls: ['./invoice.component.css']
})
export class InvoiceComponent implements OnInit {
  invoice: any;
  loading = false;
  error = '';
  jobId: number;
  sendMethod = 'Email';
  recipient = '';
  sendStatus = '';
  sendDetails = '';
  sending = false;
  sendLogs: any[] = [];

  constructor(private http: HttpClient, private route: ActivatedRoute) {
    this.jobId = +this.route.snapshot.params['jobId'];
  }

  ngOnInit() {
    this.fetchInvoice();
  }

  fetchInvoice() {
    this.loading = true;
    this.http.get(`/api/invoices/job/${this.jobId}`).subscribe({
      next: data => {
        this.invoice = data;
        this.loading = false;
        if (this.invoice?.id) {
          this.fetchSendLogs(this.invoice.id);
        }
      },
      error: err => {
        this.error = 'Failed to load invoice';
        this.loading = false;
      }
    });
  }

  fetchSendLogs(invoiceId: number) {
    this.http.get(`/api/invoices/${invoiceId}/send-logs`).subscribe({
      next: (logs: any) => this.sendLogs = logs,
      error: () => this.sendLogs = []
    });
  }

  sendInvoice() {
    if (!this.invoice?.id || !this.recipient) {
      this.sendStatus = 'Recipient required.';
      return;
    }
    this.sending = true;
    this.http.post(`/api/invoices/${this.invoice.id}/send-log`, {
      method: this.sendMethod,
      recipient: this.recipient,
      status: 'Sent',
      details: this.sendDetails
    }).subscribe({
      next: (log: any) => {
        this.sendStatus = 'Send logged!';
        this.sending = false;
        this.fetchSendLogs(this.invoice.id);
        this.sendDetails = '';
      },
      error: () => {
        this.sendStatus = 'Failed to log send.';
        this.sending = false;
      }
    });
  }
}
