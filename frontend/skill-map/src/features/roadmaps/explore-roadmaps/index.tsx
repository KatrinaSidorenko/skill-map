'use client';

import RoadmapGrid from '@/components/roadmap/roadmapGrid';
import { useLazyGetRoadmapsQuery } from '../api';
import SearchContainer from '@/components/search-container';
import useLocalization from '@/i18n/useLocalization';
import { defaultPagination } from '../helpers';
import { useRouter } from 'next/navigation';

export default function ExploreRoadmapsPage() {
  const { pageSize } = defaultPagination;
  const router = useRouter();
  const { getRoadmapsTranslations } = useLocalization();
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

  const handleCardClick = (id: string) => {
    router.push(`/roadmaps/${id}`);
  };

  return (
    <SearchContainer
      placeholder={getRoadmapsTranslations('search')}
      pageSize={pageSize}
      fetchData={getRoadmaps}
      renderContent={(roadmaps) => (
        <RoadmapGrid roadmaps={roadmaps} handleClick={handleCardClick} />
      )}
    />
  );
}
