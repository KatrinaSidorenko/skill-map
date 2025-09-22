import { useTranslations } from 'next-intl';

interface ILocalization {
  header: {
    welcome: string;
  };
  auth: {
    login: string;
    logout: string;
    signup: string;
    email: string;
    enterEmail: string;
    password: string;
    forgotPassword: string;
    passwordMinLength: string;
    noAccount: string;
    signUpHere: string;
    loginFailed: string;
    SE6: string;
    UIE1: string;
  };
}
export default function useLocalization() {
  const translation = useTranslations();

  const getHeaderTranslations = (key: keyof ILocalization['header']) => {
    return translation(`header.${key}`);
  };

  const getAuthTranslations = (key: keyof ILocalization['auth']) => {
    return translation(`auth.${key}`);
  };

  return {
    getHeaderTranslations,
    getAuthTranslations,
  };
}
