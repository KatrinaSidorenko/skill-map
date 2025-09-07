import { useSidebarContext } from '@/components/sidenav/sidebar-context';
import { Tooltip } from '@/components/ui/tooltip';
import { Avatar, Flex, IconButton } from '@chakra-ui/react';

export function TopSection() {
  const { sideBarVisible, toggleSidebar } = useSidebarContext();
  return (
    <Flex justify="space-between" align="center" p="2">
      {!sideBarVisible && (
        <Flex>
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
      )}
    </Flex>
  );
}
