'use client';

import { useAppSelector } from '@/store/hooks';
import React, { createContext, useContext, useEffect, useState } from 'react';
import { selectToken } from './store';
import { useRouter } from 'next/navigation';
import { useLazyGetMeQuery } from './api';

interface AuthContextType {
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (token: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);
export const TOKEN_KEY = 'skill-map-token';

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const token = useAppSelector(selectToken);
  const router = useRouter();
  const [getMeTrigger] = useLazyGetMeQuery({});

  const login = (token: string) => {
    localStorage.setItem(TOKEN_KEY, token);
    setIsAuthenticated(true);
  };

  if (token && !isAuthenticated) {
    login(token);
  }

  useEffect(() => {
    const token = localStorage.getItem(TOKEN_KEY);
    if (token) {
      setIsAuthenticated(true);
      async function fetchUser() {
        try {
          await getMeTrigger().unwrap();
        } catch (error) {
          console.error('Failed to fetch user data:', error);
          localStorage.removeItem(TOKEN_KEY);
          setIsAuthenticated(false);
        }
      }

      fetchUser();
      router.replace('/home');
    } else {
      router.replace('/login');
    }

    setIsLoading(false);
  }, []);

  const logout = () => {
    localStorage.removeItem(TOKEN_KEY);
    setIsAuthenticated(false);
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated, isLoading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
