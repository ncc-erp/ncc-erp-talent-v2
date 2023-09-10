export class RoleForEdit {
    role: RoleEdit;
    permissions: SystemPermission[];
    grantedPermissionNames: string[];
}

export class RoleEdit {
    name: string | undefined;
    displayName: string | undefined;
    description: string | undefined;
    isStatic: boolean | undefined;
}

export class SystemPermission {
    name: string | undefined;
    displayName: string | undefined;
    children: SystemPermission[] | undefined;
}

export class RoleUserInfo {
    userRoleId: number;
    userId: number;
    fullName: string;
    surname: string;
    name: string;
    avatarPath: string;
    linkAvatar: string;
    phone: string;
    email: string;
    branchName: string;
}
