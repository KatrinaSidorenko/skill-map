import { Flex, Spinner } from '@chakra-ui/react';

export default function SpinnerScreen() {
  return (
    <Flex w="full" h="full" alignItems="center" justifyContent="center">
      <Spinner color="blue.500" animationDuration="0.8s" size="lg" />
    </Flex>
  );
}
