import { inject } from '@angular/core';
import {
    ActivatedRouteSnapshot,
    CanActivateFn,
    RouterStateSnapshot,
} from '@angular/router';
import { map } from 'rxjs';
import {
    AuthenticationConfig, AuthenticationMode,
} from '../model/authentication/index';
import { HttpClient } from '@angular/common/http';

export const XSRFGuard: CanActivateFn =
    (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
        const httpClient = inject(HttpClient);
        const authenticationConfig = inject(AuthenticationConfig);
        if (authenticationConfig.authenticationMode === AuthenticationMode.Cookie && authenticationConfig.cookieConfig.xsrf === true) {
            return httpClient
                .post<string>(authenticationConfig.cookieConfig.xsrfEndPoint, undefined)
                .pipe(map((r) => {
                    sessionStorage.setItem(
                        authenticationConfig.cookieConfig.xsrfCacheKey,
                        r
                    );
                    return true;
                }));
        } else {
            return true;
        }
    };
