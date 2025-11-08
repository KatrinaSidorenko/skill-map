'use client';

import { Text, IconButton } from '@chakra-ui/react';
import { useLazyGetUserCreatedRoadmapsQuery } from '../api';
import { IoIosAddCircle } from 'react-icons/io';
import useLocalization from '@/i18n/useLocalization';
import SearchContainer from '@/components/search-container';
import { defaultPagination } from '../helpers';
import RoadmapGrid from '@/components/roadmap/roadmapGrid';
import { useRouter } from 'next/navigation';
import { useAppDispatch } from '@/store/hooks';
import { createRoadmapDialog, updateRoadmapDialog } from './edit-dialog';
import { RoadmapCard } from '@/components/roadmap/roadmapCard';
import { setActiveRoadmapViewId } from '../roadmap-view/store';

export default function RoadmapsSandboxContainer() {
  const { getEditorTranslations, getRoadmapsTranslations } = useLocalization();
  const router = useRouter();
  const dispatch = useAppDispatch();
  const { pageSize } = defaultPagination;
  const [fetchRoadmaps] = useLazyGetUserCreatedRoadmapsQuery();
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

  const handleOpenCreate = () => {
    createRoadmapDialog.open('createRoadmapDialog', {
      onSuccess: () => {
        // reload data or invalidate cache
      },
    });
  };

  const handleCardClick = (id: string) => {
    dispatch(setActiveRoadmapViewId(id));
    router.push('/roadmap-view');
  };

  const renderRoadmapCard = (roadmap: PlainRoadmap) => (
    <RoadmapCard
      key={roadmap.id}
      roadmap={roadmap}
      handleClick={handleCardClick}
    />
  );

  return (
    <>
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
        leftHeaderElement={
          <Text fontSize="xl" fontWeight="bold">
            {getEditorTranslations('yourRoadmaps')}
          </Text>
        }
        rightHederElement={
          <IconButton
            aria-label="Add Roadmap"
            colorScheme="teal"
            onClick={handleOpenCreate}
            borderRadius="full"
            size="lg"
          >
            <IoIosAddCircle size={24} />
          </IconButton>
        }
      />

      {<createRoadmapDialog.Viewport />}
    </>
  );
}
