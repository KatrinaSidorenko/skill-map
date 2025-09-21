import { baseQuery } from '@/store/baseQuery';
import { createApi } from '@reduxjs/toolkit/query';

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
