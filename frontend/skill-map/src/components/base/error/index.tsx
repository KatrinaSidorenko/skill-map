'use client';
import { Flex, Text, VStack, Box, Button, HStack } from '@chakra-ui/react';
import { BiError } from 'react-icons/bi';
import { FiRefreshCw, FiHome } from 'react-icons/fi';
import { useRouter } from 'next/navigation';
import useLocalization from '@/i18n/useLocalization';

interface ErrorScreenProps {
  title?: string;
  description?: string;
  onRetry?: () => void;
  showHomeButton?: boolean;
}

export default function ErrorScreen({
  title,
  description,
  onRetry,
  showHomeButton = true,
}: ErrorScreenProps) {
  const { getGeneralTranslations } = useLocalization();
  const router = useRouter();

  return (
    <Flex w="full" minH="60vh" alignItems="center" justifyContent="center" px={4}>
      <VStack gap={6} maxW="360px" textAlign="center">
        {/* Icon container with decorative ring */}
        <Box position="relative">
          <Box
            w="80px"
            h="80px"
            borderRadius="full"
            bg="red.50"
            border="2px solid"
            borderColor="red.100"
            display="flex"
            alignItems="center"
            justifyContent="center"
          >
            <Box
              w="56px"
              h="56px"
              borderRadius="full"
              bg="red.100"
              display="flex"
              alignItems="center"
              justifyContent="center"
            >
              <BiError size="28px" color="var(--chakra-colors-red-500)" />
            </Box>
          </Box>
          {/* Small brand accent dot */}
          <Box
            position="absolute"
            bottom="2px"
            right="2px"
            w="18px"
            h="18px"
            borderRadius="full"
            bg="brand.800"
            border="2px solid white"
            display="flex"
            alignItems="center"
            justifyContent="center"
          >
            <Box w="6px" h="6px" borderRadius="sm" bg="brand.200" />
          </Box>
        </Box>

        {/* Text */}
        <VStack gap={2}>
          <Text fontWeight="800" fontSize="lg" color="text.heading" lineHeight="1.2">
            {title ?? getGeneralTranslations('somethingWentWrong')}
          </Text>
          <Text fontSize="sm" color="text.muted" lineHeight="1.6">
            {description ?? getGeneralTranslations('unableToLoad')}
          </Text>
        </VStack>

        {/* Actions */}
        <HStack gap={3} justify="center">
          {onRetry && (
            <Button
              size="sm"
              colorPalette="red"
              variant="outline"
              onClick={onRetry}
              borderRadius="lg"
            >
              <FiRefreshCw />
              {getGeneralTranslations('tryAgain')}
            </Button>
          )}
          {showHomeButton && (
            <Button
              size="sm"
              bg="brand.800"
              color="brand.200"
              _hover={{ bg: 'brand.700' }}
              borderRadius="lg"
              onClick={() => router.push('/home')}
            >
              <FiHome />
              {getGeneralTranslations('goHome')}
            </Button>
          )}
        </HStack>
      </VStack>
    </Flex>
  );
}
