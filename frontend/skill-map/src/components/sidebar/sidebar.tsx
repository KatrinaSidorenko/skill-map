/* eslint-disable @typescript-eslint/no-explicit-any */
'use client';

import React, { ReactNode } from 'react';
import { Box, Flex, Icon, Text, Drawer, VStack } from '@chakra-ui/react';
import {
  FiHome,
  FiCompass,
  FiStar,
  FiSettings,
  FiLogOut,
} from 'react-icons/fi';
import { MdOutlineCreateNewFolder } from 'react-icons/md';
import { IconType } from 'react-icons';
import { useSidebar } from './sidebar-context';
import { usePathname, useRouter } from 'next/navigation';
import { AppRouterInstance } from 'next/dist/shared/lib/app-router-context.shared-runtime';
import { useAuth } from '@/features/account/useAuthContext';

interface LinkItemProps {
  name: string;
  icon: IconType;
  link?: string;
}

const LinkItems: Array<LinkItemProps> = [
  { name: 'Home', icon: FiHome, link: '/home' },
  // { name: 'Trending', icon: FiTrendingUp },
  { name: 'Explore', icon: FiCompass, link: '/explore' },
  { name: 'Favourites', icon: FiStar, link: '/saved' },
  //{ name: 'Create', icon: MdOutlineCreateNewFolder, link: '/sandbox' },
  { name: 'Settings', icon: FiSettings, link: '/settings' },
];

export default function SidebarLayout({ children }: { children: ReactNode }) {
  const { open, setOpen } = useSidebar();
  const activePath = usePathname();

  return (
    <Box minH="100vh">
      <SidebarContent
        display={{ base: 'none', md: 'flex' }}
        activePath={activePath}
      />

      {/* Mobile Drawer */}
      <Drawer.Root
        open={open}
        onOpenChange={(e) => setOpen(e.open)}
        placement={'start'}
        size="xs"
      >
        <Drawer.Backdrop />
        <Drawer.Positioner>
          <Drawer.Content width={'60'}>
            <Drawer.CloseTrigger />
            <SidebarContent isDrawer activePath={activePath} />
          </Drawer.Content>
        </Drawer.Positioner>
      </Drawer.Root>

      {/* Page Content */}
      <Box ml={{ base: 0, md: 20 }} h="full" p={4}>
        {children}
      </Box>
    </Box>
  );
}

interface SidebarProps {
  isDrawer?: boolean;
  activePath: string;
  [key: string]: any;
}

const SidebarContent = ({ isDrawer, activePath, ...rest }: SidebarProps) => {
  const router = useRouter();
  const { logout } = useAuth();

  return (
    <Flex
      direction="column"
      align="center"
      bg="sidebar.bg"
      borderRight="1px"
      w={isDrawer ? '60' : '20'}
      pos="fixed"
      h="full"
      {...rest}
    >
      {/* ── Drawer logo ── */}
      {isDrawer && (
        <Flex h="16" alignItems="center" px="6" w="full" flexShrink={0}>
          <Text fontSize="2xl" fontWeight="bold" color="text.heading">
            Logo
          </Text>
        </Flex>
      )}

      {/* ── Nav items — centered vertically ── */}
      <Flex
        direction="column"
        justify="center"
        align="center"
        flex="1"
        w="full"
        px={2}
      >
        <VStack gap={4} w={isDrawer ? 'full' : 'auto'} align="center">
          {LinkItems.map((link) => (
            <NavItem
              key={link.name}
              icon={link.icon}
              link={link.link}
              isActive={activePath?.includes(link.link || '')}
              isDrawer={isDrawer}
              router={router}
            >
              {isDrawer ? link.name : null}
            </NavItem>
          ))}
        </VStack>
      </Flex>

      {/* ── Logout — pinned to bottom ── */}
      <Box
        pb={4}
        flexShrink={0}
        w={isDrawer ? 'full' : 'auto'}
        px={isDrawer ? 2 : 0}
      >
        <Flex
          align="center"
          gap={3}
          px={3}
          py={2}
          borderRadius="lg"
          cursor="pointer"
          color="sidebar.text"
          _hover={{ bg: 'red.50', color: 'red.500' }}
          transition="all 0.15s ease"
          onClick={() => logout()}
          w={isDrawer ? 'full' : 'auto'}
        >
          <Icon boxSize={5} as={FiLogOut} flexShrink={0} />
          {isDrawer && (
            <Text fontSize="sm" fontWeight="medium">
              Logout
            </Text>
          )}
        </Flex>
      </Box>
    </Flex>
  );
};

interface NavItemProps {
  icon: IconType;
  link?: string;
  children?: ReactNode;
  isActive: boolean;
  isDrawer?: boolean;
  [key: string]: any;
  router: AppRouterInstance;
}

const NavItem = ({
  icon,
  children,
  link,
  isActive,
  isDrawer,
  router,
  ...rest
}: NavItemProps) => {
  return (
    <Flex
      align="center"
      justify={isDrawer ? 'flex-start' : 'center'}
      gap={3}
      px={3}
      py={2}
      borderRadius="lg"
      cursor="pointer"
      role="group"
      bg={isActive ? 'sidebar.active' : 'transparent'}
      color={isActive ? 'text.onAccent' : 'sidebar.text'}
      _hover={{
        bg: isActive ? 'brand.300' : 'sidebar.linkHover',
        color: isActive ? 'text.onAccent' : 'text.primary',
      }}
      transition="all 0.15s ease"
      onClick={() => router.push(link || '/home')}
      w={isDrawer ? 'full' : 'auto'}
      minW="10"
      {...rest}
    >
      <Icon boxSize={5} as={icon} flexShrink={0} />
      {children && (
        <Text fontSize="sm" fontWeight={isActive ? 'semibold' : 'medium'}>
          {children}
        </Text>
      )}
    </Flex>
  );
};
