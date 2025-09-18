'use client';
import { Flex, IconButton, Text, Avatar } from '@chakra-ui/react';
import { useSidebar } from '@/components/sidebar/sidebar-context';
import { FiMenu } from 'react-icons/fi';
import React from 'react';
import { useAppSelector } from '@/store/hooks';
import { selectUser } from '@/features/account/store';

export default function Header() {
  const { setOpen } = useSidebar();
  const user = useAppSelector(selectUser);

  return (
    <Flex
      as="header"
      direction="row"
      align="center"
      justify="space-between"
      px={4}
      py={1}
      bg="transparent"
      borderRadius="lg"
      w="full"
    >
      <IconButton
        variant="outline"
        onClick={() => setOpen(true)}
        aria-label="open menu"
        display={{ base: 'flex', md: 'none' }}
      >
        <FiMenu />
      </IconButton>
      <Text fontSize="lg" fontWeight="bold" color="text.heading">
        {user ? `Welcome, ${user.name}` : 'Welcome'}
      </Text>
      <Avatar.Root>
        <Avatar.Fallback name={user?.name ?? ''} />
        <Avatar.Image
          src={user?.avatarUrl ?? 'https://avatar.iran.liara.run/public'}
        />
      </Avatar.Root>
    </Flex>
  );
}
