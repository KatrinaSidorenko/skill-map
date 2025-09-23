import { useTranslations } from 'next-intl';

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
