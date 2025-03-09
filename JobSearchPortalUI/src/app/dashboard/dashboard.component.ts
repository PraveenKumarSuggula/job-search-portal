import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  applicationDetails: any = {};
  collaborationDetails: any[] = [];
  otherInfo: any = {};
  projectOverview: string = '';
  dataAbstraction: any[] = [];

  ngOnInit(): void {
    // Hardcoded data for the dashboard
    this.applicationDetails = {
      appName: 'JobSeeker Hub',
      description: 'A centralized job searching platform that integrates with popular job boards like LinkedIn, Indeed, and Handshake.',
      version: '4.8.0',
      status: 'Active'
    };

    this.collaborationDetails = [
      { name: 'LinkedIn', collaborationType: 'Job Listings', status: 'Active' },
      { name: 'Indeed', collaborationType: 'Job Listings', status: 'Active' },
      { name: 'Handshake', collaborationType: 'Job Listings', status: 'False' },
      { name: 'ChatGPT (3.5 Turbo)', collaborationType: 'Resume Builder', status: 'Active' }
    ];

    this.otherInfo = {
      updates: 'New feature: Generate dynamic resumes based on job listings and user profiles.',
      upcomingFeatures: 'AI-based job matching and skill recommendations.'
    };

    this.projectOverview = `In today’s competitive job market, many job seekers, especially newly graduate students, are struggling to identify the right platform to search for a suitable job based on their experience level and skill set. Even after finding a relevant job, it’s difficult to prepare their resume according to the job description in a short time. To overcome this problem, we are proposing a centralized job searching platform which integrates with popular job boards like LinkedIn, Indeed, and Handshake. When users find a suitable job from our centralized application, they can also generate and download a dynamic resume based on user-provided information and their selected predefined template before redirecting them to the actual job posting site to apply for it.`;

    this.dataAbstraction = [
      {
        source: 'Job Seeker Details',
        description: 'Collects user-specific data such as skills, projects, education, certifications, experience level, and preferred job type.'
      },
      {
        source: 'Job Listings from Job Boards',
        description: 'Fetches job listings from popular platforms like LinkedIn, Indeed, and Handshake.'
      },
      {
        source: 'Job Description Data',
        description: 'Extracts job titles, location, company details, required skills, job responsibilities, qualifications, and the number of positions available from external job listings.'
      }
    ];
  }
}
