import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ResumeService } from '../services/resume.service';
import { ProfileService } from './../services/profile.service';
import { ActivatedRoute, Router } from '@angular/router';
import { saveAs } from 'file-saver';
import Chart from 'chart.js/auto';

interface ResumeRequest {
  name: string;
  template: string;
  experienceLevel: string;
  experience: string;
  skills: string[];
  preferredJobType: string;
  education: string;
  certifications: string[];
  jobRequirements: string;
}

@Component({
  selector: 'app-resume-generate',
  templateUrl: './resume-generate.component.html',
  styleUrls: ['./resume-generate.component.css']
})
export class ResumeGenerateComponent implements OnInit, AfterViewInit {

  resumeForm: FormGroup;
  atsResult: any = null;
  resumeTemplates = [
    { id: 'template1', name: 'Basic Template' },
    { id: 'template2', name: 'Professional Template' },
    { id: 'template3', name: 'Creative Template' }
  ];

  technologies = [
    "java", "python", "javascript", "sql", "html", "css", "angular", "react", "nodejs", "typescript", 
    "aws", "azure", "docker", "kubernetes", "git", "linux", "mongodb", "mysql", "graphql", "redux", "flutter",
    "aws lambda", "dynamodb", "go", "serverless", "nosql", "gcp", "ci/cd", "sass", "less", "web components", 
    "microfrontends", "frontend", "backend", "rest apis", "graphql apis", "cloud computing", "ai", "machine learning", 
    "deep learning", "nlp", "computer vision", "scalable systems", "high-performance systems", "api design", "distributed systems", 
    "security", "accessibility", "responsive design", "web security", "devops", "agile", "scrum", "kubernetes", "terraform", 
    "cloud services", "react hooks", "redux-saga", "typescript", "nodejs", "vue.js", "html5", "css3"
  ];
  

  user: any = {};  // User data from local storage
  job: any;        // Job data passed from previous page
  loggedIn = false;
  generatedResume: any;  // Store generated resume content
  skills: any;

  constructor(
    private fb: FormBuilder,
    private resumeService: ResumeService,
    private profileService: ProfileService,
    private activatedRoute: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    const storedUser = localStorage.getItem('user');
    if (storedUser) {
      this.loggedIn = true;
      const username = JSON.parse(storedUser).username;
      this.profileService.getUserProfile(username).subscribe(
        (profile) => {
          this.user = profile;
        },
        (error) => {
          alert('Oops! Session Timeout. Try re-login again.');
          this.router.navigate(['/login']);
        }
      );
    }

    this.activatedRoute.queryParams.subscribe(params => {
      if (params['job']) {
        this.job = JSON.parse(params['job']); // Deserialize the job object
      }
    });

    // Initialize form group with some default values
    this.resumeForm = this.fb.group({
      template: ['Professional Template', Validators.required],
      jobDescription: [this.job?.jobDescription || '', Validators.required],
      experienceLevel: [this.user.experienceLevel || '', Validators.required],
      experience: [this.user.experience || '', Validators.required],
      skills: [this.user?.skills?.length ? this.user.skills?.join(', ') : '', Validators.required],
      preferredJobType: [this.user?.preferredJobType || '', Validators.required],
      education: [this.user?.education || '', Validators.required],
      certifications: [this.user?.certifications?.length ? this.user.certifications.join(', ') : '', Validators.required],
    });

    // Retrieve user profile data from localStorage if logged in
    if (localStorage.getItem('user')) {
      this.loggedIn = true;
      const username = JSON.parse(localStorage.getItem('user') || '{}').username;
      this.profileService.getUserProfile(username).subscribe(profile => {
        this.user = profile;
        this.resumeForm.patchValue({
          experienceLevel: this.user?.experienceLevel,
          experience: this.user?.experience,
          skills: this.user?.skills?.join(', ') || '',
          preferredJobType: this.user?.preferredJobType,
          education: this.user?.education,
          certifications: this.user?.certifications?.join(', ') || ''
        });
      });
    }
  }

  ngAfterViewInit() {
    this.renderChart();
  }

  renderChart() {
    const matchedCount = this.atsResult?.matched.length || 0;
    const missingCount = this.atsResult?.missing.length || 0;
    const extraCount = (this.getResumeKeywords(this.resumeForm.value) || []).length - matchedCount;

    const ctx = (document.getElementById('keywordsChart') as HTMLCanvasElement).getContext('2d');
    new Chart(ctx, {
      type: 'pie',
      data: {
        labels: ['Matched Keywords', 'Missing Keywords', 'Extra Keywords'],
        datasets: [
          {
            data: [matchedCount, missingCount, extraCount],
            backgroundColor: ['#28a745', '#dc3545', '#ffc107'],
          },
        ],
      },
      options: {
        responsive: true,
        plugins: {
          legend: {
            position: 'bottom',
          },
        },
      },
    });
  }

  getExperienceHtml(experiences: any[]): string {
    if (!experiences || !experiences.length) return '<p>No experience details available.</p>';
    return experiences
      .map(exp => `
        <div>
          <p><strong>${exp.Title} at ${exp.Company} (${exp.Duration})</strong></p>
          <ul>
            ${exp.Responsibilities.map((resp: string) => `<li>${resp}</li>`).join('')}
          </ul>
        </div>
      `)
      .join('');
  }

  getCertificationsHtml(certifications: string[]): string {
    return certifications.map(cert => `<li>${cert}</li>`).join('');
  }

  getSkillsHtml(skills: string[]): string {
    return skills?.join(', ');
  }

  // Generate resume using the form data
  onGenerateResume() {
    if (this.resumeForm.invalid) return;

    const resumeRequest: ResumeRequest = {
      name: this.user?.username || '',
      template: this.resumeForm.value.template,
      experienceLevel: this.resumeForm.value.experienceLevel,
      experience: this.resumeForm.value.experience,
      skills: this.resumeForm.value.skills.split(',').map((skill: string) => skill.trim()), // Convert to array
      preferredJobType: this.resumeForm.value.preferredJobType,
      education: this.resumeForm.value.education,
      certifications: this.resumeForm.value.certifications.split(',').map((cert: string) => cert.trim()), // Convert to array
      jobRequirements: this.resumeForm.value.jobDescription
    };

    // Call the resume service to generate the resume
    this.resumeService.generateResume(resumeRequest).subscribe(
      (response) => {
        alert('Resume generated successfully!');
        this.generatedResume = response.resume;
  
        // Perform ATS Check after resume is set
        this.atsResult = this.performATSCheck(this.resumeForm.value.jobDescription, this.generatedResume);
  
        console.log('ATS Check Result:', this.atsResult); 
      },
      (error) => {
        alert('Error generating resume: ' + error);
      }
    );
  }

  formatResume(resume: string): string {
    return resume ? resume.replace(/\n/g, '<br>') : '';
  }

  downloadResumeAsDoc() {
    const resumeContent = `
    <html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:w="urn:schemas-microsoft-com:office:word" xmlns="http://www.w3.org/TR/REC-html40">
    <head><meta charset="utf-8"><title>Resume</title></head>
    <body>
      <h1 style="text-align: center;">Generated Resume</h1>
      <h2>Name: ${this.generatedResume.Name}</h2>
      <h3>Professional Summary:</h3>
      <p>${this.generatedResume.ProfessionalSummary}</p>
      
      <h3>Experience:</h3>
      ${this.getExperienceHtml(this.generatedResume.Experiences)}

      <h3>Education:</h3>
      <p>${this.generatedResume.Education.Degree}, ${this.generatedResume.Education.Institution}</p>

      <h3>Certifications:</h3>
      <ul>
        ${this.getCertificationsHtml(this.generatedResume.Certifications)}
      </ul>

      <h3>Technical Skills:</h3>
      <p>${this.resumeForm.value?.skills}</p>
    </body>
    </html>
  `;

    const blob = new Blob([resumeContent], { type: 'application/msword;charset=utf-8' });
    saveAs(blob, 'resume.doc');
  }

  // performATSCheck(jobDescription: string, resume: any): { score: number; matched: string[]; jobKeywords: string[] } {
  //   const jobKeywords = this.extractJDKeywords(jobDescription); // Extract keywords from JD
  //   const resumeKeywords = this.getResumeKeywords(resume); // Extract keywords from resume
  
  //   // Find matched and missing keywords
  //   const matched = jobKeywords.filter(keyword => resumeKeywords.includes(keyword.toLowerCase())) || ['None'];
  //   //const missing = jobKeywords.filter(keyword => !resumeKeywords.includes(keyword.toLowerCase())) || ['None'];
  
  //   // Calculate score as a percentage
  //   const score = Math.round((matched.length / jobKeywords.length) * 100);
  
  //   return { score, matched, jobKeywords };
  // }

  performATSCheck(jobDescription: string, resume: any): { 
    //score: number; 
    precision: number; 
    recall: number; 
    f1Score: number; 
    accuracy: number; 
    matched: string[]; 
    missing: string[];
    jobKeywords: string[] 
  } {
    const jobKeywords = this.extractJDKeywords(jobDescription); // Extract keywords from JD
    const resumeKeywords = this.getResumeKeywords(resume); // Extract keywords from resume
  
    // Calculate matched, false positives, and false negatives
    const matched = jobKeywords.filter(keyword => resumeKeywords.includes(keyword.toLowerCase())) || [];
    const missing = jobKeywords.filter(keyword => !resumeKeywords.includes(keyword.toLowerCase())) || [];
    const extra = resumeKeywords.filter(keyword => !jobKeywords.includes(keyword.toLowerCase())) || [];
  
    const TP = matched.length; // True Positives: matched keywords
    const FN = missing.length; // False Negatives: keywords missing in the resume
    const FP = extra.length;  // False Positives: extra keywords in the resume
    const TN = 0;  // True Negatives: In this case, we don't calculate TN as we are just matching keywords
    
    // Calculate Precision, Recall, F1 Score, and Accuracy
    const precision = TP / (TP + FP) || 0;
     // Calculate ATS Score as a percentage
    const recall = TP / (TP + FN) || 0;
    const f1Score = (2 * precision * recall) / (precision + recall) || 0;
    const accuracy = (TP + TN) / (TP + TN + FP + FN) || 0;
    return {
      precision: Math.round(precision * 100), // Precision in percentage
      recall: Math.round(recall * 100), // Recall in percentage
      f1Score: Math.round(f1Score * 100), // F1 Score in percentage
      accuracy: Math.round(accuracy * 100), // Accuracy in percentage
      matched,
      missing,
      jobKeywords
    };
  }  
  
  extractJDKeywords(text: string): string[] {
    if (!text) return [];
  
    const words = text.toLowerCase().match(/\b(\w+)\b/g) || [];
  
    const matchedKeywords = words.filter(word => this.technologies.includes(word.toLowerCase()));
  
    // Remove duplicates by converting the array to a set and back to an array
    return [...new Set(matchedKeywords)];
  }
  
  getResumeKeywords(resume: any): string[] {
    const keywords = [];
  
    // Add each word from the skills
    this.skills = resume?.Skills || this.resumeForm?.value.skills;

    if (typeof this.skills === 'string') {
      // If it's a string (comma separated), split by commas and trim each skill
      this.skills = this.skills.split(',').map((skill: string) => skill.trim().toLowerCase());
    }
  
    // If skills is an array, process each skill
    if (Array.isArray(this.skills)) {
      this.skills.forEach((skill: string) => {
        keywords.push(...skill.toLowerCase().split(/\s+/)); // Split by spaces
      });
    }
  
    // Add each word from the certifications
    // if (resume?.Certifications) {
    //   resume.Certifications.forEach((cert: string) => {
    //     keywords.push(...cert.toLowerCase().split(/\s+/)); // Split by spaces
    //   });
    // }
  
    // Add each word from the experience titles and responsibilities
    // if (resume?.Experiences) {
    //   resume.Experiences.forEach((exp: any) => {
    //     //Title
    //     keywords.push(...exp.Title.toLowerCase().split(/\s+/)); // Split by spaces
  
    //     //Responsibilities
    //     (exp.Responsibilities || []).forEach((resp: string) => {
    //       keywords.push(...resp.toLowerCase().split(/\s+/)); // Split by spaces
    //     });
    //   });
    // }
  
    // Return the array of keywords
    return keywords;
  }
  
}
