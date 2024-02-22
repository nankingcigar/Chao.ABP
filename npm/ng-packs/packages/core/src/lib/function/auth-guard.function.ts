import { inject } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivateFn,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { map } from 'rxjs';
import { AuthService } from '../service/auth.service';

export const AuthGuard: CanActivateFn =
  (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    const authService = inject(AuthService);
    const router = inject(Router);
    return authService.sessionIsValid().pipe(map(valid => {
      if (valid == false) {
        if (authService.loginUrl.toLowerCase().startsWith("http") === false) {
          router.navigateByUrl(authService.loginUrl);
        } else {
          location.href = authService.loginUrl;
        }
      }
      return valid == true;
    }));
  };
