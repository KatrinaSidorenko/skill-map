import { Box } from '@chakra-ui/react';

export default function Container({
  children,
}: {
  children?: React.ReactNode;
}) {
  return (
    <Box
      as="main"
      flex="1"
      px={6}
      py={4}
      borderRadius="lg"
      bg="bg.section"
      w="full"
    >
      {children}
    </Box>
  );
}
