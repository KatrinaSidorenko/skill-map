'use client';

import { useEffect, useState, ReactNode } from 'react';
import {
  Box,
  Button,
  Flex,
  HStack,
  Input,
  InputGroup,
  Spinner,
  Text,
} from '@chakra-ui/react';
import { LuSearch } from 'react-icons/lu';
import { AiOutlineArrowLeft, AiOutlineArrowRight } from 'react-icons/ai';
import ErrorScreen from '@/components/base/error';
import SpinnerScreen from '@/components/base/spinner';
import ContentNotFoundScreen from '../base/notfound';
import useLocalization from '@/i18n/useLocalization';

interface SearchContainerProps<T> {
  placeholder?: string;
  pageSize?: number;
  fetchData: (params: {
    pageNumber: number;
    pageSize: number;
    query: string | null;
  }) => Promise<{ items: T[] }>;
  renderContent: (items: T[]) => ReactNode;
  rightHederElement?: ReactNode;
  leftHeaderElement?: ReactNode;
}

export default function SearchContainer<T>({
  placeholder = 'Search...',
  pageSize = 10,
  fetchData,
  renderContent,
  rightHederElement,
  leftHeaderElement,
}: SearchContainerProps<T>) {
  const { getRoadmapsTranslations } = useLocalization();
  const [page, setPage] = useState(1);
  const [items, setItems] = useState<T[]>([]);
  const [hasMore, setHasMore] = useState(false);
  const [searchInput, setSearchInput] = useState('');
  const [search, setSearch] = useState<string | null>('');

  const [isLoading, setIsLoading] = useState(true);
  const [isFetching, setIsFetching] = useState(false);
  const [error, setError] = useState<unknown | null>(null);

  const loadData = async () => {
    try {
      setIsFetching(true);
      const result = await fetchData({
        pageNumber: page,
        pageSize,
        query: search,
      });
      setItems(result.items);
      setHasMore(result.items.length === pageSize);
      setError(null);
    } catch (err) {
      console.error(err);
      setError(err);
    } finally {
      setIsLoading(false);
      setIsFetching(false);
    }
  };

  useEffect(() => {
    loadData();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, search]);

  const handleSearch = () => {
    setPage(1);
    setSearch(searchInput.trim().length > 0 ? searchInput.trim() : '');
  };

  const handlePrev = () => page > 1 && setPage((p) => p - 1);
  const handleNext = () => hasMore && setPage((p) => p + 1);

  if (error) return <ErrorScreen />;
  if (isLoading && page === 1) return <SpinnerScreen />;

  return (
    <Flex
      direction="column"
      h="80vh"
      alignItems="center"
      justifyContent="flex-start"
    >
      {/* Search bar */}
      <HStack justify="space-between" mb={6} width="full" px={1} pt={2}>
        {leftHeaderElement}
        <InputGroup
          borderRadius="xl"
          bg="bg.section"
          boxShadow="0 1px 3px rgba(0,0,0,0.06)"
          width={{ base: 'full', md: '380px' }}
          endElement={
            <LuSearch
              onClick={handleSearch}
              style={{
                cursor: 'pointer',
                color: 'var(--chakra-colors-brand-500)',
              }}
            />
          }
        >
          <Input
            placeholder={placeholder}
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
            borderRadius="xl"
            borderColor="border.muted"
            _focus={{
              borderColor: 'brand.500',
              boxShadow: '0 0 0 1px var(--chakra-colors-brand-500)',
            }}
            fontSize="sm"
          />
        </InputGroup>
        {rightHederElement}
      </HStack>

      {/* Content area */}
      <Box flex="1" w="full" px={1} position="relative">
        {isFetching ? (
          /* Overlay spinner while re-fetching */
          <Flex
            w="full"
            minH="40vh"
            alignItems="center"
            justifyContent="center"
            direction="column"
            gap={3}
          >
            <Flex
              position="relative"
              alignItems="center"
              justifyContent="center"
            >
              <Spinner
                color="brand.800"
                animationDuration="0.9s"
                size="lg"
                borderWidth="3px"
              />
              <Flex
                position="absolute"
                alignItems="center"
                justifyContent="center"
              >
                <Box w="14px" h="14px" borderRadius="sm" bg="brand.200" />
              </Flex>
            </Flex>
            <Text fontSize="sm" color="text.muted">
              Loading…
            </Text>
          </Flex>
        ) : items.length > 0 ? (
          renderContent(items)
        ) : (
          <ContentNotFoundScreen />
        )}
      </Box>

      {/* Pagination */}
      <Flex alignItems="center" justifyContent="center" gap={3} py={6}>
        <Button
          onClick={handlePrev}
          disabled={page === 1 || isFetching}
          size="sm"
          variant="outline"
          borderRadius="lg"
          borderColor="border.muted"
          _hover={{ bg: 'brand.800', borderColor: 'brand.800', color: 'white' }}
        >
          <AiOutlineArrowLeft />
        </Button>

        <Flex
          px={4}
          py={1}
          bg="bg.section"
          borderRadius="lg"
          borderWidth="1px"
          borderColor="border.muted"
        >
          <Text fontSize="sm" color="text.heading" fontWeight="600">
            {getRoadmapsTranslations('page')} {page}
          </Text>
        </Flex>

        <Button
          onClick={handleNext}
          disabled={!hasMore || isFetching}
          size="sm"
          variant="outline"
          borderRadius="lg"
          borderColor="border.muted"
          _hover={{ bg: 'brand.800', borderColor: 'brand.800', color: 'white' }}
        >
          <AiOutlineArrowRight />
        </Button>
      </Flex>
    </Flex>
  );
}
