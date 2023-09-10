import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../Models/user';
import {Observable} from 'rxjs/internal/Observable'

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient) { }

  public register(user: User): Observable<any> {
    return this.http.post<any>('https://localhost:7290/api/Auth/register',user);
  }

  public login(user: User): Observable<string> {
    return this.http.post('https://localhost:7290/api/Auth/login',user,{responseType:'text'});
  }

  public getMe(): Observable<string> {
    return this.http.get('https://localhost:7290/api/Auth',{responseType:'text'});
  }
}
