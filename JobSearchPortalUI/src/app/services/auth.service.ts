import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = 'https://localhost:44386/api';

  constructor(private http: HttpClient) {}

  login(credentials: { username: string; password: string }): Observable<any> {
    return this.http
      .post<any>(`${this.baseUrl}/auth/login`, credentials);
  }

  register(user: { username: string; password: string; email: string }): Observable<any> {
    return this.http
      .post<any>(`${this.baseUrl}/auth/register`, user);
  }
}
