/* tslint:disable */
/* eslint-disable */
import globalAxios, { AxiosRequestConfig } from 'axios';
import { Configuration } from '../configuration';
import { BASE_PATH } from '../base';

// 安全监测
export class SafetyApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getRealtimePage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/safety/getRealtimePage`, { params, ...this.configuration, ...options });
    }

    async getHistoryPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/safety/getHistoryPage`, { params, ...this.configuration, ...options });
    }

    async getSensorTypes(options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/safety/getSensorTypes`, { ...this.configuration, ...options });
    }

    async getAlarms(options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/safety/getAlarms`, { ...this.configuration, ...options });
    }
}

// 人员定位
export class PersonApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getRealtimePage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/person/getRealtimePage`, { params, ...this.configuration, ...options });
    }

    async getAreaStatistics(mineId: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/person/getAreaStatistics`, { params: { mineId }, ...this.configuration, ...options });
    }

    async getTrackHistory(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/person/getTrackHistory`, { params, ...this.configuration, ...options });
    }

    async getOnlineCount(mineId: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/person/getOnlineCount`, { params: { mineId }, ...this.configuration, ...options });
    }

    async getRecordPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/person/getRecordPage`, { params, ...this.configuration, ...options });
    }

    // 实时数据（用于实时监测页面）
    async getRealTime(mineId: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/person/getRealtimePage`, { params: { mineId, page: 1, pageSize: 1000 }, ...this.configuration, ...options });
    }
}

// 水害监测
export class WaterApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getRealtimePage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/water/getRealtimePage`, { params, ...this.configuration, ...options });
    }

    async getAlarmList(mineId: number, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/water/getAlarmList`, { params: { mineId }, ...this.configuration, ...options });
    }

    async getHistoryPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/water/getHistoryPage`, { params, ...this.configuration, ...options });
    }

    async getSensorTypes(options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/water/getSensorTypes`, { ...this.configuration, ...options });
    }
}

// 矿压监测
export class PressureApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getRealtimePage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/pressure/getRealtimePage`, { params, ...this.configuration, ...options });
    }

    async getHistoryPage(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/pressure/getHistoryPage`, { params, ...this.configuration, ...options });
    }

    async getSensorTypes(options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/pressure/getSensorTypes`, { ...this.configuration, ...options });
    }
}
