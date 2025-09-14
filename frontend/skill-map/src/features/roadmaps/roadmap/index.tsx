'use client';

import { useAppDispatch, useAppSelector } from '@/store/hooks';
import { addOrRemoveRoadmap, selectPlainRoadmap } from '../store';
import { IconButton } from '@chakra-ui/react';
import { FiStar } from 'react-icons/fi';
import { FaStar } from 'react-icons/fa';

export default function RoadmapPage({ roadmapId }: { roadmapId: string }) {
  const dispatch = useAppDispatch();
  const roadmap = useAppSelector((state) =>
    selectPlainRoadmap(state, Number(roadmapId)),
  );

  if (!roadmap) {
    return <div>Roadmap not found</div>;
  }

  return (
    <IconButton
      aria-label="Save Roadmap"
      size="sm"
      onClick={() => dispatch(addOrRemoveRoadmap(Number(roadmapId)))}
    >
      {roadmap.isSaved ? <FaStar /> : <FiStar />}
    </IconButton>
  );
}
