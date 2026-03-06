import { getAccessToken } from '@/utils/auth'
import axios from 'axios'
import { getApiUrl } from './base'

// 水害监测 API
export function getWaterRealtimePage(data) {
  return axios.post(getApiUrl('/api/Water/getRealtimePage'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getWaterRealtime(id) {
  return axios.get(getApiUrl(`/api/Water/getRealtime?id=${id}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getWaterAlarmList(mineId) {
  return axios.get(getApiUrl(`/api/Water/getAlarmList?mineId=${mineId || ''}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}
