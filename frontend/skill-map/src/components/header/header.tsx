import { Flex, Text } from '@chakra-ui/react';

export default function Header() {
  return (
    <Flex
      as="header"
      direction="row"
      align="center"
      justify="space-between"
      px={6}
      py={4}
      bg="bg.section"
      borderRadius="lg"
    >
      <Text fontSize="lg" fontWeight="bold" color="text.heading">
        Header
      </Text>
      <Text fontSize="sm" color="text.muted">
        User Menu
      </Text>
    </Flex>
  );
}
