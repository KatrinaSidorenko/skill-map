'use client';

import RoadmapGrid from '@/components/roadmap/roadmapGrid';
import { useAppSelector } from '@/store/hooks';
import { Box, Flex, Input, InputGroup, ScrollArea } from '@chakra-ui/react';
import { LuSearch } from 'react-icons/lu';
import { selectPlainRoadmaps } from '../store';

export default function ExploreRoadmapsPage() {
  const roadmaps = useAppSelector(selectPlainRoadmaps);
  return (
    <Flex gap={8} direction="column">
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
          <Input placeholder="Search contacts" />
        </InputGroup>
      </Box>
      <Box h="full" w={'full'} borderRadius="lg" p={4}>
        <ScrollArea.Root>
          <ScrollArea.Viewport>
            <ScrollArea.Content>
              <RoadmapGrid roadmaps={roadmaps} />
            </ScrollArea.Content>
          </ScrollArea.Viewport>
          <ScrollArea.Scrollbar>
            <ScrollArea.Thumb />
          </ScrollArea.Scrollbar>
          <ScrollArea.Corner />
        </ScrollArea.Root>
      </Box>
    </Flex>
  );
}
