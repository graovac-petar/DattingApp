import { Component, HostListener, inject, OnInit, viewChild } from '@angular/core';
import { Member } from '../../_model/member';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { FormsModule, NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-member-edit',
  standalone: true,
  imports: [TabsModule, FormsModule],
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css'
})
export class MemberEditComponent implements OnInit {
  editForm=viewChild<NgForm>('editForm');
  @HostListener('window:beforeunload', ['$event']) notify($event: any) {
    if(this.editForm()!.dirty) {
      $event.returnValue = true;
    }
  }
  member?: Member;
  private accountService = inject(AccountService);
  private memberService=inject(MembersService);
  private toastr = inject(ToastrService);
  

  ngOnInit(): void {
    this.loadMember();
  }
  loadMember() {
    const user = this.accountService.currentUser();
      if(!user) return;
    this.memberService.getMember(user.username).subscribe({
      next: member => {
        this.member = member;
      }
    });
  }

  updateMember() {
    console.log(this.member);
    this.toastr.success('Profile updated successfully');
    this.editForm()!.reset(this.member);
  }
}


