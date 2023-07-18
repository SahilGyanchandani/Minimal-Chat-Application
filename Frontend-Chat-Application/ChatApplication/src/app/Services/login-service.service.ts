import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Message, MessageSend } from '../Models/message.model';


@Injectable({
  providedIn: 'root'
})

export class LoginServiceService {
  constructor(private http: HttpClient) {}

  onSubmit(obj: any): Observable<any> {
    const headers = new HttpHeaders().set('content-type', 'application/json').set('Access-Control-Allow-Origin', 'https://localhost:7277/');
    return this.http.post<any>('https://localhost:7277/api/UserLogin', obj, { headers , withCredentials:true});

  }

  onReg(userData : any):Observable<any>{
     return this.http.post<any>('https://localhost:7277/api/UserReg',userData);
  }

  onUserList():Observable<any>{
    return this.http.get<any>('https://localhost:7277/api/UserReg/GetUser');
  }

  onMsgHistory(userid:any):Observable<any>{
    return this.http.get<any>(`https://localhost:7277/api/Message?userId=${userid}`);
  }

  sendMessage(message: MessageSend): Observable<any> {
    return this.http.post<any>(`https://localhost:7277/api/Message`, message);
  }

  updateMessage(id: string, content: string): Observable<Message> {
    const url = `https://localhost:7277/api/Message/${id}`;
    const body = { content }; // Assuming your backend API expects the content in the request body
    return this.http.put<Message>(url, body);
  }

  deleteMessage(id:string):Observable<any>{
    return this.http.delete<any>(`https://localhost:7277/api/Message/${id}`);
  }
 
}

