/* tslint:disable */
/* eslint-disable */
import globalAxios, { AxiosRequestConfig } from 'axios';
import { Configuration } from '../configuration';
import { BASE_PATH } from '../base';

// 解析日志
export class ParseLogApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/parseLog/page`, { params, ...this.configuration, ...options });
    }

    async getList(mineId: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/parseLog/list`, { params: { mineId }, ...this.configuration, ...options });
    }

    async get(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/parseLog/${id}`, { ...this.configuration, ...options });
    }

    async add(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/parseLog`, data, { ...this.configuration, ...options });
    }

    async update(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/parseLog`, data, { ...this.configuration, ...options });
    }

    async delete(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.delete(`${this.basePath}/api/parseLog/${id}`, { ...this.configuration, ...options });
    }
}
