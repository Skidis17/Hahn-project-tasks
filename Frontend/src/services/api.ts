import axios from 'axios'
import type { 
  User, 
  Project, 
  Task, 
  LoginCredentials, 
  CreateProjectData, 
  CreateTaskData, 
  UpdateTaskData 
} from '@/types'

// Mock data storage (in a real app, this would be API calls)
let mockUsers: User[] = [
  { id: '1', email: 'demo@example.com', name: 'Demo User' }
]

let mockProjects: Project[] = [
  {
    id: '1',
    name: 'Sample Project',
    description: 'A sample project to get started',
    userId: '1',
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString()
  }
]

let mockTasks: Task[] = [
  {
    id: '1',
    title: 'Sample Task',
    description: 'This is a sample task',
    status: 'pending',
    priority: 'medium',
    projectId: '1',
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString()
  }
]

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:3000/api'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Add auth token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Auth API
export const authAPI = {
  login: async (credentials: LoginCredentials): Promise<{ user: User; token: string }> => {
    // Mock login - accept any email/password for demo
    const user = mockUsers[0] || { 
      id: '1', 
      email: credentials.email, 
      name: credentials.email.split('@')[0] 
    }
    const token = 'mock-jwt-token-' + Date.now()
    localStorage.setItem('authToken', token)
    localStorage.setItem('user', JSON.stringify(user))
    return { user, token }
  },
  
  logout: () => {
    localStorage.removeItem('authToken')
    localStorage.removeItem('user')
  },
  
  getCurrentUser: (): User | null => {
    const userStr = localStorage.getItem('user')
    return userStr ? JSON.parse(userStr) : null
  }
}

// Projects API
export const projectsAPI = {
  getAll: async (): Promise<Project[]> => {
    // Simulate API delay
    await new Promise(resolve => setTimeout(resolve, 300))
    const user = authAPI.getCurrentUser()
    if (!user) throw new Error('Not authenticated')
    return mockProjects.filter(p => p.userId === user.id)
  },
  
  getById: async (id: string): Promise<Project> => {
    await new Promise(resolve => setTimeout(resolve, 200))
    const project = mockProjects.find(p => p.id === id)
    if (!project) throw new Error('Project not found')
    return project
  },
  
  create: async (data: CreateProjectData): Promise<Project> => {
    await new Promise(resolve => setTimeout(resolve, 300))
    const user = authAPI.getCurrentUser()
    if (!user) throw new Error('Not authenticated')
    
    const newProject: Project = {
      id: Date.now().toString(),
      ...data,
      userId: user.id,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    }
    mockProjects.push(newProject)
    return newProject
  },
  
  update: async (id: string, data: Partial<Project>): Promise<Project> => {
    await new Promise(resolve => setTimeout(resolve, 300))
    const index = mockProjects.findIndex(p => p.id === id)
    if (index === -1) throw new Error('Project not found')
    mockProjects[index] = { ...mockProjects[index], ...data, updatedAt: new Date().toISOString() }
    return mockProjects[index]
  },
  
  delete: async (id: string): Promise<void> => {
    await new Promise(resolve => setTimeout(resolve, 300))
    mockProjects = mockProjects.filter(p => p.id !== id)
    mockTasks = mockTasks.filter(t => t.projectId !== id)
  }
}

// Tasks API
export const tasksAPI = {
  getByProject: async (projectId: string): Promise<Task[]> => {
    await new Promise(resolve => setTimeout(resolve, 200))
    return mockTasks.filter(t => t.projectId === projectId)
  },
  
  getById: async (id: string): Promise<Task> => {
    await new Promise(resolve => setTimeout(resolve, 200))
    const task = mockTasks.find(t => t.id === id)
    if (!task) throw new Error('Task not found')
    return task
  },
  
  create: async (data: CreateTaskData): Promise<Task> => {
    await new Promise(resolve => setTimeout(resolve, 300))
    const newTask: Task = {
      id: Date.now().toString(),
      title: data.title,
      description: data.description,
      status: data.status || 'pending',
      priority: data.priority || 'medium',
      projectId: data.projectId,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString()
    }
    mockTasks.push(newTask)
    return newTask
  },
  
  update: async (id: string, data: UpdateTaskData): Promise<Task> => {
    await new Promise(resolve => setTimeout(resolve, 300))
    const index = mockTasks.findIndex(t => t.id === id)
    if (index === -1) throw new Error('Task not found')
    mockTasks[index] = { ...mockTasks[index], ...data, updatedAt: new Date().toISOString() }
    return mockTasks[index]
  },
  
  delete: async (id: string): Promise<void> => {
    await new Promise(resolve => setTimeout(resolve, 300))
    mockTasks = mockTasks.filter(t => t.id !== id)
  }
}

export default api

