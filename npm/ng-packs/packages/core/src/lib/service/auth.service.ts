import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import { forkJoin, from, map, Observable, of, Subject } from 'rxjs';
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
    public oAuthService: OAuthService,
    public authenticationConfig: AuthenticationConfig,
    public httpClient: HttpClient
  ) {
    switch (this.authenticationConfig.authenticationMode) {
      case AuthenticationMode.Cookie:
        this.login = this.cookieLogin;
        this.logout = this.cookieLogout;
        this.redirectUri = this.authenticationConfig.cookieConfig.redirectUri;
        this.loginUrl = this.authenticationConfig.cookieConfig.loginUrl;
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
    let logoutObservables: Observable<any>[] = [];
    this.authenticationConfig.cookieConfig.logoutApiUrl.forEach(logoutUrl => {
      logoutObservables.push(this.httpClient.get<any>(logoutUrl));
    });
    return forkJoin(logoutObservables).pipe(
      map((r) => {
        return this.authenticationConfig.logoutUri;
      })
    );
  }

  tokenLogout(): Observable<any> {
    return from(this.oAuthService.revokeTokenAndLogout()).pipe(
      map((r) => {
        return this.authenticationConfig.logoutUri;
      })
    );
  }

  sessionIsValid(): Observable<boolean> {
    return this.httpClient.get<boolean>(
      this.authenticationConfig.sessionValidationUrl
    );
  }
}
