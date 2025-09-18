import { createSlice } from '@reduxjs/toolkit';

type InitialState = {
  user?: AppUser;
};

const initialState: InitialState = {
  user: undefined,
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
  },
});

export const { setUser, clearUser } = accountSlice.actions;

export default accountSlice;

export const selectUser = (state: { account: InitialState }) => state.account.user;