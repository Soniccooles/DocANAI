import { VALID_FILE_TYPES, MAX_FILE_SIZE } from "./constants"

export const getFileType = (file: File): string => 
  file.name.split('.').pop()?.toLowerCase() || ''

export const isFileValid = (file: File): boolean => {
  const fileType = getFileType(file)

  const isTypeValid = fileType === VALID_FILE_TYPES.TXT ||
      fileType === VALID_FILE_TYPES.DOCX ||
      fileType === VALID_FILE_TYPES.PDF ||
      fileType === VALID_FILE_TYPES.MD

  return isTypeValid && file.size <= MAX_FILE_SIZE
}
