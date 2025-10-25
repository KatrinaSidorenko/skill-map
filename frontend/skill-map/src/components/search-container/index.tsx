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
  Stack,
  Text,
} from '@chakra-ui/react';
import { LuSearch } from 'react-icons/lu';
import ErrorScreen from '@/components/base/error';
import SpinnerScreen from '@/components/base/spinner';
import { AiOutlineArrowLeft, AiOutlineArrowRight } from 'react-icons/ai';
import ContentNotFoundScreen from '../base/notfound';
import useLocalization from '@/i18n/useLocalization';

interface SearchContainerProps<T> {
  placeholder?: string;
  pageSize?: number;
  fetchData: (params: {
    pageNumber: number;
    pageSize: number;
    query: string | null;
  }) => Promise<{ items: T[]; total: number }>;
  renderContent: (items: T[]) => ReactNode;
  rightHederElement?: ReactNode;
  leftHeaderElement?: ReactNode;
}

/**
 * Generic Search + Pagination container.
 * Handles search, pagination, loading and error states.
 */
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
  const [totalPages, setTotalPages] = useState(0);
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
      setTotalPages(Math.max(1, Math.ceil(result.total / pageSize)));
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
  const handleNext = () => page < totalPages && setPage((p) => p + 1);

  if (error) return <ErrorScreen />;
  if (isLoading && page === 1) return <SpinnerScreen />;

  return (
    <Flex
      direction="column"
      minH="100%"
      alignItems="center"
      justifyContent="flex-start"
    >
      <HStack justify="space-between" mb={8} width={'full'} px={4} pt={4}>
        {leftHeaderElement}
        <InputGroup
          borderRadius="md"
          bg="bg.page"
          boxShadow="sm"
          width={{ base: 'full', md: '400px' }}
          endElement={
            <LuSearch onClick={handleSearch} style={{ cursor: 'pointer' }} />
          }
        >
          <Input
            placeholder={placeholder}
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
          />
        </InputGroup>
        {rightHederElement}
      </HStack>

      <Box flex="1" w="full" px={4}>
        {isFetching && items.length === 0 ? (
          <Flex justifyContent="center" py={8}>
            <Spinner />
          </Flex>
        ) : items.length > 0 ? (
          renderContent(items)
        ) : (
          <ContentNotFoundScreen />
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
          {getRoadmapsTranslations('page')} {page}{' '}
          {getRoadmapsTranslations('of')} {totalPages}
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
