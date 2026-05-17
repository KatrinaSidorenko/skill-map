'use client';
import { Flex, Spinner, Text, VStack, Box } from '@chakra-ui/react';
import useLocalization from '@/i18n/useLocalization';

export default function SpinnerScreen() {
  const { getGeneralTranslations } = useLocalization();
  return (
    <Flex w="full" minH="80vh" alignItems="center" justifyContent="center">
      <VStack gap={5}>
        <Flex position="relative" alignItems="center" justifyContent="center">
          <Spinner
            color="brand.800"
            animationDuration="0.9s"
            size="xl"
            borderWidth="3px"
          />
          <Flex position="absolute" alignItems="center" justifyContent="center">
            <Box w="18px" h="18px" borderRadius="sm" bg="brand.200" />
          </Flex>
        </Flex>

        <VStack gap={1}>
          <Text fontWeight="700" fontSize="md" color="text.heading">
            {getGeneralTranslations('loading')}
          </Text>
          <Text fontSize="sm" color="text.muted">
            {getGeneralTranslations('fetchingContent')}
          </Text>
        </VStack>
      </VStack>
    </Flex>
  );
}
