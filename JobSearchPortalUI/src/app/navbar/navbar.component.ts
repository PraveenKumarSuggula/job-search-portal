import { Component, OnInit } from '@angular/core';
import { NavigationStart, Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  isLoggedIn: boolean = false;

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.checkLoginStatus();

    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        // If the route changes to '/login', log out the user and clear the localStorage
        if (event.url === '/login' || event.url === '/' || event.url === '/register') {
          localStorage.removeItem('user');
          this.isLoggedIn = false;
        }
        else{
          this.isLoggedIn = true;
        }
      }
    });
  }

  checkLoginStatus(): void {
    if (localStorage.getItem('user')) {
      this.isLoggedIn = true;
    } else {
      this.isLoggedIn = false;
    }
  }
  // Handle logout
  onLogout(): void {
    localStorage.removeItem('user');
    this.isLoggedIn = false;
    this.router.navigate(['/login']);
  }
  
}
