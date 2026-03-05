import { getAccessToken } from '@/utils/auth'
import axios from 'axios'
import { getApiUrl } from './base'

// 报警配置 API
export function getAlarmConfigPage(data) {
  return axios.post(getApiUrl('/api/AlarmConfig/getPage'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getAlarmConfig(id) {
  return axios.get(getApiUrl(`/api/AlarmConfig/get?id=${id}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function addAlarmConfig(data) {
  return axios.post(getApiUrl('/api/AlarmConfig/add'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function updateAlarmConfig(data) {
  return axios.post(getApiUrl('/api/AlarmConfig/update'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function deleteAlarmConfig(id) {
  return axios.delete(getApiUrl(`/api/AlarmConfig/delete?id=${id}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getSensorTypes() {
  return axios.get(getApiUrl('/api/AlarmConfig/getSensorTypes'), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getAlarmTypes() {
  return axios.get(getApiUrl('/api/AlarmConfig/getAlarmTypes'), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getAlarmLevels() {
  return axios.get(getApiUrl('/api/AlarmConfig/getAlarmLevels'), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

// 报警记录 API
export function getAlarmRecordPage(data) {
  return axios.post(getApiUrl('/api/AlarmRecord/getPage'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getAlarmRecord(id) {
  return axios.get(getApiUrl(`/api/AlarmRecord/get?id=${id}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function confirmAlarm(data) {
  return axios.post(getApiUrl('/api/AlarmRecord/confirm'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function resolveAlarm(data) {
  return axios.post(getApiUrl('/api/AlarmRecord/resolve'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getUnprocessedCount(mineId) {
  return axios.get(getApiUrl(`/api/AlarmRecord/getUnprocessedCount?mineId=${mineId || ''}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getTodayStatistics(mineId) {
  return axios.get(getApiUrl(`/api/AlarmRecord/getTodayStatistics?mineId=${mineId || ''}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}
