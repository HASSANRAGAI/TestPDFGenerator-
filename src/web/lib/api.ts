const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

interface Template {
  id: string;
  name: string;
  context: string;
  htmlContent: string;
  defaultSampleEntityId?: string;
}

interface FieldTreeResponse {
  Context: string;
  RootEntity: string;
  Fields: Field[];
}

interface Field {
  Name: string;
  Type: string;
  Path: string;
  Label: string;
  IsCollection: boolean;
  Fields?: Field[];
}

class ApiClient {
  private baseUrl: string;

  constructor(baseUrl: string = API_BASE_URL) {
    this.baseUrl = baseUrl;
  }

  async get<T>(endpoint: string): Promise<T> {
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`API Error: ${response.statusText}`);
    }

    return response.json();
  }

  async post<T>(endpoint: string, data: any): Promise<T> {
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      throw new Error(`API Error: ${response.statusText}`);
    }

    return response.json();
  }

  async put<T>(endpoint: string, data: any): Promise<T> {
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      throw new Error(`API Error: ${response.statusText}`);
    }

    return response.json();
  }

  async delete(endpoint: string): Promise<void> {
    const response = await fetch(`${this.baseUrl}${endpoint}`, {
      method: 'DELETE',
    });

    if (!response.ok) {
      throw new Error(`API Error: ${response.statusText}`);
    }
  }

  // Template endpoints
  async getTemplates(): Promise<Template[]> {
    return this.get<Template[]>('/Templates');
  }

  async getTemplate(id: string): Promise<Template> {
    return this.get<Template>(`/Templates/${id}`);
  }

  async createTemplate(template: Omit<Template, 'id'>): Promise<Template> {
    return this.post<Template>('/Templates', template);
  }

  async updateTemplate(id: string, template: Template): Promise<Template> {
    return this.put<Template>(`/Templates/${id}`, template);
  }

  async deleteTemplate(id: string): Promise<void> {
    return this.delete(`/Templates/${id}`);
  }

  // Schema endpoints
  async getFieldTree(contextName: string): Promise<FieldTreeResponse> {
    return this.get<FieldTreeResponse>(`/Schema/field-tree/${contextName}`);
  }

  // PDF endpoints
  async generatePdf(contextName: string, entityId: string): Promise<Blob> {
    const response = await fetch(`${this.baseUrl}/Pdf/generate/${contextName}/${entityId}`);
    if (!response.ok) {
      throw new Error(`API Error: ${response.statusText}`);
    }
    return response.blob();
  }

  async previewHtml(contextName: string, entityId: string): Promise<string> {
    const response = await fetch(`${this.baseUrl}/Pdf/preview/${contextName}/${entityId}`);
    if (!response.ok) {
      throw new Error(`API Error: ${response.statusText}`);
    }
    return response.text();
  }
}

export const apiClient = new ApiClient();
export type { Template, FieldTreeResponse, Field };
