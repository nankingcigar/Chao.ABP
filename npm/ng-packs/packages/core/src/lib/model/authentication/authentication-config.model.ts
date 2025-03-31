import { AuthConfig } from 'angular-oauth2-oidc';
import { AuthenticationMode } from './authentcation-mode.enum';
import { ICookieConfig } from './i-cookie-config.model';

export class AuthenticationConfig {
  authenticationMode: AuthenticationMode;
  tokenConfig: AuthConfig;
  cookieConfig: ICookieConfig;
  sessionValidationUrl: string;
  xRequestedWith: string;
  logoutUri: string;

  constructor() {
    this.authenticationMode = 0;
    this.tokenConfig = new AuthConfig();
    this.cookieConfig = {
      loginApiUrl: '',
      redirectUri: '',
      loginUrl: '',
      logoutApiUrl: [''],
      issuer: '',
      xsrf: true,
      xsrfEndPoint: '',
      xsrfHeaderKey: 'RequestVerificationToken',
      xsrfCacheKey: 'RequestVerificationToken'
    };
    this.sessionValidationUrl = '';
    this.xRequestedWith = 'XMLHttpRequest';
    this.logoutUri = '';
  }
}
