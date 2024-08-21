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
export class HomeComponent {
  
  registerMode =signal<boolean>(false);

  registerToogle() {
    this.registerMode.set(!this.registerMode());
  }

  cancelRegisterMode(event: boolean) {
    this.registerMode.set(event);
  }
}
