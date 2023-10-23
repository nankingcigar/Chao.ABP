/*
 * @Author: Chao Yang
 * @Date: 2020-12-11 19:14:24
 * @LastEditor: Chao Yang
 * @LastEditTime: 2022-08-09 18:01:06
 */
import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { map } from 'rxjs';
import { AuthService } from '../service/auth.service';

@Injectable({
  providedIn: 'root',
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ) {
    return this.authService.sessionIsValid().pipe(map(valid => {
      if (valid == false) {
        if (this.authService.loginUrl.toLowerCase().startsWith("http") === false) {
          this.router.navigateByUrl(this.authService.loginUrl);
        } else {
          location.href = this.authService.loginUrl;
        }
      }
      return valid == true;
    }));
  }
}
