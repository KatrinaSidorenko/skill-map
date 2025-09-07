import {
  AbsoluteCenter,
  Box,
  Circle,
  Flex,
  HStack,
  IconButton,
  Link,
  Stack,
  Text,
} from '@chakra-ui/react';
import { Tooltip } from '../ui/tooltip';

import { useSidebarContext } from './sidebar-context';

export function Sidebar() {
  const { sideBarVisible, toggleSidebar } = useSidebarContext();

  return (
    <Box
      bg="bg.muted"
      w={!sideBarVisible ? '0' : '260px'}
      overflow="hidden"
      transition=" width 0.3s"
    >
      <Stack h="full" px="3" py="2">
        <Flex justify="space-between">
          <Tooltip
            content="Close sidebar"
            positioning={{ placement: 'right' }}
            showArrow
          >
            <IconButton variant="ghost" onClick={toggleSidebar}>
              Open
            </IconButton>
          </Tooltip>
        </Flex>

        <Stack px="2" gap="0" flex="1"></Stack>
      </Stack>
    </Box>
  );
}
