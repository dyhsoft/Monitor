import { getAccessToken } from '@/utils/auth'
import axios from 'axios'
import { getApiUrl } from './base'

// 定时任务 API
export function getJobList() {
  return axios.get(getApiUrl('/api/SpareTimeJob/getWorkers'), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function startJob(data) {
  return axios.post(getApiUrl('/api/PersonLocationCollectJob/Start'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function stopJob(data) {
  return axios.post(getApiUrl('/api/PersonLocationCollectJob/Stop'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function executeJobOnce(data) {
  return axios.post(getApiUrl('/api/PersonLocationCollectJob/ExecuteOnce'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

// 人员定位采集任务
export function startPersonLocationJob(data) {
  return axios.post(getApiUrl('/api/PersonLocationCollectJob/Start'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function stopPersonLocationJob(data) {
  return axios.post(getApiUrl('/api/PersonLocationCollectJob/Stop'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function executePersonLocationJobOnce() {
  return axios.post(getApiUrl('/api/PersonLocationCollectJob/ExecuteOnce'), {}, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

// 水害监测采集任务
export function startWaterMonitorJob(data) {
  return axios.post(getApiUrl('/api/WaterMonitorCollectJob/Start'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function stopWaterMonitorJob(data) {
  return axios.post(getApiUrl('/api/WaterMonitorCollectJob/Stop'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function executeWaterMonitorJobOnce() {
  return axios.post(getApiUrl('/api/WaterMonitorCollectJob/ExecuteOnce'), {}, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

// 报警检测任务
export function startAlarmCheckJob(data) {
  return axios.post(getApiUrl('/api/AlarmCheckJob/Start'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function stopAlarmCheckJob(data) {
  return axios.post(getApiUrl('/api/AlarmCheckJob/Stop'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function executeAlarmCheckJobOnce() {
  return axios.post(getApiUrl('/api/AlarmCheckJob/ExecuteOnce'), {}, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

// 历史归档任务
export function startHistoryArchiveJob(data) {
  return axios.post(getApiUrl('/api/HistoryArchiveJob/Start'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function stopHistoryArchiveJob(data) {
  return axios.post(getApiUrl('/api/HistoryArchiveJob/Stop'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function executeHistoryArchiveJobOnce() {
  return axios.post(getApiUrl('/api/HistoryArchiveJob/ExecuteOnce'), {}, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}
