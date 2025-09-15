'use client';

import RoadmapGrid from '@/components/roadmap/roadmapGrid';
import SearchContainer from '@/components/search-container';
import SpinnerScreen from '@/components/spinner';
import { useGetSavedRoadmapsQuery } from '../api';
import { useState } from 'react';

export default function SavedRoadmaps() {
  const [page, setPage] = useState(1);
  const pageSize = 6;

  const { data, error, isLoading, isFetching } = useGetSavedRoadmapsQuery({
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
      pageSize={pageSize}
      total={data?.total || 0}
    >
      <RoadmapGrid roadmaps={roadmaps} />
    </SearchContainer>
  );
}
