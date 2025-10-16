import type { Metadata } from 'next';
import type { ReactNode } from 'react';
import Link from 'next/link';
import { Montserrat } from 'next/font/google';
import './globals.css';

const montserrat = Montserrat({
  subsets: ['latin'],
  display: 'swap',
  variable: '--font-montserrat'
});

export const metadata: Metadata = {
  title: 'Phase 0 Scaffold',
  description: 'Next.js 14 + TypeScript monorepo scaffold with shared tooling.'
};

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="en" className={montserrat.variable}>
      <body>
        <div className="layout-shell">
          <header className="site-header">
            <div className="brand">
              <span className="brand__title">Phase 0 Scaffold</span>
              <span className="brand__subtitle">Next.js 14 • Turborepo • TypeScript</span>
            </div>
            <nav className="site-nav">
              <Link href="#stack">Stack</Link>
              <Link href="#services">Services</Link>
              <Link href="https://nextjs.org/docs" target="_blank" rel="noreferrer">
                Next.js Docs
              </Link>
            </nav>
          </header>
          <main>{children}</main>
          <footer className="site-footer">
            <span>© {new Date().getFullYear()} Phase. Built with care in Next.js 14.</span>
          </footer>
        </div>
      </body>
    </html>
  );
}
