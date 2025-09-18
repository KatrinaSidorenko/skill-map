import { Flex } from '@chakra-ui/react';
import { BiError } from 'react-icons/bi';

export default function ErrorScreen() {
  return (
    <Flex w="full" h="full" alignItems="center" justifyContent="center">
      <BiError size="3em" color="red" />
    </Flex>
  );
}
