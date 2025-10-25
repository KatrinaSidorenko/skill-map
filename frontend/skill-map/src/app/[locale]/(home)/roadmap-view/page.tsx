'use client';
import SpinnerScreen from '@/components/base/spinner';
import { useLazyGetPlainUserCreatedRoadmapQuery } from '@/features/roadmaps/api';
import RoadmapView from '@/features/roadmaps/roadmap-view';
import {
  selectRoadmapViewId,
  setRoadmapView,
} from '@/features/roadmaps/roadmap-view/store';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import { useEffect } from 'react';

export default function RoadmapViewPage() {
  const dispatch = useAppDispatch();
  const roadmapId = useAppSelector(selectRoadmapViewId);
  const [getRoadmap, { isLoading }] = useLazyGetPlainUserCreatedRoadmapQuery();

  useEffect(() => {
    if (!roadmapId) return;

    const fetchRoadmap = async () => {
      try {
        const roadmap = await getRoadmap(roadmapId).unwrap();
        dispatch(
          setRoadmapView({
            ...roadmap,
            isSaved: false,
          } as PlainRoadmapView),
        );
      } catch (error) {
        console.error('Failed to load roadmap:', error);
      }
    };

    fetchRoadmap();
  }, [roadmapId, dispatch, getRoadmap]);

  if (isLoading) {
    return <SpinnerScreen />;
  }

  return <RoadmapView />;
}
