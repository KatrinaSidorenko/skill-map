'use client';

import { useState } from 'react';
import { SavedRoadmapCard } from '@/components/roadmap/roadmapCard';
import SearchContainer from '@/components/search-container';
import { useLazyGetSavedRoadmapsQuery, useDeleteRoadmapMutation } from '../api';
import { Flex } from '@chakra-ui/react';
import { defaultPagination } from '../helpers';
import useLocalization from '@/i18n/useLocalization';
import { useRouter } from 'next/navigation';
import { DeleteSavedRoadmapDialog } from '@/components/roadmap/deleteSavedRoadmapDialog';
import { toaster } from '@/components/ui/toaster';
import { retrieveErrorData } from '@/store/helpers';

export default function SavedRoadmapsPage() {
  const router = useRouter();
  const { pageSize } = defaultPagination;
  const { getRoadmapsTranslations, getRoadmapTransaltions } = useLocalization();
  const [fetchSavedRoadmaps] = useLazyGetSavedRoadmapsQuery();
  const [deleteRoadmap, { isLoading: isDeleting }] = useDeleteRoadmapMutation();

  const [dialogState, setDialogState] = useState<{
    isOpen: boolean;
    selected: SavedPlainRoadmap | null;
  }>({ isOpen: false, selected: null });

  const [refreshKey, setRefreshKey] = useState(0);

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
    router.push(`/saved/roadmap/${id}`);
  };

  const handleDeleteClick = (roadmap: SavedPlainRoadmap) => {
    setDialogState({ isOpen: true, selected: roadmap });
  };

  const handleDialogClose = () => {
    if (!isDeleting) {
      setDialogState({ isOpen: false, selected: null });
    }
  };

  const handleConfirmDelete = async (isSoftDelete: boolean) => {
    if (!dialogState.selected) return;
    try {
      await deleteRoadmap({
        id: dialogState.selected.id,
        isSoftDelete,
      }).unwrap();
      toaster.create({
        title: getRoadmapTransaltions('deleteSuccess'),
        type: 'success',
        closable: true,
      });
      setDialogState({ isOpen: false, selected: null });
      setRefreshKey((k) => k + 1);
    } catch (error) {
      const errorData = retrieveErrorData(error);
      toaster.create({
        title: getRoadmapTransaltions('failedToDeleteSavedRoadmap'),
        type: 'error',
        description: errorData?.message ?? 'Unexpected error',
        closable: true,
      });
    }
  };

  return (
    <>
      <SearchContainer
        key={refreshKey}
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
                onDelete={handleDeleteClick}
              />
            ))}
          </Flex>
        )}
      />

      <DeleteSavedRoadmapDialog
        isOpen={dialogState.isOpen}
        onClose={handleDialogClose}
        onConfirm={handleConfirmDelete}
        isLoading={isDeleting}
      />
    </>
  );
}
