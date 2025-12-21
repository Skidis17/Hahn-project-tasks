export interface User {
  id: string
  email: string
  name: string
}

export interface Project {
  id: string
  name: string
  description?: string
  userId: string
  createdAt: string
  updatedAt: string
}

export interface Task {
  id: string
  title: string
  description?: string
  status: 'pending' | 'in-progress' | 'completed'
  priority: 'low' | 'medium' | 'high'
  projectId: string
  createdAt: string
  updatedAt: string
}

export interface LoginCredentials {
  email: string
  password: string
}

export interface CreateProjectData {
  name: string
  description?: string
}

export interface CreateTaskData {
  title: string
  description?: string
  status?: 'pending' | 'in-progress' | 'completed'
  priority?: 'low' | 'medium' | 'high'
  projectId: string
}

export interface UpdateTaskData {
  title?: string
  description?: string
  status?: 'pending' | 'in-progress' | 'completed'
  priority?: 'low' | 'medium' | 'high'
}

