'use client';

import { useState } from 'react';
import Breadcrumb from '@/components/Breadcrumb';
import PageHeader from '@/components/PageHeader';
import Card from '@/components/Card';
import { apiClient, FieldTreeResponse, Field } from '@/lib/api';

export default function SchemaPage() {
  const [fieldTree, setFieldTree] = useState<FieldTreeResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadFieldTree = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await apiClient.getFieldTree('invoice');
      setFieldTree(data);
    } catch (err) {
      setError('Failed to load field tree. Ensure the backend API is running.');
      console.error('Error:', err);
    } finally {
      setLoading(false);
    }
  };

  const renderField = (field: Field, level: number = 0) => {
    const indent = level * 20;
    const isCollection = field.IsCollection;
    
    return (
      <div key={field.Path} style={{ marginLeft: `${indent}px` }} className="mb-2">
        <div className={`p-3 rounded-lg border-l-4 ${
          isCollection 
            ? 'bg-green-50 border-green-500' 
            : 'bg-blue-50 border-blue-500'
        }`}>
          <div className="flex items-center justify-between">
            <div>
              <span className="font-semibold text-gray-900">{field.Name}</span>
              <span className={`ml-2 px-2 py-0.5 text-xs rounded-full ${
                isCollection 
                  ? 'bg-green-200 text-green-800' 
                  : 'bg-blue-200 text-blue-800'
              }`}>
                {field.Type}
              </span>
            </div>
            <code className="text-xs bg-gray-100 px-2 py-1 rounded text-gray-600">
              {`{{${field.Path}}}`}
            </code>
          </div>
          {field.Label && (
            <p className="text-sm text-gray-600 mt-1">{field.Label}</p>
          )}
        </div>
        {field.Fields && field.Fields.length > 0 && (
          <div className="mt-2">
            {field.Fields.map(f => renderField(f, level + 1))}
          </div>
        )}
      </div>
    );
  };

  return (
    <div className="container mx-auto px-4 py-6">
      <Breadcrumb items={[{ label: 'Schema Discovery' }]} />
      
      <PageHeader 
        title="Schema Discovery"
        description="Explore the dynamically discovered database schema and available fields"
      />

      <div className="grid grid-cols-1 gap-6">
        <Card>
          <div className="mb-4 flex items-center justify-between">
            <div>
              <h3 className="text-lg font-semibold text-gray-900">Field Tree for Invoice Context</h3>
              <p className="text-sm text-gray-600 mt-1">
                Runtime-discovered fields from EF Core model introspection
              </p>
            </div>
            <button
              onClick={loadFieldTree}
              disabled={loading}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors disabled:bg-gray-400"
            >
              {loading ? '⏳ Loading...' : '🔍 Load Schema'}
            </button>
          </div>

          {error && (
            <div className="bg-red-50 border border-red-200 rounded-lg p-4 mb-4">
              <p className="text-red-800">{error}</p>
            </div>
          )}

          {fieldTree && (
            <div className="space-y-4">
              <div className="bg-gradient-to-r from-blue-50 to-purple-50 p-4 rounded-lg border border-blue-200">
                <div className="flex items-center space-x-4">
                  <div>
                    <p className="text-sm text-gray-600">Context</p>
                    <p className="text-lg font-semibold text-gray-900">{fieldTree.Context}</p>
                  </div>
                  <div className="h-8 w-px bg-gray-300"></div>
                  <div>
                    <p className="text-sm text-gray-600">Root Entity</p>
                    <p className="text-lg font-semibold text-gray-900">{fieldTree.RootEntity}</p>
                  </div>
                  <div className="h-8 w-px bg-gray-300"></div>
                  <div>
                    <p className="text-sm text-gray-600">Total Fields</p>
                    <p className="text-lg font-semibold text-gray-900">{fieldTree.Fields.length}</p>
                  </div>
                </div>
              </div>

              <div className="bg-white rounded-lg border border-gray-200 p-4">
                <h4 className="font-semibold text-gray-900 mb-4">Available Fields</h4>
                <div className="space-y-2">
                  {fieldTree.Fields.map(field => renderField(field))}
                </div>
              </div>
            </div>
          )}

          {!fieldTree && !loading && !error && (
            <div className="text-center py-12 text-gray-500">
              <svg className="w-16 h-16 mx-auto mb-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2h-6l-2-2H5a2 2 0 00-2 2z" />
              </svg>
              <p className="text-lg">No schema loaded</p>
              <p className="text-sm mt-2">Click "Load Schema" to discover the database structure</p>
            </div>
          )}
        </Card>

        <Card title="About Runtime Schema Discovery">
          <div className="prose prose-sm max-w-none">
            <p className="text-gray-700">
              The PDF Template System uses EF Core model introspection to discover your database schema at runtime. 
              This means no compile-time DTOs are needed - the system dynamically generates field trees based on your 
              database structure.
            </p>
            <div className="mt-4 space-y-2">
              <h4 className="font-semibold text-gray-900">Key Features:</h4>
              <ul className="list-disc list-inside text-gray-700 space-y-1">
                <li>Dynamic field discovery from EF Core metadata</li>
                <li>Support for nested relationships and collections</li>
                <li>Custom joins without navigation properties</li>
                <li>Field path generation for Handlebars templates</li>
                <li>Context-based field filtering and labeling</li>
              </ul>
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
}
