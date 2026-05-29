import { Provider } from '@/components/ui/provider';
import { Toaster } from '@/components/ui/toaster';
import { AuthProvider } from '@/features/account/useAuthContext';
import { routing } from '@/i18n/routing';
import { ReduxProvider } from '@/store/providers';
import { firaCode, inter, nunito } from '@/theme/fonts';
import { hasLocale, NextIntlClientProvider } from 'next-intl';
import { notFound } from 'next/navigation';
import { PublicEnvScript } from 'next-runtime-env';


export default async function RootLayout({
  children,
  params,
}: {
  children: React.ReactNode;
  params: Promise<{ locale: string }>;
}) {
  const { locale } = await params;
  if (!hasLocale(routing.locales, locale)) {
    notFound();
  }

  return (
    <html
      className={`${nunito.variable} ${inter.variable} ${firaCode.variable}`}
      suppressHydrationWarning
      lang={locale}
    >
      <PublicEnvScript />
      <body suppressHydrationWarning>
        <NextIntlClientProvider>
          <ReduxProvider>
            <AuthProvider>
              <Provider>
                {children}
                <Toaster />
              </Provider>
            </AuthProvider>
          </ReduxProvider>
        </NextIntlClientProvider>
      </body>
    </html>
  );
}
