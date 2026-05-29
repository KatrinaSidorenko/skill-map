import { fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { env } from 'next-runtime-env';

export const baseQuery = (baseUrl: string) =>
  fetchBaseQuery({
    baseUrl: `${env('NEXT_PUBLIC_API_URL')}/${baseUrl}`,
    prepareHeaders: (headers) => {
      headers.set('accept', 'application/json');
      const token = localStorage.getItem('skill-map-token');
      if (token) {
        headers.set('Authorization', `Bearer ${token}`);
      }

      return headers;
    },
  });
