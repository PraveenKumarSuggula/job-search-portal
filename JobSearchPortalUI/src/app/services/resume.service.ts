import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment'; 

@Injectable({
  providedIn: 'root'
})
export class ResumeService {

  private baseUrl = environment.apiBaseUrl + '/resume'; 

  constructor(private http: HttpClient) { }

  generateResume(resumeRequest: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/generate`, resumeRequest);
  }
}
