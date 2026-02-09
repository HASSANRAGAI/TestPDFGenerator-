'use client';

import { useState } from 'react';
import Breadcrumb from '@/components/Breadcrumb';
import PageHeader from '@/components/PageHeader';
import Card from '@/components/Card';
import { apiClient } from '@/lib/api';

const sampleInvoices = [
  { id: '11111111-1111-1111-1111-111111111111', name: 'Invoice 001 - شركة التقنية المتقدمة' },
  { id: '22222222-2222-2222-2222-222222222222', name: 'Invoice 002 - مؤسسة البرمجيات الحديثة' },
];

export default function GeneratePage() {
  const [context] = useState('invoice');
  const [entityId, setEntityId] = useState(sampleInvoices[0].id);
  const [loading, setLoading] = useState(false);
  const [previewHtml, setPreviewHtml] = useState<string>('');
  const [message, setMessage] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

  const handleGeneratePdf = async () => {
    try {
      setLoading(true);
      setMessage(null);
      
      const blob = await apiClient.generatePdf(context, entityId);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `${context}_${entityId}.pdf`;
      a.click();
      
      setMessage({ type: 'success', text: 'PDF generated and downloaded successfully!' });
    } catch (err) {
      setMessage({ type: 'error', text: 'Failed to generate PDF. Ensure the backend API is running.' });
      console.error('Error:', err);
    } finally {
      setLoading(false);
    }
  };

  const handlePreview = async () => {
    try {
      setLoading(true);
      setMessage(null);
      
      const html = await apiClient.previewHtml(context, entityId);
      setPreviewHtml(html);
      
      setMessage({ type: 'success', text: 'Preview loaded successfully!' });
    } catch (err) {
      setMessage({ type: 'error', text: 'Failed to load preview. Ensure the backend API is running.' });
      console.error('Error:', err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mx-auto px-4 py-6">
      <Breadcrumb items={[{ label: 'Generate PDF' }]} />
      
      <PageHeader 
        title="Generate PDF"
        description="Select an entity and generate PDF documents from templates"
      />

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-1">
          <Card title="PDF Configuration">
            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Context
                </label>
                <select
                  value={context}
                  disabled
                  className="w-full px-4 py-2 border border-gray-300 rounded-md bg-gray-50"
                >
                  <option value="invoice">Invoice</option>
                </select>
              </div>
              
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Select Entity
                </label>
                <select
                  value={entityId}
                  onChange={(e) => setEntityId(e.target.value)}
                  className="w-full px-4 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                >
                  {sampleInvoices.map((invoice) => (
                    <option key={invoice.id} value={invoice.id}>
                      {invoice.name}
                    </option>
                  ))}
                </select>
              </div>
              
              {message && (
                <div className={`p-4 rounded-md ${
                  message.type === 'success' 
                    ? 'bg-green-50 border border-green-200 text-green-800' 
                    : 'bg-red-50 border border-red-200 text-red-800'
                }`}>
                  {message.text}
                </div>
              )}
              
              <div className="space-y-3 pt-4">
                <button
                  onClick={handleGeneratePdf}
                  disabled={loading}
                  className="w-full px-4 py-3 bg-green-600 text-white rounded-md hover:bg-green-700 transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed font-medium"
                >
                  {loading ? '⏳ Processing...' : '📥 Generate & Download PDF'}
                </button>
                
                <button
                  onClick={handlePreview}
                  disabled={loading}
                  className="w-full px-4 py-3 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors disabled:bg-gray-400 disabled:cursor-not-allowed font-medium"
                >
                  {loading ? '⏳ Loading...' : '👁️ Preview HTML'}
                </button>
              </div>
            </div>
          </Card>
        </div>

        <div className="lg:col-span-2">
          <Card title="HTML Preview">
            {previewHtml ? (
              <div className="border border-gray-300 rounded-md overflow-hidden" style={{ height: '700px' }}>
                <iframe
                  srcDoc={previewHtml}
                  className="w-full h-full"
                  title="PDF Preview"
                />
              </div>
            ) : (
              <div className="text-center py-24 text-gray-500">
                <p className="text-lg">No preview loaded</p>
                <p className="text-sm mt-2">Click "Preview HTML" to see the rendered template</p>
              </div>
            )}
          </Card>
        </div>
      </div>
    </div>
  );
}
