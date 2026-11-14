import type { TaskStage } from '../types/api'

export const ROUTES = {
  AUTH: 'auth',
  PROFILE: 'profile',
  TASK: 'task'
}

export const VALID_FILE_TYPES = {
  TXT: 'txt',
  DOCX: 'docx',
  PDF: 'pdf',
  MD: 'md',
}

export const STAGE_MESSAGES: Record<TaskStage, string> = {
  create: "Не удалось создать задачу",
  upload: "Не удалось загрузить файлы",
  questions: "Не удалось отправить вопросы",
  start: "Не удалось запустить обработку",
  poll: "Не удалось получить статус обработки"
}

export const ACCEPT_FILE_TYPES = ".docx,.pdf,.txt,.md"