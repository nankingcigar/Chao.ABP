/*
 * @Author: Chao Yang
 * @Date: 2020-12-11 19:09:03
 * @LastEditor: Chao Yang
 * @LastEditTime: 2023-04-11 16:42:13
 */
import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { from, map, Observable, of, Subject } from 'rxjs';
import {
  AuthenticationConfig,
  AuthenticationMode,
  LoginResultType,
  IAbpLoginResult,
} from '../model/authentication/index';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  login: (userName: string, password: string) => Observable<any>;
  logout: () => Observable<any>;
  sessionValidation = new Subject<boolean>();
  redirectUri: string;
  loginUrl: string;

  constructor(
    private oAuthService: OAuthService,
    private authenticationConfig: AuthenticationConfig,
    private httpClient: HttpClient
  ) {
    switch (this.authenticationConfig.authenticationMode) {
      case AuthenticationMode.Cookie:
        this.login = this.cookieLogin;
        this.logout = this.cookieLogout;
        this.redirectUri = this.authenticationConfig.cookieConfig.redirectUri;
        this.loginUrl = this.authenticationConfig.cookieConfig.loginUrl;
        if (this.authenticationConfig.cookieConfig.xsrf === true) {
          this.sessionIsValid().subscribe((r) => {
            if (r === true) {
              this.xsrfToken();
            }
          });
        }
        break;
      case AuthenticationMode.Token:
        this.oAuthService.configure(this.authenticationConfig.tokenConfig);
        this.oAuthService.loadDiscoveryDocumentAndTryLogin();
        this.login = this.tokenLogin;
        this.logout = this.tokenLogout;
        this.redirectUri = this.authenticationConfig.tokenConfig
          .redirectUri as string;
        this.loginUrl = this.authenticationConfig.tokenConfig
          .loginUrl as string;
        break;
      default:
        this.login = this.cookieLogin;
        this.logout = this.cookieLogout;
        this.redirectUri = this.authenticationConfig.cookieConfig.redirectUri;
        this.loginUrl = this.authenticationConfig.cookieConfig.loginUrl;
        break;
    }
  }

  cookieLogin(userName: string, password: string): Observable<any> {
    return this.httpClient
      .post<IAbpLoginResult>(
        this.authenticationConfig.cookieConfig.loginApiUrl,
        {
          userNameOrEmailAddress: userName,
          password: password,
          rememberMe: false,
        }
      )
      .pipe(
        map((abpLoginResult) => {
          if (abpLoginResult.result !== LoginResultType.Success) {
            throw new Error(abpLoginResult.description);
          }
          if (this.authenticationConfig.cookieConfig.xsrf === true) {
            this.xsrfToken();
          }
          return this.authenticationConfig.cookieConfig.redirectUri;
        })
      );
  }

  tokenLogin(userName: string, password: string): Observable<any> {
    return from(
      this.oAuthService.fetchTokenUsingPasswordFlow(userName, password)
    ).pipe(
      map((r) => {
        return this.authenticationConfig.tokenConfig.redirectUri;
      })
    );
  }

  cookieLogout(): Observable<any> {
    return this.httpClient
      .get<any>(this.authenticationConfig.cookieConfig.logoutApiUrl)
      .pipe(
        map((r) => {
          return this.authenticationConfig.cookieConfig.loginUrl;
        })
      );
  }

  tokenLogout(): Observable<any> {
    return from(this.oAuthService.revokeTokenAndLogout()).pipe(
      map((r) => {
        return this.authenticationConfig.tokenConfig.loginUrl;
      })
    );
  }

  sessionIsValid(): Observable<boolean> {
    return this.httpClient.get<boolean>(
      this.authenticationConfig.sessionValidationUrl
    );
  }

  xsrfToken() {
    this.httpClient
      .get<string>(this.authenticationConfig.cookieConfig.xsrfEndPoint)
      .subscribe((r) => {
        sessionStorage.setItem(
          this.authenticationConfig.cookieConfig.xsrfHeaderKey,
          r
        );
      });
  }
}
