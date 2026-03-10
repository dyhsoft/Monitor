/* eslint-disable */
import globalAxios, { AxiosRequestConfig } from 'axios';
import { Configuration } from '../configuration';
import { BASE_PATH } from '../base';

export class LocationLeaderConfigApi {
    constructor(private configuration: Configuration, private basePath: string = BASE_PATH) {}

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/locationLeaderConfig/page`, { params, ...this.configuration, ...options });
    }

    async add(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/locationLeaderConfig`, data, { ...this.configuration, ...options });
    }

    async update(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/locationLeaderConfig`, data, { ...this.configuration, ...options });
    }

    async delete(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.delete(`${this.basePath}/api/locationLeaderConfig/${id}`, { ...this.configuration, ...options });
    }
}

export class LocationLimitConfigApi {
    constructor(private configuration: Configuration, private basePath: string = BASE_PATH) {}

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/locationLimitConfig/page`, { params, ...this.configuration, ...options });
    }

    async add(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.post(`${this.basePath}/api/locationLimitConfig`, data, { ...this.configuration, ...options });
    }

    async update(data: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/locationLimitConfig`, data, { ...this.configuration, ...options });
    }

    async delete(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.delete(`${this.basePath}/api/locationLimitConfig/${id}`, { ...this.configuration, ...options });
    }
}

export class LocationAlarmApi {
    constructor(private configuration: Configuration, private basePath: string = BASE_PATH) {}

    async getPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/locationAlarm/page`, { params, ...this.configuration, ...options });
    }

    async getRealTime(mineId: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/locationAlarm/realTime`, { params: { mineId }, ...this.configuration, ...options });
    }

    async handle(id: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.put(`${this.basePath}/api/locationAlarm/handle/${id}`, { ...this.configuration, ...options });
    }
}
