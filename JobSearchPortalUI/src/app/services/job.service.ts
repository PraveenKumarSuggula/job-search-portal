import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class JobService {

  private apiUrl = 'https://localhost:44386/api/jobs';

  constructor(private http: HttpClient) {}

  getJobListings(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}`);
  }
}
