/*
 * @Author: Chao Yang
 * @Date: 2022-08-09 13:27:22
 * @LastEditor: Chao Yang
 * @LastEditTime: 2022-08-09 13:29:51
 */
import { LoginResultType } from './login-result-type.enum';

export interface IAbpLoginResult {
  result: LoginResultType;
  description: string;
}
