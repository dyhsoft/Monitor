/* tslint:disable */
/* eslint-disable */
import globalAxios, { AxiosRequestConfig } from 'axios';
import { Configuration } from '../configuration';
import { BASE_PATH, RequestArgs } from '../base';

export class CoalMineApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getList(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/coalMine/list`, {
            params,
            ...this.configuration,
            ...options
        });
    }

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/coalMine/page`, {
            params,
            ...this.configuration,
            ...options
        });
    }

    async autoImportCoalMines(folders?: string[], options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/coalMine/AutoImportCoalMines`, null, {
            params: { folders },
            ...this.configuration,
            ...options
        });
    }

    async get(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/coalMine/${id}`, {
            ...this.configuration,
            ...options
        });
    }

    async add(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/coalMine`, data, {
            ...this.configuration,
            ...options
        });
    }

    async update(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/coalMine`, data, {
            ...this.configuration,
            ...options
        });
    }

    async delete(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.delete(`${this.basePath}/api/coalMine/${id}`, {
            ...this.configuration,
            ...options
        });
    }
}

export class CoalMineAreaApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getList(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/coalMineArea/list`, {
            params,
            ...this.configuration,
            ...options
        });
    }

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/coalMineArea/page`, {
            params,
            ...this.configuration,
            ...options
        });
    }

    async get(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/coalMineArea/${id}`, {
            ...this.configuration,
            ...options
        });
    }

    async add(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/coalMineArea`, data, {
            ...this.configuration,
            ...options
        });
    }

    async update(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/coalMineArea`, data, {
            ...this.configuration,
            ...options
        });
    }

    async delete(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.delete(`${this.basePath}/api/coalMineArea/${id}`, {
            ...this.configuration,
            ...options
        });
    }
}
