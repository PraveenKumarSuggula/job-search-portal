import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ResumeService {

  private apiUrl = 'https://localhost:44386/api/resume'; 

  constructor(private http: HttpClient) { }

  generateResume(resumeRequest: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/generate`, resumeRequest);
  }
}
