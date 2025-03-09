import { Component, OnInit } from '@angular/core';
import { JobService } from './../services/job.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-job-postings',
  templateUrl: './job-postings.component.html',
  styleUrls: ['./job-postings.component.css']
})
export class JobPostingsComponent implements OnInit {

  jobListings: any[] = [];
  activeTab: string = 'database';
  
  constructor(private jobService: JobService, private router: Router) { }

  ngOnInit(): void {
    this.jobService.getJobListings().subscribe(jobs => {
      this.jobListings = jobs;
    });
  }

  navigateToResumeBuilder(job: any) {
    const jobString = JSON.stringify(job); // Serialize the job object
    this.router.navigate(['/resumebuilder'], { queryParams: { job: jobString } });
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }
}
