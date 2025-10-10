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
import RoadmapGrid from '@/components/roadmap/roadmapGrid';
import { defaultPagination } from '../helpers';
import { useLazyGetRoadmapsQuery } from '../api';
import { AiOutlineArrowLeft, AiOutlineArrowRight } from 'react-icons/ai';

export default function ExploreRoadmapsPage() {
  const { pageSize: defaultPageSize, pageNumber: defaultPageNumber } =
    defaultPagination;

  const [page, setPage] = useState(defaultPageNumber);
  const [items, setItems] = useState<PlainRoadmap[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);

  const [searchInput, setSearchInput] = useState('');
  const [search, setSearch] = useState<string | null>('');

  const [fetchRoadmaps, { data, error, isLoading, isFetching }] =
    useLazyGetRoadmapsQuery();

  // Fetch when page or search changes
  useEffect(() => {
    fetchRoadmaps({
      pageNumber: page,
      pageSize: defaultPageSize,
      query: search,
    });
  }, [page, search]);

  // Update data when response arrives
  useEffect(() => {
    if (data?.items) {
      setItems(data.items);
      setTotalCount(data.total ?? 0);
      setTotalPages(Math.round((data.total ?? 0) / defaultPageSize));
      console.log('Total items:', data.total ?? 0);
      console.log(
        'Total pages:',
        Math.round((data.total ?? 0) / defaultPageSize),
      );
    }
  }, [data]);

  const handleSearch = () => {
    setPage(1);
    setSearch(searchInput.trim().length > 0 ? searchInput.trim() : '');
  };

  const handlePrev = () => page > 1 && setPage((p) => p - 1);
  const handleNext = () => page < totalPages && setPage((p) => p + 1);

  if (error) return <ErrorScreen />;
  if (isLoading && page === 1) return <SpinnerScreen />;

  return (
    <Flex
      direction="column"
      minH="100vh"
      alignItems="center"
      justifyContent="flex-start"
    >
      {/* Search bar */}
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

      {/* Roadmap grid */}
      <Box flex="1" w="full" px={4}>
        {isFetching && items.length === 0 ? (
          <Flex justifyContent="center" py={8}>
            <Spinner />
          </Flex>
        ) : items.length > 0 ? (
          <RoadmapGrid roadmaps={items} />
        ) : (
          <Text textAlign="center" color="gray.500" py={8}>
            No roadmaps found.
          </Text>
        )}
      </Box>

      <Flex alignItems="center" justifyContent="center" gap={4} py={6}>
        <Button
          onClick={handlePrev}
          disabled={page === 1 || isFetching}
          size="sm"
        >
          <AiOutlineArrowLeft />
        </Button>

        <Text fontSize="sm" color="gray.600">
          Page {page} of {totalPages || 1}
        </Text>

        <Button
          onClick={handleNext}
          disabled={page >= totalPages || isFetching}
          size="sm"
        >
          <AiOutlineArrowRight />
        </Button>
      </Flex>
    </Flex>
  );
}
