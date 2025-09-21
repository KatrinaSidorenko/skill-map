import { fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const baseQuery = (baseUrl: string) =>
  fetchBaseQuery({
    baseUrl: `${process.env.NEXT_PUBLIC_API_URL}${baseUrl}`,
    prepareHeaders: (headers) => {
      // const token = localStorage.getItem('token');
      // if (token) headers.set('authorization', `Bearer ${token}`);
      return headers;
    },
  });
