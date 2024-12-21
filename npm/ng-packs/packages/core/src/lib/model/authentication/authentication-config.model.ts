/*
 * @Author: Chao Yang
 * @Date: 2022-08-08 23:09:39
 * @LastEditor: Chao Yang
 * @LastEditTime: 2022-08-29 22:35:35
 */
import { AuthConfig } from 'angular-oauth2-oidc';
import { AuthenticationMode } from './authentcation-mode.enum';
import { ICookieConfig } from './i-cookie-config.model';

export class AuthenticationConfig {
  authenticationMode: AuthenticationMode;
  tokenConfig: AuthConfig;
  cookieConfig: ICookieConfig;
  sessionValidationUrl: string;

  constructor() {
    this.authenticationMode = 0;
    this.tokenConfig = new AuthConfig();
    this.cookieConfig = {
      loginApiUrl: '',
      loginUrl: '',
      redirectUri: '',
      logoutApiUrl: [''],
      issuer: '',
      xsrf: true,
      xsrfEndPoint: '',
      xsrfHeaderKey: 'RequestVerificationToken',
      xsrfCacheKey: 'RequestVerificationToken'
    };
    this.sessionValidationUrl = '';
  }
}
