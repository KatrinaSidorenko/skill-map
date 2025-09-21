import { baseQuery } from '@/store/baseQuery';
import { createApi } from '@reduxjs/toolkit/query';
import { setUser } from './store';

export const accountApi = createApi({
  reducerPath: 'accountApi',
  baseQuery: baseQuery('/api/account'),
  endpoints: (builder) => ({
    login: builder.mutation<AuthResponse, LoginRequest>({
      query: (credentials) => ({
        url: '/login',
        method: 'POST',
        body: credentials,
      }),
      onQueryStarted: async (arg, { dispatch, queryFulfilled }) => {
        const { data } = await queryFulfilled;
        dispatch(setUser(data.user));
      },
    }),
    register: builder.mutation<void, RegistrationReguest>({
      query: (userData) => ({
        url: '/register',
        method: 'POST',
        body: userData,
      }),
    }),
  }),
});
