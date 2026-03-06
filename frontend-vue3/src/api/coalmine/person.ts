import { getAccessToken } from '@/utils/auth'
import axios from 'axios'
import { getApiUrl } from './base'

// 人员信息 API
export function getPersonInfoPage(data) {
  return axios.post(getApiUrl('/api/CoalMine/PersonInfo/getPage'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getPersonInfo(id) {
  return axios.get(getApiUrl(`/api/CoalMine/PersonInfo/get?id=${id}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function addPersonInfo(data) {
  return axios.post(getApiUrl('/api/CoalMine/PersonInfo/add'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function updatePersonInfo(data) {
  return axios.post(getApiUrl('/api/CoalMine/PersonInfo/update'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function deletePersonInfo(id) {
  return axios.delete(getApiUrl(`/api/CoalMine/PersonInfo/delete?id=${id}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

// 人员出勤 API
export function getPersonAttendancePage(data) {
  return axios.post(getApiUrl('/api/CoalMine/PersonAttendance/getPage'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getPersonAttendance(id) {
  return axios.get(getApiUrl(`/api/CoalMine/PersonAttendance/get?id=${id}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function addPersonAttendance(data) {
  return axios.post(getApiUrl('/api/CoalMine/PersonAttendance/add'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function updatePersonAttendance(data) {
  return axios.post(getApiUrl('/api/CoalMine/PersonAttendance/update'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getAttendanceStatistics(mineId, date) {
  return axios.get(getApiUrl(`/api/CoalMine/PersonAttendance/getStatistics?mineId=${mineId || ''}&date=${date || ''}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}
