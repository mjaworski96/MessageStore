export interface LoggedUser {
  id: number;
  username: string;
  email: string;
}
export interface LoginDetails {
  username: string;
  password: string;
}
export interface RegisterUserDetails {
  username: string;
  password: string;
  email: string;
}
export interface UserUpdateData {
  username: string;
  email: string;
}
export interface PasswordChange {
  oldPassword: string;
  newPassword: string;
}
