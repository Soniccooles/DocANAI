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