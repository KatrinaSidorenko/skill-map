import { Flex, Image, ImageProps, Text } from '@chakra-ui/react';
import { normalizeImageUrl } from '@/store/helpers';

interface ImageWrapperProps extends Omit<ImageProps, 'src' | 'alt'> {
  imageUrl: string | null | undefined;
  title: string;
}

const PALETTE = [
  'teal.400', 'blue.400', 'purple.400',
  'pink.400',  'orange.400', 'cyan.400',
  'green.400', 'red.400',
];

function pickColor(title: string): string {
  let hash = 0;
  for (let i = 0; i < title.length; i++) {
    hash = title.charCodeAt(i) + ((hash << 5) - hash);
  }
  return PALETTE[Math.abs(hash) % PALETTE.length];
}

export default function ImageWrapper({
  imageUrl,
  title,
  ...rest
}: ImageWrapperProps) {
  if (!imageUrl) {
    return (
      <Flex
        bg={pickColor(title)}
        align="center"
        justify="center"
        flexShrink={0}
        {...(rest as React.ComponentProps<typeof Flex>)}
      >
        <Text fontWeight="bold" fontSize="3xl" color="white" userSelect="none">
          {title.charAt(0).toUpperCase()}
        </Text>
      </Flex>
    );
  }

  return (
    <Image
      src={normalizeImageUrl(imageUrl)}
      alt={title}
      {...rest}
    />
  );
}