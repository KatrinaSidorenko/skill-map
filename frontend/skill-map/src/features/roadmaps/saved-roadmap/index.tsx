'use client';

import { useState } from 'react';
import { SavedRoadmapCard } from '@/components/roadmap/roadmapCard';
import SearchContainer from '@/components/search-container';
import {
  useLazyGetSavedRoadmapsQuery,
  useDeleteRoadmapMutation,
  useUpdateSavedRoadmapMutation,
  useCreateEmptyRoadmapMutation,
} from '../api';
import { Button, Flex, HStack } from '@chakra-ui/react';
import { defaultPagination } from '../helpers';
import useLocalization from '@/i18n/useLocalization';
import { useRouter } from 'next/navigation';
import { DeleteSavedRoadmapDialog } from '@/components/roadmap/deleteSavedRoadmapDialog';
import { EditSavedRoadmapDialog } from '@/components/roadmap/editSavedRoadmapDialog';
import { CreateSavedRoadmapDialog } from '@/components/roadmap/createSavedRoadmapDialog';
import { toaster } from '@/components/ui/toaster';
import { retrieveErrorData } from '@/store/helpers';
import { FiPlus } from 'react-icons/fi';

export default function SavedRoadmapsPage() {
  const router = useRouter();
  const { pageSize } = defaultPagination;
  const { getRoadmapsTranslations, getRoadmapTranslations } = useLocalization();
  const [fetchSavedRoadmaps] = useLazyGetSavedRoadmapsQuery();
  const [deleteRoadmap, { isLoading: isDeleting }] = useDeleteRoadmapMutation();
  const [updateSavedRoadmap, { isLoading: isUpdating }] =
    useUpdateSavedRoadmapMutation();
  const [createEmptyRoadmap, { isLoading: isCreating }] =
    useCreateEmptyRoadmapMutation();

  const [deleteDialogState, setDeleteDialogState] = useState<{
    isOpen: boolean;
    selected: SavedPlainRoadmap | null;
  }>({ isOpen: false, selected: null });

  const [editDialogState, setEditDialogState] = useState<{
    isOpen: boolean;
    selected: SavedPlainRoadmap | null;
  }>({ isOpen: false, selected: null });

  const [createDialogOpen, setCreateDialogOpen] = useState(false);
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
    setDeleteDialogState({ isOpen: true, selected: roadmap });
  };

  const handleEditClick = (roadmap: SavedPlainRoadmap) => {
    setEditDialogState({ isOpen: true, selected: roadmap });
  };

  const handleDeleteDialogClose = () => {
    if (!isDeleting) {
      setDeleteDialogState({ isOpen: false, selected: null });
    }
  };

  const handleEditDialogClose = () => {
    if (!isUpdating) {
      setEditDialogState({ isOpen: false, selected: null });
    }
  };

  const handleConfirmDelete = async (isSoftDelete: boolean) => {
    if (!deleteDialogState.selected) return;
    try {
      await deleteRoadmap({
        id: deleteDialogState.selected.id,
        isSoftDelete,
      }).unwrap();
      toaster.create({
        title: getRoadmapTranslations('deleteSuccess'),
        type: 'success',
        closable: true,
      });
      setDeleteDialogState({ isOpen: false, selected: null });
      setRefreshKey((k) => k + 1);
    } catch (error) {
      const errorData = retrieveErrorData(error);
      toaster.create({
        title: getRoadmapTranslations('failedToDeleteSavedRoadmap'),
        type: 'error',
        description: errorData?.message ?? 'Unexpected error',
        closable: true,
      });
    }
  };

  const handleConfirmEdit = async (payload: UpdateRoadmapWorkspaceRequest) => {
    if (!editDialogState.selected) return;
    try {
      await updateSavedRoadmap({
        id: editDialogState.selected.id,
        payload,
      }).unwrap();
      toaster.create({
        title: getRoadmapTranslations('editSuccess'),
        type: 'success',
        closable: true,
      });
      setEditDialogState({ isOpen: false, selected: null });
      setRefreshKey((k) => k + 1);
    } catch (error) {
      const errorData = retrieveErrorData(error);
      toaster.create({
        title: getRoadmapTranslations('failedToEditSavedRoadmap'),
        type: 'error',
        description: errorData?.message ?? 'Unexpected error',
        closable: true,
      });
    }
  };

  const handleConfirmCreate = async (
    payload: CreateEmptyRoadmapWorkspaceRequest,
  ) => {
    try {
      const result = await createEmptyRoadmap(payload).unwrap();
      toaster.create({
        title: getRoadmapTranslations('createEmptySuccess'),
        type: 'success',
        closable: true,
      });
      setCreateDialogOpen(false);
      setRefreshKey((k) => k + 1);
      if (result?.id) {
        router.push(`/saved/roadmap/${result.id}`);
      }
    } catch (error) {
      const errorData = retrieveErrorData(error);
      toaster.create({
        title: getRoadmapTranslations('failedToCreateEmpty'),
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
        rightHederElement={
          <HStack>
            <Button
              size="sm"
              colorPalette="green"
              onClick={() => setCreateDialogOpen(true)}
              loading={isCreating}
            >
              <FiPlus />
              {getRoadmapTranslations('newRoadmap')}
            </Button>
          </HStack>
        }
        renderContent={(items) => (
          <Flex direction="column" gap={4}>
            {items.map((roadmap: SavedPlainRoadmap) => (
              <SavedRoadmapCard
                key={roadmap.id}
                roadmap={roadmap}
                handleClick={handleCardClick}
                onDelete={handleDeleteClick}
                onEdit={handleEditClick}
              />
            ))}
          </Flex>
        )}
      />

      <DeleteSavedRoadmapDialog
        isOpen={deleteDialogState.isOpen}
        onClose={handleDeleteDialogClose}
        onConfirm={handleConfirmDelete}
        isLoading={isDeleting}
      />

      {editDialogState.selected && (
        <EditSavedRoadmapDialog
          isOpen={editDialogState.isOpen}
          onClose={handleEditDialogClose}
          onConfirm={handleConfirmEdit}
          isLoading={isUpdating}
          roadmap={editDialogState.selected}
        />
      )}

      <CreateSavedRoadmapDialog
        isOpen={createDialogOpen}
        onClose={() => setCreateDialogOpen(false)}
        onConfirm={handleConfirmCreate}
        isLoading={isCreating}
      />
    </>
  );
}
