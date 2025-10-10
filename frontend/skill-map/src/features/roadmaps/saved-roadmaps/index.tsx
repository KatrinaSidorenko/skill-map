'use client';

import { SavedRoadmapCard } from '@/components/roadmap/roadmapCard';
import SearchContainer from '@/components/search-container';
import { useLazyGetSavedRoadmapsQuery } from '../api';
import { Flex } from '@chakra-ui/react';
import { defaultPagination } from '../helpers';
import useLocalization from '@/i18n/useLocalization';
import { useAppDispatch } from '@/store/hooks';
import { setActiveRoadmapId } from '../editor/store';
import { useRouter } from 'next/navigation';

export default function SavedRoadmapsPage() {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const { pageSize } = defaultPagination;
  const { getRoadmapsTranslations } = useLocalization();
  const [fetchSavedRoadmaps] = useLazyGetSavedRoadmapsQuery();

  const getSavedRoadmaps = async (params: {
    pageNumber: number;
    pageSize: number;
    query: string | null;
  }) => {
    const { pageNumber, pageSize, query } = params;
    const { data } = await fetchSavedRoadmaps({ pageNumber, pageSize, query });
    return {
      items: data?.items ?? [],
      total: data?.total ?? 0,
    };
  };

  const handleCardClick = (id: string) => {
    dispatch(setActiveRoadmapId(id));
    router.push('/editor');
  };

  return (
    <SearchContainer
      placeholder={getRoadmapsTranslations('search')}
      pageSize={pageSize}
      fetchData={getSavedRoadmaps}
      renderContent={(items) => (
        <Flex direction="column" gap={4}>
          {items.map((roadmap: SavedPlainRoadmap) => (
            <SavedRoadmapCard
              key={roadmap.id}
              roadmap={roadmap}
              handleClick={handleCardClick}
            />
          ))}
        </Flex>
      )}
    />
  );
}
