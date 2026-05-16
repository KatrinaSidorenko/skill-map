import { Flex, Text, VStack } from '@chakra-ui/react';
import { BiError } from 'react-icons/bi';

export default function ErrorScreen() {
  return (
    <Flex w="full" minH="60vh" alignItems="center" justifyContent="center">
      <VStack gap={4}>
        <Flex
          w="64px"
          h="64px"
          borderRadius="2xl"
          bg="red.50"
          alignItems="center"
          justifyContent="center"
        >
          <BiError size="32px" color="var(--chakra-colors-red-500)" />
        </Flex>
        <VStack gap={1}>
          <Text fontWeight="700" fontSize="md" color="text.heading">
            Something went wrong
          </Text>
          <Text fontSize="sm" color="text.muted">
            Unable to load content. Please try again.
          </Text>
        </VStack>
      </VStack>
    </Flex>
  );
}
