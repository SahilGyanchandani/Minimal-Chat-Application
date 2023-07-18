import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable,catchError,throwError } from 'rxjs';
import { Router } from '@angular/router';

@Injectable()
export class CustomInterceptor implements HttpInterceptor {

  constructor(private router:Router) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const localToken=localStorage.getItem('token');
    // request=request.clone({headers:request.headers.set('Authorization','bearer'+localToken)});
    request = request.clone({
      setHeaders: {Authorization:`Bearer ${localToken}`} // Same as  "Bearer"+myToken
    })
    // return next.handle(request);
  
    return next.handle(request).pipe(
      catchError((err : any)=>{
        if(err instanceof HttpErrorResponse){
          if(err.status === 401){
            
            console.warn("Token is expired, Please Login again")           
            this.router.navigateByUrl('/login')
          }
        }
        return throwError(()=> new Error("Some other error occured") )
      })
    )
  }

}
