import { getAccessToken } from '@/utils/auth'
import axios from 'axios'
import { getApiUrl } from './base'

// 煤矿管理 API
export function getCoalMinePage(data) {
  return axios.post(getApiUrl('/api/CoalMine/getPage'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getCoalMineList() {
  return axios.get(getApiUrl('/api/CoalMine/list'), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getCoalMine(id) {
  return axios.get(getApiUrl(`/api/CoalMine/get?id=${id}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function addCoalMine(data) {
  return axios.post(getApiUrl('/api/CoalMine/add'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function updateCoalMine(data) {
  return axios.post(getApiUrl('/api/CoalMine/update'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function deleteCoalMine(id) {
  return axios.delete(getApiUrl(`/api/CoalMine/delete?id=${id}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

// 网关配置 API
export function getGatewayConfigPage(data) {
  return axios.post(getApiUrl('/api/GatewayConfig/getPage'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function getGatewayConfig(id) {
  return axios.get(getApiUrl(`/api/GatewayConfig/get?id=${id}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function addGatewayConfig(data) {
  return axios.post(getApiUrl('/api/GatewayConfig/add'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function updateGatewayConfig(data) {
  return axios.post(getApiUrl('/api/GatewayConfig/update'), data, {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}

export function deleteGatewayConfig(id) {
  return axios.delete(getApiUrl(`/api/GatewayConfig/delete?id=${id}`), {
    headers: { Authorization: `Bearer ${getAccessToken()}` }
  })
}
