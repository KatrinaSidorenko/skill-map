'use client';

import { useAppSelector } from '@/store/hooks';
import React, { createContext, useContext, useEffect, useState } from 'react';
import { selectToken, selectUser } from './store';
import { usePathname, useRouter } from 'next/navigation';
import { useLazyGetMeQuery } from './api';

interface AuthContextType {
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (token: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);
export const TOKEN_KEY = 'skill-map-token';

const SKIP_AUTH_PAGES = ['/login', '/register', '/forgot-password'];

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [isInitialized, setIsInitialized] = useState<boolean>(false);
  const token = useAppSelector(selectToken);
  const userData = useAppSelector(selectUser);
  const router = useRouter();
  const path = usePathname();
  const [getMeTrigger] = useLazyGetMeQuery({});

  const login = (token: string) => {
    localStorage.setItem(TOKEN_KEY, token);
    setIsAuthenticated(true);
  };

  if (token && !isAuthenticated) {
    login(token);
  }

  const fetchUserData = async () => {
    try {
      await getMeTrigger().unwrap();
    } catch (error) {
      console.error('Failed to fetch user data:', error);
      localStorage.removeItem(TOKEN_KEY);
      setIsAuthenticated(false);
    }
  };

  useEffect(() => {
    if (SKIP_AUTH_PAGES.some((p) => path.includes(p))) {
      console.log('Skipping auth check for path:', path);
      return;
    }

    const storageToken = localStorage.getItem(TOKEN_KEY);
    if (storageToken && !isAuthenticated) {
      console.log('Token found in localStorage, setting authenticated state.');
      login(storageToken);
      setIsAuthenticated(true);
      fetchUserData().finally(() => setIsLoading(false));
      return;
    }

    if (!isInitialized) {
      // Ensure this block runs only once
      setIsInitialized(true);

      const token = localStorage.getItem(TOKEN_KEY);
      if (token && !userData) {
        console.log('Token found, fetching user data...');
        setIsAuthenticated(true);
        async function fetchUser() {
          try {
            await getMeTrigger().unwrap();
            if (SKIP_AUTH_PAGES.some((p) => path.includes(p))) {
              router.replace('/home');
            }
          } catch (error) {
            console.error('Failed to fetch user data:', error);
            localStorage.removeItem(TOKEN_KEY);
            setIsAuthenticated(false);
          }
        }

        fetchUser();
      } else {
        router.replace('/login');
      }

      setIsLoading(false);
    }
  }, []);

  const logout = () => {
    localStorage.removeItem(TOKEN_KEY);
    setIsAuthenticated(false);
    router.replace('/login');
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
