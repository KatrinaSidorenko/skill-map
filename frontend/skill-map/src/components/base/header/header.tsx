'use client';
import { Flex, IconButton, Text } from '@chakra-ui/react';
import { useSidebar } from '@/components/sidebar/sidebar-context';
import { FiMenu } from 'react-icons/fi';
import React from 'react';

export default function Header() {
  const { setOpen } = useSidebar();
  return (
    <Flex
      as="header"
      direction="row"
      align="center"
      justify="space-between"
      px={4}
      py={2}
      bg="transparent"
      borderRadius="lg"
      w="full"
    >
      <Text fontSize="lg" fontWeight="bold" color="text.heading">
        Header
      </Text>
      <IconButton
        variant="outline"
        onClick={() => setOpen(true)}
        aria-label="open menu"
        display={{ base: 'flex', md: 'none' }}
      >
        <FiMenu />
      </IconButton>
      <Text fontSize="sm" color="text.muted">
        User Menu
      </Text>
    </Flex>
  );
}
