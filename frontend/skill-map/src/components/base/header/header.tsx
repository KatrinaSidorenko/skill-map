'use client';

import {
  Flex,
  IconButton,
  Text,
  Avatar,
  Box,
  HStack,
  Select,
  createListCollection,
  Portal,
} from '@chakra-ui/react';
import { FiMenu } from 'react-icons/fi';
import React, { useTransition } from 'react';
import { useSidebar } from '@/components/sidebar/sidebar-context';
import { useAppSelector } from '@/store/hooks';
import { selectUser } from '@/features/account/store';
import { useGetProfileQuery } from '@/features/account/api';
import useLocalization from '@/i18n/useLocalization';
import { useLocale } from 'next-intl';
import { usePathname, useRouter } from 'next/navigation';
import { routing } from '@/i18n/routing';

export default function Header() {
  const { setOpen } = useSidebar();
  // useGetProfileQuery keeps avatar in sync after profile updates
  const { data: profile } = useGetProfileQuery();
  const user = useAppSelector(selectUser);
  const displayUser = profile ?? user;
  const { getHeaderTranslations } = useLocalization();
  const locale = useLocale();
  const router = useRouter();
  const pathname = usePathname();
  const [isPending, startTransition] = useTransition();

  const localeLabels: Record<string, string> = { en: 'EN', ua: 'UA' };

  const localeOptions = createListCollection({
    items: routing.locales.map((loc) => ({
      label: localeLabels[loc] ?? loc.toUpperCase(),
      value: loc,
    })),
  });

  const handleLocaleChange = (newLocale: string) => {
    if (newLocale === locale) return;
    const segments = pathname.split('/');
    segments[1] = newLocale;
    startTransition(() => router.push(segments.join('/')));
  };

  return (
    <Box>
      <Flex
        as="header"
        direction="row"
        align="center"
        justify="space-between"
        px={5}
        py={3}
        bg="bg.section"
        borderRadius="xl"
        w="full"
        boxShadow="0 1px 3px rgba(0,0,0,0.06)"
      >
        {/* Mobile menu button */}
        <IconButton
          variant="ghost"
          size="sm"
          onClick={() => setOpen(true)}
          aria-label="open menu"
          display={{ base: 'flex', md: 'none' }}
          color="text.heading"
        >
          <FiMenu />
        </IconButton>

        {/* User info */}
        <HStack gap={3}>
          <Avatar.Root size="sm" shape="full">
            <Avatar.Fallback
              name={displayUser?.username ?? ''}
              bg="brand.800"
              color="brand.200"
              fontSize="xs"
              fontWeight="bold"
            />
            <Avatar.Image
              src={displayUser?.imageUrl || 'https://avatar.iran.liara.run/public'}
            />
          </Avatar.Root>
          <Box>
            <Text fontSize="sm" fontWeight="700" color="text.heading" lineHeight="1.2">
              {displayUser?.username ?? getHeaderTranslations('welcome')}
            </Text>
            {displayUser?.email && (
              <Text fontSize="xs" color="text.muted" lineHeight="1.2">
                {displayUser.email}
              </Text>
            )}
          </Box>
        </HStack>

        {/* Right — language switcher */}
        <HStack gap={3}>
          <Select.Root
            size="xs"
            variant="outline"
            collection={localeOptions}
            value={[locale]}
            onValueChange={(details) => handleLocaleChange(details.value[0])}
            disabled={isPending}
          >
            <Select.HiddenSelect />
            <Select.Control>
              <Select.Trigger
                minW="70px"
                borderRadius="lg"
                borderColor="border.muted"
              >
                <Select.ValueText />
              </Select.Trigger>
              <Select.IndicatorGroup>
                <Select.Indicator />
              </Select.IndicatorGroup>
            </Select.Control>
            <Portal>
              <Select.Positioner>
                <Select.Content>
                  {localeOptions.items.map((item) => (
                    <Select.Item key={item.value} item={item}>
                      {item.label}
                      <Select.ItemIndicator />
                    </Select.Item>
                  ))}
                </Select.Content>
              </Select.Positioner>
            </Portal>
          </Select.Root>
        </HStack>
      </Flex>
    </Box>
  );
}
