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

const createNodeDialog = createOverlay<{
  onCreate: (data: {
    label: string;
    description: string;
    status: string[];
  }) => void;
}>((props) => {
  const { onCreate, ...rest } = props;
  const [label, setLabel] = useState('');
  const [description, setDescription] = useState('');
  const [status, setStatus] = useState<string[]>(['notstarted']);

  return (
    <Dialog.Root {...rest}>
      <Portal>
        <Dialog.Backdrop />
        <Dialog.Positioner>
          <Dialog.Content borderRadius="2xl" p={4} maxW="lg">
            <Dialog.Header>
              <Dialog.Title>Create New Node</Dialog.Title>
            </Dialog.Header>
            <Dialog.Body spaceY="4">
              <VStack align="stretch" gap={4}>
                <Box>
                  <Text fontSize="sm" mb={1}>
                    Label
                  </Text>
                  <Input
                    value={label}
                    onChange={(e) => setLabel(e.target.value)}
                    placeholder="Enter node title"
                  />
                </Box>

                <Box>
                  <Text fontSize="sm" mb={1}>
                    Description
                  </Text>
                  <Textarea
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    placeholder="Describe this node..."
                  />
                </Box>

                <StatusSelect value={status} onChange={setStatus} />
              </VStack>
            </Dialog.Body>

            <Dialog.Footer>
              <HStack justify="flex-end" w="full">
                <Button
                  variant="ghost"
                  onClick={() => rest.onOpenChange?.({ open: false })}
                >
                  Cancel
                </Button>
                <Button
                  colorScheme="teal"
                  onClick={() => {
                    onCreate({ label, description, status });
                    rest.onOpenChange?.({ open: false });
                  }}
                >
                  Create
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