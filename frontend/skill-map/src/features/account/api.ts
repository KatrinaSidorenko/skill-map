import { baseQuery } from '@/store/baseQuery';
import { setUser } from './store';
import { createApi } from '@reduxjs/toolkit/query/react';

export const accountApi = createApi({
  reducerPath: 'accountApi',
  baseQuery: baseQuery('account'),
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
    register: builder.mutation<void, RegistrationRequest>({
      query: (userData) => ({
        url: '/register',
        method: 'POST',
        body: userData,
      }),
    }),
    getMe: builder.query<AppUser, void>({
      query: () => ({
        url: '/me',
        method: 'GET',
      }),
      onQueryStarted: async (arg, { dispatch, queryFulfilled }) => {
        const { data } = await queryFulfilled;
        dispatch(setUser(data));
      },
    }),
    resetPassword: builder.mutation<void, PasswordResetRequest>({
      query: (request) => ({
        url: '/reset-password',
        method: 'POST',
        body: request,
      }),
    }),
    setNewPassword: builder.mutation<void, SetNewPasswordRequest>({
      query: (request) => ({
        url: '/set-new-password',
        method: 'POST',
        body: request,
      }),
    }),
    verifyToken: builder.mutation<void, { token: string }>({
      query: ({ token }) => ({
        url: `/verify-token?token=${token}`,
        method: 'GET',
      }),
    }),
    getProfile: builder.query<AppUser, void>({
      query: () => ({
        url: '/profile',
        method: 'GET',
      }),
      onQueryStarted: async (arg, { dispatch, queryFulfilled }) => {
        const { data } = await queryFulfilled;
        dispatch(setUser(data));
      },
    }),
    updateProfile: builder.mutation<AppUser, UpdateProfileRequest>({
      query: (request) => ({
        url: '/profile',
        method: 'PATCH',
        body: request,
      }),
      onQueryStarted: async (arg, { dispatch, queryFulfilled }) => {
        const { data } = await queryFulfilled;
        dispatch(setUser(data));
      },
    }),
  }),
});

export const {
  useLoginMutation,
  useRegisterMutation,
  useLazyGetMeQuery,
  useResetPasswordMutation,
  useVerifyTokenMutation,
  useSetNewPasswordMutation,
  useLazyGetProfileQuery,
  useGetProfileQuery,
  useUpdateProfileMutation,
} = accountApi;
