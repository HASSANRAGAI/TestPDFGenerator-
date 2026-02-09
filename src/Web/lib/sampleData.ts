/**
 * Sample claims data for the ACIG General Claims System
 */

export interface Claim {
  id: string;
  claimNumber: string;
  policyNumber: string;
  insuredName: string;
  insuredNameAr: string;
  claimDate: string;
  status: 'Pending' | 'Approved' | 'Rejected' | 'Under Review';
  amount: number;
  claimType: string;
  claimTypeAr: string;
  assignedTo: string;
}

export const sampleClaims: Claim[] = [
  {
    id: '1',
    claimNumber: 'CLM-2024-001',
    policyNumber: 'POL-12345',
    insuredName: 'Ahmed Mohammed',
    insuredNameAr: 'أحمد محمد',
    claimDate: '2024-01-15',
    status: 'Approved',
    amount: 15000,
    claimType: 'Medical',
    claimTypeAr: 'طبي',
    assignedTo: 'Sarah Ali',
  },
  {
    id: '2',
    claimNumber: 'CLM-2024-002',
    policyNumber: 'POL-12346',
    insuredName: 'Fatima Hassan',
    insuredNameAr: 'فاطمة حسن',
    claimDate: '2024-01-16',
    status: 'Under Review',
    amount: 25000,
    claimType: 'Vehicle',
    claimTypeAr: 'مركبات',
    assignedTo: 'Mohammed Khalid',
  },
  {
    id: '3',
    claimNumber: 'CLM-2024-003',
    policyNumber: 'POL-12347',
    insuredName: 'Omar Abdullah',
    insuredNameAr: 'عمر عبدالله',
    claimDate: '2024-01-17',
    status: 'Pending',
    amount: 8500,
    claimType: 'Property',
    claimTypeAr: 'ممتلكات',
    assignedTo: 'Layla Ahmed',
  },
  {
    id: '4',
    claimNumber: 'CLM-2024-004',
    policyNumber: 'POL-12348',
    insuredName: 'Maryam Salem',
    insuredNameAr: 'مريم سالم',
    claimDate: '2024-01-18',
    status: 'Approved',
    amount: 12000,
    claimType: 'Medical',
    claimTypeAr: 'طبي',
    assignedTo: 'Sarah Ali',
  },
  {
    id: '5',
    claimNumber: 'CLM-2024-005',
    policyNumber: 'POL-12349',
    insuredName: 'Khalid Ibrahim',
    insuredNameAr: 'خالد إبراهيم',
    claimDate: '2024-01-19',
    status: 'Rejected',
    amount: 18000,
    claimType: 'Vehicle',
    claimTypeAr: 'مركبات',
    assignedTo: 'Mohammed Khalid',
  },
  {
    id: '6',
    claimNumber: 'CLM-2024-006',
    policyNumber: 'POL-12350',
    insuredName: 'Noura Rashid',
    insuredNameAr: 'نورة راشد',
    claimDate: '2024-01-20',
    status: 'Under Review',
    amount: 22000,
    claimType: 'Medical',
    claimTypeAr: 'طبي',
    assignedTo: 'Sarah Ali',
  },
  {
    id: '7',
    claimNumber: 'CLM-2024-007',
    policyNumber: 'POL-12351',
    insuredName: 'Ali Hassan',
    insuredNameAr: 'علي حسن',
    claimDate: '2024-01-21',
    status: 'Pending',
    amount: 9500,
    claimType: 'Property',
    claimTypeAr: 'ممتلكات',
    assignedTo: 'Layla Ahmed',
  },
  {
    id: '8',
    claimNumber: 'CLM-2024-008',
    policyNumber: 'POL-12352',
    insuredName: 'Aisha Mohammed',
    insuredNameAr: 'عائشة محمد',
    claimDate: '2024-01-22',
    status: 'Approved',
    amount: 16500,
    claimType: 'Vehicle',
    claimTypeAr: 'مركبات',
    assignedTo: 'Mohammed Khalid',
  },
];
