'use client';

import { useEffect, useState } from 'react';
import {
  Box,
  Button,
  Flex,
  Input,
  InputGroup,
  Spinner,
  Text,
} from '@chakra-ui/react';
import { LuSearch } from 'react-icons/lu';
import ErrorScreen from '@/components/base/error';
import SpinnerScreen from '@/components/base/spinner';
import { SavedRoadmapCard } from '@/components/roadmap/roadmapCard';
import { defaultPagination } from '../helpers';
import { useLazyGetSavedRoadmapsQuery } from '../api';
import { AiOutlineArrowLeft, AiOutlineArrowRight } from 'react-icons/ai';

export default function SavedRoadmaps() {
  const { pageSize: defaultPageSize, pageNumber: defaultPageNumber } =
    defaultPagination;

  const [page, setPage] = useState(defaultPageNumber);
  const [items, setItems] = useState<SavedPlainRoadmap[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [search, setSearch] = useState<string | null>('');
  const [searchInput, setSearchInput] = useState('');

  const [fetchRoadmaps, { data, error, isLoading, isFetching }] =
    useLazyGetSavedRoadmapsQuery();

  useEffect(() => {
    fetchRoadmaps({
      pageNumber: page,
      pageSize: defaultPageSize,
      query: search,
    });
  }, [page, search]);

  useEffect(() => {
    if (data?.items) {
      setItems(data.items);
      setTotalCount(data.total ?? 0);
    }
  }, [data]);

  const handleSearch = () => {
    setPage(1);
    setSearch(searchInput.trim().length > 0 ? searchInput.trim() : '');
  };

  const handlePrev = () => {
    if (page > 1) setPage((p) => p - 1);
  };

  const handleNext = () => {
    const totalPages = Math.ceil(totalCount / defaultPageSize);
    if (page < totalPages) setPage((p) => p + 1);
  };

  if (error) return <ErrorScreen />;
  if (isLoading && page === 1) return <SpinnerScreen />;

  const totalPages = Math.ceil(totalCount / defaultPageSize);

  return (
    <Flex
      direction="column"
      alignItems="center"
      justifyContent="center"
      gap={6}
      minH="100%"
    >
      <Box w="sm" p={4} mb={8}>
        <InputGroup
          borderRadius="md"
          bg="bg.page"
          boxShadow="sm"
          endElement={
            <LuSearch onClick={handleSearch} style={{ cursor: 'pointer' }} />
          }
        >
          <Input
            placeholder="Search roadmaps..."
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
          />
        </InputGroup>
      </Box>

      {/* Roadmap List */}
      <Box flex="1" w="full" px={4}>
        {isFetching && items.length === 0 ? (
          <Flex justifyContent="center" py={8}>
            <Spinner />
          </Flex>
        ) : items.length > 0 ? (
          <Flex direction="column" gap={4}>
            {items.map((roadmap) => (
              <SavedRoadmapCard key={roadmap.id} roadmap={roadmap} />
            ))}
          </Flex>
        ) : (
          <Text textAlign="center" color="gray.500" py={8}>
            No roadmaps found.
          </Text>
        )}
      </Box>

      {/* Pagination Controls */}
      <Flex
        alignItems="center"
        justifyContent="center"
        justifySelf="flex-end"
        gap={4}
        py={6}
      >
        <Button onClick={handlePrev} disabled={page === 1 || isFetching}>
          <AiOutlineArrowLeft />
        </Button>

        <Text fontSize="sm" color="gray.600">
          Page {page} of {totalPages || 1}
        </Text>

        <Button
          onClick={handleNext}
          disabled={page >= totalPages || isFetching}
        >
          <AiOutlineArrowRight />
        </Button>
      </Flex>
    </Flex>
  );
}
