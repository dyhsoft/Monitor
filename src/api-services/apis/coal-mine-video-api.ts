/* tslint:disable */
/* eslint-disable */
import globalAxios, { AxiosRequestConfig } from 'axios';
import { Configuration } from '../configuration';
import { BASE_PATH } from '../base';

// 视频监控
export class VideoApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/video/page`, { params, ...this.configuration, ...options });
    }

    async get(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/video/${id}`, { ...this.configuration, ...options });
    }

    async add(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/video`, data, { ...this.configuration, ...options });
    }

    async update(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/video`, data, { ...this.configuration, ...options });
    }

    async delete(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.delete(`${this.basePath}/api/video/${id}`, { ...this.configuration, ...options });
    }

    async setEnabled(id: number, enabled: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/video/setEnabled?id=${id}&enabled=${enabled}`, null, { ...this.configuration, ...options });
    }

    async getSelectList(mineId: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/video/getSelectList`, { params: { mineId }, ...this.configuration, ...options });
    }
}
