/* tslint:disable */
/* eslint-disable */
import globalAxios, { AxiosRequestConfig } from 'axios';
import { Configuration } from '../configuration';
import { BASE_PATH, RequestArgs } from '../base';

export class FtpConfigApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getList(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/ftpConfig/list`, {
            params,
            ...this.configuration,
            ...options
        });
    }

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/ftpConfig/page`, {
            params,
            ...this.configuration,
            ...options
        });
    }

    async get(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/ftpConfig/${id}`, {
            ...this.configuration,
            ...options
        });
    }

    async add(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/ftpConfig`, data, {
            ...this.configuration,
            ...options
        });
    }

    async update(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/ftpConfig`, data, {
            ...this.configuration,
            ...options
        });
    }

    async delete(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.delete(`${this.basePath}/api/ftpConfig/${id}`, {
            ...this.configuration,
            ...options
        });
    }
}

export class ListenerConfigApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getList(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/listenerConfig/list`, {
            params,
            ...this.configuration,
            ...options
        });
    }

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/listenerConfig/page`, {
            params,
            ...this.configuration,
            ...options
        });
    }

    async get(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/listenerConfig/${id}`, {
            ...this.configuration,
            ...options
        });
    }

    async add(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/listenerConfig`, data, {
            ...this.configuration,
            ...options
        });
    }

    async update(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/listenerConfig`, data, {
            ...this.configuration,
            ...options
        });
    }

    async delete(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.delete(`${this.basePath}/api/listenerConfig/${id}`, {
            ...this.configuration,
            ...options
        });
    }
}
