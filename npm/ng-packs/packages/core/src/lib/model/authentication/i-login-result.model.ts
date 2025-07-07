import { LoginResultType } from './login-result-type.enum';

export interface IAbpLoginResult {
  result: LoginResultType;
  description: string;
}
