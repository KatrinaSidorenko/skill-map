import { createSystem, defaultConfig, defineConfig } from '@chakra-ui/react';
import { buttonRecipe, cardRecipe, drawerRecipe } from './receips';

const config = defineConfig({
  theme: {
    tokens: {
      fonts: {
        heading: { value: 'var(--font-nunito)' },
        body: { value: 'var(--font-inter)' },
        mono: { value: 'var(--font-fira-code)' },
      },
      colors: {
        brand: {
          50: { value: '#FDFDF5' }, // off-white
          100: { value: '#EAF3B2' }, // light green
          200: { value: '#9FBAF1' }, // light blue
          300: { value: '#3D568F' }, // deep blue
          400: { value: '#232323' }, // dark gray
          500: { value: '#3D568F' },
          600: { value: '#334674' },
          700: { value: '#2A3659' },
          800: { value: '#20263E' },
          900: { value: '#161A28' },
          950: { value: '#0D0F14' },
        },
      },
    },
    recipes: {
      button: buttonRecipe,
      card: cardRecipe,
      drawer: drawerRecipe,
    },
    semanticTokens: {
      colors: {
        brand: {
          solid: { value: '{colors.brand.500}' }, // button, main accents
          contrast: { value: '{colors.brand.50}' }, // text on solid background
          fg: { value: '{colors.brand.400}' }, // default text color
          muted: { value: '{colors.brand.200}' }, // secondary text or muted elements
          subtle: { value: '{colors.brand.100}' }, // subtle backgrounds or highlights
          emphasized: { value: '{colors.brand.300}' }, // headings, emphasized items
          focusRing: { value: '{colors.brand.800}' }, // focus outlines / ring
          backgroundLight: { value: '{colors.brand.50}' }, // light page backgrounds
          backgroundMuted: { value: '{colors.brand.100}' }, // section backgrounds
          backgroundDark: { value: '{colors.brand.800}' }, // card / dark section bg
          border: { value: '{colors.brand.200}' }, // borders, dividers
          heading: { value: '{colors.brand.300}' }, // headings
          accent: { value: '{colors.brand.500}' }, // links, highlights
          danger: { value: '{colors.red.500}' }, // optional if you add red
          warning: { value: '{colors.yellow.400}' }, // optional for alerts
        },
        text: {
          default: { value: '{colors.brand.400}' }, // main text
          heading: { value: '{colors.brand.300}' }, // headings
          muted: { value: '{colors.brand.200}' }, // secondary text
          accent: { value: '{colors.brand.500}' }, // links / highlights
          subtle: { value: '{colors.brand.100}' }, // placeholder text, muted text
        },
      },
    },
  },
  globalCss: {
    html: {
      colorPalette: 'brand',
      colorScheme: 'light',
    },
    body: {
      bg: 'brand.50',
      color: 'brand.400',
      minHeight: '100vh',
    },
  },
});

export const system = createSystem(defaultConfig, config);
