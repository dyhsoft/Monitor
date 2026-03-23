/* tslint:disable */
/* eslint-disable */
import globalAxios, { AxiosRequestConfig } from 'axios';
import { Configuration } from '../configuration';
import { BASE_PATH } from '../base';

// 数据源配置
export class ListenerConfigApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/listenerConfig`, { params, ...this.configuration, ...options });
    }

    async get(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/listenerConfig/${id}`, { ...this.configuration, ...options });
    }

    async add(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/listenerConfig`, data, { ...this.configuration, ...options });
    }

    async update(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/listenerConfig`, data, { ...this.configuration, ...options });
    }

    async delete(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.delete(`${this.basePath}/api/listenerConfig/${id}`, { ...this.configuration, ...options });
    }
}

// FTP配置
export class FtpConfigApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getList(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/ftpConfig/list`, { params, ...this.configuration, ...options });
    }

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/ftpConfig/page`, { params, ...this.configuration, ...options });
    }

    async get(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/ftpConfig/${id}`, { ...this.configuration, ...options });
    }

    async add(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/ftpConfig`, data, { ...this.configuration, ...options });
    }

    async update(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/ftpConfig`, data, { ...this.configuration, ...options });
    }

    async delete(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.delete(`${this.basePath}/api/ftpConfig/${id}`, { ...this.configuration, ...options });
    }

    async setEnabled(id: number, enabled: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/ftpConfig/setEnabled`, { id, enabled }, { ...this.configuration, ...options });
    }
}

// FTP监听
export class FtpWatcherApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async startFtpWatcher(configId: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/ftpWatcher/startFtpWatcher`, { configId }, { ...this.configuration, ...options });
    }

    async startAllFtpWatchers(options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/ftpWatcher/startAllFtpWatchers`, {}, { ...this.configuration, ...options });
    }

    async stopFtpWatcher(configId: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/ftpWatcher/stopFtpWatcher`, { configId }, { ...this.configuration, ...options });
    }

    async stopAllFtpWatchers(options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/ftpWatcher/stopAllFtpWatchers`, {}, { ...this.configuration, ...options });
    }

    async syncOnce(configId: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/ftpWatcher/syncOnce`, { configId }, { ...this.configuration, ...options });
    }

    async testConnection(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/ftpWatcher/testConnection`, data, { ...this.configuration, ...options });
    }

    async getStatus(options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/ftpWatcher/getFtpWatcherStatus`, { ...this.configuration, ...options });
    }
}


