import { Component } from '@angular/core';
import {FormGroup,FormControl,FormBuilder, Validators} from '@angular/forms';
import {  Router } from '@angular/router';
import { faLock } from '@fortawesome/free-solid-svg-icons';
import { LoginServiceService } from 'src/app/Services/login-service.service';
import { UserListComponent } from '../user-list/user-list.component';
import { HttpErrorResponse } from '@angular/common/http';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  faLock=faLock;
  tittle ='Login Form';
  regisform!:FormGroup
  submitted=false;
  errorMessage: string | null = null;


  constructor( private logService :LoginServiceService,private route:Router,private formBuilder:FormBuilder ){}

  ngOnInit()
{
  this.regisform=this.formBuilder.group({

    Email:['',[Validators.required,Validators.email]],
    Password:['',[Validators.required,Validators.minLength(5)]],

    //validations
  })
}

onSubmit()
{
  this.submitted=true

  if(this.regisform.invalid)
  {
    throw new Error("Please Enter Valid Values");
  }
  this.logService.onSubmit(this.regisform.value).subscribe(res => {
    console.log('res',res)
    localStorage.setItem('token',res.token);
    this.route.navigateByUrl('/userlist');
  },
    (error: HttpErrorResponse) => {
      console.log(error); // Log the error response to the console

      if (error.status === 404) {
        this.errorMessage = "User not registered.";
      } else if (error.error) {
        // Display the error message to the user
        this.errorMessage = error.error;
      } else {
        this.errorMessage = 'An error occurred: ' + error.message;
      }
    }
  );
}

}
