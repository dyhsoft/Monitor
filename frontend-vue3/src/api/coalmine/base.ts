// 获取API基础URL
export function getApiUrl(path: string): string {
  // 从环境变量或配置中获取API地址
  const baseUrl = import.meta.env.VITE_API_URL || '/api'
  return `${baseUrl}${path}`
}
