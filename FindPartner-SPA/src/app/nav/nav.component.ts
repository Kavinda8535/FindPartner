import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};

  // All services are Inject to constructor...
  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  login()
  {
    this.authService.login(this.model).subscribe(next => {
      this.alertify.seccess('Logged in successfully');
    }, error => {
        this.alertify.error('Failed to login ' + error);
    });
  }

  loggedIn(){
    const token = localStorage.getItem('token');
    return !!token; // true or false return. If the token empty return false, else true.
  }

  logout(){
    localStorage.removeItem('token');
    this.alertify.message('Logout Success...');
  }

}
