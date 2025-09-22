import { Box, Flex, Grid, GridItem } from '@chakra-ui/react';
import Image from 'next/image';

export default function AccountLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <Grid
      templateColumns={{ base: '1fr', md: '1fr 1fr' }}
      w="100vw"
      h="100vh"
      bg="bg.page"
    >
      <GridItem display={{ base: 'none', md: 'grid' }} placeItems="center">
        <Image
          src="/images/learning.png"
          alt="Roadmap"
          height={700}
          width={700}
          className="object-cover"
        />
      </GridItem>

      {/* Right side (Content) */}
      <GridItem>
        <Flex direction="column" justify="center" align="center" h="full" p={8}>
          <Box
            w={{ base: '100%', md: '70%' }}
            h={{ base: '100%', md: '80%' }}
            bg="bg.section"
            borderRadius="2xl"
            p={6}
          >
            <Flex
              direction="column"
              justify="center"
              align="center"
              gap={4}
              w="full"
              h="full"
            >
              {children}
            </Flex>
          </Box>
        </Flex>
      </GridItem>
    </Grid>
  );
}
