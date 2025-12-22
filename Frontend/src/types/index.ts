// This file defines TypeScript interfaces for the main entities used in the application

export interface User {
  id: string;
  email: string;
  name: string;
}

export interface Project {
  id: number;
  title: string;
  description?: string;
  userId: number;
  totalTasks: number;
  completedTasks: number;
  progressPercentage: number;
}

export interface Task {
  id: number;
  title: string;
  description?: string;
  status: 'Pending' | 'InProgress' | 'Completed';
  dueDate: string;
  projectId: number;
}

export interface LoginCredentials {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  email: string;
  userId: number;
  expiresAt: string;
}

export interface CreateProjectData {
  title: string;
  description?: string;
}

export interface UpdateProjectData {
  title: string;
  description?: string;
}

export interface CreateTaskData {
  title: string;
  description?: string;
  dueDate: string;
}

export interface UpdateTaskData {
  title: string;
  description?: string;
  status: 'Pending' | 'InProgress' | 'Completed';
  dueDate: string;
}

