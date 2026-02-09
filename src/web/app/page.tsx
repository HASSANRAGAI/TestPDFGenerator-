'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import Breadcrumb from '@/components/Breadcrumb';
import PageHeader from '@/components/PageHeader';
import Card from '@/components/Card';
import { apiClient, Template } from '@/lib/api';

export default function Home() {
  const [templates, setTemplates] = useState<Template[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadTemplates();
  }, []);

  const loadTemplates = async () => {
    try {
      setLoading(true);
      const data = await apiClient.getTemplates();
      setTemplates(data);
      setError(null);
    } catch (err) {
      setError('Failed to load templates. Please ensure the backend API is running.');
      console.error('Error loading templates:', err);
    } finally {
      setLoading(false);
    }
  };

  const stats = [
    { label: 'Total Templates', value: templates.length, icon: '📝', color: 'bg-blue-500' },
    { label: 'Contexts', value: new Set(templates.map(t => t.context)).size, icon: '🗂️', color: 'bg-green-500' },
    { label: 'Ready to Generate', value: templates.filter(t => t.defaultSampleEntityId).length, icon: '✅', color: 'bg-purple-500' },
    { label: 'Schema Entities', value: 2, icon: '🔍', color: 'bg-orange-500' },
  ];

  return (
    <div className="container mx-auto px-4 py-6">
      <Breadcrumb items={[{ label: 'Dashboard' }]} />
      
      <PageHeader 
        title="Dashboard"
        description="Welcome to ACIG PDF Template System - Runtime-driven PDF generation with dynamic schema discovery"
        actions={
          <>
            <Link 
              href="/templates" 
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
            >
              📝 Manage Templates
            </Link>
            <Link 
              href="/generate" 
              className="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 transition-colors"
            >
              📄 Generate PDF
            </Link>
          </>
        }
      />

      {/* Statistics Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
        {stats.map((stat, index) => (
          <Card key={index} className="hover:shadow-lg transition-shadow">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600 mb-1">{stat.label}</p>
                <p className="text-3xl font-bold text-gray-900">{stat.value}</p>
              </div>
              <div className={`w-12 h-12 ${stat.color} rounded-lg flex items-center justify-center text-2xl`}>
                {stat.icon}
              </div>
            </div>
          </Card>
        ))}
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
        <Card title="Quick Actions">
          <div className="space-y-3">
            <Link 
              href="/generate"
              className="block p-4 bg-gradient-to-r from-blue-50 to-blue-100 rounded-lg hover:from-blue-100 hover:to-blue-200 transition-colors"
            >
              <div className="flex items-center space-x-3">
                <span className="text-2xl">📄</span>
                <div>
                  <h4 className="font-semibold text-gray-900">Generate PDF</h4>
                  <p className="text-sm text-gray-600">Create a PDF from templates</p>
                </div>
              </div>
            </Link>
            <Link 
              href="/templates"
              className="block p-4 bg-gradient-to-r from-green-50 to-green-100 rounded-lg hover:from-green-100 hover:to-green-200 transition-colors"
            >
              <div className="flex items-center space-x-3">
                <span className="text-2xl">📝</span>
                <div>
                  <h4 className="font-semibold text-gray-900">Manage Templates</h4>
                  <p className="text-sm text-gray-600">Create and edit templates</p>
                </div>
              </div>
            </Link>
            <Link 
              href="/schema"
              className="block p-4 bg-gradient-to-r from-purple-50 to-purple-100 rounded-lg hover:from-purple-100 hover:to-purple-200 transition-colors"
            >
              <div className="flex items-center space-x-3">
                <span className="text-2xl">🗂️</span>
                <div>
                  <h4 className="font-semibold text-gray-900">Explore Schema</h4>
                  <p className="text-sm text-gray-600">View database schema and fields</p>
                </div>
              </div>
            </Link>
          </div>
        </Card>

        <Card title="System Information">
          <div className="space-y-4">
            <div>
              <h4 className="text-sm font-medium text-gray-600 mb-2">Features</h4>
              <ul className="space-y-2 text-sm text-gray-700">
                <li className="flex items-center space-x-2">
                  <span className="text-green-500">✓</span>
                  <span>Runtime Schema Discovery</span>
                </li>
                <li className="flex items-center space-x-2">
                  <span className="text-green-500">✓</span>
                  <span>Dynamic Field Trees</span>
                </li>
                <li className="flex items-center space-x-2">
                  <span className="text-green-500">✓</span>
                  <span>Handlebars Templating</span>
                </li>
                <li className="flex items-center space-x-2">
                  <span className="text-green-500">✓</span>
                  <span>PuppeteerSharp PDF Generation</span>
                </li>
                <li className="flex items-center space-x-2">
                  <span className="text-green-500">✓</span>
                  <span>Custom Runtime Joins</span>
                </li>
                <li className="flex items-center space-x-2">
                  <span className="text-green-500">✓</span>
                  <span>RTL Support (Arabic)</span>
                </li>
              </ul>
            </div>
            <div className="pt-4 border-t border-gray-200">
              <p className="text-xs text-gray-500">
                Powered by .NET 10, EF Core, and Next.js
              </p>
            </div>
          </div>
        </Card>
      </div>

      {/* Recent Templates */}
      <Card title="Recent Templates">
        {loading ? (
          <div className="text-center py-8">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
            <p className="mt-4 text-gray-600">Loading templates...</p>
          </div>
        ) : error ? (
          <div className="bg-red-50 border border-red-200 rounded-lg p-4 text-center">
            <p className="text-red-800">{error}</p>
            <button 
              onClick={loadTemplates}
              className="mt-3 px-4 py-2 bg-red-600 text-white rounded-md hover:bg-red-700 transition-colors"
            >
              Retry
            </button>
          </div>
        ) : templates.length === 0 ? (
          <div className="text-center py-8">
            <p className="text-gray-600">No templates found. Create your first template!</p>
            <Link 
              href="/templates"
              className="mt-4 inline-block px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
            >
              Create Template
            </Link>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Name
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Context
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {templates.slice(0, 5).map((template) => (
                  <tr key={template.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm font-medium text-gray-900">{template.name}</div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className="px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full bg-blue-100 text-blue-800">
                        {template.context}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`px-2 py-1 inline-flex text-xs leading-5 font-semibold rounded-full ${
                        template.defaultSampleEntityId 
                          ? 'bg-green-100 text-green-800' 
                          : 'bg-gray-100 text-gray-800'
                      }`}>
                        {template.defaultSampleEntityId ? '✓ Ready' : 'Draft'}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <Link 
                        href={`/templates/${template.id}`}
                        className="text-blue-600 hover:text-blue-900 mr-3"
                      >
                        Edit
                      </Link>
                      {template.defaultSampleEntityId && (
                        <Link 
                          href={`/generate?templateId=${template.id}`}
                          className="text-green-600 hover:text-green-900"
                        >
                          Generate
                        </Link>
                      )}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
        {templates.length > 5 && (
          <div className="mt-4 text-center">
            <Link 
              href="/templates"
              className="text-blue-600 hover:text-blue-900 text-sm font-medium"
            >
              View all templates →
            </Link>
          </div>
        )}
      </Card>
    </div>
  );
}

