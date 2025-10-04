'use client';

import { useEffect, useRef, useState } from 'react';
import { Box, Flex, Input, InputGroup, Spinner } from '@chakra-ui/react';
import { LuSearch } from 'react-icons/lu';
import ErrorScreen from '@/components/base/error';
import SpinnerScreen from '@/components/base/spinner';
import { RoadmapCard } from '@/components/roadmap/roadmapCard';
import { defaultPagination } from '../helpers';
import { useLazyGetSavedRoadmapsQuery } from '../api';

export default function SavedRoadmaps() {
  const { pageSize: defaultPageSize, pageNumber: defaultPageNumber } =
    defaultPagination;

  const [page, setPage] = useState(defaultPageNumber);
  const [items, setItems] = useState<PlainRoadmap[]>([]);
  const [hasMore, setHasMore] = useState(true);
  const [search, setSearch] = useState<string | null>('');

  // Lazy query hook
  const [fetchRoadmaps, { data, error, isLoading, isFetching }] =
    useLazyGetSavedRoadmapsQuery();

  // Fetch data when page changes or query changes
  useEffect(() => {
    if (!hasMore) return;
    fetchRoadmaps({
      pageNumber: page,
      pageSize: defaultPageSize,
      query: search,
    });
  }, [page, search, fetchRoadmaps, hasMore, defaultPageSize]);

  // Accumulate new items or reset on query change
  useEffect(() => {
    if (data?.items) {
      if (page === 1) {
        // replace items when starting new search
        setItems(data.items);
      } else {
        // append for infinite scroll
        const newItems = data.items.filter(
          (newItem) => !items.some((item) => item.id === newItem.id),
        );
        setItems((prev) => [...prev, ...newItems]);
      }

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

  // Handle search input change
  const handleSearch = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value.trim();
    setItems([]);
    setPage(1);
    setHasMore(true);
    setSearch(value.length > 0 ? value : null);
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
      {/* Search bar */}
      <Box w="sm" p={4} mb={8}>
        <InputGroup
          borderRadius="md"
          bg="bg.page"
          boxShadow="sm"
          endElement={<LuSearch />}
        >
          <Input
            placeholder="Search roadmaps..."
            onChange={handleSearch}
            defaultValue={search ?? ''}
          />
        </InputGroup>
      </Box>

      {/* Roadmap list */}
      <Box flex="1" w="full" px={4}>
        <Flex direction="column" gap={4} alignItems="stretch">
          {items.map((roadmap) => (
            <RoadmapCard key={roadmap.id} roadmap={roadmap} />
          ))}
        </Flex>

        {hasMore && (
          <Box ref={loaderRef} display="flex" justifyContent="center" py={4}>
            {isFetching && <Spinner />}
          </Box>
        )}
      </Box>
    </Flex>
  );
}
