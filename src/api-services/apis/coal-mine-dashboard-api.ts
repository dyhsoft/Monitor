/* tslint:disable */
/* eslint-disable */
import globalAxios, { AxiosRequestConfig } from 'axios';
import { Configuration } from '../configuration';
import { BASE_PATH } from '../base';

// 系统概览
export class DashboardApi {
    private configuration: Configuration;
    private basePath: string;

    constructor(configuration: Configuration, basePath: string = BASE_PATH) {
        this.configuration = configuration;
        this.basePath = basePath;
    }

    async getOverview(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/dashboard/getOverview`, { params, ...this.configuration, ...options });
    }

    async getStationList(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/dashboard/getStationList`, { params, ...this.configuration, ...options });
    }

    async getMineRealTimeOverview(params?: any, options: AxiosRequestConfig = {}): Promise<any> {
        return globalAxios.get(`${this.basePath}/api/dashboard/getMineRealTimeOverview`, { params, ...this.configuration, ...options });
    }
}
