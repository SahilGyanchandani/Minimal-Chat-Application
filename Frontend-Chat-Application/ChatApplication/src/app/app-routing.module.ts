import { NgModule } from '@angular/core';
import { RouterModule, Routes, mapToCanActivate } from '@angular/router';
import { RegistrationComponent } from './Component/registration/registration.component';
import { LoginComponent } from './Component/login/login.component';
import { NotFoundComponent } from './Component/not-found/not-found.component';
import { UserListComponent } from './Component/user-list/user-list.component';
// import { AuthGuard } from './guards/auth.guard';


const routes: Routes = [
  {path:'registration',component:RegistrationComponent},
  {path:'login',component:LoginComponent},
  {path:'',redirectTo:'/registration',pathMatch:'full'},
  {path:'userlist',component:UserListComponent},
  // {path:'**',component:NotFoundComponent},
  
  // {
  //   canActivate:[AuthGuard],
  // }
  
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
