import { defineRecipe } from '@chakra-ui/react';

export const buttonRecipe = defineRecipe({
  base: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    borderRadius: 'md',
    transition: 'background-color 0.2s, color 0.2s',
    fontWeight: 'semibold',
    _focusVisible: {
      outline: '2px solid',
      outlineColor: 'border.focus',
      outlineOffset: '2px',
    },
  },
  variants: {
    visual: {
      solid: {
        bg: 'button.solidBg',
        color: 'button.solidText',
        _hover: { bg: 'button.solidBg', filter: 'brightness(0.9)' },
        _active: { bg: 'button.solidBg', filter: 'brightness(0.8)' },
      },
      outline: {
        color: 'button.subtleText',
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
