/*
 * @Author: Chao Yang
 * @Date: 2020-12-11 19:17:36
 * @LastEditor: Chao Yang
 * @LastEditTime: 2022-08-09 16:04:28
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
export class UnAuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ) {
    return this.authService.sessionIsValid().pipe(map(valid => {
      if (valid == true) {
        if (this.authService.redirectUri.toLowerCase().startsWith("http") === false) {
          this.router.navigateByUrl(this.authService.redirectUri);
        } else {
          location.href = this.authService.redirectUri;
        }
      }
      return valid == false;
    }));
  }
}
