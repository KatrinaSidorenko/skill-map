import { getTranslations } from 'next-intl/server';
import {
  Box,
  Flex,
  Grid,
  GridItem,
  Heading,
  Text,
  VStack,
} from '@chakra-ui/react';

export default async function AccountLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const t = await getTranslations('auth');

  return (
    <Grid
      templateColumns={{ base: '1fr', md: '1fr 1fr' }}
      w="100vw"
      h="100vh"
      bg="bg.page"
    >
      {/* Left branding panel */}
      <GridItem display={{ base: 'none', md: 'flex' }}>
        <Flex
          direction="column"
          justify="center"
          align="center"
          bg="brand.800"
          px={16}
          position="relative"
          overflow="hidden"
          w="full"
          h="full"
        >
          {/* Decorative circles */}
          <Box
            position="absolute"
            top="-80px"
            right="-80px"
            w="320px"
            h="320px"
            borderRadius="full"
            bg="brand.700"
            opacity={0.5}
          />
          <Box
            position="absolute"
            bottom="-60px"
            left="-60px"
            w="240px"
            h="240px"
            borderRadius="full"
            bg="brand.600"
            opacity={0.4}
          />
          <Box
            position="absolute"
            top="40%"
            left="-30px"
            w="120px"
            h="120px"
            borderRadius="full"
            bg="brand.200"
            opacity={0.15}
          />

          <VStack gap={6} align="flex-start" position="relative" zIndex={1}>
            {/* Logo */}
            <Flex align="center" gap={3} mb={4}>
              <Box
                w="44px"
                h="44px"
                borderRadius="xl"
                bg="brand.200"
                display="flex"
                alignItems="center"
                justifyContent="center"
              >
                <Box w="22px" h="22px" borderRadius="sm" bg="brand.800" />
              </Box>
              <Heading
                fontSize="2xl"
                fontWeight="800"
                color="brand.200"
                letterSpacing="tight"
              >
                SkillMap
              </Heading>
            </Flex>

            <Heading
              fontSize="4xl"
              fontWeight="800"
              color="white"
              lineHeight="1.15"
              letterSpacing="tight"
            >
              {t('brandTagline')}
            </Heading>

            <Text fontSize="lg" color="brand.100" lineHeight="1.6" maxW="360px">
              {t('brandDescription')}
            </Text>
          </VStack>
        </Flex>
      </GridItem>

      {/* Right form panel */}
      <GridItem>
        <Flex direction="column" justify="center" align="center" h="full" p={8}>
          {/* Mobile logo */}
          <Flex
            align="center"
            gap={2}
            mb={6}
            display={{ base: 'flex', md: 'none' }}
          >
            <Box
              w="32px"
              h="32px"
              borderRadius="lg"
              bg="brand.800"
              display="flex"
              alignItems="center"
              justifyContent="center"
            >
              <Box w="16px" h="16px" borderRadius="sm" bg="brand.200" />
            </Box>
            <Heading fontSize="xl" fontWeight="800" color="brand.800">
              SkillMap
            </Heading>
          </Flex>

          <Box
            w={{ base: '100%', md: '80%' }}
            maxW="420px"
            bg="bg.section"
            borderRadius="2xl"
            p={8}
            boxShadow="0 4px 24px rgba(0,0,0,0.07)"
          >
            {children}
          </Box>
        </Flex>
      </GridItem>
    </Grid>
  );
}
