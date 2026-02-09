/**
 * Top Navigation Bar Component
 * ACIG branded navigation with deep blue background
 */

import React from 'react';
import { theme } from '@/lib/theme';

export default function TopNav() {
  return (
    <nav
      style={{
        backgroundColor: theme.colors.primary.DEFAULT,
        color: theme.colors.text.white,
        padding: `${theme.spacing.md} ${theme.spacing.xl}`,
        boxShadow: theme.shadows.md,
      }}
    >
      <div
        style={{
          maxWidth: '1400px',
          margin: '0 auto',
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
        }}
      >
        {/* Logo and Brand */}
        <div style={{ display: 'flex', alignItems: 'center', gap: theme.spacing.md }}>
          <div
            style={{
              fontSize: theme.typography.fontSize['2xl'],
              fontWeight: theme.typography.fontWeight.bold,
            }}
          >
            ACIG
          </div>
          <div
            style={{
              fontSize: theme.typography.fontSize.base,
              fontWeight: theme.typography.fontWeight.normal,
              opacity: 0.9,
            }}
          >
            General Claims System
          </div>
        </div>

        {/* Navigation Items */}
        <div style={{ display: 'flex', gap: theme.spacing.lg, alignItems: 'center' }}>
          <button
            style={{
              background: 'transparent',
              border: 'none',
              color: theme.colors.text.white,
              fontSize: theme.typography.fontSize.sm,
              cursor: 'pointer',
              padding: `${theme.spacing.sm} ${theme.spacing.md}`,
              borderRadius: theme.borderRadius.md,
              transition: 'background-color 0.2s',
            }}
            onMouseEnter={(e) => {
              e.currentTarget.style.backgroundColor = 'rgba(255, 255, 255, 0.1)';
            }}
            onMouseLeave={(e) => {
              e.currentTarget.style.backgroundColor = 'transparent';
            }}
          >
            Dashboard
          </button>
          <button
            style={{
              background: 'transparent',
              border: 'none',
              color: theme.colors.text.white,
              fontSize: theme.typography.fontSize.sm,
              cursor: 'pointer',
              padding: `${theme.spacing.sm} ${theme.spacing.md}`,
              borderRadius: theme.borderRadius.md,
              transition: 'background-color 0.2s',
            }}
            onMouseEnter={(e) => {
              e.currentTarget.style.backgroundColor = 'rgba(255, 255, 255, 0.1)';
            }}
            onMouseLeave={(e) => {
              e.currentTarget.style.backgroundColor = 'transparent';
            }}
          >
            Claims
          </button>
          <button
            style={{
              background: 'transparent',
              border: 'none',
              color: theme.colors.text.white,
              fontSize: theme.typography.fontSize.sm,
              cursor: 'pointer',
              padding: `${theme.spacing.sm} ${theme.spacing.md}`,
              borderRadius: theme.borderRadius.md,
              transition: 'background-color 0.2s',
            }}
            onMouseEnter={(e) => {
              e.currentTarget.style.backgroundColor = 'rgba(255, 255, 255, 0.1)';
            }}
            onMouseLeave={(e) => {
              e.currentTarget.style.backgroundColor = 'transparent';
            }}
          >
            Reports
          </button>
          <div
            style={{
              width: '32px',
              height: '32px',
              borderRadius: '50%',
              backgroundColor: theme.colors.primary.light,
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              fontSize: theme.typography.fontSize.sm,
              fontWeight: theme.typography.fontWeight.semibold,
              cursor: 'pointer',
            }}
          >
            AD
          </div>
        </div>
      </div>
    </nav>
  );
}
