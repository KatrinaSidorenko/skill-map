'use client';

import { useState } from 'react';
import { useGetRoadmapsQuery } from '../api';
import RoadmapGrid from '@/components/roadmap/roadmapGrid';
import SpinnerScreen from '@/components/base/spinner';
import SearchContainer from '@/components/search-container';
import ErrorScreen from '@/components/base/error';

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
  if (error) {
    return <ErrorScreen />;
  }
  
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
