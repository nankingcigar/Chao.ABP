import { HttpEvent, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { OAuthService } from 'angular-oauth2-oidc';
import {
  AuthenticationConfig,
  AuthenticationMode,
} from '../model/authentication/index';

@Injectable()
export class InterceptorService {
  redirectUri: string;
  loginUrl: string;
  issuer: string;
  logoutUrl: string;

  constructor(
    public authenticationConfig: AuthenticationConfig,
    public oAuthService: OAuthService
  ) {
    switch (this.authenticationConfig.authenticationMode) {
      case AuthenticationMode.Cookie:
        this.redirectUri = this.authenticationConfig.cookieConfig.redirectUri;
        this.loginUrl = this.authenticationConfig.cookieConfig.loginUrl;
        this.issuer = this.authenticationConfig.cookieConfig.issuer;
        break;
      case AuthenticationMode.Token:
        this.redirectUri = this.authenticationConfig.tokenConfig
          .redirectUri as string;
        this.loginUrl = this.authenticationConfig.tokenConfig
          .loginUrl as string;
        this.issuer = this.authenticationConfig.tokenConfig.issuer as string;
        break;
      default:
        this.redirectUri = this.authenticationConfig.cookieConfig.redirectUri;
        this.loginUrl = this.authenticationConfig.cookieConfig.loginUrl;
        this.issuer = this.authenticationConfig.cookieConfig.issuer;
        break;
    }
    this.logoutUrl = this.authenticationConfig.logoutUri;
  }

  handleBeforeRequest(request: HttpRequest<any>): HttpRequest<any> {
    const headers = {} as any;
    this.setXsrfToken(headers);
    const requestGhost = request.clone({
      setHeaders: headers,
      url: this.handleUrl(request.url),
    });
    return requestGhost;
  }

  handleSuccessResponse(response: HttpEvent<any>): HttpEvent<any> {
    if (response instanceof HttpResponse) {
      if (response.body && response.body.__chao) {
        if (response.body.success) {
          (response as any).body = response.body.result;
        } else {
          throw new Error(response.body.error);
        }
      }
    }
    return response;
  }

  handleErrorResponse(errorResponse: any): any {
    if (errorResponse.status === 401) {
      this.clearFor401();
      location.href = this.loginUrl;
    } else if (errorResponse.status === 403) {
      if (
        errorResponse.error !== null &&
        errorResponse.error !== undefined
      ) {
        if (errorResponse.error.__chao === true ||
          (
            errorResponse.error.error !== null &&
            errorResponse.error.error !== undefined
          )
        ) {
          return errorResponse.error.error;
        }
        return new Error(errorResponse.error);
      } else {
        location.href = this.redirectUri;
      }
    } else if (
      errorResponse.error !== null &&
      errorResponse.error !== undefined
    ) {
      if (errorResponse.error.__chao === true ||
        (
          errorResponse.error.error !== null &&
          errorResponse.error.error !== undefined
        )
      ) {
        return errorResponse.error.error;
      }
      return new Error(errorResponse.error);
    }
    return errorResponse;
  }

  clearFor401(): void {
    this.oAuthService.logOut();
  }

  handleUrl(url: string): string {
    if (
      url.startsWith('http://') === false &&
      url.startsWith('https://') === false &&
      (url.startsWith('api/') === true || url.startsWith('/api/') === true)
    ) {
      return this.issuer + url;
    }
    return url;
  }

  setXsrfToken(headers: { [key: string]: string }) {
    if (
      this.authenticationConfig.authenticationMode ===
      AuthenticationMode.Cookie &&
      this.authenticationConfig.cookieConfig.xsrf === true
    ) {
      const xsrf = sessionStorage.getItem(
        this.authenticationConfig.cookieConfig.xsrfCacheKey
      );
      if (xsrf !== undefined && xsrf !== null) {
        headers[this.authenticationConfig.cookieConfig.xsrfHeaderKey] = xsrf;
      }
    }
    if (this.authenticationConfig.xRequestedWith !== undefined && this.authenticationConfig.xRequestedWith !== null && this.authenticationConfig.xRequestedWith !== '') {
      headers['X-Requested-With'] = this.authenticationConfig.xRequestedWith;
    }
  }
}
