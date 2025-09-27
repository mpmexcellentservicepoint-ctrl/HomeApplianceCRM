import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

interface Part {
  id: number;
  name: string;
  description?: string;
  stockQty: number;
  msl: number;
  storageLocation?: string;
  boxNo?: string;
  alternateLocation?: string;
}

@Component({
  selector: 'app-inventory-list',
  templateUrl: './inventory-list.component.html',
  styleUrls: ['./inventory-list.component.css']
})
export class InventoryListComponent implements OnInit {
  parts: Part[] = [];
  loading = false;
  error = '';

  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit() {
    this.fetchParts();
  }

  editPart(part: Part) {
    this.router.navigate(['/inventory/edit', part.id]);
  }

  fetchParts() {
    this.loading = true;
    this.http.get<Part[]>('/api/parts').subscribe({
      next: data => {
        this.parts = data;
        this.loading = false;
      },
      error: err => {
        this.error = 'Failed to load parts';
        this.loading = false;
      }
    });
  }

  isBelowMSL(part: Part): boolean {
    return part.stockQty < part.msl;
  }
}
