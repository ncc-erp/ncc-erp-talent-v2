export interface UserModel{
    id: number;
    userName: string;
    name: string;
    surname: string;
    emailAddress: string;
    isActive: boolean;
    fullName: string | undefined;
    lastLoginTime: any;
    creationTime: any;
    roleNames: string[] | undefined;
}