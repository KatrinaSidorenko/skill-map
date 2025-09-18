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
  children: React.ReactNode;
  disabled: boolean;
  page: number;
  pageSize: number;
  setPage: (page: number) => number;
  total: number;
}

// todo: fix pagination buttons
// todo: implement search functionality
export default function SearchContainer({
  children,
  disabled,
  page,
  setPage,
  pageSize,
  total,
}: SearchContainerProps) {
  const totalPages = Math.ceil(total / pageSize);

  // todo: column on small screens
  return (
    <Flex gap={4} direction="row" h="100%">
      <Box w="sm" p={4}>
        <InputGroup
          endElement={<LuSearch />}
          borderRadius="md"
          bg="bg.section"
          boxShadow="sm"
        >
          <Input placeholder="Search roadmaps..." />
        </InputGroup>
      </Box>

      <Flex
        borderRadius="lg"
        p={4}
        flex="1"
        direction="column"
        justifyContent="space-between"
        alignContent="center"
      >
        <ScrollArea.Root style={{ height: '100%', width: '100%' }}>
          <ScrollArea.Viewport>
            <ScrollArea.Content>{children}</ScrollArea.Content>
          </ScrollArea.Viewport>
          <ScrollArea.Scrollbar orientation="vertical">
            <ScrollArea.Thumb />
          </ScrollArea.Scrollbar>
          <ScrollArea.Corner />
        </ScrollArea.Root>

        <Flex gap={4} alignSelf="center" justify="center">
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
            disabled={page >= totalPages || disabled}
            size="sm"
          >
            <AiOutlineArrowRight />
          </Button>
        </Flex>
      </Flex>
    </Flex>
  );
}
