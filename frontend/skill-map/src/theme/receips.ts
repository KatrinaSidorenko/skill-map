import { defineRecipe } from '@chakra-ui/react';

export const buttonRecipe = defineRecipe({
  base: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    borderRadius: 'md',
    transition: 'background-color 0.2s, color 0.2s',
  },
  variants: {
    visual: {
      solid: {
        bg: 'brand.20',
        color: 'brand.900',
        _hover: { bg: 'brand.100' },
        _active: { bg: 'brand.100' },
      },
      outline: {
        bg: 'brand.20',
        color: 'brand.900',
        borderWidth: '1px',
        borderColor: 'button.outlineBorder',
        _hover: { bg: 'button.subtleBg' },
        _active: { bg: 'button.subtleBg', filter: 'brightness(0.95)' },
      },
      ghost: {
        color: 'button.subtleText',
        _hover: { bg: 'button.subtleBg' },
        _active: { bg: 'button.subtleBg', filter: 'brightness(0.95)' },
      },
    },
    size: {
      xs: { px: 0.5, py: 0.5, fontSize: 'xs' },
      sm: { px: 3, py: 1.5, fontSize: 'sm' },
      md: { px: 4, py: 2, fontSize: 'md' },
      lg: { px: 6, py: 3, fontSize: 'lg' },
    },
  },
  defaultVariants: {
    visual: 'solid',
    size: 'md',
  },
});
