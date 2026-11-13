import { isAxiosError } from 'axios'

import type { TaskStage } from '../types/api'
import { STAGE_MESSAGES } from './constants'

export const getStageUserMessage = (stage: TaskStage, cause: unknown): string => {
    if (isAxiosError(cause)) {
        if (!cause.response) return "Проверьте подключение к интернету"
        if (cause.response.status === 413) return "Файл слишком большой"
    }

    return STAGE_MESSAGES[stage]
}