import { fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const baseQuery = (baseUrl: string) =>
  fetchBaseQuery({
    baseUrl: `${process.env.NEXT_PUBLIC_API_URL}${baseUrl}`,
    prepareHeaders: (headers) => {
      const token = localStorage.getItem('skill-map-token');
      if (token) headers.set('Authorization', `Bearer ${token}`);
      return headers;
    },
  });
