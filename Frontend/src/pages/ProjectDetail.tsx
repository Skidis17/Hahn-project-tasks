import { useEffect, useState, useMemo } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Dialog, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select } from '@/components/ui/select'
import { Progress } from '@/components/ui/progress'
import { Badge } from '@/components/ui/badge'
import { projectsAPI, tasksAPI } from '@/services/api'
import type { Project, Task } from '@/types'
import { ArrowLeft, Plus, Edit, Trash2, Search, X } from 'lucide-react'
import { toast } from 'sonner'

const ITEMS_PER_PAGE = 5

export default function ProjectDetail() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const [project, setProject] = useState<Project | null>(null)
  const [tasks, setTasks] = useState<Task[]>([])
  const [loading, setLoading] = useState(true)
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editDialogOpen, setEditDialogOpen] = useState(false)
  const [editingTask, setEditingTask] = useState<Task | null>(null)
  const [searchQuery, setSearchQuery] = useState('')
  const [statusFilter, setStatusFilter] = useState<string>('all')
  const [currentPage, setCurrentPage] = useState(1)

  // Form states
  const [taskTitle, setTaskTitle] = useState('')
  const [taskDescription, setTaskDescription] = useState('')
  const [taskStatus, setTaskStatus] = useState<'Pending' | 'InProgress' | 'Completed'>('Pending')
  const [taskDueDate, setTaskDueDate] = useState('')

  useEffect(() => {
    if (id) {
      loadProject()
      loadTasks()
    }
  }, [id])

  const loadProject = async () => {
    try {
      const data = await projectsAPI.getById(id!)
      setProject(data)
    } catch (error) {
      console.error('Failed to load project:', error)
      toast.error('Failed to load project')
      navigate('/projects')
    }
  }

  const loadTasks = async () => {
    try {
      setLoading(true)
      const data = await tasksAPI.getByProject(id!)
      setTasks(data)
    } catch (error) {
      console.error('Failed to load tasks:', error)
      toast.error('Failed to load tasks')
    } finally {
      setLoading(false)
    }
  }

  const handleCreateTask = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      await tasksAPI.create(id!, {
        title: taskTitle,
        description: taskDescription || undefined,
        dueDate: new Date(taskDueDate).toISOString()
      })
      resetForm()
      setDialogOpen(false)
      toast.success('Task created successfully!')
      loadTasks()
    } catch (error) {
      console.error('Failed to create task:', error)
      toast.error('Failed to create task')
    }
  }

  const handleUpdateTask = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!editingTask) return
    try {
      await tasksAPI.update(id!, editingTask.id.toString(), {
        title: taskTitle,
        description: taskDescription || undefined,
        status: taskStatus,
        dueDate: new Date(taskDueDate).toISOString()
      })
      resetForm()
      setEditDialogOpen(false)
      setEditingTask(null)
      toast.success('Task updated successfully!')
      loadTasks()
    } catch (error) {
      console.error('Failed to update task:', error)
      toast.error('Failed to update task')
    }
  }

  const handleDeleteTask = async (taskId: number) => {
    if (!window.confirm('Are you sure you want to delete this task?')) return
    try {
      await tasksAPI.delete(id!, taskId.toString())
      toast.success('Task deleted successfully!')
      loadTasks()
    } catch (error) {
      console.error('Failed to delete task:', error)
      toast.error('Failed to delete task')
    }
  }

  const handleCompleteTask = async (taskId: number) => {
    try {
      await tasksAPI.complete(id!, taskId.toString())
      toast.success('Task marked as complete!')
      loadTasks()
    } catch (error) {
      console.error('Failed to complete task:', error)
      toast.error('Failed to complete task')
    }
  }

  const resetForm = () => {
    setTaskTitle('')
    setTaskDescription('')
    setTaskStatus('Pending')
    setTaskDueDate('')
  }

  const openEditDialog = (task: Task) => {
    setEditingTask(task)
    setTaskTitle(task.title)
    setTaskDescription(task.description || '')
    setTaskStatus(task.status)
    setTaskDueDate(task.dueDate ? task.dueDate.slice(0, 10) : '')
    setEditDialogOpen(true)
  }

  // Filter and paginate tasks
  const filteredTasks = useMemo(() => {
    return tasks.filter(task => {
      const matchesSearch = task.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
                           task.description?.toLowerCase().includes(searchQuery.toLowerCase())
      const matchesStatus = statusFilter === 'all' || task.status === statusFilter
      return matchesSearch && matchesStatus
    })
  }, [tasks, searchQuery, statusFilter])

  const totalPages = Math.ceil(filteredTasks.length / ITEMS_PER_PAGE)
  const paginatedTasks = useMemo(() => {
    const start = (currentPage - 1) * ITEMS_PER_PAGE
    return filteredTasks.slice(start, start + ITEMS_PER_PAGE)
  }, [filteredTasks, currentPage])

  // Calculate progress
  const completedTasks = tasks.filter(t => t.status === 'Completed').length
  const progress = tasks.length > 0 ? (completedTasks / tasks.length) * 100 : 0

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Completed': return 'default'
      case 'InProgress': return 'secondary'
      case 'Pending': return 'outline'
      default: return 'outline'
    }
  }

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-muted-foreground">Loading...</div>
      </div>
    )
  }

  if (!project) {
    return null
  }

  return (
    <div className="min-h-screen bg-background">
      <header className="border-b">
        <div className="container mx-auto px-4 py-4">
          <div
            className="flex items-center gap-2 cursor-pointer hover:opacity-80 transition-opacity mb-4"
            onClick={() => navigate('/projects')}
          >
            <ArrowLeft className="h-6 w-6 text-primary" />
            <h2 className="text-xl font-bold">Project Manager</h2>
          </div>
          <h1 className="text-3xl font-bold">{project.title}</h1>
          {project.description && (
            <p className="text-muted-foreground mt-2">{project.description}</p>
          )}
        </div>
      </header>

      <main className="container mx-auto px-4 py-8">
        {/* Progress Bar */}
        <Card className="mb-6">
          <CardHeader>
            <CardTitle>Project Progress</CardTitle>
            <CardDescription>
              {completedTasks} of {tasks.length} tasks completed
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Progress value={progress} className="h-3" />
            <p className="text-sm text-muted-foreground mt-2">{Math.round(progress)}% complete</p>
          </CardContent>
        </Card>

        {/* Search and Filter */}
        <div className="flex flex-col sm:flex-row gap-3 mb-6">
          <div className="relative flex-[2]">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Search tasks..."
              value={searchQuery}
              onChange={(e) => {
                setSearchQuery(e.target.value)
                setCurrentPage(1)
              }}
              className="pl-10"
            />
            {searchQuery && (
              <Button
                variant="ghost"
                size="icon"
                className="absolute right-1 top-1/2 transform -translate-y-1/2 h-8 w-8"
                onClick={() => setSearchQuery('')}
              >
                <X className="h-4 w-4" />
              </Button>
            )}
          </div>
          <Select
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value)
              setCurrentPage(1)
            }}
            className="w-full sm:w-40"
          >
            <option value="all">Filter</option>
            <option value="Pending">Pending</option>
            <option value="InProgress">In Progress</option>
            <option value="Completed">Completed</option>
          </Select>

          {/* Create Button */}
          <Button onClick={() => setDialogOpen(true)}>
            <Plus className="h-4 w-4 mr-2" />
            New Task
          </Button>
        </div>

        {/* Tasks List */}
        {paginatedTasks.length === 0 ? (
          <Card>
            <CardContent className="py-12 text-center">
              <p className="text-muted-foreground">
                {searchQuery || statusFilter !== 'all' 
                  ? 'No tasks match your filters' 
                  : 'No tasks yet. Create your first task to get started.'}
              </p>
            </CardContent>
          </Card>
        ) : (
          <>
            <div className="space-y-4 mb-6">
              {paginatedTasks.map((task) => (
                <Card key={task.id}>
                  <CardHeader>
                    <div className="flex items-start justify-between">
                      <div className="flex-1">
                        <CardTitle className="text-lg">{task.title}</CardTitle>
                        {task.description && (
                          <CardDescription className="mt-2">{task.description}</CardDescription>
                        )}
                      </div>
                      <div className="flex gap-2">
                        <Button
                          variant="ghost"
                          size="icon"
                          onClick={() => handleCompleteTask(task.id)}
                          disabled={task.status === 'Completed'}
                          title="Mark complete"
                        >
                          ✓
                        </Button>
                        <Button
                          variant="ghost"
                          size="icon"
                          onClick={() => openEditDialog(task)}
                        >
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button
                          variant="ghost"
                          size="icon"
                          onClick={() => handleDeleteTask(task.id)}
                          className="hover:bg-destructive/10"
                        >
                          <Trash2 className="h-4 w-4 text-destructive" />
                        </Button>
                      </div>
                    </div>
                  </CardHeader>
                  <CardContent>
                    <div className="flex gap-2 flex-wrap">
                      <Badge variant={getStatusColor(task.status)}>
                        {task.status === 'InProgress' ? 'in progress' : task.status.toLowerCase()}
                      </Badge>
                      <Badge variant="secondary">
                        Due: {task.dueDate?.slice(0, 10)}
                      </Badge>
                    </div>
                  </CardContent>
                </Card>
              ))}
            </div>

            {/* Pagination */}
            {totalPages > 1 && (
              <div className="flex items-center justify-center gap-2">
                <Button
                  variant="outline"
                  onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
                  disabled={currentPage === 1}
                >
                  Previous
                </Button>
                <span className="text-sm text-muted-foreground">
                  Page {currentPage} of {totalPages}
                </span>
                <Button
                  variant="outline"
                  onClick={() => setCurrentPage(p => Math.min(totalPages, p + 1))}
                  disabled={currentPage === totalPages}
                >
                  Next
                </Button>
              </div>
            )}
          </>
        )}

        {/* Task (Todo) DIALOG */}
        <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
          <DialogContent>
            <form onSubmit={handleCreateTask}>
              <DialogHeader>
                <DialogTitle>Create New Task</DialogTitle>
                <DialogDescription>
                  Add a new task to this project
                </DialogDescription>
              </DialogHeader>
              <div className="space-y-4 py-4">
                <div className="space-y-2">
                  <Label htmlFor="title">Task Title</Label>
                  <Input
                    id="title"
                    value={taskTitle}
                    onChange={(e) => setTaskTitle(e.target.value)}
                    placeholder="Enter task title"
                    required
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="description">Description (optional)</Label>
                  <Input
                    id="description"
                    value={taskDescription}
                    onChange={(e) => setTaskDescription(e.target.value)}
                    placeholder="Enter task description"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="dueDate">Due Date</Label>
                  <Input
                    id="dueDate"
                    type="date"
                    value={taskDueDate}
                    onChange={(e) => setTaskDueDate(e.target.value)}
                    required
                  />
                </div>
              </div>
              <DialogFooter>
                <Button type="button" variant="outline" onClick={() => setDialogOpen(false)}>
                  Cancel
                </Button>
                <Button type="submit">Create Task</Button>
              </DialogFooter>
            </form>
          </DialogContent>
        </Dialog>

        {/* EDIT TASK DIALOG */}
        <Dialog open={editDialogOpen} onOpenChange={setEditDialogOpen}>
          <DialogContent>
            <form onSubmit={handleUpdateTask}>
              <DialogHeader>
                <DialogTitle>Edit Task</DialogTitle>
                <DialogDescription>
                  Update task details
                </DialogDescription>
              </DialogHeader>
              <div className="space-y-4 py-4">
                <div className="space-y-2">
                  <Label htmlFor="edit-title">Task Title</Label>
                  <Input
                    id="edit-title"
                    value={taskTitle}
                    onChange={(e) => setTaskTitle(e.target.value)}
                    required
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="edit-description">Description</Label>
                  <Input
                    id="edit-description"
                    value={taskDescription}
                    onChange={(e) => setTaskDescription(e.target.value)}
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="edit-dueDate">Due Date</Label>
                  <Input
                    id="edit-dueDate"
                    type="date"
                    value={taskDueDate}
                    onChange={(e) => setTaskDueDate(e.target.value)}
                    required
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="edit-status">Status</Label>
                  <Select
                    id="edit-status"
                    value={taskStatus}
                    onChange={(e) => setTaskStatus(e.target.value as any)}
                  >
                    <option value="Pending">Pending</option>
                    <option value="InProgress">In Progress</option>
                    <option value="Completed">Completed</option>
                  </Select>
                </div>
              </div>
              <DialogFooter>
                <Button type="button" variant="outline" onClick={() => setEditDialogOpen(false)}>
                  Cancel
                </Button>
                <Button type="submit">Update Task</Button>
              </DialogFooter>
            </form>
          </DialogContent>
        </Dialog>
      </main>
    </div>
  )
}