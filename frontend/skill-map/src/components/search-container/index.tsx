'use client';

import {
  Box,
  Button,
  Flex,
  Input,
  InputGroup,
  ScrollArea,
} from '@chakra-ui/react';
import { LuSearch } from 'react-icons/lu';
import { AiOutlineArrowLeft, AiOutlineArrowRight } from 'react-icons/ai';

interface SearchContainerProps {
  childeren: React.ReactNode;
  disabled: boolean;
  page: number;
  pageSize: number;
  setPage: (page: number) => number;
}

export default function SearchContainer({
  childeren,
  disabled,
  page,
  setPage,
  pageSize,
}: SearchContainerProps) {
  return (
    <Flex gap={4} direction="column">
      <Box
        w={{ base: '100%', sm: 'sm', md: 'md', lg: 'lg' }}
        alignSelf={{ base: 'stretch', md: 'flex-end' }}
      >
        <InputGroup
          flex="1"
          endElement={<LuSearch />}
          borderRadius="md"
          bg="bg.section"
          boxShadow="sm"
        >
          <Input placeholder="Search roadmaps..." />
        </InputGroup>
      </Box>

      <Box h="full" w="full" borderRadius="lg" p={4}>
        {
          <ScrollArea.Root>
            <ScrollArea.Viewport>
              <ScrollArea.Content>{childeren}</ScrollArea.Content>
            </ScrollArea.Viewport>
            <ScrollArea.Scrollbar>
              <ScrollArea.Thumb />
            </ScrollArea.Scrollbar>
            <ScrollArea.Corner />
          </ScrollArea.Root>
        }
      </Box>

      <Flex gap={4} alignSelf="center" position="fixed" bottom={10}>
        <Button
          onClick={() => setPage(Math.max(page - 1, 1))}
          disabled={page === 1 || disabled}
          variant="outline"
          size="sm"
        >
          <AiOutlineArrowLeft />
        </Button>
        <Button
          variant="outline"
          onClick={() => setPage(page + 1)}
          disabled={page < pageSize || disabled}
          size="sm"
        >
          <AiOutlineArrowRight />
        </Button>
      </Flex>
    </Flex>
  );
}
