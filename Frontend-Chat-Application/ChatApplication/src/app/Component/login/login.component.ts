import { Component } from '@angular/core';
import {FormGroup,FormControl,FormBuilder, Validators} from '@angular/forms';
import {  Router } from '@angular/router';
import { faLock } from '@fortawesome/free-solid-svg-icons';
import { LoginServiceService } from 'src/app/Services/login-service.service';
import { UserListComponent } from '../user-list/user-list.component';
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
  this.logService.onSubmit(this.regisform.value).subscribe((res:any) => {
    console.log('res',res)
    localStorage.setItem('token',res.token);
    this.route.navigateByUrl('/userlist');
    
  })
}

}
