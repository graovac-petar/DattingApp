import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { TitleCasePipe } from '@angular/common';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [FormsModule,BsDropdownModule,RouterLink,RouterLinkActive,TitleCasePipe],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  model :any = {};
  accountService = inject(AccountService);
  private router=inject(Router);
  private toastr=inject(ToastrService);

  login(){
    this.accountService.login(this.model).subscribe({
      next:() => this.router.navigateByUrl('/members'),
      error: error => this.toastr.error(error.error)
    })
  }

  logout(){
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
