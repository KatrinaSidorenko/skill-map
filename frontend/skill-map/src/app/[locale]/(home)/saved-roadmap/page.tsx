'use client';
import ContentNotFoundScreen from '@/components/base/notfound';
import SpinnerScreen from '@/components/base/spinner';
import { toaster } from '@/components/ui/toaster';
import { useLazyGetPlainUserSavedRoadmapQuery } from '@/features/roadmaps/api';
import SavedRoadmapView from '@/features/roadmaps/saved-roadmap-view';
import { selectRoadmapViewId } from '@/features/roadmaps/saved-roadmap-view/store';
import { useAppSelector } from '@/store/hooks';
import { useEffect } from 'react';

// todo: drawback of using state for this page is that on page reload the state is lost and user sees not found screen
export default function Page() {
  const savedRoadmapId = useAppSelector(selectRoadmapViewId);
  const [triggerGetRoadmap, { isLoading, data: savedRoadmap }] =
    useLazyGetPlainUserSavedRoadmapQuery();
  useEffect(() => {
    if (savedRoadmapId) {
      triggerGetRoadmap(savedRoadmapId)
        .unwrap()
        .catch(() => {
          toaster.error({
            title: 'Failed to load the saved roadmap. Please try again later.',
          });
        });
    }
  }, [savedRoadmapId, triggerGetRoadmap]);

  if (!savedRoadmapId) {
    return <ContentNotFoundScreen />;
  }

  if (isLoading || !savedRoadmap) {
    return <SpinnerScreen />;
  }

  return <SavedRoadmapView roadmap={savedRoadmap} />;
}
