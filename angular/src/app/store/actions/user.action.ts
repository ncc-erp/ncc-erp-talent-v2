import { UserModel } from './../models/user.model';
import { createAction, props } from '@ngrx/store';

export const retrievedUserList = createAction(
    "[User API] User API Success",
    props<{allUser: UserModel[]}>()
);