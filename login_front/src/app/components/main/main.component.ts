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
  
  onSubmit() {
  if (this.loginForm.valid) {
    const email = this.loginForm.value.email;
    const password = this.loginForm.value.password;

    // Example logic — replace with your actual authentication call
    if (email === 'admin@example.com' && password === 'password123') {
      // Redirect to functionalityPage
      this.router.navigate(['/functionalityPage']);
    } else {
      // Handle invalid credentials
      alert('Invalid email or password');
    }
  }
}
}
