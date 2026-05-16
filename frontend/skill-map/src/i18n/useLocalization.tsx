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
  };

  const getRoadmapTransaltions = (key: keyof ILocalization['roadmap']) => {
    return translation(`roadmap.${key}`);
  };

  const getGeneralTranslations = (key: keyof ILocalization['general']) => {
    return translation(`general.${key}`);
  };

  const getAssessmentTranslations = (key: keyof ILocalization['assessment']) => {
    return translation(`assessment.${key}`);
  };

  const getTestingHistoryTranslations = (key: keyof ILocalization['testingHistory']) => {
    return translation(`testingHistory.${key}`);
  };

  const getHomeTranslations = (key: keyof ILocalization['home']) => {
    return translation(`home.${key}`);
  };

  const getProfileTranslations = (key: keyof ILocalization['profile']) => {
    return translation(`profile.${key}`);
  };

  return {
    getHeaderTranslations,
    getAuthTranslations,
    getEditorTranslations,
    getRoadmapsTranslations,
    getRoadmapTransaltions,
    getGeneralTranslations,
    getAssessmentTranslations,
    getTestingHistoryTranslations,
    getHomeTranslations,
    getProfileTranslations,
  };
}
