import {
  Box,
  HStack,
  Button,
  Input,
  Textarea,
  VStack,
  Text,
  Portal,
  Dialog,
  createOverlay,
} from '@chakra-ui/react';
import { useState } from 'react';
import StatusSelect from '../../sidebar/status-select';
import NodeTypeSelect from '../../sidebar/node-type-select';
import useLocalization from '@/i18n/useLocalization';
import { useAppSelector } from '@/store/hooks';
import { selectEditorConfig } from '../../store';

const createNodeDialog = createOverlay<{
  onCreate: (data: {
    label: string;
    description: string;
    status: string[];
    nodeType: LearningItemType;
  }) => void;
}>((props) => {
  const { onCreate, ...rest } = props;
  const editorConfig = useAppSelector(selectEditorConfig);
  const [label, setLabel] = useState('');
  const [description, setDescription] = useState('');
  const [status, setStatus] = useState<string[]>(['notstarted']);
  const [nodeType, setNodeType] = useState<LearningItemType>('topic');
  const { getEditorTranslations } = useLocalization();

  return (
    <Dialog.Root {...rest}>
      <Portal>
        <Dialog.Backdrop />
        <Dialog.Positioner>
          <Dialog.Content borderRadius="2xl" p={4} maxW="lg">
            <Dialog.Header>
              <Dialog.Title>
                {getEditorTranslations('createNewNode')}
              </Dialog.Title>
            </Dialog.Header>
            <Dialog.Body spaceY="4">
              <VStack align="stretch" gap={4}>
                <Box>
                  <Text fontSize="sm" mb={1}>
                    {getEditorTranslations('label')}
                  </Text>
                  <Input
                    value={label}
                    onChange={(e) => setLabel(e.target.value)}
                    placeholder={getEditorTranslations('enterNodeLabel')}
                  />
                </Box>

                <Box>
                  <Text fontSize="sm" mb={1}>
                    {getEditorTranslations('description')}
                  </Text>
                  <Textarea
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    placeholder={getEditorTranslations('enterNodeDescription')}
                  />
                </Box>

                <NodeTypeSelect
                  value={[nodeType]}
                  onChange={(val) => setNodeType(val[0])}
                />

                {editorConfig.useStatus && (
                  <StatusSelect value={status} onChange={setStatus} />
                )}
              </VStack>
            </Dialog.Body>

            <Dialog.Footer>
              <HStack justify="flex-end" w="full">
                <Button
                  variant="ghost"
                  onClick={() => rest.onOpenChange?.({ open: false })}
                >
                  {getEditorTranslations('cancel')}
                </Button>
                <Button
                  colorScheme="teal"
                  onClick={() => {
                    onCreate({ label, description, status, nodeType });
                    rest.onOpenChange?.({ open: false });
                  }}
                >
                  {getEditorTranslations('create')}
                </Button>
              </HStack>
            </Dialog.Footer>
          </Dialog.Content>
        </Dialog.Positioner>
      </Portal>
    </Dialog.Root>
  );
});

export default createNodeDialog;
