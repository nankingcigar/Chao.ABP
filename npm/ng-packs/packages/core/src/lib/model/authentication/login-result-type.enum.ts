/*
 * @Author: Chao Yang
 * @Date: 2022-08-09 13:27:58
 * @LastEditor: Chao Yang
 * @LastEditTime: 2022-08-09 13:28:40
 */
export enum LoginResultType {
  Success = 1,
  InvalidUserNameOrPassword = 2,
  NotAllowed = 3,
  LockedOut = 4,
  RequiresTwoFactor = 5
}
