import { UserModel } from './../models/user.model';
import { createSelector } from "@ngrx/store";
import { AppState } from "../app.state";

export const userSelector = (state: AppState) => state.users;

export const allUsers = createSelector(
    userSelector,
    (users: UserModel[]) => {
        return users;
    }
)