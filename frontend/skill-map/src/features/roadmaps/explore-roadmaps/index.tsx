'use client';

import RoadmapGrid from '@/components/roadmap/roadmapGrid';
import { useLazyGetRoadmapsQuery } from '../api';
import SearchContainer from '@/components/search-container';

export default function ExploreRoadmapsPage() {
  const [fetchRoadmaps] = useLazyGetRoadmapsQuery();

  const getRoadmaps = async (params: {
    pageNumber: number;
    pageSize: number;
    query: string | null;
  }) => {
    const { pageNumber, pageSize, query } = params;
    const { data } = await fetchRoadmaps({ pageNumber, pageSize, query });
    return {
      items: data?.items ?? [],
      total: data?.total ?? 0,
    };
  };

  return (
    <SearchContainer
      placeholder="Search roadmaps..."
      pageSize={12}
      fetchData={getRoadmaps}
      renderContent={(roadmaps) => <RoadmapGrid roadmaps={roadmaps} />}
    />
  );
}
