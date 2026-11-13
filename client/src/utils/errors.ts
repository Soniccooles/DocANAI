import type { TaskStage } from '../types/api'
import { getStageUserMessage } from './errorMessages'

export class TaskError extends Error {
    userMessage: string

    constructor(message: string, userMessage: string) {
        super(message)
        this.userMessage = userMessage
    }
}

export class TaskFailedError extends TaskError {
    constructor(message: string) {
        super(message, "Ошибка обработки файла")
    }
}

export class TaskTimeoutError extends TaskError {
    constructor(message: string) {
        super(message, "Превышено время ожидания обработки")
    }
}

export class TaskStageError extends TaskError {
    stage: TaskStage
    originalError: unknown

    constructor(stage: TaskStage, originalError: unknown) {
        super(`Task failed at stage "${stage}"`, getStageUserMessage(stage, originalError))
        this.stage = stage
        this.originalError = originalError
    }
}