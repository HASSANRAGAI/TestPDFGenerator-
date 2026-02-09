/**
 * Breadcrumb Component
 * Shows current page location with navigation path
 */

import React from 'react';
import { theme } from '@/lib/theme';

interface BreadcrumbItem {
  label: string;
  href?: string;
}

interface BreadcrumbProps {
  items: BreadcrumbItem[];
}

export default function Breadcrumb({ items }: BreadcrumbProps) {
  return (
    <div
      style={{
        padding: `${theme.spacing.md} ${theme.spacing.xl}`,
        backgroundColor: theme.colors.background.white,
        borderBottom: `1px solid ${theme.colors.border.DEFAULT}`,
      }}
    >
      <div style={{ maxWidth: '1400px', margin: '0 auto', display: 'flex', gap: theme.spacing.sm, alignItems: 'center' }}>
        {items.map((item, index) => (
          <React.Fragment key={index}>
            {index > 0 && (
              <span style={{ color: theme.colors.text.secondary, fontSize: theme.typography.fontSize.sm }}>
                /
              </span>
            )}
            {item.href ? (
              <a
                href={item.href}
                style={{
                  color: theme.colors.primary.DEFAULT,
                  fontSize: theme.typography.fontSize.sm,
                  textDecoration: 'none',
                }}
              >
                {item.label}
              </a>
            ) : (
              <span
                style={{
                  color: theme.colors.text.primary,
                  fontSize: theme.typography.fontSize.sm,
                  fontWeight: theme.typography.fontWeight.medium,
                }}
              >
                {item.label}
              </span>
            )}
          </React.Fragment>
        ))}
      </div>
    </div>
  );
}
