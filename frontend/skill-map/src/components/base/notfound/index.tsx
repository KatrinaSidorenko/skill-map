'use client';
import useLocalization from '@/i18n/useLocalization';
import { Flex, Text, VStack } from '@chakra-ui/react';
import { LuSearchX } from 'react-icons/lu';

export default function ContentNotFoundScreen() {
  const { getGeneralTranslations } = useLocalization();
  return (
    <Flex w="full" minH="40vh" alignItems="center" justifyContent="center">
      <VStack gap={4}>
        <Flex
          w="64px"
          h="64px"
          borderRadius="2xl"
          bg="brand.100"
          alignItems="center"
          justifyContent="center"
        >
          <LuSearchX size="28px" color="var(--chakra-colors-brand-800)" />
        </Flex>
        <VStack gap={1}>
          <Text fontWeight="700" fontSize="md" color="text.heading">
            {getGeneralTranslations('notFound')}
          </Text>
          <Text fontSize="sm" color="text.muted">
            No results match your search criteria.
          </Text>
        </VStack>
      </VStack>
    </Flex>
  );
}
