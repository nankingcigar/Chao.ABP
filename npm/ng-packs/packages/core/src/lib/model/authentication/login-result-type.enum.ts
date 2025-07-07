export enum LoginResultType {
  Success = 1,
  InvalidUserNameOrPassword = 2,
  NotAllowed = 3,
  LockedOut = 4,
  RequiresTwoFactor = 5
}
