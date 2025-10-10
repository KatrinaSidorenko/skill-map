'use client';

import { SavedRoadmapCard } from '@/components/roadmap/roadmapCard';
import SearchContainer from '@/components/search-container';
import { useLazyGetSavedRoadmapsQuery } from '../api';
import { Flex } from '@chakra-ui/react';

export default function SavedRoadmapsPage() {
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

  return (
    <SearchContainer
      placeholder="Search saved roadmaps..."
      pageSize={10}
      fetchData={getSavedRoadmaps}
      renderContent={(items) => (
        <Flex direction="column" gap={4}>
          {items.map((roadmap: SavedPlainRoadmap) => (
            <SavedRoadmapCard key={roadmap.id} roadmap={roadmap} />
          ))}
        </Flex>
      )}
    />
  );
}
