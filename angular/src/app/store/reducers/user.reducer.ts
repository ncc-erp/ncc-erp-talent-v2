import { createReducer, on } from '@ngrx/store';
import { retrievedUserList } from '../actions/user.action';
import { UserModel } from './../models/user.model';


export const initialState: ReadonlyArray<UserModel> = [];

const _userReducer = createReducer(
    initialState,
    on(retrievedUserList, (state, {allUser}) => {
        return [...allUser]
    })
);

export function userReducer(state: any, action: any){
    return _userReducer(state,action);
}