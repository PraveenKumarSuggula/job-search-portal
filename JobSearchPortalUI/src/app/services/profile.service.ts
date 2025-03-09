import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

// Models for requests and responses
export interface UserProfileUpdateDto {
  email: string;
  skills: string[];
  experienceLevel: string;
  experience: string;
  preferredJobType: string;
  education?: string;
  certifications?: string;
}

export interface UserProfile {
  username: string;
  email: string;
  skills: string[];
  experienceLevel: string;
  experience: string;
  preferredJobType: string;
  password?: string;
  education?: string;
  certifications?: string;
}

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  private baseUrl = 'https://localhost:44386/api/profile';

  constructor(private http: HttpClient) {}

  // Get a user profile
  getUserProfile(username: string): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.baseUrl}/${username}`);
  }

  // Update a user profile
  updateUserProfile(username: string, updatedProfile: UserProfileUpdateDto): Observable<any> {
    return this.http.put(`${this.baseUrl}/${username}`, updatedProfile);
  }

  // Delete a user profile
  deleteUserProfile(username: string): Observable<any> {
    return this.http.delete(`${this.baseUrl}/${username}`);
  }
}
