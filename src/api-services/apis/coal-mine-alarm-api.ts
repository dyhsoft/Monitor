/* tslint:disable */
/* eslint-disable */
import globalAxios, { AxiosRequestConfig } from 'axios';
import { Configuration } from '../configuration';
import { BASE_PATH } from '../base';

// 报警配置
export class AlarmConfigApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/alarmConfig/page`, { params, ...this.configuration, ...options });
    }

    async getList(mineId: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/alarmConfig/list`, { params: { mineId }, ...this.configuration, ...options });
    }

    async get(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/alarmConfig/${id}`, { ...this.configuration, ...options });
    }

    async add(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/alarmConfig`, data, { ...this.configuration, ...options });
    }

    async update(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/alarmConfig`, data, { ...this.configuration, ...options });
    }

    async delete(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.delete(`${this.basePath}/api/alarmConfig/${id}`, { ...this.configuration, ...options });
    }

    async setEnabled(id: number, enabled: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/alarmConfig/setEnabled?id=${id}&enabled=${enabled}`, null, { ...this.configuration, ...options });
    }

    async copyToMines(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/alarmConfig/copyToMines`, data, { ...this.configuration, ...options });
    }

    async getPresetAlarmConfigs(options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/alarmConfig/getPresetAlarmConfigs`, { ...this.configuration, ...options });
    }

    async getAlarmCategoryList(options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/alarmConfig/getAlarmCategoryList`, { ...this.configuration, ...options });
    }
}

// 报警记录
export class AlarmRecordApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/alarmRecord/page`, { params, ...this.configuration, ...options });
    }

    async getUnHandleCount(mineId: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/alarmRecord/getUnHandleCount`, { params: { mineId }, ...this.configuration, ...options });
    }

    async confirm(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/alarmRecord/confirm`, data, { ...this.configuration, ...options });
    }

    async resolve(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/alarmRecord/resolve`, data, { ...this.configuration, ...options });
    }

    async get(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/alarmRecord/${id}`, { ...this.configuration, ...options });
    }
}
