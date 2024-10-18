export interface ICookieConfig {
  loginApiUrl: string;
  loginUrl: string;
  redirectUri: string;
  logoutApiUrl: string[];
  issuer: string;
  xsrf: boolean;
  xsrfEndPoint: string;
  xsrfHeaderKey: string;
}
