/**
 * ACIG General Claims System - Home Page
 * Main dashboard showing claims data table
 */

'use client';

import { useState } from 'react';
import TopNav from '@/components/TopNav';
import Breadcrumb from '@/components/Breadcrumb';
import DataTable from '@/components/DataTable';
import { sampleClaims } from '@/lib/sampleData';
import { theme } from '@/lib/theme';

export default function Home() {
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 5;

  const breadcrumbItems = [
    { label: 'Home', href: '/' },
    { label: 'Claims' },
  ];

  return (
    <div style={{ minHeight: '100vh', backgroundColor: theme.colors.background.DEFAULT }}>
      <TopNav />
      <Breadcrumb items={breadcrumbItems} />
      
      <main
        style={{
          maxWidth: '1400px',
          margin: '0 auto',
          padding: theme.spacing.xl,
        }}
      >
        {/* Page Header */}
        <div style={{ marginBottom: theme.spacing.xl }}>
          <h1
            style={{
              fontSize: theme.typography.fontSize['3xl'],
              fontWeight: theme.typography.fontWeight.bold,
              color: theme.colors.text.primary,
              marginBottom: theme.spacing.sm,
            }}
          >
            Claims Management
          </h1>
          <p
            style={{
              fontSize: theme.typography.fontSize.base,
              color: theme.colors.text.secondary,
            }}
          >
            View and manage all insurance claims in the system
          </p>
        </div>

        {/* Stats Cards */}
        <div
          style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))',
            gap: theme.spacing.lg,
            marginBottom: theme.spacing.xl,
          }}
        >
          <div
            style={{
              backgroundColor: theme.colors.background.white,
              padding: theme.spacing.lg,
              borderRadius: theme.borderRadius.lg,
              boxShadow: theme.shadows.DEFAULT,
              borderLeft: `4px solid ${theme.colors.status.info}`,
            }}
          >
            <div style={{ fontSize: theme.typography.fontSize.sm, color: theme.colors.text.secondary, marginBottom: theme.spacing.sm }}>
              Total Claims
            </div>
            <div style={{ fontSize: theme.typography.fontSize['2xl'], fontWeight: theme.typography.fontWeight.bold, color: theme.colors.text.primary }}>
              {sampleClaims.length}
            </div>
          </div>
          <div
            style={{
              backgroundColor: theme.colors.background.white,
              padding: theme.spacing.lg,
              borderRadius: theme.borderRadius.lg,
              boxShadow: theme.shadows.DEFAULT,
              borderLeft: `4px solid ${theme.colors.status.success}`,
            }}
          >
            <div style={{ fontSize: theme.typography.fontSize.sm, color: theme.colors.text.secondary, marginBottom: theme.spacing.sm }}>
              Approved
            </div>
            <div style={{ fontSize: theme.typography.fontSize['2xl'], fontWeight: theme.typography.fontWeight.bold, color: theme.colors.text.primary }}>
              {sampleClaims.filter((c) => c.status === 'Approved').length}
            </div>
          </div>
          <div
            style={{
              backgroundColor: theme.colors.background.white,
              padding: theme.spacing.lg,
              borderRadius: theme.borderRadius.lg,
              boxShadow: theme.shadows.DEFAULT,
              borderLeft: `4px solid ${theme.colors.status.warning}`,
            }}
          >
            <div style={{ fontSize: theme.typography.fontSize.sm, color: theme.colors.text.secondary, marginBottom: theme.spacing.sm }}>
              Pending
            </div>
            <div style={{ fontSize: theme.typography.fontSize['2xl'], fontWeight: theme.typography.fontWeight.bold, color: theme.colors.text.primary }}>
              {sampleClaims.filter((c) => c.status === 'Pending').length}
            </div>
          </div>
          <div
            style={{
              backgroundColor: theme.colors.background.white,
              padding: theme.spacing.lg,
              borderRadius: theme.borderRadius.lg,
              boxShadow: theme.shadows.DEFAULT,
              borderLeft: `4px solid ${theme.colors.status.error}`,
            }}
          >
            <div style={{ fontSize: theme.typography.fontSize.sm, color: theme.colors.text.secondary, marginBottom: theme.spacing.sm }}>
              Rejected
            </div>
            <div style={{ fontSize: theme.typography.fontSize['2xl'], fontWeight: theme.typography.fontWeight.bold, color: theme.colors.text.primary }}>
              {sampleClaims.filter((c) => c.status === 'Rejected').length}
            </div>
          </div>
        </div>

        {/* Data Table */}
        <DataTable
          data={sampleClaims}
          currentPage={currentPage}
          itemsPerPage={itemsPerPage}
          onPageChange={setCurrentPage}
        />
      </main>
    </div>
  );
}

