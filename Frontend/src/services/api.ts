import axios from 'axios';

// This file defines the API service for interacting with the backend 

import type { 
  User, 
  Project, 
  Task, 
  LoginCredentials, 
  LoginResponse,
  CreateProjectData, 
  UpdateProjectData,
  CreateTaskData, 
  UpdateTaskData 
} from '@/types';

type ProjectProgressResponse = {
  projectId: number
  projectTitle: string
  totalTasks: number
  completedTasks: number
  progressPercentage: number
}

// API base URL
const API_BASE_URL = 'http://localhost:5000/api';
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  // withCredentials: true
});

// Auth API
export const authAPI = {
  login: async (credentials: LoginCredentials): Promise<{ user: User; token: string }> => {
    const response = await api.post<LoginResponse>('/auth/login', { email: credentials.email, password: credentials.password});
    const {token, email, userId } = response.data;
    const user: User = { id: userId.toString(), email, name: email.split('@')[0]};

    localStorage.setItem('JWT_Token', token);
    localStorage.setItem('user', JSON.stringify(user));
    
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  
    return { user, token };
  },
  
  logout: () => {
    localStorage.removeItem('JWT_Token');
    localStorage.removeItem('user');
    delete api.defaults.headers.common['Authorization'];
  },
  
  getCurrentUser: (): User | null => {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }
};

// handling 401 Unauthorized
api.interceptors.response.use(
  response => response,
  error => {
    if (error.response?.status === 401) {
      authAPI.logout();
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Add request interceptor to add auth token to requests
api.interceptors.request.use(
  config => {
    const token = localStorage.getItem('JWT_Token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => Promise.reject(error)
);

// Projects API
export const projectsAPI = {
  getAll: async (): Promise<Project[]> => {
    const response = await api.get<Project[]>('/projects');
    return response.data;
  },
  
  getById: async (id: string): Promise<Project> => {
    const response = await api.get<Project>(`/projects/${id}`);
    return response.data;
  },
  
  create: async (data: CreateProjectData): Promise<Project> => {
    const response = await api.post<Project>('/projects', data);
    return response.data;
  },
  
  update: async (id: string, data: UpdateProjectData): Promise<Project> => {
    const response = await api.put<Project>(`/projects/${id}`, data);
    return response.data;
  },
  
  delete: async (id: string): Promise<void> => {
    await api.delete(`/projects/${id}`);
  },

  getProgress: async (id: string): Promise<ProjectProgressResponse> => {
    const response = await api.get<ProjectProgressResponse>(`/projects/${id}/progress`)
    return response.data
  }
};

// Tasks API
export const tasksAPI = {
  getByProject: async (projectId: string): Promise<Task[]> => {
    const response = await api.get<Task[]>(`/projects/${projectId}/tasks`);
    return response.data;
  },
  
  getById: async (projectId: string, taskId: string): Promise<Task> => {
    const response = await api.get<Task>(`/projects/${projectId}/tasks/${taskId}`);
    return response.data;
  },
  
  create: async (projectId: string, data: CreateTaskData): Promise<Task> => {
    const response = await api.post<Task>(`/projects/${projectId}/tasks`, data);
    return response.data;
  },
  
  update: async (projectId: string, taskId: string, data: UpdateTaskData): Promise<Task> => {
    const response = await api.put<Task>(`/projects/${projectId}/tasks/${taskId}`, data);
    return response.data;
  },

  complete: async (projectId: string, taskId: string): Promise<Task> => {
    const response = await api.patch<Task>(`/projects/${projectId}/tasks/${taskId}/complete`);
    return response.data;
  },
  
  delete: async (projectId: string, taskId: string): Promise<void> => {
    await api.delete(`/projects/${projectId}/tasks/${taskId}`);
  }
};

export default api
