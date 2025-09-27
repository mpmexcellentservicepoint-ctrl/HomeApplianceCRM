import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-inventory-form',
  templateUrl: './inventory-form.component.html',
  styleUrls: ['./inventory-form.component.css']
})
export class InventoryFormComponent implements OnInit {
  form: FormGroup;
  loading = false;
  error = '';
  isEdit = false;
  partId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.form = this.fb.group({
      id: [null],
      name: ['', Validators.required],
      description: [''],
      stockQty: [0, Validators.required],
      msl: [0, Validators.required],
      storageLocation: [''],
      boxNo: [''],
      alternateLocation: ['']
    });
  }

  ngOnInit() {
    this.partId = this.route.snapshot.params['id'] ? +this.route.snapshot.params['id'] : null;
    this.isEdit = !!this.partId;
    if (this.isEdit) {
      this.loading = true;
      this.http.get<any>(`/api/parts/${this.partId}`).subscribe({
        next: part => {
          this.form.patchValue(part);
          this.loading = false;
        },
        error: () => {
          this.error = 'Failed to load part';
          this.loading = false;
        }
      });
    }
  }

  submit() {
    if (this.form.invalid) return;
    this.loading = true;
    const part = this.form.value;
    if (this.isEdit) {
      this.http.put(`/api/parts/${this.partId}`, part).subscribe({
        next: () => this.router.navigate(['/inventory']),
        error: () => {
          this.error = 'Failed to update part';
          this.loading = false;
        }
      });
    } else {
      this.http.post('/api/parts', part).subscribe({
        next: () => this.router.navigate(['/inventory']),
        error: () => {
          this.error = 'Failed to add part';
          this.loading = false;
        }
      });
    }
  }

  cancel() {
    this.router.navigate(['/inventory']);
  }
}
