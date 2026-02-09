/**
 * Data Table Component
 * Table with filters, sorting, and pagination for claims data
 */

'use client';

import React, { useState } from 'react';
import { theme } from '@/lib/theme';
import { Claim } from '@/lib/sampleData';

interface DataTableProps {
  data: Claim[];
  currentPage: number;
  itemsPerPage: number;
  onPageChange: (page: number) => void;
}

export default function DataTable({ data, currentPage, itemsPerPage, onPageChange }: DataTableProps) {
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [typeFilter, setTypeFilter] = useState<string>('all');
  const [searchQuery, setSearchQuery] = useState<string>('');

  // Filter data
  const filteredData = data.filter((claim) => {
    const matchesStatus = statusFilter === 'all' || claim.status === statusFilter;
    const matchesType = typeFilter === 'all' || claim.claimType === typeFilter;
    const matchesSearch =
      searchQuery === '' ||
      claim.claimNumber.toLowerCase().includes(searchQuery.toLowerCase()) ||
      claim.insuredName.toLowerCase().includes(searchQuery.toLowerCase()) ||
      claim.policyNumber.toLowerCase().includes(searchQuery.toLowerCase());
    return matchesStatus && matchesType && matchesSearch;
  });

  // Pagination
  const totalPages = Math.ceil(filteredData.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const currentData = filteredData.slice(startIndex, endIndex);

  // Status badge colors
  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Approved':
        return theme.colors.status.success;
      case 'Rejected':
        return theme.colors.status.error;
      case 'Pending':
        return theme.colors.status.warning;
      case 'Under Review':
        return theme.colors.status.info;
      default:
        return theme.colors.secondary.DEFAULT;
    }
  };

  const inputStyle: React.CSSProperties = {
    padding: `${theme.spacing.sm} ${theme.spacing.md}`,
    border: `1px solid ${theme.colors.border.DEFAULT}`,
    borderRadius: theme.borderRadius.md,
    fontSize: theme.typography.fontSize.sm,
    outline: 'none',
    transition: 'border-color 0.2s',
  };

  return (
    <div style={{ backgroundColor: theme.colors.background.white, borderRadius: theme.borderRadius.lg, boxShadow: theme.shadows.DEFAULT }}>
      {/* Filters */}
      <div
        style={{
          padding: theme.spacing.lg,
          borderBottom: `1px solid ${theme.colors.border.DEFAULT}`,
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
          gap: theme.spacing.md,
        }}
      >
        <input
          type="text"
          placeholder="Search claims..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          style={{
            ...inputStyle,
            gridColumn: 'span 2',
          }}
          onFocus={(e) => {
            e.currentTarget.style.borderColor = theme.colors.primary.DEFAULT;
          }}
          onBlur={(e) => {
            e.currentTarget.style.borderColor = theme.colors.border.DEFAULT;
          }}
        />
        <select
          value={statusFilter}
          onChange={(e) => setStatusFilter(e.target.value)}
          style={inputStyle}
          onFocus={(e) => {
            e.currentTarget.style.borderColor = theme.colors.primary.DEFAULT;
          }}
          onBlur={(e) => {
            e.currentTarget.style.borderColor = theme.colors.border.DEFAULT;
          }}
        >
          <option value="all">All Statuses</option>
          <option value="Pending">Pending</option>
          <option value="Approved">Approved</option>
          <option value="Rejected">Rejected</option>
          <option value="Under Review">Under Review</option>
        </select>
        <select
          value={typeFilter}
          onChange={(e) => setTypeFilter(e.target.value)}
          style={inputStyle}
          onFocus={(e) => {
            e.currentTarget.style.borderColor = theme.colors.primary.DEFAULT;
          }}
          onBlur={(e) => {
            e.currentTarget.style.borderColor = theme.colors.border.DEFAULT;
          }}
        >
          <option value="all">All Types</option>
          <option value="Medical">Medical</option>
          <option value="Vehicle">Vehicle</option>
          <option value="Property">Property</option>
        </select>
      </div>

      {/* Table */}
      <div style={{ overflowX: 'auto' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse' }}>
          <thead>
            <tr style={{ backgroundColor: theme.colors.background.gray }}>
              <th style={{ padding: theme.spacing.md, textAlign: 'left', fontSize: theme.typography.fontSize.sm, fontWeight: theme.typography.fontWeight.semibold, color: theme.colors.text.primary, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                Claim Number
              </th>
              <th style={{ padding: theme.spacing.md, textAlign: 'left', fontSize: theme.typography.fontSize.sm, fontWeight: theme.typography.fontWeight.semibold, color: theme.colors.text.primary, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                Policy Number
              </th>
              <th style={{ padding: theme.spacing.md, textAlign: 'left', fontSize: theme.typography.fontSize.sm, fontWeight: theme.typography.fontWeight.semibold, color: theme.colors.text.primary, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                Insured Name
              </th>
              <th style={{ padding: theme.spacing.md, textAlign: 'left', fontSize: theme.typography.fontSize.sm, fontWeight: theme.typography.fontWeight.semibold, color: theme.colors.text.primary, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                Date
              </th>
              <th style={{ padding: theme.spacing.md, textAlign: 'left', fontSize: theme.typography.fontSize.sm, fontWeight: theme.typography.fontWeight.semibold, color: theme.colors.text.primary, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                Type
              </th>
              <th style={{ padding: theme.spacing.md, textAlign: 'right', fontSize: theme.typography.fontSize.sm, fontWeight: theme.typography.fontWeight.semibold, color: theme.colors.text.primary, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                Amount
              </th>
              <th style={{ padding: theme.spacing.md, textAlign: 'left', fontSize: theme.typography.fontSize.sm, fontWeight: theme.typography.fontWeight.semibold, color: theme.colors.text.primary, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                Status
              </th>
              <th style={{ padding: theme.spacing.md, textAlign: 'left', fontSize: theme.typography.fontSize.sm, fontWeight: theme.typography.fontWeight.semibold, color: theme.colors.text.primary, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                Assigned To
              </th>
            </tr>
          </thead>
          <tbody>
            {currentData.map((claim, index) => (
              <tr
                key={claim.id}
                style={{
                  backgroundColor: index % 2 === 0 ? theme.colors.background.white : theme.colors.background.gray,
                  transition: 'background-color 0.2s',
                }}
                onMouseEnter={(e) => {
                  e.currentTarget.style.backgroundColor = '#f3f4f6';
                }}
                onMouseLeave={(e) => {
                  e.currentTarget.style.backgroundColor = index % 2 === 0 ? theme.colors.background.white : theme.colors.background.gray;
                }}
              >
                <td style={{ padding: theme.spacing.md, fontSize: theme.typography.fontSize.sm, color: theme.colors.text.primary, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                  {claim.claimNumber}
                </td>
                <td style={{ padding: theme.spacing.md, fontSize: theme.typography.fontSize.sm, color: theme.colors.text.secondary, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                  {claim.policyNumber}
                </td>
                <td style={{ padding: theme.spacing.md, fontSize: theme.typography.fontSize.sm, color: theme.colors.text.primary, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                  {claim.insuredName}
                </td>
                <td style={{ padding: theme.spacing.md, fontSize: theme.typography.fontSize.sm, color: theme.colors.text.secondary, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                  {new Date(claim.claimDate).toLocaleDateString()}
                </td>
                <td style={{ padding: theme.spacing.md, fontSize: theme.typography.fontSize.sm, color: theme.colors.text.primary, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                  {claim.claimType}
                </td>
                <td style={{ padding: theme.spacing.md, fontSize: theme.typography.fontSize.sm, color: theme.colors.text.primary, textAlign: 'right', fontWeight: theme.typography.fontWeight.medium, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                  ${claim.amount.toLocaleString()}
                </td>
                <td style={{ padding: theme.spacing.md, fontSize: theme.typography.fontSize.sm, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                  <span
                    style={{
                      padding: `${theme.spacing.xs} ${theme.spacing.sm}`,
                      borderRadius: theme.borderRadius.md,
                      fontSize: theme.typography.fontSize.xs,
                      fontWeight: theme.typography.fontWeight.semibold,
                      backgroundColor: getStatusColor(claim.status),
                      color: theme.colors.text.white,
                      display: 'inline-block',
                    }}
                  >
                    {claim.status}
                  </span>
                </td>
                <td style={{ padding: theme.spacing.md, fontSize: theme.typography.fontSize.sm, color: theme.colors.text.secondary, borderBottom: `1px solid ${theme.colors.border.DEFAULT}` }}>
                  {claim.assignedTo}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Pagination */}
      <div
        style={{
          padding: theme.spacing.lg,
          borderTop: `1px solid ${theme.colors.border.DEFAULT}`,
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
        }}
      >
        <div style={{ fontSize: theme.typography.fontSize.sm, color: theme.colors.text.secondary }}>
          Showing {startIndex + 1} to {Math.min(endIndex, filteredData.length)} of {filteredData.length} claims
        </div>
        <div style={{ display: 'flex', gap: theme.spacing.sm }}>
          <button
            onClick={() => onPageChange(Math.max(1, currentPage - 1))}
            disabled={currentPage === 1}
            style={{
              padding: `${theme.spacing.sm} ${theme.spacing.md}`,
              border: `1px solid ${theme.colors.border.DEFAULT}`,
              borderRadius: theme.borderRadius.md,
              fontSize: theme.typography.fontSize.sm,
              backgroundColor: theme.colors.background.white,
              color: theme.colors.text.primary,
              cursor: currentPage === 1 ? 'not-allowed' : 'pointer',
              opacity: currentPage === 1 ? 0.5 : 1,
            }}
          >
            Previous
          </button>
          {[...Array(totalPages)].map((_, index) => (
            <button
              key={index}
              onClick={() => onPageChange(index + 1)}
              style={{
                padding: `${theme.spacing.sm} ${theme.spacing.md}`,
                border: `1px solid ${theme.colors.border.DEFAULT}`,
                borderRadius: theme.borderRadius.md,
                fontSize: theme.typography.fontSize.sm,
                backgroundColor: currentPage === index + 1 ? theme.colors.primary.DEFAULT : theme.colors.background.white,
                color: currentPage === index + 1 ? theme.colors.text.white : theme.colors.text.primary,
                cursor: 'pointer',
                minWidth: '40px',
              }}
            >
              {index + 1}
            </button>
          ))}
          <button
            onClick={() => onPageChange(Math.min(totalPages, currentPage + 1))}
            disabled={currentPage === totalPages}
            style={{
              padding: `${theme.spacing.sm} ${theme.spacing.md}`,
              border: `1px solid ${theme.colors.border.DEFAULT}`,
              borderRadius: theme.borderRadius.md,
              fontSize: theme.typography.fontSize.sm,
              backgroundColor: theme.colors.background.white,
              color: theme.colors.text.primary,
              cursor: currentPage === totalPages ? 'not-allowed' : 'pointer',
              opacity: currentPage === totalPages ? 0.5 : 1,
            }}
          >
            Next
          </button>
        </div>
      </div>
    </div>
  );
}
