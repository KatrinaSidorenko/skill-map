import { Box, Flex } from '@chakra-ui/react';
import Image from 'next/image';

export default function AccountLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <Flex direction="row" w="100wh" h="100vh" bg="bg.page">
      <Box>
        <Image
          src="/images/learning.png"
          alt="Roadmap"
          height={700}
          width={700}
          className="object-cover"
        />
      </Box>
      <Flex direction="column" justify="center" align="center" flex={1} p={8}>
        <Box w="70%" h="80%" bg="bg.section" borderRadius="2xl" p={6}>
          <Flex
            direction="column"
            justifyContent="center"
            alignItems="center"
            gap={4}
            w="full"
            h="full"
          >
            {children}
          </Flex>
        </Box>
      </Flex>
    </Flex>
  );
}
