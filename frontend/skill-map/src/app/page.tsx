'use client';

import { Button, Drawer, Portal, createOverlay } from '@chakra-ui/react';

interface DialogProps {
  title: string;
  description?: string;
  content?: React.ReactNode;
  placement?: Drawer.RootProps['placement'];
}

const drawer = createOverlay<DialogProps>((props) => {
  const { title, description, content, ...rest } = props;
  return (
    <Drawer.Root {...rest}>
      <Portal>
        <Drawer.Backdrop />
        <Drawer.Positioner>
          <Drawer.Content>
            {title && (
              <Drawer.Header>
                <Drawer.Title>{title}</Drawer.Title>
              </Drawer.Header>
            )}
            <Drawer.Body spaceY="4">
              {description && (
                <Drawer.Description>{description}</Drawer.Description>
              )}
              {content}
            </Drawer.Body>
          </Drawer.Content>
        </Drawer.Positioner>
      </Portal>
    </Drawer.Root>
  );
});

const Demo = () => {
  return (
    <>
      <Button
        onClick={() => {
          drawer.open('a', {
            title: 'Drawer Title',
            description: 'Drawer Description',
            placement: 'start',
          });
        }}
      >
        Open Drawer
      </Button>
      <drawer.Viewport />
    </>
  );
};

export default function Home() {
  return (
    <>
      {/* <Box p={8}>
        <Heading size="4xl">Heading uses Nunito</Heading>
        <Text mt={2}>Body text uses Inter font</Text>
        <Code mt={4}>This is monospace using Fira Code</Code>
      </Box>
      <ColorPalette /> */}
      {<Demo />}
    </>
  );
}
