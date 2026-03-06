import { getAccessToken } from '@/utils/auth'
import axios from 'axios'
import { getApiUrl } from './base'

// 解析日志 API
export function getParseLogPage(data) {
  return axios.post(getApiUrl('/api/CoalMine/ParseLog/getPage'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

// 解析错误日志 API
export function getParseErrorPage(data) {
  return axios.post(getApiUrl('/api/CoalMine/ParseLog/getErrorPage'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getParseLog(id) {
  return axios.get(getApiUrl(`/api/CoalMine/ParseLog/get?id=${id}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function validateParseLog(id) {
  return axios.get(getApiUrl(`/api/CoalMine/ParseLog/validateFile?id=${id}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function deleteParseLog(id) {
  return axios.delete(getApiUrl(`/api/CoalMine/ParseLog/delete?id=${id}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}
