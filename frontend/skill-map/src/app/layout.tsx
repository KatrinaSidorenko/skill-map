import { Provider } from '@/components/ui/provider';
import { ReduxProvider } from '@/store/providers';
import { firaCode, inter, nunito } from '@/theme/fonts';

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html
      className={`${nunito.variable} ${inter.variable} ${firaCode.variable}`}
      suppressHydrationWarning
    >
      <body suppressHydrationWarning>
        <ReduxProvider>
          <Provider>{children}</Provider>
        </ReduxProvider>
      </body>
    </html>
  );
}
