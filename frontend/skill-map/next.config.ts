import type { NextConfig } from 'next';
import createNextIntlPlugin from 'next-intl/plugin';

const nextConfig: NextConfig = {
  output: 'standalone',
  experimental: {
    optimizePackageImports: ['@chakra-ui/react'],
  },
};

const withNextIntl = createNextIntlPlugin();
export default withNextIntl(nextConfig);
