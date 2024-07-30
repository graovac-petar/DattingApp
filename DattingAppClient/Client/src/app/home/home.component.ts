import { Component, inject, OnInit, signal } from '@angular/core';
import { RegisterComponent } from "../register/register.component";
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit {
  
  registerMode =signal<boolean>(false);
  users: any
  http = inject(HttpClient);

  ngOnInit(): void {
    this.getUsers();
  }

  registerToogle() {
    this.registerMode.set(!this.registerMode());
  }

  getUsers() {
    this.http.get('https://localhost:7163/api/appusers').subscribe({
      next: response => this.users = response,
      error: error => console.log(error),
      complete: () => console.log('Request has completed')
    });
  }

  cancelRegisterMode(event: boolean) {
    this.registerMode.set(event);
  }
}
