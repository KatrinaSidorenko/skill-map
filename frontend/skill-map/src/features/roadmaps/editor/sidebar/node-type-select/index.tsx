'use client';

import {
  Select,
  createListCollection,
  Text,
  VStack,
} from '@chakra-ui/react';
import useLocalization from '@/i18n/useLocalization';

export default function NodeTypeSelect({
  value,
  onChange,
}: {
  value: LearningItemType[];
  onChange: (val: LearningItemType[]) => void;
}) {
  const { getEditorTranslations } = useLocalization();

  const nodeTypes = createListCollection({
    items: [
      { label: getEditorTranslations('typeTopic'), value: 'topic' },
      { label: getEditorTranslations('typeSubtopic'), value: 'subtopic' },
    ],
  });

  return (
    <VStack align="stretch" gap={2}>
      <Text fontSize="sm" fontWeight="medium" color="gray.600">
        {getEditorTranslations('nodeType')}
      </Text>
      <Select.Root
        collection={nodeTypes}
        size="sm"
        width="100%"
        value={value}
        onValueChange={(e) => onChange(e.value as LearningItemType[])}
        defaultValue={value}
      >
        <Select.HiddenSelect />
        <Select.Control>
          <Select.Trigger>
            <Select.ValueText placeholder={getEditorTranslations('selectType')} />
          </Select.Trigger>
          <Select.IndicatorGroup>
            <Select.Indicator />
          </Select.IndicatorGroup>
        </Select.Control>
        <Select.Positioner zIndex={2000}>
          <Select.Content>
            {nodeTypes.items.map((item) => (
              <Select.Item item={item} key={item.value}>
                <Select.ItemText>{item.label}</Select.ItemText>
                <Select.ItemIndicator />
              </Select.Item>
            ))}
          </Select.Content>
        </Select.Positioner>
      </Select.Root>
    </VStack>
  );
}
