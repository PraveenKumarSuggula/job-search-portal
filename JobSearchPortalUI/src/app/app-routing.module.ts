import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { ProfileManagementComponent } from './profile-management/profile-management.component';
import { JobPostingsComponent } from './job-postings/job-postings.component';
import { ResumeGenerateComponent } from './resume-generate/resume-generate.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'jobs', component: JobPostingsComponent },
  { path: 'profile', component: ProfileManagementComponent },
  { path: 'resumebuilder', component: ResumeGenerateComponent },
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
