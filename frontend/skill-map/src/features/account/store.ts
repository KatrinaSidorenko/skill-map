import { createSlice } from '@reduxjs/toolkit';
import { set } from 'zod';

type InitialState = {
  user?: AppUser;
  token?: string;
};

const initialState: InitialState = {
  user: undefined,
  token: undefined,
};

const accountSlice = createSlice({
  name: 'account',
  initialState,
  reducers: {
    setUser: (state, action) => {
      state.user = action.payload;
    },
    clearUser: (state) => {
      state.user = undefined;
    },
    setToken: (state, action) => {
      state.token = action.payload;
    }
  },
});

export const { setUser, clearUser } = accountSlice.actions;

export default accountSlice;

export const selectUser = (state: { account: InitialState }) => state.account.user;
export const selectToken = (state: { account: InitialState }) => state.account.token;