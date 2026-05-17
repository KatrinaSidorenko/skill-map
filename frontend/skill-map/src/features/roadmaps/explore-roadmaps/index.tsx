'use client';

import RoadmapGrid from '@/components/roadmap/roadmapGrid';
import { useLazyGetRoadmapsQuery } from '../api';
import SearchContainer from '@/components/search-container';
import useLocalization from '@/i18n/useLocalization';
import { defaultPagination } from '../helpers';
import { useRouter } from 'next/navigation';
import { RoadmapCard } from '@/components/roadmap/roadmapCard';

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
    };
  };

  const handleCardClick = (id: string) => {
    router.push(`explore/roadmap/${id}`);
  };

  const renderRoadmapCard = (roadmap: PlainRoadmap) => (
    <RoadmapCard
      key={roadmap.id}
      roadmap={roadmap}
      handleClick={handleCardClick}
    />
  );

  return (
    <SearchContainer
      placeholder={getRoadmapsTranslations('search')}
      pageSize={pageSize}
      fetchData={getRoadmaps}
      renderContent={(roadmaps) => (
        <RoadmapGrid
          roadmaps={roadmaps}
          renderRoadmapCard={renderRoadmapCard}
        />
      )}
    />
  );
}
