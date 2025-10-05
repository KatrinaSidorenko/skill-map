'use client';

import {
  Select,
  Portal,
  createListCollection,
  Text,
  VStack,
} from '@chakra-ui/react';

const statuses = createListCollection({
  items: [
    { label: 'Not Started', value: 'notstarted' },
    { label: 'In Progress', value: 'inprogress' },
    { label: 'Done', value: 'done' },
  ],
});

export default function StatusSelect({
  value,
  onChange,
}: {
  value: string[];
  onChange: (val: string[]) => void;
}) {
  console.log({ value });

  return (
    <VStack align="stretch" gap={2}>
      <Text fontSize="sm" fontWeight="medium" color="gray.600">
        Status
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
            <Select.ValueText placeholder="Select status" />
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
