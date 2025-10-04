'use client';

import { useEffect, useRef, useState } from 'react';
import {
  Box,
  Button,
  Flex,
  Input,
  InputGroup,
  Spinner,
} from '@chakra-ui/react';
import { LuSearch } from 'react-icons/lu';
import ErrorScreen from '@/components/base/error';
import SpinnerScreen from '@/components/base/spinner';
import RoadmapGrid from '@/components/roadmap/roadmapGrid';
import { defaultPagination } from '../helpers';
import { useLazyGetRoadmapsQuery } from '../api';

export default function ExploreRoadmapsPage() {
  const { pageSize: defaultPageSize, pageNumber: defaultPageNumber } =
    defaultPagination;

  const [page, setPage] = useState(defaultPageNumber);
  const [items, setItems] = useState<PlainRoadmap[]>([]);
  const [hasMore, setHasMore] = useState(true);

  // Separate input vs active search query
  const [searchInput, setSearchInput] = useState('');
  const [search, setSearch] = useState<string | null>(null);

  // Lazy query hook
  const [fetchRoadmaps, { data, error, isLoading, isFetching }] =
    useLazyGetRoadmapsQuery();

  // Fetch data when page or search changes
  useEffect(() => {
    fetchRoadmaps({
      pageNumber: page,
      pageSize: defaultPageSize,
      query: search,
    });
  }, [page, search]);

  // Update items when data arrives
  useEffect(() => {
    if (data?.items) {
      if (page === 1) {
        // Reset on new search
        setItems(data.items);
      } else {
        // Append new items without duplicates
        const newItems = data.items.filter(
          (newItem) => !items.some((item) => item.id === newItem.id),
        );
        setItems((prev) => [...prev, ...newItems]);
      }

      // If less than pageSize → no more data
      if (data.items.length < defaultPageSize) {
        setHasMore(false);
      }
    }
  }, [data, page]);

  // Infinite scroll observer
  const loaderRef = useRef<HTMLDivElement | null>(null);
  useEffect(() => {
    if (!hasMore || isFetching) return;

    const observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          setPage((prev) => prev + 1);
        }
      },
      { threshold: 1.0 },
    );

    if (loaderRef.current) observer.observe(loaderRef.current);
    return () => {
      if (loaderRef.current) observer.unobserve(loaderRef.current);
    };
  }, [hasMore, isFetching]);

  // Handle pressing search button
  const handleSearch = () => {
    setItems([]);
    setPage(1);
    setHasMore(true);
    setSearch(searchInput.trim().length > 0 ? searchInput.trim() : '');
  };

  if (error) return <ErrorScreen />;
  if (isLoading && page === 1) return <SpinnerScreen />;

  return (
    <Flex
      direction="column"
      gap={4}
      alignItems="center"
      justifyContent="center"
    >
      {/* Search bar with button */}
      <Box w="sm" p={4} mb={8}>
        <InputGroup borderRadius="md" bg="bg.page" boxShadow="sm">
          <>
            <Input
              placeholder="Search roadmaps..."
              value={searchInput}
              onChange={(e) => setSearchInput(e.target.value)}
            />
            <Button onClick={handleSearch} variant="ghost">
              <LuSearch />
            </Button>
          </>
        </InputGroup>
      </Box>

      {/* Roadmap grid */}
      <Box flex="1" w="full" px={4}>
        <RoadmapGrid roadmaps={items} />

        {/* Loader for infinite scroll */}
        {hasMore && (
          <Box ref={loaderRef} display="flex" justifyContent="center" py={4}>
            {isFetching && <Spinner />}
          </Box>
        )}
      </Box>
    </Flex>
  );
}
