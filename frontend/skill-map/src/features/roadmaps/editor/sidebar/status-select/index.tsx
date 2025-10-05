'use client';

import useLocalization from '@/i18n/useLocalization';
import {
  Select,
  Portal,
  createListCollection,
  Text,
  VStack,
} from '@chakra-ui/react';

export default function StatusSelect({
  value,
  onChange,
}: {
  value: string[];
  onChange: (val: string[]) => void;
}) {
  const { getEditorTranslations } = useLocalization();
  const statuses = createListCollection({
    items: [
      { label: getEditorTranslations('notStarted'), value: 'notstarted' },
      { label: getEditorTranslations('inProgress'), value: 'inprogress' },
      { label: getEditorTranslations('completed'), value: 'done' },
    ],
  });

  return (
    <VStack align="stretch" gap={2}>
      <Text fontSize="sm" fontWeight="medium" color="gray.600">
        {getEditorTranslations('status')}
      </Text>
      <Select.Root
        collection={statuses}
        size="sm"
        width="100%"
        value={value}
        onValueChange={(e) => onChange(e.value)}
      >
        <Select.HiddenSelect />
        <Select.Control>
          <Select.Trigger>
            <Select.ValueText
              placeholder={getEditorTranslations('selectStatus')}
            />
          </Select.Trigger>
          <Select.IndicatorGroup>
            <Select.Indicator />
          </Select.IndicatorGroup>
        </Select.Control>
        {/* <Portal> */}
        <Select.Positioner zIndex={2000}>
          <Select.Content>
            {statuses.items.map((status) => (
              <Select.Item item={status} key={status.value}>
                <Select.ItemText>{status.label}</Select.ItemText>
                <Select.ItemIndicator />
              </Select.Item>
            ))}
          </Select.Content>
        </Select.Positioner>
        {/* </Portal> */}
      </Select.Root>
    </VStack>
  );
}
