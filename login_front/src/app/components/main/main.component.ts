import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import {  FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import {  Router, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-main',
  imports: [FormsModule,CommonModule,ReactiveFormsModule],
  templateUrl: './main.component.html',
  styleUrl: './main.component.css'
})
export class MainComponent {
    loginForm:FormGroup;
    isSubmitting = false;
    showPassword = false;

    constructor(
      private fb: FormBuilder,
      // private http:HttpClient,
      private router: Router
    ){
      this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }
  togglePasswordVisibility() {
  this.showPassword = !this.showPassword;
}
  
  onSubmit(){
    if(this.loginForm.invalid){
      return;
    }

    this.isSubmitting = true;
    const credentials =this.loginForm.value;


  }

}
