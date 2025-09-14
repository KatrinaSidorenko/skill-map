'use client';

import { useState } from 'react';
import { useGetRoadmapsQuery } from '../api';
import RoadmapGrid from '@/components/roadmap/roadmapGrid';
import SpinnerScreen from '@/components/spinner';
import SearchContainer from '@/components/search-container';

// todo: implement search and filtering
export default function ExploreRoadmapsPage() {
  const [page, setPage] = useState(1);
  const pageSize = 6;

  const { data, error, isLoading, isFetching } = useGetRoadmapsQuery({
    page,
    pageSize,
  });

  const setPageSafe = (newPage: number) => {
    if (newPage < 1) return 1;
    setPage(newPage);
    return newPage;
  };

  const roadmaps = data?.roadmaps ?? [];
  if (isLoading) {
    return <SpinnerScreen />;
  }

  return (
    <SearchContainer
      disabled={isFetching}
      page={page}
      setPage={setPageSafe}
      childeren={<RoadmapGrid roadmaps={roadmaps} />}
      pageSize={pageSize}
    />
  );
}
