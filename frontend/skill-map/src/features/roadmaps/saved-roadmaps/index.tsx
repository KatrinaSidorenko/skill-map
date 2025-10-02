'use client';

import SpinnerScreen from '@/components/base/spinner';
import { useLazyGetRoadmapsQuery } from '../api';
import { useEffect, useRef, useState } from 'react';
import ErrorScreen from '@/components/base/error';
import { Box, Flex, Input, InputGroup, Spinner } from '@chakra-ui/react';
import { LuSearch } from 'react-icons/lu';
import { defaultPagination } from '../helpers';
import { RoadmapCard } from '@/components/roadmap/roadmapCard';

export default function SavedRoadmaps() {
  const { pageSize: defaultPageSize, pageNumber: defaultPageNumber } =
    defaultPagination;

  const [page, setPage] = useState(defaultPageNumber);
  const [items, setItems] = useState<PlainRoadmap[]>([]);
  const [hasMore, setHasMore] = useState(true);

  // Lazy query hook
  const [fetchRoadmaps, { data, error, isLoading, isFetching }] =
    useLazyGetRoadmapsQuery();

  // Fetch data when page changes
  useEffect(() => {
    if (!hasMore) return;
    fetchRoadmaps({ pageNumber: page, pageSize: defaultPageSize });
  }, [page, fetchRoadmaps, hasMore, defaultPageSize]);

  // Accumulate new items
  useEffect(() => {
    if (data?.roadmaps) {
      const newItems = data.roadmaps.filter(
        (newItem) => !items.some((item) => item.id === newItem.id),
      );
      setItems((prev) => [...prev, ...newItems]);

      if (data.roadmaps.length < defaultPageSize) {
        setHasMore(false);
      }
    }
  }, [data]);

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
          <Input placeholder="Search roadmaps..." />
        </InputGroup>
      </Box>

      {/* Roadmap grid */}
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
