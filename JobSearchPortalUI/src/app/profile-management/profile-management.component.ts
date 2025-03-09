import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ProfileService } from './../services/profile.service';

@Component({
  selector: 'app-profile-management',
  templateUrl: './profile-management.component.html',
  styleUrls: ['./profile-management.component.css']
})
export class ProfileManagementComponent implements OnInit {
  user: any = {};
  loggedIn = false;
  updateForm: FormGroup;
  isUpdating = false;

  constructor(
    private profileService: ProfileService,
    private fb: FormBuilder,
    private router: Router
  ) {
    // Initialize the reactive form
    this.updateForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      skills: ['', [Validators.required]],
      experienceLevel: ['', [Validators.required]],
      experience: ['', [Validators.required]],
      preferredJobType: ['', [Validators.required]],
      education: ['', [Validators.required]],
      certifications: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    const storedUser = localStorage.getItem('user');
    if (storedUser) {
      this.loggedIn = true;
      const username = JSON.parse(storedUser).username;
      this.profileService.getUserProfile(username).subscribe(
        (profile) => {
          this.user = profile;
          this.updateForm.patchValue({
            email: this.user.email,
            skills: this.user?.skills.join(', '),
            experienceLevel: this.user.experienceLevel,
            experience: this.user.experience,
            preferredJobType: this.user.preferredJobType,
            education: this.user.education,
            certifications: this.user.certifications?.skills.join(', ')
          });
        },
        (error) => {
          alert('Oops! Session Timeout. Try re-login again.');
          this.router.navigate(['/login']);
        }
      );
    }
  }

  // Handle profile update
  onUpdateProfile(): void {
    if (this.updateForm.valid) {
      const updatedData = {
        email: this.updateForm.value.email,
        skills: this.updateForm.value.skills.split(',').map((s: string) => s.trim()),
        experienceLevel: this.updateForm.value.experienceLevel,
        experience: this.updateForm.value.experience,
        preferredJobType: this.updateForm.value.preferredJobType,
        education: this.updateForm.value.education,
        certifications: this.updateForm.value.certifications.split(',').map((s: string) => s.trim())
      };

      const username = this.user.username;

      this.profileService.updateUserProfile(username, updatedData).subscribe(
        (res) => {
          console.log('Profile updated successfully:', res);
          this.user = { ...this.user, ...updatedData }; // Update the user profile with the new data
          this.isUpdating = false;
          alert('Profile updated successfully!');
        },
        (error) => {
          console.error('Error updating profile:', error);
          alert('Error updating profile. Please try again.');
        }
      );
    }
  }

  // Handle profile deletion
  onDeleteProfile(): void {
    if (confirm('Are you sure you want to delete your profile? This action cannot be undone.')) {
      const username = this.user.username;
      this.profileService.deleteUserProfile(username).subscribe(
        (res) => {
          console.log('Profile deleted successfully:', res);
          localStorage.removeItem('user');
          this.loggedIn = false;
          alert('Profile deleted successfully!');
          this.router.navigate(['/login']); 
        },
        (error) => {
          console.error('Error deleting profile:', error);
          alert('Error deleting profile. Please try again.');
        }
      );
    }
  }
}
