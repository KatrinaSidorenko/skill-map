import { useTranslations } from 'next-intl';

export default function useLocalization() {
  const translation = useTranslations();

  const getHeaderTranslations = (key: keyof ILocalization['header']) => {
    return translation(`header.${key}`);
  };

  const getAuthTranslations = (key: keyof ILocalization['auth']) => {
    return translation(`auth.${key}`);
  };

  const getEditorTranslations = (key: keyof ILocalization['editor']) => {
    return translation(`editor.${key}`);
  };

  const getRoadmapsTranslations = (key: keyof ILocalization['roadmaps']) => {
    return translation(`roadmaps.${key}`);
  }

  return {
    getHeaderTranslations,
    getAuthTranslations,
    getEditorTranslations,
    getRoadmapsTranslations,
  };
}
