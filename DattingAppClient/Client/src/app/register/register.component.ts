import { Component, Inject, inject, OnInit, output } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { AccountService } from '../_services/account.service';
import { JsonPipe, NgIf } from '@angular/common';
import { TextInputComponent } from '../_forms/text-input/text-input.component';
import { DatePickerComponent } from '../_forms/date-picker/date-picker.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule,NgIf,TextInputComponent,DatePickerComponent,JsonPipe],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit {
  
  private accountService = inject(AccountService)
  private fb=inject(FormBuilder)
  private router = inject(Router)
  cancelRegister = output<boolean>();
  model: any = {};
  registerForm:FormGroup=new FormGroup({});
  maxDate=new Date();
  validationErrors:string[] = [];

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['',[Validators.required,Validators.minLength(8),Validators.maxLength(15),this.specialCharValidator]],
      confirmPassword: ['',[Validators.required,this.matchValues('password')]]
    })
    this.registerForm.controls['password'].valueChanges.subscribe(()=>{
      next: () => this.registerForm.controls['confirmPassword'].updateValueAndValidity();
    })
  }

  specialCharValidator(control: AbstractControl): ValidationErrors | null {
    const specialCharPattern = /[!@#$%^&*(),.?":{}|<>]/;  // Define special characters
  
    if (control.value && !specialCharPattern.test(control.value)) {
      return { specialChar: 'Password must contain at least one special character' };
    }
  
    return null;
  }
  matchValues (matchTo: string): ValidatorFn {
    return (control:AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {isMatching: true}
    }
  }

  register() {
    const dob = this.getDateOnly(this.registerForm.get('dateOfBirth')?.value);
    const formattedValue = { ...this.registerForm.value, dateOfBirth: dob };
    this.accountService.register(formattedValue).subscribe({
      next: () => this.router.navigateByUrl('/members'),
      error: error => this.validationErrors = error
    })
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
  
  private getDateOnly(dob: string | undefined)  {
    if(!dob) return;
    return new Date(dob).toISOString().slice(0,10);
  }
}

