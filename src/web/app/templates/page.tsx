'use client';

import { useEffect, useState } from 'react';
import Breadcrumb from '@/components/Breadcrumb';
import PageHeader from '@/components/PageHeader';
import Card from '@/components/Card';
import { apiClient, Template } from '@/lib/api';

export default function TemplatesPage() {
  const [templates, setTemplates] = useState<Template[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedTemplate, setSelectedTemplate] = useState<Template | null>(null);
  const [editMode, setEditMode] = useState(false);

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
      setError('Failed to load templates');
      console.error('Error:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = (template: Template) => {
    setSelectedTemplate(template);
    setEditMode(true);
  };

  const handleSave = async () => {
    if (!selectedTemplate) return;
    
    try {
      await apiClient.updateTemplate(selectedTemplate.id, selectedTemplate);
      await loadTemplates();
      setEditMode(false);
      setSelectedTemplate(null);
    } catch (err) {
      alert('Failed to save template');
      console.error('Error:', err);
    }
  };

  return (
    <div className="container mx-auto px-4 py-6">
      <Breadcrumb items={[{ label: 'Templates' }]} />
      
      <PageHeader 
        title="Template Management"
        description="Create and manage PDF templates with Handlebars syntax"
      />

      {loading ? (
        <Card>
          <div className="text-center py-8">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto"></div>
            <p className="mt-4 text-gray-600">Loading templates...</p>
          </div>
        </Card>
      ) : error ? (
        <Card>
          <div className="bg-red-50 border border-red-200 rounded-lg p-4">
            <p className="text-red-800">{error}</p>
          </div>
        </Card>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Template List */}
          <div className="lg:col-span-1">
            <Card title="Available Templates">
              <div className="space-y-2">
                {templates.map((template) => (
                  <div
                    key={template.id}
                    onClick={() => handleEdit(template)}
                    className={`p-4 rounded-lg cursor-pointer transition-colors ${
                      selectedTemplate?.id === template.id
                        ? 'bg-blue-100 border-2 border-blue-500'
                        : 'bg-gray-50 hover:bg-gray-100 border-2 border-transparent'
                    }`}
                  >
                    <h4 className="font-semibold text-gray-900">{template.name}</h4>
                    <p className="text-sm text-gray-600 mt-1">
                      <span className="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-blue-100 text-blue-800">
                        {template.context}
                      </span>
                    </p>
                  </div>
                ))}
                {templates.length === 0 && (
                  <p className="text-gray-600 text-center py-4">No templates found</p>
                )}
              </div>
            </Card>
          </div>

          {/* Template Editor */}
          <div className="lg:col-span-2">
            {selectedTemplate ? (
              <Card title={`Edit: ${selectedTemplate.name}`}>
                <div className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Template Name
                    </label>
                    <input
                      type="text"
                      value={selectedTemplate.name}
                      onChange={(e) => setSelectedTemplate({ ...selectedTemplate, name: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                      disabled={!editMode}
                    />
                  </div>
                  
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Context
                    </label>
                    <input
                      type="text"
                      value={selectedTemplate.context}
                      onChange={(e) => setSelectedTemplate({ ...selectedTemplate, context: e.target.value })}
                      className="w-full px-4 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
                      disabled={!editMode}
                    />
                  </div>
                  
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      HTML Content (Handlebars)
                    </label>
                    <textarea
                      value={selectedTemplate.htmlContent}
                      onChange={(e) => setSelectedTemplate({ ...selectedTemplate, htmlContent: e.target.value })}
                      rows={15}
                      className="w-full px-4 py-2 border border-gray-300 rounded-md font-mono text-sm focus:ring-blue-500 focus:border-blue-500"
                      disabled={!editMode}
                    />
                  </div>
                  
                  <div className="flex space-x-3">
                    {editMode ? (
                      <>
                        <button
                          onClick={handleSave}
                          className="px-4 py-2 bg-green-600 text-white rounded-md hover:bg-green-700 transition-colors"
                        >
                          💾 Save Changes
                        </button>
                        <button
                          onClick={() => {
                            setEditMode(false);
                            setSelectedTemplate(null);
                          }}
                          className="px-4 py-2 bg-gray-600 text-white rounded-md hover:bg-gray-700 transition-colors"
                        >
                          Cancel
                        </button>
                      </>
                    ) : (
                      <button
                        onClick={() => setEditMode(true)}
                        className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition-colors"
                      >
                        ✏️ Edit Template
                      </button>
                    )}
                  </div>
                </div>
              </Card>
            ) : (
              <Card>
                <div className="text-center py-12">
                  <p className="text-gray-600 text-lg">Select a template to view and edit</p>
                  <p className="text-gray-500 text-sm mt-2">Click on a template from the list on the left</p>
                </div>
              </Card>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
