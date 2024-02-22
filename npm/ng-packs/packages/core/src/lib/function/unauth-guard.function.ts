import { inject } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivateFn,
  Router,
  RouterStateSnapshot,
} from '@angular/router';
import { map } from 'rxjs';
import { AuthService } from '../service/auth.service';

export const UnAuthGuard: CanActivateFn =
  (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
    const authService = inject(AuthService);
    const router = inject(Router);
    return authService.sessionIsValid().pipe(map(valid => {
      if (valid == true) {
        if (authService.redirectUri.toLowerCase().startsWith("http") === false) {
          router.navigateByUrl(authService.redirectUri);
        } else {
          location.href = authService.redirectUri;
        }
      }
      return valid == false;
    }));
  };
